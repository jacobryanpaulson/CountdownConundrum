using System.Collections.Generic;
using UnityEngine;

public class PressureButton : MonoBehaviour
{
    [Header("Button Visuals")]
    [SerializeField] private SpriteRenderer buttonRenderer;
    [SerializeField] private Sprite releasedSprite;
    [SerializeField] private Sprite pressedSprite; 

    [Header("Controlled Object")]
    [SerializeField] private BasicDoor controlledDoor;

    private readonly HashSet<Collider2D> activatorsOnButton =
        new HashSet<Collider2D>();

    public bool IsPressed => activatorsOnButton.Count > 0;

    private void Awake()
    {
        if (buttonRenderer == null)
        {
            buttonRenderer = GetComponent<SpriteRenderer>();
        }

        UpdateButtonVisual();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ButtonActivator activator =
            other.GetComponentInParent<ButtonActivator>();

        if (activator == null)
        {
            return;
        }

        activatorsOnButton.Add(other);
        UpdateButtonVisual();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        bool activatorWasRemoved = activatorsOnButton.Remove(other);

        if (!activatorWasRemoved)
        {
            return;
        }

        UpdateButtonVisual();
    }

    private void LateUpdate()
    {
        int removedActivators =
            activatorsOnButton.RemoveWhere(activator => activator == null);

        if (removedActivators > 0)
        {
            UpdateButtonVisual();
        }
    }

    private void UpdateButtonVisual()
    {
        if (buttonRenderer != null)
        {
            buttonRenderer.sprite = IsPressed
            ? pressedSprite
            : releasedSprite;
        }

        if(controlledDoor != null)
        {
            controlledDoor.SetOpen(IsPressed);
        }

        
    }
}