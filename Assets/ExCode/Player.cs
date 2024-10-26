using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    private Player otherPlayer; // 다른 플레이어 저장 변수
    private bool isViewingOther = false; // 다른플레이어 존재 여부
    private bool hasViewSwitched = false; // 시점변혼이 이루어졌는지 여부
    private float viewSwitchDelay = 5f; // 시점변환 시점

    public Transform firePoint; // 레이저가 발사되는 지점

    // 이동 속도
    public float moveSpeed;
    public float jumpPower; // 점프하는 힘

    // 카메라 설정
    public float lookSpeed; // 카메라 회전 속도
    public float lookXLimit; // 상하 회전 각도 제한
    public float cameraXOffset = 1.2f; // 카메라 양옆 제한
    public float cameraYOffset = 1.2f; // 카메라 위치 높이 제한
    public float cameraZOffset = 1.2f; // 카메라 앞뒤 제한
    public float rotationX = 0;

    int jumpCount; // 점프 횟수

    // 레이저 포인트
    public float laserLength = 10f;
    public Color laserColor = Color.red;


    PhotonView pv;  // 플레이어의 PhotonView 컴포넌트
    Rigidbody rb; // 플레이어의 Rigidbody 컴포넌트
    Animator anim; // 플레이어의 Animator 컴포넌트
    Camera playerCamera; // 카메라 컴포넌트
    private LineRenderer laserLine;

    // 캔버스 오브젝트
    Transform canvas;


    // Start is called before the first frame update
    void Awake()
    {
        // 플레이어의 Rigidbody,PhotonView, Animator 컴포넌트를 가져와서 저장
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();

        // 카메라 생성 및 설정
        playerCamera = new GameObject("PlayerCamera").AddComponent<Camera>();
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = new Vector3(cameraXOffset, cameraYOffset, cameraZOffset);

        // 내 캐릭터 일때만 실행
        if (pv.IsMine)
        {
            // Canvas 검색해서 가져오기
            canvas = GameObject.Find("Canvas").transform;

            // Canvas의 부모를 나로 설정
            canvas.SetParent(transform);

            playerCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            StartCoroutine(AutoSwitchViewAfterDelay());

        }
        else
        {
            playerCamera.gameObject.SetActive(false);
        }

        // 레이저 설정
        laserLine = gameObject.AddComponent<LineRenderer>();
        laserLine.startWidth = 0.05f;
        laserLine.endWidth = 0.05f;
        laserLine.material = new Material(Shader.Find("Sprites/Default"));
        laserLine.startColor = laserColor;
        laserLine.endColor = laserColor;
        laserLine.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        // 내 캐릭터가 아니라면 함수를 탈출하여 아래 코드 실행 불가
        if (!pv.IsMine) return;

        HandleMouseInput();
        UpdateLaserLine();

        // 방향키 또는 WASD키 입력을 숫자로 받아서 저장
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 카메라의 방향을 기준으로 이동 방향 설정
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        // 수직 이동과 수평 이동 벡터 계산
        Vector3 dir = (forward * v) + (right * h);
        dir.y = 0; // 수직 방향 제거 (평면 이동)

        // 모든 방향의 속도가 동일하도록 정규화
        dir.Normalize();

        // 이동할 방향에 원하는 속도 곱하기 (모든 기기에서 동일한 속도)
        // transform.position += dir * moveSpeed * Time.deltaTime;

        // 물리 작용을 이용해 적용
        rb.MovePosition(rb.position + (dir * moveSpeed * Time.deltaTime));

        // 이동하는 속도를 velocity 변수에 할당해 애니메이션 전환
        anim.SetFloat("velocity", dir.magnitude);

        // Space 키를 누를 때, 점프한 횟수가 2회 미만
        if (Input.GetKey(KeyCode.Space) && jumpCount < 2)
        {
            // 위로 힘 발생
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

            // 점프 애니메이션 실행
            anim.SetTrigger("jump");
            anim.SetBool("isJump", true);

            // 점프할 때마다 점프 횟수 증가
            jumpCount++;
        }

        // 마우스 좌클릭으로 총 발사 애니메이션
        if (Input.GetMouseButton(0))
        {
            anim.SetTrigger("shoot");
        }
    }

    // 어떤 물체와 충돌을 시작한 순간에 호출
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 물체의 태그가 "Ground"이고, 내 캐릭터일 때만
        if(collision.gameObject.tag == "Ground" && pv.IsMine)
        {
            //  점프 횟수 초기화
            jumpCount = 0;

            // 점프 애니메이션 종료
            anim.SetBool("isJump", false);
           
        }
    }

    private void HandleMouseInput()
    {
        if (pv.IsMine)
        {
            float mouseY = -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX += mouseY;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            transform.rotation *= Quaternion.Euler(0, mouseX, 0);

            // 시점 변경 중에도 자신의 캐릭터 회전을 네트워크로 동기화
            photonView.RPC("SyncRotation", RpcTarget.All, rotationX, transform.rotation);
        }
    }

    [PunRPC]
    private void SyncRotation(float rotX, Quaternion bodyRotation)
    {
        if (!pv.IsMine)
        {
            rotationX = rotX;
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation = bodyRotation;
        }
    }

    private void UpdateLaserLine()
    {
        if (pv.IsMine)
        {
            Vector3 start = firePoint.position;
            Vector3 end = start + firePoint.forward * laserLength;

            laserLine.SetPosition(0, start);
            laserLine.SetPosition(1, end);

            // 레이저 라인 정보를 네트워크로 동기화
            photonView.RPC("SyncLaserLine", RpcTarget.All, start, end);
        }
    }

    [PunRPC]
    private void SyncLaserLine(Vector3 start, Vector3 end)
    {
        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);
    }

    private IEnumerator AutoSwitchViewAfterDelay()
    {
        yield return new WaitForSeconds(viewSwitchDelay);
        if (!hasViewSwitched)
        {
            ToggleView();
            hasViewSwitched = true;
        }
    }

    private void ToggleView()
    {
        isViewingOther = !isViewingOther;

        if (isViewingOther)
        {
            // 다른 플레이어 찾기
            Player[] players = FindObjectsOfType<Player>();
            foreach (Player player in players)
            {
                if (player != this)
                {
                    otherPlayer = player;
                    break;
                }
            }

            if (otherPlayer != null)
            {
                playerCamera.gameObject.SetActive(false);
                otherPlayer.playerCamera.gameObject.SetActive(true);
                // 레이저 라인은 계속 활성화 상태를 유지
            }
        }
        else
        {
            playerCamera.gameObject.SetActive(true);
            if (otherPlayer != null)
            {
                otherPlayer.playerCamera.gameObject.SetActive(false);
            }
        }

        // 시점 전환 시 네트워크로 동기화
        photonView.RPC("SyncViewToggle", RpcTarget.All, isViewingOther);
    }

    [PunRPC]
    private void SyncViewToggle(bool isViewing)
    {
        isViewingOther = isViewing;
    }

}
