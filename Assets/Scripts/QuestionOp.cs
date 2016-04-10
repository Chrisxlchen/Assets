using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class QuestionOp
    {
        private QuestionList questions;
        private float timeElapsed = 0;
        private Questions currentQuestion;

        public QuestionOp()
        {
            questions = new QuestionList();
        }

        public void AskQuestion(AudioSource audioSource)
        {
            currentQuestion = questions.GetNextQuestion();
            audioSource.clip = Resources.Load(currentQuestion.QAudio) as AudioClip;
            audioSource.Play();
        }

        public void ResetTimeElapsed()
        {
            timeElapsed = 0;
        }

        public bool TimeIsUp(float timeDelta)
        {
            timeElapsed += timeDelta;
            if (timeElapsed >= currentQuestion.QDuration)
            {
                ResetTimeElapsed();
                return true;
            }
            return false;
        }
    }
}
