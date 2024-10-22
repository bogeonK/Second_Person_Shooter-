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

        // 특정 회전 값을 사용하여 플레이어 생성
        Quaternion playerRotation = Quaternion.Euler(0, 0, 0);

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
