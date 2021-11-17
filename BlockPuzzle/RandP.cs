using System;
namespace BlockPuzzle
{
    public class RandP
    {
        public Random rand = null; //new Random()

        public int[] nums;
        public int numsLeft;

        public RandP(int n, Random r)
        {
            rand = r;
            initNums(n);
        }

        public void initNums(int n)
        {
            nums = new int[n];
            for (int i = 0; i < n; i++)
            {
                nums[i] = i;

                numsLeft = n;
            }
        }
        public int nextInt()
        {
            // If there are no numbers left, just return 0
            if (numsLeft == 0) return 7;

            // Get a random index amongst the remaining numbers
            int index = (int)(numsLeft * rand.NextDouble());

            // The number of numbers left is decreased and numsLeft now
            // references the last index in the array in which a unique value
            // can be returned
            numsLeft--;

            // Save the return value
            int temp = nums[index];

            // Overwrite the element in position index with the
            // element in the last position containing a number
            // that has yet to be returned
            nums[index] = nums[numsLeft];

            // Return the selected value
            return temp;
        }
    }
}