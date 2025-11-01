[System.Serializable]
public class DeckMessage : Message
{
    public int[] player1_deck;
    public int[] player2_deck;
    public int[] player3_deck;
    public int[] landlord_deck;

    public DeckMessage(int shuffleMode = 0)
    {
        this.type = "deck";
        int[][] deck = new int[5][];
        switch (shuffleMode)
        {
            case 0:
                deck = RandomDecksGenerator.GetShuffledDeck();
                break;
            case 1:
                deck = RandomDecksGenerator.GetShuffledDeckForBombMode();
                break;
        }
        player1_deck = deck[0];
        player2_deck = deck[1];
        player3_deck = deck[2];
        landlord_deck = deck[3];
    }
    
}