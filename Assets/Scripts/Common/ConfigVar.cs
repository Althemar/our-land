﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ConfigVarAttribute : Attribute {
    public string Name = null;
    public string DefaultValue = "";
    public ConfigVar.Flags Flags = ConfigVar.Flags.None;
    public string Description = "";
}

public class ConfigVar {
    public static Dictionary<string, ConfigVar> ConfigVars;
    public static Flags DirtyFlags = Flags.None;

    static bool s_Initialized = false;

    public static void Init() {
        if (s_Initialized)
            return;

        ConfigVars = new Dictionary<string, ConfigVar>();
        InjectAttributeConfigVars();
        s_Initialized = true;
    }

    public static void SaveChangedVars(string filename) {
        if ((DirtyFlags & Flags.Save) == Flags.None)
            return;

        Save(filename);
    }

    public static void Save(string filename) {
        using (var st = System.IO.File.CreateText(filename)) {
            foreach (var cvar in ConfigVars.Values) {
                if ((cvar.flags & Flags.Save) == Flags.Save)
                    st.WriteLine("{0} \"{1}\"", cvar.name, cvar.Value);
            }
            DirtyFlags &= ~Flags.Save;
        }
        Debug.Log("saved: " + filename);
    }

    private static Regex validateNameRe = new Regex(@"^[a-z_+-][a-z0-9_+.-]*$");
    public static void RegisterConfigVar(ConfigVar cvar) {
        if (ConfigVars.ContainsKey(cvar.name)) {
            Debug.LogError("Trying to register cvar " + cvar.name + " twice");
            return;
        }
        if (!validateNameRe.IsMatch(cvar.name)) {
            Debug.LogError("Trying to register cvar with invalid name: " + cvar.name);
            return;
        }
        ConfigVars.Add(cvar.name, cvar);
    }

    [Flags]
    public enum Flags {
        None = 0x0,       // None
        Save = 0x1,       // Causes the cvar to be save to settings.cfg
        Cheat = 0x2,      // Consider this a cheat var. Can only be set if cheats enabled
        ServerInfo = 0x4, // These vars are sent to clients when connecting and when changed
        ClientInfo = 0x8, // These vars are sent to server when connecting and when changed
        User = 0x10,      // User created variable
    }

    public ConfigVar(string name, string description, Flags flags = Flags.None) {
        this.name = name;
        this.flags = flags;
        this.description = description;
    }

    public virtual string Value {
        get { return _stringValue; }
        set {
            if (_stringValue == value)
                return;
            DirtyFlags |= flags;
            _stringValue = value;
            if (!int.TryParse(value, out _intValue))
                _intValue = 0;
            if (!float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _floatValue))
                _floatValue = 0;
            changed = true;
        }
    }

    public int IntValue {
        get { return _intValue; }
        set {
            DirtyFlags |= flags;
            _intValue = value;
            _floatValue = (float)value;
            _stringValue = value.ToString();
            changed = true;
        }
    }

    public float FloatValue {
        get { return _floatValue; }
        set {
            DirtyFlags |= flags;
            _floatValue = value;
            _intValue = (int)value;
            _stringValue = value.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            changed = true;
        }
    }

    static void InjectAttributeConfigVars() {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach (var _class in assembly.GetTypes()) {
                if (!_class.IsClass)
                    continue;
                foreach (var field in _class.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)) {
                    if (!field.IsDefined(typeof(ConfigVarAttribute), false))
                        continue;
                    if (!field.IsStatic) {
                        Debug.LogError("Cannot use ConfigVar attribute on non-static fields");
                        continue;
                    }
                    if (field.FieldType != typeof(ConfigVar)) {
                        Debug.LogError("Cannot use ConfigVar attribute on fields not of type ConfigVar");
                        continue;
                    }
                    var attr = field.GetCustomAttributes(typeof(ConfigVarAttribute), false)[0] as ConfigVarAttribute;
                    var name = attr.Name != null ? attr.Name : _class.Name.ToLower() + "." + field.Name.ToLower();
                    var cvar = field.GetValue(null) as ConfigVar;
                    if (cvar != null) {
                        Debug.LogError("ConfigVars (" + name + ") should not be initialized from code; just marked with attribute");
                        continue;
                    }
                    cvar = new ConfigVar(name, attr.Description, attr.Flags);
                    cvar.Value = attr.DefaultValue;
                    RegisterConfigVar(cvar);
                    field.SetValue(null, cvar);
                }
            }
        }

        // Clear dirty flags as default values shouldn't count as dirtying
        DirtyFlags = Flags.None;
    }

    public bool ChangeCheck() {
        if (!changed)
            return false;
        changed = false;
        return true;
    }

    public string name;
    public string description;
    public Flags flags;
    public bool changed;

    string _stringValue;
    float _floatValue;
    int _intValue;
}
