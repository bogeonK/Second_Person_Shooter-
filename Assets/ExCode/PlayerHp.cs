using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHp : MonoBehaviour
{
    // 플레이어 체력바 (머리 위)
    public Slider hpBar_World;   
    // 플레이어 체력
    public float hp = 100;

    // 플레이어의 PhotonView 컴포넌트
    PhotonView pv;
    // 플레이어의 Animator 컴포넌트
    Animator anim; 

    // 플레이어의 체력바 (화면)
    Slider hpBar_Screen;
    // 캔버스 오브젝트
    Transform canvas;  

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();

        // 내 캐릭터일 때만 실행
        if (pv.IsMine)
        {
            // Canvas 검색해서 가져오기
            canvas = GameObject.Find("Canvas").transform;

            // Canvas의 부모를 나로 설정
            canvas.SetParent(transform);

            // 화면 왼쪽 상단에 있는 플레이어의 체력바 가져오기
            hpBar_Screen = canvas.GetComponentInChildren<Slider>();
        }
        
    }

    // Update is called once per frame
    public void Damaged(float damage, int hitter)
    {
        // 공격 받았음을 RPC통신으로 모두 전달
        pv.RPC("RPC_Damaged", RpcTarget.All, damage, hitter);

    }

    [PunRPC]
    void RPC_Damaged(float damage, int hitterViewID)
    {
        // 공격 받은 데미지만큼 체력 감소
        hp -= damage;

        // 체력바에 체력 표시
        hpBar_World.value = hp;


        // 화면 체력바에 표시하는 건 내 캐릭터일 때만
        if (pv.IsMine)
        {
            // 체력 표시
            hpBar_Screen.value = hp;

            if (hp > 0) // 체력이 있을 때
            {
                // 피격 애니메이션
            }
            else
            {
                // 죽는 모션
                anim.SetTrigger("dead");
                // 패배 시 엔딩
                StartCoroutine(Ending("패배"));
            }
        }

        // 캐릭터 체력이 없을 때
        if (hp <= 0)
        {
            // 죽으면 총에 안 맞게 태그 변경
            tag = "Untagged";

            // 플레이어 기능 중단
            GetComponent<Player>().enabled = false;
            GetComponent<PlayerShoot>().enabled = false;

            // 승리 플레이어의 PhotonView를 hitterViewID로 찾기
            PhotonView hitterPv = PhotonNetwork.GetPhotonView(hitterViewID);
            if (hitterPv != null)
            {
                PlayerHp winner = hitterPv.GetComponent<PlayerHp>();
                if (winner != null)
                {
                    // 승리 플레이어에게 승리 엔딩
                    StartCoroutine(winner.Ending("승리"));
                }
                else
                {
                    Debug.LogError("승리 플레이어의 PlayerHp 컴포넌트를 찾을 수 없습니다.");
                }
            }
            else
            {
                Debug.LogError("hitterViewID에 해당하는 PhotonView를 찾을 수 없습니다. ViewID: " + hitterViewID);
            }
        }
    }

    // DeadZone과의 충돌을 감지하는 함수 수정
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            // DeadZone에 닿은 플레이어의 ViewID
            int deadPlayerViewID = pv.ViewID;

            // 모든 플레이어의 PhotonView를 가져오기
            PhotonView[] allPlayers = FindObjectsOfType<PhotonView>();

            // 닿지 않은 플레이어의 ViewID 찾기
            int otherPlayerViewID = -1; // 기본값 설정

            foreach (PhotonView playerView in allPlayers)
            {
                if (playerView.ViewID != deadPlayerViewID) // DeadZone에 닿지 않은 플레이어 찾기
                {
                    otherPlayerViewID = playerView.ViewID;
                    break;
                }
            }

            // DeadZone에 닿았음을 RPC로 모든 클라이언트에게 DeadPlayer와 OtherPlayer의 ViewID 전달
            pv.RPC("RPC_DeadZoneDeath", RpcTarget.All, deadPlayerViewID, otherPlayerViewID);
        }
    }

    [PunRPC]
    void RPC_DeadZoneDeath(int deadPlayerViewID, int otherPlayerViewID)
    {
        if (pv.IsMine)
        {
            // DeadZone에 닿은 플레이어 처리
            if (pv.ViewID == deadPlayerViewID)
            {
                hp = 0;

                // 체력바 업데이트
                hpBar_World.value = hp;

                // 내 캐릭터일 경우 화면 체력바 업데이트 및 패배 엔딩
                StartCoroutine(Ending("패배"));
            }
        }

        // 모든 플레이어에서 hp <= 0 확인
        if (pv.ViewID == deadPlayerViewID)
        {
            // 죽으면 총에 안 맞게 태그 변경
            tag = "Untagged";

            // 플레이어 기능 중단
            GetComponent<Player>().enabled = false;
            GetComponent<PlayerShoot>().enabled = false;

            // 승리 플레이어 PlayerHp 컴포넌트 가져오기
            PlayerHp winnerPlayer = PhotonNetwork.GetPhotonView(otherPlayerViewID).GetComponent<PlayerHp>();

            // 승리 플레이어에게 승리 엔딩
            StartCoroutine(winnerPlayer.Ending("승리"));
        }
    }

    // 엔딩에서 실행됨
    public IEnumerator Ending(string result)
    {
        // 내 캐릭터 아닐 시, 함수 탈출
        if (!pv.IsMine) yield break;

        // 1초동안대기
        yield return new WaitForSeconds(1f);

        Transform ending = canvas.GetChild(2);

        // 게임결과
        ending.GetChild(0).GetComponent<Text>().text = "- " + result + " -";

        // 결과 활성화
        ending.gameObject.SetActive(true);

        // 플레이어 기능 중단
        GetComponent<Player>().enabled = false;
        GetComponent<PlayerShoot>().enabled = false;
        GetComponentInChildren<Camera>().enabled = false;

        // 엔딩 이후의 기능 실행
        StartCoroutine(FindObjectOfType<PlaySceneManager>().AfterEnding());
    }
}
