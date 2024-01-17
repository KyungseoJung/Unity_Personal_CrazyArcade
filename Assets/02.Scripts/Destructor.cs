using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructor : MonoBehaviour // #9 물풍선 없애기 - 애니메이터에서 실행
{
   public bool destroyOnAwake;      // 인스펙터에서 체크시 Awake() 에서 이 게임오브젝트의 해당 자식객체 소멸 또는 자기자신을 일정 시간 딜레이후 소멸
   public float awakeDestroyDelay;  // Awake() 에서 사용되는 소멸을 위한 딜레이 타임
   public bool findChild = false;    // 인스펙터에서 체크시 해당 자식게임오브젝트를 찾아서 소멸한다.
   public string namedChild;        // 인스펙터에서 설정 가능한 소멸될 자식객체의 이름

   void Start()   // Awake를 Start로 변경
   {
      //만약에 destroyOnAwake가 true일 때
      if(destroyOnAwake)
      {
         if(findChild)
         {
            //이 게임오브젝트의 해당 자식객체 소멸
            Destroy(transform.Find(namedChild).gameObject);
         }
         else
         {
            //딜레이 후 게임 오브젝트를 소멸
            Destroy(gameObject, awakeDestroyDelay);
         }
         }
      }

   //이 함수는 Animation Even로부터 호출될 수 있다.
   void DestroyChildGameObject()
   {
      //이 게임오브젝트의 해당 이름의 자식객체가 존재할 경우 그 child gameobject 소멸
      if(transform.Find(namedChild).gameObject != null)
      {
         Destroy(transform.Find(namedChild).gameObject);
      }
   }

   //이 함수는 Animation Even로부터 호출될 수 있다.
   void DisableChildGameObject()
   {
      //이 게임오브젝트의 해당 이름의 자식객체가 활성화중일 경우 그 child gameobject 소멸
      if(transform.Find(namedChild).gameObject.activeSelf ==true)
      {
         transform.Find(namedChild).gameObject.SetActive(false);
      }
   }

   //이 함수는 Animation Even로부터 호출될 수 있다.
   void DestroyGameObject()
   {
      Debug.Log("//#57 오브젝트 파괴");
      //이 게임오브젝트 삭제
      Destroy(gameObject);
   }




   }


