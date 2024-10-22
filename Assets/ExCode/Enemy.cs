using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // UI관련 클래스 사용

public class Enemy : MonoBehaviour
{
    // 적 체력바
    public Slider hpBar;
    // 적 체력
    public float hp = 100;

    // 공격 받는 기능
    void Damaged(float damage)
    {
        // 공격 받은 데미지만큼 체력 감소
        hp -= damage;

        // 감소한 체력을 hp바에 표시
        hpBar.value = hp;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
