using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    // #18 Musics.cs 스크립트 생성
    public AudioSource gameMusicArr;
    public AudioSource soundEffectArr;  // #21
    public AudioSource playerSoundEffectArr;    // #46 gameEffectClips 플레이어 효과음은 따로 관리
    public AudioClip[] audioClips;
    public AudioClip[] effectClips;


    /*
    * 인스펙터에서 연결하기
    
    - 배경음 -
    0 : BGSound1
    1 : login_scene (로비 화면 BGM)

    - 효과음 -
    0 : bomb_set
    1 : eatProp
    2 : bubbleBoom
    3 : ef_playerDie        // #28 #46 뭔가 터지는 소리
    4 : ef_playerInBalloon  // #44 #46
    5 : bomb_pop            // #43 바늘 아이템 사용해서 물풍선 벗어날 때 효과음
    6 : ef_playerRevival    // #45 #46 플레이어 부활 효과음 - 원래 이 효과음이 아닌데, 대체할 게 이것 뿐.
    7 : charClick           // #49 로비 화면에서 버튼에 마우스 hover 했을 때 효과음
    8 : gameStart           // #19 게임 시작 효과음
    9 : button_click        // #49 마우스 클릭인데, 사실 'bomb_set'과 같은 효과음
    10 : ef_clickclick      // #54 통조림 아이템 사용해서 빠른 거북으로 바뀔 때의 효과음
    11 : player_shield      // #59 플레이어 shield item 사용하고 있을 때의 효과음
    */

    public enum BGM_TYPE {MAINMUSIC =1, LOBBYMUSIC}; // #20 메인 배경음
    public enum EFFECT_TYPE {BOMB_SET = 1, EAT_PROP, BUBBLE_BOOM, PLAYER_DIE, PLYAER_IN_BALLOON, 
    BOMB_POP, PLAYER_REVIVAL, BUTTON_HOVER, GAME_START, BUTTON_CLICK, TURTLE_CHANGE, PLAYER_SHIELD};    // #21 #22 #28 #44 #43 #45 #54 효과음 종류

    void Awake()
    {
        gameMusicArr = gameObject.AddComponent<AudioSource>();  // #18 오디오소스 없기 때문에, 추가해서 지정해줘야 함
        soundEffectArr = gameObject.AddComponent<AudioSource>();    // #21 효과음 - 오디오소스 없기 때문에, 추가해서 지정해줘야 함
        playerSoundEffectArr = gameObject.AddComponent<AudioSource>();    // #46 플레이어 전용 효과음 - 오디오소스 없기 때문에, 추가해서 지정해줘야 함
    }

    void Start()
    {
        // BackGroundMusic(BGM_TYPE.MAINMUSIC);    // #20 메인 뮤직 BGM
    }

    public void BackGroundMusic(BGM_TYPE _type)
    {   
        Debug.Log("//#20 메인 배경음 시작");
        gameMusicArr.Stop();
        gameMusicArr.clip = audioClips[(int)_type -1];
        gameMusicArr.Play();
        gameMusicArr.loop = true;  // #20 메인 뮤직 BGM 반복되도록 설정

    }

    public void GameSoundEffect(EFFECT_TYPE _type, float _volume = 1f, bool _loop = false)
    // #21 효과음 크기도 설정
    {
        Debug.Log("//#21 게임 효과음 시작");
        soundEffectArr.Stop();
        soundEffectArr.clip = effectClips[(int)_type -1];

        soundEffectArr.volume = _volume;
            
        soundEffectArr.Play();

        soundEffectArr.loop = _loop;    // #44 효과음 반복 - ex) 플레이어가 물풍선에 갇혀 있을 때 나는 효과음
    }

    public void PlayerSoundEffect(EFFECT_TYPE _type, float _volume = 1f, bool _loop = false)    // #46 플레이어에게서 나타나는 효과음은 따로 관리 - 게임 효과음이 묻히지 않도록
    {
        Debug.Log("//#46 플레이어 효과음 시작");
        playerSoundEffectArr.Stop();
        playerSoundEffectArr.clip = effectClips[(int)_type -1];

        playerSoundEffectArr.volume = _volume;
            
        playerSoundEffectArr.Play();

        playerSoundEffectArr.loop = _loop;
    }

    public void StopPlayerSoundEffect() // #47
    {
        playerSoundEffectArr.Stop();
    }
}
