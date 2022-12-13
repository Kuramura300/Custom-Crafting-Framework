using Assets.Properties;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Script for the functionality of the Properties Example
/// </summary>
public class PropertiesExampleScript : MonoBehaviour
{
    //
    // Code related to the demonstration of combining the Stone with the Stick
    //
    
    /// <summary>
    /// The base object which is the Stick by default
    /// </summary>
    public GameObject BaseObject;

    /// <summary>
    /// The object to combine, which is the Stone by default
    /// </summary>
    public GameObject ObjectToCombine;

    /// <summary>
    /// The button used to combine the two objects
    /// </summary>
    public Button CombineBtn;

    /// <summary>
    /// The button used to combine the two objects with a new name
    /// </summary>
    public Button CombineWithNameBtn;

    /// <summary>
    /// Text to display the BaseObject's properties
    /// </summary>
    public TMP_Text BaseObjectText;

    /// <summary>
    /// Text to display the ObjectToCombine's properties
    /// </summary>
    public TMP_Text ObjectToCombineText;

    /// <summary>
    /// Input field where the user can define the new name of the object after combination
    /// </summary>
    public TMP_InputField NewNameInput;

    private void Update()
    {
        UpdatePropertiesUIText();
    }

    /// <summary>
    /// Combine objects
    /// </summary>
    public void Combine()
    {
        BaseObject.GetComponent<ObjectProperties>() // get ObjectProperties component of BaseObject
            .CombinePropertiesIn( ObjectToCombine.GetComponent<ObjectProperties>().GetProperties() ); // get properties of ObjectToCombine's ObjectProperties and combine them in
    }

    /// <summary>
    /// Combine objects with a new object name
    /// </summary>
    public void Combine(string newName)
    {
        Combine();

        BaseObject.name = newName;
    }

    /// <summary>
    /// Combine objects and give base object the name from the Input Field when the button is pressed
    /// </summary>
    public void CombineWithNewNameButtonPress()
    {
        Combine( NewNameInput.text );
    }

    /// <summary>
    /// Behaviour to be run before the Stone is combined with the Stick
    /// </summary>
    public void CustomBeforeCombinationBehaviour()
    {
        Debug.Log( "Combining '" + BaseObject.name + "' and '" + ObjectToCombine.name + "'." );

        CombineBtn.interactable = false;
        CombineWithNameBtn.interactable = false;
    }

    /// <summary>
    /// Behaviour to be run after the Stone is combined with the Stick
    /// </summary>
    public void CustomAfterCombinationBehaviour()
    {
        // Remove ObjectProperties of the object
        Destroy( ObjectToCombine.GetComponent<ObjectProperties>() );

        // Make object a child of the other
        ObjectToCombine.transform.parent = BaseObject.transform;

        Debug.Log( "'" + ObjectToCombine.name + "' is now a child of '" + ObjectToCombine.transform.parent.name + "'." );
    }

    /// <summary>
    /// Get strings representing name and value of each property for given ObjectProperties
    /// </summary>
    /// <param name="props">ObjectProperties to get property strings of</param>
    /// <returns>String with each property and value separated by a newline</returns>
    private string GetPropertiesAsStringForUI( ObjectProperties props )
    {
        string value = "";

        foreach ( Property p in props.GetProperties() )
        {
            value += p.Name + ": " + p.ActualValue.ToString() + "\n";
        }

        return value;
    }

    /// <summary>
    /// Updates the text on the UI to display the name and properties of each object. If the object has no properties, the text will be empty.
    /// </summary>
    private void UpdatePropertiesUIText()
    {
        BaseObjectText.text = BaseObject.name + "\n";
        ObjectToCombineText.text = ObjectToCombine.name + "\n";

        // Find ObjectProperties of both objects
        ObjectProperties baseProps = BaseObject.GetComponent<ObjectProperties>();
        ObjectProperties combineProps = ObjectToCombine.GetComponent<ObjectProperties>();

        if ( baseProps != null )
        {
            // Get properties text
            string basePropsText = "\n" + GetPropertiesAsStringForUI( baseProps );

            // Add string onto the end of the UI text
            BaseObjectText.text += basePropsText;
        }
        else
        {
            BaseObjectText.text = "";
        }

        if ( combineProps != null )
        {
            // Get properties text
            string combinePropsText = "\n" + GetPropertiesAsStringForUI( combineProps );

            // Add string onto the end of the UI text
            ObjectToCombineText.text += combinePropsText;
        }
        else
        {
            ObjectToCombineText.text = "";
        }
    }


    //
    // Reset
    //

    /// <summary>
    /// Reset scene by getting current scene name (in case name was changed) and loading it again
    /// </summary>
    public void ResetScene()
    {
        string s = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene( s );
    }
}
