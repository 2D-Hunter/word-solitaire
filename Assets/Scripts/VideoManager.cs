using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [SerializeField] string videoFileName;
    // Start is called before the first frame update
    void Start()
    {
        //PlayVideo();
    }
    public void PlayVideo()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        if(videoPlayer)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            Debug.Log("Video Path: " + videoPath);
            videoPlayer.url = videoPath;
            videoPlayer.Play();
        }
        
    }
}
