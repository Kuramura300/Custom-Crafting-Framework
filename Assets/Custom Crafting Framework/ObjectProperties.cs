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

    [Tooltip( "When true, adds properties together (up to max value) on combination. When false, the highest number is taken instead" )]
    /// <summary>
    /// When true, adds properties together (up to max value) on combination. When false, the highest number is taken instead
    /// </summary>
    public bool AddPropertiesOnCombine = false;

    /// <summary>
    /// When true, enables the combination point functionality
    /// </summary>
    [Header( "Combination Point Functionality" )]
    [Tooltip( "If true, when combining, move object to combine to this object based on selected combination points" )]
    public bool UseCombinationPoints = true;

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
    /// Add property to this object with given property name with a random value or default value if defined in xml
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

                XElement defaultValue = xmlDocument.Descendants( "Property" )
                                        .Where( n => n.Element( "Name" ).Value == propertyName )
                                        .Select( n => n.Element( "DefaultValue" ) ).FirstOrDefault();

                if ( defaultValue == null )
                {
                    // Generate a random actual value for this property to have based on the min/max values
                    // We add 1 to the MaxValue since Random.Range(int, int) is exclusive on the max value
                    newProperty.ActualValue = Random.Range( newProperty.MinValue, newProperty.MaxValue + 1 );
                }
                else
                {
                    // Set value to the default value if one is defined
                    newProperty.ActualValue = int.Parse( defaultValue.Value );
                }

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
    /// <param name="objectToCombine">Object with the properties to be combined in</param>
    public void CombinePropertiesIn( GameObject objectToCombine )
    {
        if ( objectToCombine != null )
        {
            ObjectProperties objectToCombineProperties = objectToCombine.GetComponent<ObjectProperties>();

            if ( objectToCombineProperties != null )
            {
                List<Property> newProperties = objectToCombine.GetComponent<ObjectProperties>().properties;

                // Invoke any custom behaviour that is wanted
                BeforeCombinationBehaviour.Invoke();

                foreach ( Property p in newProperties )
                {
                    // If property is not already in the properties list, add it
                    if ( properties.Any( prop => prop.Name == p.Name ) == false )
                    {
                        properties.Add( p );
                    }
                    // Behaviour if it is already in the list
                    else
                    {
                        // If the one currently in the list has a lower value, remove it and add the new one instead
                        if ( AddPropertiesOnCombine == false )
                        {
                            Property sameProp = properties.Where( prop => prop.Name == p.Name ).FirstOrDefault();

                            if ( sameProp.ActualValue < p.ActualValue )
                            {
                                properties.Remove( sameProp );
                                properties.Add( p );
                            }
                        }
                        // Add values together and set it
                        else
                        {
                            int newValue = properties.Where( prop => prop.Name == p.Name ).FirstOrDefault().ActualValue += p.ActualValue;

                            SetProperty( p.Name, newValue );
                        }
                    }
                }

                if ( UseCombinationPoints == true )
                {
                    if ( SelectedCombinationPoint != null )
                    {
                        // Move object to combine to this object based on selected combination points
                        CombinationPoint objectToCombinePoint = objectToCombine.GetComponent<ObjectProperties>().SelectedCombinationPoint;

                        if ( objectToCombinePoint != null )
                        {
                            Vector3 selectedPointPositionOffset = objectToCombine.transform.position - objectToCombinePoint.transform.position;
                            objectToCombine.transform.position = SelectedCombinationPoint.transform.position + selectedPointPositionOffset;

                            // Rotate the object to combine and adjust position so that both points stay overlapped
                            objectToCombine.transform.rotation = SelectedCombinationPoint.transform.rotation;
                            objectToCombine.transform.position = SelectedCombinationPoint.transform.rotation * ( objectToCombine.transform.position - SelectedCombinationPoint.transform.position ) + SelectedCombinationPoint.transform.position;

                            // Hide points and set as combined
                            SelectedCombinationPoint.transform.Find( "PointHighlight" ).gameObject.SetActive( false );
                            objectToCombinePoint.transform.Find( "PointHighlight" ).gameObject.SetActive( false );

                            SelectedCombinationPoint.HasCombined = true;
                            objectToCombinePoint.HasCombined = true;

                            // Update selected point of this object and turn green
                            SelectedCombinationPoint = combinationPoints.FirstOrDefault( p => p.HasCombined == false );

                            if ( SelectedCombinationPoint != null )
                            {
                                SelectedCombinationPoint.GetComponentInChildren<SpriteRenderer>().color = new Color( 0, 255, 0, 0.59f );
                            }
                        }
                        else
                        {
                            Debug.LogError( "Object '" + objectToCombine.name + "' has no Selected Combination Point. Cannot move object to combine to this object based on selected combination point." );
                        }
                    }
                    else
                    {
                        Debug.LogError( "Object '" + transform.name + "' has no Selected Combination Point. Cannot move object to combine to this object based on selected combination point." );
                    }
                }

                // Remove ObjectProperties of the combined object
                Destroy( objectToCombine.GetComponent<ObjectProperties>() );

                // Make combined object a child of the this one
                objectToCombine.transform.parent = transform;

                // Invoke any custom behaviour that is wanted
                AfterCombinationBehaviour.Invoke();
            }
            else
            {
                Debug.LogError( "Could not find an ObjectProperties component on the object to combine. Combination failed." );
            }
        }
        else
        {
            Debug.LogError( "The object to combine was null. Combination failed." );
        }
    }

    /// <summary>
    /// Update the Selected Combination Point for this object to given point
    /// </summary>
    /// <param name="newSelectedPoint">The new selected combination point</param>
    public void UpdateSelectedCombinationPoint( CombinationPoint newSelectedPoint )
    {
        // Check this combination point exists in list
        if ( combinationPoints.Any( p => p == newSelectedPoint ) )
        {
            foreach ( CombinationPoint point in combinationPoints.Where( p => p.HasCombined == false ) )
            {
                // If this is the point, select it
                if ( point == newSelectedPoint )
                {
                    // Turn this point green
                    newSelectedPoint.GetComponentInChildren<SpriteRenderer>().color = new Color( 0, 255, 0, 0.59f );

                    SelectedCombinationPoint = point;
                }
                else
                {
                    // Turn other points blue
                    point.GetComponentInChildren<SpriteRenderer>().color = new Color( 0, 0, 255, 0.59f );
                }
            }
        }
        else
        {
            Debug.LogError( "Selected point is not present within the list of Combination Points for object '" + transform.name + "'" );
        }
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
        combinationPoints = GetComponentsInChildren<CombinationPoint>().ToList();

        SelectedCombinationPoint = combinationPoints.FirstOrDefault();
    }

    private const string PropertiesHandlerObject = "PropertiesHandlerObject";

    private List<CombinationPoint> combinationPoints;
    private List<Property> properties = new List<Property>();
    private XDocument xmlDocument;
}
