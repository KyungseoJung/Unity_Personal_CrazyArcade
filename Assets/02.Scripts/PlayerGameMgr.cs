using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameMgr : MonoBehaviour    // #11 플레이어의 스킬 관리 
{
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




}