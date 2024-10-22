using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // UI 관련 클래스를 사용하기 위함
using Photon.Pun;       // 포톤 관련 클래스를 사용
using Photon.Realtime;  // 방 설정 관련 클래스를 사용하기 위함

public class Lobby : MonoBehaviourPunCallbacks  // 포톤 관련 콜백 함수를 사용하기 위함
{
    public InputField roomNameInput;    // 방 이름 입력란
    public Button createBtn;            // 방 생성 버튼
    public Text connectInfoTxt;         // 연결 현황 텍스트
    public Transform content;           // 방 목록을 출력할 Scroll View의 Content
    public GameObject roomBtnPref;      // 방 참가 버튼

    // 존재하는 모든 방을 관리하는 리스트
    List<RoomInfo> allRoomList = new List<RoomInfo>();

    // Start is called before the first frame update
    void Start()
    {
        // 생성 버튼의 OnClick() 함수에 OnClickCreate() 함수 연결
        createBtn.onClick.AddListener(OnClickCreate);

    }

    // 생성 버튼 누르면 호출
    void OnClickCreate()
    {
        // 방 이름 입력란이 비어있다면 함수 탈출하여 아래 코드 실행 불가
        if (roomNameInput.text == "") return;

        // 중복 클릭을 방지하기 위해 버튼 비활성화
        ButtonOnOff(false);

        // 새로운 방 옵션 생성
        RoomOptions roomOptions = new RoomOptions();

        // 최대 인원 수 2명으로 설정
        roomOptions.MaxPlayers = 2;

        // 사용자가 입력한 방 이름과 방 옵션으로 방 생성 시도
        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
        connectInfoTxt.text = "방 생성 중...";

    }

    // 참가 버튼 누르면 호출
    void OnClickJoin(string roomName)
    {
        // 중복 클릭을 방지하기 위해 버튼 비활성화
        ButtonOnOff(false);

        // 버튼에 적혀있던 이름의 방으로 접속 시도
        PhotonNetwork.JoinRoom(roomName);
    }

    // 버튼 활성화/비활성화 처리
    void ButtonOnOff(bool isOn)
    {
        //방 생성 버튼
        createBtn.interactable = isOn;

        // 방 목록 UI에 있는 모든 버튼
        foreach (var roomBtns in content.GetComponentsInChildren<Button>())
        {
            roomBtns.interactable = isOn;
        }

    }

    // 방 접속에 성공하면 호출
    public override void OnJoinedRoom()
    {
        // PlayScene이라는 이름의 씬 불러오기(씬 전환)
        PhotonNetwork.LoadLevel("PlayScene");
        connectInfoTxt.text = "방 접속 성공!";
    }

    // 방 생성 실패 시, 호출
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // 다시 버튼 클릭할 수 있도록 활성화
        ButtonOnOff(true);
        connectInfoTxt.text = "방 생성 실패";
    }

    // 로비에 접속하거나 방 목록에 변화가 생기면 호출
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 변화가 생긴 방 목록을 토대로 전체 방 리스트 갱신
        foreach (var changedRoom in roomList)
        {
            // 전체 방 리스트에 이미 있던 방이라면 사라진 방이므로 삭제
            if (allRoomList.Contains(changedRoom))
                allRoomList.Remove(changedRoom);

            // 전체 방 리스트에 없던 방이라면 생성된 방이므로 추가
            else
                allRoomList.Add(changedRoom);
            
        }

        // 방 목록 UI 초기화를 위한 반복
        for (int i = 0; i < content.childCount; i++)
        {
            // content의 모든 자식 삭제
            Destroy(content.GetChild(i).gameObject);
        }

        // 존재하는 모든 방의 리스트대로 버튼 생성
        foreach (var room in allRoomList)
        {
            // content의 자식으로 버튼 복제 생성 후, 복사본을 변수로 저장
            GameObject roomBtn = Instantiate(roomBtnPref, content);

            // 버튼의 자식 Text 컴포넌트에 방의 이름 출력
            Text roomNameTxt = roomBtn.GetComponentInChildren<Text>();
            roomNameTxt.text = room.Name;

            // 참가 버튼의 OnClick()함수에 OnClickJoin() 함수 연결 및 방의 이름 전달
            roomBtn.GetComponent<Button>().onClick.AddListener(() => OnClickJoin(room.Name));

        }
    }

}
