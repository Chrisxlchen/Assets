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
        string path = "Questions/QuestionList";
        /*TextAsset txt = Resources.Load("Questions/QuestionList") as TextAsset;
        Debug.Log("Text: " + txt.text);

        string path = Application.dataPath + "/Resources/Questions/QuestionList.txt";
        WWW request = new WWW(path);

        while (!request.isDone)
        {
            Debug.Log("Loading...");
        }

        Debug.Log("Data: " + request.text);
        Debug.Log(path);
        GuiTextDebug.debug(path);
        GuiTextDebug.debug("Data: " + request.text);*/
        LoadQuestions(path);
    }

    private void LoadQuestions(string fileName)
    {
        TextAsset txt = Resources.Load("Questions/QuestionList") as TextAsset;
        string[] words;
        string[] lines;
        char[] delimiterChars1 = { '\n' };
        char[] delimiterChars2 = { '\t' };
        lines = txt.text.Split(delimiterChars1);
        foreach (string line in lines)
        {
            words = line.Split(delimiterChars2);
            qList.Add(new Questions(int.Parse(words[0]), words[1], words[2], int.Parse(words[3])));
            Debug.Log("load " + words[0] + " " + words[1]);
            GuiTextDebug.debug("load " + words[0] + " " + words[1]);
        }
        /*
        string[] words;
        string line;
        char[] delimiterChars = { '\t' };
        StreamReader file = new StreamReader(fileName);
        while ((line = file.ReadLine()) != null)
        {
            words = line.Split(delimiterChars);
            qList.Add(new Questions(int.Parse(words[0]), words[1], words[2], int.Parse(words[3])));
            Debug.Log("load " + words[0] + " " + words[1]);
            GuiTextDebug.debug("load " + words[0] + " " + words[1]);
        }
        file.Close();*/
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
