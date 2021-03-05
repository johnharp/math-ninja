using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerCoin : MonoBehaviour
{
    private int Answer;

    private UnityEngine.UI.Text Text = null;
    private GameController GameController = null;
    void Start()
    {
        GameObject controllers = GameObject.Find("Controllers");
        GameController = controllers.GetComponent<GameController>();
    }

    public void SetAnswer(int answer)
    {
        if (Text == null)
        {
            Text = gameObject
                .GetComponentInChildren<UnityEngine.UI.Text>();
        }
        Answer = answer;
        Text.text = answer.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.CheckAnswer(Answer, this.gameObject);
        }
    }
}
