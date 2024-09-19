using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCtrl : MonoBehaviour // #1 
{
// #1 플레이어 기본 이동 =============================
    private enum MOVE_ARROW {UP = 1, DOWN, RIGHT, LEFT};    // #5 refactor: 플레이어 움직이는 방향
    // private MOVE_ARROW moveArrow = MOVE_ARROW.UP;
    private enum PLAYER_POS {UP=1, DOWN, RIGHT, LEFT};      // #5 refactor: 플레이어의 위치 - 장애물과 비교했을 때
    private PLAYER_POS playerPos = PLAYER_POS.UP;

    [SerializeField]
    private GameObject bazziObj;        // #6 fix: 'Bazzi' Object - 덤불(Bush)에 배찌가 숨는 것처럼 보이도록 오브젝트 자체를 활성화/ 비활성화
    private GameObject shadowObj;       // #6 fix: 'shadow' Object - 덤불(Bush)에 배찌가 숨는 것처럼 보이도록 오브젝트 자체를 활성화/ 비활성화
    private Rigidbody rBody;               // 2D에서 3D로 변경
    // private SpriteRenderer bazziSprite;                  // #2 플레이어 위치에 따라 오브젝트 앞에 or 뒤에 그려지도록 
    [SerializeField] private SortingGroup mainPlayerGroup;   // #2 fix #48 feat 최상위 오브젝트 'MainPlayer'의 SortingGroup 
    private Animator anim;                 // #3 플레이어 애니메이터

    private PlayerLife playerLife;         // #28 플레이어 기절 확인
    private MapManager mapMgr;             // #4 물풍선 놓기 위함
    private Obstacle obstacle;             // #6 플레이어가 숨을 수 있는 덤불
    private Music music;                   // #43 바늘 아이템 사용해서 물풍선 벗어날 때 효과음
    private Vector2 slideDirection = new Vector2(0, 0); // #5
    private Vector3 pos;                   // #24 플레이어가 게임 맵 경계선 밖으로 넘어가지 않도록 확인

    // [SerializeField]
    // private bool dirRight = false;         // 플레이어가 바라보는 방향(오른쪽 : 1, 왼쪽 : -1)

[SerializeField]    private float moveForce = 17f;         // 이동할 때 주는 힘 - 처음 설정 값은 20
    private float originMoveForce;          // #1 fix 처음 설정 값 가져오기 - 처음 설정 값 저장용
    private float turtleMountMoveForce;     // #35 거북에 탔을 때, 이동 속도

    private float slideSpeed = 3f;       // #5 장애물에 닿으면 옆으로 부드럽게 지나가게 하기 위한 변수

[SerializeField]    private float maxSpeed = 1f;            // 가속도 적용 속도
    private float originMaxSpeed;           // #1 fix 처음 설정 값 가져오기
    private float turtleMountMaxSpeed;      // #35 거북에 탔을 때, 가속도

    private float h;                        // 좌우 버튼 누르는 것 감지
    private float v;                        // 상하 버튼 누르는 것 감지
    private float distX;                     // #5 플레이어와 장애물 간의 거리 (X축)
    private float distY;                     // #5 플레이어와 장애물 간의 거리 (Y축)
    private float lastMoveTime =0f;             // #23 플레이어가 움직임을 보인 마지막 시각
    private float checkTimeInterval = 2f;   // #23 2초

    private float posX, posY;               // #33
    private bool lookingAhead = false;              // #23 정면 바라보는지 체크
    // public bool balloonInFront = false;    // #33 앞에 물풍선 있는지 확인 - 있다면, 플레이어 이동 불가
    public bool turtleMount = false;       // #35 거북에 올라탐

    void Awake()
    {
        bazziObj = transform.GetChild(2).gameObject;    // #6 fix   // #6 fix: 하위 3번째 오브젝트가 BazziObj
        shadowObj = transform.GetChild(0).gameObject;   // #6 fix   // #6 fix: 하위 1번째 오브젝트가 shadowObj
        rBody = GetComponent<Rigidbody>();
        // bazziSprite = transform.GetChild(2).GetComponent<SpriteRenderer>();  // #2  // #6 fix: 하위 3번째 오브젝트가 BazziObj
        mainPlayerGroup = transform.GetComponent<SortingGroup>();   // #2 fix #48 feat 최상위 오브젝트 'MainPlayer'의 SortingGroup
        anim = GetComponent<Animator>();    // #3

        playerLife = GetComponent<PlayerLife>();    // #28
        mapMgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>(); // #4 
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>(); // #43
    }
    
    void Start()
    {
        // #1 디폴트 속도 설정
        moveForce =25f;
        maxSpeed = 4f;

        originMoveForce = moveForce;    // #1 fix
        originMaxSpeed = maxSpeed;      // #1 fix

        turtleMountMoveForce = moveForce - 5f;  // #35 거북에 탔을 때 속도 설정
        turtleMountMaxSpeed = maxSpeed - 3f;   // #35 거북에 탔을 때 가속도 설정

        anim.SetInteger("MoveDir", 2);  // #5 플레이어의 첫 방향을 DOWN으로 설정
    }
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            PlayerMove(true);
        else if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            PlayerMove(false);

        // #23 플레이어가 2초동안 움직이지 않고 있다면, 정면을 바라보는 애니메이션 실행되도록
        if((Time.time - lastMoveTime > checkTimeInterval) && !lookingAhead)
        {
            lookingAhead = true;
            anim.SetInteger("MoveDir", 0);  //상하좌우 어느쪽도 쳐다보지 않도록

            // #23 fix: 애니메이션은 SetBool이 아닌 Trigger로 설정 - 애니메이션이 계속 시작 부분만 실행되는 문제 해결 목적
            // anim.SetBool("LookingAhead", true); 
            // StartCoroutine(StartLookingAhead());
            anim.SetTrigger("LookingAhead");
            
            // Debug.Log("//#23 플레이어 정면 바라보기");
            // Debug.Log("Time.time: " + Time.time);
            // Debug.Log("시간차: " + (Time.time - lastMoveTime));
        }

        
        // #1 플레이어 방향키 누르는 값 Set - 방향키 누르고 있다면, 달리는 애니메이션 재생
        // Debug.Log("//#1 rBody.velocity.x: " + rBody.velocity.x );
        // Debug.Log("//#1 h: " + h);
        anim.SetFloat("horizontalSpeed", h);    
        anim.SetFloat("verticalSpeed", v);      

        if(Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            anim.SetFloat("horizontalSpeed", 0);
        }
        // #2 y축 기준으로 밑에 있을수록 더 위에 그려져야 하므로 반비례 -> -1
        // bazziSprite.sortingOrder = - Mathf.RoundToInt(transform.position.y); 
        // #2 fix #48 feat 하위 오브젝트 'Bazzi'의 OrderInLayer만 수정하는 방법 대신, 최상위 오브젝트 'MainPlayer'의 OrderInLayer를 수정하는 방법 이용.
        mainPlayerGroup.sortingOrder = - Mathf.RoundToInt(transform.position.y);    


        // #4 물풍선 놓기
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("//#4 fix x좌표: " + transform.position.x + ", y좌표: " + transform.position.y);
            mapMgr.PlaceWaterBalloon(transform.position.x, transform.position.y);  // x위치는 열의 값으로, y위치는 행의 값으로 
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if((playerLife.trappedInWater) && (PlayerGameMgr.Mgr.needle > 0))
            {
                // 만약 물풍선에 갇혀 있을 때, 키보드의 숫자 '2'를 누르면~
                playerLife.trappedInWater = false;  // #41 물풍선 벗어나도록
                playerLife.waterApplied = false;    // #17 fix: 플레이어가 물풍선 벗어날 때, 변수 'waterApplied' 를 false로 설정하라 - 한번 물풍선 탈출하면, 그 이후에 물풍선 적용이 안 되는 문제 해결하기 위해
                anim.SetTrigger("EscapeWater");     // #41 물풍선 벗어나는 애니메이션 실행 뒤, 기존 PlayerLookingAhead 애니메이션 실행
                Debug.Log("//#41 물풍선 탈출");
                music.GameSoundEffect(Music.EFFECT_TYPE.BOMB_POP, 0.6f);    // #43 바늘 아이템 사용해서 물풍선 벗어날 때 효과음
                music.StopPlayerSoundEffect();  // #47 플레이어에게 적용되었던 'PLYAER_IN_BALLOON' 효과음 멈추기
                PlayerGameMgr.Mgr.needle -= 1;      // #43 바늘 아이템 사용
            }
            Debug.Log("//#41 키보드의 숫자 '2'를 누름");  
        }
    }


    void FixedUpdate()
    {   

// #1 플레이어 움직이기 ===============================

        h = Input.GetAxis("Horizontal");  // 좌우 키
        v = Input.GetAxis("Vertical");    // 상하 키

        CheckBorder();  // #24 플레이어가 경계선 넘어가지 않도록 확인

        // Debug.Log("//#42 fix: 플레이어 위치: " + this.transform.position);

// #1 fix : 플레이어 이동 방법 바꾸자
/*
        float h = Input.GetAxis("Horizontal");  // 좌우 키
        float v = Input.GetAxis("Vertical");    // 상하 키
        Vector3 moveDirection = new Vector3(0, v);

    // #2 상하좌우 키 하나만 작동되도록
        if(h != 0)
            v = 0;
        else if( v != 0)
            h = 0;
        // Debug.Log("좌우 키 h값 : " + h);
        // Debug.Log("상하 키 v값 : " + v);


    // #1 가속도 적용 ===============================

    // #1 좌우 움직임 
        // maxSpeed에 아직 도달하지 않을때까지 플레이어 객체에 힘을 가해
        // h(-1.0f~1.0f)는 velocity.x를 다르게 표시한다
        if(h * rBody.velocity.x < maxSpeed)	// h가 음수이면-> rigidbody2d.velocity.x도 음수. // h가 양수이면-> 양수
            rBody.AddForce(Vector3.right * h * moveForce);	//오른쪽방향(1,0) * 방향 * 힘 <-> 왼쪽 방향이면 (-1, 0)

    // #1 상하 움직임 
        if(v * rBody.velocity.y < maxSpeed)
            rBody.AddForce(moveDirection * moveForce);

        Debug.Log(moveDirection.magnitude);
            // rBody.AddForce(Vector2.up * v * moveForce);
            // 걱정: rigidBody2D인데 Vector3가 적용될까?

    // #1 좌우 움직임 
        // 가속도해서 더래진 플레이어의 <<수평>> 속도가 maxSpeed 보다 커지면 maxSpeed로 속도 유지
        if(Mathf.Abs(rBody.velocity.x) > maxSpeed)  
        {	
            //플레이어의 velocity(속도)를 x축방향으로 maxSpeed 로 셋팅해줘라 또한 기존 rigidbody2D.velocity.y 도 셋팅 해 줘야 한다.
            // Mathf.Sign() 는 매개변수를 참조해서 1 또는 -1(float)을 반환  
            rBody.velocity = new Vector2(Mathf.Sign(rBody.velocity.x) * maxSpeed, rBody.velocity.y);
        }
    // #1 상하 움직임 
        else if(Mathf.Abs(rBody.velocity.y) > maxSpeed)
        {
            rBody.velocity = new Vector2(rBody.velocity.x, Mathf.Sign(rBody.velocity.y) * maxSpeed);
        }
*/        

        // // #1 플레이어 이미지 뒤집기 =============================== --> // #3 애니메이터로 조작해서 Flip 기능 필요 없어짐
        // if(((h>0) && !dirRight) || ((h<0) && dirRight))// 움직이는 방향과 바라보는 방향이 다르다면
        // {
        //     Flip();
        // } 

    }

    private void OnCollisionStay(Collision other) 
    {

        // if(other.gameObject.tag == "Obstacle")  // #5 장애물에 닿으면, 미끄러지듯이 지나갈 수 있도록 - 플레이어 몸을 옆으로 밀기
        if((other.gameObject.tag == "Obstacle") || (other.gameObject.tag == "WaterBalloon") || (other.gameObject.tag == "Block"))  // #5 미끄러지듯이 지나가는 장애물에 "WaterBalloon"이나 "Block" tag를 가진 장애물도 포함되도록 하기
        {
            distX = (transform.position.x - other.transform.position.x)*(transform.position.x - other.transform.position.x);
            distY = (transform.position.y - other.transform.position.y)*(transform.position.y - other.transform.position.y);
            
            if(Input.GetKey(KeyCode.DownArrow)) // #5 fix 플레이어가 장애물 위에서 아래로 가려고 할 때
            {
                if(distX < (0.2)*(0.2))   //#5 x축을 기준으로 플레이어와 장애물 간의 거리 차가 별로 없다면, 미끄러지지 않도록 = 플레이어가 장애물에 계속 걸리도록
                    return;

                if(transform.position.x > other.transform.position.x)   // 플레이어가 장애물보다 오른쪽에 있으면
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.DOWN, PLAYER_POS.RIGHT);    
                else    // 플레이어가 장애물보다 왼쪽에 있으면
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.DOWN, PLAYER_POS.LEFT);    
            }
            else if(Input.GetKey(KeyCode.UpArrow))   // #5 fix 플레이어가 장애물 아래에서 위로 가려고 할 때
            {
                if(distX < (0.2)*(0.2))   //#5 x축을 기준으로 플레이어와 장애물 간의 거리 차가 별로 없다면, 미끄러지지 않도록 = 플레이어가 장애물에 계속 걸리도록
                    return;

                if(transform.position.x > other.transform.position.x)   // 플레이어가 장애물보다 오른쪽에 있으면
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.UP, PLAYER_POS.RIGHT);    
                else
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.UP, PLAYER_POS.LEFT);    
            }
            else if(Input.GetKey(KeyCode.RightArrow))   // #5 fix 플레이어가 장애물 왼쪽에서 오른쪽으로 가려고 할 때
            {
                if(distY < (0.2)*(0.2))   //#5 Y축을 기준으로 플레이어와 장애물 간의 거리 차가 별로 없다면, 미끄러지지 않도록 = 플레이어가 장애물에 계속 걸리도록
                    return;

                if(transform.position.y > other.transform.position.y)   // 플레이어가 장애물보다 위쪽에 있으면
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.RIGHT, PLAYER_POS.UP);
                else
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.RIGHT, PLAYER_POS.DOWN);
            }
            else if(Input.GetKey(KeyCode.LeftArrow))    // #5 fix 플레이어가 장애물 오른쪽에서 왼쪽으로 가려고 할 때
            {
                if((distY < (0.2)*(0.2)))   //#5 Y축을 기준으로 플레이어와 장애물 간의 거리 차가 별로 없다면, 미끄러지지 않도록 = 플레이어가 장애물에 계속 걸리도록
                    return;
                
                // Debug.Log("//#5 LeftArrow: 장애물과 미끄러지는 중 | distY" + distY);

                if(transform.position.y > other.transform.position.y)   // 플레이어가 장애물보다 위쪽에 있으면
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.LEFT, PLAYER_POS.UP);
                else
                    SlideAlongObstacle(other.contacts[0].normal, MOVE_ARROW.LEFT, PLAYER_POS.DOWN);                
            }

            // Debug.Log("//#5 장애물 부딪힘");

            // if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            // {
            //     if(transform.position.x > other.gameObject.transform.position.x) // x 위치 값 비교해서 - 오른쪽 or 왼쪽으로 미끄러지기
            //     {
            //         Debug.Log("//#5 플레이어 오른쪽으로 밀기");

            //         rBody.AddForce(Vector3.right * slideSpeed);
            //     }    
            //     else
            //     {
            //         Debug.Log("//#5 플레이어 왼쪽으로 밀기");

            //         rBody.AddForce(Vector3.left * slideSpeed);                
            //     }
            // }
        }    
    }

    private void OnTriggerEnter(Collider other)     
    {
        if(other.gameObject.tag == "Bush")  // #6 
        {
            // SetAlpha(bazziSprite, 0f);    // #6 fix
            ObjSetActive(bazziObj, false);        // #6 fix
            ObjSetActive(shadowObj, false);       // #6 fix

            obstacle = other.gameObject.GetComponentInParent<Obstacle>();   // 콜라이더 부모 위치에 스크립트가 있으므로

            if(obstacle != null)
            {
                obstacle.BushShake();
            }
        }    
    }

    // private void OnTriggerStay(Collider other) 
    // {
    //     if(other.gameObject.tag == "WaterBalloon")
    //     {
    //         // Debug.Log("//#33 OnTriggerStay - WaterBalloon 닿음");
    //         CheckObstacleBalloon();    // #33 플레이어가 이동하고자 하는 방향에 물풍선이 있는지 확인
    //     }    
    // }
    
    private void OnTriggerExit(Collider other)      
    {
        if(other.gameObject.tag == "Bush")  // #6 
        {
            // SetAlpha(bazziSprite, 1f);    // #6 fix
            ObjSetActive(bazziObj, true);         // #6 fix
            ObjSetActive(shadowObj, true);        // #6 fix

            obstacle = other.gameObject.GetComponentInParent<Obstacle>();   // 콜라이더 부모 위치에 스크립트가 있으므로

            if(obstacle != null)
            {
                obstacle.BushShake();
            }        
        }    
        
        // #33 fix: SphereCollider로 통제해서 코드 필요 없음
        // if(other.gameObject.tag == "WaterBalloon")  // #33 fix: 앞에 물풍선 있는지 확인 - 있다면, 플레이어 이동 불가
        // {
        //     CheckObstacleBalloon(); // #33 fix
        // }
    }
    
    void SlideAlongObstacle(Vector2 obstacleNormal, MOVE_ARROW moveArrow, PLAYER_POS playerPos) // #5 fix   
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

    // void Flip() // #1 플레이어 바라보는 방향에 따라 적용  --> // #3 애니메이터로 조작해서 Flip 기능 필요 없어짐
    // {
    //     Debug.Log("//#2 플레이어 뒤집어");

    //     dirRight = !dirRight;   //바라보는 방향 변경

    //     Vector3 theScale = transform.localScale;
    //     theScale.x *= -1;
    //     transform.localScale = theScale;
    // }

    private void CheckBorder()
    {
        // Debug.Log("#42 fix: CheckBorder 함수 실행");

        // #24 플레이어가 게임 맵 경계선 밖으로 넘어가지 않도록 확인
        if((transform.position.x) * (transform.position.x) > 16 )
        {
            // Debug.Log("// #24 플레이어가 x좌표 경계선 넘어감");
            pos = this.transform.position;
            pos.x = (int)this.transform.position.x; // -4 또는 4로 지정
            this.transform.position = pos;
        }
        
        if((transform.position.y) * (transform.position.y) > 9)
        {
            // Debug.Log("// #24 플레이어가 y좌표 경계선 넘어감");
            pos = this.transform.position;
            pos.y = (int)this.transform.position.y; // -3 또는 3으로 지정
            this.transform.position = pos;
        }

    }

    void PlayerMove(bool moveHorizontal)
    {

        if(playerLife.playerFaint) // #28 플레이어 기절하고 있다면, 움직일 수 없도록
            return; 

        Vector3 moveDirection = new Vector3(0, v);
        
        // Debug.Log("//#1 PlayerMove");

        lastMoveTime = Time.time;   // #23 플레이어가 움직임을 보인 마지막 시각
        lookingAhead = false;       // #23
        // #23 fix: 애니메이션은 SetBool이 아닌 Trigger로 설정 - 애니메이션이 계속 시작 부분만 실행되는 문제 해결 목적
        // anim.SetBool("LookingAhead", false);


        if(moveHorizontal)
        {
            if((h<0) && anim.GetInteger("MoveDir")!=3 ) // #3   // 중복 방지 - 이미 0인 값을 또 0이라 설정하지 않도록 
                anim.SetInteger("MoveDir", 3);  //왼쪽 쳐다보도록
            else if((h>0) && anim.GetInteger("MoveDir")!=4)     // 중복 방지
                anim.SetInteger("MoveDir", 4);  //오른쪽 쳐다보도록

            // #33 fix: SphereCollider로 통제해서 balloonInFront 변수 필요 없음
            // if(balloonInFront)  // #33 fix: 앞에 물풍선 있는지 확인 - 있다면, 플레이어 이동 불가
            // {
            //     // Debug.Log("//#33 앞에 물풍선 있음");
            //     return;
            // }

            // #1 좌우 움직임 
                // maxSpeed에 아직 도달하지 않을때까지 플레이어 객체에 힘을 가해
                // h(-1.0f~1.0f)는 velocity.x를 다르게 표시한다
                if(h * rBody.velocity.x < maxSpeed)	// h가 음수이면-> rigidbody2d.velocity.x도 음수. // h가 양수이면-> 양수
                {
                    rBody.AddForce(Vector3.right * h * moveForce);	//오른쪽방향(1,0) * 방향 * 힘 <-> 왼쪽 방향이면 (-1, 0)
                    // Debug.Log("//#1 좌우 움직임");
                }    
            // #1 좌우 움직임 
                // 가속도해서 더래진 플레이어의 <<수평>> 속도가 maxSpeed 보다 커지면 maxSpeed로 속도 유지
                if(Mathf.Abs(rBody.velocity.x) > maxSpeed)  
                {	
                    //플레이어의 velocity(속도)를 x축방향으로 maxSpeed 로 셋팅해줘라 또한 기존 rigidbody2D.velocity.y 도 셋팅 해 줘야 한다.
                    // Mathf.Sign() 는 매개변수를 참조해서 1 또는 -1(float)을 반환  
                    rBody.velocity = new Vector2(Mathf.Sign(rBody.velocity.x) * maxSpeed, rBody.velocity.y);
                }
        }
        else
        {
            if((v>0) && anim.GetInteger("MoveDir")!=1 ) // #3
                anim.SetInteger("MoveDir", 1);  //위쪽 쳐다보도록
            else if((v<0) && anim.GetInteger("MoveDir")!=2 )
                anim.SetInteger("MoveDir", 2);  //아래쪽 쳐다보도록

            // #33 fix: SphereCollider로 통제해서 balloonInFront 변수 필요 없음
            // if(balloonInFront)  // #33 fix: 앞에 물풍선 있는지 확인 - 있다면, 플레이어 이동 불가
            // {
            //     Debug.Log("//#33 앞에 물풍선 있음");
            //     return;
            // }

            // #1 상하 움직임 
            if(v * rBody.velocity.y < maxSpeed)
            {
                rBody.AddForce(moveDirection * moveForce);
                // Debug.Log("//#1 상하 움직임");
            }

            // Debug.Log(moveDirection.magnitude);
                // rBody.AddForce(Vector2.up * v * moveForce);
                // 걱정: rigidBody2D인데 Vector3가 적용될까?

            // #1 상하 움직임 
            if(Mathf.Abs(rBody.velocity.y) > maxSpeed)
            {
                rBody.velocity = new Vector2(rBody.velocity.x, Mathf.Sign(rBody.velocity.y) * maxSpeed);
            }
        }

    }

    // public void PlayerStandsStill()         // #33 플레이어 제자리걸음 - 정수 좌표에 맞춰서
    // {
    //     pos = this.transform.position;
    //     pos.x = Mathf.RoundToInt(this.transform.position.x);
    //     pos.y = Mathf.RoundToInt(this.transform.position.y);
    //     this.transform.position = pos;
    // }

    // #33 fix: SphereCollider로 통제해서 코드 필요 없음
    // private void CheckObstacleBalloon()    // #33 플레이어가 가려고 하는 방향에 물풍선이 있는지 확인 - 있다면 물풍선 위로 못 지나가도록
    // {
    //     posX = Mathf.RoundToInt(transform.position.x);
    //     posY = Mathf.RoundToInt(transform.position.y);

    //     // if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
    //     // {
    //     //     Debug.Log("//#33 CheckObstacleBalloon 실행");
    //     // }

    //     if(Input.GetKey(KeyCode.UpArrow))   // 상하좌우 중 상
    //     {
    //         mapMgr.CheckIsThereWaterBalloon(posX, posY+1, MapManager.CHECK_TYPE.PLAYERMOVE);
    //     }
    //     if(Input.GetKey(KeyCode.DownArrow)) // 상하좌우 중 하
    //     {
    //         mapMgr.CheckIsThereWaterBalloon(posX, posY-1, MapManager.CHECK_TYPE.PLAYERMOVE);
    //     }
    //     if(Input.GetKey(KeyCode.LeftArrow)) // 상하좌우 중 좌
    //     {
    //         mapMgr.CheckIsThereWaterBalloon(posX-1, posY, MapManager.CHECK_TYPE.PLAYERMOVE);
    //     }
    //     if(Input.GetKey(KeyCode.RightArrow)) // 상하좌우 중 우
    //     {
    //         mapMgr.CheckIsThereWaterBalloon(posX+1, posY, MapManager.CHECK_TYPE.PLAYERMOVE);
    //     }
    // }

    // #6 fix: 플레이어가 덤불에 숨는 것처럼 보이도록 하기 위한 방법으로 Alpha를 설정하는 게 아닌, 오브젝트 자체를 비활성화 하는 방법을 채택해보자
    // void SetAlpha(SpriteRenderer _sprite, float _alpha) // #6 플레이어가 덤불 오브젝트에 가까이에 가면 안 보이도록
    // {
    //     Debug.Log("//#6 플레이어 sprite의 alpha 설정: " + _alpha);
    //     _sprite.color = new Color(1f, 1f, 1f, _alpha);
    // }

    private void ObjSetActive(GameObject _obj, bool _active)
    {
        Debug.Log("//#6 fix: "+ _obj + "를 활성화한다?: " + _active);
        _obj.SetActive(_active);
    }

    public void ChangePlayerSpeed(int rollerCount)   // #15 ROLLER 아이템 획득에 따라 플레이어 이동 속도 달라지도록
    {
        if(turtleMount) // #35 만약 플레이어가 거북에 타고 있었다면
        {
            LimitToTurtleSpeed();
            return;
        }

        // ROLLER 아이템 획득 개수에 따라 플레이어 이동 속도 설정
        // moveForce 디폴트 값: 30f, maxSpeed 디폴트 값: 5f
        moveForce = originMoveForce + (rollerCount) * 3;
        maxSpeed = originMaxSpeed + (rollerCount) * 0.3f;

        Debug.Log("//#15 플레이어 속도 증가. moveForce: " + moveForce + "| maxSpeed: " + maxSpeed);
    }

    public void SetPlayerSpeed(bool _down = true)   // #17 style: 함수 이름 변경
    {
        switch(_down)
        {
            case true:
                // #17 플레이어가 물풍선에 갇히면, 플레이어 이동 속도 느려지도록
                Debug.Log("//#17 플레이어 이동 속도 느려지도록");

                // #17 fix: 
                moveForce = originMoveForce - 10f;    // 7f - 2f 
                maxSpeed = originMaxSpeed - 0.6f;      // 2f - 1f;
                break;
            case false: 
            // #29 플레이어 본래 (디폴트) 속도로
            // - 플레이어 부활할 때

                // #29 fix
                moveForce = originMoveForce;    // 7f
                maxSpeed = originMaxSpeed;      // 2f      
                break;
        }

    }

    public void TurtleMount(bool _mount = true) // #35 플레이어가 거북에 올라탐
    {
        if(_mount)
        {
            turtleMount = true;
            
            Debug.Log("//#35 플레이어가 거북에 올라탐");

            LimitToTurtleSpeed();
            anim.SetBool("turtleMount", true);
        }
    }

    private void LimitToTurtleSpeed()   // #35 플레이어의 움직임 속도를 거북 탔을 때 속도로 설정
    {
        moveForce = turtleMountMoveForce;
        maxSpeed = turtleMountMaxSpeed;
    }
}
