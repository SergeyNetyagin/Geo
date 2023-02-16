using UnityEngine;
using UnityEngine.SceneManagement;
using VertexStudio.Networking;

namespace VostokVR.Geo {

    public class NetworkTransactionsControlTools : NetworkTransactionsControl {

        public static NetworkTransactionsControlTools Instance { get; private set; }

	    // Use this for initialization #########################################################################################################################################################################################################################
	    protected override void Awake() {
		
            Instance = this;

            base.Awake();
	    }
	
	    // Use this for initialization #########################################################################################################################################################################################################################
	    protected override void Start() {
		
            base.Start();
	    }

        // Set hotspots panel active or inactive ###############################################################################################################################################################################################################
        public void SetHotspotsPanelActive( int view_ID, bool activate, string scene_name ) {
         
            photonView.RPC( "RPC_SetHotspotsPanelActive", PhotonTargets.Others, view_ID, activate, scene_name );
        }

        // Set hotspots panel active or inactive ###############################################################################################################################################################################################################
        [PunRPC]
        private void RPC_SetHotspotsPanelActive( int view_ID, bool activate, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {
            
                if( activate ) { 
                
                    CanvasController.Instance.ShowHotspotsPanel();
                }

                else { 
                
                    CanvasController.Instance.HideHotspotsPanel();
                }
            }
        }

        // Plays current video #################################################################################################################################################################################################################################
        public void PlayVideo( int view_ID, string scene_name ) {
            
            photonView.RPC( "RPC_PlayVideo", PhotonTargets.Others, view_ID, scene_name );
        }

        // Plays current video #################################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_PlayVideo( int view_ID, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                VideoControl video_control = CanvasHotspots.Instance.GetComponentInChildren<VideoControl>( false );

