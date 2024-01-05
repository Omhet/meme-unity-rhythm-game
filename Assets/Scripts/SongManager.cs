using System.Collections;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.Events;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance;

    public AudioSource danceAudioSource;
    public AudioSource danceCoruptAudioSource;

    public Lane[] lanes;
    public DanceProgressBar danceProgressBar;
    public DanceManager danceManager;

    [SerializeField] private Interval[] intervals;

    public float songDelayInSeconds;
    public double marginOfError; // in seconds

    public int inputDelayInMilliseconds;


    public string fileLocation;
    public float bpm;

    public float noteTime;
    public float noteSpawnY;
    public float noteTapY;
    public float noteDespawnY
    {
        get
        {
            return noteTapY - (noteSpawnY - noteTapY);
        }
    }

    public static MidiFile midiFile;

    public bool isSongFinishedPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://"))
        {
            StartCoroutine(ReadFromWebsite());
        }
        else
        {
            ReadFromFile();
        }

        // 1 Bar
        noteTime = (60f / bpm) * 4;
    }

    private IEnumerator ReadFromWebsite()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileLocation))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                byte[] results = www.downloadHandler.data;
                using (var stream = new MemoryStream(results))
                {
                    midiFile = MidiFile.Read(stream);
                    GetDataFromMidi();
                }
            }
        }
    }

    private void ReadFromFile()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        GetDataFromMidi();
    }

    public void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        midiFile.ReplaceTempoMap(TempoMap.Create(Tempo.FromBeatsPerMinute(bpm)));
        var tempoMap = midiFile.GetTempoMap();

        foreach (var lane in lanes) lane.SetTimeStamps(array, tempoMap);

        danceManager.Setup(array, tempoMap);

        int count = notes.Count(note => note.Velocity > 1);
        danceProgressBar.SetMaximumValue(count * 2);

        Invoke(nameof(StartSong), songDelayInSeconds);
    }

    public void StartSong()
    {
        danceAudioSource.volume = 1;
        danceCoruptAudioSource.volume = 0;

        danceAudioSource.Play();
        danceCoruptAudioSource.Play();
    }

    public void CoruptSong()
    {
        danceAudioSource.volume = 0;
        danceCoruptAudioSource.volume = 1;
    }

    public void RestoreSong()
    {
        danceAudioSource.volume = 1;
        danceCoruptAudioSource.volume = 0;
    }

    public static double GetAudioSourceTime()
    {
        double time = (double)Instance.danceAudioSource.timeSamples / Instance.danceAudioSource.clip.frequency;

        return time;
    }

    void Update()
    {
        foreach (Interval interval in intervals)
        {
            float sampledTime = danceAudioSource.timeSamples / (danceAudioSource.clip.frequency * interval.GetIntervalLength(bpm));
            interval.CheckForNewInterval(sampledTime);
        }

        if (!danceAudioSource.isPlaying && danceAudioSource.time > 0)
        {
            isSongFinishedPlaying = true;
        }

        if (!isSongFinishedPlaying)
        {
            float currentMainVolume = danceAudioSource.volume;
            float newMainVolume = Mathf.Lerp(currentMainVolume, 1f, Time.deltaTime * 2);

            danceAudioSource.volume = newMainVolume;
            danceCoruptAudioSource.volume = 1 - newMainVolume;
        }
    }
}

[System.Serializable]
public class Interval
{
    [SerializeField] private float steps;
    [SerializeField] private UnityEvent trigger;
    private int lastInterval;

    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * steps);
    }

    public void CheckForNewInterval(float interval)
    {
        if (Mathf.FloorToInt(interval) != lastInterval)
        {
            lastInterval = Mathf.FloorToInt(interval);
            trigger.Invoke();
        }
    }
}