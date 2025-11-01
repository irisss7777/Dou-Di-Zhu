using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public static class RandomDecksGenerator
{
    public static int[][] GetShuffledDeck()
    {
        List<int> values = new List<int>();
        for (int i = 0; i <= 53; i++)
        {
            values.Add(i);
        }
        int[][] arr = new int[4][];
        arr[0] = new int[17];
        arr[1] = new int[17];
        arr[2] = new int[17];
        arr[3] = new int[3];
        for (int i = 0; i < arr.Length; i++)
        {
            for (int j = 0; j < arr[i].Length; j++)
            {
                int position = UnityEngine.Random.Range(0, values.Count);
                arr[i][j] = values[position];
                values.RemoveAt(position);
            }
        }

        return arr;
    }

    public static int[][] GetShuffledDeckForBombMode()
    {
        // Фаза 0
        List<List<int>> subsets = GenerateSubsets();

        // Фаза 1
        List<List<int>> incompleteBombs;
        List<int> tableCards = GenerateTableCards(ref subsets, out incompleteBombs);

        string a = "Фаза 1. Карты на столе: ";
        foreach (int b in tableCards)
            a += b.ToString() + " ";
        a += ". Карты в колоде: ";
        foreach (List<int> s in subsets)
            foreach (int b in s)
                a += b.ToString() + " ";
        a += ". Карты в колоде неполных бомб: ";
        foreach (List<int> s in incompleteBombs)
            foreach (int b in s)
                a += b.ToString() + " ";
        Debug.Log(a);

        // Фаза 2
        int jackpotPlayer = UnityEngine.Random.Range(0.00f, 1.00f) < 0.10f ? UnityEngine.Random.Range(0, 3) : -1;

        List<int>[] players = new List<int>[3];
        for (int i = 0; i < 3; i++)
        {
            players[i] = new List<int>();
        }

        if (jackpotPlayer != -1)
        {
            foreach (List<int> subset in incompleteBombs)
            {
                foreach (int item in subset)
                {
                    players[jackpotPlayer].Add(item);
                }
            }
        }
        else
        {
            float shufflesRand = UnityEngine.Random.Range(0.00f, 1.00f);
            if (shufflesRand < 0.25f)
                incompleteBombs = ShuffleIncompleteSubsets(2, incompleteBombs);
            else if (shufflesRand < 0.50f)
                incompleteBombs = ShuffleIncompleteSubsets(4, incompleteBombs);
            else if (shufflesRand < 0.75f)
                incompleteBombs = ShuffleIncompleteSubsets(6, incompleteBombs);

            foreach (List<int> subset in incompleteBombs)
            {
                int pos = UnityEngine.Random.Range(0, 3);
                foreach (int item in subset)
                {
                    players[pos].Add(item);
                }
            }
        }

        a = "Фаза 2. Карты в колоде: ";
        foreach (List<int> s in subsets)
            foreach (int b in s)
                a += b.ToString() + " ";
        a += ". Карты игрок1: ";
        foreach (int b in players[0])
            a += b.ToString() + " ";
        a += ". Карты игрок2: ";
        foreach (int b in players[1])
            a += b.ToString() + " ";
        a += ". Карты игрок3: ";
        foreach (int b in players[2])
            a += b.ToString() + " ";
        Debug.Log(a);

        // Фаза 3
        if (jackpotPlayer != -1)
        {
            switch (incompleteBombs.Count)
            {
                case 1:
                    for (int i = 0; i < 4; i++)
                        AddConnectedSubsetToPlayer(jackpotPlayer, ref players, ref subsets);
                    break;

                case 2:
                    for (int i = 0; i < 3; i++)
                        AddConnectedSubsetToPlayer(jackpotPlayer, ref players, ref subsets);
                    break;

                case 3:
                    for (int i = 0; i < 2; i++)
                        AddConnectedSubsetToPlayer(jackpotPlayer, ref players, ref subsets);
                    break;
            }
        }

        for (int i = 0; i < 3; i++)
            if (i != jackpotPlayer)
            {
                float bombsCountRand = UnityEngine.Random.Range(0.00f, 1.00f);
                int bombsCount;
                if (bombsCountRand < 0.45f)
                    bombsCount = 2;
                else if (bombsCountRand < 0.80f)
                    bombsCount = 3;
                else
                    bombsCount = 4;

                for (int j = 0; j < bombsCount; j++)
                {
                    if (players[i].Count + 4 <= 17)
                    {
                        float connectedRand = UnityEngine.Random.Range(0.00f, 1.00f);
                        if (connectedRand < 0.75f)
                            AddConnectedSubsetToPlayer(i, ref players, ref subsets);
                        else
                            AddRandomSubsetToPlayer(i, ref players, ref subsets);
                    }
                }
            }

        a = "Фаза 3. Карты в колоде: ";
        foreach (List<int> s in subsets)
            foreach (int b in s)
                a += b.ToString() + " ";
        a += ". Карты игрок1: ";
        foreach (int b in players[0])
            a += b.ToString() + " ";
        a += ". Карты игрок2: ";
        foreach (int b in players[1])
            a += b.ToString() + " ";
        a += ". Карты игрок3: ";
        foreach (int b in players[2])
            a += b.ToString() + " ";
        Debug.Log(a);

        // Фаза 4
        subsets.Add(new List<int> { 52, 53 });
        Shuffle(subsets);

        for (int i = 0; i < 3; i++)
            while (players[i].Count < 17)
            {
                if (17 - players[i].Count >= 6)
                {
                    var cards = TryGetPlane(ref subsets);
                    if (cards != null)
                        foreach (int item in cards)
                            players[i].Add(item);
                }

                if (17 - players[i].Count >= 5)
                {
                    var cards = TryGetFullHouse(ref subsets);
                    if (cards != null)
                        foreach (int item in cards)
                            players[i].Add(item);
                }

                if (17 - players[i].Count >= 3)
                {
                    var cards = TryGetTriplet(ref subsets);
                    if (cards != null)
                        foreach (int item in cards)
                            players[i].Add(item);
                }

                if (17 - players[i].Count >= 2)
                {
                    var cards = TryGetPair(ref subsets);
                    if (cards != null)
                        foreach (int item in cards)
                            players[i].Add(item);
                }

                if (17 - players[i].Count >= 1)
                {
                    var cards = TryGetSingle(ref subsets);
                    if (cards != -1)
                        players[i].Add(cards);
                }
            }

        a = "Фаза 4. Карты в колоде: ";
        foreach (List<int> s in subsets)
            foreach (int b in s)
                a += b.ToString() + " ";
        a += ". Карты игрок1: ";
        foreach (int b in players[0])
            a += b.ToString() + " ";
        a += ". Карты игрок2: ";
        foreach (int b in players[1])
            a += b.ToString() + " ";
        a += ". Карты игрок3: ";
        foreach (int b in players[2])
            a += b.ToString() + " ";
        Debug.Log(a);

        // Завершение
        int[][] result = new int[4][];

        List<List<int>> playersList = new List<List<int>>() { players[0], players[1], players[2] };
        Shuffle(playersList);
        result[0] = playersList[0].ToArray();
        result[1] = playersList[1].ToArray();
        result[2] = playersList[2].ToArray();
        result[3] = tableCards.ToArray();

        return result;
    }

    #region helpers

    private static List<List<int>> GenerateSubsets()
    {
        List<List<int>> subsets = new List<List<int>>();
        for (int i = 0; i < 13; i++)
        {
            List<int> group = new List<int>();
            for (int j = 0; j < 4; j++)
            {
                group.Add(i + j * 13);
            }
            subsets.Add(group);
        }
        //subsets.Add(new List<int> { 52, 53 });
        Shuffle(subsets);
        return subsets;
    }

    private static List<int> GenerateTableCards(ref List<List<int>> subsetsArray, out List<List<int>> incompleteBombs)
    {
        List<int> tableCards = new List<int>();
        incompleteBombs = new List<List<int>>();

        float typeRand = UnityEngine.Random.Range(0.00f, 1.00f);


        if (typeRand < 0.4f)
        {
            TakeItemsFromSubsets(3, ref tableCards, ref subsetsArray, ref incompleteBombs);
        }
        else if (typeRand < 0.8f)
        {
            TakeItemsFromSubsets(2, ref tableCards, ref subsetsArray, ref incompleteBombs);
            TakeItemsFromSubsets(1, ref tableCards, ref subsetsArray, ref incompleteBombs);
        }
        else
        {
            TakeItemsFromSubsets(1, ref tableCards, ref subsetsArray, ref incompleteBombs);
            TakeItemsFromSubsets(1, ref tableCards, ref subsetsArray, ref incompleteBombs);
            TakeItemsFromSubsets(1, ref tableCards, ref subsetsArray, ref incompleteBombs);
        }

        return tableCards;
    }

    private static void TakeItemsFromSubsets(int itemsCount, ref List<int> tokeItems, ref List<List<int>> subsetsArray, ref List<List<int>> incompleteSubsets)
    {
        int randomIndex;
        do
        {
            randomIndex = UnityEngine.Random.Range(0, subsetsArray.Count);
        }
        while (subsetsArray[randomIndex].Count < itemsCount);

        for (int i = 0; i < itemsCount; i++)
        {
            tokeItems.Add(subsetsArray[randomIndex][0]);
            subsetsArray[randomIndex].Remove(subsetsArray[randomIndex][0]);
        }

        if (subsetsArray[randomIndex].Count > 0)
        {
            incompleteSubsets.Add(subsetsArray[randomIndex]);
        }

        subsetsArray.Remove(subsetsArray[randomIndex]);
    }

    private static void AddRandomSubsetToPlayer(int playerPos, ref List<int>[] players, ref List<List<int>> subsets)
    {
        int randPos = UnityEngine.Random.Range(0, subsets.Count);
        foreach (int item in subsets[randPos])
            players[playerPos].Add(item);
        subsets.Remove(subsets[randPos]);
    }

    private static void AddConnectedSubsetToPlayer(int playerPos, ref List<int>[] players, ref List<List<int>> subsets)
    {
        List<int> shuffledPlayerCards = players[playerPos];
        Shuffle(shuffledPlayerCards);

        foreach (int item in shuffledPlayerCards)
        {
            List<int> connectedSubset = GetConnectedSubset(item, subsets);
            if (connectedSubset != null)
            {
                foreach (int el in connectedSubset)
                {
                    players[playerPos].Add(el);
                }
                subsets.Remove(connectedSubset);
                return;
            }
        }
        AddRandomSubsetToPlayer(playerPos, ref players, ref subsets);
    }

    private static List<int> GetConnectedSubset(int item, List<List<int>> subsets)
    {
        foreach (List<int> subset in subsets)
            foreach (int subsetItem in subset)
                if (subsetItem + 1 == item || subsetItem - 1 == item)
                {
                    return subset;
                }
        return null;
    }

    private static List<List<int>> ShuffleIncompleteSubsets(int times, List<List<int>> subsets)
    {
        List<int> unitedList = new List<int>();
        foreach (List<int> subset in subsets)
            foreach (int item in subset)
                unitedList.Add(item);
        while (times > 0)
        {
            times--;
            int element1 = UnityEngine.Random.Range(0, unitedList.Count);
            int element2 = UnityEngine.Random.Range(0, unitedList.Count);
            int value = unitedList[element1];
            unitedList[element1] = unitedList[element2];
            unitedList[element2] = value;
        }
        for (int i = 0; i < subsets.Count; i++)
            for (int j = 0; j < subsets[i].Count; j++)
            {
                subsets[i][j] = unitedList[0];
                unitedList.Remove(unitedList[0]);
            }
        return subsets;
    }

    #endregion

    #region getting_combos_methods

    private static List<int> TryGetPlane(ref List<List<int>> subsets)
    {
        for (int i = 0; i < subsets.Count - 1; i++)
        {
            if (subsets[i].Count >= 3)
            {
                for (int j = i + 1; j < subsets.Count; j++)
                {
                    if (subsets[j].Count >= 3 && Math.Abs(subsets[i][0] - subsets[j][0]) == 4)
                    {
                        List<int> plane = new List<int>() { subsets[i][0], subsets[i][1], subsets[i][2], subsets[j][0], subsets[j][1], subsets[j][2] };
                        foreach (int item in plane)
                        {
                            subsets[i].Remove(item);
                            subsets[j].Remove(item);
                        }
                        if (subsets[i].Count == 0)
                            subsets.Remove(subsets[i]);
                        if (subsets[j].Count == 0)
                            subsets.Remove(subsets[j]);
                        return plane;
                    }
                }
            }
        }
        return null;
    }

    private static List<int> TryGetFullHouse(ref List<List<int>> subsets)
    {
        for (int i = 0; i < subsets.Count; i++)
        {
            if (subsets[i].Count >= 3)
            {
                for (int j = 0; j < subsets.Count; j++)
                {
                    if (j != i && subsets[j].Count >= 2)
                    {
                        List<int> fullHouse = new List<int>() { subsets[i][0], subsets[i][1], subsets[i][2], subsets[j][0], subsets[j][1] };
                        foreach (int item in fullHouse)
                        {
                            subsets[i].Remove(item);
                            subsets[j].Remove(item);
                        }
                        if (subsets.Count > i && subsets[i].Count == 0)
                            subsets.Remove(subsets[i]);
                        if (subsets.Count > j && subsets[j].Count == 0)
                            subsets.Remove(subsets[j]);
                        return fullHouse;
                    }
                }
            }
        }
        return null;
    }

    private static List<int> TryGetTriplet(ref List<List<int>> subsets)
    {
        for (int i = 0; i < subsets.Count; i++)
        {
            if (subsets[i].Count >= 3)
            {
                List<int> triplet = new List<int>() { subsets[i][0], subsets[i][1], subsets[i][2] };
                foreach (int item in triplet)
                {
                    subsets[i].Remove(item);
                }
                if (subsets[i].Count == 0)
                    subsets.Remove(subsets[i]);
                return triplet;
            }
        }
        return null;
    }

    private static List<int> TryGetPair(ref List<List<int>> subsets)
    {
        for (int i = 0; i < subsets.Count; i++)
        {
            if (subsets[i].Count >= 2)
            {
                List<int> pair = new List<int>() { subsets[i][0], subsets[i][1] };
                foreach (int item in pair)
                {
                    subsets[i].Remove(item);
                }
                if (subsets[i].Count == 0)
                    subsets.Remove(subsets[i]);
                return pair;
            }
        }
        return null;
    }

    private static int TryGetSingle(ref List<List<int>> subsets)
    {
        for (int i = 0; i < subsets.Count; i++)
        {
            if (subsets[i].Count >= 1)
            {
                int single = subsets[i][0];
                subsets[i].Remove(single);
                if (subsets[i].Count == 0)
                    subsets.Remove(subsets[i]);
                return single;
            }
        }
        return -1;
    }

    #endregion

    private static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void LogArrays(int[][] arr)
    {
        Debug.Log("________________________________________________");
        Debug.Log("Array 0:");
        foreach (int i in arr[0])
            Debug.Log(i);
        Debug.Log("________________________________________________");
        Debug.Log("Array 1:");
        foreach (int i in arr[1])
            Debug.Log(i);
        Debug.Log("________________________________________________");
        Debug.Log("Array 2:");
        foreach (int i in arr[2])
            Debug.Log(i);
        Debug.Log("________________________________________________");
        Debug.Log("Array 3:");
        foreach (int i in arr[3])
            Debug.Log(i);
    }
}
