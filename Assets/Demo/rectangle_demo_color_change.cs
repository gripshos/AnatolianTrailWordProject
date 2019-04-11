using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rectangle_demo_color_change : MonoBehaviour
{
    SpriteRenderer rectangle_sprite;
    bool isSelected;
    bool previouslySelected;
    RectTransform rectangle;
    Vector3 mouseLocation;

    // Start is called before the first frame update
    void Start()
    {
        rectangle_sprite = GetComponent<SpriteRenderer>();
        rectangle = GetComponent<RectTransform>();
        previouslySelected = false;
    }

    // Update is called once per frame
    void Update()
    {
        mouseLocation = Input.mousePosition;
        mouseLocation.z = 10F;
        mouseLocation = Camera.main.ScreenToWorldPoint(mouseLocation);
        mouseLocation = rectangle.InverseTransformPoint(mouseLocation);

        if (rectangle.rect.Contains(mouseLocation))
        {
            isSelected = true;
        }
        else
        {
            isSelected = false;
        }

        if ((Input.GetMouseButton(0) && isSelected) || previouslySelected)
        {
            rectangle_sprite.color = Color.green;
            previouslySelected = true;
        }
        else
        {
            rectangle_sprite.color = Color.red;
        }

        if (!Input.GetMouseButton(0))
        {
            previouslySelected = false;
        }
    }
}