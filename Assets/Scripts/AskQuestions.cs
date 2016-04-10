using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class AskQuestions : MonoBehaviour {
    // Use this for initialization
    private GameObject player;
    private AudioSource audioS;
    private bool initial_greeting;
    private bool endOfTest;
    private MicOp microPhoneOp;
    private AudioClip recClip;
    private float timeElapse = 0;
    private int recTimeLen = 10;
    private QuestionOp questionOperation;
    private Questions curQuestion;

    void Start () {
        timeElapse = 0;
        initial_greeting = false;
        endOfTest = false;
        player = GameObject.FindGameObjectWithTag("Player");
        audioS = GetComponent<AudioSource>();
        microPhoneOp = new MicOp();
        questionOperation = new QuestionOp();
    }
	
	// Update is called once per frame
	void Update ()
    {
        timeElapse += Time.deltaTime;
        //print(timeElapse);
        if (endOfTest)
            return;
        //print((transform.position - player.transform.position).sqrMagnitude);
        // Start conversation here.
        if ((transform.position - player.transform.position).sqrMagnitude <= 15 && initial_greeting == false)
        {
            curQuestion = questionOperation.AskQuestion(audioS);
            //audioS.clip = Resources.Load("Audio/howdoyoudo") as AudioClip;
            //audioS.Play();
            initial_greeting = true;
            return;
        }

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
                    microPhoneOp.StopRecording(recClip, "savedFileName"+curQuestion.ID);
                    curQuestion = questionOperation.AskQuestion(audioS);
                    if (curQuestion == null)
                    {
                        // Test is finished. No more questions. what we do now???
                        endOfTest = true;
                    }
                }
            }
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
