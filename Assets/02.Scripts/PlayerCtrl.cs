using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour // #1 
{
// #1 플레이어 기본 이동 =============================
    [SerializeField]
    private bool dirRight = false;         // 플레이어가 바라보는 방향(오른쪽 : 1, 왼쪽 : -1)

    private float moveSpeed = 30f;         // 이동 속도 (50 > 20)
    private float maxSpeed = 5f;
    private float h;
    private float v;

    private Rigidbody rBody;               // 2D에서 3D로 변경
    private SpriteRenderer sprite;                  // #2 플레이어 위치에 따라 오브젝트 앞에 or 뒤에 그려지도록 

    private Animator anim;                 // #3 플레이어 애니메이터

    void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();  // #2
        anim = GetComponent<Animator>();    // #3
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            PlayerMove(true);
        else if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            PlayerMove(false);

        // #2 y축 기준으로 밑에 있을수록 더 위에 그려져야 하므로 반비례 -> -1
        sprite.sortingOrder = - Mathf.RoundToInt(transform.position.y); 
    }


    void FixedUpdate()
    {   

// #1 플레이어 움직이기 ===============================

        h = Input.GetAxis("Horizontal");  // 좌우 키
        v = Input.GetAxis("Vertical");    // 상하 키

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
            rBody.AddForce(Vector3.right * h * moveSpeed);	//오른쪽방향(1,0) * 방향 * 힘 <-> 왼쪽 방향이면 (-1, 0)

    // #1 상하 움직임 
        if(v * rBody.velocity.y < maxSpeed)
            rBody.AddForce(moveDirection * moveSpeed);

        Debug.Log(moveDirection.magnitude);
            // rBody.AddForce(Vector2.up * v * moveSpeed);
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

    // void Flip() // #1 플레이어 바라보는 방향에 따라 적용  --> // #3 애니메이터로 조작해서 Flip 기능 필요 없어짐
    // {
    //     Debug.Log("//#2 플레이어 뒤집어");

    //     dirRight = !dirRight;   //바라보는 방향 변경

    //     Vector3 theScale = transform.localScale;
    //     theScale.x *= -1;
    //     transform.localScale = theScale;
    // }

    void PlayerMove(bool moveHorizontal)
    {
        Vector3 moveDirection = new Vector3(0, v);

        if(moveHorizontal)
        {
            if((h<0) && anim.GetInteger("MoveDir")!=0 ) // #3   // 중복 방지 - 이미 0인 값을 또 0이라 설정하지 않도록 
                anim.SetInteger("MoveDir", 0);  //왼쪽 쳐다보도록
            else if((h>0) && anim.GetInteger("MoveDir")!=1)     // 중복 방지
                anim.SetInteger("MoveDir", 1);  //오른쪽 쳐다보도록

            // #1 좌우 움직임 
                // maxSpeed에 아직 도달하지 않을때까지 플레이어 객체에 힘을 가해
                // h(-1.0f~1.0f)는 velocity.x를 다르게 표시한다
                if(h * rBody.velocity.x < maxSpeed)	// h가 음수이면-> rigidbody2d.velocity.x도 음수. // h가 양수이면-> 양수
                    rBody.AddForce(Vector3.right * h * moveSpeed);	//오른쪽방향(1,0) * 방향 * 힘 <-> 왼쪽 방향이면 (-1, 0)
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
            if((v>0) && anim.GetInteger("MoveDir")!=2 ) // #3
                anim.SetInteger("MoveDir", 2);  //위쪽 쳐다보도록
            else if((v<0) && anim.GetInteger("MoveDir")!=3 )
                anim.SetInteger("MoveDir", 3);  //아래쪽 쳐다보도록

            // #1 상하 움직임 
                if(v * rBody.velocity.y < maxSpeed)
                    rBody.AddForce(moveDirection * moveSpeed);

                // Debug.Log(moveDirection.magnitude);
                    // rBody.AddForce(Vector2.up * v * moveSpeed);
                    // 걱정: rigidBody2D인데 Vector3가 적용될까?

            // #1 상하 움직임 
                if(Mathf.Abs(rBody.velocity.y) > maxSpeed)
                {
                    rBody.velocity = new Vector2(rBody.velocity.x, Mathf.Sign(rBody.velocity.y) * maxSpeed);
                }
        }



    }


}
