﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBurst : MonoBehaviour
{
    private int firstRow;
    private int firstCol;
    private int secondRow;
    private int secondCol;
    private MapManager mapMgr;  // #37 좌표 값을 행렬로 변환하기 위함 & obstacleArr 확인하기 위함
    [SerializeField] private GameObject firstWaterObj;   // #37 waterUp 오브젝트 자식들 중 상위 오브젝트는 up1
                // 하위 물 폭탄 오브젝트(up2)가 비활성화 되면, 상위 물 폭탄 오브젝트(up1)도 비활성화 하기 위함
    [SerializeField] private GameObject secondWaterObj;   // #37 waterUp 오브젝트 자식들 중 하위 오브젝트는 up2

    private void Awake()
    {
        mapMgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>(); // #37
        // topWaterBurst = transform.parent.Find(this.gameObejct.name + "")
        Debug.Log("상위 오브젝트: " + firstWaterObj.name);
        Debug.Log("하위 오브젝트: " + secondWaterObj.name);
    }

    private void Start()
    {
        // Debug.Log("//#37 위치: " + this.transform.position);
        CheckFirstWater();
        CheckSecondWater();
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if(other.gameObject.tag == "Obstacle")
    //     {
    //         Debug.Log("//#37 " + this.gameObject.name + "물폭탄이 Obstacle에 닿음");
    //     }
    // }

    private void CheckFirstWater()
    {
        firstRow = mapMgr.ReturnRowInMatrix(firstWaterObj.transform.position.y);
        firstCol = mapMgr.ReturnColInMatrix(firstWaterObj.transform.position.x);

        if(mapMgr.obstacleArr[firstRow, firstCol] == 1)   // 만약 하위 물줄기가 장애물에 닿으면, 하위 물줄기도 비활성화 하고, 상위 물줄기도 비활성화하기
        {
            firstWaterObj.SetActive(false);
            Debug.Log("//#37 " + firstWaterObj.name + "물폭탄 비활성화 | Obstacle에 닿음");

        }
    }
    private void CheckSecondWater()
    {
        secondRow = mapMgr.ReturnRowInMatrix(secondWaterObj.transform.position.y);
        secondCol = mapMgr.ReturnColInMatrix(secondWaterObj.transform.position.x);

        if(mapMgr.obstacleArr[secondRow, secondCol] == 1)   // 만약 하위 물줄기가 장애물에 닿으면, 하위 물줄기도 비활성화 하고, 상위 물줄기도 비활성화하기
        {
            secondWaterObj.SetActive(false);
            Debug.Log("//#37 " + secondWaterObj.name + "물폭탄 비활성화 | Obstacle에 닿음");

            firstWaterObj.SetActive(false); 
            Debug.Log("//#37 " + firstWaterObj.name + "물폭탄 비활성화");
        }
    }
}