using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletLife : MonoBehaviour
{
    // 총알 수명
    public float lifeTime = 1f;
    private PhotonView pv;
    private bool isDestroyed = false;

    // 임팩트 이펙트 프리팹
    public GameObject impactEffectPrefab;
    public float damageAmount = 10f;  // 데미지 양 설정

    public int shooterViewID; // 발사자의 ViewID를 저장할 변수

    void Start()
    {
        pv = GetComponent<PhotonView>();

        // Instantiation Data에서 shooterViewID 가져오기
        if (pv.InstantiationData != null && pv.InstantiationData.Length > 0)
        {
            shooterViewID = (int)pv.InstantiationData[0];
        }
        else
        {
            Debug.LogError("Instantiation Data가 없습니다. shooterViewID를 설정할 수 없습니다.");
        }

        // 객체의 소유자만 코루틴을 실행하도록 조건 추가
        if (pv.IsMine)
        {
            StartCoroutine(DestroyAfterTime());
        }
    }

    IEnumerator DestroyAfterTime()
    {
        // lifeTime 만큼 대기
        yield return new WaitForSeconds(lifeTime);

        if (isDestroyed) yield break; // 이미 파괴 처리된 경우 종료
        isDestroyed = true;

        // 네트워크 상에서 총알 파괴 (소유자만 호출)
        if (pv != null && pv.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed) return; // 이미 파괴 처리 중이면 실행하지 않음
        isDestroyed = true;

        if (collision.contacts.Length > 0)
        {
            // 충돌 지점에 임팩트 이펙트 생성
            ContactPoint contact = collision.contacts[0];

            if (impactEffectPrefab != null)
            {
                GameObject impactEffect = Instantiate(impactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
                // 이펙트 지속 시간 후 제거
                Destroy(impactEffect, 1f); // 필요에 따라 시간 조절
            }
            else
            {
                Debug.LogError("impactEffectPrefab이 설정되어 있지 않습니다.");
            }
        }
        else
        {
            Debug.LogWarning("collision.contacts 배열이 비어 있습니다.");
        }

        // 내 총알인 경우에만 데미지 적용
        if (pv.IsMine)
        {
            // 플레이어에게만 데미지 적용
            if (collision.transform.CompareTag("Player"))
            {
                // 충돌한 플레이어의 PlayerHp 컴포넌트 가져오기
                PlayerHp playerHp = collision.transform.GetComponent<PlayerHp>();
                if (playerHp != null)
                {
                    // 맞은 플레이어에게 데미지를 적용, shooterViewID 전달
                    playerHp.Damaged(damageAmount, shooterViewID);
                }
                else
                {
                    Debug.LogError("PlayerHp 컴포넌트를 찾을 수 없습니다.");
                }
            }

            // 네트워크 상에서 총알 파괴
            if (pv != null && pv.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        // 비소유자는 총알을 로컬에서 파괴하지 않습니다.
    }
}