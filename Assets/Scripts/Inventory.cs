using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySlot
{

}

[Serializable]
public class ResourcesGet : SerializableDictionaryBase<ResourceType, float> { }

public class Inventory : MonoBehaviour
{
    public ResourcesGet resources;
    public CallbackFunction OnInventoryChange;

    public Dictionary<string, ResourceType> items = new Dictionary<string, ResourceType>();

    public void Start() {
        Console.AddCommand("addItem", CmdAdd, "Add item to your inventory", CmdAddAutocomplete);
        
        ResourceType[] resources = Resources.LoadAll<ResourceType>("ResourceItems");
        foreach(ResourceType r in resources) {
            items.Add(r.name.ToLower(), r);
        }
    }

    string CmdAddAutocomplete(string[] arg) {
        if (arg.Length == 3) {
            return arg[0] + " " + arg[1] + " " + AutocompleteItems(arg[2]);
        }
        else {
            return String.Join(" ", arg);
        }
    }

    public string AutocompleteItems(string cmd) {
        // Look for possible tab completions
        List<string> matches = new List<string>();

        foreach (var c in items) {
            var name = c.Key;
            if (!name.StartsWith(cmd, true, null))
                continue;
            matches.Add(name);
        }

        if (matches.Count == 0)
            return cmd;

        // Look for longest common prefix
        int lcp = matches[0].Length;
        for (var i = 0; i < matches.Count - 1; i++) {
            lcp = Mathf.Min(lcp, Console.CommonPrefix(matches[i], matches[i + 1]));
        }
        cmd += matches[0].Substring(cmd.Length, lcp - cmd.Length);
        if (matches.Count > 1) {
            Console.Write("Possible completions:");
            // write list of possible completions
            for (var i = 0; i < matches.Count; i++)
                Console.Write(" " + matches[i]);
        }
        return cmd;
    }

    void CmdAdd(string[] args) {
        if (args.Length == 2) {
            float n = 0;
            if (!float.TryParse(args[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out n)) {
                Console.Write("Error: Invalid amount");
                return;
            }

            if (items.ContainsKey(args[1].ToLower())) {
                AddItem(items[args[1].ToLower()], n);
            }
            else {
                Console.Write("Error: Unknow item");
            }
        }
        else {
            Console.Write("Usage: addItem [n] [item] \nAdd n items to your inventory.");
        }
    }

    public void AddItem(ResourceType type, float amount) {
        if (!resources.ContainsKey(type)) {
            resources.Add(type, 0);
        }
        resources[type] += amount;
        
        OnInventoryChange();
    }

    public float GetResource(ResourceType type) {
        return resources[type];
    }
}
