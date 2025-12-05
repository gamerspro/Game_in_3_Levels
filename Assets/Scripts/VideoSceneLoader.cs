using UnityEngine;
using UnityEngine.Video;
using System.IO;
using UnityEngine.SceneManagement;

public class VideoSceneLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string videoFileName = "intro.webm";
    public string nextSceneName;

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, videoFileName);
        Debug.Log("Attempting to play video: " + path);

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = path;

        videoPlayer.prepareCompleted += (vp) => vp.Play();
        videoPlayer.loopPointReached += OnVideoEnd;

        videoPlayer.Prepare();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
