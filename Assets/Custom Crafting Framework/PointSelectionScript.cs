using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Script for the functionality of combination point selection
/// </summary>
public class PointSelectionScript : MonoBehaviour
{
    private void Start()
    {
        objectProperties = transform.parent.GetComponentInParent<ObjectProperties>();
        combinationPoint = GetComponentInParent<CombinationPoint>();

        // Turn this point green if this is selected at the start
        if ( objectProperties.SelectedCombinationPoint == combinationPoint )
        {
            GetComponent<SpriteRenderer>().color = new Color( 0, 255, 0, 0.59f );
        }
    }

    private void OnMouseDown()
    {
        objectProperties.UpdateSelectedCombinationPoint( combinationPoint );
    }

    private ObjectProperties objectProperties = null;
    private CombinationPoint combinationPoint = null;
}
