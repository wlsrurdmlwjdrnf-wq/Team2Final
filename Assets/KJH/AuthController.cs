using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class AuthController : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField nicknameInput;
    public Button signup;
    public Button login;

    private void Start()
    {
        signup.onClick.AddListener(OnClickSignUp);
        login.onClick.AddListener(OnClickLogin);
    }

    public void OnClickSignUp()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string nickname = nicknameInput.text;

        var req = new SignUpRequest(email, password, nickname);


        ServerManager.instance.Post<SignUpRequest, SignUpResponse>(HttpPath.SignUp, req, (connect, res) =>
        {
            if (!connect) { return; } // 서버통신 오류 UI
            if (!res.isSuccess) { Debug.Log(res.msg); } // res.msg 텍스트 기반 UI

            
            //아래부터는 성공 Response 에서 받아온 데이터로 UI 오픈

            // Debug.Log($"{res.nickname}님 회원가입완료");

            ServerManager.instance.UpdateToken(res.token); //인증 토큰발행
        });
    }

    public void OnClickLogin()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        var req = new LoginRequest(email, password);

        ServerManager.instance.Post<LoginRequest, LoginResponse>(HttpPath.Login, req, (connect, res) =>
        {
            if (!connect) { return; } //서버통신 오류 UI
            if (!res.isSuccess) { Debug.Log(res.msg); } // res.msg 텍스트 기반 UI 


            //아래부터는 성공 Response 에서 받아온 데이터로 UI 오픈

            /*
            Debug.Log($"성공여부 : {res.isSuccess}");
            Debug.Log($"환영합니다 {res.nickname}님");
            Debug.Log($"서버 메세지 : {res.msg}");
            Debug.Log($"플레이어 인증토큰 : {res.token}님");
            */

            ServerManager.instance.UpdateToken(res.token); //인증 토큰발행
        });
    }
}
