using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;          // #19 Scene 전환 목적

using UnityEngine.UI;                       // #27 플레이어 목숨 표시

using UnityEngine.EventSystems;

public class LobbyManager : MonoBehaviour
{
    private Music music;    // #49 버튼 눌렀을 때 효과음 들리도록 하기 위함
    [SerializeField] GameObject pnlStartScene;     // 비활성화할 패널 오브젝트

    // #49 feat '게임 방법' 버튼에 마우스 올려 놓으면, '게임 방법' 버튼이 더 밝게 빛나도록 

    [SerializeField] GameObject pnlLoading;                 // #51 처음 로딩될 때 pnlLoading 먼저 보이고, 그 다음에 pnlStartScnee이 보이도록

    [SerializeField] GameObject pnlbtnPressHowToGame;       // 버튼 눌렀을 때 보이는 panel ('게임 방법' 버튼)
    [SerializeField] GameObject pnlbtnPressGameStart;       // 버튼 눌렀을 때 보이는 panel ('게임 시작' 버튼)
    [SerializeField] GameObject pnlHowToGameScreen;         // #52 '게임 방법' 버튼 눌렀을 때, '게임 방법' 보여주는 화면 보이도록 하기
    
    private Animator anim;  // #50 시작 화면 효과 주기 위한 Animator
    public Text txtPlayerLife;                    // #27 플레이어 목숨 표시

    [SerializeField] Button btnHowToGame;           // #49 '게임 방법' 버튼
    [SerializeField] Button btnStartGame;           // #49 '게임 시작' 버튼

    void Awake()
    {
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>(); // #49
    }

    void Start()
    {   
        // #50 로비 화면 입장할 때, 로비 BGM 시작
        music.BackGroundMusic(Music.BGM_TYPE.LOBBYMUSIC);

        // #50 시작 화면 효과 주기 위한 Animator
        anim = pnlStartScene.transform.GetComponent<Animator>();

        // #51 처음 로딩될 때 pnlLoading 먼저 보이고, 그 다음에 pnlStartScnee이 보이도록
        if(pnlStartScene.activeSelf)
            pnlStartScene.SetActive(false);
        if(!pnlLoading.activeSelf)
            pnlLoading.SetActive(true);
        
        Invoke("ActivePnlStartScene", 3f);
        
        // StartGame(); // #49 '게임 시작' 버튼 누르면, StartGame() 함수 실행되도록 하기
        // #49 특정 버튼에 대해 함수를 연결하는 부분을 인스펙터(Inspector)에서 했었음. -> 코드상으로 설정하는 방식으로 변경하기.
        if(btnHowToGame != null)
        {
            btnHowToGame.onClick.AddListener(ShowHowToGame);

            // #49 feat '게임 방법' 버튼에 마우스 올려 놓으면, '게임 방법' 버튼이 더 밝게 빛나도록 
            // EventTrigger 컴포넌트가 없으면 추가
            EventTrigger eventTrigger1 = btnHowToGame.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger1 == null)
            {
                eventTrigger1 = btnHowToGame.gameObject.AddComponent<EventTrigger>();
            }

            // 마우스 오버 이벤트
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => { OnHoverEnterHowToGame(); });

            // 마우스 나가기 이벤트
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnHoverExitHowToGame(); });

            // 이벤트 트리거에 추가
            eventTrigger1.triggers.Add(pointerEnter);
            eventTrigger1.triggers.Add(pointerExit);
        }

        if(btnStartGame != null)
        {
            btnStartGame.onClick.AddListener(StartGame);

            // #49 feat '게임 시작' 버튼에 마우스 올려 놓으면, '게임 시작' 버튼이 더 밝게 빛나도록 
            // EventTrigger 컴포넌트가 없으면 추가
            EventTrigger eventTrigger2 = btnStartGame.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger2 == null)
            {
                eventTrigger2 = btnStartGame.gameObject.AddComponent<EventTrigger>();
            }

            // 마우스 오버 이벤트
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => { OnHoverEnterStartGame(); });

            // 마우스 나가기 이벤트
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnHoverExitStartGame(); });

            // 이벤트 트리거에 추가
            eventTrigger2.triggers.Add(pointerEnter);
            eventTrigger2.triggers.Add(pointerExit);
        }
    }

    private void ActivePnlStartScene()  // #51 "Invoke"실행 - 처음 로딩될 때 pnlLoading 먼저 보이고, 그 다음에 pnlStartScnee이 보이도록 
    {
        if(!pnlStartScene.activeSelf)
            pnlStartScene.SetActive(true);

        // #50 시작 화면 효과 주기 위한 Animator
        anim.SetTrigger("StartScene");
    }

    public void StartGame() // #19 시작하자마자 화면 전환
    {
        if(pnlStartScene != null)   // #19 게임 시작하면, 필요한 UI는 남기고 pnlStartScene 오브젝트 비활성화 - 게임 화면 보이도록 하기 위함.
        {
            pnlStartScene.SetActive(false);
        }
        
        music.BackGroundMusic(Music.BGM_TYPE.MAINMUSIC);

        SceneManager.LoadScene("scStage1-3D");
    }

    public void ShowHowToGame() // #49
    {
        Debug.Log("#49 어떻게 게임하는지 보여주기!");

        // #52 '게임 방법' 버튼 눌렀을 때, '게임 방법' 보여주는 화면 보이도록 하기
        if(!pnlHowToGameScreen.activeSelf)  
        {
            pnlHowToGameScreen.SetActive(true);
        }
    }


    // #49 '게임 방법' 마우스 오버 시 패널 활성화
    private void OnHoverEnterHowToGame()
    {
        if(pnlbtnPressHowToGame != null)
        {
            pnlbtnPressHowToGame.SetActive(true);
            music.GameSoundEffect(Music.EFFECT_TYPE.BUTTON_HOVER);  // #49 버튼에 마우스 hover 했을 때 효과음
        }
    }

    // #49 '게임 방법' 마우스 나갈 시 패널 비활성화
    private void OnHoverExitHowToGame()
    {
        if(pnlbtnPressHowToGame != null)
        {
            pnlbtnPressHowToGame.SetActive(false);
        }
    }

    // #49 '게임 방법' 마우스 오버 시 패널 활성화
    private void OnHoverEnterStartGame()
    {
        if(pnlbtnPressGameStart != null)
        {
            pnlbtnPressGameStart.SetActive(true);
            music.GameSoundEffect(Music.EFFECT_TYPE.BUTTON_HOVER);  // #49 버튼에 마우스 hover 했을 때 효과음
        }
    }

    // #49 '게임 방법' 마우스 나갈 시 패널 비활성화
    private void OnHoverExitStartGame()
    {
        if(pnlbtnPressGameStart != null)
        {
            pnlbtnPressGameStart.SetActive(false);
        }
    }

}
