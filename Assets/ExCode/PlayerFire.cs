using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerFire : MonoBehaviour
{
    // 총 효과 프리팹 담아둘 변수
    public GameObject shootEffectPref;


    PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();


        // 마우스 커서 숨기기
        Cursor.visible = false;

        // 마우스 커서가 게임화면을 벗어나지 못하게 잠금
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스 좌클릭누르는 순간, 내 캐릭터 일 때만
        if (Input.GetMouseButtonDown(0)&& pv.IsMine)
        {


            // 화면 가운데에서 시작하는 Ray 생성
            Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));


            // Ray 맞은 물체를 담아둘 변수
            RaycastHit hit;

            // Ray를 발사하고, Ray에 맞은 물체는 hit에 저장, 맞은 물체가 있을때만
            if(Physics.Raycast(ray, out hit))
            {
                // 맞은 위치에, 맞은 표면의 수직이 되는 각도로 총 효과 프리팹의 본사본 생성
                GameObject shootEffect = Instantiate(shootEffectPref, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));

                // 총알 자국을 맞은 오브젝트의 자식으로 설정
                shootEffect.transform.SetParent(hit.transform);
                
                // Ray에 맞은 물체가 적이라면
                if (hit.transform.tag == "Enemy")
                {
                    // 적에게 10만큼 데미지
                    hit.transform.SendMessage("Damaged", 10);
                }

                // Ray에 맞은 물체가 플레이어이라면
                if (hit.transform.tag == "Player")
                {
                    // 플레이어에게 10만큼 데미지, 나의 ViewID 전달
                    hit.transform.GetComponent<PlayerHp>().Damaged(10, pv.ViewID);
                }
            }
        }
    }
}
