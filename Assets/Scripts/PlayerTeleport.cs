using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlayerTeleport : MonoBehaviour
{
    public GameObject interactText;
    private GameObject currentTeleporter;
    private static bool isOnCooldown = false;
    [SerializeField] private float teleportCooldown =.5f;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        interactText.SetActive(false);
    }

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
           if (!isOnCooldown)
            {
                Teleporter teleporter = currentTeleporter.GetComponent<Teleporter>();
 
                if(teleporter != null)
                {
                  StartCoroutine(TeleportRoutine(teleporter.GetDestination().position));
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))
        {
            currentTeleporter = collision.gameObject;
            interactText.SetActive(true);


            //Uncomment This if you want more portal style and remove the Input Keydown function in update
            /*if (!isOnCooldown)
            {
                Teleporter teleporter = currentTeleporter.GetComponent<Teleporter>();

                if(teleporter != null)
                {
                  StartCoroutine(TeleportRoutine(teleporter.GetDestination().position));
                }
            }*/
        }
    }
    private IEnumerator TeleportRoutine(Vector3 destinationPosition){
        isOnCooldown = true;
         
         if (playerController != null)
        {
            playerController.TeleportTo(destinationPosition);
        }

        

        yield return new WaitForSeconds(teleportCooldown);

        isOnCooldown = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
          if (collision.CompareTag("Teleporter"))
        {
            if(collision.gameObject == currentTeleporter)
            {
                currentTeleporter = null;
                interactText.SetActive(false);

               

            }
        }
    }
}
