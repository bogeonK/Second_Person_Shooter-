using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // StarSceneManeger Ŭ���� ����ϱ� ����

public class StartSceneManager : MonoBehaviour
{
    // Start��ư Ŭ�� �� ȣ��
    public void OnClickStart()
    {
        // 2. PlayScene �� �ҷ�����
        SceneManager.LoadScene("PlayScene");
    }
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayBGM(0); // StartScene BGM ���
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
