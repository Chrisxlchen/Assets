﻿using System;
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
        // private Questions currentQuestion;

        public QuestionOp()
        {
            questions = new QuestionList();
        }

        public Questions AskQuestion(AudioSource audioSource)
        {
            Questions currentQuestion = questions.GetNextQuestion();
            if (currentQuestion != null)
            {
                Debug.Log("Question: " + currentQuestion.QText);
                GuiTextDebug.debug("Question: " + currentQuestion.QText);
                audioSource.clip = Resources.Load("Audio/" + currentQuestion.QAudio) as AudioClip;
                audioSource.Play();
            } else
            {
                Debug.Log("Failed to get Question.");

            }
            return currentQuestion;
        }

        public void ResetTimeElapsed()
        {
            timeElapsed = 0;
        }

        public bool TimeIsUp(float timeDelta, float duration)
        {
            timeElapsed += timeDelta;
            if (timeElapsed >= duration)
            {
                ResetTimeElapsed();
                return true;
            }
            return false;
        }
    }
}
