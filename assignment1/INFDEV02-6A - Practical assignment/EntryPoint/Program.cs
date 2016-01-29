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
            int length = 0;
            foreach (Vector2 vector in specialBuildings)
            {
                length++;
                specialBuildingList.Add(vector);
            }
            //TODO MERGE SORT
      MergeSort(specialBuildingList, house, 0, length -1);

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

        private static void Merge(List<Vector2> specialBuildings, Vector2 house, int leftIndex, int middleIndex, int rightIndex)
        {
            int lengthLeft = middleIndex - leftIndex +1;
            int lengthRight = rightIndex - middleIndex;
            Vector2[] leftArray = new Vector2[lengthLeft +1]; //+1
            Vector2[] rightArray = new Vector2[lengthRight +1]; //+1

            //fill leftArray & rightArray with indexes from the unsortedArray
            for (int i = 0; i < lengthLeft; i++)
            {
                leftArray[i] = specialBuildingList[leftIndex + i];
            }
            for (int j = 0; j < lengthRight; j++)
            {
                rightArray[j] = specialBuildingList[middleIndex + j + 1];
            }

           
            //Add maxvalue add end of left and right array
            leftArray[lengthLeft] = new Vector2(Int32.MaxValue);
            rightArray[lengthRight] = new Vector2(Int32.MaxValue);

            int iIndex = 0; //left
            int jIndex = 0; //right

            //Comparison left and right array & rearrange
            for (int k = leftIndex; k <= rightIndex ; k++)
            {
                double distanceLeft =  Math.Sqrt(Math.Pow((house.X) - (leftArray[iIndex].X), 2) + Math.Pow((house.Y) - leftArray[iIndex].Y, 2));
                double distanceRight = Math.Sqrt(Math.Pow((house.X) - (rightArray[jIndex].X), 2) + Math.Pow((house.Y) - rightArray[jIndex].Y, 2));
                if (distanceLeft <= distanceRight)
                {
                    specialBuildingList[k] = leftArray[iIndex];
                    iIndex++;
                }
                else
                {
                    specialBuildingList[k] = rightArray[jIndex];
                    jIndex++;
                }
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
      List<List<Tuple<Vector2, Vector2>>> result = new List<List<Tuple<Vector2, Vector2>>>();
      foreach (var d in destinationBuildings)
      {
        var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
        List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
        var prevRoad = startingRoad;
        for (int i = 0; i < 30; i++)
        {
          prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, d)).First());
          fakeBestPath.Add(prevRoad);
        }
        result.Add(fakeBestPath);
      }
      return result;
    }
  }
#endif
}
