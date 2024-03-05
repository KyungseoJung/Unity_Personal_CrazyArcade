using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSetting : MonoBehaviour   // #2
{
    SpriteRenderer sprite;

    private void Awake()
    {
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

    }

    private void Start()    // #2
    {
        SetSortingOrder();
    }

    public void SetSortingOrder()
    {
        sprite.sortingOrder = - Mathf.RoundToInt(transform.position.y);  // y축 기준으로 밑에 있을수록 더 위에 그려져야 하므로 반비례 -> -1
    }
}
