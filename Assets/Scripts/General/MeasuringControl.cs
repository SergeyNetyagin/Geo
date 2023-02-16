using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VertexStudio.Networking;
using VertexStudio.VirtualRealty;

namespace VostokVR.Geo {

    public class MeasuringControl : MonoBehaviour {

        public static MeasuringControl Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private Transform scaler_transform;
        public Transform Scaler_transform { get { return scaler_transform; } }

        [SerializeField]
        private LookAtTarget scaler_look_component;
        public LookAtTarget Scaler_look_component { get { return scaler_look_component; } }

        [Space( 10 ), SerializeField]
        private LineRenderer measurer_line_renderer;
        public LineRenderer Measurer_line_renderer { get { return measurer_line_renderer; } }

        [SerializeField]
        private Transform start_sphere_transform;

        [SerializeField]
        private Transform end_sphere_transform;

        [SerializeField]
        private Transform measurer_text_transform;

        [SerializeField]
        private Text measurer_text;

        [SerializeField]
        private Vector3 measurer_text_offset = new Vector3( 0f, 0.1f, 0f );

        [Space( 10 ), SerializeField]
        private LineRenderer section_line_renderer;
        public LineRenderer Section_line_renderer { get { return section_line_renderer; } }

        [SerializeField]
        private LineRenderer angle_ray_line_renderer;
        public LineRenderer Angle_ray_line_renderer { get { return angle_ray_line_renderer; } }

        [SerializeField]
        private Transform corner_point_transform;

        [SerializeField]
        private Transform section_end_point_transform;

        [SerializeField]
        private Transform ray_end_point_transform;

        [SerializeField]
        private Transform angle_text_transform;

        [SerializeField]
        private Transform angle_value_transform;

        [SerializeField]
        private Text angle_text;

        [SerializeField]
        private Image angle_value_image;

        [SerializeField]
        private Vector3 angle_text_offset = new Vector3( 0f, 0.1f, 0f );

        // Awake is called before the first frame update
        private void Awake() {
        
            Instance = this;
        }

        // Start is called before the first frame update
        private void Start() {
        
            scaler_transform.gameObject.SetActive( false );

            measurer_line_renderer.gameObject.SetActive( false );
            measurer_line_renderer.SetPosition( 0, Vector3.zero );
            measurer_line_renderer.SetPosition( 1, Vector3.zero );

            section_line_renderer.gameObject.SetActive( false );
            section_line_renderer.SetPosition( 0, Vector3.zero );
            section_line_renderer.SetPosition( 1, Vector3.zero );

            angle_ray_line_renderer.gameObject.SetActive( false );
            angle_ray_line_renderer.SetPosition( 0, Vector3.zero );
            angle_ray_line_renderer.SetPosition( 1, Vector3.zero );
        }

        // Put scaler to the place and shows it ##########################################################################################################################################################################################################
        public void ShowScaler( Vector3 position ) { 
        
            scaler_transform.position = position;

            if( !scaler_transform.gameObject.activeSelf ) { 

                scaler_transform.gameObject.SetActive( true );
            }

            if( !scaler_look_component.enabled ) { 

                scaler_look_component.enabled = true;
            }
        }

        // Activates measurer #############################################################################################################################################################################################################################
        public void ActivateMeasurer() { 
        
            measurer_line_renderer.gameObject.SetActive( true );

            if( PhotonNetwork.connected && (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkProjectManager.Instance != null) && (NetworkTransactionsControlMeasurers.Instance != null) ) { 

                NetworkTransactionsControlMeasurers.Instance.CreateMeasurer( VertexStudio.Networking.NetworkPlayer.Instance.View_ID, SceneManager.GetActiveScene().name );
            }
        }

        // Deactivates measurer ###########################################################################################################################################################################################################################
        public void DeactivateMeasurer( bool state ) { 
        
            measurer_line_renderer.gameObject.SetActive( state );

            if( PhotonNetwork.connected && (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkProjectManager.Instance != null) && (NetworkTransactionsControlMeasurers.Instance != null) ) { 

                NetworkTransactionsControlMeasurers.Instance.DestroyMeasurer( VertexStudio.Networking.NetworkPlayer.Instance.View_ID, SceneManager.GetActiveScene().name );
            }
        }

        // Puts measurer and shows distance value #########################################################################################################################################################################################################
        public void ShowMeasurer( Vector3 start_point, Vector3 end_point ) { 

            start_sphere_transform.position = start_point;
            end_sphere_transform.position = end_point;

            measurer_line_renderer.SetPosition( 0, start_sphere_transform.position );
            measurer_line_renderer.SetPosition( 1, end_sphere_transform.position );

            float distance = Vector3.Distance( start_sphere_transform.position, end_sphere_transform.position );

            measurer_text.text = EffectiveText.FloatToString( distance );
            measurer_text.text += " m";

            measurer_text_transform.position = new Vector3( 
                
                end_sphere_transform.position.x, 
                end_sphere_transform.position.y, 
                end_sphere_transform.position.z
            );

            measurer_text_transform.Translate( measurer_text_offset, Space.Self );
            measurer_text_transform.LookAt( ViveInteractionsManager.Instance.Eye_camera_transform );
            measurer_text_transform.eulerAngles = new Vector3( 0f, measurer_text_transform.eulerAngles.y + 180f, 0f );

            if( PhotonNetwork.connected && (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkProjectManager.Instance != null) && (NetworkTransactionsControlMeasurers.Instance != null) ) { 

                NetworkTransactionsControlMeasurers.Instance.ShowMeasurer( 
                    
                    VertexStudio.Networking.NetworkPlayer.Instance.View_ID, 
                    SceneManager.GetActiveScene().name,
                    start_sphere_transform.position,
                    end_sphere_transform.position,
                    measurer_text_transform.position,
                    measurer_text_transform.eulerAngles,
                    measurer_text.text
                );
            }
        }

