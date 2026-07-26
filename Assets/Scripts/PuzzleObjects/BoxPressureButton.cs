using System.Collections.Generic;
using UnityEngine;

public class BoxPressureButton : MonoBehaviour
{
    [Header("Button Visuals")]
    [SerializeField] private SpriteRenderer buttonRenderer;
    [SerializeField] private Sprite releasedSprite;
    [SerializeField] private Sprite pressedSprite;

    [Header("Controlled Object")]
    [SerializeField] private BasicDoor controlledDoor;

    private readonly HashSet<Collider2D> boxCollidersOnButton =
        new HashSet<Collider2D>();

    public bool IsPressed =>
        boxCollidersOnButton.Count > 0;

    private void Awake()
    {
        if (buttonRenderer == null)
        {
            buttonRenderer =
                GetComponent<SpriteRenderer>();
        }

        UpdateButtonState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GridBox box =
            other.GetComponentInParent<GridBox>();

        if (box == null)
        {
            return;
        }

        boxCollidersOnButton.Add(other);

        UpdateButtonState();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        bool boxWasRemoved =
            boxCollidersOnButton.Remove(other);

        if (!boxWasRemoved)
        {
            return;
        }

        UpdateButtonState();
    }

    private void LateUpdate()
    {
        int removedColliders =
            boxCollidersOnButton.RemoveWhere(
                boxCollider => boxCollider == null
            );

        if (removedColliders > 0)
        {
            UpdateButtonState();
        }
    }

    private void UpdateButtonState()
    {
        if (buttonRenderer != null)
        {
            buttonRenderer.sprite =
                IsPressed
                    ? pressedSprite
                    : releasedSprite;
        }

        if (controlledDoor != null)
        {
            controlledDoor.SetOpen(IsPressed);
        }
    }
}