using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerShoot : MonoBehaviour
{
    // 총알 프리팹 담아둘 변수
    public GameObject bulletPref;

    // 총알 발사하는 힘
    public float firePower;

    // 총알 발사 위치
    public Transform firePoint;

    // 발사 딜레이 시간 (초 단위)
    public float fireDelay = 0.2f; // 원하는 딜레이 시간 

    // 총구 이펙트 프리팹
    public GameObject muzzleFlashPrefab;

    // 발사 중인지 확인하는 변수
    private bool isFiring = false;

    PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        // 마우스 커서 숨기기
        Cursor.visible = false;

        // 마우스 커서가 게임 화면을 벗어나지 못하게 잠금
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        // 마우스 좌클릭을 누르는 순간, 발사 중이 아니고 내 캐릭터일 때만
        if (Input.GetMouseButtonDown(0) && !isFiring && pv.IsMine)
        {
            // 발사 코루틴 시작
            StartCoroutine(FireBulletWithDelay());
        }
    }

    // 딜레이 후 총알 발사하는 코루틴
    IEnumerator FireBulletWithDelay()
    {
        isFiring = true; // 발사 중 상태로 변경

        // 딜레이 시간만큼 대기
        yield return new WaitForSeconds(fireDelay);

        // 총구 이펙트 생성 (null 체크 추가)
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);

            // 이펙트 지속 시간 후 제거
            Destroy(muzzleFlash, 1f); // 필요에 따라 시간 조절
        }

        // 총알 생성
        object[] instantiationData = new object[] { pv.ViewID };
        GameObject bullet = PhotonNetwork.Instantiate(bulletPref.name, firePoint.position, firePoint.rotation, 0, instantiationData);

        // 총알의 BulletLife 스크립트에 shooterViewID 전달
        BulletLife bulletLife = bullet.GetComponent<BulletLife>();

        if (bulletLife != null)
        {
            bulletLife.shooterViewID = pv.ViewID; // 발사자의 ViewID 전달
        }
        // 총알에 힘 적용
        bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * firePower, ForceMode.Impulse);

        isFiring = false; // 발사 완료 상태로 변경
    }
}
