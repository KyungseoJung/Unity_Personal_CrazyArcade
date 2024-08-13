using System.Collections;
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
    [SerializeField] private BoxCollider firstWatObjCol;     // #37 콜라이더 처리가 먼저 되지 않도록, 장애물이 없는 걸 확인한 후에 각 오브젝트의 BoxCollider 활성화하기
    [SerializeField] private GameObject secondWaterObj;   // #37 waterUp 오브젝트 자식들 중 하위 오브젝트는 up2

    [SerializeField] private BoxCollider secondWatObjCol;    // #37 콜라이더 처리가 먼저 되지 않도록, 장애물이 없는 걸 확인한 후에 각 오브젝트의 BoxCollider 활성화하기

    private void Awake()
    {
        mapMgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>(); // #37
        // topWaterBurst = transform.parent.Find(this.gameObejct.name + "")
        Debug.Log("상위 오브젝트: " + firstWaterObj.name);
        Debug.Log("하위 오브젝트: " + secondWaterObj.name);

        // #37 콜라이더 처리가 먼저 되지 않도록, 장애물이 없는 걸 확인한 후에 각 오브젝트의 BoxCollider 활성화하기
        // firstWatObjCol = firstWaterObj.GetComponent<BoxCollider>();
        // secondWatObjCol = secondWaterObj.GetComponent<BoxCollider>();

        if (firstWaterObj == null)
        {
            Debug.LogError("firstWaterObj가 null입니다.");
        }
        else
        {
            firstWatObjCol = firstWaterObj.GetComponent<BoxCollider>();
            if (firstWatObjCol == null)
            {
                Debug.LogError("firstWaterObj의 BoxCollider를 찾을 수 없습니다.");
            }
        }

        if (secondWaterObj == null)
        {
            Debug.LogError("secondWaterObj가 null입니다.");
        }
        else
        {
            secondWatObjCol = secondWaterObj.GetComponent<BoxCollider>();
            if (secondWatObjCol == null)
            {
                Debug.LogError("secondWaterObj의 BoxCollider를 찾을 수 없습니다.");
            }
        }

    }

    private void Start()
    {
        // Debug.Log("//#37 위치: " + this.transform.position);
        // CheckFirstWater();
        CheckSecondWater();
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if(other.gameObject.tag == "Obstacle")
    //     {
    //         Debug.Log("//#37 " + this.gameObject.name + "물폭탄이 Obstacle에 닿음");
    //     }
    // }

    private void CheckFirstWater()  // 상위 물줄기
    {
        firstRow = mapMgr.ReturnRowInMatrix(firstWaterObj.transform.position.y);
        firstCol = mapMgr.ReturnColInMatrix(firstWaterObj.transform.position.x);

        if((firstRow < 0) || (firstRow > 6) || (firstCol < 0) || (firstCol > 8))
        {
            // IndexOutOfRangeException 에러 방지 - 예외 처리 (바운더리 넘어가면 장애물 있는 것으로 파악. -> 물풍선이 아예 활성화 되지 않도록)
            firstWaterObj.SetActive(false);
            Debug.Log("//#37 " + firstWaterObj.name + "물폭탄 비활성화 | 맵 경계에 닿음");
        }
        else if((mapMgr.obstacleArr[firstRow, firstCol] == 1) && (mapMgr.waterBalloonArr[firstRow, firstCol] == 0))   
        // #31 fix: 단, 물풍선이 아닌 장애물일 때만 적용되도록 
        // 만약 물풍선이라면, 이 물줄기가 다른 물풍선에 닿아서 그 물풍선도 터져야 하기 때문에 
        {
            firstWaterObj.SetActive(false);
            Debug.Log("//#37 " + firstWaterObj.name + "물폭탄 비활성화 | Obstacle에 닿음");
        }
        else    // #37 feat: 장애물 없는 거 확인한 후, 물줄기 콜라이더 켜기
        {
            firstWatObjCol.enabled = true;
        }
    }
    private void CheckSecondWater() // 하위 물줄기
    {
        secondRow = mapMgr.ReturnRowInMatrix(secondWaterObj.transform.position.y);
        secondCol = mapMgr.ReturnColInMatrix(secondWaterObj.transform.position.x);

        if((secondRow < 0) || (secondRow > 6) || (secondCol < 0) || (secondCol > 8))
        {
            // IndexOutOfRangeException 에러 방지 - 예외 처리 (바운더리 넘어가면 장애물 있는 것으로 파악. -> 물풍선이 아예 활성화 되지 않도록)
            secondWaterObj.SetActive(false);
            Debug.Log("//#37 " + secondWaterObj.name + "물폭탄 비활성화 | 맵 경계에 닿음");
        }
        else if((mapMgr.obstacleArr[secondRow, secondCol] == 1) && (mapMgr.waterBalloonArr[secondRow, secondCol] == 0))   // 만약 하위 물줄기가 장애물에 닿으면, 하위 물줄기도 비활성화 하고, 상위 물줄기도 비활성화하기
        // #31 fix: 단, 물풍선이 아닌 장애물일 때만 적용되도록 
        // 만약 물풍선이라면, 이 물줄기가 다른 물풍선에 닿아서 그 물풍선도 터져야 하기 때문에
        {
            secondWaterObj.SetActive(false);
            Debug.Log("//#37 " + secondWaterObj.name + "물폭탄 비활성화 | Obstacle에 닿음");

            firstWaterObj.SetActive(false); 
            Debug.Log("//#37 " + firstWaterObj.name + "물폭탄 비활성화");
        }
        else if((mapMgr.blockArr[secondRow, secondCol] == 1))   // #40 secondWatObjCol 자리에 블록이 있을 경우, firstWatObjCol 자리에는 물줄기가 생기지 않도록
        {
            secondWatObjCol.enabled = true;
            firstWaterObj.SetActive(false);
        }
        else    // #37 feat: 장애물 없는 거 확인한 후, 물줄기 콜라이더 켜기
        {
            secondWatObjCol.enabled = true;
        }

        CheckFirstWater();  // #37 fix: CheckSecondWater 함수 이후에 CheckFirstWater 함수 실행하기 - CheckSecondWater에서 firstWaterObj 활성화 유무를 확인 후, CheckFirstWater 함수 실행하기 위해
    }
}
