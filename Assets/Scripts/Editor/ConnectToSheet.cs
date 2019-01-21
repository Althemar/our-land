using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectToSheet : EditorWindow {

    private static string sheetId, tableDataId, tableI18nId;
    private static bool initialized;
    private static UnityWebRequest www;
    private static Action<List<List<string>>> ProcessData;

    public delegate void CallbackFunction();

    public static CallbackFunction TextUpdated;

    private static void Initialize () {
        if (!initialized) {
            sheetId = EditorPrefs.GetString ("GoogleSheetID", "");
            tableDataId = EditorPrefs.GetString ("TableDataID", "");
            tableI18nId = EditorPrefs.GetString ("TableI18nID", "");

            initialized = true;
        }
    }

    public static string GoogleSheetID {
        get {
            Initialize ();
            return sheetId;
        }
        set {
            EditorPrefs.SetString ("GoogleSheetID", value);
            sheetId = value;
        }
    }

    public static string TableDataID {
        get {
            Initialize ();
            return tableDataId;
        }
        set {
            EditorPrefs.SetString ("TableDataID", value);
            tableDataId = value;
        }
    }

    public static string TableI18nID {
        get {
            Initialize ();
            return tableI18nId;
        }
        set {
            EditorPrefs.SetString ("TableI18nID", value);
            tableI18nId = value;
        }
    }

    [MenuItem ("Sheets/Fetch Test Data")]
    static void FetchData() {
        ProcessData = Show;
        RequestGoogleSheet(GoogleSheetID, TableDataID);
    }

    [MenuItem ("Sheets/Fetch i18n Text")]
    static void FetchText() {
        ProcessData = UpdateI18N;
        RequestGoogleSheet(GoogleSheetID, TableI18nID);
    }

    [MenuItem ("Sheets/Fetch Test Data", true)]
    [MenuItem ("Sheets/Fetch i18n Text", true)]
    static bool Validate() {
        if(www == null)
            return true;

        if(!www.isDone)
            return false;
        
        return true;
    }

    [MenuItem ("Sheets/Parameters")]
    static void Parameters () {
        EditorWindow.GetWindowWithRect<ConnectToSheet> (new Rect (100, 100, 400, 130), true, "Google Sheet Settings");
    }
    
    public static void RequestGoogleSheet(string docId, string sheetId = null) {
        string url = "https://docs.google.com/spreadsheets/d/" + docId + "/export?format=csv";

        if (!string.IsNullOrEmpty (sheetId))
            url += "&gid=" + sheetId;

        www = UnityWebRequest.Get(url);
        www.SendWebRequest();

        EditorApplication.update += EditorUpdate;
    }

    public static void Show(List<List<string>> data) {
        foreach (List<string> l in data) {
            Debug.Log ("List " + l);
            foreach (string s in l) {
                Debug.Log (s);
            }
        }
    }

    public static void UpdateI18N(List<List<string>> data) {
        I18nData i18nFile = ScriptableObject.CreateInstance<I18nData>();
        i18nFile.name = "I18n Data";
        i18nFile.i18nData = new List<I18nTextSave>();
        foreach(List<string> l in data) {
            i18nFile.i18nData.Add(
                new I18nTextSave(){
                    key = l[0], 
                    fr = l[1], 
                    en = l[2]
                }
            );
        }
        
        if(!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if(!AssetDatabase.IsValidFolder("Assets/Resources/i18n"))
            AssetDatabase.CreateFolder("Assets/Resources", "i18n");
        AssetDatabase.CreateAsset(i18nFile, "Assets/Resources/i18n/Data.asset");

        I18N.LoadText();

        if(TextUpdated != null)
            TextUpdated();
    }

    class Styles {
        public GUIContent sheetID = EditorGUIUtility.TrTextContent ("Google Sheet ID", "The ID of the Google Sheet");
        public GUIContent dataID = EditorGUIUtility.TrTextContent ("Test Data Table ID", "The ID of the test data table");
        public GUIContent i18nID = EditorGUIUtility.TrTextContent ("i18n Table ID", "The ID of the i18n table");
    }
    static Styles ms_Styles;

    void OnGUI () {
        if (ms_Styles == null)
            ms_Styles = new Styles ();

        GUILayout.Space (5);

        EditorGUI.BeginChangeCheck ();
        GoogleSheetID = EditorGUILayout.TextField (ms_Styles.sheetID, GoogleSheetID);

        GUILayout.Space (5);

        TableDataID = EditorGUILayout.TextField (ms_Styles.dataID, TableDataID);
        TableI18nID = EditorGUILayout.TextField (ms_Styles.i18nID, TableI18nID);
    }

    static void EditorUpdate () {
        while (!www.isDone)
            return;

        if (www.isNetworkError || www.isHttpError)
            Debug.LogError(www.error);
        else {
            List<List<string>> res = CSV.ParseCSV (www.downloadHandler.text);

            ProcessData(res);
        }

        EditorApplication.update -= EditorUpdate;
    }
}