using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public interface IConsoleUI {
    void Init();
    void Shutdown();
    void OutputString(string message);
    bool IsOpen();
    void SetOpen(bool open);
    void ConsoleUpdate();
    void ConsoleLateUpdate();
}

public class ConsoleGUI : MonoBehaviour, IConsoleUI {

    [ConfigVar(Name = "console.alpha", DefaultValue = "0.9", Description = "Alpha of console")]
    static ConfigVar consoleAlpha;

    void Awake() {
        input_field.onEndEdit.AddListener(OnSubmit);
    }

    public void Init() {
        buildIdText.text = "Playtest 22/01" + " (" + Application.unityVersion + ")";
        Console.AddCommand("clear", CmdClear, "Clear the console log");
        SetOpen(false);
    }

    public void CmdClear(string[] arguments) {
        m_Lines = new List<string>();
        text_area.text = "";
        Console.Write("Cleared");
    }

    public void Shutdown() {

    }

    public void OutputString(string s) {
        m_Lines.Add(s);
        var count = Mathf.Min(100, m_Lines.Count);
        var start = m_Lines.Count - count;
        text_area.text = string.Join("\n", m_Lines.GetRange(start, count).ToArray());
    }

    public bool IsOpen() {
        return panel.gameObject.activeSelf;
    }

    public void SetOpen(bool open) {
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Console, open);

        panel.gameObject.SetActive(open);
        if (open) {
            input_field.ActivateInputField();
        }
    }

    public void ConsoleUpdate() {
        if (Input.GetKeyDown(toggle_console_key) || Input.GetKeyDown(KeyCode.Backslash))
            SetOpen(!IsOpen());

        if (!IsOpen())
            return;

        var c = text_area_background.color;
        c.a = Mathf.Clamp01(consoleAlpha.FloatValue);
        text_area_background.color = c;

        // This is to prevent clicks outside input field from removing focus
        input_field.ActivateInputField();

        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (input_field.caretPosition == input_field.text.Length && input_field.text.Length > 0) {
                var res = Console.TabComplete(input_field.text);
                input_field.text = res;
                input_field.caretPosition = res.Length;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            input_field.text = Console.HistoryUp(input_field.text);
            m_WantedCaretPosition = input_field.text.Length;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            input_field.text = Console.HistoryDown();
            input_field.caretPosition = input_field.text.Length;
        }
    }

    public void ConsoleLateUpdate() {
        // This has to happen here because keys like KeyUp will navigate the caret
        // int the UI event handling which runs between Update and LateUpdate
        if (m_WantedCaretPosition > -1) {
            input_field.caretPosition = m_WantedCaretPosition;
            m_WantedCaretPosition = -1;
        }
    }


    void OnSubmit(string value) {
        // Only react to this if enter was actually pressed. Submit can also happen by mouseclicks.
        if (!Input.GetKey(KeyCode.Return) && !Input.GetKey(KeyCode.KeypadEnter))
            return;

        input_field.text = "";
        input_field.ActivateInputField();

        Console.EnqueueCommand(value);
    }

    List<string> m_Lines = new List<string>();
    int m_WantedCaretPosition = -1;

    [SerializeField] Transform panel;
    [SerializeField] InputField input_field;
    [SerializeField] Text text_area;
    [SerializeField] Image text_area_background;
    [SerializeField] KeyCode toggle_console_key;
    [SerializeField] Text buildIdText;

}


public class Console {
    public delegate void MethodDelegate(string[] args);
    public delegate string AutocompleteDelegate(string[] args);

    static IConsoleUI s_ConsoleUI;

    public static void Init(IConsoleUI consoleUI) {
        if (isInit)
            return;

        s_ConsoleUI = consoleUI;
        s_ConsoleUI.Init();

        AddCommand("help", CmdHelp, "Show available commands", CmdHelpAutocomplete);
        AddCommand("vars", CmdVars, "Show available variables");
        AddCommand("wait", CmdWait, "Wait for X frames");
        AddCommand("exec", CmdExec, "Executes commands from file");

        Write("Console ready");

        isInit = true;
    }

