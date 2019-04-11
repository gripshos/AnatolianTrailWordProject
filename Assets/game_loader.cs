using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class game_loader : MonoBehaviour
{
    GameObject[,,] bodyTile = new GameObject[7, 3, 2];
    GameObject[,] headerTile = new GameObject[7, 2];
    List<string[]> SolvedWordList = new List<string[]>();
    List<string[]> UnsolvedWordList = new List<string[]>();
    int starterWordIndex;
    int[] bodyWordIndex = new int[3];
    string[] pieWord;
    string[] germanicWord;
    int syllableCount = 0;
    System.Random randomNum = new System.Random();
    bool[] finishedSyllables;
    public bool solved;
    bool headerSelect;

    void Start()
    { 
        // Call function to create data structure for word list
        CreateDataStructure();

        // Call function to load game objects 
        initTiles();

        // Generate starting words
        starterWordIndex = randomNum.Next(0, UnsolvedWordList.Count - 1);
        pieWord = UnsolvedWordList[starterWordIndex][2].Split('-');
        germanicWord = UnsolvedWordList[starterWordIndex][5].Split('-');

        // Display header tiles
        displayHeaderTiles();

        // Display body tiles
        if (!pieWord.Contains("h₁") && !pieWord.Contains("h₂") && !pieWord.Contains("h₃"))
        {
            generateBodyWords();
            displayBodyTiles();
        }

        // Initialize the finished syllables tracker
        finishedSyllables = new bool[germanicWord.Length];
        for (int i = 0; i < germanicWord.Length; i++)
        {
            finishedSyllables[i] = false;
        }

        headerSelect = false;
    }

    void Update()
    {
        int hLocation = 0;

        if (solved)
        {
            solved = false;
            headerTile[syllableCount, 1].GetComponent<TextMesh>().text = germanicWord[syllableCount];
            syllableCount++;
            generateBodyWords();
            displayBodyTiles();
        }

        // if pieword has an h, get the location

        if (pieWord.Contains("h₁") || pieWord.Contains("h₂") || pieWord.Contains("h₃"))
        {
            for (int i = 0; i < pieWord.Length; i++)
            {
                if (pieWord[i].Contains("h₁") || pieWord[i].Contains("h₂") || pieWord[i].Contains("h₃"))
                {
                    hLocation = i;
                }
            }
        }

        // If h is clicked
        if ((headerTile[hLocation,0].GetComponent<header_tile_behavior>().isSelected) && (pieWord.Contains("h₁") || pieWord.Contains("h₂") || pieWord.Contains("h₃")))
        {
            headerTile[pieWord.Length - 1, 0].GetComponent<TextMesh>().text = null;
            pieWord = UnsolvedWordList[starterWordIndex][3].Split('-');
            displayHeaderTiles();
            generateBodyWords();
            displayBodyTiles();
        }

        // If solve part of word
        if (answerCorrect())
        {
            solved = true;
        }
        else
        {
            solved = false;
        }

        // letter scroll
        for (int i = 0; i < pieWord.Length; i++)
        {
            if (headerTile[i, 0].GetComponent<header_tile_behavior>().isSelected  && !headerSelect)
            {
                headerTile[syllableCount, 1].GetComponent<TextMesh>().text = germanicWord[syllableCount];
                syllableCount = i;
                generateBodyWords();
                displayBodyTiles();
                headerSelect = true;
            }
        }
        if (!Input.GetMouseButton(0))
        {
            headerSelect = false;
        }

        // If the puzzle is finished
        if (finishedSyllables.All(finished => finished))
        {
            Debug.Log("you win!");
            //you win!
        }
    }

    // Create structure for word list
    void CreateDataStructure()
    {
        // Bring in data from file
        string path = "Assets/PIE_Lexicon_in_Trxwentix.txt";
        StreamReader reader = new StreamReader(path);
        string line = reader.ReadLine();

        // Initialize while loop
        while (line != null)
        {
            // Split each line into parts and add to list array
            string[] words = line.Split('\t');
            if (words[7] == "Y")
                UnsolvedWordList.Add(words);
            else if (words[7] == "N")
                SolvedWordList.Add(words);

            // Prepare for next run of loop
            line = reader.ReadLine();
        }
    }

    // This function will load in the tile objects incrementally by generating the name as a string and finding the GameObect of that name 
    void initTiles()
    {
        string initializingString;
        for (int i = 0; i < 7; i++)
        // loop for each column
        {
            for (int j = 0; j < 2; j++)
            // loop for each page
            {
                for (int k = 0; k < 3; k++)
                // loop for each row
                {
                    //create string and find
                    initializingString = "Page " + (j + 1).ToString() + " body " + (k + 1).ToString() + " " + (i + 1).ToString();
                    bodyTile[i, k, j] = GameObject.Find(initializingString);
                }
                // headers only have 1 row, so create string and find
                initializingString = "Page " + (j + 1).ToString() + " header " + (i + 1).ToString();
                headerTile[i, j] = GameObject.Find(initializingString);
            }
        }
    }

    // This function generates correct words for the body based on the given starter word
    void generateBodyWords()
    {
        string[] temp1, temp2;
        List<int> matchingWords = new List<int>();

        // Create list of all matching words
        for (int i = 0; i < SolvedWordList.Count - 1; i++)
        {
            temp1 = SolvedWordList[i][2].Split('-');
            temp2 = SolvedWordList[i][5].Split('-');
            if (temp1.Length - 1 >= syllableCount)
            {
                if (temp1.Contains(pieWord[syllableCount]) && temp2.Contains(germanicWord[syllableCount]))
                {
                    matchingWords.Add(i);
                    Debug.Log(pieWord[syllableCount] + " " + SolvedWordList[i][2]);
                }
            }
        }

        // Loop for each word needing to be generated
        for (int j = 0; j < 3; j++)
        {
            // Generate random word
            if (matchingWords.Count > 0)
            {
                int rand = randomNum.Next(0, matchingWords.Count - 1);
                bodyWordIndex[j] = matchingWords[rand];
                matchingWords.RemoveAt(rand);
            }
        }
    }

    // This function displays both header words
    void displayHeaderTiles()
    {
        // Display PIE header
        for (int i = 0; i < pieWord.Length; i++)
        {
            headerTile[i, 0].GetComponent<TextMesh>().text = pieWord[i];
            headerTile[i, 0].GetComponent<TextMesh>().color = Color.black;
            headerTile[i, 0].GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
            headerTile[i, 0].GetComponent<TextMesh>().alignment = TextAlignment.Center;
            headerTile[i, 0].GetComponent<TextMesh>().characterSize = 0.05f;
            headerTile[i, 0].GetComponent<TextMesh>().fontSize = 200;
        }

        // Display Germanic header
        for (int i = 0; i < germanicWord.Length; i++)
        {
            headerTile[i, 1].GetComponent<TextMesh>().text = "_";
            headerTile[i, 1].GetComponent<TextMesh>().color = Color.black;
            headerTile[i, 1].GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
            headerTile[i, 1].GetComponent<TextMesh>().alignment = TextAlignment.Center;
            headerTile[i, 1].GetComponent<TextMesh>().characterSize = 0.05f;
            headerTile[i, 1].GetComponent<TextMesh>().fontSize = 200;
        }
    }

    // This function displays the body tiles
    void displayBodyTiles()
    {
        // Clear bodyTiles
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                bodyTile[j, i, 0].GetComponent<TextMesh>().text = null;
                bodyTile[j, i, 1].GetComponent<TextMesh>().text = null;
            }
        }

        // Display PIE body tiles
        for (int i = 0; i < 3; i++)
        {
            string[] bodyWord = SolvedWordList[bodyWordIndex[i]][2].Split('-');

            for (int j = 0; j < bodyWord.Length; j++)
            {
                bodyTile[j, i, 0].GetComponent<TextMesh>().text = bodyWord[j];
                bodyTile[j, i, 0].GetComponent<TextMesh>().color = Color.black;
                bodyTile[j, i, 0].GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
                bodyTile[j, i, 0].GetComponent<TextMesh>().alignment = TextAlignment.Center;
                bodyTile[j, i, 0].GetComponent<TextMesh>().characterSize = 0.05f;
                bodyTile[j, i, 0].GetComponent<TextMesh>().fontSize = 200;
            }
        }

        // Display Germanic body tiles
        for (int i = 0; i < 3; i++)
        {
            string[] bodyWord = SolvedWordList[bodyWordIndex[i]][5].Split('-');

            for (int j = 0; j < bodyWord.Length; j++)
            {
                bodyTile[j, i, 1].GetComponent<TextMesh>().text = bodyWord[j];
                bodyTile[j, i, 1].GetComponent<TextMesh>().color = Color.black;
                bodyTile[j, i, 1].GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
                bodyTile[j, i, 1].GetComponent<TextMesh>().alignment = TextAlignment.Center;
                bodyTile[j, i, 1].GetComponent<TextMesh>().characterSize = 0.05f;
                bodyTile[j, i, 1].GetComponent<TextMesh>().fontSize = 200;
            }
        }
    }

    // Returns true if the user has selected the correct letters
    bool answerCorrect()
    {
        string[][] bodyWords = new string[3][];

        //may need these for solving other side of words
        string[][] translatedBodyWords = new string[3][];
        bool[] translatedCorrect = new bool[3];


        bool[] rowCorrect = new bool[3];
        body_tile_behavior focusTile;

        // Get body words
        for (int i = 0; i < 3; i++)
        {
            bodyWords[i] = SolvedWordList[bodyWordIndex[i]][2].Split('-');
            
        }

        /*for (int e = 3; e < 6; e++)
        {
            translatedBodyWords[e] = SolvedWordList[bodyWordIndex[e]][2].Split('-');
            Debug.Log(translatedBodyWords[e]);
        }*/

        // Loop for each row
        for (int i = 0; i < 3; i++)
        {
            // Loop for each column
            for (int j = 0; j < bodyWords[i].Length; j++)
            {
                focusTile = bodyTile[j, i, 0].GetComponent<body_tile_behavior>();
                // If selected and correct, set that row to correct
                if (focusTile.isSelected && (pieWord[syllableCount] == bodyWords[i][j]))
                {
                    rowCorrect[i] = true;
                }
                // If selected and incorrect and not h, set that row to incorrect and leave the loop
                else if ((focusTile.isSelected && (pieWord[syllableCount] != bodyWords[i][j]))
                    && !(bodyWords[i][j] == "h₁" || bodyWords[i][i] == "h₂" || bodyWords[i][j] == "h₃"))
                {
                    rowCorrect[i] = false;
                    break;
                }
            }
        }

        // If all rows are correct, return true
        if (rowCorrect[0] && rowCorrect[1] && rowCorrect[2])
        {
            finishedSyllables[syllableCount] = true;
            return true;
        }
        // Otherwise return false 
        else 
        {
            return false;
        }
    }
}