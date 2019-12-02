using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

public class Video_Animatic : MonoBehaviour
{
    [SerializeField] private VideoPlayer m_videoPlayer;

    [SerializeField] private UnityEvent OnVideoEnd;

    private void Awake()
    {
        if(m_videoPlayer)
        {
            m_videoPlayer.loopPointReached += VideoEnded;
        }
    }

    private void VideoEnded(VideoPlayer _videoPlayer)
    {
        OnVideoEnd.Invoke();
    }
}
