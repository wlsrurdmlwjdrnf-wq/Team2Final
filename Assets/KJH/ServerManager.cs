using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public enum HttpPath
{
    SignUp,
    Login,
    UpgradeItem,
    PickItem,
    GetPlayerInfo,
}

public enum HttpMethod
{
    GET,
    POST,
    PUT,
    DELETE,
}


public class ServerManager : Singleton<ServerManager>
{
    private string baseURL = "http://localhost:5016/api";
    private string token = "";

    private Dictionary<HttpPath, string> pathMap = new Dictionary<HttpPath, string>()
    {
        { HttpPath.SignUp, "/auth/signup" },
        { HttpPath.Login, "/auth/login" },
    };
        

    public void UpdateToken(string tok)
    {
        token = tok;
        PlayerPrefs.SetString("PlayerToken", token);
        PlayerPrefs.Save();
        Debug.Log("토큰 갱신");
    }

    public void ClearToken(string tok)
    {
        token = "";
        PlayerPrefs.DeleteKey("PlayerToken");
        PlayerPrefs.Save();
        Debug.Log("토큰 제거");
    }

    public void Post<TRequest, TResponse>(HttpPath path, TRequest body, Action<bool, TResponse> callback)
    {
        StartCoroutine(RequestCo(HttpMethod.POST, path, body, callback));
    }

    private IEnumerator RequestCo<TRequest, TResponse>(HttpMethod meth, HttpPath path, TRequest body, Action<bool, TResponse> callback)
    {
        string ApiURL = baseURL + pathMap[path];

        string json = JsonUtility.ToJson(body);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        string method = meth.ToString();

        using (UnityWebRequest www = new UnityWebRequest(ApiURL, method)) //메모리 자동해제
        {
            //POST
            if (meth == HttpMethod.POST && body != null)
            {
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);                
                www.SetRequestHeader("Content-Type", "application/json");
            }

            //GET
            if (meth == HttpMethod.GET)
            {

            }

            //PUT
            if (meth == HttpMethod.PUT) { }

            //DELETE
            if (meth == HttpMethod.DELETE) { }

            www.downloadHandler = new DownloadHandlerBuffer();

            if (!string.IsNullOrEmpty(token))
            {
                www.SetRequestHeader("Authorization", "Bearer " + token);
            }

            yield return www.SendWebRequest();            

            if (www.result != UnityWebRequest.Result.Success) // 물리적 통신오류
            {
                Debug.LogError($"서버 통신 오류 : {www.error}");
                callback?.Invoke(false, default);
                yield break;
            }

            var jsonRes = www.downloadHandler.text;
            
            if (string.IsNullOrEmpty(jsonRes))
            {
                Debug.LogError("Response 데이터 오류");
                callback?.Invoke(false, default);
                yield break;
            }

            TResponse result;
            try
            {
                result = JsonUtility.FromJson<TResponse>(jsonRes);
            }
            catch
            {
                Debug.LogError($"파싱 오류");
                callback?.Invoke(false, default);
                yield break;
            }

            callback?.Invoke(true, result);
        }
    }
}
