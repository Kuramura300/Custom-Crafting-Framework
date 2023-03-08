using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Script for the example functionality of combination point selection
/// </summary>
public class ExamplePointScript : MonoBehaviour
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
        // Check all points in the ObjectProperties this point is for
        foreach ( CombinationPoint point in objectProperties.CombinationPoints.Where( p => p.HasCombined == false ) )
        {
            // If this is the point, select it
            if ( point == combinationPoint )
            {
                // Turn this point green
                GetComponent<SpriteRenderer>().color = new Color( 0, 255, 0, 0.59f );

                objectProperties.SelectedCombinationPoint = point;
            }
            else
            {
                // Turn other points blue
                point.GetComponentInChildren<SpriteRenderer>().color = new Color( 0, 0, 255, 0.59f );
            }
        }
    }

    private ObjectProperties objectProperties = null;
    private CombinationPoint combinationPoint = null;
}
