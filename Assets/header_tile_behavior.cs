using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class header_tile_behavior : MonoBehaviour
{
    public bool isSelected;
    RectTransform rectangle;
    Vector3 mouseLocation;
    game_loader game;
    int thisTile;
    int thisPage;

    void Start()
    {
        rectangle = GetComponent<RectTransform>();
        game = GameObject.Find("Master Script").GetComponent<game_loader>();
        thisTile = gameObject.name[14] - '0' - 1;
        thisPage = gameObject.name[5] - '0' - 1;
    }

    void Update()
    {
        // Convert mouse coordinates to world coordinates to local coordinates
        mouseLocation = Input.mousePosition;
        mouseLocation.z = 10F;
        mouseLocation = Camera.main.ScreenToWorldPoint(mouseLocation);
        mouseLocation = rectangle.InverseTransformPoint(mouseLocation);

        // Is the tile being clicked?
        if (rectangle.rect.Contains(mouseLocation) && Input.GetMouseButtonDown(0))
        {
            isSelected = true;
        }
        else
        {
            isSelected = false;
        }

        if (thisTile < game.GetComponent<game_loader>().pieWord.Length)
        {
            if (game.GetComponent<game_loader>().finishedSyllables[thisPage][thisTile])
            {
                GetComponent<TextMesh>().color = Color.blue;
            }
        }
    }
}
