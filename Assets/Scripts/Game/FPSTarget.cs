using UnityEngine;

/* Unity FPS API 
 * https://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html?ampDeviceId=b5bb0e6a-1d5d-447a-9f08-6f1cc539fe39&ampSessionId=1766793079356&ampTimestamp=1766893756936
 * For those who still don’t get it, the value may be partially or fully ignored for multiple reasons. 
 * First, in case you’re enabling VSync (above 0), the framerate will be ignoring the targetFrameRate value (as explained here) and will, instead, use the system default framerate.
 * If your device has a default FPS of 120 with VSync at 1, it will try to render at 120 fps regardless of targetFrameRate. 
 * If you set VSync to 1-4 (max value applicable is 4), the fps will be equal to the device’s default framerate divided by the VSync value. 
 * So, if you set it to 2 and your device has a default framerate of 120, it will be render at 60 fps. At the same time, if it’s a default 30 fps, it will drop at a max of 15 fps.
 */
public class FPSTarget : MonoBehaviour
{
    public int targetFrameRate = 30;
    private void Start()
    {
        ///  If QualitySettings.vSyncCount is set to 0, 
        ///  then Application.targetFrameRate chooses a target frame rate for the game
        ///  vSyncCount != 0, then targetFrameRate is ignored.
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
}