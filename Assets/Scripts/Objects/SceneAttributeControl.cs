using UnityEngine;
using UnityEngine.SceneManagement;

namespace VostokVR.Geo {

    public class SceneAttributeControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private bool enable_object_on_start = false;
        public bool Enable_object_on_start { get { return enable_object_on_start; } }

        [Space( 10 ), SerializeField]
        private SceneAttribute[] scene_attributes;

        // Start is called before the first frame update ##################################################################################################################################################################################################
        private void Start() {
        
            CheckForActiveScene();

            #if( UNITY_EDITOR )
            //Debug.Log( GetSceneAttribute().name + " activated by SceneAttributeControl" );
            #endif
        }

        // Activates specified attribute ##################################################################################################################################################################################################################
        public void CheckForActiveScene() { 

            if( enable_object_on_start ) {
            
                ActivateObject( SceneManager.GetActiveScene().name );
            }            
        }

        // Activates specified attribute ##################################################################################################################################################################################################################
        public SceneAttribute GetSceneAttribute() { 

            SceneAttribute scene_attribute = null;

            for( int i = 0; i < scene_attributes.Length; i++ ) { 

                if( SceneManager.GetActiveScene().name == scene_attributes[i].Scene_name ) { 
                    
                    scene_attribute = scene_attributes[i];

                    break;
                }
            }

            return scene_attribute;
        }

        // Activates specified attribute ##################################################################################################################################################################################################################
        public void ActivateObject( string scene_name ) { 
        
            for( int i = 0; i < scene_attributes.Length; i++ ) { 

                if( scene_attributes[i].Scene_name == scene_name ) { 

                    scene_attributes[i].gameObject.SetActive( true );
                }

                else { 

                    scene_attributes[i].gameObject.SetActive( false );
                }
            }

            if( !gameObject.activeSelf ) { 
            
                gameObject.SetActive( true );
            }
        }

        // Deactivates specified attribute ################################################################################################################################################################################################################
        public void DeactivateObject( string scene_name ) { 
        
            for( int i = 0; i < scene_attributes.Length; i++ ) { 
            
                if( scene_attributes[i].Scene_name == scene_name ) { 
                    
                    scene_attributes[i].gameObject.SetActive( false );
                }
            }
        }

        // Deactivates specified attribute ###############################################################################################################################################################################################################
        public void DeactivateAllObjects( bool include_parent ) { 

            for( int i = 0; i < scene_attributes.Length; i++ ) { 

                scene_attributes[i].gameObject.SetActive( false );
            }

            if( include_parent ) { 
            
                gameObject.SetActive( false );
            }
        }

        // Switches scene attribute object ################################################################################################################################################################################################################
        public void SwitchState() { 

            if( gameObject.activeSelf ) { 
             
                gameObject.SetActive( false );
            }

            else { 
                
                gameObject.SetActive( true );    
            }
        }
    }
}
