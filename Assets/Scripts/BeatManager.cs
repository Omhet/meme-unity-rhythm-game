using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    [SerializeField] private float bpm;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Interval[] intervals;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        foreach (Interval interval in intervals)
        {
            float sampledTime = (audioSource.timeSamples / (audioSource.clip.frequency * interval.GetIntervalLength(bpm)));
            interval.CheckForNewInterval(sampledTime);
        }
        
    }
}

//[System.Serializable]
//public class Interval
//{
//    [SerializeField] private float steps;
//    [SerializeField] private UnityEvent trigger;
//    private int lastInterval;

//    public float GetIntervalLength(float bpm)
//    {
//        return 60f / (bpm * steps);
//    }

//    public void CheckForNewInterval(float interval)
//    {
//        if (Mathf.FloorToInt(interval) != lastInterval)
//        {
//            lastInterval = Mathf.FloorToInt(interval);
//            trigger.Invoke();
//        }
//    }
//}