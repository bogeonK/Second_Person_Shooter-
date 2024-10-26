using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlaySceneManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPoints;   // �÷��̾� ���� ��ġ

    // Start is called before the first frame update
    void Start()
    {
        // ���� �濡 ������ �÷��̾� �ο�
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        // �� �÷��̾��� ȸ�� �� ����
        Quaternion playerRotation;

        // �÷��̾� �ο��� ���� �ٸ� ���� ��ġ�� ȸ�� ����
        if (playerCount == 1)
        {
            // ù ��° �÷��̾�� ����(0��)�� �ٶ�
            playerRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (playerCount == 2)
        {
            // �� ��° �÷��̾�� ����(180��)�� �ٶ�
            playerRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            // �⺻�� ���� (�ٸ� �ο��� �߰��� ���)
            playerRotation = Quaternion.Euler(0, 0, 0);
        }

        // �÷��̾� �ο��� ���� �ٸ� ���� ��ġ�� �÷��̾� ���� (1���̸� 0��, 2���̸� 1��)
        PhotonNetwork.Instantiate("Player", playerSpawnPoints[playerCount - 1].position, playerRotation);
    }

    // ���� ���� ����
    public IEnumerator AfterEnding()
    {
        // �濡�� ������ ������ �ݼ�
        while (PhotonNetwork.InRoom)
        {
            // �ƹ�Ű �Էµƴٸ�
            if (Input.anyKeyDown)
            {
                // �濡�� ���� �õ�
                PhotonNetwork.LeaveRoom();

                // �ݺ��� ���� �� �Լ� Ż��
                break;
            }
            // ���� �����ӱ��� ����
            yield return null;
        }

    }

    // �� ���� ���� �� ȣ��
    public override void OnLeftRoom()
    {
        // StartScene �� �ҷ�����
        PhotonNetwork.LoadLevel("StartScene");

        // ���콺 Ŀ�� ���̰�
        Cursor.visible = true;

        // ���콺 Ŀ���� ������ �� �ֵ��� ��� ����
        Cursor.lockState = CursorLockMode.None;
    }
}
