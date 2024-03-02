using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    // #18 Musics.cs 스크립트 생성
    public AudioSource gameMusicArr;

    void Awake()
    {
        gameMusicArr = gameObject.AddComponent<AudioSource>();  // #18 오디오소스 없기 때문에, 추가해서 지정해줘야 함

    }

}
