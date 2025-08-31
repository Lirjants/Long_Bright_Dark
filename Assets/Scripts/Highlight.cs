using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    private SpriteRenderer[] sprites;
    private Color[] originalColors;
    public Color highlightColor = Color.yellow;

    void Start()
    {
        // Grab all SpriteRenderers in children (including inactive if needed)
        sprites = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            originalColors[i] = sprites[i].color;
        }
    }

    void OnMouseEnter()
    {
        Debug.Log("Mouse entered " + gameObject.name);
        foreach (var sprite in sprites)
        {
            sprite.color = highlightColor;
        }
    }

    void OnMouseExit()
    {
        Debug.Log("Mouse exited " + gameObject.name);
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = originalColors[i];
        }
    }
}
