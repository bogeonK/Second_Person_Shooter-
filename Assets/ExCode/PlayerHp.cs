using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHp : MonoBehaviour
{
    // �÷��̾� ü�¹� (�Ӹ� ��)
    public Slider hpBar_World;   
    // �÷��̾� ü��
    public float hp = 100;

    // �÷��̾��� PhotonView ������Ʈ
    PhotonView pv;
    // �÷��̾��� Animator ������Ʈ
    Animator anim; 

    // �÷��̾��� ü�¹� (ȭ��)
    Slider hpBar_Screen;
    // ĵ���� ������Ʈ
    Transform canvas;  

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();

        // �� ĳ������ ���� ����
        if (pv.IsMine)
        {
            // Canvas �˻��ؼ� ��������
            canvas = GameObject.Find("Canvas").transform;

            // Canvas�� �θ� ���� ����
            canvas.SetParent(transform);

            // ȭ�� ���� ��ܿ� �ִ� �÷��̾��� ü�¹� ��������
            hpBar_Screen = canvas.GetComponentInChildren<Slider>();
        }
        
    }

    // Update is called once per frame
    public void Damaged(float damage, int hitter)
    {
        // ���� �޾����� RPC������� ��� ����
        pv.RPC("RPC_Damaged", RpcTarget.All, damage, hitter);

    }

    [PunRPC]
    void RPC_Damaged(float damage, int hitterViewID)
    {
        // ���� ���� ��������ŭ ü�� ����
        hp -= damage;

        // ü�¹ٿ� ü�� ǥ��
        hpBar_World.value = hp;


        // ȭ�� ü�¹ٿ� ǥ���ϴ� �� �� ĳ������ ����
        if (pv.IsMine)
        {
            // ü�� ǥ��
            hpBar_Screen.value = hp;

            if (hp > 0) // ü���� ���� ��
            {
                // �ǰ� �ִϸ��̼�
            }
            else
            {
                // �״� ���
                anim.SetTrigger("dead");
                // �й� �� ����
                StartCoroutine(Ending("�й�"));
            }
        }

        // ĳ���� ü���� ���� ��
        if (hp <= 0)
        {
            // ������ �ѿ� �� �°� �±� ����
            tag = "Untagged";

            // �÷��̾� ��� �ߴ�
            GetComponent<Player>().enabled = false;
            GetComponent<PlayerShoot>().enabled = false;

            // �¸� �÷��̾��� PhotonView�� hitterViewID�� ã��
            PhotonView hitterPv = PhotonNetwork.GetPhotonView(hitterViewID);
            if (hitterPv != null)
            {
                PlayerHp winner = hitterPv.GetComponent<PlayerHp>();
                if (winner != null)
                {
                    // �¸� �÷��̾�� �¸� ����
                    StartCoroutine(winner.Ending("�¸�"));
                }
                else
                {
                    Debug.LogError("�¸� �÷��̾��� PlayerHp ������Ʈ�� ã�� �� �����ϴ�.");
                }
            }
            else
            {
                Debug.LogError("hitterViewID�� �ش��ϴ� PhotonView�� ã�� �� �����ϴ�. ViewID: " + hitterViewID);
            }
        }
    }

    // DeadZone���� �浹�� �����ϴ� �Լ� ����
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            // DeadZone�� ���� �÷��̾��� ViewID
            int deadPlayerViewID = pv.ViewID;

            // ��� �÷��̾��� PhotonView�� ��������
            PhotonView[] allPlayers = FindObjectsOfType<PhotonView>();

            // ���� ���� �÷��̾��� ViewID ã��
            int otherPlayerViewID = -1; // �⺻�� ����

            foreach (PhotonView playerView in allPlayers)
            {
                if (playerView.ViewID != deadPlayerViewID) // DeadZone�� ���� ���� �÷��̾� ã��
                {
                    otherPlayerViewID = playerView.ViewID;
                    break;
                }
            }

            // DeadZone�� ������� RPC�� ��� Ŭ���̾�Ʈ���� DeadPlayer�� OtherPlayer�� ViewID ����
            pv.RPC("RPC_DeadZoneDeath", RpcTarget.All, deadPlayerViewID, otherPlayerViewID);
        }
    }

    [PunRPC]
    void RPC_DeadZoneDeath(int deadPlayerViewID, int otherPlayerViewID)
    {
        if (pv.IsMine)
        {
            // DeadZone�� ���� �÷��̾� ó��
            if (pv.ViewID == deadPlayerViewID)
            {
                hp = 0;

                // ü�¹� ������Ʈ
                hpBar_World.value = hp;

                // �� ĳ������ ��� ȭ�� ü�¹� ������Ʈ �� �й� ����
                StartCoroutine(Ending("�й�"));
            }
        }

        // ��� �÷��̾�� hp <= 0 Ȯ��
        if (pv.ViewID == deadPlayerViewID)
        {
            // ������ �ѿ� �� �°� �±� ����
            tag = "Untagged";

            // �÷��̾� ��� �ߴ�
            GetComponent<Player>().enabled = false;
            GetComponent<PlayerShoot>().enabled = false;

            // �¸� �÷��̾� PlayerHp ������Ʈ ��������
            PlayerHp winnerPlayer = PhotonNetwork.GetPhotonView(otherPlayerViewID).GetComponent<PlayerHp>();

            // �¸� �÷��̾�� �¸� ����
            StartCoroutine(winnerPlayer.Ending("�¸�"));
        }
    }

    // �������� �����
    public IEnumerator Ending(string result)
    {
        // �� ĳ���� �ƴ� ��, �Լ� Ż��
        if (!pv.IsMine) yield break;

        // 1�ʵ��ȴ��
        yield return new WaitForSeconds(1f);

        Transform ending = canvas.GetChild(2);

        // ���Ӱ��
        ending.GetChild(0).GetComponent<Text>().text = "- " + result + " -";

        // ��� Ȱ��ȭ
        ending.gameObject.SetActive(true);

        // �÷��̾� ��� �ߴ�
        GetComponent<Player>().enabled = false;
        GetComponent<PlayerShoot>().enabled = false;
        GetComponentInChildren<Camera>().enabled = false;

        // ���� ������ ��� ����
        StartCoroutine(FindObjectOfType<PlaySceneManager>().AfterEnding());
    }
}
