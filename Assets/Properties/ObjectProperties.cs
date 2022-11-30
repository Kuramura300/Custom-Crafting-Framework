using Assets.Properties;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

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
    /// Initialise properties on Awake since this runs before Start
    /// </summary>
    private void Awake()
    {
        if (Properties.Count > 0)
        {
            var handler = GameObject.Find( PropertiesHandlerObject );

            if ( handler != null )
            {
                var handlerScript = handler.GetComponent<PropertiesHandler>();

                if ( handlerScript != null )
                {
                    XDocument doc = handlerScript.GetXmlFile();

                    if (doc != null)
                    {
                        // Check each property exists in xml file and add it to the list of Property objects if it does
                        foreach ( var p in Properties )
                        {
                            if (doc.Descendants("Name").Any(n => n.Value == p))
                            {
                                Property newProperty = new Property();

                                // Set properties known from xml file
                                newProperty.Name = p;
                                newProperty.MinValue = int.Parse( doc.Descendants( "Property" )
                                                       .Where( n => n.Element( "Name" ).Value == p )
                                                       .Select( n => n.Element( "MinValue" ).Value ).FirstOrDefault() );
                                newProperty.MaxValue = int.Parse( doc.Descendants( "Property" )
                                                       .Where( n => n.Element( "Name" ).Value == p )
                                                       .Select( n => n.Element( "MaxValue" ).Value ).FirstOrDefault() );

                                // Generate a random actual value for this property to have based on the min/max values
                                // We add 1 to the MaxValue since Random.Range(int, int) is exclusive on the max value
                                newProperty.ActualValue = Random.Range( newProperty.MinValue, newProperty.MaxValue + 1 );

                                properties.Add( newProperty );
                            }
                            else
                            {
                                Debug.LogError( "Object '" + gameObject.name + "' could not find a property with name '" + p + "' in xml file. It will ignore this property." );
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError( "Could not find or parse the properties xml file." );
                    }
                }
                else
                {
                    Debug.LogError( "Could not find the PropertiesHandler script on the " + PropertiesHandlerObject + ". Please add it to the object." );
                }
            }
            else
            {
                Debug.LogError( "Could not find " + PropertiesHandlerObject + " in scene. Please add it to the scene so that object properties may be initialised." );
            }
        }
    }

    private void Start()
    {
        // TEMPORARY: Testing purposes
        foreach ( var p in properties )
        {
            Debug.Log( p.Name + " - Random initialised value: " + p.ActualValue );
        }

        SetProperty( "Length", 99 );
        Debug.Log( "Attempted to set Length property to 99. Length property value is now: " + GetProperty( "Length" ).ActualValue );

        SetProperty( "Length", 101 );
        Debug.Log( "Attempted to set Length property to 101 (above maximum). Length property value is now: " + GetProperty( "Length" ).ActualValue );

        SetProperty( "Length", -1 );
        Debug.Log( "Attempted to set Length property to -1 (below minimum). Length property value is now: " + GetProperty( "Length" ).ActualValue );

        Debug.Log( "Testing GetProperties() function:" );
        foreach ( var p in GetProperties() )
        {
            Debug.Log( p.Name + " - Value: " + p.ActualValue + " MinValue: " + p.MinValue + " MaxValue: " + p.MaxValue );
        }

        SetProperty( "propertyThatDoesntExist", 50 );
        GetProperty( "propertyThatDoesntExist" );
    }

    private const string PropertiesHandlerObject = "PropertiesHandlerObject";

    private List<Property> properties = new List<Property>();
}
