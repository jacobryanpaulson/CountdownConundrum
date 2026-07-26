using UnityEngine;

public class BoxSound : MonoBehaviour
{
  
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;
    [SerializeField] private float minPitch = .9f;
    [SerializeField] private float maxPitch = 1.1f;

    public void BoxPushSound()
    {
        if (audioClip == null)
       return;
        
        AudioClip selectedClip = audioClip;
        
        audioSource.pitch = Random.Range(minPitch, maxPitch);

        audioSource.PlayOneShot(selectedClip);


    }
}