    public static void Reset() {
        s_Commands.Clear();

        s_ConsoleUI.Init();

        AddCommand("help", CmdHelp, "Show available commands", CmdHelpAutocomplete);
        AddCommand("vars", CmdVars, "Show available variables");
        AddCommand("wait", CmdWait, "Wait for X frames");
        AddCommand("exec", CmdExec, "Executes commands from file");

        Write("Console ready");
    }

    public static void Shutdown() {
        s_ConsoleUI.Shutdown();
    }

    static void OutputString(string message) {
        if (s_ConsoleUI != null)
            s_ConsoleUI.OutputString(message);
    }

    public static void Write(string msg) {
        OutputString(msg);
    }

    public static void AddCommand(string name, MethodDelegate method, string description, AutocompleteDelegate autocompleteMethod = null, int tag = 0) {
        if (s_Commands.ContainsKey(name.ToLower())) {
            RemoveCommand(name.ToLower());
            //OutputString("Cannot add command " + name + " twice");
            //return;
        }
        s_Commands.Add(name.ToLower(), new ConsoleCommand(name, method, description, tag, autocompleteMethod));
    }

    public static bool RemoveCommand(string name) {
        return s_Commands.Remove(name.ToLower());
    }

    public static void RemoveCommandsWithTag(int tag) {
        var removals = new List<string>();
        foreach (var c in s_Commands) {
            if (c.Value.tag == tag)
                removals.Add(c.Key);
        }
        foreach (var c in removals)
            RemoveCommand(c);
    }

    public static void ProcessCommandLineArguments(string[] arguments) {
        // Process arguments that have '+' prefix as console commands. Ignore all other arguments

        OutputString("ProcessCommandLineArguments: " + string.Join(" ", arguments));

        var commands = new List<string>();

        foreach (var argument in arguments) {
            var newCommandStarting = argument.StartsWith("+") || argument.StartsWith("-");

            // Skip leading arguments before we have seen '-' or '+'
            if (commands.Count == 0 && !newCommandStarting)
                continue;

            if (newCommandStarting)
                commands.Add(argument);
            else
                commands[commands.Count - 1] += " " + argument;
        }

        foreach (var command in commands) {
            if (command.StartsWith("+"))
                EnqueueCommandNoHistory(command.Substring(1));
        }
    }

    public static bool IsOpen() {
        return s_ConsoleUI.IsOpen();
    }

    public static void SetOpen(bool open) {
        s_ConsoleUI.SetOpen(open);
    }

    public static void ConsoleUpdate() {
        s_ConsoleUI.ConsoleUpdate();

        while (s_PendingCommands.Count > 0) {
            if (s_PendingCommandsWaitForFrames > 0) {
                s_PendingCommandsWaitForFrames--;
                break;
            }
            // Remove before executing as we may hit an 'exec' command that wants to insert commands
            var cmd = s_PendingCommands[0];
            s_PendingCommands.RemoveAt(0);
            ExecuteCommand(cmd);
        }
    }

    public static void ConsoleLateUpdate() {
        s_ConsoleUI.ConsoleLateUpdate();
    }

    static void SkipWhite(string input, ref int pos) {
        while (pos < input.Length && " \t".IndexOf(input[pos]) > -1) {
            pos++;
        }
    }

    static string ParseQuoted(string input, ref int pos) {
        pos++;
        int startPos = pos;
        while (pos < input.Length) {
            if (input[pos] == '"' && input[pos - 1] != '\\') {
                pos++;
                return input.Substring(startPos, pos - startPos - 1);
            }
            pos++;
        }
        return input.Substring(startPos);
    }

    static string Parse(string input, ref int pos) {
        int startPos = pos;
        while (pos < input.Length) {
            if (" \t".IndexOf(input[pos]) > -1) {
                return input.Substring(startPos, pos - startPos);
            }
            pos++;
        }
        return input.Substring(startPos);
    }

