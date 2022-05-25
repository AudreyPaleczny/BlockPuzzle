using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Piece;

public class BlockQueue : MonoBehaviour
{
    public List<GameObject> prefabsOfPieces = new List<GameObject>();
    public List<GameObject> queue = new List<GameObject>(5);
    public Vector3 initialPosition;

    // 7 bag implementation here
    // the pieces are not random everytime we launch the game because the seed is the same

    public int[] nums;
    public int numsLeft;
    // public double p = Random.Range(0f, 1f);

    public void initNums(int n)
    {
        nums = new int[n];
        for (int i = 0; i < n; i++)
        {
            nums[i] = i;

            numsLeft = n;
        }

        shuffle(nums);
    }

    public void shuffle(int[] a)
    {
        for (int i = a.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i);
            int temp = a[j];
            a[j] = a[i];
            a[i] = temp;
        }
    }

    public int nextInt()
    {
        // If there are no numbers left, just return 0
        if (numsLeft == 0) return 7;

        // Get a random index amongst the remaining numbers
        // int index = (int) (numsLeft * p);

        // The number of numbers left is decreased and numsLeft now
        // references the last index in the array in which a unique value
        // can be returned
        numsLeft--;

        // Save the return value
        //int temp = nums[index];

        // Overwrite the element in position index with the
        // element in the last position containing a number
        // that has yet to be returned
        ///nums[index] = nums[numsLeft];

        // Return the selected value
        return nums[numsLeft];
    }

    public int randomizer()
    {
        int rgn = nextInt();
        if (rgn == 7)
        {
            initNums(7);
            rgn = nextInt();
        }

        return rgn;
    }

    public void makeQueue()
    {
        GameObject pieceOne = Instantiate(prefabsOfPieces[randomizer()]);
        GameObject pieceTwo = Instantiate(prefabsOfPieces[randomizer()]);
        GameObject pieceThree = Instantiate(prefabsOfPieces[randomizer()]);
        GameObject pieceFour = Instantiate(prefabsOfPieces[randomizer()]);
        GameObject pieceFive = Instantiate(prefabsOfPieces[randomizer()]);

        queue.Add(pieceOne);
        queue.Add(pieceTwo);
        queue.Add(pieceThree);
        queue.Add(pieceFour);
        queue.Add(pieceFive);

        pieceOne.transform.position = initialPosition;
        pieceTwo.transform.position = initialPosition + Vector3.down * 4;
        pieceThree.transform.position = initialPosition + Vector3.down * 8;
        pieceFour.transform.position = initialPosition + Vector3.down * 12;
        pieceFive.transform.position = initialPosition + Vector3.down * 16;
    }
    
    public void updateQueue()
    {
        Destroy(queue[0]);
        queue.Remove(queue[0]);
        GameObject temp = Instantiate(prefabsOfPieces[randomizer()]);
        queue.Add(temp);
    }

    public void printQueue()
    {
        queue[0].transform.position += Vector3.up * 4;
        queue[1].transform.position += Vector3.up * 4;
        queue[2].transform.position += Vector3.up * 4;
        queue[3].transform.position += Vector3.up * 4;
        queue[4].transform.position = initialPosition + Vector3.down * 16;
    }

}
