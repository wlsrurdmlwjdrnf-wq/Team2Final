using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite4Unity3d;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;


public class ImportJson : EditorWindow
{
    private string sheetURL = "https://script.google.com/macros/s/AKfycbxbx3aAGHfHrDj4FRME-VRNAhOOMVELDPLyXQ_HrnR98MExlxmwdvfJBmQ8SXPs_9fIPg/exec";
    private bool isDownloading = false;

    [MenuItem("Tools/데이터 동기화")]
    public static void ShowWindow() => GetWindow<ImportJson>("Data Importer");

    private void OnGUI()
    {
        GUILayout.Label("구글 시트 -> SQLite 동기화", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        sheetURL = EditorGUILayout.TextField("Sheet URL", sheetURL);

        EditorGUILayout.Space();

        if (GUILayout.Button("데이터 동기화", GUILayout.Height(40)))
        {
            DownloadAndSave();
        }

        if (isDownloading)
        {
            EditorGUILayout.HelpBox("데이터 가져오는중..", MessageType.Info);
        }
    }

    private void DownloadAndSave()
    {
        if (isDownloading) return;
        isDownloading = true;

        EditorApplication.CallbackFunction updateAction = null;

        var www = UnityWebRequest.Get(sheetURL);
        var operation = www.SendWebRequest();

        updateAction = () => {
            if (operation.isDone)
            {
                EditorApplication.update -= updateAction;
                if (www.result == UnityWebRequest.Result.Success)
                {
                    string json = www.downloadHandler.text;
                    Debug.Log($"데이터 수신 완료");

                    SaveToLocalDatabase(json);

                    EditorUtility.DisplayDialog("성공", "데이터가 성공적으로 로컬 DB에 반영되었습니다.", "확인");
                }
                else
                {
                    Debug.LogError($"[Error] {www.error}");
                    EditorUtility.DisplayDialog("실패", "데이터 다운로드 중 오류가 발생했습니다.", "확인");
                }

                isDownloading = false;
                www.Dispose();
            }
        };
        EditorApplication.update += updateAction;
    }

    private void SaveToLocalDatabase(string json)
    {
        try
        {
            var root = JObject.Parse(json);
            var dataNode = root["data"] as JObject;
            string version = root["version"]?.ToString();


            string dbPath = Path.Combine(Application.persistentDataPath, "LocalGameData.db");

            string directory = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            using (var db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create))
            {
                db.BeginTransaction();

                SaveTable<WeaponData>(db, dataNode, "Weapon");
                SaveTable<AccessoryData>(db, dataNode, "Accessory");
                SaveTable<ArtifactData>(db, dataNode, "Artifact");
                SaveTable<PlayerInitData>(db, dataNode, "PlayerInit");
                SaveTable<SkillData>(db, dataNode, "Skill");
                SaveTable<StageData>(db, dataNode, "Stage");

                db.Commit();
                DBFromSO();

                PlayerPrefs.SetString("GameDataVersion", version);
                EditorUtility.DisplayDialog("성공", $"버전 {version} 저장 완료!", "확인");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("저장 시도 데이터: " + json);
            Debug.LogError($"데이터 처리 에러: {e.Message}");
        }
    }

    private void SaveTable<T>(SQLiteConnection db, JObject allData , string sheetName) where T : new()
    {
        if (allData.ContainsKey(sheetName))
        {
            db.DropTable<T>(); // 기존 데이터 삭제
            db.CreateTable<T>();
            Debug.Log(allData.ToString());
            // 객체 변환
            string tableJson = JsonConvert.SerializeObject(allData[sheetName]);
            var list = JsonConvert.DeserializeObject<List<T>>(tableJson);

            db.InsertAll(list);
            Debug.Log($"[LocalDB] {sheetName} 테이블 : {list.Count}개 데이터 저장");
        }
        else
        {
            Debug.LogWarning($"{sheetName} 시트가 JSON 데이터에 존재하지 않습니다.");
        }
    }

    private void DBFromSO()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "LocalGameData.db");
        string assetPath = "Assets/JSH/SO/GameDatabase.asset";

        using (var db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadOnly))
        {
            UpdateSO<ItemDatabaseSO, WeaponData>(db, assetPath, "weapons");
            UpdateSO<ItemDatabaseSO, AccessoryData>(db, assetPath, "accessories");
            UpdateSO<ItemDatabaseSO, ArtifactData>(db, assetPath, "artifacts");
            UpdateSO<ItemDatabaseSO, SkillData>(db, assetPath, "skills");
            UpdateSO<ItemDatabaseSO, PlayerInitData>(db, assetPath, "playerInits");
            UpdateSO<ItemDatabaseSO, StageData>(db, assetPath, "stages");
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void UpdateSO<TSO, TData>(SQLiteConnection db, string path, string fieldName) where TSO : ScriptableObject where TData : new()
    {
        TSO so = AssetDatabase.LoadAssetAtPath<TSO>(path);
        if (so == null)
        {
            so = ScriptableObject.CreateInstance<TSO>();
            AssetDatabase.CreateAsset(so, path);
        }

        // DB 테이블을 리스트로 로드
        var list = db.Table<TData>().ToList();
        Debug.Log($"[DB Check] {fieldName} 테이블에서 {list.Count}개 읽어옴");
        // SO 내부의 해당 리스트 필드(FieldName)를 찾아 데이터 주입
        var field = typeof(TSO).GetField(fieldName);
        if (field != null)
        {
            field.SetValue(so, list);
            EditorUtility.SetDirty(so);
        }
        else
        {
            Debug.LogError($"{typeof(TSO).Name} 클래스에 '{fieldName}' 필드가 없습니다!");
        }
    }
}
