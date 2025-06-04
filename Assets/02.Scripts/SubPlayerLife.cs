using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPlayerLife : MonoBehaviour
{
    private Animator anim;  // 플레이어 물풍선에 갇힐 때 - 애니메이션 설정
    [SerializeField] private GameObject playerShield;    // 방패 아이템 사용할 때의 애니메이션
    private SubPlayerCtrl subPlayerCtrl;  // 플레이어 물풍선에 갇힐 때 - 이동 속도 느려짐
    private MapManager mapMgr;      //  배열 확인 후, item들을 랜덤으로 놓기 위함
    private Music music;            // 플레이어 죽을 때, 효과음
    private SubLobbyManager subLobbyMgr;  // 플레이어 죽을 때, 남은 목숨 -1 표시
    private LobbyManager lobbyMgr;  // 플레이어 죽을 때, 남은 목숨 -1 표시
    private Vector3 respawnPos;     // 리스폰 위치 지정

    // 비어있는 공간 찾기
    int mapRow;
    int mapCol;
    int randomNum;
    int mapPlaceNum;
    int mapPlaceRow;
    int mapPlaceCol;
    float mapPlaceX;
    float mapPlaceY;
    
    public bool trappedInWater = false;    // 플레이어 물풍선에 갇혔는지 확인용 bool형 변수
    public bool playerFaint;       // 플레이어 기절했는지 확인
    private bool playerCompletelyDie = false;         // 플레이어가 완전히 죽었는지 확인 (목숨 모두 소진)
    public bool waterApplied = false;      // 이미 물풍선이 적용되었는지 확인용 bool형 변수
    private bool playerInvincible = false;  // 플레이어 무적 상태인지 확인하는 bool형 변수 (방패 아이템 이용시, 3초 동안 무적 상태)

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerShield = transform.GetChild(3).gameObject;
        subPlayerCtrl = GetComponent<SubPlayerCtrl>();
        mapMgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>();
        subLobbyMgr = GameObject.Find("SubLobbyManager").GetComponent<SubLobbyManager>();
        lobbyMgr = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();

        AllSkillReset_Player2();   // #61 아예 게임을 처음 시작할 때
        Debug.Log("//#61 SubPlayerLife.cs의 Awake 실행");
    }

    void Start()
    {
        playerFaint = true; // 처음 시작할 때, 기절한 상태로 시작. 완전히 등장하면, 그때 기절 상태 false로 만들기. 물풍선 놓을 수 없도록 하기 위해
        respawnPos = new Vector3(-4, 3, -0.05f); // 리스폰 위치 지정
        anim.SetBool("canMove", false);     // 처음에는 플레이어가 등장하는 애니메이션 (PlayerSpin)을 위해서 "canMove"를 false로 설정. PlayerSpin 애니메이션이 끝나면 PlayerCanMove함수를 이용해서 "canMove"를 true로 설정.
        trappedInWater = false;
        playerShield.SetActive(false);  // 디폴트 상태에서는 방패 이미지 가리기
    }

    void Update()
    {
        // 방패 아이템 사용 - 외부 공격으로부터 막아주는 shield item
        if(!trappedInWater && Input.GetKeyDown(KeyCode.Alpha3) && (SubPlayerGameMgr.SubMgr.shield>0)) // 물풍선에 갇혀있을 땐 사용하지 못하도록
        {
            SubPlayerGameMgr.SubMgr.shield -= 1;      // 방패 아이템 사용
            subLobbyMgr.UpdateNumberOfItems();     // 아이템 개수 업데이트해서 UI에 표시

            StartCoroutine(PlayerBeInvincible(3.0f));
        }
    }
    private void OnCollisionEnter(Collision other) 
    {
        if((other.gameObject.tag == "Player") && (this.gameObject.layer == 10))
        {
            // Debug.Log("//#44 플레이어2에 플레이어1이 닿음");    // 물풍선에 갇힌 플레이어 2에 플레이어1이 닿은 것임

            anim.Play("SubPlayerTimeOutTrapped");   // 무조건 SubPlayerTimeOutTrapped 애니메이션 실행하도록
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.tag == "WaterBurst"))
        {
            PlayerInWaterBalloon(); // 플레이어 물풍선에 닿음
        }
    }
    public void PlayerInWaterBalloon() // 플레이어가 물풍선에 닿음
    {
        if(!trappedInWater && !waterApplied && !playerInvincible)   // 플레이어가 무적 상태이면 물풍선 맞아도 아무 영향 받지 않도록
        {
            if(!subPlayerCtrl.CheckPlayerVisible())    // 만약 플레이어가 Bush 안에 가려져 있었다면, 다시 보이게 하기
            {
                subPlayerCtrl.MakePlayerVisible();
            }
            
            SetStateInWaterBalloon();   // balloonInFront 변수 사용을 하지 않고, SphereCollider를 통해 이동 제한을 함 -> 함수를 바로 실행해도 됨
            // balloonInFront 변수를 true로 만들어주는 CheckIsThereWaterBalloon함수 보다 먼저 실행되어서 생기는 문제 있었음 - 이를 보완하기 위해 Invoke 함수 이용
            
            // 물풍선에 맞고나서 0.3초 후에 물풍선 적용 확인 변수를 false로 - 함수가 여러번 실행되는 것을 방지하기 위함 (거북에 타있는데도 물풍선에 갇히는 상황 방지)
            waterApplied = true;
            Invoke("CancleWaterApplied", 0.3f);
        }
    }

    private void SetStateInWaterBalloon()
    {
        // SphereCollider로 통제해서 balloonInFront 변수 필요 없음
        // playerCtrl.balloonInFront = false;  // 물풍선에 맞았을 때 - balloonInfront = false 처리해주기.
        // 여기서 이 처리를 안 해주면, 물풍선을 바라보는 상태에서 물풍선을 맞은 후, 계속 가만히 서 있는 경우가 있음
        // 결론적으로 MapManager.cs에서는 balloonInFront = false 처리를 해주지 않으므로, 계속 가만히 서 있는 경우를 방지하기 위해
        // 플레이어가 거북에 타고 있었다면 - 거북에서만 내려오게 하고 함수 탈출
        // if(playerCtrl.turtleMount) 
        if((SubPlayerGameMgr.SubMgr.slowTurtle) || (SubPlayerGameMgr.SubMgr.fastTurtle))
        {
            // playerCtrl.turtleMount = false;
            SubPlayerGameMgr.SubMgr.slowTurtle = false;   // PlayerGameMgr에사도 플레이어가 거북에 타있는 것을 의미하는 변수들은 모두 false로 만들어주기.
            SubPlayerGameMgr.SubMgr.fastTurtle = false;
            // 느린 거북 or 빠른 거북 모두 내려오도록
            anim.SetBool("turtleMount", false);
            anim.SetBool("fastTurtleMount", false);

            subPlayerCtrl.ChangePlayerSpeed(SubPlayerGameMgr.SubMgr.roller); // 획득한 roller 아이템을 바탕으로 본래 속도로 돌아가기
            return;
        }
        trappedInWater = true;  // 중복 실행 방지
        // 플레이어가 물풍선에 갇혀 있는 효과음 시작
        music.SubPlayerSoundEffect(Music.EFFECT_TYPE.PLYAER_IN_BALLOON, 0.6f, true);

        this.gameObject.layer = 10;    // #44 플레이어가 물풍선에 갇히면, 두 플레이어끼리 충돌이 가능하도록 하기 위해 Layer 변경 ("PlayerInWater")

        anim.SetBool("canMove", false);
        anim.SetTrigger("trappedInWater");
        subPlayerCtrl.SetPlayerSpeed();           // 플레이어 속도 느려짐
    }

    private void PlayerDie()   // PlayerTimeOutTrapped 애니메이션 끝 부분에 연결
    {
        this.gameObject.layer = 9;    // #44 플레이어가 물풍선에 벗어나 죽으면, 다시 두 플레이어끼리 충돌이 불가능하도록 설정

        music.SubPlayerSoundEffect(Music.EFFECT_TYPE.PLAYER_DIE, 0.6f);  // 플레이어 죽을 때 효과음

        if(SubPlayerGameMgr.SubMgr.life <=0 )
        {
            playerCompletelyDie = true;   // 플레이어 완전히 죽음
            // Debug.Log("//#45 플레이어2의 playerCompletelyDie: " + playerCompletelyDie);
            lobbyMgr.Player1Win();  // #45 플레이어1 우승
            mapMgr.Player1Faint();  // #45 플레이어2 이동하지 못하도록 기절시키기
            return;
        }

        // PlayerTimeOutTrapped 애니메이션과 PlayerRespawn 애니메이션 사이에 Exit Tiem을 최소 1이상으로 설정하기
        // 플레이어가 물풍선에 갇힌 시간이 오래되면 - 죽는 애니메이션 재생 & 플레이어 죽음
        SubPlayerGameMgr.SubMgr.life -=1;
        // Debug.Log("//#100 #44 플레이어2 목숨 -1");
        subLobbyMgr.txtPlayerLife.text = $"{SubPlayerGameMgr.SubMgr.life}";
        
        trappedInWater = false; // 물풍선이 터지면서 플레이어가 죽으면, 물풍선에 갇혀 있는지 확인하는 bool형 변수도 false로
        playerFaint = true;     // 플레이어 기절 - 움직임 불가능

        ReturnSkillToMap();     // 플레이어가 획득했던 아이템 모두 뱉기
    }

    private void SpecifyLocation()  // PlayerRespawn 애니메이션 시작될 때 실행되도록
    {
        this.gameObject.transform.position = respawnPos; // 리스폰 위치 지정

        // 플레이어 부활 효과음 - PlayerRespawn 애니메이션 시작될 때 실행되도록
        music.SubPlayerSoundEffect(Music.EFFECT_TYPE.PLAYER_REVIVAL, 0.6f);

        // Debug.Log("#100 플레이어 부활 | SpecifyLocation 함수 실행됨");
    }
    private void PlayerRespawn()    // 플레이어 부활 - PlayerRespawn 애니메이션 끝날 때 실행되도록
    {
        playerFaint = false;    // 플레이어 기절 종료 - 움직임 가능
        trappedInWater = false; 
        waterApplied = false;
        // 플레이어 죽은 후, 부활할 때
        anim.SetBool("canMove", true);  // 플레이어 죽고 살아나면 다시 움직이는 애니메이션 정상 작동하도록
        subPlayerCtrl.SetPlayerSpeed(false);  // 플레이어 본래 속도로 돌아가기

        SkillReset();
    }

    private void SkillReset()   // 부활 시, 스킬 & 할당량 설정
    {
        // 스킬 리셋
        SubPlayerGameMgr.SubMgr.waterballoonNum = 1;
        SubPlayerGameMgr.SubMgr.fluid = 1;
        SubPlayerGameMgr.SubMgr.roller = 0;
        SubPlayerGameMgr.SubMgr.turtleNum = 0;
        SubPlayerGameMgr.SubMgr.coin = 0;
    }
    private void AllSkillReset_Player2()
    {
        //#61 플레이어 목숨부터 기본 스킬, 획득한 스킬 모두 초기화
        SubPlayerGameMgr.SubMgr.life = 3;  // #61 아예 게임을 처음 시작할 때
    
        SubPlayerGameMgr.SubMgr.waterballoonNum = 1;
        SubPlayerGameMgr.SubMgr.fluid = 1;
        SubPlayerGameMgr.SubMgr.roller = 0;
        SubPlayerGameMgr.SubMgr.turtleNum = 0;
        SubPlayerGameMgr.SubMgr.coin = 0;
        
        SubPlayerGameMgr.SubMgr.turtleCan = 1;
        SubPlayerGameMgr.SubMgr.needle = 1;
        SubPlayerGameMgr.SubMgr.shield = 1;

        subLobbyMgr.UpdateAllSkill_Player2();
    }
    private void ReturnSkillToMap() // 플레이어 죽을 때마다 획득한 아이템을 모두 Map에 뱉도록 - 랜덤 위치
    {
        // 플레이어가 갖고 있는 아이템 종류 & 수 확인

        Vector3 placePos;

        int fluidNum = SubPlayerGameMgr.SubMgr.fluid;
        int waterballoonNum = SubPlayerGameMgr.SubMgr.waterballoonNum;
        int rollerNum = SubPlayerGameMgr.SubMgr.roller;
        int turtleNum = SubPlayerGameMgr.SubMgr.turtleNum;
        int coinNum = SubPlayerGameMgr.SubMgr.coin;

        for(int i=1; i<fluidNum; i++)
        {
            mapPlaceNum = FindEmptyPlace();   // Map에서 비어있는 공간 찾기
            mapPlaceRow = mapPlaceNum/9;    // mapPlaceNum을 9로 나누었을 때의 몫
            mapPlaceCol = mapPlaceNum%9;    // mapPlaceNum을 9로 나누었을 때의 나머지0 

            mapPlaceX = mapMgr.ConvertColToXCoordinate(mapPlaceCol);
            mapPlaceY = mapMgr.ConvertRowToYCoordinate(mapPlaceRow);
            
            placePos = new Vector3(mapPlaceX, mapPlaceY, 0);
            mapMgr.PlaceItemPrefab(Item.ITEM_TYPE.FLUID, placePos, mapPlaceRow, mapPlaceCol);
        }

        for(int i=1; i<waterballoonNum; i++)
        {
            mapPlaceNum = FindEmptyPlace();   // Map에서 비어있는 공간 찾기
            mapPlaceRow = mapPlaceNum/9;    // mapPlaceNum을 9로 나누었을 때의 몫
            mapPlaceCol = mapPlaceNum%9;    // mapPlaceNum을 9로 나누었을 때의 나머지0 

            mapPlaceX = mapMgr.ConvertColToXCoordinate(mapPlaceCol);
            mapPlaceY = mapMgr.ConvertRowToYCoordinate(mapPlaceRow);

            
            placePos = new Vector3(mapPlaceX, mapPlaceY, 0);
            mapMgr.PlaceItemPrefab(Item.ITEM_TYPE.BUBBLE, placePos, mapPlaceRow, mapPlaceCol);
        }

        for(int i=0; i<rollerNum; i++)
        {
            mapPlaceNum = FindEmptyPlace();   // Map에서 비어있는 공간 찾기
            mapPlaceRow = mapPlaceNum/9;    // mapPlaceNum을 9로 나누었을 때의 몫
            mapPlaceCol = mapPlaceNum%9;    // mapPlaceNum을 9로 나누었을 때의 나머지0 

            mapPlaceX = mapMgr.ConvertColToXCoordinate(mapPlaceCol);
            mapPlaceY = mapMgr.ConvertRowToYCoordinate(mapPlaceRow);

            
            placePos = new Vector3(mapPlaceX, mapPlaceY, 0);
            mapMgr.PlaceItemPrefab(Item.ITEM_TYPE.ROLLER, placePos, mapPlaceRow, mapPlaceCol);
        }

        for(int i=0; i<turtleNum; i++)
        {
            mapPlaceNum = FindEmptyPlace();   // Map에서 비어있는 공간 찾기
            mapPlaceRow = mapPlaceNum/9;    // mapPlaceNum을 9로 나누었을 때의 몫
            mapPlaceCol = mapPlaceNum%9;    // mapPlaceNum을 9로 나누었을 때의 나머지0 

            mapPlaceX = mapMgr.ConvertColToXCoordinate(mapPlaceCol);
            mapPlaceY = mapMgr.ConvertRowToYCoordinate(mapPlaceRow);

            
            placePos = new Vector3(mapPlaceX, mapPlaceY, 0);
            mapMgr.PlaceItemPrefab(Item.ITEM_TYPE.TURTLE, placePos, mapPlaceRow, mapPlaceCol);
        }

        for(int i=0; i<coinNum; i++)
        {
            mapPlaceNum = FindEmptyPlace();   // Map에서 비어있는 공간 찾기
            mapPlaceRow = mapPlaceNum/9;    // mapPlaceNum을 9로 나누었을 때의 몫
            mapPlaceCol = mapPlaceNum%9;    // mapPlaceNum을 9로 나누었을 때의 나머지0 

            mapPlaceX = mapMgr.ConvertColToXCoordinate(mapPlaceCol);
            mapPlaceY = mapMgr.ConvertRowToYCoordinate(mapPlaceRow);

            
            placePos = new Vector3(mapPlaceX, mapPlaceY, 0);
            mapMgr.PlaceItemPrefab(Item.ITEM_TYPE.COIN, placePos, mapPlaceRow, mapPlaceCol);   
        }
    }

    private int FindEmptyPlace()
    {
        // 비어있는 공간 찾기
        // int mapRow;
        // int mapCol;
        // int randomNum;

        randomNum= Random.Range(0,63);  // MapManager.cs 에서의 배열이 7행9열로 -> 63개의 배열값들이 존재하기 때문 -> 0부터 62까지 63개 배열
        // 예를 들어, Range(0,10) 이면 0부터 9까지의 숫자 중 랜덤으로 선택됨

        mapRow = randomNum/9;   // randomNum을 9로 나누었을 때의 몫이 mapRow
        mapCol = randomNum%9;   // randomNum을 9로 나누었을 때의 나머지가 mapCol


        while((mapMgr.obstacleArr[mapRow, mapCol] == 1) || (mapMgr.itemArr[mapRow, mapCol] == 1) || (mapMgr.waterBalloonArr[mapRow, mapCol] == 1)
                || (mapMgr.bushArr[mapRow, mapCol] == 1) || (mapMgr.blockArr[mapRow, mapCol] ==1))   // 장애물 또는 아이템이 하나라도 겹쳐 있다면, 다시 좌표 찾기
        {

            // waterBalloonArr, obstacleArr, itemArr 배열에 이미 놓인 것이 있다면, 다시 숫자 설정
            // + (2025.06.04 blockArr도 검사하기)
            randomNum= Random.Range(0,63);

            mapRow = randomNum/9;   // randomNum을 9로 나누었을 때의 몫이 mapRow
            mapCol = randomNum%9;   // randomNum을 9로 나누었을 때의 나머지가 mapCol
        }

        return randomNum;
        
    }
    private void PlayerCanMove()    // PlayerSpin 애니메이션이 끝나면 PlayerCanMove함수를 이용해서 "canMove"를 true로 설정.
    {
        // Debug.Log("//#103 (플레이어 등장) PlayerCanMove 함수 실행");
        anim.SetBool("canMove", true);
        
        playerFaint = false;    // 처음 시작할 때, 기절한 상태로 시작. 완전히 등장하면, 그때 기절 상태 false로 만들기. 물풍선 놓을 수 없도록 하기 위해
    }
    private void PlayerCanMoveLookingAhead()    // PlayerEscapeWater 애니메이션 종료되는 시점에 실행 - 물풍선 탈출 애니메이션 후에, 원래 애니메이션이 정상적으로 작동하도록
    {

        // 자연스럽게 물풍선 탈출하는 애니메이션까지 완벽히 실행한 후, LookingAhead 애니메이션이 실행되도록
        anim.SetBool("canMove", true);
        anim.SetTrigger("LookingAhead");
    }
    
    private void CancleWaterApplied()   // 물풍선에 맞고나서 0.3초 후에 물풍선 적용 확인 변수를 false로
    {
        waterApplied = false; // 변수를 false로 설정
    }

    private void CheckAnyLivesLeft()    // (SubPlayerTimeOutTrapped.anim 에서 실행) 플레이어 목숨 남았는지 확인한 후, 부활 시도
    {
        // Debug.Log("//#45 CheckAnyLivesLeft 함수 실행");
        if(playerCompletelyDie)   // 만약 PlayerDie 함수에서 플레이어가 완전히 죽은 것이 확인된다면, 플레이어를 아예 비활성화 하기
        {
            this.gameObject.SetActive(false);
            // Debug.Log("//#100 #44 플레이어2 완전히 죽음");
        }
        music.StopSubPlayerSoundEffect();  // 플레이어에게 적용되었던 'PLYAER_IN_BALLOON' 효과음 멈추기
    }

    IEnumerator PlayerBeInvincible(float time)
    {
        playerInvincible = true;
        playerShield.SetActive(true);
        music.SubPlayerSoundEffect(Music.EFFECT_TYPE.PLAYER_SHIELD);   // 플레이어가 shield 아이템 사용하고 있는 동안의 효과음

        yield return  new WaitForSeconds(time);
        playerShield.SetActive(false);
        playerInvincible = false;

    }
}
