using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using VertexStudio.VirtualRealty;
using VertexStudio.Networking;
using UnityEngine.Playables;

namespace VostokVR.Geo {

    public class VideoControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private VideoPlayer video_player;

        [Space( 10 ), SerializeField]
        private Button play_button;

        [SerializeField]
        private Button pause_button;

        [SerializeField]
        private Button replay_button;

        [Space( 10 ), SerializeField]
        private SliderControl slider_control;

        [SerializeField]
        private InteractableObject handle_slide_area;

        [SerializeField]
        private InteractableObject handle_sphere;

        [Space( 10 ), SerializeField]
        private PlayableDirector timeline;

        [Space( 10 ), SerializeField, Range( 0f, 10f )]
        private float min_sphere_local_position = 1f;

        [SerializeField, Range( 0f, 10f )]
        private float max_sphere_local_position = 4f;

        private bool drag_handle_sphere = false;

        private Transform touch_pointer_transform;
        private RectTransform handle_rect_transform;

        public void SetPlayerValue( float value ) { video_player.time = value; }
        public void SetSliderValue( float value ) { slider_control.SetValueWithoutNotify( value ); }

        private bool update_slider = true;

        // Start is called before the first frame update #################################################################################################################################################################################################
        private void Start() {
            
            video_player.sendFrameReadyEvents = true;

            video_player.frameReady += ( VideoPlayer player, long frame_index ) => { SetSliderValue( (float) player.time ); update_slider = true; };
            //video_player.seekCompleted += ( VideoPlayer player ) => { SetSliderValue( (float) player.time ); update_slider = true; };
        }

        // OnEnable is called before the first frame update ###############################################################################################################################################################################################
        private void OnEnable() {
        
            touch_pointer_transform = ViveInteractionsManager.Instance.Vive_pointer_right.Touch_pointer_mesh_renderer.GetComponent<Transform>();
            handle_rect_transform = handle_sphere.GetComponent<RectTransform>();

            slider_control.wholeNumbers = false;
            slider_control.minValue = 0f;
            slider_control.maxValue = (float) video_player.length;
            slider_control.SetValueWithoutNotify( 0f );

            play_button.gameObject.SetActive( false );
            pause_button.gameObject.SetActive( true );
            replay_button.gameObject.SetActive( true );

            video_player.time = 0f;

            if( !video_player.isPlaying ) { 
                
                video_player.Play();
            }

            if( timeline != null ) { 
            
                timeline.time = 0;
                timeline.Play();
            }
        }

        // LateUpdate is called once per frame ############################################################################################################################################################################################################
        private void LateUpdate() {

            #if( UNITY_EDITOR )
            if( Input.GetMouseButtonUp( 0 ) ) { 
                
                RaycastHit hit_info = new RaycastHit();

                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

                HandleSliderArea handle_slide_area = null;

                if( Physics.Raycast( ray, out hit_info, 100f ) ) { 
                    
                    handle_slide_area = hit_info.collider.GetComponent<HandleSliderArea>();
                }

                if( handle_slide_area != null ) {
                
                    ReleaseSliderHandle();
                }
            }
            #endif

            if( drag_handle_sphere ) { 

                Vector3 slider_position = GetHitPosition( handle_rect_transform.position );

                UpdateSliderValue( slider_position );

                if( (NetworkProjectManager.Instance != null) && PhotonNetwork.isMasterClient ) { 
            
                    if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                        NetworkTransactionsControlTools.Instance.SetVideoSliderValue( VertexStudio.Networking.NetworkPlayer.Instance.View_ID, slider_control.value, SceneManager.GetActiveScene().name );
                    }
                }
            }

