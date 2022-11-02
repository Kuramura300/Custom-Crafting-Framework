using System;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Assets.Properties
{
    /// <summary>
    /// Class which handles the validating of xml files
    /// </summary>
    public class XmlValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlValidator"/> class
        /// </summary>
        public XmlValidator()
        {
            schema = new XmlSchemaSet();

            // This is a hard-coded path as the validation will only take place within the editor
            schema.Add( "", "Assets\\Properties\\Properties.xsd" );
        }

        /// <summary>
        /// Validate the given xml document against the schema
        /// </summary>
        /// <param name="doc">The xml file to be validated</param>
        public void Validate( XDocument doc )
        {
            doc.Validate( schema, ValidationEventHandler );
        }

        /// <summary>
        /// Handles the validation event
        /// </summary>
        static void ValidationEventHandler( object sender, ValidationEventArgs e )
        {
            XmlSeverityType type = XmlSeverityType.Warning;

            if ( Enum.TryParse( "Error", out type ) == true )
            {
                if ( type == XmlSeverityType.Error )
                {
                    throw new Exception( e.Message );
                }
            }
        }

        private XmlSchemaSet schema = null;
    }
}