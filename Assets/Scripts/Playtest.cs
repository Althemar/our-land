using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Playtest {
    static string filename = null;
    static DateTime timeStart;

    public static void StartNewPlaytest() {
        filename = @"C:\PlaytestOurLand\Playtest" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".txt";

        string text = "Playtest "+ DateTimeOffset.UtcNow.ToUnixTimeSeconds() + "\r\n";
        System.IO.File.WriteAllText(filename, text);

        timeStart = DateTime.Now;
    }

    public static void Log(string data) {
        if (filename == null)
            return;

        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(filename, true)) {
            file.WriteLine(data);
        }
    }

    public static void TimedLog(string data) {
        if (filename == null)
            return;

        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(filename, true)) {
            string t = "" + DateTime.Now.Subtract(timeStart).ToString(@"dd\.hh\:mm\:ss");
            file.WriteLine(DateTime.Now.Subtract(timeStart).ToString(@"mm\:ss") + " " + data);
        }
    }

    public static void EndPlaytest() {
        Log("End Playtest");
        filename = null;
    }
}
