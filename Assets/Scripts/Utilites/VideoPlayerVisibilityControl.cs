using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace VostokVR.Geo {

    public class VideoPlayerVisibilityControl : MonoBehaviour {

        private static List<Camera> visible_cameras = new List<Camera>();

        private Camera eye_camera;

        private VideoPlayer video_player;

	    // Use this for initialization #####################################################################################################################################################################################################################
	    private void Awake() {

            //eye_camera = ControllerData.Instance.Eye_camera_transform.GetComponent<Camera>();
            video_player = GetComponent<VideoPlayer>();
        }

	    // Use this for initialization #####################################################################################################################################################################################################################
	    private void Start() {

        }

        // On will render ##################################################################################################################################################################################################################################
        private void OnWillRenderObject() {

            if( Camera.current == null ) return;
//Debug.Log( "Register camera: " + Camera.current );
//            visible_cameras.Add( Camera.current );
        }

        // On became visible ###############################################################################################################################################################################################################################
        private void OnBecameVisible() {

            if( video_player == null ) return;
//Debug.Log( "VISIBLE >>>>>>>>>>>>>>>>>>>>>>>> " + Camera.current );
//            if( visible_cameras.Contains( eye_camera ) ) {
//Debug.Log( "Enabled for ---> " + eye_camera );

                if( !video_player.isPlaying ) video_player.Play();

//                visible_cameras.Remove( eye_camera );
//            }
        }

        // On became invisible #############################################################################################################################################################################################################################
        private void OnBecameInvisible() {
        
            if( video_player == null ) return;
//Debug.Log( "invisible " + Camera.current );
//            if( !visible_cameras.Contains( eye_camera ) ) {
//Debug.Log( "Disnabled for ---> " + eye_camera );

                if( video_player.isPlaying ) video_player.Pause();

//                visible_cameras.Remove( eye_camera );
//            }
        }

        // On render object ################################################################################################################################################################################################################################
        private void OnRenderObject() {
            
//            visible_cameras.Clear();
        }
    }
}