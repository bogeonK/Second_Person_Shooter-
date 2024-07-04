using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // StarSceneManeger 클래스 사용하기 위함

public class StartSceneManager : MonoBehaviour
{
    // Start버튼 클릭 시 호출
    public void OnClickStart()
    {
        // 2. PlayScene 씬 불러오기
        SceneManager.LoadScene("PlayScene");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
