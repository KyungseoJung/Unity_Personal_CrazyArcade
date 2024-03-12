using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour   // #10
{
    public enum ITEM_TYPE {FLUID = 1, BUBBLE, ROLLER, TURTLE}   // #10 Item마다 TYPE 설정하기   (추가: #13 BUBBLE #15 ROLLER #16 TURTLE)
    public ITEM_TYPE itemType = ITEM_TYPE.FLUID;    // #10 디폴트 = 물병

    private PlayerCtrl playerCtrl;                  // #15
    private Music music;                            // #22

    void Awake()
    {
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>(); // #4 
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>(); // #22
    }
    

    private void OnTriggerEnter(Collider other)     // #10 플레이어에 닿으면 사라지도록 
    {
        Debug.Log("//#10 OnTriggerEnter");   
        if(other.gameObject.tag == "Player")    // 플레이어에 닿으면
        {
            Debug.Log("//#10 플레이어 - " + itemType + " 먹음");

            switch(itemType) 
            {
                case ITEM_TYPE.FLUID : 
                    PlayerGameMgr.Mgr.fluid +=1;  // #10 플레이어가 물병 하나 먹을 때마다, 물줄기 하나씩 증가하도록
                    Destroy(this.gameObject);
                    break;
                
                case ITEM_TYPE.BUBBLE :     // #13 놓을 수 있는 물풍선 개수 늘어나는 아이템
                    PlayerGameMgr.Mgr.waterballoonNum += 1;
                    Destroy(this.gameObject);       
                    break;
                
                case ITEM_TYPE.ROLLER :     // #15 ROLLER 아이템
                    PlayerGameMgr.Mgr.roller += 1;  
                    playerCtrl.PlayerSpeedUp(PlayerGameMgr.Mgr.roller); // #15 플레이어 이동 속도 증가
                    Destroy(this.gameObject);   // 플레이어 획득
                    break;
                
                case ITEM_TYPE.TURTLE :     // #16 TURTLE 아이템
                    PlayerGameMgr.Mgr.turtle = true;
                    Destroy(this.gameObject);   // 플레이어 획득
                    break;
            }

            music.SoundEffect(Music.EFFECT_TYPE.EAT_PROP);  // #22 플레이어 아이템 획득시 효과음
        }  
    }

    private void OnCollisionEnter(Collision other) 
    {
        Debug.Log("//#10 OnCollisionEnter");   

    }
}
