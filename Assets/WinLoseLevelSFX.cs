using UnityEngine;

public class WinLoseLevelSFX: MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip winaudioClip;
    [SerializeField] AudioClip loseaudioClip;
    [SerializeField] AudioClip dieaudioClip;
    [SerializeField] AudioClip resetaudioClip;
    [SerializeField] AudioClip cloneaudioClip;
    [SerializeField] AudioClip recordingaudioClip;
    
    [SerializeField] AudioClip buttonaudioClip1;
    [SerializeField] AudioClip buttonaudioClip2;
    [SerializeField] AudioClip doorSound;

    public static WinLoseLevelSFX Instance {get; private set;}

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else {Destroy(gameObject);}
    }


    public void WinSound()
    {
        if (winaudioClip == null)
       {return;}
        audioSource.PlayOneShot(winaudioClip);
    }
     public void LoseSound()
    {
        if (loseaudioClip == null && dieaudioClip == null)
       {return;}

        audioSource.volume = .3f;
        audioSource.PlayOneShot(loseaudioClip);
        audioSource.PlayOneShot(dieaudioClip);


    }
       public void ResetSound()
    {
        if (resetaudioClip == null)
       {return;}
        audioSource.PlayOneShot(resetaudioClip);
    }
       public void CloneSound()
    {
        if (cloneaudioClip == null)
       {return;}
       audioSource.volume = .3f;

        audioSource.PlayOneShot(cloneaudioClip);
    }
     public void RecordCloneSound()
    {
        if (recordingaudioClip == null)
       {return;}
        audioSource.PlayOneShot(recordingaudioClip);
    }
      public void ButtonSound()
    {
        if (buttonaudioClip1 == null && buttonaudioClip2 == null)
       {return;}
        audioSource.PlayOneShot(buttonaudioClip1);
        audioSource.PlayOneShot(buttonaudioClip2);
    }
      public void DoorSound()
    {
        if (doorSound == null)
       {return;}
        audioSource.PlayOneShot(doorSound);
    }
}
