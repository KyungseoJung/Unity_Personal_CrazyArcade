using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    private Animator anim;  // #17 플레이어 물풍선에 갇힐 때 - 애니메이션 설정

    void Awake()
    {
        anim = GetComponent<Animator>();    // #17
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "WaterWeapon")    // #17 플레이어가 물풍선에 닿으면 플레이어 물풍선에 갇힘
        {
            Debug.Log("//#17 플레이어 물풍선에 닿음. 갇힘.");
            anim.SetBool("canMove", false);
            anim.SetTrigger("trappedInWater");
        }
    }
    
}
