using UnityEngine;

namespace SvetoforGroup.PddVR {

    public class VRMotionEmulator : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private float rotation_margin = 45;

        private bool is_dragging = false;
        private Vector2 start_mouse_position;
        private Camera main_camera;


        /// <summary>
        /// Use this for initialization.
        /// </summary>
        private void Awake() {

        }


        /// <summary>
        /// Use this for initialization.
        /// </summary>
        private void Start() {

            main_camera = GetComponentInChildren<Camera>();

            if( main_camera == null ) { 
            
                Debug.LogError( "Cannot found a camera to emulate VR mode!" );
            }

            if( SteamVR.enabled ) { 
            
                enabled = false;
            }
        }
  

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        private void Update() {
        
            ReticleMotionForFlatUI();
        }


        /// <summary>
        /// Emulates a reticle motion in editor mode.
        /// </summary>
        private void ReticleMotionForFlatUI() {

            if( Input.GetMouseButtonDown( 0 ) && !is_dragging ) {

                is_dragging = true;

                start_mouse_position.x = Input.mousePosition.x;
                start_mouse_position.y = Input.mousePosition.y;

                Vector2 end_mouse_position = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
                Vector2 delta_position = new Vector2( end_mouse_position.x - start_mouse_position.x, end_mouse_position.y - start_mouse_position.y );
                Vector2 center_position = new Vector2( Screen.width * 0.5f + delta_position.x, Screen.height * 0.5f + delta_position.y );

                Vector3 look_point = main_camera.ScreenToWorldPoint( new Vector3( center_position.x, center_position.y, main_camera.nearClipPlane ) );

                transform.LookAt( look_point );

                start_mouse_position = end_mouse_position;
            }

            else if( Input.GetMouseButtonUp( 0 ) && is_dragging ) {

                is_dragging = false;
            }
        }
    }
}