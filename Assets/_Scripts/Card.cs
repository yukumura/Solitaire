using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    //Sound to play when reveal the card
    public AudioClip clipRevealCard;

    //Audio source
    private AudioSource audioSource;

    // mouseDragging
    public bool mouseDragging = false;

    // Is this a first card in the place?
    public bool isFirst = false;

    // In deck
    public bool InDeck = true;

    // In Waste
    public bool inWaste = false;

    //boolean for animation
    private bool moveInWaste = false;

    //speed for animaition
    private readonly float speed = 2f;

    //for put the card above to all
    public int sortingOrderAdd = 100;
    public int sortingOrderAddZ = 100;

    //old position for the card, before the drag
    public Vector3 oldPositionInCell;

    //render layer
    public int backgroundSRSortingOrder;
    public int mastSRSortingOrder;
    public int valueSRSortingOrder;

    // In one of the 7 cells
    public bool inCell7 = false;

    // In one of the 4 cells
    public bool inCell4 = false;

    // Is this reveal?
    public bool isOpen = false;

    // Texture
    public SpriteRenderer backgroundSR;
    public Sprite openedCardTexture;
    public Sprite closedCardTexture;

    //Colors and suit: Red, hearts and diamonds - Black, clubs and spades 
    public int suit = 0;
    public SpriteRenderer suitRenderer;

    // Color
    public bool isCardColorRed = false;

    // Cards value: 1 (A), 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 (J), 12 (Q), 13 (K)
    public int value = 0;
    public SpriteRenderer valueSR;

    // Reference for old position
    public Vector3 oldCardPosition;
    public Transform oldParent;

    //Already take the score for this card?
    private bool alreadyTakeScore = false;

    //Reference for other object
    private DetectCondition GameManager;
    private Animator animator;
    private Waste wastePileHelper;
    private Score scoreManager;
    private CardGenerator cardGenerator;

    /// <summary>
    /// Method for apply the card changes
    /// </summary>
    public void ApplySettings()
    {
        // Set property for card
        if (isOpen)
        {
            backgroundSR.sprite = openedCardTexture;
            suitRenderer.gameObject.SetActive(true);
            valueSR.gameObject.SetActive(true);
        }
        else
        {
            backgroundSR.sprite = closedCardTexture;
            suitRenderer.gameObject.SetActive(false);
            valueSR.gameObject.SetActive(false);
        }

        // Set color
        if (suit < 3)
        {
            isCardColorRed = true;
        }
        else
        {
            isCardColorRed = false;
        }
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
        GameManager = FindObjectOfType<DetectCondition>();
        wastePileHelper = FindObjectOfType<Waste>();
        scoreManager = FindObjectOfType<Score>();
        cardGenerator = FindObjectOfType<CardGenerator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clipRevealCard;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.pausedGame)
        {
            if (!mouseDragging && transform.parent != null && transform.parent.GetComponent<Card>() != null)
            {
                DrawCard();
            }

            //if the card is moving to reach the waste
            if (moveInWaste)
            {
                Vector3 newPosition = new Vector3(wastePileHelper.transform.position.x, wastePileHelper.transform.position.y, -24);
                MoveCardInWaste(newPosition);

                if (Vector3.Distance(transform.position, newPosition) < .1f)
                {
                    //the card has reached the waste
                    moveInWaste = false;
                    GameManager.SetFlippingCard(false);
                    transform.position = new Vector3(wastePileHelper.transform.position.x, wastePileHelper.transform.position.y, -wastePileHelper.cardCount - 1);
                    SetRenderOrder();
                }
            }
        }
    }

    /// <summary>
    /// Draw the card
    /// </summary>
    private void DrawCard()
    {
        Card parentCard = transform.parent.GetComponent<Card>();
        SpriteRenderer parentCardSR = parentCard.GetComponent<SpriteRenderer>();
        backgroundSR.sortingOrder = parentCardSR.sortingOrder + 1;
        suitRenderer.sortingOrder = backgroundSR.sortingOrder + 1;
        valueSR.sortingOrder = backgroundSR.sortingOrder + 1;
        parentCard.isFirst = false;
    }

    void OnMouseDrag()
    {
        if (!GameManager.pausedGame)
        {
            mouseDragging = true;

            if (isOpen && (inWaste || inCell7 || inCell4))
            {
                // Move the card
                Vector3 newPosition = Camera.main.ScreenPointToRay(Input.mousePosition).origin + oldCardPosition;
                transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
            }
        }
    }

    /// <summary>
    /// when holding the left of the mouse
    /// </summary>
    void OnMouseDown()
    {
        if (!GameManager.pausedGame)
        {
            // Save the position card
            oldCardPosition = transform.position - Camera.main.ScreenPointToRay(Input.mousePosition).origin;

            if (!isOpen)
            {
                // if the card is in the deck
                if (InDeck)
                {
                    GameManager.SetFlippingCard(true);
                    moveInWaste = true;

                    SaveCardPosition();
                    GameManager.SetCards(this, null, null);

                    // Reveal the card  
                    isOpen = true;
                    FlipCard();

                    //Play sound for reveal card
                    audioSource.Play();

                    // Move the card in the spot near the deck
                    SetMaxRenderOrder();
                }
                // if the card is in the one of the 7 cells
                if (inCell7 && isFirst)
                {
                    // Reveal the card
                    isOpen = true;
                    ApplySettings();
                }
            }

            if (isOpen && (inWaste || inCell7 || inCell4))
            {
                SaveCardPosition();                
                RevealFirstCard();
            }
        }
    }

    /// <summary>
    /// Set render layer
    /// </summary>
    private void SetRenderOrder()
    {
        backgroundSR.sortingOrder = wastePileHelper.cardCount;
        suitRenderer.sortingOrder = wastePileHelper.cardCount + 1;
        valueSR.sortingOrder = wastePileHelper.cardCount + 1;
        wastePileHelper.cardCount++;
        isFirst = true;
    }

    /// <summary>
    /// Set render layer in a way that the card appears above all
    /// </summary>
    private void SetMaxRenderOrder()
    {
        backgroundSR.sortingOrder = 23;
        suitRenderer.sortingOrder = 24;
        valueSR.sortingOrder = 24;
    }

    /// <summary>
    /// Flip the card. The animation has an event that triggers the function ApplySettings
    /// </summary>
    private void FlipCard()
    {
        animator.SetTrigger("flipCard");
    }

    /// <summary>
    /// Reveal the first card 
    /// </summary>
    private void RevealFirstCard()
    {
        backgroundSR.sortingOrder += sortingOrderAdd;
        suitRenderer.sortingOrder += sortingOrderAdd;
        valueSR.sortingOrder += sortingOrderAdd;
        transform.position = new Vector3(transform.position.x, transform.position.y, -sortingOrderAddZ);
    }

    /// <summary>
    /// Save the position of the card in the cell
    /// </summary>
    private void SaveCardPosition()
    {
        oldPositionInCell = transform.position;
        oldParent = transform.parent;
        transform.SetParent(null);

        backgroundSRSortingOrder = backgroundSR.sortingOrder;
        mastSRSortingOrder = suitRenderer.sortingOrder;
        valueSRSortingOrder = valueSR.sortingOrder;
    }

    /// <summary>
    /// Move card in waste at specific speed
    /// </summary>
    /// <param name="newPosition">the position of Waste</param>
    private void MoveCardInWaste(Vector3 newPosition)
    {
        float step = speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y, -24);
        transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, -24), newPosition, step);
    }

    /// <summary>
    /// When release the left of the mouse
    /// </summary>
    void OnMouseUp()
    {
        if (!GameManager.pausedGame)
        {
            mouseDragging = false;

            if (isOpen && !InDeck)
            {
                //Set refrence for Undo action
                if (inWaste)
                {
                    GameManager.cardPreviouslyInWaste = true;
                }
                else
                {
                    GameManager.cardPreviouslyInWaste = false;
                }

                ToggleCollider(false);

                // Check what hit with the raycast
                RaycastHit2D hit;
                hit = Physics2D.Raycast(transform.position + (-oldCardPosition), Vector3.forward);

                Card otherCard = null;
                Cell7 cell7 = null;
                Cell4 cell4 = null;

                //if hit something
                if (hit.collider != null)
                {
                    otherCard = hit.collider.GetComponent<Card>();
                    cell7 = hit.collider.GetComponent<Cell7>();
                    cell4 = hit.collider.GetComponent<Cell4>();
                }

                //case king in the 7 cell
                if (cell7 != null && cell7.isFirst && value == 13)
                {
                    PutCardInCell(hit, null, cell7);
                }
                //case ace in the 4 cell
                else if (cell4 != null && value == 1 && isFirst)
                {
                    if (!inCell4 && !alreadyTakeScore)
                    {
                        //add point if put the card in cell 4
                        scoreManager.AddPoint(10);
                        alreadyTakeScore = true;
                    }

                    PutCardInCell(hit, null, cell4);
                }
                //case card in 7 cell not empty
                else if (otherCard != null &&
                         otherCard.inCell7 &&
                         otherCard.isFirst &&
                         otherCard.isOpen &&
                         otherCard.isCardColorRed != isCardColorRed &&
                         otherCard.value == value + 1
                    )
                {
                    float offSetForOpenCards = cardGenerator.cell7YOffsetForOpenCards;
                    PutCardInCell(hit, hit.collider.transform, otherCard, offSetForOpenCards);

                    if (transform.childCount < 3)
                    {
                        isFirst = true;
                    }
                }
                //case card in 4 cell not empty
                else if (otherCard != null &&
                         isFirst &&
                         otherCard.inCell4 &&
                         otherCard.isFirst &&
                         otherCard.isOpen &&
                         otherCard.suit == suit &&
                         otherCard.value == value - 1
                    )
                {
                    if (!inCell4 && !alreadyTakeScore)
                    {
                        scoreManager.AddPoint(10);
                        alreadyTakeScore = true;
                    }

                    PutCardInCell(hit, null, otherCard, 0);
                }
                // Position not allowed: replace the card in the old position
                else if (!InDeck)
                {
                    ResetOldPositionCard();
                    hit = new RaycastHit2D();
                }

                // Reveal the card under the card moved
                if (hit.collider != null && (cell7 != null || cell4 != null || otherCard != null))
                {
                    //Set refrence for Undo action
                    if (cell7 != null)
                    {
                        GameManager.SetCards(this, otherCard, cell7);
                    }
                    else if (cell4 != null)
                    {
                        GameManager.SetCards(this, otherCard, cell4);
                    }
                    else
                    {
                        GameManager.SetCards(this, otherCard, null);

                    }

                    RevealCard();
                }

                ToggleCollider(true);
            }

            if (InDeck)
            {
                // Not in the deck anymore
                InDeck = false;
                inWaste = true;
            }
        }
    }

    /// <summary>
    /// Reveal the card under the card moved
    /// </summary>
    private void RevealCard()
    {
        RaycastHit2D hitDownCard;
        hitDownCard = Physics2D.Raycast(oldPositionInCell, Vector3.forward);
        Card oldDownCard = null;
        Cell7 cell7Down = null;
        Cell4 cell4Down = null;

        if (hitDownCard.collider != null)
        {
            oldDownCard = hitDownCard.collider.GetComponent<Card>();
            cell7Down = hitDownCard.collider.GetComponent<Cell7>();
            cell4Down = hitDownCard.collider.GetComponent<Cell4>();
        }

        if (oldDownCard != null)
        {
            oldDownCard.isFirst = true;
            oldDownCard.isOpen = true;
            oldDownCard.ApplySettings();
        }
        else
        if (cell7Down != null)
        {
            cell7Down.isFirst = true;
        }
        else
        if (cell4Down != null)
        {
            cell4Down.isFirst = true;
        }
    }

    /// <summary>
    /// Hide the card under the card
    /// </summary>
    public void HideCard()
    {
        RaycastHit2D hitDownCard;
        hitDownCard = Physics2D.Raycast(oldPositionInCell, Vector3.forward);
        Card oldDownCard = null;
        Cell7 cell7Down = null;
        Cell4 cell4Down = null;

        if (hitDownCard.collider != null)
        {
            oldDownCard = hitDownCard.collider.GetComponent<Card>();
            cell7Down = hitDownCard.collider.GetComponent<Cell7>();
            cell4Down = hitDownCard.collider.GetComponent<Cell4>();
        }

        if (oldDownCard != null)
        {
            oldDownCard.isFirst = false;
            oldDownCard.isOpen = false;
            oldDownCard.ApplySettings();
        }
        else
        if (cell7Down != null)
        {
            cell7Down.isFirst = false;
        }
        else
        if (cell4Down != null)
        {
            cell4Down.isFirst = false;
        }
    }

    /// <summary>
    /// Reset position cald into old saved position
    /// </summary>
    public void ResetOldPositionCard()
    {
        transform.position = oldPositionInCell;
        transform.SetParent(oldParent);

        backgroundSR.sortingOrder = backgroundSRSortingOrder;
        suitRenderer.sortingOrder = mastSRSortingOrder;
        valueSR.sortingOrder = valueSRSortingOrder;
    }

    /// <summary>
    /// Put the card in the cell
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="parent">parent to set for card</param>
    /// <param name="pile">The pile where put the card</param>
    private void PutCardInCell(RaycastHit2D hit, Transform parent, Pile pile)
    {
        transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z - 1);
        transform.SetParent(parent);

        SpriteRenderer cell4SR = pile.GetComponent<SpriteRenderer>();
        backgroundSR.sortingOrder = cell4SR.sortingOrder + 1;
        suitRenderer.sortingOrder = backgroundSR.sortingOrder + 1;
        valueSR.sortingOrder = backgroundSR.sortingOrder + 1;

        pile.isFirst = false;
        isFirst = true;

        if (pile.GetType() == typeof(Cell7))
        {
            inCell4 = false;
            inWaste = false;
            inCell7 = true;
        }
        else if (pile.GetType() == typeof(Cell4))
        {
            inCell7 = false;
            inWaste = false;
            inCell4 = true;
        }
    }

    /// <summary>
    /// Put the card above another card
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="parent">parent to set for card</param>
    /// <param name="card"></param>
    /// <param name="offset"></param>
    private void PutCardInCell(RaycastHit2D hit, Transform parent, Card card, float offset)
    {
        transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y - offset, hit.collider.transform.position.z - 1);
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.localEulerAngles = new Vector3(0, 0, 0);

        transform.SetParent(parent);
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.localEulerAngles = new Vector3(0, 0, 0);

        backgroundSR.sortingOrder = card.backgroundSR.sortingOrder + 1;
        suitRenderer.sortingOrder = card.suitRenderer.sortingOrder + 1;
        valueSR.sortingOrder = card.valueSR.sortingOrder + 1;

        card.isFirst = false;
        isFirst = true;

        if (card.inCell7)
        {
            inCell4 = false;
            inWaste = false;
            inCell7 = true;
        }
        else if (card.inCell4)
        {
            inCell7 = false;
            inWaste = false;
            inCell4 = true;
        }
    }

    /// <summary>
    /// Activate / Deactivate collider
    /// </summary>
    /// <param name="enableCollider"></param>
    private void ToggleCollider(bool enableCollider)
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = enableCollider;
    }
}
