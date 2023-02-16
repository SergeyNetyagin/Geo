using UnityEngine;
using UnityEngine.UI;

namespace VostokVR.Geo {

    public class CheckIntValueControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private int min_value = 0;

        [SerializeField]
        private int max_value = 100;

        [SerializeField]
        private int default_value = 1;
        public int Default_value { get { return default_value; } }

        [SerializeField]
        private int current_value = 0;
        public int Current_value { get { return current_value; } }

        // #################################################################################################################################################################################################################################################
        private void Awake() {
        
        }

        // #################################################################################################################################################################################################################################################
        private void Start() {
        
        }

        // #################################################################################################################################################################################################################################################
        public void CheckValue( InputField input_field ) {
        
            try {
            
                current_value = int.Parse( input_field.text );
            }

            catch { 
            
                #if( UNITY_EDITOR )
                Debug.LogError( "Cannot parse value to int from string: " + input_field.text );
                #endif

                current_value = default_value;
            }

            if( current_value < min_value ) { 
            
                current_value = default_value;
            }

            else if( current_value > max_value ) { 
            
                current_value = default_value;
            }

            input_field.text = current_value.ToString();
        }
    }
}