    static List<string> Tokenize(string input) {
        var pos = 0;
        var res = new List<string>();
        var c = 0;
        while (pos < input.Length && c++ < 10000) {
            SkipWhite(input, ref pos);
            if (pos == input.Length)
                break;

            if (input[pos] == '"' && (pos == 0 || input[pos - 1] != '\\')) {
                res.Add(ParseQuoted(input, ref pos));
            }
            else
                res.Add(Parse(input, ref pos));
        }
        return res;
    }

    public static void ExecuteCommand(string command) {
        var tokens = Tokenize(command);
        if (tokens.Count < 1)
            return;

        OutputString('>' + command);
        var commandName = tokens[0].ToLower();

        ConsoleCommand consoleCommand;
        ConfigVar configVar;

        if (s_Commands.TryGetValue(commandName, out consoleCommand)) {
            var arguments = tokens.GetRange(1, tokens.Count - 1).ToArray();
            consoleCommand.method(arguments);
        }
        else if (ConfigVar.ConfigVars.TryGetValue(commandName, out configVar)) {
            if (tokens.Count == 2) {
                configVar.Value = tokens[1];
            }
            else if (tokens.Count == 1) {
                // Print value
                OutputString(string.Format("{0} = {1}", configVar.name, configVar.Value));
            }
            else {
                OutputString("Too many arguments");
            }
        }
        else {
            OutputString("Unknown command: " + tokens[0]);
        }
    }

    static void CmdHelp(string[] arguments) {
        if (arguments.Length == 0) {
            OutputString("Available commands:");
            foreach (var c in s_Commands)
                OutputString(c.Value.name + ": " + c.Value.description);
        }
        else if (arguments.Length == 1) {
            string command = arguments[0].ToLower();
            if (s_Commands.ContainsKey(command))
                OutputString(command + ": " + s_Commands[command].description);
            else if (ConfigVar.ConfigVars.ContainsKey(command))
                OutputString(command + ": " + ConfigVar.ConfigVars[command].description);
            else
                OutputString(command + " unknow");
        }
        else {
            OutputString("Too many arguments");
        }
    }

    static string CmdHelpAutocomplete(string[] arg) {
        if (arg.Length == 2 && arg[1].Length > 0) {
            return arg[0] + " " + AutocompleteCommandsAndVars(arg[1], false);
        }
        else {
            return String.Join(" ", arg);
        }
    }

    static void CmdVars(string[] arguments) {
        var varNames = new List<string>(ConfigVar.ConfigVars.Keys);
        varNames.Sort();
        foreach (var v in varNames) {
            var cv = ConfigVar.ConfigVars[v];
            OutputString(string.Format("{0} = {1}", cv.name, cv.Value));
        }
    }

    static void CmdWait(string[] arguments) {
        if (arguments.Length == 0) {
            s_PendingCommandsWaitForFrames = 1;
        }
        else if (arguments.Length == 1) {
            int f = 0;
            if (int.TryParse(arguments[0], out f)) {
                s_PendingCommandsWaitForFrames = f;
            }
        }
        else {
            OutputString("Usage: wait [n] \nWait for next n frames. Default is 1\n");
        }
    }

    static void CmdExec(string[] arguments) {
        bool silent = false;
        string filename = "";
        if (arguments.Length == 1) {
            filename = arguments[0];
        }
        else if (arguments.Length == 2 && arguments[0] == "-s") {
            silent = true;
            filename = arguments[1];
        }
        else {
            OutputString("Usage: exec [-s] <filename>");
            return;
        }

        try {
            var lines = System.IO.File.ReadAllLines(filename);
            s_PendingCommands.InsertRange(0, lines);
            if (s_PendingCommands.Count > 128) {
                s_PendingCommands.Clear();
                OutputString("Command overflow. Flushing pending commands!!!");
            }
        }
        catch (Exception e) {
            if (!silent)
                OutputString("Exec failed: " + e.Message);
        }
    }

