using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class CardGenerator : MonoBehaviour
{
    // Prefab and properties of cell 7
    public GameObject cell7Prefab;
    public float cell7XOffset = 3f;
    public float cell7YOffsetForClosedCards = 0.2f;
    public float cell7YOffsetForOpenCards = 0.5f;
    public GameObject[] allCell7 = new GameObject[7];

    // Prefab and properties of cell 4
    public GameObject cell4Prefab;
    public float cell4XOffset = 3f;
    public GameObject[] allCell4 = new GameObject[4];

    // Deck
    public Deck deck;

    // All 52 cards
    public GameObject[] allCards = new GameObject[52];

    // Distance between cards
    public float offsetX = 3;
    public float offsetY = 4;

    // Card Prefab
    public GameObject cardPrefab;

    // Suit Sprites
    public Sprite[] suitSprites = new Sprite[4];

    // Value for cards
    public Sprite[] cardValueTextures = new Sprite[13];

    //Index for access into card array
    int allCardsIndex = 0;


    // Use this for initialization
    void Start()
    {
        // Create all cards
        for (int iSuite = 0; iSuite <= 3; iSuite++)
        {
            for (int iValue = 0; iValue <= 12; iValue++)
            {
                CreateCard(iSuite, iValue);                
            }
        }

        // Deck shuffle
        DeckShuffle();        

        // Create 7 cell
        for (int iCell7 = 0; iCell7 < 7; iCell7++)
        {
            GameObject cell7GO = CreateCell(cell7Prefab, iCell7, allCell7);            

            // Fill the cell with cards
            for (int iCell7Cards = 0; iCell7Cards < iCell7 + 1; iCell7Cards++)
            {
                InsertCard(cell7GO, iCell7Cards, iCell7);                                
            }
        }

        // Create 4 cell
        for (int iCell4 = 0; iCell4 < 4; iCell4++)
        {
            CreateCell(cell4Prefab, iCell4, allCell4);
        }
    }

    /// <summary>
    /// Insert into cell a card. If the two indexes match, no longer have to insert cards in that cell
    /// </summary>
    /// <param name="cell7GO">Cell 7 Game Object: where insert the card</param>
    /// <param name="iCell7Cards">index for card in the cell</param>
    /// <param name="iCell7">index for cell</param>
    private void InsertCard(GameObject cell7GO, int iCell7Cards, int iCell7)
    {
        // Move card from deck into cell
        Card card = deck.allCardsInDeck[allCardsIndex].GetComponent<Card>();

        Cell7 cell7 = cell7GO.GetComponent<Cell7>();

        // Place card on top to another
        card.backgroundSR.sortingOrder = cell7.cardCount;
        card.suitRenderer.sortingOrder = cell7.cardCount + 1;
        card.valueSR.sortingOrder = cell7.cardCount + 1;
        cell7.cardCount++;
        card.inCell7 = true;

        // Set card not in deck anymore
        card.InDeck = false;

        // Reveal the card
        Vector3 newPositionForCard;
        if (iCell7Cards == iCell7)
        {
            card.isOpen = true;
            card.isFirst = true;
            card.ApplySettings();

            newPositionForCard = new Vector3(cell7.transform.position.x, cell7.transform.position.y - iCell7Cards * cell7YOffsetForClosedCards, -cell7.cardCount);
        }
        else
        {
            newPositionForCard = new Vector3(cell7.transform.position.x, cell7.transform.position.y - iCell7Cards * cell7YOffsetForClosedCards, -cell7.cardCount);
        }

        card.transform.position = newPositionForCard;
        deck.allCardsInDeck[allCardsIndex] = null;
        allCardsIndex++;
    }

    /// <summary>
    /// For cell creation
    /// </summary>
    /// <param name="cell7Prefab">Cell 7 prefab</param>
    /// <param name="iCell7">index for the cell</param>
    /// <param name="allCell7">array of cell 7</param>
    /// <returns></returns>
    private GameObject CreateCell(GameObject cell7Prefab, int iCell7, GameObject[] allCell7)
    {
        Vector3 newPositionForCell7 = new Vector3(cell7Prefab.transform.position.x + iCell7 * cell7XOffset, cell7Prefab.transform.position.y, cell7Prefab.transform.position.z);
        GameObject cell7GO = Instantiate(cell7Prefab, newPositionForCell7, Quaternion.identity);
        allCell7[iCell7] = cell7GO;

        return cell7GO;
    }

    /// <summary>
    /// Method for shuffle the deck
    /// </summary>
    private void DeckShuffle()
    {
        int[] allCardsIndexes = new int[52];
        for (int i = 0; i < allCardsIndexes.Length; i++)
        {
            allCardsIndexes[i] = i;
        }
        for (int i = 0; i < allCardsIndexes.Length; i++)
        {
            int index = allCardsIndexes[i];
            int randomIndex = UnityEngine.Random.Range(0, allCardsIndexes.Length - 1);
            allCardsIndexes[i] = allCardsIndexes[randomIndex];
            allCardsIndexes[randomIndex] = index;
        }

        for (int i = allCards.Length - 1; i >= 0; i--)
        {
            GameObject cardGO = allCards[allCardsIndexes[i]];
            Card card = cardGO.GetComponent<Card>();
            deck.allCardsInDeck[i] = cardGO;

            // Place card on top to another
            Vector3 newPosition = new Vector3(deck.transform.position.x, deck.transform.position.y, -deck.cardCount - 1);
            cardGO.transform.position = newPosition;
            card.backgroundSR.sortingOrder = deck.cardCount;
            card.suitRenderer.sortingOrder = deck.cardCount + 1;
            card.valueSR.sortingOrder = deck.cardCount + 1;

            deck.cardCount++;
        }

        allCardsIndex = 0;
    }

    /// <summary>
    /// Method for card creation
    /// </summary>
    /// <param name="iSuite">index for suite</param>
    /// <param name="iValue">index for value</param>
    private void CreateCard(int iSuite, int iValue)
    {
        Vector3 newPosition = new Vector3(deck.transform.position.x, deck.transform.position.y, -deck.cardCount);
        GameObject cardGO = Instantiate(cardPrefab, newPosition, cardPrefab.transform.rotation);
        Card card = cardGO.GetComponent<Card>();
        card.isOpen = false;

        // Apply suit  
        card.suit = iSuite + 1;
        card.suitRenderer.sprite = suitSprites[iSuite];

        card.ApplySettings();

        // Apply value
        card.value = iValue + 1;
        card.valueSR.sprite = cardValueTextures[iValue];


        // Rename card
        card.name = "Card (" + card.value + ", " + (card.isCardColorRed ? "Red" : "Black") + " (" + card.suitRenderer.sprite.name + "))";

        // Add card into deck
        allCards[allCardsIndex] = card.gameObject;
        allCardsIndex++;
    }
}
