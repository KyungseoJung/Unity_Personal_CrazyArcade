using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject waterBalloonObj;  // 배치할 물풍선 프리팹

    private Vector3 balloonPos;         // #4 물풍선 배치 위치

    private int waterballoonPlaceNum = 0;   // #13 맵에 놓여진 물풍선의 개수

    int row, col;   // #4 선언 위치만 바꿈

    [SerializeField]
    int[,] waterBalloonArr =            // #4 7행 9열 이차원 배열    
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

    private void Awake()
    {
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>(); // #21
    }

    private void Start()
    {
        waterballoonPlaceNum = 0;   // #13 맵에 놓여진 물풍선의 개수
    }

    public void PlaceWaterBalloon(float _x, float _y)    // #4
    {
        if(waterballoonPlaceNum+1 >= PlayerGameMgr.Mgr.waterballoonNum)    //#13 물풍선 개수 제한
            return;

        row = -Mathf.RoundToInt(_y) + 3;
        col = Mathf.RoundToInt(_x) + 4;

        Debug.Log("//#8 물풍선 생성: row = " + row + "/ col = " + col);

        if (waterBalloonArr[row, col] == 1) // 이미 그 자리에 물풍선이 있다면 - 중복해서 물풍선을 놓지 못하도록
            return;

        // 위 조건들 모두 만족하면, 물풍선 놓기

        waterBalloonArr[row, col] = 1;  // 배열 설정

        // 물풍선 놓기
        balloonPos.x = Mathf.RoundToInt(_x);    
        balloonPos.y = Mathf.RoundToInt(_y);
        Instantiate(waterBalloonObj, balloonPos, Quaternion.identity);

        waterballoonPlaceNum += 1; // #13 물풍선 개수 하나 증가
        music.SoundEffect(Music.EFFECT_TYPE.PLACEWATERBALLOON); // #21 물풍선 놓을 때의 효과음

        Debug.Log("//#13 물풍선 개수: " + waterballoonPlaceNum);
    }

    public void RemoveWaterBalloon(float _x, float _y)  // #8 시간이 지남에 따라 물풍선 터짐
    {
        Debug.Log("//#8 물풍선 소멸: row = " + row + "/ col = " + col);

        row = -Mathf.RoundToInt(_y) + 3;
        col = Mathf.RoundToInt(_x) + 4;

        waterBalloonArr[row, col] = 0;  // 배열 설정

        waterballoonPlaceNum -= 1; // #13 물풍선 개수 하나 감소
        Debug.Log("//#13 물풍선 개수: " + waterballoonPlaceNum);

    }

}