            else if( update_slider && video_player.isPlaying ) { 

                slider_control.SetValueWithoutNotify( (float) video_player.time );

                float sphere_position_x = min_sphere_local_position + (max_sphere_local_position - min_sphere_local_position) * slider_control.normalizedValue;

                handle_rect_transform.anchoredPosition3D = new Vector3( sphere_position_x, 0f, 0f );

                if( timeline != null ) {

                    timeline.time = video_player.time;
                }
            }
        }

        // ################################################################################################################################################################################################################################################
        public void Play() { 
         
            play_button.gameObject.SetActive( false );
            pause_button.gameObject.SetActive( true );

            video_player.Play();

            if( timeline != null ) {

                timeline.Play();
            }

            if( (NetworkProjectManager.Instance != null) && PhotonNetwork.isMasterClient ) { 
            
                if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                    NetworkTransactionsControlTools.Instance.PlayVideo( VertexStudio.Networking.NetworkPlayer.Instance.View_ID, SceneManager.GetActiveScene().name );
                }
            }
        }

        // ################################################################################################################################################################################################################################################
        public void Pause() { 
            
            play_button.gameObject.SetActive( true );
            pause_button.gameObject.SetActive( false );

            video_player.Pause();

            if( timeline != null ) {

                timeline.Pause();
            }

            if( (NetworkProjectManager.Instance != null) && PhotonNetwork.isMasterClient ) { 
            
                if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                    NetworkTransactionsControlTools.Instance.PauseVideo( VertexStudio.Networking.NetworkPlayer.Instance.View_ID, SceneManager.GetActiveScene().name );
                }
            }
        }

        // ################################################################################################################################################################################################################################################
        public void Replay() { 
            
            play_button.gameObject.SetActive( false );
            pause_button.gameObject.SetActive( true );

            video_player.time = 0f;
            video_player.Play();

            slider_control.SetValueWithoutNotify( 0f );

            if( (NetworkProjectManager.Instance != null) && PhotonNetwork.isMasterClient ) { 
            
                if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                    NetworkTransactionsControlTools.Instance.ReplayVideo( VertexStudio.Networking.NetworkPlayer.Instance.View_ID, SceneManager.GetActiveScene().name );
                }
            }
        }

        // Calculates and returns slider value ############################################################################################################################################################################################################
        public void UpdateSliderValue( Vector3 handle_position ) { 

            handle_rect_transform.position = handle_position;
            handle_rect_transform.anchoredPosition3D = new Vector3( handle_rect_transform.anchoredPosition3D.x, 0f, 0f );

            if( handle_rect_transform.anchoredPosition3D.x < min_sphere_local_position ) { 
                    
                handle_rect_transform.anchoredPosition3D = new Vector3( min_sphere_local_position, 0f, 0f );
            }

            else if( handle_rect_transform.anchoredPosition3D.x > max_sphere_local_position ) { 
                    
                handle_rect_transform.anchoredPosition3D = new Vector3( max_sphere_local_position, 0f, 0f );
            }

            float slider_value = (
                    
                (handle_rect_transform.anchoredPosition3D.x - min_sphere_local_position) / 
                (max_sphere_local_position - min_sphere_local_position) *
                (float) video_player.length
            );

            slider_control.SetValueWithoutNotify( slider_value );

            #if( UNITY_EDITOR )
            //Debug.Log( "Set slider value to " + slider_value );
            #endif
        }

        // Returns hit position ###########################################################################################################################################################################################################################
        private Vector3 GetHitPosition( Vector3 initial_position ) { 

            Vector3 hit_position = initial_position;

            HandleSliderArea handle_slide_area = null;
            
            RaycastHit hit_info = new RaycastHit();

            if( SteamVR.active ) { 
                
                hit_position = touch_pointer_transform.position;
            }

            else { 

                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

                #if( UNITY_EDITOR )
                //Debug.DrawRay( ray.origin, ray.direction, Color.red, 10f, true );
                #endif

                if( Physics.Raycast( ray, out hit_info, 100f ) ) { 
                    
                    handle_slide_area = hit_info.collider.GetComponent<HandleSliderArea>();
                }
            }

            if( handle_slide_area != null ) { 
                        
                hit_position = hit_info.point;
            }            

            return hit_position;
        }

        // ################################################################################################################################################################################################################################################
        public void ReleaseSliderHandle() { 
            
            update_slider = false;

            drag_handle_sphere = false;
            
            Vector3 handle_position = GetHitPosition( handle_rect_transform.position );

            UpdateSliderValue( handle_position );

            video_player.time = slider_control.value;

            if( !video_player.isPlaying && pause_button.gameObject.activeSelf ) { 
            
                Play();
            }

            if( (NetworkProjectManager.Instance != null) && PhotonNetwork.isMasterClient ) { 
            
                if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                    NetworkTransactionsControlTools.Instance.PlayVideoFromTime( VertexStudio.Networking.NetworkPlayer.Instance.View_ID, slider_control.value, SceneManager.GetActiveScene().name );
                }
            }
        }

        // ################################################################################################################################################################################################################################################
        public void OnClickSliderHandle() {

            if( !SteamVR.active ) { 
            
                video_player.time = slider_control.value;

                return;
            }

            else {

                ViveInteractionsManager.Instance.Vive_pointer_right.Input_manager.Hovered_video_control_handle = this;

                drag_handle_sphere = true;

                Pause();
            }
        }

        // ################################################################################################################################################################################################################################################
        public void OnClickSlideArea() { 

            update_slider = false;

            Vector3 handle_position = GetHitPosition( handle_rect_transform.position );

            UpdateSliderValue( handle_position );

            video_player.time = slider_control.value;

            drag_handle_sphere = false;

            if( !video_player.isPlaying && pause_button.gameObject.activeSelf ) { 
            
                Play();
            }
        }
    }
}