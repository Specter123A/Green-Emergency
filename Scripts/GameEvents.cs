using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameEvents", menuName = "Quiz")]
public class GameEvents : ScriptableObject
{
    public delegate void UpdateQuestionUICallBack (QuizQuestions question);
    public UpdateQuestionUICallBack UpdateQuestionUI;

    public delegate void UpdateAnswerUICallBack (AnswerData answer);
    public UpdateAnswerUICallBack UpdateAnswerUI;

    public delegate void DisplayResolutionScreenCallBack (UIManager.ResolutionScreenType type, int score);
    public DisplayResolutionScreenCallBack DisplayResolutionScreen;

    public delegate void ScoreUpdatedCallBack();
    public ScoreUpdatedCallBack ScoreUpdated;

   [HideInInspector]
    public int CurrentFinalScore;

    [HideInInspector]
    public int StartHighScore;

    //the current final high score and start high score will be compared to update the highscores
}
