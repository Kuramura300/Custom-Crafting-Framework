using Assets.Properties;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// Class that handles the reading and handling of Properties
/// </summary>
public class PropertiesHandler : MonoBehaviour
{
    /// <summary>
    /// Xml file to be used for the Properties
    /// </summary>
    public TextAsset PropertiesXmlFile = null;

    // Start is called before the first frame update
    void Start()
    {
        // Only validate while inside of editor
        if ( ( PropertiesXmlFile != null ) && ( Application.isEditor == true ) )
        {
            XmlValidator xmlValidator = new XmlValidator();

            XDocument doc = XDocument.Parse( PropertiesXmlFile.text );

            xmlValidator.Validate( doc );
        }
    }
}
