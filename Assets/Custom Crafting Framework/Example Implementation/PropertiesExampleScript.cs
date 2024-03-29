using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CustomCraftingFramework
{
    /// <summary>
    /// Script for the functionality of the Properties Example
    /// </summary>
    public class PropertiesExampleScript : MonoBehaviour
    {
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
        /// The button used to add a property to an object
        /// </summary>
        public Button AddPropertyBtn;

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

        /// <summary>
        /// Input field where the user can define the property name to add to an object
        /// </summary>
        public TMP_InputField NewPropertyNameInput;

        /// <summary>
        /// Dropdown where the user can choose which object to add a new property to
        /// </summary>
        public TMP_Dropdown NewPropertyObjectDropdown;

        /// <summary>
        /// Dropdown where the user can choose which object to set a property of
        /// </summary>
        public TMP_Dropdown ObjectToSetPropertyOfDropdown;

        /// <summary>
        /// Dropdown where the user can choose which property of selected object to set
        /// </summary>
        public TMP_Dropdown PropertyToSetDropdown;

        /// <summary>
        /// Input field where the user can choose the new value to set for the selected property of selected object
        /// </summary>
        public TMP_InputField ValueToSetToPropertyInput;

        private void Start()
        {
            UpdateDropdownOptions();
        }

        private void Update()
        {
            UpdatePropertiesUIText();
        }


        //
        // Code related to the demonstration of combining the Stone with the Stick
        //

        /// <summary>
        /// Combine objects
        /// </summary>
        public void Combine()
        {
            BaseObject.GetComponent<ObjectProperties>() // get ObjectProperties component of BaseObject
                .CombinePropertiesIn( ObjectToCombine ); // combine ObjectToCombine in
        }

        /// <summary>
        /// Combine objects with a new object name
        /// </summary>
        public void Combine( string newName )
        {
            Combine();

            BaseObject.name = newName;

            UpdateDropdownOptions();
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
            Debug.Log( "This is the custom after combination behaviour being run. The objects have been combined, and this runs after!" );
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
        // Adding Properties demonstration
        //

        /// <summary>
        /// Add a new property to object selected in dropdown on button press
        /// </summary>
        public void AddNewProperty()
        {
            GameObject selectedObject = GameObject.Find( NewPropertyObjectDropdown.options[NewPropertyObjectDropdown.value].text );

            if ( selectedObject != null )
            {
                // Get ObjectProperties of the selected object in the dropdown
                ObjectProperties props = selectedObject.GetComponent<ObjectProperties>();

                if ( props != null )
                {
                    // Check the property is not already added
                    if ( props.GetProperties().Any( p => p.Name == NewPropertyNameInput.text ) == false )
                    {
                        // Add property
                        props.AddProperty( NewPropertyNameInput.text );
                    }
                    else
                    {
                        Debug.LogError( "The property '" + NewPropertyNameInput.text + "' is already added to the object '" + selectedObject.name + "'." );
                    }
                }
                else
                {
                    Debug.LogError( "Could not find the ObjectProperties component of object selected in the dropdown." );
                }
            }
            else
            {
                Debug.LogError( "Could not find object selected in the dropdown." );
            }
        }

        /// <summary>
        /// Updates the options of the dropdown, for use when the object names change
        /// </summary>
        private void UpdateDropdownOptions()
        {
            // Add BaseObject and ObjectToCombine options to the NewPropertyObjectDropdown
            List<string> dropdownOptions = new List<string> { BaseObject.name };

            // If ObjectToCombine hasn't been combined, show it on the dropdown. We do this by checking if the ObjectProperties component is still enabled,
            // since this implementation of the framework will remove it from the ObjectToCombine after combination
            if ( ObjectToCombine.GetComponent<ObjectProperties>().isActiveAndEnabled == true )
            {
                dropdownOptions.Add( ObjectToCombine.name );
            }

            NewPropertyObjectDropdown.ClearOptions();
            NewPropertyObjectDropdown.AddOptions( dropdownOptions );

            // Also add the options to ObjectToSetPropertyOfDropdown
            ObjectToSetPropertyOfDropdown.ClearOptions();
            ObjectToSetPropertyOfDropdown.AddOptions( dropdownOptions );

            // Update PropertyToSetDropdown options
            UpdatePropertyToSetDropdownOptions();
        }


        //
        // Set properties demonstration
        //

        /// <summary>
        /// Sets a new value from user input for selected property of selected object
        /// </summary>
        public void SetPropertyValueFromUI()
        {
            int value = 0;

            // Check that the input field is a valid integer
            if ( int.TryParse( ValueToSetToPropertyInput.text, out value ) == true )
            {
                // Get the ObjectProperties from the selected object from ObjectToSetPropertyOfDropdown
                // and set the property selected from PropertyToSetDropdown to the new value
                GameObject.Find( ObjectToSetPropertyOfDropdown.options[ObjectToSetPropertyOfDropdown.value].text ).GetComponent<ObjectProperties>()
                    .SetProperty( PropertyToSetDropdown.options[PropertyToSetDropdown.value].text, value );
            }
            else
            {
                Debug.LogError( "The input field for setting a property must be an integer." );
            }
        }

        /// <summary>
        /// Updates the options of the dropdown
        /// </summary>
        public void UpdatePropertyToSetDropdownOptions()
        {
            List<string> dropdownOptions = new List<string>();

            // Get the properties list from the selected object from ObjectToSetPropertyOfDropdown
            List<Property> props = GameObject.Find( ObjectToSetPropertyOfDropdown.options[ObjectToSetPropertyOfDropdown.value].text ).GetComponent<ObjectProperties>().GetProperties();

            foreach ( Property p in props )
            {
                // Add property name to options
                dropdownOptions.Add( p.Name );
            }

            PropertyToSetDropdown.ClearOptions();
            PropertyToSetDropdown.AddOptions( dropdownOptions );
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
}
