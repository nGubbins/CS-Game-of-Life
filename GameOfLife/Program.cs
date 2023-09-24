using System;
using System.Timers;
//MIT License
/* 
 MIT License

Copyright (c) [2023] [hso141]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

namespace GameOfLife
{
    class Program
    {
        //static float timer = 0.0f;
        //static float tickTime = 2000.0f;
        static int tickCount = 0;
        static float tickTime = 200;
        static int nodeAmount = 1000;
        static int lineLength = 100;
        static float maxGenerations = 100.0f;
        static string characterImg = "o";
        static Node[] nodeArray;
        static int livingNodes = 0;

        //run automatically, if false requires input after each generation
        static bool auto = true;

        static void Main(string[] args)
        {
            Console.WriteLine("PRESS [ENTER] TO START\nNode Amount: " + nodeAmount.ToString() + "\nLine Length: " + lineLength.ToString() 
                + "\nTick Time: " + (tickTime/1000).ToString("0.00") + "s");
            Console.ReadLine();
            Console.Clear();
            //Create initial nodes
            GenerateNodes();
            Draw();
            
            //main loop
            while (tickCount < maxGenerations)
            {
               CheckNodes();
               Draw();

                if (auto)
                {
                    System.Threading.Thread.Sleep(int.Parse(tickTime.ToString()));
                }
            }

            //end
            Console.WriteLine("\n\nEND");

        }

        //Check nodes against their neighbours and set new state based on population
        static void CheckNodes()
        {
            int counter = 0;

            while (counter < nodeArray.Length)
            {
                int livingNeighbours = 0;
                int neighbourCounter = 0;

                //count living neighbours
                while (neighbourCounter < nodeArray[counter].neighbours.Length)
                {
                    if (nodeArray[counter].neighbours[neighbourCounter] != null)
                    {
                        if (nodeArray[counter].neighbours[neighbourCounter].active)
                            ++livingNeighbours;
                    }
                    ++neighbourCounter;
                }

                //prepare new state based on current state and living neighbour count
                if (nodeArray[counter].active)
                {
                    if (livingNeighbours < 2) //underpopulated (<2)
                    {
                        nodeArray[counter].newState = false;
                    }
                    else if (livingNeighbours >= 2 && livingNeighbours <= 3)
                    {
                        nodeArray[counter].newState = true;//sustainable (2-3)
                    }
                    else if(livingNeighbours > 3)
                    {
                        nodeArray[counter].newState = false;//overpopulated (>)
                    }
                }
                else
                {
                    if(livingNeighbours == 3)
                        nodeArray[counter].newState = true;//new development (exactly 3)
                }

                //increase counter
                ++counter;
            }

            //reset counter and living node count
            counter = 0;
            livingNodes = 0;

            //set new state and count currently living nodes
            while (counter < nodeArray.Length)
            {
                nodeArray[counter].active = nodeArray[counter].newState;

                if (nodeArray[counter].active)
                    ++livingNodes;

                ++counter;
            }

            //increase tick
            ++tickCount;
        }


        static void GenerateNodes()
        {

            //create node array
            nodeArray = new Node[nodeAmount];

            //Create random number generator
            Random rand = new Random();

            //initialise members
            int counter = 0;
            int xPos = 0;
            int yPos = 0;

            //Create each node and randomise initial node activation
            while (counter < nodeArray.Length)
            {
                //Create node
                nodeArray[counter] = new Node();

                //Set position
                nodeArray[counter].pos = new int[xPos, yPos];
                nodeArray[counter].xPos = xPos;
                nodeArray[counter].yPos = yPos;

                

                //Generate new random number (in this case a double between 0-1)
                float randomNumber = (float)rand.NextDouble();

                //Set activation
                if (randomNumber > 0.5)
                {
                    nodeArray[counter].active = true;
                    ++livingNodes;
                }

                //increase counters
                ++xPos;

                //new line
                if (xPos > (lineLength - 1))
                {
                    ++yPos;
                    xPos = 0;
                }

                ++counter;
            }

            //Set neighbours
            foreach (Node curNode in nodeArray)
            {
                SetNeighbours(curNode);
            }
        }

        //for reach node loop through all other nodes
        //if it is a neighbour, add it to the neighbour array
        static void SetNeighbours(Node curNode)
        {
            int neighbourCounter = 0;
            int counter = 0;
            curNode.neighbours = new Node[8];
            while (counter < nodeArray.Length)
            {
                //Node otherNode = nodeArray[counter]; //Leftover from when I was attempting to use foreach(Node otherNode in nodeArray)

                //check direction
                float xDelta = nodeArray[counter].xPos - curNode.xPos;
                float yDelta = nodeArray[counter].yPos - curNode.yPos;

                //check if within 1 space, if yes, is add to neighbour array
                if(xDelta <= 1 && xDelta >= -1)
                {
                    if (yDelta <= 1 && yDelta >= -1)
                    {
                        if (nodeArray[counter] != curNode)
                        {
                            curNode.neighbours[neighbourCounter] = nodeArray[counter];

                            ++neighbourCounter;

                            if (neighbourCounter > curNode.neighbours.Length - 1)
                                break;
                        }
                    }
                }

                ++counter;
            }
        }

        //Draw the current Node Array
        static void Draw()
        {
            if(!auto)
                Console.ReadLine();

            //clear for new generation and write header
            //Console.Clear();

            for (int i = 0; i < (lineLength + 3); ++i)
            {
                Console.Write("\r");
            }

            Console.WriteLine("\n ----NEW GENERATION----  Tick Count:" + tickCount.ToString() + "\n Living Nodes: " + livingNodes.ToString() + "\n");
            
            int count = 0;
            string drawString = "";

            //loop, foreach node in the list of the array and form a string then write it
            foreach (Node curNode in nodeArray)
            {
                if(curNode.active)
                {
                    drawString = drawString + characterImg;
                }
                else
                {
                    drawString = drawString + " ";
                }

                ++count;
                if(count == lineLength)
                {
                    Console.WriteLine(drawString);
                    drawString = "";
                    count = 0;
                }
            }
        }
    }
}
