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
        else if (/*(transform.position - player.transform.position).sqrMagnitude <= 15 && */curState == states.Greeting)
        {
            curQuestion = questionOperation.AskQuestion(audioS);
            QuestionsAndAnswers.Add(Application.dataPath + "/Resources/Audio/" + curQuestion.QAudio + ".mp3");
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
                    microPhoneOp.StartRecording(ref recClip, curQuestion.QDuration);
                }
                else
                {
                    if (questionOperation.TimeIsUp(Time.deltaTime, curQuestion.QDuration))// || NoSoundForThreeSec())
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
                            QuestionsAndAnswers.Add(Application.dataPath + "/Resources/Audio/" + curQuestion.QAudio + ".mp3");
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

        foreach (string song in QuestionsAndAnswers)
        {
            WWW audioLoader = new WWW("file://" + song);

            while (!audioLoader.isDone)
                yield return null;

            audioClips.Add(audioLoader.GetAudioClip(false));
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
    /*
    /// Starts the Mic, and plays the audio back in (near) real-time.
    private void StartMicListener()
    {
        audioS.clip = Microphone.Start("Built-in Microphone", true, 999, maxFreq);
        // HACK - Forces the function to wait until the microphone has started, before moving onto the play function.
        while (!(Microphone.GetPosition("Built-in Microphone") > 0)) { }
        audioS.Play();
    }*/
}
