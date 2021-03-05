using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Text QuestionText = null;

    [SerializeField]
    private UnityEngine.UI.Text ScoreText = null;

    [SerializeField]
    private UnityEngine.UI.Text WinText = null;

    [SerializeField]
    private UnityEngine.UI.Button NextLevelButton = null;

    [SerializeField]
    private UnityEngine.UI.Button BackButton = null;

    [SerializeField]
    private UnityEngine.UI.Button BackButton2 = null;


    [SerializeField]
    private GameObject AnswerCoinPrefab = null;

    [SerializeField]
    private GameObject CharcterRagdollPrefab = null;

    [SerializeField]
    private AudioClip CoinSound = null;

    [SerializeField]
    private AudioClip DeathSound = null;

    [SerializeField]
    private AudioClip WrongAnswerSound = null;

    [SerializeField]
    private UnityEngine.UI.RawImage HealthRawImage = null;

    [SerializeField]
    private Texture[] HealthImages = null;

    private int Level = 0;

    public void SetLevel(int l)
    {
        Level = l;
    }

    private int Health = 6;

    private string Op = "*";
    public void SetOp(string newOp)
    {
        Op = newOp;
    }





    [SerializeField]
    private Vector3 collectVelocity = new Vector3(0, 3, 0);
    [SerializeField]
    private float collectDestroyTime = 1.5f;

    private int? Argument1;
    private int? Argument2;
    private string Operation;
    private int? Answer;

    private int _Score = 0;

    private bool isQuestionActive = false;
    private GameObject QuestionGameObject = null;
    private GameObject[] Barriers = null;

    private List<GameObject> DisplayedAnswerCoins = new List<GameObject>();

    GameObject hero = null;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        // The hero prefab is placed on each level
        // and doesn't persist across levels
        // On each level load, find the hero
        hero = GameObject.Find("character-03");

        if (BackButton2 != null)
        {
            if (scene.name == "TitleScene")
            {
                BackButton2.gameObject.SetActive(false);
            }
            else
            {
                BackButton2.gameObject.SetActive(true);
            }
        }

    }

    private void Harm(int v)
    {
        Health -= v;
        Health = Mathf.Max(Health, 0);

        UpdateHearts();

        if (Health == 0)
        {
            GameObject heroRagDoll = Instantiate(
                CharcterRagdollPrefab,
                hero.transform.position,
                hero.transform.rotation);
            AudioSource.PlayClipAtPoint(DeathSound, transform.position, 1f);

            hero.SetActive(false);
        }
        else
        {
            StartCoroutine(Stun());
        }
    }

    private IEnumerator Stun()
    {
        GameObject heroRagDoll = Instantiate(
            CharcterRagdollPrefab,
            hero.transform.position,
            hero.transform.rotation);
        hero.SetActive(false);

        yield return new WaitForSeconds(1);

        hero.SetActive(true);
        hero.transform.position = new Vector3(
            heroRagDoll.transform.position.x,
            heroRagDoll.transform.position.y,
            0
            );
        Rigidbody r = hero.GetComponent<Rigidbody>();
        r.velocity = new Vector3(0, 0, 0);
        Destroy(heroRagDoll);

    }


    private void UpdateHearts()
    {
        Texture t = HealthImages[Health];
        HealthRawImage.texture = t;
    }

    public (int, int, string) RandomQuestion()
    {
        int a;
        int b;
        if (Level == 2)
        {
            a = Random.Range(2, 10);
            b = Random.Range(2, 10);
        }
        else
        {
            a = Random.Range(1, 6);
            b = Random.Range(2, 7);
        }

        string op = Op;

        if (op == "-")
        {
            int c = a + b;
            a = c;
        }

        if (op == "/")
        {
            int c = a * b;
            a = c;
        }

        return (a, b, op);
    }


    public void AddToScore(int delta)
    {
        _Score += delta;
        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        string scoreStr = _Score.ToString("N0");
        ScoreText.text = scoreStr;
    }

    public void ClearScoreText()
    {
        ScoreText.text = "";
    }


    /**
     * Computes n random answers with variation v.
     * Assumes that the correct answer is already stored in
     * member variable Answer.
     * 
     * - One of the slots 0 - n is chosen to house the
     *   correct answer.  This correct answer won't be duplicated in any
     *   other slot.
     * - Each answer is unique and won't duplicate any other of the wrong
     *   answers.
     * - The wrong answers will be between [Answer - v, Answer + v]
     * 
     */
    public List<int> RandomAnswers(int n, int v)
    {
        // choose the lucky slot with the correct answer
        int correctAnswerPosition = Random.Range(0, n);

        List<int> answers = new List<int>();

        // Should never call this method if an current
        // question hasn't been set.  Therefore Answer should
        // always have a value.  But, just in case...
        if (Answer == null) return answers;

        for (int i = 0; i <n; i++)
        {
            if (i == correctAnswerPosition)
            {
                answers.Add(Answer.Value);
            }
            else
            {
                bool rulesOK = false;

                while (!rulesOK)
                {
                    int wrongDelta = Random.Range(-v, v);
                    int answer = Answer.Value + wrongDelta;

                    // if this duplicates the right answer try again
                    if (answer == Answer) continue;

                    // consider all the previously selected answers
                    if (answers.Contains(answer)) continue;

                    rulesOK = true;
                    answers.Add(answer);
                }

            }
        }

        return answers;
    }


    public void StartQuestionSequence(
        //int Arg1, int Arg2, string Op,
        Vector3 center,
        GameObject questionGameObject,
        GameObject[] barriers)
    {
        var q = RandomQuestion();
        int Arg1 = q.Item1;
        int Arg2 = q.Item2;
        string Op = q.Item3;

        // Just triggered the same question again
        // do nothing.
        if (questionGameObject == QuestionGameObject)
        {
            return;
        }

        // If another question is in progress clear it before creating new one
        if (isQuestionActive) {
            ClearDisplayedCoins();
        }

        isQuestionActive = true;

        QuestionGameObject = questionGameObject;
        Barriers = barriers;

        Argument1 = Arg1;
        Argument2 = Arg2;
        Operation = Op;
        ComputeAnswerFromCurrentArgs();


        UpdateQuizText();
        DisplayAnswerCoins(center);
    }

    public void ComputeAnswerFromCurrentArgs()
    {
        if (Argument1.HasValue && Argument2.HasValue)
        {
            switch (Operation)
            {
                case "+":
                    Answer = Argument1 + Argument2;
                    break;
                case "-":
                    Answer = Argument1 - Argument2;
                    break;
                case "*":
                    Answer = Argument1 * Argument2;
                    break;
                case "/":
                    Answer = Argument1 / Argument2;
                    break;
            }
        }
    }

    public void DisplayAnswerCoin(Vector3 location, int answer)
    {
        GameObject coin = Instantiate(
            AnswerCoinPrefab,
            location,
            AnswerCoinPrefab.transform.rotation);

        AnswerCoin script = coin.GetComponent<AnswerCoin>();
        script.SetAnswer(answer);
        DisplayedAnswerCoins.Add(coin);
    }

    public void DisplayAnswerCoins(Vector3 center)
    {
        List<int> randomAnswers = RandomAnswers(n:3, v:5);
        DisplayAnswerCoin(center + new Vector3(-2, 3, 0), randomAnswers[0]);
        DisplayAnswerCoin(center + new Vector3(0, 3.5f, 0), randomAnswers[1]);
        DisplayAnswerCoin(center + new Vector3(2, 3, 0), randomAnswers[2]);
    }

    public void UpdateQuizText()
    {
        StringBuilder sb = new StringBuilder();

        if (Argument1.HasValue)
        {
            sb.Append(Argument1);
            sb.Append(" ");
        }

        if (Operation != null)
        {
            sb.Append(Operation);
            sb.Append(" ");
        }

        if (Argument2.HasValue)
        {
            sb.Append(Argument2);
            sb.Append(" = ?");
        }

        QuestionText.text = sb.ToString();
    }

    public void ClearQuiz()
    {
        Argument1 = null;
        Argument2 = null;
        Operation = null;
        isQuestionActive = false;
        QuestionGameObject = null;
        UpdateQuizText();
        //ClearScoreText();
    }

    public bool CheckAnswer(int ans, GameObject answerCoin)
    {
        bool isCorrect = ans == Answer;

        if (isCorrect)
        {
            AudioSource.PlayClipAtPoint(CoinSound, transform.position, 0.75f);

            var _rigidbody = answerCoin.GetComponent<Rigidbody>();
            //_rigidbody.useGravity = true;
            _rigidbody.velocity = collectVelocity;

            DisplayedAnswerCoins.Remove(answerCoin);
            Destroy(answerCoin, collectDestroyTime);

            foreach (var coin in DisplayedAnswerCoins)
            {
                Destroy(coin);
                AddToScore(1);
            }
            Destroy(QuestionGameObject);
            DisplayedAnswerCoins = new List<GameObject>();
            ClearQuiz();

            if (Barriers != null)
            {
                foreach (var barrier in Barriers)
                {
                    Destroy(barrier);
                }
            }
            Barriers = null;
        }
        else
        {
            AudioSource.PlayClipAtPoint(WrongAnswerSound, transform.position, 1.0f);
            Harm(1);
            DisplayedAnswerCoins.Remove(answerCoin);
            Destroy(answerCoin);
        }

        return isCorrect;
    }

    private void ClearDisplayedCoins()
    {
        foreach (var coin in DisplayedAnswerCoins)
        {
            Destroy(coin);
        }
        Destroy(QuestionGameObject);
        DisplayedAnswerCoins = new List<GameObject>();
        ClearQuiz();
    }

    public void WinLevel()
    {
        WinText.gameObject.SetActive(true);

        if (Level == 1)
        {
            NextLevelButton.gameObject.SetActive(true);
        }
        else
        {
            NextLevelButton.gameObject.SetActive(false);
        }
        BackButton.gameObject.SetActive(true);
    }

    public void HideWinControls()
    {
        WinText.gameObject.SetActive(false);
        NextLevelButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);
    }

    public void BackButtonClicked()
    {
        Debug.Log("Back button clicked");
        ClearQuiz();
        HideWinControls();
        GoToTitle();
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void GoToNextLevel()
    {
        WinText.gameObject.SetActive(false);
        NextLevelButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);
        BackButton2.gameObject.SetActive(true);
        SetLevel(2);
        SceneManager.LoadScene("Level-02");
    }
}
