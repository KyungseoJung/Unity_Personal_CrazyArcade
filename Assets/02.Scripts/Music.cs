using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    // #18 Musics.cs 스크립트 생성
    public AudioSource gameMusicArr;
    public AudioSource soundEffectArr;  // #21
    public AudioClip[] audioClips;
    public AudioClip[] effectClips;

    /*
    * 인스펙터에서 연결하기
    
    - 배경음 -
    0 : BGSound1

    - 효과음 -
    0 : bomb_set
    1 : eatProp
    2 : bubbleBoom
    */

    public enum BGM_TYPE {MAINMUSIC =1 }; // #20 메인 배경음
    public enum EFFECT_TYPE {BOMB_SET = 1, EAT_PROP, BUBBLE_BOOM, PLAYER_DIE };    // #21 #22 #28 효과음 종류

    void Awake()
    {
        gameMusicArr = gameObject.AddComponent<AudioSource>();  // #18 오디오소스 없기 때문에, 추가해서 지정해줘야 함
        soundEffectArr = gameObject.AddComponent<AudioSource>();    // #21 효과음 - 오디오소스 없기 때문에, 추가해서 지정해줘야 함
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

    public void SoundEffect(EFFECT_TYPE _type, float _volume = 1f)
    // #21 효과음 크기도 설정
    {
        Debug.Log("//#21 효과음 시작");
        soundEffectArr.Stop();
        soundEffectArr.clip = effectClips[(int)_type -1];

        soundEffectArr.volume = _volume;
            
        soundEffectArr.Play();
    }
}
