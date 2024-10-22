using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviour
{
    // �̵� �ӵ�
    public float moveSpeed;
    public float jumpPower; // �����ϴ� ��

    PhotonView pv;  // �÷��̾��� PhotonView ������Ʈ

    int jumpCount; // ���� Ƚ��

    Rigidbody rb; // �÷��̾��� Rigidbody ������Ʈ
    Animator anim; // �÷��̾��� Animator ������Ʈ

    // ĵ���� ������Ʈ
    Transform canvas;


    // Start is called before the first frame update
    void Awake()
    {
        // �÷��̾��� Rigidbody,PhotonView, Animator ������Ʈ�� �����ͼ� ����
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();

        // �� ĳ���� �϶��� ����
        if (pv.IsMine)
        {
            // Canvas �˻��ؼ� ��������
            canvas = GameObject.Find("Canvas").transform;

            // Canvas�� �θ� ���� ����
            canvas.SetParent(transform);

            // Main Camera �˻��ؼ� ��������
            Transform camera = Camera.main.transform;

            // Main Camera�� �θ� ���� ����
            camera.SetParent(transform);

            // ���� �������� ������ ��ġ�� �̵�
            camera.localPosition = new Vector3(-4f, 2f, 0);

            // ī�޶��� ���� ȸ������ ���� (Quaternion���� ��ȯ)

            camera.localRotation = Quaternion.Euler(new Vector3(0, 90f, 0));  // ������ ������ ȸ��
        }
    }

    // Update is called once per frame
    void Update()
    {
        // �� ĳ���Ͱ� �ƴ϶�� �Լ��� Ż���Ͽ� �Ʒ� �ڵ� ���� �Ұ�
        if (!pv.IsMine) return;

        // ����Ű �Ǵ� WASDŰ �Է��� ���ڷ� �޾Ƽ� ����
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // x�࿡��  h�� ����, z�࿡�� v�� ���� ���� ���� ����
        Vector3 dir = new Vector3(h, 0, v);

        // ��� ������ �ӵ��� �����ϵ��� ����ȭ
        dir.Normalize();

        // �̵��� ���⿡ ���ϴ� �ӵ� ���ϱ� (��� ��⿡�� ������ �ӵ�)
        // transform.position += dir * moveSpeed * Time.deltaTime;

        // ���� �ۿ��� �̿��� ����
        rb.MovePosition(rb.position + (dir * moveSpeed * Time.deltaTime));

        // �̵��ϴ� �ӵ��� velocity ������ �Ҵ��� �ִϸ��̼� ��ȯ
        anim.SetFloat("velocity", dir.magnitude);

        // Space Ű�� ���� ��, ������ Ƚ���� 2ȸ �̸�
        if (Input.GetKey(KeyCode.Space) && jumpCount < 2)
        {
            // ���� �� �߻�
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

            // ���� �ִϸ��̼� ����
            anim.SetTrigger("jump");
            anim.SetBool("isJump", true);

            // ������ ������ ���� Ƚ�� ����
            jumpCount++;
        }

        // ���콺 ��Ŭ������ �� �߻� �ִϸ��̼�
        if (Input.GetMouseButton(0))
        {
            anim.SetTrigger("shoot");
        }
    }

    // � ��ü�� �浹�� ������ ������ ȣ��
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ��ü�� �±װ� "Ground"�̰�, �� ĳ������ ����
        if(collision.gameObject.tag == "Ground" && pv.IsMine)
        {
            //  ���� Ƚ�� �ʱ�ȭ
            jumpCount = 0;

            // ���� �ִϸ��̼� ����
            anim.SetBool("isJump", false);
           
        }
    }

}
