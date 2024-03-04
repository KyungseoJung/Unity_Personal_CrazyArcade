using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;          // #19 Scene 전환 목적

public class LobbyManager : MonoBehaviour
{
    void Start()
    {
        StartGame();
    }

    public void StartGame() // #19 시작하자마자 화면 전환
    {
        SceneManager.LoadScene("scStage1-3D");
    }


}
