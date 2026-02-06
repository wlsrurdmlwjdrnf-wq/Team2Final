using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class BaseResponse
{
    public bool isSuccess;
    public string msg;
}

[Serializable]
public class SignUpRequest
{
    public string email;
    public string password;
    public string nickname;

    public SignUpRequest(string e, string p, string n)
    {
        email = e;
        password = p;
        nickname = n;
    }
}

[Serializable]
public class SignUpResponse : BaseResponse
{
    public string nickname;
    public string token;
}

[Serializable]
public class LoginRequest
{
    public string email;  
    public string password;

    public LoginRequest(string e, string p)
    {
        email = e;
        password = p;
    }
}

[Serializable]
public class LoginResponse : BaseResponse
{
    public string nickname;
    public string token;
}