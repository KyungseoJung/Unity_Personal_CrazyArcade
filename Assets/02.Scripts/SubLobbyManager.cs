using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;                       // #101 플레이어 목숨 표시

public class SubLobbyManager : MonoBehaviour
{
    // #101 여기부터 - UI 이미지  ========================================
    public Text txtPlayerLife;                    // 플레이어 목숨 표시
    public Text txtNumberOfCan;                    // 느린 거북을 빠르게 해주는 can item
    public Text txtNumberOfNeedle;                 // 물풍선에 갇혔을 때, 벗어나게 해주는 needle item
    public Text txtNumberOfShield;                 // 외부 공격으로부터 막아주는 shield item

    void Start()
    {
        txtPlayerLife.text = $"{SubPlayerGameMgr.SubMgr.life}";   // 플레이어의 목숨 확인한 후 UI로 표시

        // #101 여기부터 - UI 이미지  ========================================
        txtNumberOfCan.text = $"{SubPlayerGameMgr.SubMgr.turtleCan}";
        txtNumberOfNeedle.text = $"{SubPlayerGameMgr.SubMgr.needle}";
        txtNumberOfShield.text = $"{SubPlayerGameMgr.SubMgr.shield}";
    }

    public void UpdateNumberOfItems()  //#101 UI에 나타나는 아이템의 개수 업데이트
    {
        txtNumberOfCan.text = $"{SubPlayerGameMgr.SubMgr.turtleCan}";
        txtNumberOfNeedle.text = $"{SubPlayerGameMgr.SubMgr.needle}";
        txtNumberOfShield.text = $"{SubPlayerGameMgr.SubMgr.shield}";   
    }
}
