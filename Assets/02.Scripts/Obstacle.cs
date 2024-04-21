using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public enum OBSTACLE_TYPE {WATERBALLOON = 1, BUSH, WOODBLOCK}    // #7 Obstacle마다 TYPE 설정하기   // #14 (WOODBLOCK)
    public OBSTACLE_TYPE obstacleType = OBSTACLE_TYPE.WATERBALLOON; // #7
    
    [SerializeField]    
    private Animator anim;  // #6 덤불 Animator 조정

    private MapManager mapMgr;             // #8 물풍선 지우기 위함
    private LayerSetting layerSetting;      // #2 WOODBLOCK 층 번호 설정
    private Transform playerTrans;          // #17 플레이어가 물줄기에 닿았는지 확인

    Vector3 playerTransform;  // #14 장애물과 부딪힌 플레이어의 위치
    Vector3 woodPos;          // #14 

    // [SerializeField]    
    // private int waterLength;    // #9 물풍선 터질 때 길이->  PlayerGameMgr.Mgr.fluid로 설정하면 되니까 필요 x.

    float pushForce = 10f;

    float xPosDiff;              // #14 WOODBLOCK 밀기: 장애물과 플레이어와의 x축 거리 차

    float yPosDiff;              // #14 WOODBLOCK 밀기: 장애물과 플레이어와의 y축 거리 차

    int row;                     // #14 row, col: WOODBLOCK 밀 때, 밀고자 하는 위치 좌표에 장애물 있는지 확인용
    int col;

    bool alreadyBurst = false;           // #31 물풍선이 터졌는지 확인 - 다른 물풍선에 의해 터지는 경우를 구분하기 위함


    void Awake()
    {
        mapMgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>(); // #8
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;   // #17 

        layerSetting = this.GetComponent<LayerSetting>();   // #2
        anim = transform.GetComponent<Animator>();    
        
    }
    
    void Start()
    {
        switch(obstacleType)    // #8 시간이 지남에 따라 물풍선 터짐
        {
            case OBSTACLE_TYPE.WATERBALLOON :
                StartCoroutine(WaterBalloonBursts(PlayerGameMgr.Mgr.fluid));    
                // waterLength = 1;    // #9 첫 물풍선 길이는 일단 1로 설정 // #9 fix: PlayerGameMgr.Mgr.fluid로 설정
                // #9 feat: 물풍선을 놓는 순간의 fluid 스킬 실력만큼 나중에 터지도록. - 3초(물풍선 터지는 딜레이 시간) 전에 물풍선 길이를 input해야 함.
                break;
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        Debug.Log("//#14 Obstacle 충돌 처리");

        switch(obstacleType)
        {
            case OBSTACLE_TYPE.WOODBLOCK:
                // #14 플레이어가 WoodBlock 밀 수 있도록
                if(other.gameObject.tag == "Player")
                {
                    // 플레이어가 미는 방향 확인
                    Debug.Log("//#14 플레이어가 " + obstacleType + " 밀고 있음");

                    playerTransform = other.gameObject.transform.position;
                    woodPos = this.gameObject.transform.position;

                    xPosDiff = woodPos.x - playerTransform.x;
                    yPosDiff = woodPos.y - playerTransform.y;

                    Debug.Log("yPosDiff: " + yPosDiff);
                    Debug.Log("xPosDiff: " + xPosDiff);

                    if(xPosDiff * xPosDiff < 0.25)  //  위 or 아래로 밀기: x축 간의 위치 차이가 적을 때만 실행되도록 - 차이가 클 때에는 미끄러지도록
                    {
                        if((yPosDiff <0) && (Input.GetKey(KeyCode.DownArrow)))  // 플레이어가 더 위에 있고 && 아래 방향키 누르고 있다면
                        {
                            if(IsThereObstacle(KeyCode.DownArrow) == true)  // #14
                                return;

                            Debug.Log("//#14 플레이어가 위에서 아래로 밀고 있음");
                            woodPos.y -=1;          // 우드블럭 위치 1칸씩 이동하기
                        }
                        else if((yPosDiff >0) && (Input.GetKey(KeyCode.UpArrow)))
                        {
                            if(IsThereObstacle(KeyCode.UpArrow) == true)  // #14
                                return;

                            Debug.Log("//#14 플레이어가 아래에서 위로 밀고 있음");
                            woodPos.y +=1;
                        }
                    }

                    if(yPosDiff * yPosDiff < 0.25)  // 좌 or 우로 밀기: x축 간의 위치 차이가 적을 때만 실행되도록 - 차이가 클 때에는 미끄러지도록
                    {
                        if((xPosDiff <0) && (Input.GetKey(KeyCode.LeftArrow)))
                        {
                            if(IsThereObstacle(KeyCode.LeftArrow) == true)  // #14
                                return;

                            Debug.Log("//#14 플레이어가 오른쪽에서 왼쪽으로 밀고 있음");
                            woodPos.x -=1;
                        }
                        else if((xPosDiff >0) && (Input.GetKey(KeyCode.RightArrow)))
                        {
                            if(IsThereObstacle(KeyCode.RightArrow) == true)  // #14
                                return;

                            Debug.Log("//#14 플레이어가 왼쪽에서 오른쪽으로 밀고 있음");
                            woodPos.x +=1;
                        }
                    }

                    this.gameObject.transform.position = woodPos; 
                    layerSetting.SetSortingOrder(); // #2 WOODBLOCK 위치 바뀔 때마다 Layer 번호 설정

                }
                break;

        }
    }

    private bool IsThereObstacle(KeyCode _arrow)    // #14 해당 위치에 Obstacle 있는지 확인
    {
        // #14 만약 밀고자 하는 위치에 이미 장애물이 있다면 밀리지 않도록
        switch(_arrow)
        {
            case KeyCode.DownArrow:
                row = mapMgr.ReturnRowInMatrix(woodPos.y-1);
                col = mapMgr.ReturnColInMatrix(woodPos.x);
 
                break;

            case KeyCode.UpArrow:
                row = mapMgr.ReturnRowInMatrix(woodPos.y+1);
                col = mapMgr.ReturnColInMatrix(woodPos.x);

                break;

            case KeyCode.LeftArrow:
                row = mapMgr.ReturnRowInMatrix(woodPos.y);
                col = mapMgr.ReturnColInMatrix(woodPos.x-1);

                break;

            case KeyCode.RightArrow:
                row = mapMgr.ReturnRowInMatrix(woodPos.y);
                col = mapMgr.ReturnColInMatrix(woodPos.x+1);

                break;
        }
        if((mapMgr.obstacleArr[row, col] ==1) || (row > 6) || (row < 0) || (col > 8) || (col < 0) )   
        {
            // #14 아예 배열 바운더리를 넘어가도 장애물 있는 것으로 판단
            return true;
        }
        else
            return false;
    }
         

    public void BushShake() // #6 애니메이터 설정: 플레이어가 덤불에 숨으면, 덤불 흔들리도록 
    {
        Debug.Log("//#6 덤불 흔들림");
        anim.SetTrigger("Shake");
    }

    public void StartWaterBalloonBursts(bool _burstedByAnoterBalloon = false)   // #31 Manager.cs에서 실행하기 위함
    {
        Debug.Log("//#31 WaterBalloonBursts 코루틴 실행");
        StartCoroutine(WaterBalloonBursts(PlayerGameMgr.Mgr.fluid, _burstedByAnoterBalloon));    
    }

    IEnumerator WaterBalloonBursts(int _waterLength, bool _burstedByAnoterBalloon = false)    // #8 3초 뒤에 해당 물풍선 파괴
    {
        if(_burstedByAnoterBalloon == true)
        {
            Debug.Log("//#31 다른 물풍선에 의해 터짐");
        }
        if(_burstedByAnoterBalloon == false)
        {
            Debug.Log("//#8 3초 기다림 시작");
            yield return new WaitForSeconds(3.0f);
        }

        // #31 3초 뒤에 자동으로 터지는 경우와 다른 물풍선에 의해 터지는 경우 둘 중 하나만 실행되도록 하기 위함
        if(alreadyBurst == false)   // #31 처음 물풍선이 터지는 거면, 그대로 아래 함수들 실행
            alreadyBurst = true;
        else                        
            yield break;
        
        Debug.Log("//#8 파괴 | 위치는" + transform.position.x + ", " + transform.position.y);
        anim.SetTrigger("Bursts");
        anim.SetInteger("WaterLength",  _waterLength);  // #9 물줄기 길이 설정

        mapMgr.RemoveWaterBalloon(this.transform.position.x, this.transform.position.y);


        // #17 플레이어가 물풍선의 물줄기와 닿았나 확인
        Debug.Log("//#17 플레이어의 좌표: " + playerTrans.position);
        mapMgr.CheckPlayerTouchFluid(this.transform, 
                                    playerTrans.position.x, playerTrans.position.y, _waterLength);
        // #31 물풍선의 물줄기가 다른 물풍선에 닿았나 확인
        mapMgr.CheckBubbleTouchFluid(this.transform, _waterLength); 
                                    
    }

    // public void DestroyWaterBalloon()   // #9 애니메이터 Clips에서 접근 및 실행 // #9 fix: Destructor.cs 스크립트 자체를 이용하도록
    // {
    //     Destroy(this.gameObject);    // #9 물풍선 파괴
    // }

}   
