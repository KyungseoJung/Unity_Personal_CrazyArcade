using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInfo
{
    public int waterballoonNum = 1; // 놓을 수 있는 물풍선 개수
    public int fluid = 1;   // 물줄기 세기
}


public class PlayerGameMgr : MonoBehaviour    // #11 플레이어의 스킬 관리 
{

    private SkillInfo skillInfo;

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
    }

    public int waterballoonNum  // #13
    {
        get {return skillInfo.waterballoonNum; }
        set {skillInfo.waterballoonNum = value; }
    }

    public int fluid
    {
        get {return skillInfo.fluid; }
        set {skillInfo.fluid = value; }
    }


}