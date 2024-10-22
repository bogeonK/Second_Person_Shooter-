using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletLife : MonoBehaviour
{
    // �Ѿ� ����
    public float lifeTime = 1f;
    private PhotonView pv;
    private bool isDestroyed = false;

    // ����Ʈ ����Ʈ ������
    public GameObject impactEffectPrefab;
    public float damageAmount = 10f;  // ������ �� ����

    public int shooterViewID; // �߻����� ViewID�� ������ ����

    void Start()
    {
        pv = GetComponent<PhotonView>();

        // Instantiation Data���� shooterViewID ��������
        if (pv.InstantiationData != null && pv.InstantiationData.Length > 0)
        {
            shooterViewID = (int)pv.InstantiationData[0];
        }
        else
        {
            Debug.LogError("Instantiation Data�� �����ϴ�. shooterViewID�� ������ �� �����ϴ�.");
        }

        // ��ü�� �����ڸ� �ڷ�ƾ�� �����ϵ��� ���� �߰�
        if (pv.IsMine)
        {
            StartCoroutine(DestroyAfterTime());
        }
    }

    IEnumerator DestroyAfterTime()
    {
        // lifeTime ��ŭ ���
        yield return new WaitForSeconds(lifeTime);

        if (isDestroyed) yield break; // �̹� �ı� ó���� ��� ����
        isDestroyed = true;

        // ��Ʈ��ũ �󿡼� �Ѿ� �ı� (�����ڸ� ȣ��)
        if (pv != null && pv.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed) return; // �̹� �ı� ó�� ���̸� �������� ����
        isDestroyed = true;

        if (collision.contacts.Length > 0)
        {
            // �浹 ������ ����Ʈ ����Ʈ ����
            ContactPoint contact = collision.contacts[0];

            if (impactEffectPrefab != null)
            {
                GameObject impactEffect = Instantiate(impactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
                // ����Ʈ ���� �ð� �� ����
                Destroy(impactEffect, 1f); // �ʿ信 ���� �ð� ����
            }
            else
            {
                Debug.LogError("impactEffectPrefab�� �����Ǿ� ���� �ʽ��ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("collision.contacts �迭�� ��� �ֽ��ϴ�.");
        }

        // �� �Ѿ��� ��쿡�� ������ ����
        if (pv.IsMine)
        {
            // �÷��̾�Ը� ������ ����
            if (collision.transform.CompareTag("Player"))
            {
                // �浹�� �÷��̾��� PlayerHp ������Ʈ ��������
                PlayerHp playerHp = collision.transform.GetComponent<PlayerHp>();
                if (playerHp != null)
                {
                    // ���� �÷��̾�� �������� ����, shooterViewID ����
                    playerHp.Damaged(damageAmount, shooterViewID);
                }
                else
                {
                    Debug.LogError("PlayerHp ������Ʈ�� ã�� �� �����ϴ�.");
                }
            }

            // ��Ʈ��ũ �󿡼� �Ѿ� �ı�
            if (pv != null && pv.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        // ������ڴ� �Ѿ��� ���ÿ��� �ı����� �ʽ��ϴ�.
    }
}