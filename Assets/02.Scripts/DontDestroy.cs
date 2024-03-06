using System.Collections;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()    
    {
        // #19 (씬 전환 시)이 게임오브젝트가 사라지지 않도록
        DontDestroyOnLoad(this.gameObject);
        
    }
}
