using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{

    private Transform spineBone; // ô�ߺ� �ҷ�����
    private bool hasCoroutineStarted = false;  // �ڷ�ƾ ������ ���� ����

    private Player otherPlayer; // �ٸ� �÷��̾� ���� ����
    private bool isViewingOther = false; // �ٸ��÷��̾� ���� ����
    private bool hasViewSwitched = false; // ������ȯ�� �̷�������� ����
    private float viewSwitchDelay = 4f; // ������ȯ ����

    public Transform firePoint; // �������� �߻�Ǵ� ����

    // �̵� �ӵ�
    public float moveSpeed;
    public float jumpPower; // �����ϴ� ��

    // ī�޶� ����
    public float lookSpeed; // ī�޶� ȸ�� �ӵ�
    public float lookXLimit; // ���� ȸ�� ���� ����
    public float cameraXOffset = 1.2f; // ī�޶� �翷 ����
    public float cameraYOffset = 1.2f; // ī�޶� ���� ����
    public float cameraZOffset = 1.2f; // ī�޶� �յ� ����
    public float rotationX = 0;

    int jumpCount; // ���� Ƚ��

    // ������ ����Ʈ
    public float laserLength = 10f;
    public Color laserColor = Color.red;


    PhotonView pv;  // �÷��̾��� PhotonView ������Ʈ
    Rigidbody rb; // �÷��̾��� Rigidbody ������Ʈ
    Animator anim; // �÷��̾��� Animator ������Ʈ
    Camera playerCamera; // ī�޶� ������Ʈ
    private LineRenderer laserLine;

    // ĵ���� ������Ʈ
    Transform canvas;


    // Start is called before the first frame update
    void Awake()
    {
        // �÷��̾��� Rigidbody,PhotonView, Animator ������Ʈ�� �����ͼ� ����
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();

        // ô�� �� ��������
        spineBone = anim.GetBoneTransform(HumanBodyBones.Spine);

        // ī�޶� ���� �� ����
        playerCamera = new GameObject("PlayerCamera").AddComponent<Camera>();
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = new Vector3(cameraXOffset, cameraYOffset, cameraZOffset);

        // �� ĳ���� �϶��� ����
        if (pv.IsMine)
        {

            // Canvas �˻��ؼ� ��������
            canvas = GameObject.Find("Canvas").transform;

            // Canvas�� �θ� ���� ����
            canvas.SetParent(transform);

            playerCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // �� �÷��̾ ������ �� �÷��̾� ���� 2���� ��� Ȯ��
            if (PhotonNetwork.PlayerList.Length == 2 && !hasCoroutineStarted)
            {
                StartCoroutine(AutoSwitchViewAfterDelay());
                hasCoroutineStarted = true;
            }

        }
        else
        {
            playerCamera.gameObject.SetActive(false);
        }

        // ������ ����
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
        // �� ĳ���Ͱ� �ƴ϶�� �Լ��� Ż���Ͽ� �Ʒ� �ڵ� ���� �Ұ�
        if (!pv.IsMine) return;

        HandleMouseInput();
        UpdateLaserLine();

        // ����Ű �Ǵ� WASDŰ �Է��� ���ڷ� �޾Ƽ� ����
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // ĳ������ ������ �������� �̵� ���� ����
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // ���� �̵��� ���� �̵� ���� ���
        Vector3 dir = (forward * v) + (right * h);
        dir.y = 0; // ���� ���� ���� (��� �̵�)

        // ��� ������ �ӵ��� �����ϵ��� ����ȭ
        dir.Normalize();

        // ���� �ۿ��� �̿��� ����
        rb.MovePosition(rb.position + (dir * moveSpeed * Time.deltaTime));

        // �̵��ϴ� �ӵ��� velocity ������ �Ҵ��� �ִϸ��̼� ��ȯ
        anim.SetFloat("velocity", dir.magnitude);

        // Space Ű�� ���� ��, ������ Ƚ���� 2ȸ �̸�
        if (Input.GetKey(KeyCode.Space) && jumpCount < 2)
        {
            // ���� �� �߻�
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

            // ���� �ִϸ��̼� ����
            anim.SetTrigger("jump");
            anim.SetBool("isJump", true);

            // ������ ������ ���� Ƚ�� ����
            jumpCount++;
        }

        // ���콺 ��Ŭ������ �� �߻� �ִϸ��̼�
        if (Input.GetMouseButton(0))
        {
            anim.SetTrigger("shoot");
        }
    }

    // � ��ü�� �浹�� ������ ������ ȣ��
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ��ü�� �±װ� "Ground"�̰�, �� ĳ������ ����
        if (collision.gameObject.tag == "Ground" && pv.IsMine)
        {
            //  ���� Ƚ�� �ʱ�ȭ
            jumpCount = 0;

            // ���� �ִϸ��̼� ����
            anim.SetBool("isJump", false);

        }
    }
    void LateUpdate()
    {
        if (spineBone != null)
        {
            // ī�޶� ��ġ�� ������� X�� ������ ȸ�� ����
            Quaternion spineRotation = Quaternion.Euler(-rotationX, 0, 0);

            // �ִϸ��̼��� �����ϱ� ���� ���� ���� ȸ���� ����
            spineBone.localRotation = spineRotation * spineBone.localRotation;
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

            // ���� ���� �߿��� �ڽ��� ĳ���� ȸ���� ��Ʈ��ũ�� ����ȭ
            photonView.RPC("SyncRotation", RpcTarget.All, rotationX, transform.rotation);
        }
    }

    [PunRPC]
    private void SyncRotation(float rotX, Quaternion bodyRotation)
    {
        if (!pv.IsMine)
        {
            rotationX = rotX;
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

            // ������ ���� ������ ��Ʈ��ũ�� ����ȭ
            photonView.RPC("SyncLaserLine", RpcTarget.All, start, end);
        }
    }

    [PunRPC]
    private void SyncLaserLine(Vector3 start, Vector3 end)
    {
        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (pv.IsMine)
        {
            if (PhotonNetwork.PlayerList.Length == 2 && !hasCoroutineStarted)
            {
                StartCoroutine(AutoSwitchViewAfterDelay());
                hasCoroutineStarted = true;
            }
        }
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
            // �ٸ� �÷��̾� ã��
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
                // �ڽ��� ī�޶� �ٸ� �÷��̾��� ��ġ�� �̵�
                playerCamera.transform.SetParent(otherPlayer.transform);
                playerCamera.transform.localPosition = new Vector3(cameraXOffset, cameraYOffset, cameraZOffset);
                playerCamera.transform.localRotation = Quaternion.identity;
            }
        }
        else
        {
            // ī�޶� �ڽ��� ĳ���ͷ� ����
            playerCamera.transform.SetParent(this.transform);
            playerCamera.transform.localPosition = new Vector3(cameraXOffset, cameraYOffset, cameraZOffset);
            playerCamera.transform.localRotation = Quaternion.identity;
        }
    }

    [PunRPC]
    private void SyncViewToggle(bool isViewing)
    {
        isViewingOther = isViewing;
    }


}