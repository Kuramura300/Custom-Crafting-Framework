using CustomCraftingFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class that handles the properties of an object
/// </summary>
public class ObjectProperties : MonoBehaviour
{
    /// <summary>
    /// List of properties that this object has which can be manipulated from the inspector
    /// </summary>
    [Tooltip("The list of properties that this object has")]
    public List<string> Properties = new List<string>();

    /// <summary>
    /// Behaviour which can be added to the inspector which will be invoked before another object's properties are combined into this one
    /// </summary>
    [Header( "Behaviour to be invoked before another object's\nproperties are combined into this one" )]
    public UnityEvent BeforeCombinationBehaviour;

    /// <summary>
    /// Behaviour which can be added to the inspector which will be invoked after another object's properties are combined into this one
    /// </summary>
    [Header( "Behaviour to be invoked after another object's\nproperties are combined into this one" )]
    public UnityEvent AfterCombinationBehaviour;

    /// <summary>
    /// List of combination points for this object that other objects can be combined to
    /// </summary>
    public List<CombinationPoint> CombinationPoints;

    /// <summary>
    /// The currently selected combination point (defaults to first in list or null)
    /// </summary>
    public CombinationPoint SelectedCombinationPoint;

    /// <summary>
    /// Get a list of the properties of this object
    /// </summary>
    /// <returns>List of properties of this objcet</returns>
    public List<Property> GetProperties()
    {
        return properties;
    }

    /// <summary>
    /// Get a specific property of this object
    /// </summary>
    /// <param name="propertyName">The name of the property</param>
    /// <returns>The Property object, otherwise null</returns>
    public Property GetProperty( string propertyName )
    {
        Property returnProperty = null;

        if ( properties.Any( p => p.Name == propertyName ) )
        {
            returnProperty = properties.Where( p => p.Name == propertyName ).FirstOrDefault();
        }
        else
        {
            Debug.LogError( "Tried to get property " + propertyName + " from object " + gameObject.name + ", but this property was not found on the object." );
        }

        return returnProperty;
    }

    /// <summary>
    /// Set a property to a new value
    /// </summary>
    /// <param name="propertyName">Name of the property to give a new value to</param>
    /// <param name="newValue">New value to set the propety to. If above or below the min/max value of this property,
    /// it will be set to the min/max value instead.</param>
    public void SetProperty( string propertyName, int newValue )
    {
        if ( properties.Any( p => p.Name == propertyName ) )
        {
            // Ensure that the new property value is within the min/max values
            Property reference = properties.Where( p => p.Name == propertyName ).FirstOrDefault();
            if ( newValue < reference.MinValue ) newValue = reference.MinValue;
            if ( newValue > reference.MaxValue ) newValue = reference.MaxValue;

            // Set new value
            properties.Where( p => p.Name == propertyName ).FirstOrDefault().ActualValue = newValue;
        }
        else
        {
            Debug.LogError( "Tried to set property " + propertyName + " in object " + gameObject.name + ", but this property was not found on the object." );
        }
    }

    /// <summary>
    /// Add property to this object with given property name with a random value
    /// </summary>
    /// <param name="propertyName">Property name as defined in the xml document</param>
    public void AddProperty( string propertyName )
    {
        if ( xmlDocument != null )
        {
            if ( xmlDocument.Descendants( "Name" ).Any( n => n.Value == propertyName ) )
            {
                Property newProperty = new Property();

                // Set properties known from xml file
                newProperty.Name = propertyName;
                newProperty.MinValue = int.Parse( xmlDocument.Descendants( "Property" )
                                        .Where( n => n.Element( "Name" ).Value == propertyName )
                                        .Select( n => n.Element( "MinValue" ).Value ).FirstOrDefault() );
                newProperty.MaxValue = int.Parse( xmlDocument.Descendants( "Property" )
                                        .Where( n => n.Element( "Name" ).Value == propertyName )
                                        .Select( n => n.Element( "MaxValue" ).Value ).FirstOrDefault() );

                // Generate a random actual value for this property to have based on the min/max values
                // We add 1 to the MaxValue since Random.Range(int, int) is exclusive on the max value
                newProperty.ActualValue = Random.Range( newProperty.MinValue, newProperty.MaxValue + 1 );

                properties.Add( newProperty );
            }
            else
            {
                Debug.LogError( "Object '" + gameObject.name + "' could not find a property with name '" + propertyName + "' in xml file. It will ignore this property." );
            }
        }
        else
        {
            Debug.LogError( "Could not find or parse the properties xml file." );
        }
    }

