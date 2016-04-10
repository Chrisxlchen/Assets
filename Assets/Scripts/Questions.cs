using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Questions
{
    private int id;
    public int ID { get { return id; } }
    private string qText;
    public string QText { get { return qText; } }
    private string qAudio;
    public string QAudio { get { return qAudio; } }
    private int qDuration;
    public int QDuration { get { return qDuration; } }
    
    public Questions(int Id, string text, string audio, int duration)
    {
        id = Id;
        qText = text;
        qAudio = audio;
        qDuration = duration;
    }
}

public class QuestionList
{
    private List<Questions> qList = new List<Questions>();
    public List<Questions> QList { get { return qList; } }
    private int index;


    public QuestionList(string fileName)
    {
        LoadQuestions(fileName);
    }

    public QuestionList()
    {
        string curDir = Directory.GetCurrentDirectory();
        Debug.Log(curDir);
        //string path = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName;

        string path = curDir + "/assets/Resources/Questions/QuestionList.txt";
        Debug.Log(path);
        LoadQuestions(path);
    }

    private void LoadQuestions(string fileName)
    {
        string[] words;
        string line;
        char[] delimiterChars = { '\t' };
        StreamReader file = new StreamReader(fileName);
        while ((line = file.ReadLine()) != null)
        {
            words = line.Split(delimiterChars);
            qList.Add(new Questions(int.Parse(words[0]), words[1], words[2], int.Parse(words[3])));
        }
        file.Close();
    }

    public Questions GetNextQuestion()
    {
        if (index < qList.Count)
        {
            Questions q = qList[index];
            index++;
            return q;
        }
        else
        {
            return null;
        }
    }
    public string GetNextQuestionAudioFile()
    {
        Questions q = GetNextQuestion();
        if (q != null)
        {
            string audioFile = q.QAudio;
            return audioFile;
        }
        else
        {
            return null;
        }
    }
}
