using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

[Serializable()]
public struct UIManagerParameters
{
    [Header("Answer Options")]
    [SerializeField]
    float margins;
    public float Margins {get { return margins; }}

    [Header("Resolution Screens")]
    [SerializeField]
    Color correct;
    public Color Correct {get { return correct; }}

    [SerializeField]
    Color incorrect;
    public Color Incorrect {get { return incorrect; }}

    [SerializeField]
    Color final;
    public Color Final {get { return final; }}
}


[Serializable()]
public struct UIElements
{
    [SerializeField]
    RectTransform answerContentArea;
    public RectTransform AnswerContentArea { get { return answerContentArea; }}

    [SerializeField]
    TextMeshProUGUI questionInfoText;
    public TextMeshProUGUI QuestionInfoText { get { return questionInfoText; }}

    [SerializeField]
    TextMeshProUGUI scoreText;
    public TextMeshProUGUI ScoreText { get { return scoreText ; }}

    [Space]

    [SerializeField]
    Animator resolutionScreenAnimator;
    public Animator ResolutionScreenAnimator { get { return resolutionScreenAnimator; }}


    [SerializeField]
    Image resolutionScreenBack;
    public Image ResolutionScreenBack { get { return resolutionScreenBack; }}

    [SerializeField]
    TextMeshProUGUI resolutionScreenInfoText;
    public TextMeshProUGUI ResolutionScreenInfoText { get { return resolutionScreenInfoText ; }}

    [SerializeField]
    TextMeshProUGUI resolutionScoreText;
    public TextMeshProUGUI ResolutionScoreText { get { return resolutionScoreText ; }}


    [Space]

     [SerializeField]
    TextMeshProUGUI highScoreText;
    public TextMeshProUGUI HighScoreText { get { return highScoreText ; }}

    [SerializeField]
    CanvasGroup mainCanvas;
    public CanvasGroup MainCanvas { get { return mainCanvas; }}

    [SerializeField]
    RectTransform finishUIElements;
    public RectTransform FinishUIElements { get { return finishUIElements ; }}


}


public class UIManager : MonoBehaviour
{
    public enum ResolutionScreenType { Correct, Incorrect, Finish}   //Resolution Screen Types 

    [Header("References")]
    [SerializeField]
    GameEvents events;

    [Header("UI Elements/Prefabs")]
    [SerializeField]
    AnswerData answerPrefab;

    [SerializeField]
    UIElements uIElements;

    [Space]

     [SerializeField]
    UIManagerParameters parameters;

    List<AnswerData> currentAnswer = new List<AnswerData>();
    private int resolutionStateParamHash =0;    //use this to store the animator paramter hash
    //more effective than string

    private IEnumerator IEDisplayTimedRes;

    void OnEnable()
    {
      events.UpdateQuestionUI += UpdateQuestionUI;
      events.DisplayResolutionScreen += DisplayRes;
    }

    void OnDisable()
    {
      events.UpdateQuestionUI -= UpdateQuestionUI;
      events.DisplayResolutionScreen -= DisplayRes;
    }

    void Start()
    {
        resolutionStateParamHash = Animator.StringToHash("ScreenState");
    }

    void UpdateQuestionUI(QuizQuestions questions)
    {
      uIElements.QuestionInfoText.text = questions.Info;
      CreateAnswers(questions);
    }
    
    void DisplayRes(ResolutionScreenType type, int score)  //to DisplayResolutionscreen
    {
       UpdateResUI(type, score);
       uIElements.ResolutionScreenAnimator.SetInteger(resolutionStateParamHash, 2);   //as defined earlier for the correct incorrect and finish screens 
       uIElements.MainCanvas.blocksRaycasts = false;   //the screen will not take any input here

       if(type != ResolutionScreenType.Finish)
       {
          if(IEDisplayTimedRes != null)
          {
                StopCoroutine(IEDisplayTimedRes);
          }

          IEDisplayTimedRes = DisplayTimedResolution();
          StartCoroutine(IEDisplayTimedRes);

       }

    }

    IEnumerator DisplayTimedResolution()
    {
       yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
       uIElements.ResolutionScreenAnimator.SetInteger(resolutionStateParamHash, 2);
       uIElements.MainCanvas.blocksRaycasts = true;
    }


    void UpdateResUI(ResolutionScreenType type, int score)  //Updating the screens for every correct/incorrect or final screen
    {
      //Getting the highscore from other game utility using PlayerPrefs
      var highscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);  //the score will be stored here
    
      switch (type)
       {
              case ResolutionScreenType.Correct:
              uIElements.ResolutionScreenBack.color = parameters.Correct;
              uIElements.ResolutionScreenInfoText.text = "Correct!";
              uIElements.ResolutionScoreText.text = "+" + score;
              break;

              case ResolutionScreenType.Incorrect:   // /////////////////////////////////////////////
              uIElements.ResolutionScreenBack.color = parameters.Incorrect;
              uIElements.ResolutionScreenInfoText.text = "Incorrect!";
              uIElements.ResolutionScoreText.text = "-" + score;
              break;

              case ResolutionScreenType.Finish:// /////////////////////////////////
               uIElements.ResolutionScreenBack.color = parameters.Final;
              uIElements.ResolutionScreenInfoText.text = "Final Score";
              ///uIElements.ResolutionScoreText.text = "-" + score;

              StartCoroutine(CalculateScore());
              uIElements.FinishUIElements.gameObject.SetActive(true);   //All game finish elements will be displayed like total score
              uIElements.HighScoreText.gameObject.SetActive(true); //the HighScoreText will indicate the final score obtained. 
              uIElements.HighScoreText.text = ((highscore > events.StartHighScore) ? "<color=yellow>new</color>" : string.Empty) + "High SCore: " + highscore;
              // rich text tags 
              //You can use rich text tags to change the appearance and layout of teh text. 
              //These tags work like HTML or XML tags, but have easier syntax
              break;
       }
    }

    //To Store the score
    IEnumerator CalculateScore()
    {
        int scoreValue =0;
        while(scoreValue < events.CurrentFinalScore)
        {
          scoreValue++;
          uIElements.ResolutionScoreText.text = scoreValue.ToString();
          yield return null;
        }
    }




    void CreateAnswers(QuizQuestions questions)
    {
      EraseAnswers();  //Erase all previous answers

      //Create new answers
      float offset =0 - parameters.Margins;

      for (int i=0; i< questions.Answers.Length; i++)
      {
            AnswerData newAnswer = (AnswerData)Instantiate(answerPrefab, uIElements.AnswerContentArea);
            newAnswer.UpdateData(questions.Answers[i].Info, i);

            newAnswer.Rect.anchoredPosition = new Vector2(0, offset);

            offset -= (newAnswer.Rect.sizeDelta.y + parameters.Margins);
            uIElements.AnswerContentArea.sizeDelta = new Vector2(uIElements.AnswerContentArea.sizeDelta.x, offset * -1);

            currentAnswer.Add(newAnswer);

      }

    }

    void EraseAnswers()
    {
        foreach (var answer in currentAnswer)
        { 
            Destroy(answer.gameObject);  //destroy all the previously stores answers 
        }

        currentAnswer.Clear(); // clear the list for next items to be stored 
    }
}
