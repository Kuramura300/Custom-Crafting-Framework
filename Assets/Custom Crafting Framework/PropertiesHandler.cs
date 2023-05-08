using System.Xml.Linq;
using UnityEngine;

namespace CustomCraftingFramework
{
    /// <summary>
    /// Class that handles the reading and handling of Properties
    /// </summary>
    public class PropertiesHandler : MonoBehaviour
    {
        /// <summary>
        /// Xml file to be used for the Properties
        /// </summary>
        public TextAsset PropertiesXmlFile = null;

        void Awake()
        {
            // Only validate while inside of editor
            if ( ( PropertiesXmlFile != null ) && ( Application.isEditor == true ) )
            {
                XmlValidator xmlValidator = new XmlValidator();

                XDocument doc = XDocument.Parse( PropertiesXmlFile.text );

                xmlValidator.Validate( doc );
            }
        }

        /// <summary>
        /// Returns the properties xml file as an XDocument
        /// </summary>
        /// <returns>The XDocument for the properties, otherwise null</returns>
        public XDocument GetXmlFile()
        {
            XDocument doc = null;

            if ( PropertiesXmlFile != null )
            {
                doc = XDocument.Parse( PropertiesXmlFile.text );
            }

            return doc;
        }
    }
}
