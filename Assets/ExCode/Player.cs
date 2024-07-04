using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // �̵� �ӵ�
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ����Ű �Ǵ� WASDŰ �Է��� ���ڷ� �޾Ƽ� ����
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // x�࿡��  h�� ����, z�࿡�� v�� ���� ���� ���� ����
        Vector3 dir = new Vector3(h, 0, v);

        // ��� ������ �ӵ��� �����ϵ��� ����ȭ
        dir.Normalize();

        // �̵��� ���⿡ ���ϴ� �ӵ� ���ϱ� (��� ��⿡�� ������ �ӵ�)
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
