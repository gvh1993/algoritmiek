using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntryPoint
{
#if WINDOWS || LINUX
  public static class Program
  {
        static List<Vector2> specialBuildingList = new List<Vector2>();
        [STAThread]        
    static void Main()
    {
      var fullscreen = false;
      read_input:
      switch (Microsoft.VisualBasic.Interaction.InputBox("Which assignment shall run next? (1, 2, 3, 4, or q for quit)", "Choose assignment", VirtualCity.GetInitialValue()))
      {
        case "1":
          using (var game = VirtualCity.RunAssignment1(SortSpecialBuildingsByDistance, fullscreen))
            game.Run();
          break;
        case "2":
          using (var game = VirtualCity.RunAssignment2(FindSpecialBuildingsWithinDistanceFromHouse, fullscreen))
            game.Run();
          break;
        case "3":
          using (var game = VirtualCity.RunAssignment3(FindRoute, fullscreen))
            game.Run();
          break;
        case "4":
          using (var game = VirtualCity.RunAssignment4(FindRoutesToAll, fullscreen))
            game.Run();
          break;
        case "q":
          return;
      }
      goto read_input;
    }

    private static IEnumerable<Vector2> SortSpecialBuildingsByDistance(Vector2 house, IEnumerable<Vector2> specialBuildings)
    {
            specialBuildingList = specialBuildings.ToList();
            //TODO MERGE SORT
      MergeSort(specialBuildingList, house, 0, specialBuildingList.Count -1);

      return specialBuildingList;
    }

        private static void MergeSort(List<Vector2> specialBuildings, Vector2 house, int firstIndex, int lastIndex)
        {
            if (firstIndex < lastIndex)
            {
                int middleIndex = (firstIndex + lastIndex) / 2;
                MergeSort(specialBuildings, house, firstIndex, middleIndex);
                MergeSort(specialBuildings, house, middleIndex +1, lastIndex);
                Merge(specialBuildings, house, firstIndex, middleIndex, lastIndex);
            }
        }

        private static void Merge(List<Vector2> specialBuildings, Vector2 house, int beginIndex, int middleIndex, int endIndex)
        {
            Vector2[] tempArray = new Vector2[endIndex - beginIndex + 1];

            int pointerLeft = beginIndex;
            int tempPointer = 0;
            int pointerRight = middleIndex + 1;


            while (pointerLeft <= middleIndex && pointerRight <= endIndex)
            {
                double distanceLeft = Vector2.Distance(house, specialBuildings[pointerLeft]);
                double distanceRight = Vector2.Distance(house, specialBuildings[pointerRight]);

                if (distanceLeft <= distanceRight)
                {
                    tempArray[tempPointer++] = specialBuildings[pointerLeft++];
                }
                else
                {
                    tempArray[tempPointer++] = specialBuildings[pointerRight++];
                }
            }

            while (pointerLeft <= middleIndex)
            {
                tempArray[tempPointer++] = specialBuildings[pointerLeft++];
            }
            while (pointerRight <= endIndex)
            {
                tempArray[tempPointer++] = specialBuildings[pointerRight++];
            }
            tempPointer = 0;
            pointerLeft = beginIndex;
            while (tempPointer < tempArray.Length && pointerLeft <= endIndex)
            {
                specialBuildings[pointerLeft++] = tempArray[tempPointer++];
            }
            
        }

        private static IEnumerable<IEnumerable<Vector2>> FindSpecialBuildingsWithinDistanceFromHouse(
      IEnumerable<Vector2> specialBuildings, 
      IEnumerable<Tuple<Vector2, float>> housesAndDistances)
    {
      return
          from h in housesAndDistances
          select
            from s in specialBuildings
            where Vector2.Distance(h.Item1, s) <= h.Item2
            select s;
    }

    private static IEnumerable<Tuple<Vector2, Vector2>> FindRoute(Vector2 startingBuilding, 
      Vector2 destinationBuilding, IEnumerable<Tuple<Vector2, Vector2>> roads)
    {
      var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
      List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
      var prevRoad = startingRoad;
      for (int i = 0; i < 30; i++)
      {
        prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, destinationBuilding)).First());
        fakeBestPath.Add(prevRoad);
      }
      return fakeBestPath;
    }

    private static IEnumerable<IEnumerable<Tuple<Vector2, Vector2>>> FindRoutesToAll(Vector2 startingBuilding, 
      IEnumerable<Vector2> destinationBuildings, IEnumerable<Tuple<Vector2, Vector2>> roads)
    {
            List<Vector2> destinationBuildingsList = destinationBuildings.ToList();
            List<Tuple<Vector2, Vector2>> roadsList = roads.ToList();

            float[,] distanceMatrix = new float[roadsList.Count,roadsList.Count];//array of minimum distances initialized to infinite
            //int[,] distanceMatrix = new int[roadsList.Count, roadsList.Count];//array of minimum distances initialized to infinite
            Tuple<Vector2, Vector2>[,] predecessorMatrix = new Tuple<Vector2, Vector2>[roadsList.Count,roadsList.Count];

            //initialize distance & predecessor matrix
            for (int i = 0; i < roadsList.Count; i++)
            {
                Tuple<Vector2, Vector2> roadItem = roadsList[i];

                for (int j = 0; j < roadsList.Count; j++)
                {                    
                    Tuple<Vector2,Vector2> roadItem2 = roadsList[j];

                    //add to predecessor matrix
                    //predecessorMatrix[i, j] = roadItem2;
                    predecessorMatrix[i, j] = null;
                    
                    //add infinite to distance matrix
                    distanceMatrix[i, j] = float.PositiveInfinity;
                    
                }
            }

            //first initialize distance & predecessor matrix
            for (int i = 0; i < roadsList.Count; i++)
            {
                Tuple<Vector2, Vector2> roadItem = roadsList[i];

                for (int j = 0; j < roadsList.Count; j++)
                {
                    Tuple<Vector2, Vector2> roadItem2 = roadsList[j];

                    //check if they are the same
                    if (roadItem == roadItem2)
                    {
                        distanceMatrix[i, j] = 0;
                        predecessorMatrix[i, j] = roadItem;
                    }
                    //check if they are connected
                    else if (roadItem.Item1 == roadItem2.Item1 || roadItem.Item2 == roadItem2.Item1)
                    {
                        distanceMatrix[i, j] = 1;
                        predecessorMatrix[i, j] = roadItem2;
                    }
                }
            }
            //update adjacency matrix & compute shortest path
            for (int k = 0; k < roadsList.Count; k++)
            {
                if (k > 20)
                {
                    break;
                }
                Console.WriteLine("k = " + k);
                for (int i = 0; i < roadsList.Count; i++)
                {                    
                    for (int j = 0; j < roadsList.Count; j++)
                    {
                        if (distanceMatrix[i, k] + distanceMatrix[k, j] < distanceMatrix[i, j] )
                        {
                            distanceMatrix[i, j] = distanceMatrix[i, k] + distanceMatrix[k, j];
                            predecessorMatrix[i, j] = roadsList[k];
                        }
                        
                    }
                }
            }
            //PrintPredecessorMatrix(predecessorMatrix);
            //PrintDistanceMatrix(distanceMatrix);

            List<List<Tuple<Vector2, Vector2>>> result = new List<List<Tuple<Vector2, Vector2>>>();
            foreach (var d in destinationBuildings)
            {
                var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
                List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
                var prevRoad = startingRoad;
                for (int i = 0; i < 500; i++)
                {
                    prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, d)).First());
                    fakeBestPath.Add(prevRoad);
                }
                result.Add(fakeBestPath);
            }
                return result;

        }

        private static void PrintPredecessorMatrix(Tuple<Vector2, Vector2>[,] predecessorMatrix)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\gertj\myPredecessorMatrix.txt");
            //Print distance matrix
            for (int j = 0; j < predecessorMatrix.GetLength(0); j++)
            {
                string array = "";
                Console.Write("[");
                for (int k = 0; k < predecessorMatrix.GetLength(1); k++)
                {
                    if (predecessorMatrix[j, k] == null)
                    {
                        array += "X";
                        Console.Write("X");
                    }
                    else
                    {
                        array += predecessorMatrix[j, k];
                        Console.Write(" " + predecessorMatrix[j, k]);
                    }
                }
                array += " ]";
                Console.Write(" ] \r\n");

                file.WriteLine(array);
                array = "";
            }
            Console.Write(" ] \r\n");
        }
    

        private static void PrintDistanceMatrix(float[,] a)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\gertj\myDistancematrix.txt");
            //Print distance matrix
            for (int j = 0; j < a.GetLength(0); j++)
            {
                string array = "";
                Console.Write("[");
                for (int k = 0; k < a.GetLength(1); k++)
                {
                    if (float.IsPositiveInfinity(a[j, k]))
                    {
                        array += "X";
                        Console.Write("X");
                    }
                    else
                    {
                        array += a[j, k];
                        Console.Write(" " + a[j, k]);
                    }
                }
                array += " ]";
                Console.Write(" ] \r\n");

                file.WriteLine(array);
                array = "";
            }
            Console.Write(" ] \r\n");
        }
    }
    
}
#endif