                if( video_control != null ) { 

                    #if( UNITY_EDITOR )
                    Debug.Log( "<PLAY> command received for " + video_control.GetComponentInParent<Hotspot>() );
                    #endif
                
                    video_control.Play();
                }
            }
        }

        // Pauses current video ################################################################################################################################################################################################################################
        public void PauseVideo( int view_ID, string scene_name ) {
            
            photonView.RPC( "RPC_PauseVideo", PhotonTargets.Others, view_ID, scene_name );
        }

        // Pauses current video ################################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_PauseVideo( int view_ID, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                VideoControl video_control = CanvasHotspots.Instance.GetComponentInChildren<VideoControl>( false );

                if( video_control != null ) { 

                    #if( UNITY_EDITOR )
                    Debug.Log( "<PAUSE> command received for " + video_control.GetComponentInParent<Hotspot>() );
                    #endif
                    
                    video_control.Pause();
                }
            }
        }

        // Replays current video ###############################################################################################################################################################################################################################
        public void ReplayVideo( int view_ID, string scene_name ) {
            
            photonView.RPC( "RPC_ReplayVideo", PhotonTargets.Others, view_ID, scene_name );
        }

        // Replays current video ###############################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_ReplayVideo( int view_ID, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                VideoControl video_control = CanvasHotspots.Instance.GetComponentInChildren<VideoControl>( false );

                if( video_control != null ) { 
                
                    #if( UNITY_EDITOR )
                    Debug.Log( "<REPLAY> command received for " + video_control.GetComponentInParent<Hotspot>() );
                    #endif

                    video_control.Replay();
                }
            }
        }

        // Playes video from the specified time ################################################################################################################################################################################################################
        public void PlayVideoFromTime( int view_ID, float video_time, string scene_name ) {
            
            photonView.RPC( "RPC_PlayVideoFromTime", PhotonTargets.Others, view_ID, video_time, scene_name );
        }

        // Playes video from the specified time ################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_PlayVideoFromTime( int view_ID, float video_time, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                VideoControl video_control = CanvasHotspots.Instance.GetComponentInChildren<VideoControl>( false );

                if( video_control != null ) { 
                
                    #if( UNITY_EDITOR )
                    Debug.Log( "<UPDATE VIDEO PLAYER AND SLIDER POSITION> command received for " + video_control.GetComponentInParent<Hotspot>() );
                    #endif

                    video_control.SetPlayerValue( video_time );
                    video_control.SetSliderValue( video_time );
                }
            }
        }

        // Sets video slider position ##########################################################################################################################################################################################################################
        public void SetVideoSliderValue( int view_ID, float slider_value, string scene_name ) {
            
            photonView.RPC( "RPC_SetVideoSliderValue", PhotonTargets.Others, view_ID, slider_value, scene_name );
        }

        // Sets video slider position ##########################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_SetVideoSliderValue( int view_ID, float slider_value, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                VideoControl video_control = CanvasHotspots.Instance.GetComponentInChildren<VideoControl>( false );

                if( video_control != null ) { 
                
                    #if( UNITY_EDITOR )
                    Debug.Log( "<SET SLIDER VALUE> command received for " + video_control.GetComponentInParent<Hotspot>() );
                    #endif

                    video_control.SetSliderValue( slider_value );
                }
            }
        }

        // Set the specified guide type ########################################################################################################################################################################################################################
        public void SetGuide( int view_ID, bool active, int page_index, string scene_name ) {
         
            photonView.RPC( "RPC_SetGuide", PhotonTargets.Others, view_ID, active, page_index, scene_name );
        }

        // Set the specified guide type ########################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_SetGuide( int view_ID, bool active, int page_index, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                if( active ) { 
                    
                    CanvasController.Instance.ShowFieldGuide( page_index );
                }

                else { 
                
                    CanvasController.Instance.HideFieldGuide();
                }
            }
        }

        // Set the specified environment type ##################################################################################################################################################################################################################
        public void SetEnvironment( int view_ID, EnvironmentType environment_type, string scene_name ) {
         
            photonView.RPC( "RPC_SetEnvironment", PhotonTargets.Others, view_ID, (int) environment_type, scene_name );
        }

        // Set the specified environment type ##################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_SetEnvironment( int view_ID, int environment_ID, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {
            
                EnvironmentType environment_type = (EnvironmentType) environment_ID;

                if( environment_type == EnvironmentType.Rocks ) { 
                    
                    CanvasController.Instance.ShowRocks();
                }

                else if( environment_type == EnvironmentType.Panorama ) { 
                    
                    CanvasController.Instance.ShowPanorama();
                }
            }
        }

        // Set scale meter active or inactive ##################################################################################################################################################################################################################
        public void SetScaleToolActive( int view_ID, bool activate, string scene_name ) {
         
            photonView.RPC( "RPC_SetScaleToolActive", PhotonTargets.Others, view_ID, activate, scene_name );
        }

        // Set scale meter active or inactive ##################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_SetScaleToolActive( int view_ID, bool activate, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {
            
                if( activate ) { 
                
                    CanvasController.Instance.ShowStaticScalers();
                }

                else { 
                
                    CanvasController.Instance.HideStaticScalers();
                }
            }
        }

        // Opens the specified hotspot #########################################################################################################################################################################################################################
        public void ActivateHotspot( int view_ID, int hotspot_ID ) {
         
            photonView.RPC( "RPC_ActivateHotspot", PhotonTargets.Others, view_ID, hotspot_ID );
        }

        // Opens the specified hotspot #########################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_ActivateHotspot( int view_ID, int hotspot_ID ) {

            if( CanvasHotspots.Instance == null ) { 
            
                return;
            }

            Hotspot hotspot = CanvasHotspots.Instance.GetHotspot( hotspot_ID );

            if( hotspot != null ) { 
            
                hotspot.Activate();
            }
        }

        // Closes the specified hotspot #########################################################################################################################################################################################################################
        public void DeactivateHotspot( int view_ID, int hotspot_ID ) {
            
            photonView.RPC( "RPC_DeactivateHotspot", PhotonTargets.Others, view_ID, hotspot_ID );
        }

        // Closes the specified hotspot #########################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_DeactivateHotspot( int view_ID, int hotspot_ID ) {

            if( CanvasHotspots.Instance == null ) { 
            
                return;
            }

            Hotspot hotspot = CanvasHotspots.Instance.GetHotspot( hotspot_ID );

            if( hotspot != null ) { 
            
                hotspot.Deactivate();
            }
        }
    }
}