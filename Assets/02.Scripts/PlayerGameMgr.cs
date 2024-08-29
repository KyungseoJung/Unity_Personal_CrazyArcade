using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInfo
{
    public int fluid = 1;   // 물줄기 세기
    public int waterballoonNum = 1; // 놓을 수 있는 물풍선 개수
    public int roller = 0;   // #15 플레이어 달리는 속도
    public bool turtle = false; // #16 거북을 타고 있는지 확인
    public int turtleNum = 0;      // #16 거북 아이템 획득 개수
    public int coin = 0;    // #30
    public int needle = 1;  // #43 바늘 아이템
}

public class GivenQuota
{
    public int life = 3;  // #28 플레이어에게 주어진 목숨 할당량
}


public class PlayerGameMgr : MonoBehaviour    // #11 플레이어의 스킬 관리 
{

    private SkillInfo skillInfo;
    private GivenQuota givenQuota;  // #28

    private static PlayerGameMgr mgr = null;  // 싱글톤 객체 (인스턴스)
    public static PlayerGameMgr Mgr           // 싱글톤 프로퍼티
    {
        get
        {
            if(mgr == null)
            {
                mgr = GameObject.FindObjectOfType(typeof(PlayerGameMgr)) as PlayerGameMgr;
                    //이런 타입을 가진 오브젝트가 있다면, 그 오브젝트를 InfoManager로서 객체화 해라
                if(mgr == null)
                {
                    mgr = new GameObject("Singleton_PlayerGameMgr", typeof(PlayerGameMgr)).GetComponent<PlayerGameMgr>();
                    DontDestroyOnLoad(mgr);
                }

            }
            return mgr;
        }
    }
    void Awake()    //Start에 적으면 다른 것들보다 늦게 실행돼서 Null 에러 발생함.
    {
        skillInfo = new SkillInfo();
        givenQuota = new GivenQuota();  // #28
    }

    public int fluid
    {
        get {return skillInfo.fluid; }
        set {skillInfo.fluid = value; }
    }

    public int waterballoonNum  // #13
    {
        get {return skillInfo.waterballoonNum; }
        set {skillInfo.waterballoonNum = value; }
    }

    public int roller   // #15
    {
        get {return skillInfo.roller; }
        set {skillInfo.roller = value; }
    }

    public bool turtle  // #16
    {
        get {return skillInfo.turtle; }
        set {skillInfo.turtle = value;}
    }

    public int turtleNum
    {
        get {return skillInfo.turtleNum; }
        set {skillInfo.turtleNum = value;} 
    }

    public int coin     // #30
    {
        get {return skillInfo.coin;}
        set {skillInfo.coin = value;}
    }

    public int needle   // #43
    {
        get {return skillInfo.needle;}
        set {skillInfo.needle = value;}
    }

    public int life // #28
    {
        get {return givenQuota.life; }
        set {givenQuota.life = value;}
    }

}