        // Activates angle meter ##########################################################################################################################################################################################################################
        public void ActivateAngleMeter( bool show_angle_ray ) { 
        
            section_line_renderer.gameObject.SetActive( true );
            angle_ray_line_renderer.gameObject.SetActive( show_angle_ray );

            if( PhotonNetwork.connected && (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkProjectManager.Instance != null) && (NetworkTransactionsControlMeasurers.Instance != null) ) { 

                if( !show_angle_ray ) {
                
                    NetworkTransactionsControlAngleMeters.Instance.CreateAngleMeter( VertexStudio.Networking.NetworkPlayer.Instance.View_ID, SceneManager.GetActiveScene().name );
                }

                else { 
                    
                    NetworkTransactionsControlAngleMeters.Instance.ShowEntireAngleMeter( VertexStudio.Networking.NetworkPlayer.Instance.View_ID, SceneManager.GetActiveScene().name );
                }
            }
        }

        // Deactivates angle meter ########################################################################################################################################################################################################################
        public void DeactivateAngleMeter( bool state ) { 
        
            section_line_renderer.gameObject.SetActive( state );

            if( PhotonNetwork.connected && (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkProjectManager.Instance != null) && (NetworkTransactionsControlMeasurers.Instance != null) ) { 

                NetworkTransactionsControlAngleMeters.Instance.DestroyAngleMeter( VertexStudio.Networking.NetworkPlayer.Instance.View_ID, SceneManager.GetActiveScene().name );
            }
        }

        // Puts angle meter and shows angle value #########################################################################################################################################################################################################
        public void ShowAngleMeter( Vector3 corner_point, Vector3 end_section_point, Vector3 end_ray_point, bool show_angle_value ) { 

            corner_point_transform.position = corner_point;
            section_end_point_transform.position = end_section_point;// new Vector3( end_section_point.x, corner_point.y, end_section_point.z );

            // Draw ray for the angle
            if( angle_ray_line_renderer.gameObject.activeSelf ) {

                ray_end_point_transform.position = end_ray_point;

                if( !ray_end_point_transform.gameObject.activeSelf ) { 

                    ray_end_point_transform.gameObject.SetActive( true );
                }

                angle_ray_line_renderer.SetPosition( 0, corner_point_transform.position );
                angle_ray_line_renderer.SetPosition( 1, ray_end_point_transform.position );
            }

            // Draw horizontal section
            else { 

                if( ray_end_point_transform.gameObject.activeSelf ) { 

                    ray_end_point_transform.gameObject.SetActive( false );
                }

                section_line_renderer.SetPosition( 0, corner_point_transform.position );
                section_line_renderer.SetPosition( 1, section_end_point_transform.position );
            }

            if( show_angle_value ) {

                if( !angle_text.gameObject.activeSelf ) { 

                    angle_text.gameObject.SetActive( true );
                }

                Vector3 normalized_from_position = ray_end_point_transform.position - corner_point_transform.position;
                Vector3 normalized_to_position = section_end_point_transform.position - corner_point_transform.position;
                Vector3 axle = Vector3.Cross( normalized_from_position, normalized_to_position );

                float angle = Vector3.SignedAngle( 
                    
                    normalized_from_position,
                    normalized_to_position,
                    axle.normalized
                );

                angle_text.text = EffectiveText.FloatToString( angle, 1 );

                angle_text_transform.position = new Vector3( 
                
                    ray_end_point_transform.position.x, 
                    ray_end_point_transform.position.y, 
                    ray_end_point_transform.position.z
                );

                angle_text_transform.Translate( angle_text_offset, Space.Self );
                angle_text_transform.LookAt( ViveInteractionsManager.Instance.Eye_camera_transform );
                angle_text_transform.eulerAngles = new Vector3( 0f, angle_text_transform.eulerAngles.y + 180f, 0f );

                if( (angle_value_image != null) && angle_value_image.gameObject.activeInHierarchy ) { 

                    angle_value_image.fillClockwise = (angle < 180f);
                    angle_value_image.fillAmount = angle_value_image.fillClockwise ? (1f / 360f * angle) : (1f - 1f / 360f * angle);
                    
                    angle_value_transform.position = section_line_renderer.GetPosition( 0 );
                    angle_value_transform.LookAt( section_line_renderer.GetPosition( 1 ) );
                    angle_value_transform.Rotate( 0f, 90f, 0f, Space.Self );
                }
            }

            else { 

                if( angle_text.gameObject.activeSelf ) { 

                    angle_text.gameObject.SetActive( false );
                }                
            }

            if( PhotonNetwork.connected && (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkProjectManager.Instance != null) && (NetworkTransactionsControlMeasurers.Instance != null) ) { 

                NetworkTransactionsControlAngleMeters.Instance.ShowAngleMeter( 
                    
                    VertexStudio.Networking.NetworkPlayer.Instance.View_ID, 
                    SceneManager.GetActiveScene().name,
                    angle_ray_line_renderer.gameObject.activeSelf,
                    show_angle_value,
                    corner_point_transform.position,
                    section_end_point_transform.position,
                    ray_end_point_transform.position,
                    angle_text_transform.position,
                    angle_text_transform.eulerAngles,
                    angle_text.text
                );
            }
        }
    }
}