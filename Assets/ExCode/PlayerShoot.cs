using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerShoot : MonoBehaviour
{
    // �Ѿ� ������ ��Ƶ� ����
    public GameObject bulletPref;

    // �Ѿ� �߻��ϴ� ��
    public float firePower;

    // �Ѿ� �߻� ��ġ
    public Transform firePoint;

    // �߻� ������ �ð� (�� ����)
    public float fireDelay = 0.2f; // ���ϴ� ������ �ð� 

    // �ѱ� ����Ʈ ������
    public GameObject muzzleFlashPrefab;

    // �߻� ������ Ȯ���ϴ� ����
    private bool isFiring = false;

    PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        // ���콺 Ŀ�� �����
        Cursor.visible = false;

        // ���콺 Ŀ���� ���� ȭ���� ����� ���ϰ� ���
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        // ���콺 ��Ŭ���� ������ ����, �߻� ���� �ƴϰ� �� ĳ������ ����
        if (Input.GetMouseButtonDown(0) && !isFiring && pv.IsMine)
        {
            // �߻� �ڷ�ƾ ����
            StartCoroutine(FireBulletWithDelay());
        }
    }

    // ������ �� �Ѿ� �߻��ϴ� �ڷ�ƾ
    IEnumerator FireBulletWithDelay()
    {
        isFiring = true; // �߻� �� ���·� ����

        // ������ �ð���ŭ ���
        yield return new WaitForSeconds(fireDelay);

        // �ѱ� ����Ʈ ���� (null üũ �߰�)
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);

            // ����Ʈ ���� �ð� �� ����
            Destroy(muzzleFlash, 1f); // �ʿ信 ���� �ð� ����
        }

        // �Ѿ� ����
        object[] instantiationData = new object[] { pv.ViewID };
        GameObject bullet = PhotonNetwork.Instantiate(bulletPref.name, firePoint.position, firePoint.rotation, 0, instantiationData);

        // �Ѿ��� BulletLife ��ũ��Ʈ�� shooterViewID ����
        BulletLife bulletLife = bullet.GetComponent<BulletLife>();

        if (bulletLife != null)
        {
            bulletLife.shooterViewID = pv.ViewID; // �߻����� ViewID ����
        }
        // �Ѿ˿� �� ����
        bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * firePower, ForceMode.Impulse);

        isFiring = false; // �߻� �Ϸ� ���·� ����
    }
}
