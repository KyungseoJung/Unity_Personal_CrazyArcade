using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "WaterWeapon")    // #17 플레이어가 물풍선에 닿으면 플레이어 물풍선에 갇힘
        {
            Debug.Log("//#17 플레이어 물풍선에 닿음. 갇힘.");
        }
    }
    
}
