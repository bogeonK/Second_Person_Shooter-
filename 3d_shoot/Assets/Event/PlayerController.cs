using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Movement3D movement3D;
    private KeyCode jumpKeyCode = KeyCode.Space;

    private void Awake()
    {
        movement3D = GetComponent<Movement3D>();
    }

    private void Update()
    {
        // x, z ���� �̵�
        float x = Input.GetAxisRaw("Horizontal"); // ����Ű ��/�� ������
        float z = Input.GetAxisRaw("Vertical"); // ����Ű ��/�Ʒ� ������

        movement3D.MoveTo(new Vector3(x, 0, z));

        // ����Ű(�����̽�)�� ���� y�� �������� �پ������
        if (Input.GetKeyDown(jumpKeyCode))
        {
            movement3D.JumpTo();
        }
    }
}
