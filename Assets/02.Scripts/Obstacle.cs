using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public enum OBSTACLE_TYPE {WATERBALLOON = 1, BUSH, WOODBLOCK, NORMALBLOCK, SUBWATERBALLOON}    // #7 Obstacle마다 TYPE 설정하기   // #14 (WOODBLOCK) // #39 NORMALBLOCK 
    // // #4 플레이어1과 플레이어2가 놓는 물풍선 구분을 위해 SUBWATERBALLOON 추가 (끝에 추가해야 OBSTACLE_TYPE 번호가 밀리는 문제를 막을 수 있음)
    public OBSTACLE_TYPE obstacleType = OBSTACLE_TYPE.WATERBALLOON; // #7 디폴트 obstacleType 설정
    private Item.ITEM_TYPE randomItemType = Item.ITEM_TYPE.FLUID;   // #38  (1번부터 5번까지)
    
    [SerializeField]    
    private Animator anim;  // #6 덤불 Animator 조정

    private MapManager mapMgr;             // #8 물풍선 지우기 위함
    private LayerSetting layerSetting;      // #2 WOODBLOCK 층 번호 설정
    private Transform playerTrans;          // #17 플레이어가 물줄기에 닿았는지 확인
    private SphereCollider sphereCollider;  // #33

    [SerializeField] private GameObject spriteObj;      // #60 해당 장애물의 spriteRenderer를 갖고 있는 게임오브젝트 가져오기. (하위 0번째 오브젝트)
    // #38 WoodBlock이 사라진 위치에 랜덤으로 아이템 놓기
    [SerializeField] private GameObject waterballoonObj; // #8 물풍선 터질 때, 물줄기 뿐만 아니라, 물풍선 자체에 있는 오브젝트도 물풍선 맞은 것으로 적용되어야 함.
        // So, 물풍선이 터지는 순간에, waterballoon 오브젝트의 Tag를 "WaterBurst"로 바꿔주기
    [SerializeField] private GameObject itemBubble; // #38
    [SerializeField] private GameObject itemCoin;   // #38
    [SerializeField] private GameObject itemFluid;  // #38
    [SerializeField] private GameObject itemRoller; // #38
    [SerializeField] private GameObject itemTurtle; // #38
    
    Vector3 playerTransform;  // #14 장애물과 부딪힌 플레이어의 위치
    Vector3 woodPos;          // #14 

    // [SerializeField]    
    // private int waterLength;    // #9 물풍선 터질 때 길이->  PlayerGameMgr.Mgr.fluid로 설정하면 되니까 필요 x.

    float pushForce = 10f;

    float xPosDiff;              // #14 WOODBLOCK 밀기: 장애물과 플레이어와의 x축 거리 차

    float yPosDiff;              // #14 WOODBLOCK 밀기: 장애물과 플레이어와의 y축 거리 차

    int row;                     // #14 row, col: WOODBLOCK 밀 때, 밀고자 하는 위치 좌표에 장애물 있는지 확인용
    int col;
    int obsRow;                 // #14 obsRow, obsCol: 현재 장애물의 위치를 행렬로 확인하기 위함 (장애물 이동하면 배열값도 바꿔야 하니까)
    int obsCol;
    int fluidLength;             // #8 물풍선 터지는 길이는 터지는 순간에 정해지는 게 아니라, 물풍선을 놓는 순간에 정해지도록
    int randomNumber;           // #38 WoodBlock이 가지는 랜덤 아이템 (0이면 아이템 없는 경우임)

    bool alreadyBurst = false;           // #31 물풍선이 터졌는지 확인 - 다른 물풍선에 의해 터지는 경우를 구분하기 위함


    void Awake()
    {
        mapMgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>(); // #8
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;   // #17 

        layerSetting = this.GetComponent<LayerSetting>();   // #2
        anim = transform.GetComponent<Animator>();    

        if(obstacleType == OBSTACLE_TYPE.WOODBLOCK)
            spriteObj = transform.GetChild(0).gameObject;   // #60
        if((obstacleType == OBSTACLE_TYPE.WATERBALLOON) || (obstacleType == OBSTACLE_TYPE.SUBWATERBALLOON))  // #33
        {
            sphereCollider = this.GetComponent<SphereCollider>();

            // # 물풍선 프리팹에서 waterballoonObj과 spriteObj은 완전히 동일한 GameObject를 지칭함.
            waterballoonObj = transform.GetChild(0).gameObject; // #8 물풍선 터질 때, 물줄기 뿐만 아니라, 물풍선 자체에 있는 오브젝트도 물풍선 맞은 것으로 적용되어야 함.
            // So, 물풍선이 터지는 순간에, waterballoon 오브젝트의 Tag를 "WaterBurst"로 바꿔주기

            spriteObj = transform.GetChild(0).gameObject;   // #60
        }
                
    }
    
    void Start()
    {
        switch(obstacleType)    // #8 시간이 지남에 따라 물풍선 터짐
        {
            case OBSTACLE_TYPE.WATERBALLOON :
                fluidLength = PlayerGameMgr.Mgr.fluid;
                StartCoroutine(WaterBalloonBursts(fluidLength, true));    
                // waterLength = 1;    // #9 첫 물풍선 길이는 일단 1로 설정 // #9 fix: PlayerGameMgr.Mgr.fluid로 설정
                // #9 feat: 물풍선을 놓는 순간의 fluid 스킬 실력만큼 나중에 터지도록. - 3초(물풍선 터지는 딜레이 시간) 전에 물풍선 길이를 input해야 함.
                break;
            case OBSTACLE_TYPE.SUBWATERBALLOON :
                fluidLength = SubPlayerGameMgr.SubMgr.fluid;
                StartCoroutine(WaterBalloonBursts(fluidLength, false));    
                // waterLength = 1;    // #9 첫 물풍선 길이는 일단 1로 설정 // #9 fix: SubPlayerGameMgr.SubMgr.fluid로 설정
                // #9 feat: 물풍선을 놓는 순간의 fluid 스킬 실력만큼 나중에 터지도록. - 3초(물풍선 터지는 딜레이 시간) 전에 물풍선 길이를 input해야 함.
                break;
                
            case OBSTACLE_TYPE.WOODBLOCK :
            case OBSTACLE_TYPE.NORMALBLOCK :    // #38 NORMALBLOCK도 (물풍선에 의해 없어질 때) 랜덤 아이템 생기도록 설정
                randomNumber = Random.Range(0, 100);  // #38 fix: 0부터 99까지의 랜덤 숫자 생성 (아이템이 없는 경우도 포함)
                
                // if(randomNumber != 0)   // #38 fix 랜덤 아이템이 없는 경우는 배제한 if문 (randomNumber == 0 이면, 아이템이 없는 경우임)
                //     randomItemType = (Item.ITEM_TYPE)randomNumber;                // #38 각 item별로 등장 확률 설정

                // 아이템 없는 경우가 40%
                // 나머지 ITEM_TYPE 5가지가 각각 12(=60/5)%
                if(randomNumber<40)
                    // break;
                    randomItemType = Item.ITEM_TYPE.NONE;   // 아무것도 지정해주지 않으니까, 디폴트인 FLUID로 설정되는 문제가 발생함.
                else if((randomNumber>=40) && (randomNumber<52))
                    randomItemType = Item.ITEM_TYPE.FLUID;  // 10%의 확률로 FLUID 아이템이 설정되도록
                else if((randomNumber>=52) && (randomNumber<64))
                    randomItemType = Item.ITEM_TYPE.BUBBLE;  // 10%의 확률로 BUBBLE 아이템이 설정되도록
                else if((randomNumber>=64) && (randomNumber<76))
                    randomItemType = Item.ITEM_TYPE.ROLLER;  // 10%의 확률로 ROLLER 아이템이 설정되도록
                else if((randomNumber>=76) && (randomNumber<88))
                    randomItemType = Item.ITEM_TYPE.TURTLE;  // 10%의 확률로 TURTLE 아이템이 설정되도록
                else if((randomNumber>=88) && (randomNumber<100))
                    randomItemType = Item.ITEM_TYPE.COIN;  // 10%의 확률로 COIN 아이템이 설정되도록


                Debug.Log("//#38 randomNumber: " + randomNumber);
                Debug.Log("//#38 randomItemType: " + randomItemType);
                break;
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        // Debug.Log("//#14 Obstacle 충돌 처리| 주체: " + this.gameObject.name + "상대:" + other.gameObject.name);

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


                            // #14 fix: WoodBlock 위치 바뀌면 배열 값도 바꾸도록 ================
                            obsRow = mapMgr.ReturnRowInMatrix(woodPos.y);
                            obsCol = mapMgr.ReturnColInMatrix(woodPos.x);
                            mapMgr.blockArr[obsRow, obsCol] = 0; // #14 fix: 위치 바뀌었으니까 이전 위치의 배열값을 0으로 설정  // #40 fix: obstacleArr이 아닌 blockArr 값을 변경

                            Debug.Log("//#14 플레이어가 위에서 아래로 밀고 있음");
                            woodPos.y -=1;          // 우드블럭 위치 1칸씩 이동하기

                            obsRow = mapMgr.ReturnRowInMatrix(woodPos.y);
                            mapMgr.blockArr[obsRow, obsCol] = 1; // #14 fix: 위치 바뀌었으니까 새로운 위치의 배열값을 1로 설정  // #40 fix: obstacleArr이 아닌 blockArr 값을 변경

                        }
                        else if((yPosDiff >0) && (Input.GetKey(KeyCode.UpArrow)))
                        {
                            if(IsThereObstacle(KeyCode.UpArrow) == true)  // #14
                                return;

                            // #14 fix: WoodBlock 위치 바뀌면 배열 값도 바꾸도록 ================
                            obsRow = mapMgr.ReturnRowInMatrix(woodPos.y);
                            obsCol = mapMgr.ReturnColInMatrix(woodPos.x);
                            mapMgr.blockArr[obsRow, obsCol] = 0; // #14 fix: 위치 바뀌었으니까 이전 위치의 배열값을 0으로 설정  // #40 fix: obstacleArr이 아닌 blockArr 값을 변경

                            Debug.Log("//#14 플레이어가 아래에서 위로 밀고 있음");
                            woodPos.y +=1;

                            obsRow = mapMgr.ReturnRowInMatrix(woodPos.y);
                            mapMgr.blockArr[obsRow, obsCol] = 1; // #14 fix: 위치 바뀌었으니까 새로운 위치의 배열값을 1로 설정  // #40 fix: obstacleArr이 아닌 blockArr 값을 변경
                        }
                    }

                    if(yPosDiff * yPosDiff < 0.25)  // 좌 or 우로 밀기: y축 간의 위치 차이가 적을 때만 실행되도록 - 차이가 클 때에는 미끄러지도록
                    {
                        Debug.Log("//#14 WoodBlock 밀고 있음 -2");
                        if((xPosDiff <0) && (Input.GetKey(KeyCode.LeftArrow)))
                        {
                            if(IsThereObstacle(KeyCode.LeftArrow) == true)  // #14
                                return;

                            // #14 fix: WoodBlock 위치 바뀌면 배열 값도 바꾸도록 ================
                            obsRow = mapMgr.ReturnRowInMatrix(woodPos.y);
                            obsCol = mapMgr.ReturnColInMatrix(woodPos.x);
                            mapMgr.blockArr[obsRow, obsCol] = 0; // #14 fix: 위치 바뀌었으니까 이전 위치의 배열값을 0으로 설정  // #40 fix: obstacleArr이 아닌 blockArr 값을 변경

                            Debug.Log("//#14 플레이어가 오른쪽에서 왼쪽으로 밀고 있음");
                            woodPos.x -=1;
                            
                            obsCol = mapMgr.ReturnColInMatrix(woodPos.x);
                            mapMgr.blockArr[obsRow, obsCol] = 1; // #14 fix: 위치 바뀌었으니까 새로운 위치의 배열값을 1로 설정  // #40 fix: obstacleArr이 아닌 blockArr 값을 변경
                        }
                        else if((xPosDiff >0) && (Input.GetKey(KeyCode.RightArrow)))
                        {
                            if(IsThereObstacle(KeyCode.RightArrow) == true)  // #14
                                return;

                            // #14 fix: WoodBlock 위치 바뀌면 배열 값도 바꾸도록 ================
                            obsRow = mapMgr.ReturnRowInMatrix(woodPos.y);
                            obsCol = mapMgr.ReturnColInMatrix(woodPos.x);
                            mapMgr.blockArr[obsRow, obsCol] = 0; // #14 fix: 위치 바뀌었으니까 이전 위치의 배열값을 0으로 설정  // #40 fix: obstacleArr이 아닌 blockArr 값을 변경

                            Debug.Log("//#14 플레이어가 왼쪽에서 오른쪽으로 밀고 있음");
                            woodPos.x +=1;

                            obsCol = mapMgr.ReturnColInMatrix(woodPos.x);
                            mapMgr.blockArr[obsRow, obsCol] = 1; // #14 fix: 위치 바뀌었으니까 새로운 위치의 배열값을 1로 설정  // #40 fix: obstacleArr이 아닌 blockArr 값을 변경
                        }
                    }

                    this.gameObject.transform.position = woodPos; 
                    layerSetting.SetSortingOrder(); // #2 WOODBLOCK 위치 바뀔 때마다 Layer 번호 설정

                }
                break;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("//#31 OnTriggerEnter 작동: " + "주체: " + this.gameObject.name + " | 상대: " + other.gameObject.name);
        
        if(other.gameObject.tag == "WaterBurst")
        {
            switch(obstacleType)
            {
                case OBSTACLE_TYPE.WATERBALLOON:
                    // #31 물풍선이 다른 물풍선의 물줄기에 맞았는지 확인 - 만약 맞았다면, 이 물풍선도 터지도록
                    // #31 fix: 자꾸 Trigger 처리를 인식하지 못하는 문제 - 물풍선에 Rigidbody가 없어서 그랬음. 충돌하는 두 물체 중 하나라도 rigidbody가 있어야 인식함 
                    Debug.Log("//#31 플레이어1이 놓은 물풍선이 다른 물줄기에 맞음");
                    StartWaterBalloonBursts(true, true);
                    break;
                case OBSTACLE_TYPE.SUBWATERBALLOON:
                    // #31 물풍선이 다른 물풍선의 물줄기에 맞았는지 확인 - 만약 맞았다면, 이 물풍선도 터지도록
                    // #31 fix: 자꾸 Trigger 처리를 인식하지 못하는 문제 - 물풍선에 Rigidbody가 없어서 그랬음. 충돌하는 두 물체 중 하나라도 rigidbody가 있어야 인식함 
                    Debug.Log("//#31 플레이어2가 놓은 물풍선이 다른 물줄기에 맞음");
                    StartWaterBalloonBursts(true, false);
                    break;
                case OBSTACLE_TYPE.BUSH:
                    Debug.Log("//#36 물풍선이 Bush에 맞음");
                    // DestroyObstacle();
                    DestroyBush();  // #36 fix: Obstacle이 아닌 Bush의 배열 값을 0으로 설정하도록
                    break;
                case OBSTACLE_TYPE.WOODBLOCK:
                    Debug.Log("//#38 물풍선이 WoodBlock에 맞음");
                    // DestroyObstacle();
                    DestroyBlock(); // #38 fix: DestroyObstacle(); 대신 DestroyBlock(); 함수 사용
                    // PlaceRandomItem();  // #38 WoodBlock이 사라진 자리에 랜덤으로 아이템 생기도록
                    // #38 fix: (아이템이 생기자마자 블록을 깼던 물풍선에 의해 바로 사라지는 것을 방지하기 위함) 시간 term을 두고 랜덤 아이템이 생기도록
                    mapMgr.PlaceRandomItem(randomItemType, this.transform.position); 

                    break;

                case OBSTACLE_TYPE.NORMALBLOCK:
                    Debug.Log("//#39 물풍선이 NormalBlock에 맞음");
                    DestroyBlock(); // #39 fix: DestroyObstacle(); 대신 DestroyBlock(); 함수 사용
                    // PlaceRandomItem();  // #38 NORMALBLOCK이 사라진 자리에 랜덤으로 아이템 생기도록
                    // #38 fix: (아이템이 생기자마자 블록을 깼던 물풍선에 의해 바로 사라지는 것을 방지하기 위함) 시간 term을 두고 랜덤 아이템이 생기도록
                    mapMgr.PlaceRandomItem(randomItemType, this.transform.position); 

                    break;
            }
        }

        if(other.gameObject.tag == "Bush")  // #60 장애물(WOODBLOCK만 해당)이 Bush 안으로 들어가면 눈에 보이지 않도록 SpriteRenderer로 이루어진 오브젝트를 비활성화
        {
            if((obstacleType == OBSTACLE_TYPE.WOODBLOCK) || (obstacleType == OBSTACLE_TYPE.WATERBALLOON) || (obstacleType == OBSTACLE_TYPE.SUBWATERBALLOON))
                ObjSetActive(spriteObj, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            switch(obstacleType)
            {
                case OBSTACLE_TYPE.WATERBALLOON:
                case OBSTACLE_TYPE.SUBWATERBALLOON:
                    // #33 BoxCollider와 반응 - 
                    // 플레이어가 물풍선을 놓은 후, 물풍선에서 벗어나면 그제서야 물풍선의 Sphere Collider가 활성화되도록 
                    // - 플레이어가 물풍선을 놓은 후, 콜라이더 처리 때문에 물풍선을 벗어나지 못하는 문제를 방지하기 위해
                    sphereCollider.enabled = true;
                    break;
            }
        }

    }

    private void ObjSetActive(GameObject _obj, bool _active)
    {
        Debug.Log("//#60 fix: "+ _obj + "를 활성화?: " + _active);
        _obj.SetActive(_active);
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
        if((row > 6) || (row < 0) || (col > 8) || (col < 0) )   
        {
            // #14 아예 배열 바운더리를 넘어가도 장애물 있는 것으로 판단
            return true;
        }
        else if((mapMgr.obstacleArr[row, col] ==1) || (mapMgr.itemArr[row, col] ==1)
         || (mapMgr.waterBalloonArr[row, col]==1) || (mapMgr.blockArr[row, col]==1))    
         // #14 fix: 고려되지 않았던 blockArr도 파악한 후, WOODBLOCK 밀기
        {
            // IndexOutOfRangeException 에러를 피하기 위해 바운더리 넘어간 것부터 확인 후, 배열 확인
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

    public void StartWaterBalloonBursts(bool _burstedByAnoterBalloon = false, bool _player1 = true)   // #31 Manager.cs에서 실행하기 위함
    {
        Debug.Log("//#31 WaterBalloonBursts 코루틴 실행");
        StartCoroutine(WaterBalloonBursts(fluidLength, _burstedByAnoterBalloon, _player1));    
    }

    IEnumerator WaterBalloonBursts(int _waterLength, bool _burstedByAnoterBalloon = false, bool _player1 = true)    // #8 3초 뒤에 해당 물풍선 파괴
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
        else                        // #31 이미 alreadyBurst 값이 true이면, 아래 함수들 실행 X        
            yield break;
        
        // #8 물풍선 터질 때, 물줄기 뿐만 아니라, 물풍선 자체에 있는 오브젝트도 물풍선 맞은 것으로 적용되어야 함.
        // So, 물풍선이 터지는 순간에, waterballoon 오브젝트의 Tag를 "WaterBurst"로 바꿔주기
        waterballoonObj.tag = "WaterBurst";

        //#60 fix: 물풍선이 Bush 같은 것에 의해 가려져 있었다면(비활성화 되어 있었다면), 다시 활성화해주기.
        if(!spriteObj.activeSelf)
            spriteObj.SetActive(true);

        //#8 fix: 태그가 인식되기 전에 물풍선이 먼저 사라져 버려서 Bush가 사라지지 않는 경우가 존재함.
        // 직접 위치를 받아와서 배열 값을 확인한 뒤에, 해당 배열 값에 Bush가 있으면 직접 Destroy 하는 방법 적용.
        mapMgr.CheckBubbleInBush(this.transform);

        Debug.Log("//#8 파괴 | 위치는" + transform.position.x + ", " + transform.position.y);
        anim.SetTrigger("Bursts");
        if(_waterLength > 2)    //#9 fix: 물줄기는 최대 2칸으로 제한 - 물풍선의 waterLength가 2보다 더 크게 설정된 경우의 애니메이션을 설정하지 않아서, 물풍선 자체가 안 터지는 문제를 해결
            _waterLength = 2;
        anim.SetInteger("WaterLength",  _waterLength);  // #9 물줄기 길이 설정

        mapMgr.RemoveWaterBalloon(this.transform.position.x, this.transform.position.y, _player1);  // #4 어떤 플레이어가 놓은 물풍선이 터지는 건지 구분


        // #17 플레이어가 물풍선의 물줄기와 닿았나 확인 // #17 fix: 콜라이더 Trigger 처리로 확인하기
        // Debug.Log("//#17 플레이어의 좌표: " + playerTrans.position);
        // mapMgr.CheckPlayerTouchFluid(this.transform, 
        //                             playerTrans, _waterLength);

        // #31 물풍선의 물줄기가 다른 물풍선에 닿았나 확인
        // mapMgr.CheckBubbleTouchFluid(this.transform, _waterLength); 

        // #34 물풍선의 물줄기가 아이템에 닿았나 확인 - 닿았으면 아이템 Destroy
        // mapMgr.CheckItemTouchFluid(this.transform, _waterLength);

        // #36 물풍선의 물줄기가 덤불(Bush)에 닿았나 확인 - 닿았으면 Bush Destroy
        // mapMgr.CheckBushTouchFluid(this.transform, _waterLength);
                                    
    }

    // public void DestroyWaterBalloon()   // #9 애니메이터 Clips에서 접근 및 실행 // #9 fix: Destructor.cs 스크립트 자체를 이용하도록
    // {
    //     Destroy(this.gameObject);    // #9 물풍선 파괴
    // }
    public void DestroyObstacle()   // #36
    {
        Debug.Log("//#36: " + this.gameObject.name + " 장애물(Obstacle) 삭제");
        Destroy(this.gameObject);

        mapMgr.RemoveObsPos(this.transform);    // #36 해당 위치의 obstacleArr 배열값을 0으로 설정
    }

    public void DestroyBlock()  // #39 fix
    {
        Debug.Log("//#39: " + this.gameObject.name + " 블록(Block) 삭제");
        Destroy(this.gameObject);

        mapMgr.RemoveBlockPos(this.transform);    // #39 해당 위치의 blockArr 배열값을 0으로 설정
    }

    public void DestroyBush()   // #36 fix: Obstacle이 아닌 Bush의 배열 값을 0으로 설정하도록
    {
        Debug.Log("//#36: " + this.gameObject.name + " 덤불(Bush) 삭제");
        Destroy(this.gameObject);

        mapMgr.RemoveBushPos(this.transform);   // #36 해당 위치의 bushArr 배열 값을 0으로 설정
    }

    // private void PlaceRandomItem()  // #38 물풍선을 맞은 WOODBLOCK 또는 NORMALBLOCK이 사라진 자리에 랜덤으로 아이템 놓기
    // {
    //     Debug.Log("//#38 fix: PlaceRandomItem함수 실행");
    //     if(randomNumber ==0)    // #38 fix: randomNumber가 0이면 랜덤 아이템이 없는 것 (아이템 생성 없이 return)
    //         return;

    //     Debug.Log("//#38 WOODBLOCK 또는 NORMALBLOCK 이 사라진 자리에 랜덤 아이템 배치");

    //     switch(randomItemType)
    //     {
    //         case Item.ITEM_TYPE.FLUID:
    //             Instantiate(itemFluid, this.transform.position, Quaternion.identity);   
    //             break;
    //         case Item.ITEM_TYPE.BUBBLE:
    //             Instantiate(itemBubble, this.transform.position, Quaternion.identity);
    //             break;
    //         case Item.ITEM_TYPE.COIN:
    //             Instantiate(itemCoin, this.transform.position, Quaternion.identity);
    //             break;
    //         case Item.ITEM_TYPE.TURTLE:
    //             Instantiate(itemTurtle, this.transform.position, Quaternion.identity);
    //             break;
    //         case Item.ITEM_TYPE.ROLLER:
    //             Instantiate(itemRoller, this.transform.position, Quaternion.identity);
    //             break;
            
    //     }
    // }
}   
