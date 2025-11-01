using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ComboValidator
{
    public static ComboType ValidateCombo(Card[] cards)
    {
        if (cards == null || cards.Length == 0)
            return ComboType.Invalid;

        cards = cards.OrderBy(c => c.value).ToArray();
        Dictionary<int, int> countMap = cards.GroupBy(c => c.value)
                                             .ToDictionary(g => g.Key, g => g.Count());

        int distinctCount = countMap.Count;
        int maxCount = countMap.Values.Max();

        switch (cards.Length)
        {
            case 1:
                return ComboType.Single;
            case 2:
                if (countMap.Count == 1)
                    return ComboType.Pair;
                if (IsRocket(cards))
                    return ComboType.Rocket;
                break;
            case 3:
                if (maxCount == 3)
                    return ComboType.Triple;
                break;
            case 4:
                if (maxCount == 4)
                    return ComboType.Bomb;
                if (maxCount == 3 && distinctCount == 2)
                    return ComboType.TripleWithSingle;
                break;
            case 5:
                if (maxCount == 3 && distinctCount == 2 && countMap.Values.Contains(2))
                    return ComboType.TripleWithPair;
                if (IsStraight(cards))
                    return ComboType.Straight;
                break;
            case 6:
                if (IsStraight(cards))
                    return ComboType.Straight;
                if (IsDoubleStraight(cards))
                    return ComboType.DoubleStraight;
                if (IsPlane(cards))
                    return ComboType.Plane;
                if (maxCount == 4 && distinctCount == 2)
                    return ComboType.FourWithTwo;
                break;
            default:
                if (IsStraight(cards))
                    return ComboType.Straight;
                if (IsDoubleStraight(cards))
                    return ComboType.DoubleStraight;
                if (IsPlane(cards))
                    return ComboType.Plane;
                if (IsPlaneWithSingle(cards))
                    return ComboType.PlaneWithSingle;
                if (IsPlaneWithPair(cards))
                    return ComboType.PlaneWithPair;
                if (maxCount == 4 && (cards.Length == 8) && countMap.Values.Count(v => v == 2) == 2)
                    return ComboType.FourWithTwoPairs;
                if (IsMultipleBombs(cards))
                    return ComboType.MultipleBombs;
                break;
        }
        return ComboType.Invalid;
    }

    public static bool IsStrongerCombo(Card[] newCards, Card[] previousCards)
    {
        ComboType newCombo = ValidateCombo(newCards);
        ComboType previousCombo = ValidateCombo(previousCards);

        if (newCombo == ComboType.Invalid)
            return false;
        if (previousCombo == ComboType.Invalid)
            return true;
        if (newCombo == ComboType.Rocket)
            return true;
        if (previousCombo == ComboType.Rocket)
            return false;
        if (newCombo == ComboType.MultipleBombs && previousCombo != ComboType.MultipleBombs)
            return true;
        if (newCombo == ComboType.Bomb && previousCombo != ComboType.Bomb && previousCombo != ComboType.MultipleBombs)
            return true;

        //if (newCombo == previousCombo && newCards.Length == previousCards.Length)
        //    return newCards.Max(c => c.value) > previousCards.Max(c => c.value);

        if (newCombo == previousCombo && newCards.Length == previousCards.Length)
        {
            switch (newCombo)
            {
                case ComboType.Plane:
                case ComboType.PlaneWithSingle:
                case ComboType.PlaneWithPair:
                    int newMaxTriple = GetHighestTripleValue(newCards);
                    int prevMaxTriple = GetHighestTripleValue(previousCards);
                    return newMaxTriple > prevMaxTriple;

                case ComboType.TripleWithSingle:
                case ComboType.TripleWithPair:
                    int newTriple = GetTripleValue(newCards);
                    int prevTriple = GetTripleValue(previousCards);
                    return newTriple > prevTriple;

                case ComboType.FourWithTwo:
                case ComboType.FourWithTwoPairs:
                    int newFour = GetFourValue(newCards);
                    int prevFour = GetFourValue(previousCards);
                    return newFour > prevFour;

                case ComboType.MultipleBombs:
                    if (newCards.Length > previousCards.Length)
                        return true;
                    if (newCards.Length < previousCards.Length)
                        return false;
                    int newFour1 = GetHighestFourValue(newCards);
                    int prevFour1 = GetHighestFourValue(previousCards);
                    return newFour1 > prevFour1;

                default:
                    return newCards.Max(c => c.value) > previousCards.Max(c => c.value);
            }
        }

        return false;
    }

    public static Card[] FindStrongerCombo(Card[] newCards, Card[] previousCards) //среди массива ищет, есть ли комбинация, которая сможет победить предыдущую выложенную
    {
        Array.Reverse(newCards);
        int maxSubsetsSize = previousCards.Length > 4 ? previousCards.Length : 4;
        var allSubsets = GetAllSubsets(newCards, maxSubsetsSize);
        foreach (var subset in allSubsets)
        {
            var arrayToCheck = subset.ToArray();
            if (IsStrongerCombo(arrayToCheck, previousCards))
            {
                return arrayToCheck;
            }
        }
        return null;
    }

    public static Card[] FindRandomCombo(Card[] newCards)
    {
        bool comboFound = false;
        int type = UnityEngine.Random.Range(0, 9);
        var allSubsets = GetAllSubsets(newCards, 6);
        while (!comboFound)
        {
            switch (type)
            {
                case 8:
                    foreach (var subset in allSubsets)
                    {
                        var arrayToCheck = subset.ToArray();
                        if (ValidateCombo(arrayToCheck) == ComboType.Rocket)
                        {
                            comboFound = true;
                            return arrayToCheck;
                        }
                    }
                    break;
                case 7:
                    foreach (var subset in allSubsets)
                    {
                        var arrayToCheck = subset.ToArray();
                        if (ValidateCombo(arrayToCheck) == ComboType.Bomb)
                        {
                            comboFound = true;
                            return arrayToCheck;
                        }
                    }
                    break;
                case 6:
                    foreach (var subset in allSubsets)
                    {
                        var arrayToCheck = subset.ToArray();
                        if (ValidateCombo(arrayToCheck) == ComboType.Plane)
                        {
                            comboFound = true;
                            return arrayToCheck;
                        }
                    }
                    break;
                case 5:
                    foreach (var subset in allSubsets)
                    {
                        var arrayToCheck = subset.ToArray();
                        if (ValidateCombo(arrayToCheck) == ComboType.Straight)
                        {
                            comboFound = true;
                            return arrayToCheck;
                        }
                    }
                    break;
                case 4:
                    foreach (var subset in allSubsets)
                    {
                        var arrayToCheck = subset.ToArray();
                        if (ValidateCombo(arrayToCheck) == ComboType.TripleWithPair)
                        {
                            comboFound = true;
                            return arrayToCheck;
                        }
                    }
                    break;
                case 3:
                    foreach (var subset in allSubsets)
                    {
                        var arrayToCheck = subset.ToArray();
                        if (ValidateCombo(arrayToCheck) == ComboType.TripleWithSingle)
                        {
                            comboFound = true;
                            return arrayToCheck;
                        }
                    }
                    break;
                case 2:
                    foreach (var subset in allSubsets)
                    {
                        var arrayToCheck = subset.ToArray();
                        if (ValidateCombo(arrayToCheck) == ComboType.FourWithTwo)
                        {
                            comboFound = true;
                            return arrayToCheck;
                        }
                    }
                    break;
                case 1:
                    foreach (var subset in allSubsets)
                    {
                        var arrayToCheck = subset.ToArray();
                        if (ValidateCombo(arrayToCheck) == ComboType.Pair)
                        {
                            comboFound = true;
                            return arrayToCheck;
                        }
                    }
                    break;
                case 0:
                    foreach (var subset in allSubsets)
                    {
                        var arrayToCheck = subset.ToArray();
                        if (ValidateCombo(arrayToCheck) == ComboType.Single)
                        {
                            comboFound = true;
                            return arrayToCheck;
                        }
                    }
                    break;
            }
            if (type > 0)
                type--;
            else
                return null;
        }
        return null;
    }

    //private static List<List<Card>> GetAllSubsets(Card[] cards, int maxSubsetSize)
    //{
    //    List<List<Card>> subsets = new List<List<Card>>();
    //    int subsetCount = 1 << cards.Length;
    //    Debug.Log("SUBSETS = " + subsetCount);
    //
    //    for (int i = 1; i < subsetCount; i++)
    //    {
    //        List<Card> subset = new List<Card>();
    //        int count = 0; // Счётчик элементов в подмножестве
    //
    //        for (int j = 0; j < cards.Length; j++)
    //        {
    //            if ((i & (1 << j)) != 0)
    //            {
    //                subset.Add(cards[j]);
    //                count++;
    //
    //                // Если количество элементов в подмножестве превышает максимальное, выходим
    //                if (count > maxSubsetSize)
    //                {
    //                    break;
    //                }
    //            }
    //        }
    //
    //        // Если подмножество не превысило максимальный размер, добавляем его в список
    //        if (count <= maxSubsetSize && ValidateCombo(subset.ToArray()) != ComboType.Invalid)
    //        {
    //            subsets.Add(subset);
    //        }
    //    }
    //
    //    return subsets;
    //}






    private static List<List<Card>> GetAllSubsets(Card[] cards, int maxSubsetSize)
    {
        List<List<Card>> result = new List<List<Card>>();
        List<Card> jokers = new List<Card>();
        List<Card> normalCards = new List<Card>();
        foreach (Card card in cards)
        {
            if (card.suit == Suits.jokers)
                jokers.Add(card);
            else
                normalCards.Add(card);
        }

        Dictionary<Rank, List<Card>> byRank = normalCards
            .GroupBy(c => (Rank)c.numberInSuit)
            .ToDictionary(g => g.Key, g => g.ToList());

        Dictionary<Suits, List<Card>> bySuit = normalCards
            .GroupBy(c => c.suit)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Генерация одиночных карт (Single)
        foreach (Card card in cards)
        {
            result.Add(new List<Card> { card });
        }

        // Rocket (2 джокера)
        if (jokers.Count >= 2)
        {
            foreach (var combo in GetCombinations(jokers, 2))
            {
                if (combo.Count == 2) result.Add(combo);
            }
        }

        GenerateSetCombinations(byRank, result);
        GenerateTripleCombinations(byRank, jokers, result);
        GenerateStraightCombinations(normalCards, result);
        GenerateBombCombinations(byRank, jokers, result);

        GenerateSequentialCombinations(byRank, 3, 2, result);
        GenerateSequentialCombinations(byRank, 2, 3, result);
        GenerateSequentialCombinations(byRank, 2, 4, result);

        List<List<Card>> allPlanes = new List<List<Card>>();
        GenerateSequentialCombinations(byRank, 2, 3, allPlanes);
        List<Card> allNormalCards = normalCards.ToList();
        GeneratePlaneWithAttachments(allPlanes, allNormalCards, 1, false, byRank, result);
        GeneratePlaneWithAttachments(allPlanes, allNormalCards, 2, true, byRank, result);

        var res = result.Distinct(new CardSetComparer()).ToList();
        //foreach (List<Card> a in res)
        //{
        //    string b = "Subset: ";
        //    foreach (Card c in a)
        //    {
        //        b += c.value + " ";
        //    }
        //    Debug.Log(b);
        //}
//
        // Удаление дубликатов
        return res;
    }


    private static void GenerateSetCombinations(Dictionary<Rank, List<Card>> byRank, List<List<Card>> result)
    {
        foreach (var kvp in byRank)
        {
            List<Card> cards = kvp.Value;
            int count = cards.Count;

            // Пары (2 карты)
            if (count >= 2)
            {
                foreach (var combo in GetCombinations(cards, 2))
                {
                    result.Add(combo);
                }
            }

            // Тройки (3 карты)
            if (count >= 3)
            {
                foreach (var combo in GetCombinations(cards, 3))
                {
                    result.Add(combo);
                }
            }

            // Бомбы (4 карты)
            if (count >= 4)
            {
                foreach (var combo in GetCombinations(cards, 4))
                {
                    result.Add(combo);
                }
            }
        }
    }

    private static void GenerateTripleCombinations(Dictionary<Rank, List<Card>> byRank, List<Card> jokers, List<List<Card>> result)
    {
        // TripleWithSingle: тройка + 1 карта (обычная или джокер)
        foreach (var triple in GetAllTriples(byRank))
        {
            List<Card> available = new List<Card>();
            available.AddRange(jokers);

            // Добавляем все обычные карты кроме текущей тройки
            foreach (var group in byRank)
            {
                if ((int)group.Key == triple[0].numberInSuit)
                    available.AddRange(group.Value.Except(triple));
                else
                    available.AddRange(group.Value);
            }

            foreach (Card card in available)
            {
                List<Card> combo = new List<Card>(triple);
                combo.Add(card);
                result.Add(combo);
            }
        }

        // TripleWithPair: тройка + пара (только обычные карты)
        List<List<Card>> allPairs = GetAllPairs(byRank);
        foreach (var triple in GetAllTriples(byRank))
        {
            foreach (var pair in allPairs)
            {
                // Проверка, что пара не использует карты из тройки
                if (pair.Intersect(triple).Any()) continue;

                List<Card> combo = new List<Card>(triple);
                combo.AddRange(pair);
                result.Add(combo);
            }
        }
    }

    private static void GenerateStraightCombinations(List<Card> normalCards, List<List<Card>> result)
    {
        // Получаем уникальные ранги (исключая 2)
        List<Rank> allRanks = normalCards
            .Select(c => (Rank)c.numberInSuit)
            .Distinct()
            .Where(r => r != Rank.Two)
            .OrderBy(r => r)
            .ToList();

        // Находим все последовательности длиной 5+
        for (int start = 0; start < allRanks.Count; start++)
        {
            for (int length = 5; length <= allRanks.Count - start; length++)
            {
                List<Rank> sequence = allRanks.Skip(start).Take(length).ToList();
                if (IsConsecutive(sequence))
                {
                    // Генерируем все возможные комбинации для последовательности
                    var cardsInSequence = normalCards
                        .Where(c => sequence.Contains((Rank)c.numberInSuit))
                        .ToList();

                    foreach (var combo in GenerateCombinationsForSequence(cardsInSequence, sequence))
                    {
                        result.Add(combo);
                    }
                }
            }
        }
    }

    private static void GenerateBombCombinations(Dictionary<Rank, List<Card>> byRank, List<Card> jokers, List<List<Card>> result)
    {
        // FourWithTwo: бомба + 2 карты (обычные или джокеры)
        foreach (var bomb in GetAllBombs(byRank))
        {
            List<Card> available = new List<Card>();
            available.AddRange(jokers);

            // Добавляем все карты кроме бомбы
            foreach (var group in byRank)
            {
                if ((int)group.Key == bomb[0].numberInSuit)
                    available.AddRange(group.Value.Except(bomb));
                else
                    available.AddRange(group.Value);
            }

            foreach (var combo in GetCombinations(available, 2))
            {
                List<Card> fullCombo = new List<Card>(bomb);
                fullCombo.AddRange(combo);
                result.Add(fullCombo);
            }
        }
    }

    // Вспомогательные методы
    private static IEnumerable<List<Card>> GetAllTriples(Dictionary<Rank, List<Card>> byRank)
    {
        foreach (var kvp in byRank.Where(g => g.Value.Count >= 3))
        {
            foreach (var combo in GetCombinations(kvp.Value, 3))
            {
                yield return combo;
            }
        }
    }

    private static List<List<Card>> GetAllPairs(Dictionary<Rank, List<Card>> byRank)
    {
        List<List<Card>> pairs = new List<List<Card>>();
        foreach (var kvp in byRank.Where(g => g.Value.Count >= 2))
        {
            pairs.AddRange(GetCombinations(kvp.Value, 2));
        }
        return pairs;
    }

    private static IEnumerable<List<Card>> GetAllBombs(Dictionary<Rank, List<Card>> byRank)
    {
        foreach (var kvp in byRank.Where(g => g.Value.Count >= 4))
        {
            foreach (var combo in GetCombinations(kvp.Value, 4))
            {
                yield return combo;
            }
        }
    }

    private static bool IsConsecutive(List<Rank> ranks)
    {
        for (int i = 1; i < ranks.Count; i++)
        {
            if (ranks[i] != ranks[i - 1] + 1)
                return false;
        }
        return true;
    }

    private static List<List<Card>> GenerateCombinationsForSequence(List<Card> cards, List<Rank> sequence)
    {
        List<List<Card>> result = new List<List<Card>>();
        GenerateSequenceCombos(cards, sequence, 0, new List<Card>(), result);
        return result;
    }

    private static void GenerateSequenceCombos(List<Card> cards, List<Rank> sequence, int depth, List<Card> current, List<List<Card>> result)
    {
        if (depth == sequence.Count)
        {
            result.Add(new List<Card>(current));
            return;
        }

        Rank target = sequence[depth];
        foreach (Card card in cards.Where(c => (Rank)c.numberInSuit == target))
        {
            if (!current.Contains(card))
            {
                current.Add(card);
                GenerateSequenceCombos(cards, sequence, depth + 1, current, result);
                current.RemoveAt(current.Count - 1);
            }
        }
    }

    private static void GenerateSequentialCombinations(Dictionary<Rank, List<Card>> byRank, int minSequenceLength, int setSize, List<List<Card>> result)
    {
        var validRanks = byRank
            .Where(kvp => kvp.Key != Rank.Two && kvp.Value.Count >= setSize)
            .OrderBy(kvp => kvp.Key)
            .ToList();

        if (validRanks.Count < minSequenceLength) return;

        for (int start = 0; start <= validRanks.Count - minSequenceLength; start++)
        {
            int length = 1;
            for (int i = start; i < validRanks.Count - 1; i++)
            {
                if (validRanks[i + 1].Key == validRanks[i].Key + 1)
                    length++;
                else
                    break;
            }

            for (int len = minSequenceLength; len <= length; len++)
            {
                var sequence = validRanks
                    .Skip(start)
                    .Take(len)
                    .Select(kvp => kvp.Key)
                    .ToList();

                GenerateSequenceCombosRecursive(
                    byRank,
                    sequence,
                    setSize,
                    0,
                    new List<List<Card>>(),
                    result);
            }
        }
    }

    private static void GenerateSequenceCombosRecursive(Dictionary<Rank, List<Card>> byRank, List<Rank> sequence, int setSize, int depth, List<List<Card>> currentSets, List<List<Card>> result)
    {
        if (depth == sequence.Count)
        {
            result.Add(currentSets.SelectMany(s => s).ToList());
            return;
        }

        Rank rank = sequence[depth];
        foreach (var set in GetCombinations(byRank[rank], setSize))
        {
            if (currentSets.Any(s => s.Intersect(set).Any())) continue;

            currentSets.Add(set);
            GenerateSequenceCombosRecursive(byRank, sequence, setSize, depth + 1, currentSets, result);
            currentSets.RemoveAt(currentSets.Count - 1);
        }
    }

    private static void GeneratePlaneWithAttachments(List<List<Card>> planes, List<Card> allCards, int attachmentCountPerPlane, bool usePairs, Dictionary<Rank, List<Card>> byRank, List<List<Card>> result)
    {
        foreach (var plane in planes)
        {
            int planeCount = plane.Count / 3;
            int totalAttachments = planeCount * attachmentCountPerPlane;
            var attachments = new List<List<Card>>();

            if (usePairs)
            {
                attachments = byRank.Values
                    .SelectMany(cards => GetCombinations(cards, 2))
                    .Where(p => !p.Intersect(plane).Any())
                    .ToList();
            }
            else
            {
                attachments = allCards
                    .Except(plane)
                    .Select(card => new List<Card> { card })
                    .ToList();
            }

            foreach (var combo in GetCombinations(attachments, planeCount))
            {
                if (combo.SelectMany(c => c).Distinct().Count() != totalAttachments) continue;

                var fullCombo = new List<Card>(plane);
                fullCombo.AddRange(combo.SelectMany(c => c));
                result.Add(fullCombo);
            }
        }
    }

    public static IEnumerable<List<T>> GetCombinations<T>(List<T> list, int k)
    {
        if (k < 0 || k > list.Count) yield break;
        if (k == 0)
        {
            yield return new List<T>();
            yield break;
        }

        for (int i = 0; i < list.Count; i++)
        {
            T element = list[i];
            foreach (var combo in GetCombinations(list.Skip(i + 1).ToList(), k - 1))
            {
                combo.Insert(0, element);
                yield return combo;
            }
        }
    }

    public class CardSetComparer : IEqualityComparer<List<Card>>
    {
        public bool Equals(List<Card> x, List<Card> y)
        {
            return x.Count == y.Count &&
                new HashSet<Card>(x).SetEquals(y);
        }

        public int GetHashCode(List<Card> obj)
        {
            return obj.Aggregate(0, (hash, card) =>
                hash ^ card.GetHashCode());
        }
    }










    private static bool IsStraight(Card[] cards)
    {
        if (cards.Length < 5 || cards.Any(c => c.value > 14)) return false;
        return cards.Select(c => c.value).Distinct().SequenceEqual(Enumerable.Range(cards[0].value, cards.Length));
    }

    private static bool IsDoubleStraight(Card[] cards)
    {
        if (cards.Length < 6 || cards.Length % 2 != 0) return false;
        var grouped = cards.GroupBy(c => c.value).OrderBy(g => g.Key).ToArray();
        return grouped.All(g => g.Count() == 2) && grouped.Select(g => g.Key).SequenceEqual(Enumerable.Range(grouped[0].Key, grouped.Length));
    }

    private static bool IsPlane(Card[] cards)
    {
        var groups = cards.GroupBy(c => c.value).Where(g => g.Count() == 3).Select(g => g.Key).OrderBy(v => v).ToList();
        return groups.Count >= 2 && groups.SequenceEqual(Enumerable.Range(groups[0], groups.Count));
    }

    private static bool IsPlaneWithSingle(Card[] cards)
    {
        var groups = cards.GroupBy(c => c.value)
                         .ToDictionary(g => g.Key, g => g.Count());
        var triples = groups.Where(kv => kv.Value == 3)
                            .Select(kv => kv.Key)
                            .OrderBy(v => v)
                            .ToList();
        int tripleCount = triples.Count;

        if (tripleCount < 2 ||
            tripleCount * 4 != cards.Length ||
            !IsConsecutive(triples) ||
            groups.Values.Any(count => count != 1 && count != 3))
        {
            return false;
        }

        int singleCount = groups.Count(g => g.Value == 1);
        return singleCount == tripleCount;
    }

    private static bool IsPlaneWithPair(Card[] cards)
    {
        var groups = cards.GroupBy(c => c.value)
                         .ToDictionary(g => g.Key, g => g.Count());
        var triples = groups.Where(kv => kv.Value == 3)
                            .Select(kv => kv.Key)
                            .OrderBy(v => v)
                            .ToList();
        int tripleCount = triples.Count;
        if (tripleCount < 2 ||
            tripleCount * 5 != cards.Length ||
            !IsConsecutive(triples) ||
            groups.Values.Any(count => count != 2 && count != 3))
        {
            return false;
        }

        int pairCount = groups.Count(g => g.Value == 2);
        return pairCount == tripleCount;
    }

    private static bool IsMultipleBombs(Card[] cards)
    {
        if (cards.Length < 8 || cards.Length % 4 != 0) return false;
        var grouped = cards.GroupBy(c => c.value).OrderBy(g => g.Key).ToArray();
        return grouped.All(g => g.Count() == 4) && grouped.Select(g => g.Key).SequenceEqual(Enumerable.Range(grouped[0].Key, grouped.Length));
    }

    private static bool IsRocket(Card[] cards)
    {
        return cards.Length == 2 && cards.All(c => c.suit == Suits.jokers);
    }

    private static int GetTripleValue(Card[] cards)
    {
        return cards.GroupBy(c => c.value)
                   .First(g => g.Count() == 3).Key;
    }

    private static int GetFourValue(Card[] cards)
    {
        return cards.GroupBy(c => c.value)
                   .First(g => g.Count() == 4).Key;
    }

    private static int GetHighestFourValue(Card[] cards)
    {
        return cards.GroupBy(c => c.value)
                   .Where(g => g.Count() == 4)
                   .Max(g => g.Key);
    }

    private static int GetHighestTripleValue(Card[] cards)
    {
        return cards.GroupBy(c => c.value)
                   .Where(g => g.Count() == 3)
                   .Max(g => g.Key);
    }

    private static bool IsConsecutive(List<int> values)
    {
        for (int i = 1; i < values.Count; i++)
        {
            if (values[i] != values[i - 1] + 1)
                return false;
        }
        return true;
    }
}

public enum ComboType
{
    Invalid,
    Single,
    Pair,
    Triple,
    TripleWithSingle,
    TripleWithPair,
    Straight,
    DoubleStraight,
    Plane,
    PlaneWithSingle,
    PlaneWithPair,
    Bomb,
    MultipleBombs,
    FourWithTwo,
    FourWithTwoPairs,
    Rocket
}

public enum Rank
{
    Three = 0,
    Four = 1,
    Five = 2,
    Six = 3,
    Seven = 4,
    Eight = 5,
    Nine = 6,
    Ten = 7,
    Jack = 8,
    Queen = 9,
    King = 10,
    Ace = 11,
    Two = 12
}