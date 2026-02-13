
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class AuthManager : MonoBehaviour
{
    [Header("입력")]
    public TMP_InputField emailInput;        // 이메일 입력 칸
    public TMP_InputField passwordInput;     // 비밀번호 입력 칸
    public TMP_InputField nicknameInput;     // 닉네임 입력 칸

    [Header("버튼")]
    public Button signup;                    // 회원가입 버튼
    public Button login;                     // 로그인 버튼

    [Header("연결")]
    [SerializeField] private TitleManager titleManager; // 타이틀 매니저랑 연결할 것!!!

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
            if (!connect)
            {
                titleManager.OnSignupResult(false, "서버와의 연결이 끊어졌습니다.");
                return;
            }
            if (!res.isSuccess)
            {
                titleManager.OnSignupResult(false, "회원가입 실패: " + res.msg);
                return;
            }

            ServerManager.instance.UpdateToken(res.token);
            titleManager.OnSignupResult(true, "회원가입이 완료되셨습니다.");

            // 안내 문구와 병행해서 확인할 것!!!
            Debug.Log($"{res.nickname}님 회원가입완료");
        });
    }

    public void OnClickLogin()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        var req = new LoginRequest(email, password);

        ServerManager.instance.Post<LoginRequest, LoginResponse>(HttpPath.Login, req, (connect, res) =>
        {
            if (!connect)
            {
                titleManager.OnLoginFail("서버와의 연결이 끊어졌습니다.");
                return;
            }
            if (!res.isSuccess)
            {
                titleManager.OnLoginFail("로그인 실패: " + res.msg);
                return;
            }

            ServerManager.instance.UpdateToken(res.token);
            titleManager.OnLoginSuccess();

            // 안내 문구와 병행해서 확인할 것!!!
            Debug.Log($"성공여부 : {res.isSuccess}");
            Debug.Log($"환영합니다 {res.nickname}님");
            Debug.Log($"서버 메세지 : {res.msg}");
            Debug.Log($"플레이어 인증토큰 : {res.token}님");
        });
    }
}