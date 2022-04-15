using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionDetection 
{
    public static bool isColliding(Vector2Int[] minoPos, GameObject[][] objectMatrix)
    {
        //Vector2Int[] minoPos = minoCoords();
        //4 minos per piece
        for (int i = 0; i < minoPos.Length; i++)
        {
            int x = minoPos[i].x;
            int y = minoPos[i].y;

            //if (y < 0) return true;

            if (objectMatrix[y][x] != null)
            {
                //Debug.Log("Is Colliding");
                return true;
            }
        }

        return false;
    }
}
