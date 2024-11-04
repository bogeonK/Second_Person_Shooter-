using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlaySceneManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPoints;  // �÷��̾� ���� ��ġ
    public Text statusText;                // ���� �޽��� �ؽ�Ʈ
    public Text countdownText;             // ī��Ʈ�ٿ� �ؽ�Ʈ

    private void Start()
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
            statusText.text = "Waiting for player...";  // ù ��° �÷��̾� ��� �޽���
        }
        else if (playerCount == 2)
        {
            // �� ��° �÷��̾�� ����(180��)�� �ٶ�
            playerRotation = Quaternion.Euler(0, 180, 0);
            statusText.text = "";  // �� ��° �÷��̾� ���� �� ��� �޽��� ����
            StartCoroutine(StartCountdown()); // �� ��° �÷��̾ �����ϸ� ī��Ʈ�ٿ� ����
        }
        else
        {
            playerRotation = Quaternion.Euler(0, 0, 0);
        }

        // �÷��̾� �ο��� ���� �ٸ� ���� ��ġ�� �÷��̾� ����
        PhotonNetwork.Instantiate("Player", playerSpawnPoints[playerCount - 1].position, playerRotation);
    }

    // �ٸ� �÷��̾ �濡 ������ �� ȣ�� (������ Ŭ���̾�Ʈ������ ȣ���)
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            statusText.text = "";  // �� ��° �÷��̾� ���� �� ��� �޽��� ����
            StartCoroutine(StartCountdown()); // �� �÷��̾� ���� �� ī��Ʈ�ٿ� ����
        }
    }

    // ī��Ʈ�ٿ� �ڷ�ƾ
    private IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);  // ī��Ʈ�ٿ� �ؽ�Ʈ Ȱ��ȭ

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);  // 1�� ���
        }

        countdownText.text = "Go!"; // ������ "Go!" �ؽ�Ʈ ǥ��
        yield return new WaitForSeconds(1); // 1�� ���
        countdownText.gameObject.SetActive(false);  // ī��Ʈ�ٿ� �ؽ�Ʈ ��Ȱ��ȭ

    }


    // ���� ���� ����
    public IEnumerator AfterEnding()
    {
        // �濡�� ������ ������ �ݺ�
        while (PhotonNetwork.InRoom)
        {
            if (Input.anyKeyDown)
            {
                PhotonNetwork.LeaveRoom();
                break;
            }
            yield return null;
        }
    }

    // �� ���� ���� �� ȣ��
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("StartScene");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
