using UnityEngine;


public class MicOp
{
    private int minFreq, maxFreq;
    private const int sampleWindow = 1024;
    private const float REFVALUE = 0.1f;    // RMS value for 0 dB.
    private float[] waveData;
    private int clamp = 160;            // Used to clamp dB (I don't really understand this either).
    private float timeElapsed;

    public MicOp()
    {
        Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
        if (minFreq == 0 && maxFreq == 0)
        {
            maxFreq = 44100;
        }
        waveData = new float[sampleWindow];
        timeElapsed = 0f;
    }

    public AudioClip StartRecording(int duration)
    {
        AudioClip rClip = new AudioClip();
        if (!Microphone.IsRecording(null))
        {
            rClip = Microphone.Start(null, true, duration, maxFreq);
        }
        else
        {
            Debug.Log("Failed to start recording, Recording is already in progress!");
        }

        return rClip;
    }

    public void StopRecording(AudioClip rClip, string fileName)
    {
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
            SavWav.Save(fileName, rClip);
        }
    }

    private float[] GetAudioData(AudioClip clip)
    {
        int micPosition = Microphone.GetPosition(null) - (sampleWindow+1);
        if (micPosition < 0)
        {
            Debug.Log("Mic Position is negtive!");
            return null;
        }
        clip.GetData(waveData, micPosition);
        return waveData;
    }

    private float AnalyzeSound(float[] samples)
    {
        // Sums squared samples
        float sum = 0;
        for (int i = 0; i < sampleWindow; i++)
        {
            sum += Mathf.Pow(samples[i], 2);
        }

        // RMS is the square root of the average value of the samples.
        float rmsValue = Mathf.Sqrt(sum / sampleWindow);
        float dbValue = 20 * Mathf.Log10(rmsValue / REFVALUE);

        // Clamp it to {clamp} min
        if (dbValue < -clamp)
        {
            dbValue = -clamp;
        }

        Debug.Log("volume:" + dbValue);
        return dbValue;
    }

    public bool SilenceForNSecs(int nSecs, AudioClip clip)
    {
        float[] samples = GetAudioData(clip);
        if (samples != null)
        {
            if (AnalyzeSound(samples) <= 0)
            {
                timeElapsed += Time.deltaTime;
            }
            else
            {
                timeElapsed = 0;
            }

            Debug.Log(timeElapsed);
            if (timeElapsed >= nSecs)
            {
                return true;
            }
        }
        else
        {
            timeElapsed = 0;
        }

        return false;
    }
}

