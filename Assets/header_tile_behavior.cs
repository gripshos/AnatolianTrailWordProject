using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class header_tile_behavior : MonoBehaviour
{
    public bool isSelected;
    RectTransform rectangle;
    Vector3 mouseLocation;
    game_loader game;

    void Start()
    {
        rectangle = GetComponent<RectTransform>();
        isSelected = false;
    }

    void Update()
    {
        if (isSelected)
            Debug.Log("isSelected = true");
        // Convert mouse coordinates to world coordinates to local coordinates
        mouseLocation = Input.mousePosition;
        mouseLocation.z = 10F;
        mouseLocation = Camera.main.ScreenToWorldPoint(mouseLocation);
        mouseLocation = rectangle.InverseTransformPoint(mouseLocation);

        // Is the tile being clicked?
        if (rectangle.rect.Contains(mouseLocation) && Input.GetMouseButton(0))
        {
            isSelected = true;
        }
        else
        {
            isSelected = false;
        }
    }
}
