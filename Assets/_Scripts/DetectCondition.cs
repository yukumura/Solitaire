using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for determine conditions in game
/// </summary>
public class DetectCondition : MonoBehaviour
{
    //Panel and text box where the class write
    public GameObject panel;
    private Text text;

    //AudioSource and audioclip for Win / Lose condition
    public AudioClip winJingle;
    public AudioClip loseJingle;
    private AudioSource audioSource;

    //Reference to card generator for determine win 
    private CardGenerator cardGenerator;

    //Reference for Undo action

    //the card that we are moving
    public Card childCard;

    //the card revealed by moving the child card
    public Card parentCard;

    //the pile that interacted with the card
    public Pile parentPile;

    //booleans for restore the card in deck or in waste
    public bool cardPreviouslyInDeck;
    public bool cardPreviouslyInWaste;

    //true when a card is flipping
    public bool flippingCard = false;

    //bool for pause the game
    public bool pausedGame = false;

    // Use this for initialization
    void Start()
    {
        GetReference();
    }

    void Update()
    {
        if(!pausedGame)
            DetectWin();
    }

    /// <summary>
    /// Initialize reference
    /// </summary>
    private void GetReference()
    {
        cardGenerator = FindObjectOfType<CardGenerator>();
        audioSource = GetComponent<AudioSource>();
        text = panel.GetComponentInChildren<Text>();
    }

    /// <summary>
    /// If all cards are revealed -> win
    /// </summary>
    private void DetectWin()
    {
        foreach (GameObject cardGO in cardGenerator.allCards)
        {
            Card card = cardGO.GetComponent<Card>();
            if (!card.isOpen)
            {
                return;
            }
        }

        Win();
    }

    private void Win()
    {
        audioSource.clip = winJingle;
        audioSource.loop = false;
        audioSource.Play();
        WriteInPanel("You Win!", Color.white);
        pausedGame = true;
        GetComponent<Timer>().enabled = false;
    }

    /// <summary>
    /// Method throw by deck if we don't have any attempt to shuffle deck
    /// </summary>
    public void Lose()
    {
        audioSource.clip = loseJingle;
        audioSource.loop = false;
        audioSource.Play();
        WriteInPanel("You Lose!", Color.red);
        GetComponent<Timer>().enabled = false;
        pausedGame = true;
    }

    /// <summary>
    /// Undo a single action. 
    /// if we have already done the undo, it is not possible to use the function again for the same card and for the same action
    /// WARNING: Unfortunately, this feature sometimes causes bugs when trying to put a card back into the deck. TODO: Fix this method. I think the problem is when fold too fast a card from deck into waste.
    /// </summary>
    public void UndoMove()
    {
        if (childCard != null && !flippingCard)
        {
            //restore the card in waste
            if (cardPreviouslyInWaste)
            {
                childCard.isOpen = true;
                childCard.InDeck = false;
                childCard.inWaste = true;
            }
            //hide the card for replace it in the deck
            else if (!cardPreviouslyInDeck)
            {
                childCard.HideCard();
            }
            //for cases where the card was in the cells  
            else
            {
                childCard.isOpen = false;
                childCard.InDeck = true;
                childCard.inWaste = false;
            }

            //restore the old position
            childCard.ResetOldPositionCard();

            //if the card was attached to another card, that card must also be reset
            if (parentCard != null)
            {
                parentCard.isFirst = true;

            }

            //if the card was attached to cell, that cell must also be reset
            if (parentPile != null)
            {
                parentPile.isFirst = true;
            }

            childCard.ApplySettings();
            childCard = null;
            parentCard = null;
        }
    }

    /// <summary>
    /// set cards (and/or cell) for the undo action
    /// </summary>
    /// <param name="childCard"></param>
    /// <param name="parentCard"></param>
    /// <param name="parentPile"></param>
    public void SetCards(Card childCard, Card parentCard, Pile parentPile)
    {
        this.childCard = childCard;
        this.parentCard = parentCard;
        this.parentPile = parentPile;
        if (childCard.InDeck)
        {
            cardPreviouslyInDeck = true;
        }
        else
        {
            cardPreviouslyInDeck = false;
        }
    }

    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {

        if (!pausedGame)
        {
            pausedGame = true;
            Time.timeScale = 0;
            WriteInPanel("Pause");
        }
        else
        {
            ResumeGame();
            return;
        }
    }

    /// <summary>
    /// Resume the game
    /// </summary>
    public void ResumeGame()
    {
        pausedGame = false;
        Time.timeScale = 1;
        HidePanel();
    }

    /// <summary>
    /// Write in the panel the textToShow
    /// </summary>
    /// <param name="textToShow"></param>
    private void WriteInPanel(string textToShow)
    {
        text.text = textToShow;
        Color c = panel.GetComponentInParent<Image>().color;
        c.a = .5f;
        panel.GetComponentInParent<Image>().color = c;
    }

    private void WriteInPanel(string textToShow, Color color)
    {
        text.text = textToShow;
        Color c = panel.GetComponentInParent<Image>().color;
        c.a = .5f;
        panel.GetComponentInParent<Image>().color = c;
        text.color = color;
    }

    /// <summary>
    /// Hide the panel
    /// </summary>
    private void HidePanel()
    {
        text.text = string.Empty;
        Color c = panel.GetComponentInParent<Image>().color;
        c.a = 0f;
        panel.GetComponentInParent<Image>().color = c;
    }

    /// <summary>
    /// Set bool for block every action while flip the card
    /// </summary>
    /// <param name="state"></param>
    public void SetFlippingCard(bool state)
    {
        flippingCard = state;
    }
}

