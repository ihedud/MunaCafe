using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadNextScene(string m_NextScene)
    {
        SceneManager.LoadScene(m_NextScene);
    }
}
