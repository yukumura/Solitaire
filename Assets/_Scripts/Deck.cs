using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents the deck
/// </summary>
public class Deck : Pile
{
    // All cards in deck
    public GameObject[] allCardsInDeck = new GameObject[52];

    //Sound to play when rebuild deck
    public AudioClip clipShuffleDeck;

    //Audio source
    private AudioSource audioSource;

    // Sprite for the end of the deck
    public Sprite endSprite;

    // Number of attempts
    public int attemptsRemaining = 2;

    //Waste reference
    public Waste waste;

    //DetectCondition reference
    private DetectCondition GameManager;

    void OnMouseDown()
    {
        if (!GameManager.pausedGame)
        {
            if (attemptsRemaining > 0)
            {
                //Play sound
                audioSource.Play();

                RebuildDeck();
                attemptsRemaining--;
                if (attemptsRemaining == 0)
                {
                    GetComponent<SpriteRenderer>().sprite = endSprite;
                }
            }
            else
            {
                //we no longer have any attempts to shuffle the deck
                GameManager.Lose();
            } 
        }
    }

    /// <summary>
    /// Reposition the cards in the deck. If a card hitted with raycast, that card must be replace it in the deck
    /// </summary>
    private void RebuildDeck()
    {
        // Rebuild deck
        Transform wasteTransform = waste.transform;
        cardCount = 0;

        while (true)
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(wasteTransform.position, Vector3.back);

            if (hit)
            {
                GameObject cardGO = hit.collider.gameObject;
                Card card = cardGO.GetComponent<Card>();
                
                PlaceCardOnTop(card, cardGO);
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Place the card on top of another
    /// </summary>
    /// <param name="card"></param>
    /// <param name="cardGO"></param>
    private void PlaceCardOnTop(Card card, GameObject cardGO)
    {
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, -cardCount - 1);
        cardGO.transform.position = newPosition;
        card.backgroundSR.sortingOrder = cardCount;
        card.suitRenderer.sortingOrder = cardCount + 1;
        card.valueSR.sortingOrder = cardCount + 1;
        card.isOpen = false;
        card.ApplySettings();
        card.InDeck = true;
        card.isFirst = false;
        card.inWaste = false;

        cardCount++;
        waste.cardCount--;
    }

    // Use this for initialization
    void Start()
    {
        GetReference();
    }

    /// <summary>
    /// Initialize reference
    /// </summary>
    private void GetReference()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clipShuffleDeck;
        GameManager = FindObjectOfType<DetectCondition>();
    }
}
