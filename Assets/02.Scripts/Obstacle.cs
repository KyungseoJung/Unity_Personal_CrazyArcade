using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public enum OBSTACLE_TYPE {WATERBALLOON = 1, BUSH, WOODBLOCK}    // #7 Obstacle마다 TYPE 설정하기   // #14 (WOODBLOCK)
    public OBSTACLE_TYPE obstacleType = OBSTACLE_TYPE.WATERBALLOON; // #7

    private MapManager mapMgr;             // #8 물풍선 지우기 위함

    [SerializeField]    private Animator anim;  // #6 덤불 Animator 조정

    [SerializeField]    private int waterLength;    // #9 물풍선 터질 때 길이

    void Awake()
    {
        mapMgr = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>(); // #8

        anim = transform.GetComponent<Animator>();     
    }
    
    void Start()
    {
        switch(obstacleType)    // #8 시간이 지남에 따라 물풍선 터짐
        {
            case OBSTACLE_TYPE.WATERBALLOON :
                StartCoroutine(WaterBalloonBursts());
                waterLength = 1;    // #9 첫 물풍선 길이는 일단 1로 설정
                break;
        }
    }

    public void BushShake() // #6 애니메이터 설정: 플레이어가 덤불에 숨으면, 덤불 흔들리도록 
    {
        Debug.Log("//#6 덤불 흔들림");
        anim.SetTrigger("Shake");
    }

    IEnumerator WaterBalloonBursts()    // #8 3초 뒤에 해당 물풍선 파괴
    {
        Debug.Log("//#8 3초 기다림 시작");
        yield return new WaitForSeconds(3.0f);
        
        Debug.Log("//#8 파괴 | 위치는" + transform.position.x + ", " + transform.position.y);
        anim.SetTrigger("Bursts");
        anim.SetInteger("WaterLength", waterLength);  // #9 물줄기 길이 설정

        mapMgr.RemoveWaterBalloon(this.transform.position.x, this.transform.position.y);
    }

    // public void DestroyWaterBalloon()   // #9 애니메이터 Clips에서 접근 및 실행 // #9 fix: Destructor.cs 스크립트 자체를 이용하도록
    // {
    //     Destroy(this.gameObject);    // #9 물풍선 파괴
    // }

}   
