using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour   // #10
{
    public enum ITEM_TYPE {FLUID = 1, BUBBLE, ROLLER, TURTLE, COIN}   // #10 Item마다 TYPE 설정하기   (추가: #13 BUBBLE #15 ROLLER #16 TURTLE)
    public ITEM_TYPE itemType = ITEM_TYPE.FLUID;    // #10 디폴트 = 물병

    private PlayerCtrl playerCtrl;                  // #15
    private Music music;                            // #22
    private MapManager mapMgr;                      // #10 아이템 획득시 - ObstacleArr 배열을 0으로 만들기 위함


    void Awake()
    {
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>(); // #4 
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>(); // #22
        mapMgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>(); // #10
    }
    

    private void OnTriggerEnter(Collider other)     
    {
        Debug.Log("//#10 OnTriggerEnter");   
        if(other.gameObject.tag == "Player")     // #10 플레이어에 닿으면 사라지도록
        {

            // #10 만약 플레이어가 물풍선 안에 갇혀 있다면, 아이템 획득 불가능
            if(other.gameObject.GetComponent<PlayerLife>().trappedInWater == true)  
                return;

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
                    playerCtrl.ChangePlayerSpeed(PlayerGameMgr.Mgr.roller); // #15 플레이어 이동 속도 증가
                    Destroy(this.gameObject);   // 플레이어 획득
                    break;
                
                case ITEM_TYPE.TURTLE :     // #16 TURTLE 아이템
                    PlayerGameMgr.Mgr.turtle = true;
                    PlayerGameMgr.Mgr.turtleNum += 1;   // #16 아이템 획득 수 추가
                    other.gameObject.GetComponent<PlayerCtrl>().TurtleMount(true);  // #35 플레이어가 거북에 올라탐
                    Destroy(this.gameObject);   // 플레이어 획득
                    break;
                
                case ITEM_TYPE.COIN :
                    PlayerGameMgr.Mgr.coin += 1;
                    Destroy(this.gameObject);   // 플레이어 획득
                    break;
            }

            mapMgr.RemoveItemPos(this.transform);   // #10 아이템 획득시, ObstacleArr 배열 값을 0으로 설정
            music.SoundEffect(Music.EFFECT_TYPE.EAT_PROP, 0.6f);  // #22 플레이어 아이템 획득시 효과음
        }  
    
        if(other.gameObject.tag == "WaterBurst")    // #34 물풍선의 물줄기에 닿으면 사라지도록 (단, 두 오브젝트 중 하나는 Rigidbody와 Collider 모두 있어야 함)
        {
            // 물풍선에 Rigidbody가 있긴 한데, "WaterBurst" tag가 달린 오브젝트에는 Rigidbody가 달려있지 않음. 그런데도 Trigger 함수가 작동함.
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        Debug.Log("//#10 OnCollisionEnter");   

    }

    public void DestroyItem()   // #34
    {
        Debug.Log("//#34: " + this.gameObject.name + "아이템 삭제");
        Destroy(this.gameObject);
    }
}
