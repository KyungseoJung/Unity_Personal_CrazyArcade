using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private Animator anim;  // #6 덤불 Animator 조정

    void Awake()
    {
        anim = transform.GetComponent<Animator>();     
    }

    public void BushShake() // #6 애니메이터 설정: 플레이어가 덤불에 숨으면, 덤불 흔들리도록 
    {
        Debug.Log("//#6 덤불 흔들림");
        anim.SetTrigger("Shake");
    }
}   
