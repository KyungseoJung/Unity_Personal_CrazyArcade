using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    private Animator anim;  // #17 플레이어 물풍선에 갇힐 때 - 애니메이션 설정

    private PlayerCtrl playerCtrl;  // #17 플레이어 물풍선에 갇힐 때 - 이동 속도 느려짐

    public bool trappedInWater = false;    // #17 플레이어 물풍선에 갇혔는지 확인용 bool형 변수

    void Awake()
    {
        anim = GetComponent<Animator>();    // #17
        playerCtrl = GetComponent<PlayerCtrl>();    // #17
    }

    void Start()
    {
        anim.SetBool("canMove", true);  // #17 첫 설정은 true로 해서 애니메이션 정상 작동하도록
        trappedInWater = false;         // #17
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "WaterWeapon")    // #17 플레이어가 물풍선에 닿으면 플레이어 물풍선에 갇힘
        {
            if(!trappedInWater)
            {
                trappedInWater = true;  // #17 중복 실행 방지
                Debug.Log("//#17 플레이어 물풍선에 닿음. 갇힘.");
                anim.SetBool("canMove", false);
                anim.SetTrigger("trappedInWater");
                playerCtrl.PlayerSpeedDown();           // #17 플레이어 속도 느려짐
            }

        }
    }
    
}
