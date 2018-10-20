using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {
    
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitializeList()
    {
        gridPositions.Clear();

        //Everything within this space will be randomly generated.
        //The reason it goes from 1 to columns - 1 is to keep the outer borders from being populated with impassable objects, thereby creating impassable levels
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));

            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        
        //Create the outer edge with impassable walls and floors, and then build everything inside it
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < columns + 1; y++)
            {
                //Create floors from the list of floor tiles at random based off the length of the array
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                //If the array position in the instantiation phase is on the outer edge, populate the location with an impassable outer wall tile
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                //According to what toInstantiate is equal to, create the new object at the array's position, and store it in the boardHolder variable
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
                
            }
        }
    }

    Vector3 RandomPosition()
    {
        //Generate a random location based off the stored positions in gridPositions, and then remove the location from gridPositions to make sure it isn't called again
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;

    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        //objectCount controls how many of a given object will spawn in the level
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            //Create the game object from the tile array and the random positon as defined by the RandomPosition() function
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    /*As a note, it seems possible to create this design based not off tiles (mostly) but based off game objects themselves.  
     It might be possible, instead of tiles, to create connected, pre-defined dungeon rooms and tie them together, including having an exit room at the end, plus a boss room.*/
    public void SetupScene (int level)
    {
        //Setup the board and list, and then lay down the walls and food at random
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        //The number of enemies themselves will increase as the level count increases, however it will be done logarithmically instead of linearly so as to not overflow the board
        //There will be no minimum or maximum the way this is setup as a result
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        //Finally, create the exit as a pre-defined location
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);
    }
}
