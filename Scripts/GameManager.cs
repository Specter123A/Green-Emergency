using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    QuizQuestions[] questions = null;
    public QuizQuestions[] Questions {get { return questions; }}

    private List<int> FinishedQuestions = new List<int>();
    private int currentQuestion = 0;
    private int score;

    [SerializeField]
    GameEvents events = null;

    private List<AnswerData> pickedAnswers = new List<AnswerData>();
    private IEnumerator  IEWaitTillNextRound = null;
    private bool isFinished  { get { return (FinishedQuestions.Count < Questions.Length) ? false : true; }}  //this getter will check and return if the game is finished
    

    void OnEnable()
    {
       events.UpdateAnswerUI +=UpdateAnswers;
    }

    void OnDisable()
    {
         events.UpdateAnswerUI -=UpdateAnswers;
    }

    void Start()
    {
        
    LoadQuestions();  
    events.CurrentFinalScore =0;

    var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    UnityEngine.Random.InitState(seed);
      
    Display();
    }

    public void UpdateAnswers(AnswerData newAnswer)
    {
        if (Questions[currentQuestion].GetAnswerType == QuizQuestions.AnswerType.Single)
        {
        foreach( var answer in pickedAnswers )
        {
           if (answer != newAnswer)
           {
                  answer.Reset();
           }
            pickedAnswers.Clear();
            pickedAnswers.Add(newAnswer);
        }
    }

         else 
         { 
              bool alreadyPicked = pickedAnswers.Exists(x => x== newAnswer);
              if(alreadyPicked)
              {
                    pickedAnswers.Remove(newAnswer);
              }

              else 
              {
                    pickedAnswers.Add(newAnswer);
              }
         }
    }


    private void EraseAnswers()
    {
      pickedAnswers = new List<AnswerData>();
    }


    void Display()
    {
      EraseAnswers();
      var question = GetRandomQuestion();

      if(events.UpdateQuestionUI != null)
      {
            events.UpdateQuestionUI(question);
      }

      else 
      {
            Debug.Log(" Delegate is null. Check GameManager.Display() method");
      }

    }

    public void Accept()
    {
        bool isCorrect = CheckAnswers();
        FinishedQuestions.Add(currentQuestion);

        UpdateScore((isCorrect)  ? Questions[currentQuestion].AddScore  : -Questions[currentQuestion].AddScore);
        //Here, I am checking if the answer is correct. 
        //If the answer is correct, the score will be added by +10
        //But if the answer is not correct, the score will be -10.  //this is done by sending a negative value of AddScore  
        
        var type = (isFinished) ? UIManager.ResolutionScreenType .Finish : (isCorrect)  ? UIManager.ResolutionScreenType.Correct : UIManager.ResolutionScreenType.Incorrect;
        //Here it will check the the answer types and based on that the resolution screen will be displayed. 
        //if the answers are correct, ResolutionScreenType.Correct, and if false, ResolutionScreenType.Incorrect 

        //Check if the resolution screen is !null
        if(events.DisplayResolutionScreen != null)
        {
            events.DisplayResolutionScreen(type, Questions[currentQuestion].AddScore);
        }
        
        if(IEWaitTillNextRound !=null)
        {
            StopCoroutine(IEWaitTillNextRound); //If it is not not null, then the coroutine should stop
        }

        IEWaitTillNextRound = WaitTillNextRound();  //else we will start it
        StartCoroutine(IEWaitTillNextRound);
    }

    IEnumerator WaitTillNextRound()
    {
        // here we need to determine how much wait time is needed before next question
        //Related to the Script Game Utilities 
        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        Display();
    }

    bool CheckAnswers()
    {
       if(!CompareAnswers())
       {
              return false;
       }

       return true;

       //update score
    }


    bool CompareAnswers()
    {
      if(pickedAnswers.Count >0)
      {
         List<int> c= Questions[currentQuestion].GetCorrectAnswers();  //c is for correct answers
         List<int> p = pickedAnswers.Select(x => x.AnswerIndex).ToList();   //p is for picked answers
         //the two lists will be compared to see how many of the picked amswers were correct

         var f=c.Except(p).ToList();
         //will remove all elements from the list except picked answers

         var s =p.Except(c).ToList();
         //will remove all elements from the list except correct answers

         return !f.Any() && !s.Any();   //will return if there are any items in the list

      }

      return false;
    }



    QuizQuestions GetRandomQuestion()
    {
       var randomIndex = GetRandomQuestionIndex();
       currentQuestion = randomIndex;

       return Questions[currentQuestion];
    }

    int GetRandomQuestionIndex()
    {
        var random = 0;
        if( FinishedQuestions.Count < Questions.Length)
        {
            do 
            {
                random = UnityEngine.Random.Range(0, Questions.Length);
            }

            while (FinishedQuestions.Contains(random) || random == currentQuestion);

        }

        return random;
    }


    void LoadQuestions()
    {
        Object[] obj = Resources.LoadAll("Questions", typeof(QuizQuestions));
        questions = new QuizQuestions[obj.Length];

        for (int i =0; i< obj.Length; i++)
        {
            questions[i] = (QuizQuestions)obj[i];
        }
    }


    private void UpdateScore(int add)
    {
       events.CurrentFinalScore += add;
       if(events.ScoreUpdated  != null)    //check if the delegate is not equal to null 
       {
              events.ScoreUpdated();  //call the delegate from events class if not null
       }
    }
}
