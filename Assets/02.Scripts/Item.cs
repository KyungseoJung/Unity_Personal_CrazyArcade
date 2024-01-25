using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour   // #10
{
    public enum ITEM_TYPE {FLUID = 1}   // #10 Item마다 TYPE 설정하기
    public ITEM_TYPE itemType = ITEM_TYPE.FLUID;    // #10 디폴트 = 물병

    
    private void OnTriggerEnter(Collider other)     // #10 플레이어에 닿으면 사라지도록 
    {
        Debug.Log("//#10 OnTriggerEnter");   
        if(other.gameObject.tag == "Player")    // 플레이어에 닿으면
        {
            Debug.Log("//#10 플레이어 - 물풍선 먹음");
            PlayerGameMgr.Mgr.fluid +=1;  // #10 플레이어가 물병 하나 먹을 때마다, 물줄기 하나씩 증가하도록
            Destroy(this.gameObject);
        }  
    }

    private void OnCollisionEnter(Collision other) 
    {
        Debug.Log("//#10 OnCollisionEnter");   

    }
}
