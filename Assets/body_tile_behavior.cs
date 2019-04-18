using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class body_tile_behavior : MonoBehaviour
{
    bool isBeingClicked;
    public bool isSelected;
    RectTransform rectangle;
    Vector3 mouseLocation;
    game_loader game;

    void Start()
    {
        rectangle = GetComponent<RectTransform>();
        isSelected = false;
        game = GameObject.Find("Master Script").GetComponent<game_loader>();
    }

    void Update()
    {
        // Convert mouse coordinates to world coordinates to local coordinates
        mouseLocation = Input.mousePosition;
        mouseLocation.z = 10F;
        mouseLocation = Camera.main.ScreenToWorldPoint(mouseLocation);
        mouseLocation = rectangle.InverseTransformPoint(mouseLocation);

        // Is the tile being clicked?
        if (rectangle.rect.Contains(mouseLocation))
        {
            isBeingClicked = true;
        }
        else
        {
            isBeingClicked = false;
        }

        // Reset status when the puzzle is 

        if (game.solved)
        {
            isBeingClicked = false;
            isSelected = false;
        }

        if ((Input.GetMouseButton(0) && isBeingClicked) || isSelected)
        {
            isSelected = true;
            GetComponent<TextMesh>().color = Color.green;
        }
        else
        {
            GetComponent<TextMesh>().color = Color.black;
        }
        if (!Input.GetMouseButton(0))
        {
            isSelected = false;
        }
    }
}
