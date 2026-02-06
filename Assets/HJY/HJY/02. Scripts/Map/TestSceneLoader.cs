
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneLoader : MonoBehaviour
{
    public void Start()
    {
        BattleSceneAdditive();
    }

    public void BattleSceneAdditive()
    {
        SceneManager.LoadScene("Battle", LoadSceneMode.Additive);
    }
    
}
