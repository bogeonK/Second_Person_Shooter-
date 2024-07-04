using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 이동 속도
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 방향키 또는 WASD키 입력을 숫자로 받아서 저장
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // x축에서  h의 값을, z축에는 v의 값을 넣은 변수 생성
        Vector3 dir = new Vector3(h, 0, v);

        // 모든 방향의 속도가 동일하도록 정규화
        dir.Normalize();

        // 이동할 방향에 원하는 속도 곱하기 (모든 기기에서 동일한 속도)
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
