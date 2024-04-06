using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    private Animator anim;  // #17 플레이어 물풍선에 갇힐 때 - 애니메이션 설정

    private PlayerCtrl playerCtrl;  // #17 플레이어 물풍선에 갇힐 때 - 이동 속도 느려짐

    public bool trappedInWater = false;    // #17 플레이어 물풍선에 갇혔는지 확인용 bool형 변수
    public bool playerFaint = false;       // #28 플레이어 기절했는지 확인
    private bool playerDie = false;         // #28 플레이어가 완전히 죽었는지 확인 (목숨 모두 소진)

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

    private void PlayerDie()   // 애니메이션 끝 부분에 연결
    {
        // #28 플레이어가 물풍선에 갇힌 시간이 오래되면 - 죽는 애니메이션 재생 & 플레이어 죽음
        PlayerGameMgr.Mgr.life -=1;
        Debug.Log("//#28 플레이어 남은 목숨: " + PlayerGameMgr.Mgr.life);
        
        trappedInWater = false; // #28 물풍선이 터지면서 플레이어가 죽으면, 물풍선에 갇혀 있는지 확인하는 bool형 변수도 false로
        playerFaint = true;     // #28 플레이어 기절 - 움직임 불가능
        PlayerRespawn();    // #29
    }

    private void PlayerRespawn()    // #29 플레이어 부활
    {
        playerFaint = false;    // #28 플레이어 기절 종료 - 움직임 가능
        // #29 플레이어 죽은 후, 부활할 때
        anim.SetBool("canMove", true);  // #29 플레이어 죽고 살아나면 다시 움직이는 애니메이션 정상 작동하도록
        playerCtrl.PlayerSpeedDown(false);  // #29 플레이어 본래 속도로 돌아가기
    }
    
}
