using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System.Collections.Generic;
using System.IO;

public class AskQuestions : MonoBehaviour {
    // Use this for initialization
    private enum states { Greeting = 1, Testing, Replay, TheEnd};
    private GameObject player;
    private AudioSource audioS;
    private MicOp microPhoneOp;
    private AudioClip recClip;
    private float timeElapse;
    private QuestionOp questionOperation;
    private Questions curQuestion;
    private List<string> QuestionsAndAnswers;
    private int index;
    private List<AudioClip> audioClips;
    private states curState;
    private const int MaxSliencePeriod = 3;

    void Start () {
        curState = states.Greeting;
        timeElapse = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        audioS = GetComponent<AudioSource>();
        microPhoneOp = new MicOp();

        questionOperation = new QuestionOp();
        QuestionsAndAnswers = new List<string>();
        audioClips = new List<AudioClip>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        timeElapse += Time.deltaTime;
        //print(Application.dataPath);
        if (curState == states.TheEnd)
        {
            return;
        }
        else if (curState == states.Replay)
        {
            //Replay();
            StartCoroutine(PlayAudioList());
            curState = states.TheEnd;
            return;
        }
        //(transform.position - player.transform.position).sqrMagnitude <= 15 && 
        else if (curState == states.Greeting)
        {
            curQuestion = questionOperation.AskQuestion(audioS);
            QuestionsAndAnswers.Add("Audio/" + curQuestion.QAudio);
            curState = states.Testing;
            return;
        }
        else
        {
            // If the audio has stopped playing 
            if (!audioS.isPlaying)
            {
                // and Mic is not start recording, Then start recording.
                if (!Microphone.IsRecording(null))
                {
                    recClip = microPhoneOp.StartRecording(curQuestion.QDuration);
                }
                else
                {
                    if (questionOperation.TimeIsUp(Time.deltaTime, curQuestion.QDuration) || 
                        microPhoneOp.SilenceForNSecs(MaxSliencePeriod, recClip))
                    {
                        string answerToSave = "savedFileName" + curQuestion.QAudio;
                        microPhoneOp.StopRecording(recClip, answerToSave);
                        QuestionsAndAnswers.Add(Application.persistentDataPath + "/" + answerToSave + ".wav");
                        curQuestion = questionOperation.AskQuestion(audioS);
                        if (curQuestion == null)
                        {
                            // Test is finished. No more questions. what we do now???
                            curState = states.Replay;
                            index = 0;
                            printTheReplay();
                        }
                        else
                        {
                            QuestionsAndAnswers.Add("Audio/" + curQuestion.QAudio);
                        }
                    }
                }
            }
        }
    }

    private void Replay()
    {
        if (!audioS.isPlaying)
        {
            if (index < QuestionsAndAnswers.Count)
            {
                string path = Path.Combine(Application.persistentDataPath, QuestionsAndAnswers[index]);

                audioS.clip = Resources.Load(QuestionsAndAnswers[index]) as AudioClip;
                audioS.Play();
                index++;
            }
        }
    }

    IEnumerator DownloadPlaylist()
    {
        //string[] playlist = Directory.GetFiles("Application.persistentDataPath", "*.wav", SearchOption.TopDirectoryOnly);
        AudioClip clip;
        foreach (string song in QuestionsAndAnswers)
        {
            if (song.StartsWith("Audio"))
            {
                clip = Resources.Load(song) as AudioClip;
            }
            else
            {
                WWW audioLoader = new WWW("file://" + song);

                while (!audioLoader.isDone)
                    yield return null;

                clip = audioLoader.GetAudioClip(false);
            }

            audioClips.Add(clip);
        }
    }

    IEnumerator PlayAudioList()
    {
        yield return StartCoroutine("DownloadPlaylist");

        foreach (AudioClip song in audioClips)
        {
            audioS.clip = song;
            audioS.Play();
            yield return new WaitForSeconds(song.length);
        }
    }

    private void printTheReplay()
    {
        int idx = 0;
        for (idx = 0; idx < QuestionsAndAnswers.Count; idx++)
        {
            print(QuestionsAndAnswers[idx]);
        }
    }
}
