using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject waterBalloonObj;  // 배치할 물풍선 프리팹

    private Vector3 balloonPos;         // #4 물풍선 배치 위치

    private int waterballoonPlaceNum = 0;   // #13 맵에 놓여진 물풍선의 개수

    int row, col;   // #4 선언 위치만 바꿈
    int playerRow, playerCol;   // #17
    int rowNum = 4; // #25
    int colNum = 3; // #25

    // [SerializeField]
    int[,] waterBalloonArr =            // #4 7행 9열 이차원 배열 - 0행 0열부터 시작
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

    private Music music; 
    private PlayerLife playerLife;   

    private void Awake()
    {
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>(); // #21
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>(); // #4
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

    private void CheckObstaclePos() // #25 장애물 위치 - 배열 확인
    {
        Debug.Log("//#25 CheckObstaclePos");
        // #25 Obstacle 태그를 가진 모든 오브젝트에 접근
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        // #25 
        for(int i=0; i<obstacles.Length; i++)
        {
            row = ReturnRowInMatrix(obstacles[i].transform.position.y);     // #26 함수 이용
            col =  ReturnColInMatrix(obstacles[i].transform.position.x);    // #26 함수 이용
            Debug.Log("//#25 Obstacle 존재 - row: " + row + ", col: " + col);
            Debug.Log("//#25 Ostacle 총 몇 개= " + (i + 1) + "오브젝트 이름: " + obstacles[i].gameObject.name);
            obstacleArr[row, col] = 1;  // #25 

        }
    }

    public void PlaceWaterBalloon(float _x, float _y)    // #4 받아오는 parameter는 플레이어의 좌표
    {
        if(playerLife.trappedInWater)   // #4 플레이어가 물풍선에 갇혀 있다면, PlaceWaterBalloon 실행되지 않도록
            return;
        Debug.Log("//#4 fix | 놓여진 물풍선 수: " + waterballoonPlaceNum + ", 놓을 수 있는 물풍선 수: " + PlayerGameMgr.Mgr.waterballoonNum);
        if(waterballoonPlaceNum >= PlayerGameMgr.Mgr.waterballoonNum)    //#13 물풍선 개수 제한
            return;

        playerRow = ReturnRowInMatrix(_y);    // #26 함수 이용 
        playerCol = ReturnColInMatrix(_x);    // #26 함수 이용

        Debug.Log("//#8 #4 물풍선 생성: row = " + playerRow + "/ col = " + playerCol);

        if ((waterBalloonArr[playerRow, playerCol] == 1) || (obstacleArr[playerRow, playerCol] == 1)) 
        // 이미 그 자리에 물풍선 or 장애물이 있다면 - 중복해서 물풍선을 놓지 못하도록
            return;

        // 위 조건들 모두 만족하면, 물풍선 놓기

        waterBalloonArr[playerRow, playerCol] = 1;  // 배열 설정
        obstacleArr[playerRow, playerCol] = 1;      // #25 배열 설정

        // 물풍선 놓기
        balloonPos.x = Mathf.RoundToInt(_x);    
        balloonPos.y = Mathf.RoundToInt(_y);
        Instantiate(waterBalloonObj, balloonPos, Quaternion.identity);

        waterballoonPlaceNum += 1; // #13 물풍선 개수 하나 증가
        music.SoundEffect(Music.EFFECT_TYPE.BOMB_SET); // #21 물풍선 놓을 때의 효과음

        Debug.Log("//#13 물풍선 개수: " + waterballoonPlaceNum);
    }

    public void RemoveWaterBalloon(float _x, float _y)  // #8 시간이 지남에 따라 물풍선 터짐 - 받아오는 parameter는 물풍선의 좌표
    {
        Debug.Log("//#8 물풍선 소멸: row = " + row + "/ col = " + col);

        row = ReturnRowInMatrix(_y);
        col = ReturnColInMatrix(_x);

        waterBalloonArr[row, col] = 0;  // 배열 설정
        obstacleArr[row, col] = 0;      // #25 배열 설정

        waterballoonPlaceNum -= 1; // #13 물풍선 개수 하나 감소
        music.SoundEffect(Music.EFFECT_TYPE.BUBBLE_BOOM); // #21 물풍선 터질 때의 효과음

        Debug.Log("//#13 물풍선 개수: " + waterballoonPlaceNum);

    }

    public void CheckPlayerTouchFluid(Transform _balloon, float _playerX, float _playerY, int _waterLength)   
    // #17 플레이어가 물풍선의 물줄기와 닿았나 확인
    {
        row = ReturnRowInMatrix(_balloon.position.y);
        col = ReturnColInMatrix(_balloon.position.x);

        playerRow = ReturnRowInMatrix(_playerY);
        playerCol = ReturnColInMatrix(_playerX);

        // 만약 플레이어가 터지는 물풍선과 같은 행이라면
        // && 플레이어와 물풍선과의 거리가 _waterLength보다 가깝다면
        if((row == playerRow) && 
            ((playerCol - col)*(playerCol - col) <= (_waterLength)*(_waterLength)))
        {
            Debug.Log("//#17 같은 행 - 플레이어가 물줄기에 닿음");
            Debug.Log("//#17 물줄기 길이: " + _waterLength);
            Debug.Log("//#17 물줄기와 플레이어 간의 거리: " + (playerCol-col));

            playerLife.PlayerInWaterBalloon(); // #17 플레이어 물풍선에 갇힘
        }

        // 만약 플레이어가 터지는 물풍선과 같은 열이라면
        // && 플레이어와 물풍선과의 거리가 _waterLength보다 가깝다면
        if((col == playerCol) &&
            ((playerRow-row)*(playerRow-row) <= (_waterLength)*(_waterLength)))
        {
            Debug.Log("//#17 같은 열 - 플레이어가 물줄기에 닿음");
            Debug.Log("//#17 물줄기 길이: " + _waterLength);
            Debug.Log("//#17 물줄기와 플레이어 간의 거리: " + (playerRow-row));

            playerLife.PlayerInWaterBalloon(); // #17 플레이어 물풍선에 갇힘
        }
    }
    public void CheckBubbleTouchFluid(Transform _balloon, int _waterLength)
    // #31 물풍선의 물줄기가 다른 물풍선에 닿았나 확인
    {
        row = ReturnRowInMatrix(_balloon.position.y);
        col = ReturnColInMatrix(_balloon.position.x);
        // #31 물풍선의 상하좌우 파악
        for(int i=0; i<_waterLength; i++)
        {
            if(waterBalloonArr[row-i-1, col]==1)    // 물풍선의 상(위)에 다른 물풍선이 있는지 파악
            {
                Debug.Log("//#31 물풍선의 물줄기가 다른 \"위쪽\" 물풍선에 닿음");
                CheckIsThereWaterBalloon(_balloon.position.x, _balloon.position.y+i+1); // #32
            }
            if(waterBalloonArr[row+i+1, col]==1)    // 물풍선의 하(아래)
            {
                Debug.Log("//#31 물풍선의 물줄기가 다른 \"아래쪽\" 물풍선에 닿음");
                CheckIsThereWaterBalloon(_balloon.position.x, _balloon.position.y-i-1); // #32
            }
            if(waterBalloonArr[row, col-i-1]==1)    // 물풍선의 좌(왼쪽)
            {
                Debug.Log("//#31 물풍선의 물줄기가 다른 \"왼쪽\" 물풍선에 닿음");
                Debug.Log("//#31 터진 물풍선 위치| 행: " + row + ", 열: " + col);
                Debug.Log("//#31 물줄기에 맞은 물풍선 위치| 행:  "+ row + ", 열: " + (col-i-1));
                CheckIsThereWaterBalloon(_balloon.position.x-i-1, _balloon.position.y); // #32
            }
            if(waterBalloonArr[row, col+i+1]==1)    // 물풍선의 우(오른쪽)
            {
                Debug.Log("//#31 물풍선의 물줄기가 다른 \"오른쪽\" 물풍선에 닿음");
                Debug.Log("//#31 터진 물풍선 위치| 행: " + row + ", 열: " + col);
                Debug.Log("//#31 물줄기에 맞은 물풍선 위치| 행:  "+ row + ", 열: " + (col-i-1));
                CheckIsThereWaterBalloon(_balloon.position.x+i+1, _balloon.position.y); // #32
            }
        }
    }

    private void CheckIsThereWaterBalloon(float _row, float _col) // #32 특정 위치(_row, _col)에 물풍선이 있는지 확인
    {
        Vector3 targetPos = new Vector3(_row, _col, 0);
        Debug.Log("//#32 (" + targetPos + ") 위치에 확인." );


        // "WAterBalloon" 태그를 가진 오브젝트들을 모두 찾기
        GameObject[] waterBalloons = GameObject.FindGameObjectsWithTag("WaterBalloon");

        // 특정 위치에 위치한 오브젝트가 있는지 확인
        foreach(GameObject obj in waterBalloons)
        {
            if(obj.transform.position == targetPos)
            {
                Debug.Log("//#32 (" + _row + ", " + _col + ") 위치에 물풍선이 있습니다." );
                obj.GetComponent<Obstacle>().StartWaterBalloonBursts(true);
            }
        }
    }

}
