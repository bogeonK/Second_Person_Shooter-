using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlaySceneManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPoints;   // 플레이어 스폰 위치

    // Start is called before the first frame update
    void Start()
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
        }
        else if (playerCount == 2)
        {
            // 두 번째 플레이어는 뒤쪽(180도)을 바라봄
            playerRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            // 기본값 설정 (다른 인원이 추가될 경우)
            playerRotation = Quaternion.Euler(0, 0, 0);
        }

        // 플레이어 인원에 따라 다른 스폰 위치에 플레이어 생성 (1명이면 0번, 2명이면 1번)
        PhotonNetwork.Instantiate("Player", playerSpawnPoints[playerCount - 1].position, playerRotation);
    }

    // 엔딩 이후 실행
    public IEnumerator AfterEnding()
    {
        // 방에서 나가기 전까지 반속
        while (PhotonNetwork.InRoom)
        {
            // 아무키 입력됐다면
            if (Input.anyKeyDown)
            {
                // 방에서 퇴장 시도
                PhotonNetwork.LeaveRoom();

                // 반복문 끝낸 후 함수 탈출
                break;
            }
            // 다음 프레임까지 쉬기
            yield return null;
        }

    }

    // 방 퇴장 성공 시 호출
    public override void OnLeftRoom()
    {
        // StartScene 씬 불러오기
        PhotonNetwork.LoadLevel("StartScene");

        // 마우스 커서 보이게
        Cursor.visible = true;

        // 마우스 커서가 움직일 수 있도록 잠금 해제
        Cursor.lockState = CursorLockMode.None;
    }
}