    /// <summary>
    /// Add property to this object with given property name and value
    /// </summary>
    /// <param name="propertyName">Property name as defined in the xml document</param>
    /// <param name="value">Value to set the property to</param>
    public void AddProperty( string propertyName, int value )
    {
        if ( xmlDocument != null )
        {
            if ( xmlDocument.Descendants( "Name" ).Any( n => n.Value == propertyName ) )
            {
                Property newProperty = new Property();

                // Set properties known from xml file
                newProperty.Name = propertyName;
                newProperty.MinValue = int.Parse( xmlDocument.Descendants( "Property" )
                                        .Where( n => n.Element( "Name" ).Value == propertyName )
                                        .Select( n => n.Element( "MinValue" ).Value ).FirstOrDefault() );
                newProperty.MaxValue = int.Parse( xmlDocument.Descendants( "Property" )
                                        .Where( n => n.Element( "Name" ).Value == propertyName )
                                        .Select( n => n.Element( "MaxValue" ).Value ).FirstOrDefault() );

                // Ensure that the new property value is within the min/max values
                if ( value < newProperty.MinValue ) value = newProperty.MinValue;
                if ( value > newProperty.MaxValue ) value = newProperty.MaxValue;

                newProperty.ActualValue = value;

                properties.Add( newProperty );
            }
            else
            {
                Debug.LogError( "Object '" + gameObject.name + "' could not find a property with name '" + propertyName + "' in xml file. It will ignore this property." );
            }
        }
        else
        {
            Debug.LogError( "Could not find or parse the properties xml file." );
        }
    }

    /// <summary>
    /// Combine a list of properties into this object's list of properties
    /// </summary>
    /// <param name="newProperties">Properties to be combined in</param>
    public void CombinePropertiesIn( List<Property> newProperties )
    {
        // Invoke any custom behaviour that is wanted
        BeforeCombinationBehaviour.Invoke();

        foreach ( Property p in newProperties )
        {
            // If property is not already in the properties list, add it
            if ( properties.Any( prop => prop.Name == p.Name ) == false )
            {
                properties.Add( p );
            }
            // If it is already in the list, if the one currently in the list has a lower value, remove it and add the new one instead
            else
            {
                Property sameProp = properties.Where( prop => prop.Name == p.Name ).FirstOrDefault();

                if ( sameProp.ActualValue < p.ActualValue )
                {
                    properties.Remove( sameProp );
                    properties.Add( p );
                }
            }
        }

        // Invoke any custom behaviour that is wanted
        AfterCombinationBehaviour.Invoke();
    }

    /// <summary>
    /// Initialise properties on Awake since this runs before Start
    /// </summary>
    private void Awake()
    {
        // Initialise properties
        if (Properties.Count > 0)
        {
            var handler = GameObject.Find( PropertiesHandlerObject );

            if ( handler != null )
            {
                var handlerScript = handler.GetComponent<PropertiesHandler>();

                if ( handlerScript != null )
                {
                    xmlDocument = handlerScript.GetXmlFile();

                    // Check each property exists in xml file and add it to the list of Property objects if it does
                    foreach ( var p in Properties )
                    {
                        AddProperty( p );
                    }
                }
                else
                {
                    Debug.LogError( "Could not find the PropertiesHandler script on the '" + PropertiesHandlerObject + "'. Please add it to the object." );
                }
            }
            else
            {
                Debug.LogError( "Could not find '" + PropertiesHandlerObject + "' prefab in scene. Please add it to the scene so that object properties may be initialised." );
            }
        }

        // Find all combination points
        CombinationPoints = GetComponentsInChildren<CombinationPoint>().ToList();

        SelectedCombinationPoint = CombinationPoints.FirstOrDefault();
    }

    private const string PropertiesHandlerObject = "PropertiesHandlerObject";

    private List<Property> properties = new List<Property>();
    private XDocument xmlDocument;
}
