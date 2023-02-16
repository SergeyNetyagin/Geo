using UnityEngine;

namespace VostokVR.Geo {

    public class TextureDistanceControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private SphereCollider player_sphere_collider;

        // Start is called before the first frame update ##################################################################################################################################################################################################
        private void Start() {

            if( TexturesQualityControl.Instance == null ) { 
            
                return;
            }

            if( TexturesQualityControl.Instance.Textures_visualization_mode == TexturesVisualizationMode.Variable ) {

                player_sphere_collider.radius = TexturesQualityControl.Instance.Change_texture_distance;

                #if( DEBUG_MODE )
                //CanvasController.Instance.ShowDebugInfo( this, Camera.main.name + " camera rigidbody state: " + ((sphere_rigidbody != null) ? "ADDED" : "NONE") );
                #endif

                #if( DEBUG_MODE )
                //CanvasController.Instance.ShowDebugInfo( this, "Camera sphere collider state: " + ((player_sphere_collider != null) ? "ADDED" : "NONE") );
                #endif

                #if( DEBUG_MODE )
                //CanvasController.Instance.ShowDebugInfo( this, "Camera textures control state: " + ((texture_control != null) ? "ADDED" : "NONE") );
                #endif
            }        
        }

        // On trigger enter ###############################################################################################################################################################################################################################
        private void OnTriggerEnter( Collider other ) {

            #if( DEBUG_MODE )
            //CanvasDebug.Instance.ShowMessage( "OnTriggerEnter detected in " + other.name );
            #endif

            TeleportSurfaceRock rock = other.GetComponent<TeleportSurfaceRock>();

            if( rock != null ) {
                
                if( rock.Material_control.Material_resolution != MaterialResolution.High ) {

                    rock.Material_control.Material_resolution = MaterialResolution.High;

                    #if( DEBUG_MODE )
                    //CanvasController.Instance.ShowDebugInfo( this, "OnTriggerEnter detected with " + rock.name );
                    #endif
                }
            }
        }

        // On trigger exit ################################################################################################################################################################################################################################
        private void OnTriggerExit( Collider other ) {

            #if( DEBUG_MODE )
            //CanvasDebug.Instance.ShowMessage( "OnTriggerExit detected from " + other.name );
            #endif

            TeleportSurfaceRock rock = other.GetComponent<TeleportSurfaceRock>();

            if( rock != null ) {

                if( rock.Material_control.Material_resolution != MaterialResolution.Low ) {

                    rock.Material_control.Material_resolution = MaterialResolution.Low;

                    #if( DEBUG_MODE )
                    //CanvasController.Instance.ShowDebugInfo( this, "OnTriggerExit detected with " + rock.name );
                    #endif
                }
            }
        }
    }
}