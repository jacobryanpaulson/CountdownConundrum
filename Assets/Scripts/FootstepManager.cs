using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip[] footstepClips;

    [SerializeField] private float minPitch = .9f;
    [SerializeField] private float maxPitch = 1.1f;


    public void PlayFootsteps()
    {
        if(footstepClips == null || footstepClips.Length == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, footstepClips.Length);
        AudioClip selectedClip = footstepClips[randomIndex];

        audioSource.pitch = Random.Range(minPitch, maxPitch);

        audioSource.PlayOneShot(selectedClip);

    }

}
