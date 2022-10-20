using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private Button m_Button;
    [SerializeField] private string m_NextScene;
    
    void Start()
    {
        m_Button = gameObject.GetComponent<Button>();
    }

    void Update()
    {
        m_Button.onClick.AddListener(LoadNextScene);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(m_NextScene);
    }
}
