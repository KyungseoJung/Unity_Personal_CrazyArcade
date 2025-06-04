using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSkillInfo
{
    public int fluid = 1;   // 물줄기 세기
    public int waterballoonNum = 1; // 놓을 수 있는 물풍선 개수
    public int roller = 0;   // 플레이어 달리는 속도
    public bool slowTurtle = false; // 거북을 타고 있는지 확인  // 빠른 거북과 느린 거북을 구분 지어서 설정
    public bool fastTurtle = false; // 거북을 타고 있음 && 빠른 속도의 거북임
    public int turtleNum = 0;      // 거북 아이템 획득 개수
    public int coin = 0;
    public int turtleCan = 1;   //느린 거북을 빠르게 해주는 can item
    public int needle = 1;  // 바늘 아이템
    public int shield = 1;  // 외부 공격으로부터 막아주는 shield item
}

public class SubGivenQuota
{
    public int life = 3;  // 플레이어에게 주어진 목숨 할당량
}

public class SubPlayerGameMgr : MonoBehaviour   // 플레이어의 스킬 관리
{
    private SubSkillInfo subSkillInfo;
    private SubGivenQuota subGivenQuota;

    private static SubPlayerGameMgr submgr = null;  // 싱글톤 객체 (인스턴스)
    public static SubPlayerGameMgr SubMgr           // 싱글톤 프로퍼티
    {
        get
        {
            // Debug.Log("#100 체크1");
            if(submgr == null)
            {
                Debug.Log("#100 체크2");
                submgr = GameObject.FindObjectOfType(typeof(SubPlayerGameMgr)) as SubPlayerGameMgr;
                    //이런 타입을 가진 오브젝트가 있다면, 그 오브젝트를 InfoManager로서 객체화 해라
                if(submgr == null)
                {
                    Debug.Log("#100 체크3");
                    submgr = new GameObject("Singleton_SubPlayerGameMgr", typeof(SubPlayerGameMgr)).GetComponent<SubPlayerGameMgr>();
                    DontDestroyOnLoad(submgr);
                }

            }
            return submgr;
        }
    }
    void Awake()    //Start에 적으면 다른 것들보다 늦게 실행돼서 Null 에러 발생함.
    {
        subSkillInfo = new SubSkillInfo();
        subGivenQuota = new SubGivenQuota();
    }

    public int fluid
    {
        get {return subSkillInfo.fluid; }
        set {subSkillInfo.fluid = value; }
    }

    public int waterballoonNum
    {
        get {return subSkillInfo.waterballoonNum; }
        set {subSkillInfo.waterballoonNum = value; }
    }

    public int roller
    {
        get {return subSkillInfo.roller; }
        set {subSkillInfo.roller = value; }
    }

    public bool slowTurtle  // 빠른 거북
    {
        get {return subSkillInfo.slowTurtle; }
        set {subSkillInfo.slowTurtle = value;}
    }
    
    public bool fastTurtle  // 느린 거북
    {
        get {return subSkillInfo.fastTurtle; }
        set {subSkillInfo.fastTurtle = value;}
    }

    public int turtleNum
    {
        get {return subSkillInfo.turtleNum; }
        set {subSkillInfo.turtleNum = value;} 
    }

    public int coin
    {
        get {return subSkillInfo.coin;}
        set {subSkillInfo.coin = value;}
    }
    public int turtleCan
    {
        get {return subSkillInfo.turtleCan;}
        set {subSkillInfo.turtleCan = value;}
    }
    public int needle
    {
        get {return subSkillInfo.needle;}
        set {subSkillInfo.needle = value;}
    }
    public int shield
    {
        get {return subSkillInfo.shield;}
        set {subSkillInfo.shield = value;}
    }

    public int life
    {
        get {return subGivenQuota.life; }
        set {subGivenQuota.life = value;}
    }
}
