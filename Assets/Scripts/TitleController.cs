using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Button StartButton = null;

    [SerializeField]
    private UnityEngine.UI.Button PlusButton = null;
    [SerializeField]
    private UnityEngine.UI.Button MinusButton = null;
    [SerializeField]
    private UnityEngine.UI.Button TimesButton = null;
    [SerializeField]
    private UnityEngine.UI.Button DivideButton = null;

    [SerializeField]
    private UnityEngine.UI.Text StartText = null;

    private GameController GameController = null;


    const string StartTextFormat = "Practice {0}... GO!";

    // Start is called before the first frame update
    void Start()
    {
        GameObject controllers = GameObject.Find("Controllers");
        GameController = controllers.GetComponent<GameController>();

        StartButton.onClick.AddListener(StartButtonClicked);
        PlusButton.onClick.AddListener(PlusButtonClicked);
        MinusButton.onClick.AddListener(MinusButtonClicked);
        TimesButton.onClick.AddListener(TimesButtonClicked);
        DivideButton.onClick.AddListener(DivideButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SceneManager.LoadScene("Level-01");
        //}
    }

    void StartButtonClicked()
    {
        GameController.SetLevel(1);
        SceneManager.LoadScene("Level-01");
    }

    void PlusButtonClicked()
    {
        GameController.SetOp("+");
        StartButton.gameObject.SetActive(true);
        StartText.text = string.Format(StartTextFormat, "Addition");
    }

    void MinusButtonClicked()
    {
        GameController.SetOp("-");
        StartButton.gameObject.SetActive(true);
        StartText.text = string.Format(StartTextFormat, "Subtraction");
    }

    void TimesButtonClicked()
    {
        GameController.SetOp("*");
        StartButton.gameObject.SetActive(true);
        StartText.text = string.Format(StartTextFormat, "Multiplication");
    }

    void DivideButtonClicked()
    {
        GameController.SetOp("/");
        StartButton.gameObject.SetActive(true);
        StartText.text = string.Format(StartTextFormat, "Division");
    }
}
