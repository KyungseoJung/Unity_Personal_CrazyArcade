﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SubPlayerCtrl : MonoBehaviour
{

// #100 플레이어 기본 이동 =============================
    private enum MOVE_ARROW {UP = 1, DOWN, RIGHT, LEFT};    // refactor: 플레이어 움직이는 방향
    private enum PLAYER_POS {UP=1, DOWN, RIGHT, LEFT};      // refactor: 플레이어의 위치 - 장애물과 비교했을 때
    private PLAYER_POS playerPos = PLAYER_POS.UP;

    [SerializeField] private GameObject maridObj;        // fix: 'Bazzi' Object - 덤불(Bush)에 배찌가 숨는 것처럼 보이도록 오브젝트 자체를 활성화/ 비활성화
    [SerializeField] private GameObject ridesObj;        // fix: 'rides' Object - 덤불(Bush)에 배찌가 숨는 것처럼 보이도록 오브젝트 자체를 활성화/ 비활성화
    [SerializeField] private SpriteRenderer ridesSprite;                  // fix: 거북이 덤불로 들어가면 alpha로 설정해서 안 보이게 하기

    private GameObject shadowObj;       // fix: 'shadow' Object - 덤불(Bush)에 배찌가 숨는 것처럼 보이도록 오브젝트 자체를 활성화/ 비활성화
    private Rigidbody rBody;               // 2D에서 3D로 변경
    [SerializeField] private SortingGroup Player2Group;   // 최상위 오브젝트 'MainPlayer'의 SortingGroup 
    private Animator anim;                 // #104 플레이어 애니메이터

    private SubPlayerLife subPlayerLife;    // #102 플레이어2가 물풍선 놓을 수 있게 하기 위함.- 플레이어 기절 확인
    private MapManager mapMgr;              // #102 플레이어2가 물풍선 놓을 수 있게 하기 위함.
    private Obstacle obstacle;             // 플레이어가 숨을 수 있는 덤불
    private Music music;                   // 바늘 아이템 사용해서 물풍선 벗어날 때 효과음
    private SubLobbyManager subLobbyMgr;         // 플레이어가 아이템 사용할 때마다, UI에 표시되는 아이템 개수를 업데이트 하기 위함.

    private Vector2 slideDirection = new Vector2(0, 0); 
    private Vector3 pos;                   // 플레이어가 게임 맵 경계선 밖으로 넘어가지 않도록 확인


[SerializeField]    private float moveForce;  //17f-> 50f로 변경(Start함수에서) // 이동할 때 주는 힘 - 처음 설정 값은 20
    private float originMoveForce;          // 처음 설정 값 가져오기 - 처음 설정 값 저장용
    private float slowTurtleMountMoveForce;     // moveForce의 1/2배로 설정(Start함수에서) // #35 거북에 탔을 때, 이동 속도
    private float fastTurtleMountMoveForce;     // moveForce의 2배로 설정(Start함수에서) - 빠른 거북에 탔을 때, 이동 속도
    private float trappedInWaterMoveForce;  // feat: 물풍선 안에 갇혔을 때의 속도 설정
    private float slideSpeed = 3f;       // 장애물에 닿으면 옆으로 부드럽게 지나가게 하기 위한 변수

[SerializeField]    private float maxSpeed;            // 가속도 적용 속도
    private float originMaxSpeed;           // fix 처음 설정 값 가져오기
    private float slowTurtleMountMaxSpeed;      // 거북에 탔을 때, 가속도
    private float fastTurtleMountMaxSpeed;      // 빠른 거북에 탔을 때, 가속도
    private float trappedInWaterMaxSpeed;   // 물풍선 안에 갇혔을 때의 속도 설정

    private float h;                        // 좌우 버튼 누르는 것 감지
    private float v;                        // 상하 버튼 누르는 것 감지
    private float distX;                     // 플레이어와 장애물 간의 거리 (X축)
    private float distY;                     // 플레이어와 장애물 간의 거리 (Y축)
    private float lastMoveTime =0f;             // 플레이어가 움직임을 보인 마지막 시각
    private float checkTimeInterval = 2f;   // 2초

    private float posX, posY;               // 
    private bool lookingAhead = false;              // 정면 바라보는지 체크


    void Awake()
    {
        maridObj = transform.GetChild(2).gameObject;    // 하위 3번째 오브젝트가 maridObj
        ridesObj = transform.GetChild(1).gameObject;    // 하위 2번째 오브젝트가 ridesObj
        ridesSprite = transform.GetChild(1).GetComponent<SpriteRenderer>(); // 거북이 덤불로 들어가면 alpha로 설정해서 안 보이게 하기

        shadowObj = transform.GetChild(0).gameObject;   // 하위 1번째 오브젝트가 shadowObj
        rBody = GetComponent<Rigidbody>();
        Player2Group = transform.GetComponent<SortingGroup>();   // 최상위 오브젝트 'MainPlayer'의 SortingGroup
        anim = GetComponent<Animator>();    //#104

        subPlayerLife = GetComponent<SubPlayerLife>();  // #102
        mapMgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>(); // #102
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>(); 
        subLobbyMgr = GameObject.Find("SubLobbyManager").GetComponent<SubLobbyManager>(); 
    }
    
    void Start()
    {
        // 디폴트 속도 설정
        moveForce =50f;
        maxSpeed = 5f;

        originMoveForce = moveForce;    
        originMaxSpeed = maxSpeed;      

        slowTurtleMountMoveForce = moveForce/2f;  // 거북에 탔을 때 속도 설정
        slowTurtleMountMaxSpeed = maxSpeed - 3f;   // 거북에 탔을 때 가속도 설정

        fastTurtleMountMoveForce = moveForce*2f;   // 빠른 거북에 탔을 때 속도 설정
        fastTurtleMountMaxSpeed = maxSpeed + 3f;   // 빠른 거북에 탔을 때 가속도 설정

        trappedInWaterMoveForce = moveForce/5f; // 물풍선 안에 갇혔을 때의 속도 설정
        trappedInWaterMaxSpeed = maxSpeed - 4f; // 물풍선 안에 갇혔을 때의 속도 설정

        anim.SetInteger("MoveDir", 2);  // 플레이어의 첫 방향을 DOWN으로 설정

        lastMoveTime = Time.time;       // lastMoveTime을 시작 Time.time으로 설정해야, Update 함수에서 SetTrigger("LookingAhead")이 실행이 안 되고,
                                        // 그렇게 해야 처음에 플레이어가 '앞을 바라보는 애니메이션'이 아닌, 'PlayerSpin'애니메이션을 실행할 수 있게 됨.
    }
    void Update()
    {
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            PlayerMove(true);
        else if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            PlayerMove(false);  
            
        // #23 플레이어가 2초동안 움직이지 않고 있다면, 정면을 바라보는 애니메이션 실행되도록
        if((Time.time - lastMoveTime > checkTimeInterval) && !lookingAhead)
        {
            lookingAhead = true;
            anim.SetInteger("MoveDir", 0);  //상하좌우 어느쪽도 쳐다보지 않도록

            anim.SetTrigger("LookingAhead");
            // Debug.Log("//#104 플레이어2 LookingAhead Trigger 실행 체크");
        }

        // 플레이어 방향키 누르는 값 Set - 방향키 누르고 있다면, 달리는 애니메이션 재생
        anim.SetFloat("horizontalSpeed", h);    
        anim.SetFloat("verticalSpeed", v);      

        Player2Group.sortingOrder = - Mathf.RoundToInt(transform.position.y);    

        if((Input.GetKeyDown(KeyCode.LeftShift)) && (subPlayerLife.playerFaint == false))
        {
            mapMgr.PlaceWaterBalloon(transform.position.x, transform.position.y, false);  // x위치는 열의 값으로, y위치는 행의 값으로 
        }


        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if((subPlayerLife.trappedInWater) && (SubPlayerGameMgr.SubMgr.needle > 0))
            {
                // #41 만약 물풍선에 갇혀 있을 때, 키보드의 숫자 '2'를 누르면~ 물풍선에서 벗어나도록
                subPlayerLife.trappedInWater = false;  // #41 물풍선 벗어나도록
                subPlayerLife.waterApplied = false;    // #17 fix: 플레이어가 물풍선 벗어날 때, 변수 'waterApplied' 를 false로 설정하라 - 한번 물풍선 탈출하면, 그 이후에 물풍선 적용이 안 되는 문제 해결하기 위해
                anim.SetTrigger("EscapeWater");     // #41 물풍선 벗어나는 애니메이션 실행 뒤, 기존 PlayerLookingAhead 애니메이션 실행

                music.GameSoundEffect(Music.EFFECT_TYPE.BOMB_POP, 0.6f);    // #43 바늘 아이템 사용해서 물풍선 벗어날 때 효과음
                music.StopSubPlayerSoundEffect();  // #47 플레이어에게 적용되었던 'PLYAER_IN_BALLOON' 효과음 멈추기
                SubPlayerGameMgr.SubMgr.needle -= 1;      // #43 바늘 아이템 사용

                ChangePlayerSpeed(SubPlayerGameMgr.SubMgr.roller); // #41 플레이어가 물풍선에서 벗어나면, 본래 속도로 돌아가도록 - 획득한 roller 아이템을 바탕으로 본래 속도로 돌아가기
                subLobbyMgr.UpdateNumberOfItems();     //#59 아이템 개수 업데이트해서 UI에 표시
            }
            // Debug.Log("//#41 키보드의 숫자 '2'를 누름");  
        }


        // #54 거북 속도 변경
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if((SubPlayerGameMgr.SubMgr.slowTurtle == true) && (SubPlayerGameMgr.SubMgr.fastTurtle == false)
                && (SubPlayerGameMgr.SubMgr.turtleCan > 0))   //#54 플레이어가 느린 거북을 타고 있을 때에만 Can 아이템이 작동하도록
            {
                music.GameSoundEffect(Music.EFFECT_TYPE.TURTLE_CHANGE); //#54 빠른 거북으로 바뀔 때의 효과음
                
                // 플레이어가 타고 있는 거북의 이미지 & 속도 변경
                SubPlayerGameMgr.SubMgr.slowTurtle = false;
                SubPlayerGameMgr.SubMgr.fastTurtle = true;
                //#54 animation 설정도 해주기
                anim.SetBool("turtleMount", false);
                anim.SetBool("fastTurtleMount", true);
                
                anim.SetBool("canMove", false);     //#54 거북 바뀌는 애니메이션 실행 목적
                anim.SetTrigger("ChangeTurtle");    //#54 거북 바뀌는 애니메이션 실행 목적
                Invoke("CanMoveTrue", 0.5f);        //#54 다시 canMove 변수를 True로 만들어주기
                
                LimitToTurtleSpeed(true);   //#54 플레이어의 속도를 빠른 거북 속도로 바꿔주기

                SubPlayerGameMgr.SubMgr.turtleCan -= 1;   //#59 아이템을 사용했으니까, 개수 -1
                subLobbyMgr.UpdateNumberOfItems();     //#59 아이템 개수 업데이트해서 UI에 표시

            }
        }
 
    }

    void FixedUpdate()
    {   

// 플레이어 움직이기 ===============================

        h = Input.GetAxis("SubHorizontal");  // 좌우 키
        v = Input.GetAxis("SubVertical");    // 상하 키
        // Debug.Log("//#100 sub h: " + h);
        // Debug.Log("//#100 sub v: " + v);

        // CheckBorder();  // 플레이어가 경계선 넘어가지 않도록 확인 - 코드 대신에 BoxCollider로 움직임을 제어하는 게 더 자연스러워 보임.
    }

    private void OnCollisionStay(Collision other) 
    {

        // if(other.gameObject.tag == "Obstacle")  // 장애물에 닿으면, 미끄러지듯이 지나갈 수 있도록 - 플레이어 몸을 옆으로 밀기
        if((other.gameObject.tag == "Obstacle") || (other.gameObject.tag == "WaterBalloon") || (other.gameObject.tag == "Block"))  // 미끄러지듯이 지나가는 장애물에 "WaterBalloon"이나 "Block" tag를 가진 장애물도 포함되도록 하기
        {
            distX = (transform.position.x - other.transform.position.x)*(transform.position.x - other.transform.position.x);
            distY = (transform.position.y - other.transform.position.y)*(transform.position.y - other.transform.position.y);
            
            if(Input.GetKey(KeyCode.S)) // 플레이어가 장애물 위에서 아래로 가려고 할 때
            {
                if(distX < (0.2)*(0.2))   // x축을 기준으로 플레이어와 장애물 간의 거리 차가 별로 없다면, 미끄러지지 않도록 = 플레이어가 장애물에 계속 걸리도록
                    return;

                if(transform.position.x > other.transform.position.x)   // 플레이어가 장애물보다 오른쪽에 있으면
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.DOWN, PLAYER_POS.RIGHT);    
                else    // 플레이어가 장애물보다 왼쪽에 있으면
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.DOWN, PLAYER_POS.LEFT);    
            }
            else if(Input.GetKey(KeyCode.W))   // fix 플레이어가 장애물 아래에서 위로 가려고 할 때
            {
                if(distX < (0.2)*(0.2))   // x축을 기준으로 플레이어와 장애물 간의 거리 차가 별로 없다면, 미끄러지지 않도록 = 플레이어가 장애물에 계속 걸리도록
                    return;

                if(transform.position.x > other.transform.position.x)   // 플레이어가 장애물보다 오른쪽에 있으면
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.UP, PLAYER_POS.RIGHT);    
                else
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.UP, PLAYER_POS.LEFT);    
            }
            else if(Input.GetKey(KeyCode.D))   // 플레이어가 장애물 왼쪽에서 오른쪽으로 가려고 할 때
            {
                if(distY < (0.2)*(0.2))   //Y축을 기준으로 플레이어와 장애물 간의 거리 차가 별로 없다면, 미끄러지지 않도록 = 플레이어가 장애물에 계속 걸리도록
                    return;

                if(transform.position.y > other.transform.position.y)   // 플레이어가 장애물보다 위쪽에 있으면
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.RIGHT, PLAYER_POS.UP);
                else
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.RIGHT, PLAYER_POS.DOWN);
            }
            else if(Input.GetKey(KeyCode.A))    // 플레이어가 장애물 오른쪽에서 왼쪽으로 가려고 할 때
            {
                if((distY < (0.2)*(0.2)))   // Y축을 기준으로 플레이어와 장애물 간의 거리 차가 별로 없다면, 미끄러지지 않도록 = 플레이어가 장애물에 계속 걸리도록
                    return;
                

                if(transform.position.y > other.transform.position.y)   // 플레이어가 장애물보다 위쪽에 있으면
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.LEFT, PLAYER_POS.UP);
                else
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.LEFT, PLAYER_POS.DOWN);                
            }

        }    
    }

    private void OnTriggerEnter(Collider other)     
    {
        if(other.gameObject.tag == "Bush")  // #105 Bush에 들어가면 플레이어가 아예 안 보이도록
        {
            ObjSetActive(maridObj, false);
            ObjSetActive(ridesObj, false);
            SetAlpha(ridesSprite, 0f);

            ObjSetActive(shadowObj, false);

            obstacle = other.gameObject.GetComponentInParent<Obstacle>();   // 콜라이더 부모 위치에 스크립트가 있으므로

            if(obstacle != null)
            {
                obstacle.BushShake();
            }
        }    
    }
    
    private void OnTriggerExit(Collider other)      
    {
        if(other.gameObject.tag == "Bush")  // #105 Bush에서 나오면 플레이어 모습이 보이도록
        {
            ObjSetActive(maridObj, true);
            ObjSetActive(ridesObj, true);
            SetAlpha(ridesSprite, 1f);

            ObjSetActive(shadowObj, true);

            obstacle = other.gameObject.GetComponentInParent<Obstacle>();   // 콜라이더 부모 위치에 스크립트가 있으므로

            if(obstacle != null)
            {
                obstacle.BushShake();
            }        
        }
    }
    private void CanMoveTrue()
    {
        // 따로 invoke로 실행이 필요한 때를 위함.
        anim.SetBool("canMove", true);  
    }
    void SlideAlongObstacle(Vector2 obstacleNormal, MOVE_ARROW moveArrow, PLAYER_POS playerPos)    
    //# refactor 플레이어가 누르는 방향 키와, 플레이어의 위치(장애물과 비교했을 때 상대적 위치)를 parameter로 받기
    {
        switch(moveArrow)
        {
            case MOVE_ARROW.UP:
                if(playerPos == PLAYER_POS.RIGHT)
                    slideDirection = new Vector2(-obstacleNormal.y, 0);
                else if(playerPos == PLAYER_POS.LEFT)
                    slideDirection = new Vector2(obstacleNormal.y, 0);
                
                break;
                
            case MOVE_ARROW.DOWN:
                if(playerPos == PLAYER_POS.RIGHT)
                    slideDirection = new Vector2(obstacleNormal.y, 0);
                else if(playerPos == PLAYER_POS.LEFT)
                    slideDirection = new Vector2(-obstacleNormal.y, 0);

                break;

            case MOVE_ARROW.RIGHT:
                if(playerPos == PLAYER_POS.UP)
                    slideDirection = new Vector2(0, -obstacleNormal.x);
                else if(playerPos == PLAYER_POS.DOWN)
                    slideDirection = new Vector2(0, obstacleNormal.x);

                break;
            case MOVE_ARROW.LEFT:
                if(playerPos == PLAYER_POS.UP)
                    slideDirection = new Vector2(0, obstacleNormal.x);
                else if(playerPos == PLAYER_POS.DOWN)
                    slideDirection = new Vector2(0, -obstacleNormal.x);

                break;

        }
        // 장애물의 법선 벡터를 기반으로 옆으로 미끄러지는 효과 적용
        transform.Translate(slideDirection * slideSpeed * Time.deltaTime);
    }


    // private void CheckBorder()
    // {
    //     // 플레이어가 게임 맵 경계선 밖으로 넘어가지 않도록 확인
    //     if((transform.position.x) * (transform.position.x) > 16 )
    //     {
    //         pos = this.transform.position;
    //         pos.x = (int)this.transform.position.x; // -4 또는 4로 지정
    //         this.transform.position = pos;
    //     }
        
    //     if((transform.position.y) * (transform.position.y) > 9)
    //     {
    //         pos = this.transform.position;
    //         pos.y = (int)this.transform.position.y; // -3 또는 3으로 지정
    //         this.transform.position = pos;
    //     }

    // }

    void PlayerMove(bool moveHorizontal)
    {
        if(subPlayerLife.playerFaint) // #28 플레이어 기절하고 있다면, 움직일 수 없도록
            return; 

        // Debug.Log("//#28 플레이어 움직이고 있음. 기절 안 함.");

        Vector3 moveDirection = new Vector3(0, v);

        lastMoveTime = Time.time;   // #23 플레이어가 움직임을 보인 마지막 시각
        lookingAhead = false;       // #23

        if(moveHorizontal)
        {
            if((h<0) && anim.GetInteger("MoveDir")!=3 ) // 중복 방지 - 이미 0인 값을 또 0이라 설정하지 않도록 
                anim.SetInteger("MoveDir", 3);  //왼쪽 쳐다보도록
            else if((h>0) && anim.GetInteger("MoveDir")!=4)     // 중복 방지
                anim.SetInteger("MoveDir", 4);  //오른쪽 쳐다보도록

            // 좌우 움직임 
                // maxSpeed에 아직 도달하지 않을때까지 플레이어 객체에 힘을 가해
                // h(-1.0f~1.0f)는 velocity.x를 다르게 표시한다
                if(h * rBody.velocity.x < maxSpeed)	// h가 음수이면-> rigidbody2d.velocity.x도 음수. // h가 양수이면-> 양수
                {
                    rBody.AddForce(Vector3.right * h * moveForce);	//오른쪽방향(1,0) * 방향 * 힘 <-> 왼쪽 방향이면 (-1, 0)
                    // Debug.Log("//#28 플레이어 이동 중");
                }    
            // 좌우 움직임 
                // 가속도해서 더래진 플레이어의 <<수평>> 속도가 maxSpeed 보다 커지면 maxSpeed로 속도 유지
                if(Mathf.Abs(rBody.velocity.x) > maxSpeed)  
                {	
                    //플레이어의 velocity(속도)를 x축방향으로 maxSpeed 로 셋팅해줘라 또한 기존 rigidbody2D.velocity.y 도 셋팅 해 줘야 한다.
                    // Mathf.Sign() 는 매개변수를 참조해서 1 또는 -1(float)을 반환  
                    rBody.velocity = new Vector2(Mathf.Sign(rBody.velocity.x) * maxSpeed, rBody.velocity.y);
                    // Debug.Log("//#28 플레이어 이동 중");
                }
        }
        else
        {
            if((v>0) && anim.GetInteger("MoveDir")!=1 ) 
                anim.SetInteger("MoveDir", 1);  //위쪽 쳐다보도록
            else if((v<0) && anim.GetInteger("MoveDir")!=2 )
                anim.SetInteger("MoveDir", 2);  //아래쪽 쳐다보도록

            // #1 상하 움직임 
            if(v * rBody.velocity.y < maxSpeed)
            {
                rBody.AddForce(moveDirection * moveForce);
            }
            // #1 상하 움직임 
            if(Mathf.Abs(rBody.velocity.y) > maxSpeed)
            {
                rBody.velocity = new Vector2(rBody.velocity.x, Mathf.Sign(rBody.velocity.y) * maxSpeed);
            }
        }

    }

    void SetAlpha(SpriteRenderer _sprite, float _alpha) // #6 플레이어가 덤불 오브젝트에 가까이에 가면 안 보이도록
    {
        // Debug.Log("//#6 플레이어 sprite의 alpha 설정: " + _alpha);
        _sprite.color = new Color(1f, 1f, 1f, _alpha);
    }
    
    private void ObjSetActive(GameObject _obj, bool _active)
    {
        // Debug.Log(_obj + "를 활성화한다?: " + _active);
        _obj.SetActive(_active);
    }

    public bool CheckPlayerVisible()    // 만약 플레이어가 (Bush 같은 것들로 인해) 가려져 있었다면, 다시 보이게 하기
    {
        if((maridObj.activeSelf))  // 플레이어가 잘 보이고 있다면, true return
            return true;

        return false;               // 만약 (Bush 같은 것들로 인해) 가려져 있다면 false return
    }

    public void MakePlayerVisible() // 만약 플레이어가 (Bush 같은 것들로 인해) 가려져 있었다면, 다시 보이게 하기
    {
        ObjSetActive(maridObj, true);
        ObjSetActive(ridesObj, true);
        ObjSetActive(shadowObj, true);
    }

    public void ChangePlayerSpeed(int rollerCount)   // #109 ROLLER 아이템 획득에 따라 플레이어 이동 속도 달라지도록
    {
        if((SubPlayerGameMgr.SubMgr.slowTurtle) || (SubPlayerGameMgr.SubMgr.fastTurtle))    // 만약 플레이어2가 거북에 타고 있었다면
        {
            return;
        }

        // ROLLER 아이템 획득 개수에 따라 플레이어 이동 속도 설정
        // moveForce 디폴트 값: 30f, maxSpeed 디폴트 값: 5f
        moveForce = originMoveForce + (rollerCount) * 3;
        maxSpeed = originMaxSpeed + (rollerCount) * 0.3f;

        // Debug.Log("//#109 플레이어2 속도 증가. moveForce: " + moveForce + "| maxSpeed: " + maxSpeed);
    }

    public void SetPlayerSpeed(bool _down = true)   // style: 함수 이름 변경
    {
        switch(_down)
        {
            case true:
                // 플레이어가 물풍선에 갇히면, 플레이어 이동 속도 느려지도록
                // Debug.Log("//#17 플레이어 이동 속도 느려지도록");

                moveForce = trappedInWaterMoveForce;    // originMoveForce - 10f;    // 7f - 2f 
                maxSpeed = trappedInWaterMaxSpeed;      //  originMaxSpeed - 0.6f;      // 2f - 1f;
                break;
            case false: 
            // 플레이어 본래 (디폴트) 속도로
            // - 플레이어 부활할 때
                moveForce = originMoveForce;    // 7f
                maxSpeed = originMaxSpeed;      // 2f      
                break;
        }

    }

    public void TurtleMount(bool _mount = true, bool _fastTurtle = false) // #110 플레이어가 거북에 올라탐 - 디폴트는 느린 거북
    {
        if(_mount)
        {
            // turtleMount = true;
            // ㄴItem.cs에서 OnTriggerEnter 함수에서 직접 PlayerGameMgr.Mgr.slowTurtle 또는 PlayerGameMgr.Mgr.fastTurtle 값을 설정해줌.
            // Debug.Log("//#110 플레이어2가 거북에 올라탐");

            LimitToTurtleSpeed(_fastTurtle);

            switch(_fastTurtle)
            {
                case false:
                    anim.SetBool("turtleMount", true);
                    break;
                case true:  // 빠른 거북 설정
                    anim.SetBool("fastTurtleMount", true);
                    // Debug.Log("//#110 빠른 거북 탑승 anim 설정");
                    break;
            }

        }
    }
    private void LimitToTurtleSpeed(bool _fastTurtle = false)   // #110 플레이어의 움직임 속도를 거북 탔을 때 속도로 설정
    {
        if(!_fastTurtle)
        {
            moveForce = slowTurtleMountMoveForce;
            maxSpeed = slowTurtleMountMaxSpeed;
        }
        else if(_fastTurtle)    // #110 빠른 거북을 타고 있을 땐 속도 fastTurtle로 맞추기
        {
            moveForce = fastTurtleMountMoveForce;
            maxSpeed = fastTurtleMountMaxSpeed;
        }

    }

}
