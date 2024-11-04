using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlaySceneManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPoints;  // 플레이어 스폰 위치
    public Text statusText;                // 상태 메시지 텍스트
    public Text countdownText;             // 카운트다운 텍스트

    private void Start()
    {
        // 현재 방에 참여한 플레이어 인원
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        // 각 플레이어의 회전 값 설정
        Quaternion playerRotation;

        // 플레이어 인원에 따라 다른 스폰 위치와 회전 설정
        if (playerCount == 1)
        {
            // 첫 번째 플레이어는 정면(0도)을 바라봄
            playerRotation = Quaternion.Euler(0, 0, 0);
            statusText.text = "Waiting for player...";  // 첫 번째 플레이어 대기 메시지
        }
        else if (playerCount == 2)
        {
            // 두 번째 플레이어는 뒤쪽(180도)을 바라봄
            playerRotation = Quaternion.Euler(0, 180, 0);
            statusText.text = "";  // 두 번째 플레이어 입장 시 대기 메시지 제거
            StartCoroutine(StartCountdown()); // 두 번째 플레이어가 입장하면 카운트다운 시작
        }
        else
        {
            playerRotation = Quaternion.Euler(0, 0, 0);
        }

        // 플레이어 인원에 따라 다른 스폰 위치에 플레이어 생성
        PhotonNetwork.Instantiate("Player", playerSpawnPoints[playerCount - 1].position, playerRotation);
    }

    // 다른 플레이어가 방에 들어왔을 때 호출 (마스터 클라이언트에서만 호출됨)
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            statusText.text = "";  // 두 번째 플레이어 입장 시 대기 메시지 제거
            StartCoroutine(StartCountdown()); // 두 플레이어 입장 시 카운트다운 시작
        }
    }

    // 카운트다운 코루틴
    private IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);  // 카운트다운 텍스트 활성화

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);  // 1초 대기
        }

        countdownText.text = "Go!"; // 마지막 "Go!" 텍스트 표시
        yield return new WaitForSeconds(1); // 1초 대기
        countdownText.gameObject.SetActive(false);  // 카운트다운 텍스트 비활성화

    }


    // 엔딩 이후 실행
    public IEnumerator AfterEnding()
    {
        // 방에서 나가기 전까지 반복
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

    // 방 퇴장 성공 시 호출
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("StartScene");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
