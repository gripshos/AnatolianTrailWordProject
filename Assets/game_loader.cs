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
    public string[] pieWord;
    string[] germanicWord;
    int syllableCount = 0;
    System.Random randomNum = new System.Random();
    public bool[][] finishedSyllables;
    public bool solved;
    public Transform anim1, anim2, anim3;
    bool triskTimeGood;
    float triskTime;
    bool leftSolved;
    bool rightSolved;

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
        finishedSyllables = new bool[2][];
        finishedSyllables[0] = new bool[pieWord.Length];
        finishedSyllables[1] = new bool[pieWord.Length];
        for (int i = 0; i < pieWord.Length; i++)
        {
            finishedSyllables[0][i] = false;
            finishedSyllables[1][i] = false;
        }

        rightSolved = false;
        leftSolved = false;
    }

    void Update()
    {
        // Special h case
        specialH();

        if (solved)
        {
            solved = false;
        }

        if (answerCorrect())
        {
            headerTile[syllableCount, 1].GetComponent<TextMesh>().text = germanicWord[syllableCount];
            if (syllableCount < pieWord.Length)
            {
                syllableCount++;
            }
            generateBodyWords();
            displayBodyTiles();
        }

        // Change between letters
        //changeLetter();

        // If the puzzle is finished
        if (finishedSyllables[0].All(finished => finished) && finishedSyllables[1].All(finished => finished))
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
        // Loop for each column
        for (int i = 0; i < 7; i++)
        {
            // Loop for each page
            for (int j = 0; j < 2; j++)
            {
                // Loop for each row
                for (int k = 0; k < 3; k++)
                {
                    // Create string and find
                    initializingString = "Page " + (j + 1).ToString() + " body " + (k + 1).ToString() + " " + (i + 1).ToString();
                    bodyTile[i, k, j] = GameObject.Find(initializingString);
                }
                // Headers only have 1 row, so create string and find
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
                }
            }
        }

        // Loop for each word needing to be generated
        for (int j = 0; j < 3; j++)
        {
            // Generate random word
            int rand = randomNum.Next(0, matchingWords.Count);
            bodyWordIndex[j] = matchingWords[rand];
            matchingWords.RemoveAt(rand);
        }
    }

    // This function displays both header words
    void displayHeaderTiles()
    {
        // Spacing for pieWord
        if (pieWord.Length == 2)
        {
            headerTile[0, 0].transform.position = new Vector2(-4f, 3f);
            headerTile[1, 0].transform.position = new Vector2(-3f, 3f);
        }
        else if (pieWord.Length == 3)
        {
            headerTile[0, 0].transform.position = new Vector2(-4.5f, 3f);
            headerTile[1, 0].transform.position = new Vector2(-3.5f, 3f);
            headerTile[2, 0].transform.position = new Vector2(-2.5f, 3f);
        }
        else if (pieWord.Length == 4)
        {
            headerTile[0, 0].transform.position = new Vector2(-5f, 3f);
            headerTile[1, 0].transform.position = new Vector2(-4f, 3f);
            headerTile[2, 0].transform.position = new Vector2(-3f, 3f);
            headerTile[3, 0].transform.position = new Vector2(-2f, 3f);
        }
        else if (pieWord.Length == 5)
        {
            headerTile[0, 0].transform.position = new Vector2(-5.5f, 3f);
            headerTile[1, 0].transform.position = new Vector2(-4.5f, 3f);
            headerTile[2, 0].transform.position = new Vector2(-3.5f, 3f);
            headerTile[3, 0].transform.position = new Vector2(-2.5f, 3f);
            headerTile[4, 0].transform.position = new Vector2(-1.5f, 3f);
        }
        else if (pieWord.Length == 6)
        {
            headerTile[0, 0].transform.position = new Vector2(-6f, 3f);
            headerTile[1, 0].transform.position = new Vector2(-5f, 3f);
            headerTile[2, 0].transform.position = new Vector2(-4f, 3f);
            headerTile[3, 0].transform.position = new Vector2(-3f, 3f);
            headerTile[4, 0].transform.position = new Vector2(-2f, 3f);
            headerTile[5, 0].transform.position = new Vector2(-1f, 3f);
        }
        else if (pieWord.Length == 7)
        {
            headerTile[0, 0].transform.position = new Vector2(-6.5f, 3f);
            headerTile[1, 0].transform.position = new Vector2(-5.5f, 3f);
            headerTile[2, 0].transform.position = new Vector2(-4.5f, 3f);
            headerTile[3, 0].transform.position = new Vector2(-3.5f, 3f);
            headerTile[4, 0].transform.position = new Vector2(-2.5f, 3f);
            headerTile[5, 0].transform.position = new Vector2(-1.5f, 3f);
            headerTile[6, 0].transform.position = new Vector2(-0.5f, 3f);
        }

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

        // Spacing for germanicWord
        if (germanicWord.Length == 2)
        {
            headerTile[0, 1].transform.position = new Vector2(3f, 3f);
            headerTile[1, 1].transform.position = new Vector2(4f, 3f);
        }
        else if (germanicWord.Length == 3)
        {
            headerTile[0, 1].transform.position = new Vector2(2.5f, 3f);
            headerTile[1, 1].transform.position = new Vector2(3.5f, 3f);
            headerTile[2, 1].transform.position = new Vector2(4.5f, 3f);
        }
        else if (germanicWord.Length == 4)
        {
            headerTile[0, 1].transform.position = new Vector2(2f, 3f);
            headerTile[1, 1].transform.position = new Vector2(3f, 3f);
            headerTile[2, 1].transform.position = new Vector2(4f, 3f);
            headerTile[3, 1].transform.position = new Vector2(5f, 3f);
        }
        else if (germanicWord.Length == 5)
        {
            headerTile[0, 1].transform.position = new Vector2(1.5f, 3f);
            headerTile[1, 1].transform.position = new Vector2(2.5f, 3f);
            headerTile[2, 1].transform.position = new Vector2(3.5f, 3f);
            headerTile[3, 1].transform.position = new Vector2(4.5f, 3f);
            headerTile[4, 1].transform.position = new Vector2(5.5f, 3f);
        }
        else if (germanicWord.Length == 6)
        {
            headerTile[0, 1].transform.position = new Vector2(1f, 3f);
            headerTile[1, 1].transform.position = new Vector2(2f, 3f);
            headerTile[2, 1].transform.position = new Vector2(3f, 3f);
            headerTile[3, 1].transform.position = new Vector2(4f, 3f);
            headerTile[4, 1].transform.position = new Vector2(5f, 3f);
            headerTile[5, 1].transform.position = new Vector2(6f, 3f);
        }
        else if (germanicWord.Length == 7)
        {
            headerTile[0, 1].transform.position = new Vector2(0.5f, 3f);
            headerTile[1, 1].transform.position = new Vector2(1.5f, 3f);
            headerTile[2, 1].transform.position = new Vector2(2.5f, 3f);
            headerTile[3, 1].transform.position = new Vector2(3.5f, 3f);
            headerTile[4, 1].transform.position = new Vector2(4.5f, 3f);
            headerTile[5, 1].transform.position = new Vector2(6.5f, 3f);
            headerTile[6, 1].transform.position = new Vector2(6.5f, 3f);
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

            // Spacing for pieWord
            float x = i;
            float y = 1f - 1.5f * x;
            if (bodyWord.Length == 2)
            {
                bodyTile[0, i, 0].transform.position = new Vector2(-4f, y);
                bodyTile[1, i, 0].transform.position = new Vector2(-3f, y);
            }
            else if (bodyWord.Length == 3)
            {
                bodyTile[0, i, 0].transform.position = new Vector2(-4.5f, y);
                bodyTile[1, i, 0].transform.position = new Vector2(-3.5f, y);
                bodyTile[2, i, 0].transform.position = new Vector2(-2.5f, y);
            }
            else if (bodyWord.Length == 4)
            {
                bodyTile[0, i, 0].transform.position = new Vector2(-5f, y);
                bodyTile[1, i, 0].transform.position = new Vector2(-4f, y);
                bodyTile[2, i, 0].transform.position = new Vector2(-3f, y);
                bodyTile[3, i, 0].transform.position = new Vector2(-2f, y);
            }
            else if (bodyWord.Length == 5)
            {
                bodyTile[0, i, 0].transform.position = new Vector2(-5.5f, y);
                bodyTile[1, i, 0].transform.position = new Vector2(-4.5f, y);
                bodyTile[2, i, 0].transform.position = new Vector2(-3.5f, y);
                bodyTile[3, i, 0].transform.position = new Vector2(-2.5f, y);
                bodyTile[4, i, 0].transform.position = new Vector2(-1.5f, y);
            }
            else if (bodyWord.Length == 6)
            {
                bodyTile[0, i, 0].transform.position = new Vector2(-6f, y);
                bodyTile[1, i, 0].transform.position = new Vector2(-5f, y);
                bodyTile[2, i, 0].transform.position = new Vector2(-4f, y);
                bodyTile[3, i, 0].transform.position = new Vector2(-3f, y);
                bodyTile[4, i, 0].transform.position = new Vector2(-2f, y);
                bodyTile[5, i, 0].transform.position = new Vector2(-1f, y);
            }
            else if (bodyWord.Length == 7)
            {
                bodyTile[0, i, 0].transform.position = new Vector2(-6.5f, y);
                bodyTile[1, i, 0].transform.position = new Vector2(-5.5f, y);
                bodyTile[2, i, 0].transform.position = new Vector2(-4.5f, y);
                bodyTile[3, i, 0].transform.position = new Vector2(-3.5f, y);
                bodyTile[4, i, 0].transform.position = new Vector2(-2.5f, y);
                bodyTile[5, i, 0].transform.position = new Vector2(-1.5f, y);
                bodyTile[6, i, 0].transform.position = new Vector2(-0.5f, y);
            }

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

            // Spacing for pieWord
            float x = i;
            float y = 1f - 1.5f * x;
            if (bodyWord.Length == 2)
            {
                bodyTile[0, i, 1].transform.position = new Vector2(3f, y);
                bodyTile[1, i, 1].transform.position = new Vector2(4f, y);
            }
            else if (bodyWord.Length == 3)
            {
                bodyTile[0, i, 1].transform.position = new Vector2(2.5f, y);
                bodyTile[1, i, 1].transform.position = new Vector2(3.5f, y);
                bodyTile[2, i, 1].transform.position = new Vector2(4.5f, y);
            }
            else if (bodyWord.Length == 4)
            {
                bodyTile[0, i, 1].transform.position = new Vector2(2f, y);
                bodyTile[1, i, 1].transform.position = new Vector2(3f, y);
                bodyTile[2, i, 1].transform.position = new Vector2(4f, y);
                bodyTile[3, i, 1].transform.position = new Vector2(5f, y);
            }
            else if (bodyWord.Length == 5)
            {
                bodyTile[0, i, 1].transform.position = new Vector2(1.5f, y);
                bodyTile[1, i, 1].transform.position = new Vector2(2.5f, y);
                bodyTile[2, i, 1].transform.position = new Vector2(3.5f, y);
                bodyTile[3, i, 1].transform.position = new Vector2(4.5f, y);
                bodyTile[4, i, 1].transform.position = new Vector2(5.5f, y);
            }
            else if (bodyWord.Length == 6)
            {
                bodyTile[0, i, 1].transform.position = new Vector2(1f, y);
                bodyTile[1, i, 1].transform.position = new Vector2(2f, y);
                bodyTile[2, i, 1].transform.position = new Vector2(3f, y);
                bodyTile[3, i, 1].transform.position = new Vector2(4f, y);
                bodyTile[4, i, 1].transform.position = new Vector2(5f, y);
                bodyTile[5, i, 1].transform.position = new Vector2(6f, y);
            }
            else if (bodyWord.Length == 7)
            {
                bodyTile[0, i, 1].transform.position = new Vector2(0.5f, y);
                bodyTile[1, i, 1].transform.position = new Vector2(1.5f, y);
                bodyTile[2, i, 1].transform.position = new Vector2(2.5f, y);
                bodyTile[3, i, 1].transform.position = new Vector2(3.5f, y);
                bodyTile[4, i, 1].transform.position = new Vector2(4.5f, y);
                bodyTile[5, i, 1].transform.position = new Vector2(5.5f, y);
                bodyTile[6, i, 1].transform.position = new Vector2(6.5f, y);
            }

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

    // Returns true if the user has selected the correct letters on both sides of the game
    bool answerCorrect()
    {

        string[][] bodyWords = new string[3][];

        //may need these for solving other side of words
        string[][] translatedBodyWords = new string[3][];
        bool bothSolved = false;

        // Get body words
        for (int i = 0; i < 3; i++)
        {
            bodyWords[i] = SolvedWordList[bodyWordIndex[i]][2].Split('-');
            translatedBodyWords[i] = SolvedWordList[bodyWordIndex[i]][5].Split('-');
        }

        if (!leftSolved)
        {
            leftSolved = testLeftSide(bodyWords);
        }

        if (!rightSolved)
        {
            rightSolved = testRightSide(translatedBodyWords);
        }

        if (leftSolved && rightSolved)
        {
            Debug.Log("BOFF");
            bothSolved = true;
            leftSolved = false;
            rightSolved = false;
        }

        return bothSolved;
    }

    //Function that returns true if the user selects all correct letters on a side
    bool testLeftSide(string[][] bodyWords)
    {

        bool[] rowCorrect = new bool[3];
        body_tile_behavior focusTile;

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
                else if (focusTile.isSelected && (pieWord[syllableCount] != bodyWords[i][j]))
                {
                    rowCorrect[i] = false;
                    return false;
                }
            }
        }
        if (rowCorrect.All(finished => finished))
        {
            solved = true;
            finishedSyllables[0][syllableCount] = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    //Function that tests whether a side is correct
    bool testRightSide(string[][] bodyWords)
    {
        bool[] rowCorrect = new bool[3];
        body_tile_behavior focusTile;

        // Loop for each row
        for (int i = 0; i < 3; i++)
        {
            // Loop for each column
            for (int j = 0; j < bodyWords[i].Length; j++)
            {
                focusTile = bodyTile[j, i, 1].GetComponent<body_tile_behavior>();
                // If selected and correct, set that row to correct
                if (focusTile.isSelected && (germanicWord[syllableCount] == bodyWords[i][j]))
                {
                    rowCorrect[i] = true;
                }
                // If selected and incorrect, set that row to incorrect and leave the loop
                else if (focusTile.isSelected && (germanicWord[syllableCount] != bodyWords[i][j]))
                {
                    rowCorrect[i] = false;
                    return false;
                }
            }
        }
        if (rowCorrect.All(finished => finished))
        {
            finishedSyllables[1][syllableCount] = true;
            solved = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    // Function for switching between letters when clicked
/*    void changeLetter()
    {
        if (!pieWord.Contains("h₁") && !pieWord.Contains("h₂") && !pieWord.Contains("h₃"))
        {
            for (int i = 2; i < pieWord.Length; i++)
            {
                if (headerTile[i, 0].GetComponent<header_tile_behavior>().isSelected && syllableCount != i)
                {
                    syllableCount = i;
                    generateBodyWords();
                    displayBodyTiles();
                }
            }
        }
    }*/

    void specialH()
    {
        int hLocation = 0;

        // If pieword has an h, get the location
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
        if ((headerTile[hLocation, 0].GetComponent<header_tile_behavior>().isSelected) && (pieWord.Contains("h₁") || pieWord.Contains("h₂") || pieWord.Contains("h₃")))
        {
            headerTile[pieWord.Length - 1, 0].GetComponent<TextMesh>().text = null;
            pieWord = UnsolvedWordList[starterWordIndex][3].Split('-');
            displayHeaderTiles();
            generateBodyWords();
            displayBodyTiles();

            // Play animation
            int rand = randomNum.Next(0, 3);
            if (rand == 0)
                Instantiate(anim1, headerTile[hLocation, 0].transform.position, headerTile[hLocation, 0].transform.rotation);
            else if (rand == 1)
                Instantiate(anim2, headerTile[hLocation, 0].transform.position, headerTile[hLocation, 0].transform.rotation);
            else if (rand == 2)
                Instantiate(anim3, headerTile[hLocation, 0].transform.position, headerTile[hLocation, 0].transform.rotation);
        }
    }

    void triskelion()
    {
        bool[,] rowSelected = new bool[3, 2];
        string[][] bodyWords = new string[3][];
        bool oneSelected = false;
        bool multipleSelected = false;
        int[] selectedRow = new int[2];

        // Get body words
        for (int i = 0; i < 3; i++)
        {
            bodyWords[i] = SolvedWordList[bodyWordIndex[i]][2].Split('-');
        }

        // Loop for each row
        for (int i = 0; i < 3; i++)
        {
            // Loop for each column
            for (int j = 0; j < bodyWords[i].Length; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    if (bodyTile[i, j, k].GetComponent<body_tile_behavior>().isSelected)
                    {
                        rowSelected[i, k] = true;
                    }
                    else
                    {
                        rowSelected[i, k] = false;
                    }
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (!oneSelected && rowSelected[i, j])
                {
                    oneSelected = true;
                    selectedRow[0] = i;
                    selectedRow[1] = j;
                }
                else if (oneSelected && rowSelected[i, j])
                {
                    multipleSelected = true;
                    triskTimeGood = false;
                }
            }
        }

        if (!oneSelected)
        {
            triskTimeGood = false;
        }

        if (oneSelected && !multipleSelected && !triskTimeGood)
        {
            triskTime = Time.time;
            triskTimeGood = true;
        }

        if (triskTimeGood && ((triskTime - Time.time) == 4))
        {
            //open triskelion
            Debug.Log("trisk has been opened");
        }
    }
}