using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public enum CHECK_TYPE {BALLOONBURST = 1, PLAYERMOVE}   // #33 물풍선 확인하는 TYPE 설정하기
    public CHECK_TYPE checkType = CHECK_TYPE.BALLOONBURST;    // #33 
    [SerializeField] private GameObject waterBalloonObj;  // 배치할 물풍선 프리팹
    private GameObject[] obstacles;     // #25
    private GameObject[] items;         // #34
    private GameObject[] bushes;        // #25 Bush가 있는 곳에는 obstacleArr배열 값을 1로 설정해서 - 물풍선 놓을 수 없도록

    // #28 플레이어가 죽으면 아이템 뱉도록 - 아이템 프리팹 넣어놓기
    [Header("Item Prefabs")]
    [SerializeField] private GameObject fluidItemPrefab;
    [SerializeField] private GameObject bubbleItemPrefab;
    [SerializeField] private GameObject rollerItemPrefab;
    [SerializeField] private GameObject turtleItemPrefab;
    [SerializeField] private GameObject coinItemPrefab;

    private Vector3 balloonPos;         // #4 물풍선 배치 위치
    private Vector3 itemPos;            // #28 아이템 배치 위치

    private int waterballoonPlaceNum = 0;   // #13 맵에 놓여진 물풍선의 개수

    int balloonRow, balloonCol;   // #4 선언 위치만 바꿈
    int obsRow, obsCol;           // #4 장애물 전용 row, col
    int itemRow, itemCol;         // #4 아이템 전용 row, col
    int playerRow, playerCol;   // #17
    int rowNum = 4; // #25
    int colNum = 3; // #25
    int checkNum = 0;   // # 33 fix: 특정 위치에 물풍선 있는지, 몇 개 있는지 확인하는 변수

    // [SerializeField]
    int[,] waterBalloonArr =            // #4 7행 9열 이차원 배열 - 0행 0열부터 시작
    {
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    // #25 obstacleArr은 waterBalloonArr을 포함하고 있음 - 즉, waterBalloonArr 배열값이 1인 행열은 obstacleArr 배열값에서도 1임
    public int[,] obstacleArr =                // #25 장애물 배열 - 7행 9열 이차원 배열
    {
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    public int[,] itemArr =                    // #25 아이템 배열 - 7행 9열 이차원 배열
    {
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    private Music music; 
    private PlayerLife playerLife;   
    private PlayerCtrl playerCtrl;                  // #33


    private void Awake()
    {
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>(); // #21
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>(); // #4
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>(); // #4 
    }

    private void Start()
    {
        waterballoonPlaceNum = 0;   // #13 맵에 놓여진 물풍선의 개수
        CheckObstaclePos();         // #25 장애물 위치 - 배열로 확인
        Debug.Log("//#25 Start");

    }

    public int ReturnRowInMatrix(float _y)  // #26 Transform의 y좌표를 바탕으로 배열의 row(행) 값 얻기
    {
        return ( -Mathf.RoundToInt(_y) + 3);
    }
    public int ReturnColInMatrix(float _x)  // #26 Transform의 x좌표를 바탕으로 배열의 col(열) 값 얻기
    {
        return (Mathf.RoundToInt(_x) + 4);
    }

    public int ConvertRowToYCoordinate(int _row)
    {
        return (-(_row)+3);
    }
        public int ConvertColToXCoordinate(int _col)
    {
        return (_col -4);
    }

    private void CheckObstaclePos() // #25 장애물 위치 - 배열 확인 -> Start에서 1번 실행
    {
        Debug.Log("//#25 CheckObstaclePos");
        // #25 Obstacle 태그를 가진 모든 오브젝트에 접근
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        items = GameObject.FindGameObjectsWithTag("Item");
        bushes = GameObject.FindGameObjectsWithTag("Bush");

        // #25 
        for(int i=0; i<obstacles.Length; i++)
        {
            obsRow = ReturnRowInMatrix(obstacles[i].transform.position.y);     // #26 함수 이용
            obsCol =  ReturnColInMatrix(obstacles[i].transform.position.x);    // #26 함수 이용
            Debug.Log("//#25 Obstacle(Bush 제외) 존재 - obsRow: " + obsRow + ", obsCol: " + obsCol);
            Debug.Log("//#25 Ostacle(Bush 제외) 총 몇 개= " + (i + 1) + "오브젝트 이름: " + obstacles[i].gameObject.name);
            obstacleArr[obsRow, obsCol] = 1;  // #25 

        }

        //#25 ObstacleArr 배열
        // Bush가 있는 곳에는 obstacleArr배열 값을 1로 설정해서 - 물풍선 놓을 수 없도록
        for(int i=0; i<bushes.Length; i++)
        {
            obsRow = ReturnRowInMatrix(bushes[i].transform.position.y);     // #26 함수 이용
            obsCol =  ReturnColInMatrix(bushes[i].transform.position.x);    // #26 함수 이용
            Debug.Log("//#25 Obstacle(Bush만) 존재 - obsRow: " + obsRow + ", obsCol: " + obsCol);
            Debug.Log("//#25 Ostacle(Bush만) 총 몇 개= " + (i + 1) + "오브젝트 이름: " + bushes[i].gameObject.name);

            obstacleArr[obsRow, obsCol] = 1;
        }

        // #25 아이템 배열
        for(int i=0; i<items.Length; i++)
        {
            itemRow = ReturnRowInMatrix(items[i].transform.position.y);     // #26 함수 이용
            itemCol =  ReturnColInMatrix(items[i].transform.position.x);    // #26 함수 이용
            Debug.Log("//#25 Obstacle 존재 - itemRow: " + itemRow + ", itemCol: " + itemCol);
            Debug.Log("//#25 Ostacle 총 몇 개= " + (i + 1) + "오브젝트 이름: " + items[i].gameObject.name);
            itemArr[itemRow, itemCol] = 1;  // #25 

        }
    }
    
    public void RemoveItemPos(Transform _trans) // #10 아이템 획득하면 - 해당 아이템의 ItemArr을 0으로 전황
    {
        itemRow = ReturnRowInMatrix(_trans.position.y);     
        itemCol =  ReturnColInMatrix(_trans.position.x);    
        Debug.Log("// #10 아이템 획득 - 해당 위치 배열을 0으로 전환");
        itemArr[itemRow, itemCol] = 0;  // #10 
    }

    public void PlaceWaterBalloon(float _x, float _y)    // #4 플레이어 물풍선 놓기 - 받아오는 parameter는 플레이어의 좌표
    {
        if(playerLife.trappedInWater)   // #4 플레이어가 물풍선에 갇혀 있다면, PlaceWaterBalloon 실행되지 않도록
            return;
        Debug.Log("//#4 fix | 놓여진 물풍선 수: " + waterballoonPlaceNum + ", 놓을 수 있는 물풍선 수: " + PlayerGameMgr.Mgr.waterballoonNum);
        if(waterballoonPlaceNum >= PlayerGameMgr.Mgr.waterballoonNum)    //#13 물풍선 개수 제한
            return;

        playerRow = ReturnRowInMatrix(_y);    // #26 함수 이용 
        playerCol = ReturnColInMatrix(_x);    // #26 함수 이용

        Debug.Log("//#8 #4 물풍선 생성: balloonRow = " + playerRow + "/ balloonCol = " + playerCol);

        if ((waterBalloonArr[playerRow, playerCol] == 1) || (obstacleArr[playerRow, playerCol] == 1)) 
        {
            // 이미 그 자리에 물풍선 or 장애물이 있다면 - 중복해서 물풍선을 놓지 못하도록
            // Debug.Log("//#4 fix: 물풍선 이미 있음");
            // Debug.Log("//#4 fix 장애물 이미 있음");
            return;
        }

        // 위 조건들 모두 만족하면, 물풍선 놓기

        waterBalloonArr[playerRow, playerCol] = 1;  // 배열 설정
        obstacleArr[playerRow, playerCol] = 1;      // #25 배열 설정 - 물풍선이 놓여지면 obstacleArr 배열값도 1로 설정

        // 물풍선 놓기
        balloonPos.x = Mathf.RoundToInt(_x);    
        balloonPos.y = Mathf.RoundToInt(_y);
        Instantiate(waterBalloonObj, balloonPos, Quaternion.identity);

        waterballoonPlaceNum += 1; // #13 물풍선 개수 하나 증가
        music.SoundEffect(Music.EFFECT_TYPE.BOMB_SET); // #21 물풍선 놓을 때의 효과음

        Debug.Log("//#13 물풍선 개수: " + waterballoonPlaceNum);
    }

    public void PlaceItemPrefab(Item.ITEM_TYPE _type, Vector3 _pos) // #28 맵 위에 특정 위치에 아이템 배치하기
    {
        // 아이템 배치 위치 지정
        itemPos = _pos;

        switch(_type)
        {
            case Item.ITEM_TYPE.FLUID:
                Instantiate(fluidItemPrefab, itemPos, Quaternion.identity);
                break;

            case Item.ITEM_TYPE.BUBBLE:
                Instantiate(bubbleItemPrefab, itemPos, Quaternion.identity);
                break;

            case Item.ITEM_TYPE.ROLLER:
                Instantiate(rollerItemPrefab, itemPos, Quaternion.identity);
                break;

            case Item.ITEM_TYPE.TURTLE:
                Instantiate(turtleItemPrefab, itemPos, Quaternion.identity);
                break;

            case Item.ITEM_TYPE.COIN:
                Instantiate(coinItemPrefab, itemPos, Quaternion.identity);
                break;
        }
    }
    public void RemoveWaterBalloon(float _x, float _y)  // #8 시간이 지남에 따라 물풍선 터짐 - 받아오는 parameter는 물풍선의 좌표
    {
        balloonRow = ReturnRowInMatrix(_y);
        balloonCol = ReturnColInMatrix(_x);

        Debug.Log("//#8 //#31 fix: 물풍선 소멸: balloonRow = " + balloonRow + "/ balloonCol = " + balloonCol);

        waterBalloonArr[balloonRow, balloonCol] = 0;  // 배열 설정
        obstacleArr[balloonRow, balloonCol] = 0;      // #25 배열 설정

        waterballoonPlaceNum -= 1; // #13 물풍선 개수 하나 감소
        music.SoundEffect(Music.EFFECT_TYPE.BUBBLE_BOOM); // #21 물풍선 터질 때의 효과음

        Debug.Log("//#13 물풍선 개수: " + waterballoonPlaceNum);

    }

    public void CheckPlayerTouchFluid(Transform _balloonTrans, Transform _playerTrans, int _waterLength)   
    // #17 플레이어가 물풍선의 물줄기와 닿았나 확인
    {
        balloonRow = ReturnRowInMatrix(_balloonTrans.position.y);
        balloonCol = ReturnColInMatrix(_balloonTrans.position.x);

        playerRow = ReturnRowInMatrix(_playerTrans.position.y);
        playerCol = ReturnColInMatrix(_playerTrans.position.x);

        // 만약 플레이어가 터지는 물풍선과 같은 행이라면
        // && 플레이어와 물풍선과의 거리가 _waterLength보다 가깝다면
        if((balloonRow == playerRow) && 
            ((playerCol - balloonCol)*(playerCol - balloonCol) <= (_waterLength)*(_waterLength)))
        {
            Debug.Log("//#17 같은 행 - 플레이어가 물줄기에 닿음");
            Debug.Log("//#17 물줄기 길이: " + _waterLength);
            Debug.Log("//#17 물줄기와 플레이어 간의 거리: " + (playerCol-balloonCol));

            playerLife.PlayerInWaterBalloon(); // #17 플레이어 물풍선에 갇힘
        }

        // 만약 플레이어가 터지는 물풍선과 같은 열이라면
        // && 플레이어와 물풍선과의 거리가 _waterLength보다 가깝다면
        if((balloonCol == playerCol) &&
            ((playerRow-balloonRow)*(playerRow-balloonRow) <= (_waterLength)*(_waterLength)))
        {
            Debug.Log("//#17 같은 열 - 플레이어가 물줄기에 닿음");
            Debug.Log("//#17 물줄기 길이: " + _waterLength);
            Debug.Log("//#17 물줄기와 플레이어 간의 거리: " + (playerRow-balloonRow));

            playerLife.PlayerInWaterBalloon(); // #17 플레이어 물풍선에 갇힘
        }
    }

    public void CheckItemTouchFluid(Transform _balloonTrans, int _waterLength)   // #34
    {
        items = GameObject.FindGameObjectsWithTag("Item");
        for(int i=0; i<items.Length; i++)
        {
            // 배열 확인
            balloonRow = ReturnRowInMatrix(_balloonTrans.position.y);
            balloonCol = ReturnColInMatrix(_balloonTrans.position.x);
            itemRow = ReturnRowInMatrix(items[i].transform.position.y);
            itemCol = ReturnColInMatrix(items[i].transform.position.x);
            // 만약 아이템이 '터지는 물풍선'과 같은 행이라면
            // && 아이템과 물풍선과의 거리가 _waterLength보다 가깝다면
            if((balloonRow == itemRow) && 
                ((itemCol - balloonCol)*(itemCol - balloonCol) <= (_waterLength)*(_waterLength)))
            {
                Debug.Log("//#34 같은 행 - 아이템이 물줄기에 닿음");
                items[i].gameObject.GetComponent<Item>().DestroyItem(); // 아이템 오브젝트 Destroy
            }
            // 만약 플레이어가 터지는 물풍선과 같은 열이라면
            // && 플레이어와 물풍선과의 거리가 _waterLength보다 가깝다면
            if((balloonCol == itemCol) &&
                ((itemRow-balloonRow)*(itemRow-balloonRow) <= (_waterLength)*(_waterLength)))
            {
                Debug.Log("//#17 같은 열 - 아이템이 물줄기에 닿음");
                items[i].gameObject.GetComponent<Item>().DestroyItem(); // 아이템 오브젝트 Destroy
            }
        }
    } 
    
    public void CheckBubbleTouchFluid(Transform _balloon, int _waterLength)
    // #31 물풍선의 물줄기가 다른 물풍선에 닿았나 확인
    {
        balloonRow = ReturnRowInMatrix(_balloon.position.y);
        balloonCol = ReturnColInMatrix(_balloon.position.x);
        // #31 물풍선의 상하좌우 파악
        for(int i=0; i<_waterLength; i++)
        {
            // #31 만약 체크하고자 하는 배열의 값이 범위를 넘는다면 return (아래 함수 실행 X)
            // #31 만약 체크하고자 하는 배열의 값이 범위를 넘는다면 return (아래 함수 실행 X)
            // #31 만약 체크하고자 하는 배열의 값이 범위를 넘는다면 return (아래 함수 실행 X)
            // #31 만약 체크하고자 하는 배열의 값이 범위를 넘는다면 return (아래 함수 실행 X)
            if((balloonRow-i-1 < 0) || (balloonRow+i+1 > 6) || (balloonCol-i-1 < 0) || (balloonCol+i+1 > 8))
            {
                return;
            }


            if(waterBalloonArr[balloonRow-i-1, balloonCol]==1)    // 물풍선의 상(위)에 다른 물풍선이 있는지 파악
            {
                Debug.Log("//#31 물풍선의 물줄기가 다른 \"위쪽\" 물풍선에 닿음");
                CheckIsThereWaterBalloon(_balloon.position.x, _balloon.position.y+i+1); // #32
            }
            if(waterBalloonArr[balloonRow+i+1, balloonCol]==1)    // 물풍선의 하(아래)
            {
                Debug.Log("//#31 물풍선의 물줄기가 다른 \"아래쪽\" 물풍선에 닿음");
                CheckIsThereWaterBalloon(_balloon.position.x, _balloon.position.y-i-1); // #32
            }
            if(waterBalloonArr[balloonRow, balloonCol-i-1]==1)    // 물풍선의 좌(왼쪽)
            {                 
                Debug.Log("//#31 물풍선의 물줄기가 다른 \"왼쪽\" 물풍선에 닿음");
                Debug.Log("//#31 터진 물풍선 위치| 행: " + balloonRow + ", 열: " + balloonCol);
                Debug.Log("//#31 물줄기에 맞은 물풍선 위치| 행:  "+ balloonRow + ", 열: " + (balloonCol-i-1));
                CheckIsThereWaterBalloon(_balloon.position.x-i-1, _balloon.position.y); // #32
            }
            if(waterBalloonArr[balloonRow, balloonCol+i+1]==1)    // 물풍선의 우(오른쪽)
            {
                Debug.Log("//#31 물풍선의 물줄기가 다른 \"오른쪽\" 물풍선에 닿음");
                Debug.Log("//#31 터진 물풍선 위치| 행: " + balloonRow + ", 열: " + balloonCol);
                Debug.Log("//#31 물줄기에 맞은 물풍선 위치| 행:  "+ balloonRow + ", 열: " + (balloonCol-i-1));
                CheckIsThereWaterBalloon(_balloon.position.x+i+1, _balloon.position.y); // #32
            }
        }
    }

    public void CheckIsThereWaterBalloon(float posX, float posY, CHECK_TYPE _type = CHECK_TYPE.BALLOONBURST) 
    // #32 CHECK_TYPE.BALLOONBURST의 경우: 특정 위치(_row, _col)에 물풍선이 있는지 확인 - 있다면, 물풍선 터뜨리기
    // #33 CHECK_TYPE.PLAYERMOVE 경우: 특정 위치(_row, _col)에 물풍선이 있는지 확인 - 있다면, 플레이어 제자리걸음
    {
        checkNum = 0;   // #33 특정 위치에 물풍선이 있는지 확인하기 위함 - 0으로 초기화

        Vector3 targetPos = new Vector3(posX, posY, 0);
        // Debug.Log("//#32 (" + targetPos + ") 위치에 확인." );


        // "WAterBalloon" 태그를 가진 오브젝트들을 모두 찾기
        GameObject[] waterBalloons = GameObject.FindGameObjectsWithTag("WaterBalloon");

        // 특정 위치에 위치한 오브젝트가 있는지 확인
        foreach(GameObject obj in waterBalloons)
        {
            if(obj.transform.position == targetPos)
            {
                checkNum++; // #33 fix

                Debug.Log("//#32 (" + posX + ", " + posY + ") 좌표에 물풍선이 있습니다." );
                switch(_type)
                {
                    case CHECK_TYPE.BALLOONBURST:   // 다른 물풍선도 터지도록
                        obj.GetComponent<Obstacle>().StartWaterBalloonBursts(true);
                        break;
                    case CHECK_TYPE.PLAYERMOVE: // 
                        Debug.Log("//#33 물풍선 때문에 플레이어 이동 불가");
                        // playerCtrl.PlayerStandsStill(); // #33 플레이어 제자리걸음 // #33 fix 주석 처리
                        playerCtrl.balloonInFront = true;   // #33 fix: 앞에 물풍선 있는지 확인 - 있다면, 플레이어 이동 불가
                        break;
                }
            }
        }

        // #33 fix: 플레이어가 가고자 하는 방향에 물풍선이 하나도 없다면, 플레이어 이동 가능하도록
        if(checkNum == 0)   
        {
            // Debug.Log("//#33 (" + posX + ", " + posY + ") 위치에 물풍선이 없습니다." );
            playerCtrl.balloonInFront = false;   // #33 fix: 앞에 물풍선 있는지 확인 - 있다면, 플레이어 이동 불가/ 없다면, 플레이어 이동 가능
        }
        else
        {
            Debug.Log("//#33 checkNum이 0이 아님: "+ checkNum);
        }
    }

}
