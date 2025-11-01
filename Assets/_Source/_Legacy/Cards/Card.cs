using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using System.Linq;

public class Card : MonoBehaviour
{
    public int numberInDeck; // номер карты в колоде, растасованной по порядку по мастям 
    public int numberInSuit; // номер карты в массиве карт своей масти (3-0, 4-1, 5-2, 6-3, 7-4, 8-5, 9-6, 10-7, J-8, Q-9, K-10, A-11, 2-12), если масть джокеров, то BlackJoker-0, RedJoker-1
    public Suits suit; // clubs, diamonds, hearts, spades, jokers 
    public int value; //значение, написанное на самой карте (A-1, 2-2, 3-3, 4-4, 5-5, 6-6, 7-7, 8-8, 9-9, 10-10, J-11, Q-12, K-13), если масть джокеров, то BlackJoker-1, RedJoker-2

    public bool IsIconActive { get; private set; } = false;

    public CardView cardView;

    private Coroutine moveCoroutine;

    void Start()
    {
        if (!cardView)
            cardView = GetComponentInChildren<CardView>();
    }

    public void StartMoveCoroutine(MonoBehaviour owner, Vector3 target, float duration)
    {
        if (moveCoroutine != null)
        {
            owner.StopCoroutine(moveCoroutine);
        }
        moveCoroutine = owner.StartCoroutine(MoveSmoothly(target, duration));
    }

    private IEnumerator MoveSmoothly(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.position = target;
    }

    public void AssignView(CardView view)
    {
        if (!cardView)
            cardView = GetComponentInChildren<CardView>();
        for (int i = 0; i < cardView.valueElements.Length; i++)
        {
            cardView.valueElements[i].sprite = null;
        }
        cardView.mainTexture.sprite = view.mainTexture.sprite;
        for (int i = 0; i < view.valueElements.Length; i++)
        {
            cardView.valueElements[i].sprite = view.valueElements[i].sprite;
            cardView.valueElements[i].color = view.valueElements[i].color;
            cardView.valueElements[i].gameObject.transform.localScale = view.valueElements[i].transform.localScale;
            cardView.valueElements[i].gameObject.transform.localPosition = view.valueElements[i].transform.localPosition;
        }
    }

    public void SetBombIcon(bool type)
    {

        cardView.bombIcon.gameObject.SetActive(type);
        IsIconActive = type;
    }

    public void SetBombSelectffect(bool type)
    {
        cardView.bombSelectionParticle.SetActive(type);
    }

    public void SetViewLayer(int position)
    {
        if (!cardView)
        {
            cardView = GetComponentInChildren<CardView>();
            if (!cardView)
                return;
        }
        int startValue = 100 + position * 5;
        if (cardView.shadow)
            cardView.shadow.sortingOrder = startValue;
        if (cardView.bombSelectionParticle)
        {
            cardView.bombSelectionParticle.GetComponent<ParticleSystemRenderer>().sortingOrder = startValue + 1;
            foreach (var rend in cardView.bombSelectionParticle.GetComponentsInChildren<ParticleSystemRenderer>())
                rend.sortingOrder = startValue + 1;
        }
        ParticleSystem a = new ParticleSystem();

        if (cardView.mainTexture)
            cardView.mainTexture.sortingOrder = startValue + 2;
        foreach (var sprite in cardView.valueElements)
        {
            sprite.sortingOrder = startValue + 3;
        }
        if (cardView.bombIcon)
            cardView.bombIcon.sortingOrder = startValue + 4;
    }


    public static List<Card> SortCardsByValue(List<Card> cards)
    {
        cards.Sort((y, x) => x.value.CompareTo(y.value));
        return cards;
    }

    public static List<Card> SortCardsBySubsets(List<Card> cards)
    {
        // Группируем карты по номиналу и считаем количество в каждой группе
        var groups = cards.GroupBy(c => c.value)
                          .Select(g => new
                          {
                              Value = g.Key,
                              Count = g.Count(),
                              Cards = g.ToList()
                          })
                          .ToList();

        // Сортируем группы: сначала по размеру группы (убывание), потом по номиналу (убывание)
        groups.Sort((a, b) =>
        {
            int countComparison = b.Count.CompareTo(a.Count);
            if (countComparison != 0)
                return countComparison;
            return b.Value.CompareTo(a.Value);
        });

        // Собираем результат, сохраняя порядок карт внутри групп
        List<Card> sorted = new List<Card>();
        foreach (var group in groups)
        {
            // Дополнительная сортировка внутри группы по масти (если нужно)
            var groupCards = group.Cards;
            groupCards.Sort((a, b) => b.suit.CompareTo(a.suit));
            sorted.AddRange(groupCards);
        }

        return sorted;
    }
}


public enum Suits
{
    clubs,
    diamonds,
    hearts,
    spades,
    jokers // black - 1, red - 2
}
