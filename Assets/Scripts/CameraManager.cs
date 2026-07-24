using UnityEngine;

public class CameraManager : MonoBehaviour
{
   public Camera currentCam;
   public Camera nextCam;


   public void SwitchToNextCam()
    {
        nextCam.gameObject.SetActive(true);
        currentCam.gameObject.SetActive(false);
    }
   
}
