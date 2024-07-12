using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBurst : MonoBehaviour
{
    private int row;
    private int col;
    private MapManager mapMgr;  // #37 좌표 값을 행렬로 변환하기 위함 & obstacleArr 확인하기 위함

    private void Awake()
    {
        mapMgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>(); // #37
    }

    private void Start()
    {
        // Debug.Log("//#37 위치: " + this.transform.position);
        CheckObstacle();
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if(other.gameObject.tag == "Obstacle")
    //     {
    //         Debug.Log("//#37 " + this.gameObject.name + "물폭탄이 Obstacle에 닿음");
    //     }
    // }

    private void CheckObstacle()
    {
        row = mapMgr.ReturnRowInMatrix(this.transform.position.y);
        col = mapMgr.ReturnColInMatrix(this.transform.position.x);

        if(mapMgr.obstacleArr[row, col] == 1)
        {
            this.gameObject.SetActive(false);
            Debug.Log("//#37 " + this.gameObject.name + "물폭탄이 Obstacle에 닿음");
        } 
    }
}
