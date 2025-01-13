using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    private Vector3 pos;                   // #101 Enemy가 게임 맵 경계선 밖으로 넘어가지 않도록 확인

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        CheckBorder();  // #101 Enemy가 게임 맵 경계선 밖으로 넘어가지 않도록 확인
    }


    private void CheckBorder()
    {
        // #24 플레이어가 게임 맵 경계선 밖으로 넘어가지 않도록 확인
        if((transform.position.x) * (transform.position.x) > 4*4 )
        {
            // Debug.Log("// #101 Enemy(Lodumani)가 x좌표 경계선 넘어감");
            pos = this.transform.position;
            pos.x = (int)this.transform.position.x; // -4 또는 4로 지정
            this.transform.position = pos;
        }
        
        if((transform.position.y) * (transform.position.y) > 3*3)
        {
            // Debug.Log("// #101 Enemy(Lodumani)가 y좌표 경계선 넘어감");
            pos = this.transform.position;
            pos.y = (int)this.transform.position.y; // -3 또는 3으로 지정
            this.transform.position = pos;
        }

    }
}
