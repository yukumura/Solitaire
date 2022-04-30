using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for Cells, Deck and Waste
/// </summary>
public class Pile : MonoBehaviour {

    public int cardCount = 0;

    //If true means there are no cards above
    public bool isFirst = false;
}
