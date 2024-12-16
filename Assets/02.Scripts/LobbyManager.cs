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

    // 여기부터 - 로딩 화면 이미지  ========================================
    [SerializeField] GameObject pnlLoading0;                 // #53 로딩(로딩바) 화면 설정 - 처음 로딩될 때 pnlLoading0이 가장 먼저 보이도록 
    [SerializeField] GameObject pnlLoading1;                 // #51 처음 로딩될 때 pnlLoading1 먼저 보이고, 그 다음에 pnlStartScnee이 보이도록
    [SerializeField] GameObject objLogo_START;               // #19 처음 게임 시작할 때, START 로고 보이도록

    [SerializeField] GameObject pnlbtnPressHowToGame;       // 버튼 눌렀을 때 보이는 panel ('게임 방법' 버튼)
    [SerializeField] GameObject pnlbtnPressGameStart;       // 버튼 눌렀을 때 보이는 panel ('게임 시작' 버튼)
    [SerializeField] GameObject pnlbtnPressGameStart2;       // #49 버튼 눌렀을 때 보이는 panel ('게임 방법' 화면에 있는 '게임 시작' 버튼)
    [SerializeField] GameObject pnlHowToGameScreen;         // #52 '게임 방법' 버튼 눌렀을 때, '게임 방법' 보여주는 화면 보이도록 하기
    
    [SerializeField] Image imgNexonLogo;                 // #51 처음 로딩될 때 pnlLoading1에서 Nexon Logo 보이도록
    [SerializeField] Image imgBnbLogo;                  // #51 처음 로딩될 때 pnlLoading1에서 BnB Logo 보이도록
    private float fadeDuration = 1f;                    // #51 투명도가 변하는 데 걸리는 시간

    // 여기까지 - 로딩 화면 이미지  ========================================

    private Animator startSceneAnim;  // #50 시작 화면 효과 주기 위한 Animator
    private Animator howToGameAnim;     // #50 게임 방법 화면 시작할 때 효과 주기 위한 Animator
    private Animator loadingSceneAnim;     // #53 로딩 화면 애니메이션 설정하기 위한 Animator
    public Text txtPlayerLife;                    // #27 플레이어 목숨 표시

    // #53 여기부터 - 로딩바 이미지  ========================================
    [SerializeField] GameObject imgLoadingText;      // #53 로딩 텍스트 처음에는 보이지 않도록
    [SerializeField] Image imgProgressBar;  
    [SerializeField] float time_loadingbar = 0.7f; // 0.7초만에 로딩바가 모두 채워지도록
    [SerializeField] float time_passed; // 처음부터 현재까지 지난 시간
    [SerializeField] float time_start;
    [SerializeField] bool loadingEnded;

    // #53 여기까지 - 로딩바 이미지  ========================================

    [SerializeField] Button btnHowToGame;           // #49 '게임 방법' 버튼
    [SerializeField] Button btnStartGame;           // #49 '게임 시작' 버튼
    [SerializeField] Button btnStartGame2;           // #49 '게임 시작' 버튼

    // #58 여기부터 - UI 이미지  ========================================
    public Text txtNumberOfCan;                    // 느린 거북을 빠르게 해주는 can item
    public Text txtNumberOfNeedle;                 // 물풍선에 갇혔을 때, 벗어나게 해주는 needle item
    public Text txtNumberOfShield;                 // 외부 공격으로부터 막아주는 shield item


    void Awake()
    {
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>(); // #49
        loadingSceneAnim = pnlLoading0.transform.GetComponent<Animator>();      // #53 로딩 화면 설정

        // #50 시작 화면 효과 주기 위한 Animator
        startSceneAnim = pnlStartScene.transform.GetComponent<Animator>();

        howToGameAnim = pnlHowToGameScreen.transform.GetComponent<Animator>();  // #50 게임 방법 화면 시작할 때 효과 주기 위한 Animator
    }

    void Start()
    {   
        // #53 로딩바 이미지 설정
        Loading_Reset();    // 로딩바 첫 설정 해주기
        loadingSceneAnim.SetTrigger("startLoading");  // #53 로딩 화면 설정 - 로딩바 올라가는 애니메이션 시작하도록
        if(!imgLoadingText.activeSelf)   // #53 처음에는 로딩 text 보이지 않은 상태라면, 활성화해서 시작되도록
        {
            imgLoadingText.SetActive(true);
        }


        // #51 처음 로딩될 때 pnlLoading1 먼저 보이고, 그 다음에 pnlStartScnee이 보이도록
        if(pnlStartScene.activeSelf)
            pnlStartScene.SetActive(false);
        if(!pnlLoading0.activeSelf)         // #51 맨 처음 로딩 화면 설정
            pnlLoading0.SetActive(true);
        if(pnlLoading1.activeSelf)          // #51 맨 처음 로딩 화면 설정
            pnlLoading1.SetActive(false);
        if(imgNexonLogo.gameObject.activeSelf)  // #51 맨 처음 로딩 화면 설정
            imgNexonLogo.gameObject.SetActive(false);
        if(imgBnbLogo.gameObject.activeSelf)    // #51 맨 처음 로딩 화면 설정
            imgBnbLogo.gameObject.SetActive(false);
        if(objLogo_START.activeSelf)        // #19 맨 처음 로딩 화면 설정 | 게임 처음 시작할 때 보이는 START 로고 처음에는 보이지 않도록
            objLogo_START.SetActive(false);
        if(pnlHowToGameScreen.activeSelf)   // 처음 시작할 때, 게임 방법 보여주는 창은 비활성화 되어 있어야 함.
            pnlHowToGameScreen.SetActive(false);
        
        // Invoke("ActivePnlStartScene", 3f);  // #51 나중에 고쳐야 하는 코드
        
        // StartGame(); // #49 '게임 시작' 버튼 누르면, StartGame() 함수 실행되도록 하기
        // #49 특정 버튼에 대해 함수를 연결하는 부분을 인스펙터(Inspector)에서 했었음. -> 코드상으로 설정하는 방식으로 변경하기.
        if(btnHowToGame != null)
        {
            // btnStartGame.onClick.AddListener(ShowHowToGame);
            // #49 버튼 클릭할 때의 효과음 추가
            btnHowToGame.onClick.AddListener(() => {
                ShowHowToGame();
                music.GameSoundEffect(Music.EFFECT_TYPE.BUTTON_CLICK);
            });

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
            // btnStartGame.onClick.AddListener(StartGame);
            // #49 버튼 클릭할 때의 효과음 추가
            btnStartGame.onClick.AddListener(() => {
                StartGame();
                music.GameSoundEffect(Music.EFFECT_TYPE.BUTTON_CLICK);
            });

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
    
        if(btnStartGame2 != null)
        {
            // btnStartGame2.onClick.AddListener(StartGame);
            // #49 버튼 클릭할 때의 효과음 추가
            btnStartGame2.onClick.AddListener(() => {
                StartGame();
                music.GameSoundEffect(Music.EFFECT_TYPE.BUTTON_CLICK);
            });

            // #49 feat '게임 시작' 버튼에 마우스 올려 놓으면, '게임 시작' 버튼이 더 밝게 빛나도록 
            // EventTrigger 컴포넌트가 없으면 추가
            EventTrigger eventTrigger3 = btnStartGame2.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger3 == null)
            {
                eventTrigger3 = btnStartGame2.gameObject.AddComponent<EventTrigger>();
            }

            // 마우스 오버 이벤트
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => { OnHoverEnterStartGame2(); });

            // 마우스 나가기 이벤트
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnHoverExitStartGame2(); });

            // 이벤트 트리거에 추가
            eventTrigger3.triggers.Add(pointerEnter);
            eventTrigger3.triggers.Add(pointerExit);
        }

        // #58 여기부터 - UI 이미지  ========================================
        txtNumberOfCan.text = $"{PlayerGameMgr.Mgr.turtleCan}";
        txtNumberOfNeedle.text = $"{PlayerGameMgr.Mgr.needle}";
        txtNumberOfShield.text = $"{PlayerGameMgr.Mgr.shield}";
    }
    
    private void Update()
    {
        if(loadingEnded)
            return;
        // #53 로딩바 이미지 설정
        Loading_Check();    // 로딩바 길이 체크해서 채우기
    }

    private void ActivePnlStartScene()  // #51 "Invoke"실행 - 처음 로딩될 때 pnlLoading1 먼저 보이고, 그 다음에 pnlStartScnee이 보이도록 
    {
        // #50 fix: (로딩 모두 끝난 뒤) 로비 화면 입장할 때, 로비 BGM 시작
        music.BackGroundMusic(Music.BGM_TYPE.LOBBYMUSIC);

        if(pnlLoading0.activeSelf)
            pnlLoading0.SetActive(false);
        if(pnlLoading1.activeSelf)
            pnlLoading1.SetActive(false);

        if(!pnlStartScene.activeSelf)
            pnlStartScene.SetActive(true);

        // #50 시작 화면 효과 주기 위한 Animator
        startSceneAnim.SetTrigger("StartScene");
    }

    // 여기부터 - #53 로딩바 이미지  ========================================

    private void Loading_Reset()
    {
        time_start = Time.time;
        FillLoadingBar(0);
        loadingEnded = false;
    }

    private void Loading_Check()
    {
        time_passed = Time.time - time_start;
        if(time_passed < time_loadingbar)
        {
            FillLoadingBar(time_passed / time_loadingbar);
        }
        else if (!loadingEnded) // #53 로딩 화면 시작하고나서 지나간 시간이 time_loadingbar 길고 && 로딩이 끝난 상황도 아니라면 Loading_End 실행
        {
            Loading_End();
        }

    }
    private void Loading_End()
    {
        FillLoadingBar(1);
        loadingEnded = true;

        if(!pnlLoading1.activeSelf)  // #51 Nexon 로고와 BnB 로고 보이는 panel 로 넘어가기
            pnlLoading1.SetActive(true);
        
        StartCoroutine(FadeInOutNexonLogo());
    }

    private void FillLoadingBar(float _fill)
    {
        imgProgressBar.fillAmount = _fill;
        // Debug.Log("//#53 로딩 정도: " + _fill * 100 + "%");
    }

    // 여기까지 - #53 로딩바 이미지  ========================================

    IEnumerator FadeInOutNexonLogo() // #51 로딩 화면에서 로고 이미지 잠깐 나타났다가 사라지도록 Fade In Out
    {
        if(!imgNexonLogo.gameObject.activeSelf)     // 비활성화 되어 있다면, 일단 활성화 해놓고 페이드 인 & 아웃 하기
            imgNexonLogo.gameObject.SetActive(true);

        // 투명도를 0에서 1로 증가 (페이드 인)
        yield return StartCoroutine(Fade(imgNexonLogo, 0f, 1f));

        // 투명도를 1에서 0으로 감소 (페이드 아웃)
        yield return StartCoroutine(Fade(imgNexonLogo, 1f, 0f));

        StartCoroutine(FadeInOutBnbLogo());
    }

    IEnumerator FadeInOutBnbLogo() // #51 로딩 화면에서 로고 이미지 잠깐 나타났다가 사라지도록 Fade In Out
    {
        if(!imgBnbLogo.gameObject.activeSelf)     // 비활성화 되어 있다면, 일단 활성화 해놓고 페이드 인 & 아웃 하기
            imgBnbLogo.gameObject.SetActive(true);

        // 투명도를 0에서 1로 증가 (페이드 인)
        yield return StartCoroutine(Fade(imgBnbLogo, 0f, 1f));

        // 투명도를 1에서 0으로 감소 (페이드 아웃)
        yield return StartCoroutine(Fade(imgBnbLogo, 1f, 0f));

        ActivePnlStartScene();  // #51 로딩 화면 모두 끝난 뒤, pnlStartScene 활성화 되도록
    }

    IEnumerator Fade(Image _img, float startAlpha, float endAlpha)  // #51 특정 이미지의 투명도가 점진적으로 바뀌도록
    {
        Color imgColor = _img.color;
        float elapsedTime = 0f;

        while(elapsedTime < fadeDuration)
        {
            imgColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            _img.color = imgColor;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 마지막 알파 값 (투명도) 적용
        imgColor.a = endAlpha;
        _img.color = imgColor;
    }

    public void StartGame() // #19 시작하자마자 화면 전환
    {
        if(pnlStartScene != null)   // #19 게임 시작하면, 필요한 UI는 남기고 pnlStartScene 오브젝트 비활성화 - 게임 화면 보이도록 하기 위함.
        {
            pnlStartScene.SetActive(false);
        }

        if((pnlHowToGameScreen != null) && (pnlHowToGameScreen.activeSelf)) // #19 게임 시작하면, 필요한 UI는 남기고 pnlHowToGameScreen 오브젝트 비활성화 - 게임 화면 보이도록 하기 위함.
        {
            pnlHowToGameScreen.SetActive(false);
        }

        if((pnlLoading1 != null) && (pnlLoading1.activeSelf)) // #19 게임 시작하면, 필요한 UI는 남기고 pnlLoading1 오브젝트 비활성화 - 게임 화면 보이도록 하기 위함.
        {
            pnlLoading1.SetActive(false);
        }
        
        if(!objLogo_START.activeSelf)   // #19 게임 시작화면 설정 | START 로고 보이도록
            objLogo_START.SetActive(true);

        music.BackGroundMusic(Music.BGM_TYPE.MAINMUSIC);

        // music.GameSoundEffect(Music.EFFECT_TYPE.GAME_START);    // #19 게임 시작 효과음
        music.PlayerSoundEffect(Music.EFFECT_TYPE.GAME_START);    // #19 게임 시작 효과음
        // #19 fix: '게임 시작' 효과음이 들리지 않는 문제 해결하라 - 하나의 Audio Source에서 거의 동시에 2개의 효과음을 실행해서 생기는 문제를, 다른 AudioSource를 실행함으로써

        Debug.Log("//#19 게임 스타트! 효과음");

        SceneManager.LoadScene("scStage1-3D");
    }

    public void ShowHowToGame() // #49
    {
        Debug.Log("#49 어떻게 게임하는지 보여주기!");

        // #52 '게임 방법' 버튼 눌렀을 때, '게임 방법' 보여주는 화면 보이도록 하기
        if(!pnlHowToGameScreen.activeSelf)  
        {
            pnlHowToGameScreen.SetActive(true);

            // #50 게임 방법 화면 시작할 때 효과 주기 위한 Animator
            howToGameAnim.SetTrigger("ShowHowToGame");
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

    // #49 '게임 방법' 마우스 오버 시 패널 활성화
    private void OnHoverEnterStartGame2()
    {
        if(pnlbtnPressGameStart2 != null)
        {
            pnlbtnPressGameStart2.SetActive(true);
            music.GameSoundEffect(Music.EFFECT_TYPE.BUTTON_HOVER);  // #49 버튼에 마우스 hover 했을 때 효과음
        }
    }

    // #49 '게임 방법' 마우스 나갈 시 패널 비활성화
    private void OnHoverExitStartGame2()
    {
        if(pnlbtnPressGameStart2 != null)
        {
            pnlbtnPressGameStart2.SetActive(false);
        }
    }

    public void UpdateNumberOfItems()  //#59 UI에 나타나는 아이템의 개수 업데이트
    {
        txtNumberOfCan.text = $"{PlayerGameMgr.Mgr.turtleCan}";
        txtNumberOfNeedle.text = $"{PlayerGameMgr.Mgr.needle}";
        txtNumberOfShield.text = $"{PlayerGameMgr.Mgr.shield}";   
    }
}
