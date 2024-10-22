using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerFire : MonoBehaviour
{
    // �� ȿ�� ������ ��Ƶ� ����
    public GameObject shootEffectPref;


    PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();


        // ���콺 Ŀ�� �����
        Cursor.visible = false;

        // ���콺 Ŀ���� ����ȭ���� ����� ���ϰ� ���
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        // ���콺 ��Ŭ�������� ����, �� ĳ���� �� ����
        if (Input.GetMouseButtonDown(0)&& pv.IsMine)
        {


            // ȭ�� ������� �����ϴ� Ray ����
            Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));


            // Ray ���� ��ü�� ��Ƶ� ����
            RaycastHit hit;

            // Ray�� �߻��ϰ�, Ray�� ���� ��ü�� hit�� ����, ���� ��ü�� ��������
            if(Physics.Raycast(ray, out hit))
            {
                // ���� ��ġ��, ���� ǥ���� ������ �Ǵ� ������ �� ȿ�� �������� ���纻 ����
                GameObject shootEffect = Instantiate(shootEffectPref, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));

                // �Ѿ� �ڱ��� ���� ������Ʈ�� �ڽ����� ����
                shootEffect.transform.SetParent(hit.transform);
                
                // Ray�� ���� ��ü�� ���̶��
                if (hit.transform.tag == "Enemy")
                {
                    // ������ 10��ŭ ������
                    hit.transform.SendMessage("Damaged", 10);
                }

                // Ray�� ���� ��ü�� �÷��̾��̶��
                if (hit.transform.tag == "Player")
                {
                    // �÷��̾�� 10��ŭ ������, ���� ViewID ����
                    hit.transform.GetComponent<PlayerHp>().Damaged(10, pv.ViewID);
                }
            }
        }
    }
}
