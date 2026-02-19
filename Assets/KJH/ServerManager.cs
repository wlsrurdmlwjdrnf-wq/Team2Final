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
    Offline,
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
    private string baseURL = "https://janett-blanketlike-unusefully.ngrok-free.dev/api";
    private string token = ""; // 서버로부터 응답받은 토큰

    private Dictionary<HttpPath, string> pathMap = new Dictionary<HttpPath, string>() // 편의성을 위한 enum키 딕셔너리
    {
        { HttpPath.SignUp, "/auth/signup" },
        { HttpPath.Login, "/auth/login" },
        { HttpPath.Offline, "/offlinereward/offline" }
    };
        

    public void UpdateToken(string tok) // 토큰 PlayerPrefs에 저장
    {
        token = tok;
        PlayerPrefs.SetString("PlayerToken", token);
        PlayerPrefs.Save();
        //Debug.Log("토큰 갱신");
    }

    public void ClearToken() // 토큰 제거
    {
        token = "";
        PlayerPrefs.DeleteKey("PlayerToken");
        PlayerPrefs.Save();
        //Debug.Log("토큰 제거");
    }

    // 서버 리퀘스트용 Post 함수
    // <리퀘스트용 DTO, 리스폰스용 DTO>(api주소 enum, 리퀘스트 바디값(객체), 콜백액션(bool = 서버연결여부, 응답받은 DTO 객체))

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
            www.SetRequestHeader("ngrok-skip-browser-warning", "true");
            //POST
            if (meth == HttpMethod.POST && body != null)
            {
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);                
                www.SetRequestHeader("Content-Type", "application/json");
            }

            //GET
            if (meth == HttpMethod.GET) { }

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

            if (www.result == UnityWebRequest.Result.ConnectionError) // 물리적 통신오류
            {
                Debug.LogError($"서버 통신 오류 : {www.error}");
                callback?.Invoke(false, default);
                yield break;
            }

            var jsonRes = www.downloadHandler.text;
            TResponse resultRes;
            if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (!string.IsNullOrEmpty(jsonRes))
                {
                    try
                    {
                        resultRes = JsonUtility.FromJson<TResponse>(jsonRes);
                        callback?.Invoke(true, resultRes);
                    }
                    catch
                    {
                        Debug.LogError($"파싱 오류");
                        callback?.Invoke(false, default);
                        yield break;
                    }
                    yield break;
                }
            }
            if (string.IsNullOrEmpty(jsonRes))
            {
                Debug.LogError("Response 데이터 오류");
                callback?.Invoke(false, default);
                yield break;
            }
            resultRes = JsonUtility.FromJson<TResponse>(jsonRes);
            callback?.Invoke(true, resultRes);
        }
    }
}
