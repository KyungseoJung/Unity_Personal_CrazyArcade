using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    private Animator anim;  // #17 플레이어 물풍선에 갇힐 때 - 애니메이션 설정

    private PlayerCtrl playerCtrl;  // #17 플레이어 물풍선에 갇힐 때 - 이동 속도 느려짐

    private Vector3 respawnPos;     // #29 리스폰 위치 지정
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
        respawnPos = new Vector3(4, -3, -0.05f); // #29 리스폰 위치 지정
        anim.SetBool("canMove", true);  // #17 첫 설정은 true로 해서 애니메이션 정상 작동하도록
        trappedInWater = false;         // #17
    }

    public void PlayerInWaterBalloon() // #17 플레이어가 물풍선에 갇힘
    {
        if(!trappedInWater)
        {
            Invoke("SetStateInWaterBalloon", 0.2f); // #17 fix: 시간 지연을 두고, 플레이어가 물풍선에 닿았을 때 상태 변화를 주기
            // balloonInFront 변수를 true로 만들어주는 CheckIsThereWaterBalloon함수 보다 먼저 실행되어서 생기는 문제 있었음 - 이를 보완하기 위해 Invoke 함수 이용
        }
    }

    private void SetStateInWaterBalloon()
    {
        playerCtrl.balloonInFront = false;  // #33 fix 물풍선에 맞았을 때 - balloonInfront = false 처리해주기.
        // 여기서 이 처리를 안 해주면, 물풍선을 바라보는 상태에서 물풍선을 맞은 후, 계속 가만히 서 있는 경우가 있음
        // 결론적으로 MapManager.cs에서는 balloonInFront = false 처리를 해주지 않으므로, 계속 가만히 서 있는 경우를 방지하기 위해
        // #35 플레이어가 거북에 타고 있었다면 - 거북에서만 내려오게 하고 함수 탈출
        if(playerCtrl.turtleMount) 
        {
            Debug.Log("//#35 플레이어 - 물 맞아서 거북에서 내려오기");
            Debug.Log("//#35 balloonInFront 변수: " + playerCtrl.balloonInFront);
            anim.SetBool("turtleMount", false);

            playerCtrl.ChangePlayerSpeed(PlayerGameMgr.Mgr.roller); // #15 획득한 roller 아이템을 바탕으로 본래 속도로 돌아가기
            return;
        }
        trappedInWater = true;  // #17 중복 실행 방지
        Debug.Log("//#17 플레이어 물풍선에 닿음. 갇힘.");
        anim.SetBool("canMove", false);
        anim.SetTrigger("trappedInWater");
        playerCtrl.SetPlayerSpeed();           // #17 플레이어 속도 느려짐
    }

    private void PlayerDie()   // - PlayerTimeOutTrapped 애니메이션 끝 부분에 연결
    {
        Debug.Log("//#28 PlayerLife.cs - PlayerDie()함수 실행");
        // #28 PlayerTimeOutTrapped 애니메이션과 PlayerRespawn 애니메이션 사이에 Exit Tiem을 최소 1이상으로 설정하기
        // #28 플레이어가 물풍선에 갇힌 시간이 오래되면 - 죽는 애니메이션 재생 & 플레이어 죽음
        PlayerGameMgr.Mgr.life -=1;
        Debug.Log("//#28 플레이어 남은 목숨: " + PlayerGameMgr.Mgr.life);
        
        trappedInWater = false; // #28 물풍선이 터지면서 플레이어가 죽으면, 물풍선에 갇혀 있는지 확인하는 bool형 변수도 false로
        playerFaint = true;     // #28 플레이어 기절 - 움직임 불가능
        // PlayerRespawn();    // #29 - PlayerRespawn 애니메이션 끝날 때 실행되도록
    }

    private void SpecifyLocation()  // #29
    {
        this.gameObject.transform.position = respawnPos; // #29 리스폰 위치 지정
    }
    private void PlayerRespawn()    // #29 플레이어 부활 - PlayerRespawn 애니메이션 끝날 때 실행되도록
    {
        playerFaint = false;    // #28 플레이어 기절 종료 - 움직임 가능
        // #29 플레이어 죽은 후, 부활할 때
        anim.SetBool("canMove", true);  // #29 플레이어 죽고 살아나면 다시 움직이는 애니메이션 정상 작동하도록
        playerCtrl.SetPlayerSpeed(false);  // #29 플레이어 본래 속도로 돌아가기

        SkillReset();           // #29
    }

    private void SkillReset()   // #29 부활 시, 스킬 & 할당량 설정
    {
        // 스킬 리셋
        PlayerGameMgr.Mgr.waterballoonNum = 1;
        PlayerGameMgr.Mgr.fluid = 1;
        PlayerGameMgr.Mgr.roller = 0;
        PlayerGameMgr.Mgr.coin = 0;
    }
    
}