    public static void EnqueueCommandNoHistory(string command) {
        Debug.Log("cmd: " + command);
        s_PendingCommands.Add(command);
    }

    public static void EnqueueCommand(string command) {
        s_History[s_HistoryNextIndex % k_HistoryCount] = command;
        s_HistoryNextIndex++;
        s_HistoryIndex = s_HistoryNextIndex;

        EnqueueCommandNoHistory(command);
    }


    public static string TabComplete(string prefix) {
        string[] arg = prefix.Split(' ');
        if (arg.Length == 1) {
            return AutocompleteCommandsAndVars(arg[0]);
        }
        else if (s_Commands.ContainsKey(arg[0].ToLower())) {
            if (s_Commands[arg[0].ToLower()].autocompleteMethod != null)
                return s_Commands[arg[0].ToLower()].autocompleteMethod(arg);
            else
                return prefix;
        }
        else {
            return prefix;
        }
    }

    public static string AutocompleteCommandsAndVars(string cmd, bool addSpace = true) {
        // Look for possible tab completions
        List<string> matches = new List<string>();

        foreach (var c in s_Commands) {
            var name = c.Key;
            if (!name.StartsWith(cmd, true, null))
                continue;
            matches.Add(c.Value.name);
        }

        foreach (var v in ConfigVar.ConfigVars) {
            var name = v.Key;
            if (!name.StartsWith(cmd, true, null))
                continue;
            matches.Add(v.Value.name);
        }

        if (matches.Count == 0)
            return cmd;

        // Look for longest common prefix
        int lcp = matches[0].Length;
        for (var i = 0; i < matches.Count - 1; i++) {
            lcp = Mathf.Min(lcp, CommonPrefix(matches[i], matches[i + 1]));
        }
        cmd += matches[0].Substring(cmd.Length, lcp - cmd.Length);
        if (matches.Count > 1) {
            // write list of possible completions
            for (var i = 0; i < matches.Count; i++)
                Console.Write(" " + matches[i]);
        }
        else if (addSpace) {
            cmd += " ";
        }
        return cmd;
    }

    public static string HistoryUp(string current) {
        if (s_HistoryIndex == 0 || s_HistoryNextIndex - s_HistoryIndex >= k_HistoryCount - 1)
            return "";

        if (s_HistoryIndex == s_HistoryNextIndex) {
            s_History[s_HistoryIndex % k_HistoryCount] = current;
        }

        s_HistoryIndex--;

        return s_History[s_HistoryIndex % k_HistoryCount];
    }

    public static string HistoryDown() {
        if (s_HistoryIndex == s_HistoryNextIndex)
            return "";

        s_HistoryIndex++;

        return s_History[s_HistoryIndex % k_HistoryCount];
    }

    // Returns length of largest common prefix of two strings
    public static int CommonPrefix(string a, string b) {
        int minl = Mathf.Min(a.Length, b.Length);
        for (int i = 1; i <= minl; i++) {
            if (!a.StartsWith(b.Substring(0, i), true, null))
                return i - 1;
        }
        return minl;
    }

    class ConsoleCommand {
        public string name;
        public MethodDelegate method;
        public string description;
        public int tag;
        public AutocompleteDelegate autocompleteMethod;

        public ConsoleCommand(string name, MethodDelegate method, string description, int tag, AutocompleteDelegate autocompleteMethod) {
            this.name = name;
            this.method = method;
            this.description = description;
            this.tag = tag;
            this.autocompleteMethod = autocompleteMethod;
        }
    }

    static List<string> s_PendingCommands = new List<string>();
    public static int s_PendingCommandsWaitForFrames = 0;
    static Dictionary<string, ConsoleCommand> s_Commands = new Dictionary<string, ConsoleCommand>();
    const int k_HistoryCount = 50;
    static string[] s_History = new string[k_HistoryCount];
    static int s_HistoryNextIndex = 0;
    static int s_HistoryIndex = 0;
    public static bool isInit = false;
}
