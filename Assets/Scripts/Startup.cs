using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    [SerializeField]
    private string InitialScene = "TitleScene";

    void Start()
    {
        SceneManager.LoadScene(InitialScene);
    }
}
