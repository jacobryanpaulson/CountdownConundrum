using UnityEngine;

public class ColorChange : MonoBehaviour
{
  [SerializeField] private Color[] loopColors = new Color[] {Color.red, Color.blue, Color.green, Color.orange, Color.pink};

  public void ColorSet(int loopIndex)
    {
        int colorIndex = loopIndex % loopColors.Length;
        Color assignedColor = loopColors[colorIndex];

        if(TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.color = assignedColor;
        }
        else
        {
            GetComponentInChildren<SpriteRenderer>().color = assignedColor;
        }
    }


   
}
