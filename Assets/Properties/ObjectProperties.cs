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

    private const string PropertiesHandlerObject = "PropertiesHandlerObject";

    private List<Property> properties = new List<Property>();
}
