using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

public class DanceManager : MonoBehaviour
{
    public static DanceManager Instance;

    public bool isPlayerTurn = false;

    public Animator playerAnimator;
    public Animator rivalAnimator;

    public List<double> timeStamps = new List<double>();
    private Melanchall.DryWetMidi.Interaction.Note[] midiNotes;
    List<Melanchall.DryWetMidi.Interaction.Note> distinctVelocityNotes;

    public RectTransform playerProfileRect;
    public RectTransform rivalProfileRect;

    int spawnIndex = 0;

    float profileScaleUp = 1.2f;
    float profileScaleDown = 0.8f;

    void Start()
    {
        Instance = this;
    }

    public void Setup(Melanchall.DryWetMidi.Interaction.Note[] notes, TempoMap tempoMap)
    {
        float speed = SongManager.Instance.bpm / 120f;
        playerAnimator.speed = speed;
        rivalAnimator.speed = speed;

        midiNotes = new Melanchall.DryWetMidi.Interaction.Note[notes.Length];
        notes.CopyTo(midiNotes, 0);

        distinctVelocityNotes = new List<Melanchall.DryWetMidi.Interaction.Note>();

        if (midiNotes.Length > 0)
        {
            distinctVelocityNotes.Add(midiNotes[0]);

            for (int i = 1; i < midiNotes.Length; i++)
            {
                if (midiNotes[i].Velocity != midiNotes[i - 1].Velocity)
                {
                    distinctVelocityNotes.Add(midiNotes[i]);
                }
            }
        }

        foreach (var note in distinctVelocityNotes)
        {
            var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
            var timeStamp = (double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f;
            timeStamps.Add(timeStamp);
        }
    }

    public void DisruptPlayerDance()
    {
        float playerError = Mathf.Min(playerAnimator.GetFloat("Error") + 0.3f, 0.8f);
        playerAnimator.SetFloat("Error", playerError);
    }

    public void StabilizePlayerDance()
    {
        playerAnimator.SetFloat("Error", 0);
    }

    void Update()
    {
        if (spawnIndex < timeStamps.Count && SongManager.Instance.isSongStarted)
        {
            // noteTime (4 beats) before dance
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
            {
                isPlayerTurn = distinctVelocityNotes[spawnIndex].Velocity > 1;

                RectTransform rectToScaleUp = isPlayerTurn ? playerProfileRect : rivalProfileRect;
                RectTransform rectToScaleDown = isPlayerTurn ? rivalProfileRect : playerProfileRect;

                rectToScaleUp.localScale = new Vector3(profileScaleUp, profileScaleUp, rectToScaleUp.localScale.z);
                rectToScaleDown.localScale = new Vector3(profileScaleDown, profileScaleDown, rectToScaleDown.localScale.z);

                playerAnimator.SetFloat("Error", 0);

                if (isPlayerTurn)
                {
                    CameraController.Instance.SetActiveCamera(0);

                    playerAnimator.SetBool("isBouncing", true);

                    rivalAnimator.SetBool("isDancing", false);
                    rivalAnimator.SetBool("isBouncing", false);
                }
                else
                {
                    CameraController.Instance.SetActiveCamera(1);

                    rivalAnimator.SetBool("isBouncing", true);

                    playerAnimator.SetBool("isDancing", false);
                    playerAnimator.SetBool("isBouncing", false);
                }
            }

            // Dance starts
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex])
            {
                print("Dance start");
                playerAnimator.SetBool("isDancing", isPlayerTurn);
                rivalAnimator.SetBool("isDancing", !isPlayerTurn);

                spawnIndex++;
            }
        }


        float playerDanceError = playerAnimator.GetFloat("Error");
        float newPlayerDanceError = Mathf.Lerp(playerDanceError, 0f, Time.deltaTime * 0.5f);
        playerAnimator.SetFloat("Error", newPlayerDanceError);

        if (SongManager.Instance.isSongFinishedPlaying)
        {
            playerAnimator.SetBool("isDancing", false);
            playerAnimator.SetBool("isBouncing", false);

            rivalAnimator.SetBool("isDancing", false);
            rivalAnimator.SetBool("isBouncing", false);
        }
    }
}
