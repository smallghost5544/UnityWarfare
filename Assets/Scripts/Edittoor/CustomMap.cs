using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class CustomMap : MonoBehaviour
{
    List<SpriteRenderer>sprites = new List<SpriteRenderer>();
    public int Layer = 0;
    public Vector2  range = new Vector2(0,0);
    public RectTransform rectTransform;
    public void SetSpriteLayer()
    {
        var currentLayer = Layer;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            sprites.Add(child.GetComponent<SpriteRenderer>());
        }
        foreach (var c in sprites)
        {
            c.  sortingOrder = Layer;
            Layer++;
        }
        sprites.Clear();
        Layer = currentLayer;   
    }

    public void SetBlockRange()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.position.x+range.x, rectTransform.position.y + range.y);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
