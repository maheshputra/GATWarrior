﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineActivator : MonoBehaviour
{
    public static TimelineActivator instance;

    [Header("Camera lerp")]
    public RectTransform topBorder;
    public RectTransform bottomBorder;
    public float lerpSpeed;
    float curY;
    float targetY;
    bool lerping;

    public PlayableDirector newGameTimeline;
    public TimelineName timelineName;
    public enum TimelineName {
        newGameTimeline
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    void Start()
    {
        topBorder.localScale = new Vector3(0, 0, 0);
        bottomBorder.localScale = new Vector3(0, 0, 0);
        curY = topBorder.localScale.y;
        lerping = false;
        if (timelineName == TimelineName.newGameTimeline)
        {
            if (GameDataBase.instance.newGame)
            {
                StartTimeline(newGameTimeline);
            }
        }
    }

    void Update()
    {
        if (lerping)
        {
            curY = Mathf.Lerp(curY, targetY, Time.deltaTime * lerpSpeed);
            topBorder.localScale = new Vector3(1, curY, 1);
            bottomBorder.localScale = new Vector3(1, curY, 1);

            if (Mathf.Abs(curY-targetY)<=0.005f)
            {
                lerping = false;
                Debug.Log("stop");
            }
        }
    }

    public void StartTimeline(PlayableDirector playableDirector) {
        UIManager.instance.StartCoroutine(UIManager.instance.ChangeState(UIState.Timeline));
        targetY = 1f;
        lerping = true;
        playableDirector.Play();

        StartCoroutine(WaitTimelineDone(playableDirector, UIState.Gameplay));
    }

    /// <summary>
    /// function ketika timeline selesai
    /// </summary>
    /// <param name="playableDirector"></param>
    /// <param name="targetState"></param>
    /// <returns></returns>
    IEnumerator WaitTimelineDone(PlayableDirector playableDirector, UIState targetState) {
        yield return new WaitUntil(() => playableDirector.state != PlayState.Playing);
        //yield return new WaitForSeconds((float)playableDirector.duration);
        UIManager.instance.StartCoroutine(UIManager.instance.ChangeState(targetState));
        targetY = 0f;
        lerping = true;
    }
}
