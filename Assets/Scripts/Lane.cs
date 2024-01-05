using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;
    public GameObject notePrefab;
    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();
    private Melanchall.DryWetMidi.Interaction.Note[] midiNotes;

    int spawnIndex = 0;
    int inputIndex = 0;

    void Start()
    {

    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array, TempoMap tempoMap)
    {

        midiNotes = new Melanchall.DryWetMidi.Interaction.Note[array.Length];
        array.CopyTo(midiNotes, 0);

        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
                var timeStamp = (double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f;
                timeStamps.Add(timeStamp);
            }
        }
    }

    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
            {
                var note = Instantiate(notePrefab, transform);
                notes.Add(note.GetComponent<Note>());
                note.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];
                note.GetComponent<Note>().isPlayerNote = midiNotes[spawnIndex].Velocity > 1;

                spawnIndex++;
            }
        }

        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double marginOfError = SongManager.Instance.marginOfError;
            double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

            if (DanceManager.Instance.isPlayerTurn)
            {
                if (Input.GetKeyDown(input))
                {
                    if (Math.Abs(audioTime - timeStamp) < marginOfError)
                    {
                        Hit();
                        print($"Hit on {inputIndex} note");
                        notes[inputIndex].Dissolve();
                        inputIndex++;
                    }
                    else
                    {
                        //print($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay");
                        Miss();
                    }
                }
                if (timeStamp + marginOfError <= audioTime)
                {
                    Miss();
                    print($"Missed {inputIndex} note");
                    notes[inputIndex].FadeOut();
                    inputIndex++;
                }
            }
            else
            // Rivals turn
            {
                if (timeStamp <= audioTime)
                {
                    notes[inputIndex].Dissolve();
                    inputIndex++;
                }
            }
        }

    }

    private void Hit()
    {
        ScoreManager.Hit();
    }

    private void Miss()
    {
        ScoreManager.Miss();
    }
}