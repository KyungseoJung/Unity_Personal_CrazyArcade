using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    // #18 Musics.cs 스크립트 생성
    public AudioSource gameMusicArr;
    public AudioClip[] audioClips;
    /*
    - 배경음 -
    0 : BGSound1

    - 효과음 -
    
    */

    public enum BGM_TYPE {MAINMUSIC =1 }; // #20 메인 배경음

    void Awake()
    {
        gameMusicArr = gameObject.AddComponent<AudioSource>();  // #18 오디오소스 없기 때문에, 추가해서 지정해줘야 함

    }

    void Start()
    {
        BackGroundMusic(BGM_TYPE.MAINMUSIC);    // #20 메인 뮤직 BGM
    }

    public void BackGroundMusic(BGM_TYPE _type)
    {   
        Debug.Log("//#20 메인 배경음 시작");
        gameMusicArr.Stop();
        gameMusicArr.clip = audioClips[(int)_type -1];
        gameMusicArr.Play();
        gameMusicArr.loop = true;  // #20 메인 뮤직 BGM 반복되도록 설정

    }

}
