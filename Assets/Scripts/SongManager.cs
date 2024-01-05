using System.Collections;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance;

    public AudioSource danceAudioSource;
    public AudioSource danceCoruptAudioSource;

    public Lane[] lanes;
    public DanceProgressBar danceProgressBar;
    public DanceManager danceManager;

    public Image overlay;

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
    public bool isSongStarted = false;
    private float overlayFadeOutTimer = 1f;

    // Start is called before the first frame update
    [System.Obsolete]
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

    [System.Obsolete]
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
        ScoreManager.Instance.maximumLevelScore = count;

        Invoke(nameof(StartSong), songDelayInSeconds);
    }

    public void StartSong()
    {
        danceAudioSource.volume = 1;
        danceCoruptAudioSource.volume = 0;

        danceAudioSource.Play();
        danceCoruptAudioSource.Play();

        isSongStarted = true;
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

    public void Pause()
    {
        danceAudioSource.Pause();
        danceCoruptAudioSource.Pause();
    }

    public void Resume()
    {
        danceAudioSource.UnPause();
        danceCoruptAudioSource.UnPause();
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
            PopupsManager.Instance.OpenLevelEndPopup();
        }

        if (!isSongFinishedPlaying)
        {
            float currentMainVolume = danceAudioSource.volume;
            float newMainVolume = Mathf.Lerp(currentMainVolume, 1f, Time.deltaTime * 2);

            danceAudioSource.volume = newMainVolume;
            danceCoruptAudioSource.volume = 1 - newMainVolume;
        }

        if (overlayFadeOutTimer > 0)
        {
            overlayFadeOutTimer -= Time.deltaTime * 0.5f;
            Color color = overlay.color;
            color.a = overlayFadeOutTimer;
            overlay.color = color;
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