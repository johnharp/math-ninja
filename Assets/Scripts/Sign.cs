using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Sign : MonoBehaviour
{

    //private QuizTrigger _quizTrigger = null;

    [SerializeField]
    private UnityEngine.UI.Text Text = null;

    [SerializeField]
    private GameObject[] Barriers = null;

    private GameController GameController = null;

    private int Argument1;
    private int Argument2;
    private string Operation;

    // Start is called before the first frame update
    void Start()
    {
        GameObject controllers = GameObject.Find("Controllers");
        GameController = controllers.GetComponent<GameController>();

        //var q = GameController.RandomQuestion();
        //Argument1 = q.Item1;
        //Argument2 = q.Item2;
        //Operation = q.Item3;

        //StringBuilder sb = new StringBuilder();

        //sb.Append(Argument1);
        //sb.Append(" ");
        //sb.Append(Operation);
        //sb.Append(" ");
        //sb.Append(Argument2);

        //Text.text = sb.ToString();
        Text.text = "?";
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 displayAt = new Vector3(transform.position.x, transform.position.y, 0);

            GameController.StartQuestionSequence(
                displayAt,
                gameObject,
                Barriers);
        }

        //GameController.StartQuestionSequence(
        //    Argument1, Argument2, Operation,
        //    displayAt,
        //    gameObject,
        //    Barriers);
    }
}
