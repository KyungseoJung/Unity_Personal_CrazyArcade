using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;          // #19 Scene 전환 목적

using UnityEngine.UI;                       // #27 플레이어 목숨 표시

public class LobbyManager : MonoBehaviour
{
    public Text txtPlayerLife;                    // #27 플레이어 목숨 표시

    void Start()
    {
        // StartGame(); // #49 '게임 시작' 버튼 누르면, StartGame() 함수 실행되도록 하기
    }

    public void StartGame() // #19 시작하자마자 화면 전환
    {
        SceneManager.LoadScene("scStage1-3D");
    }

    public void ShowHowToGame() // #49
    {
        Debug.Log("#49 어떻게 게임하는지 보여주기!");
    }


}
