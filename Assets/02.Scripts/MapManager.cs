using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject waterBalloonObj;  // 배치할 물풍선 프리팹

    private Vector3 balloonPos;         // #4 물풍선 배치 위치
    
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


    public void PlaceWaterBalloon(float _x, float _y)    // #4
    {
        int row, col;
        row = -Mathf.RoundToInt(_y) + 3;
        col = Mathf.RoundToInt(_x) + 4;

        if (waterBalloonArr[row, col] == 1) // 이미 그 자리에 물풍선이 있다면 - 중복해서 물풍선을 놓지 못하도록
            return;

        waterBalloonArr[row, col] = 1;  // 배열 설정

        // 물풍선 놓기
        balloonPos.x = Mathf.RoundToInt(_x);    
        balloonPos.y = Mathf.RoundToInt(_y);
        Instantiate(waterBalloonObj, balloonPos, Quaternion.identity);

    }

}
