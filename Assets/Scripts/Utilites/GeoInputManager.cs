using UnityEngine;
using UnityEngine.SceneManagement;
using VertexStudio.VirtualRealty;
using VertexStudio.Generic;

namespace VostokVR.Geo {

    public class GeoInputManager : StandaloneInputManager {

        public static GeoInputManager Instance { get; private set; }

        [Space( 10 ), SerializeField]
        protected KeyCode key_menu = KeyCode.M;

        [SerializeField]
        protected KeyCode key_instructions = KeyCode.I;

        [SerializeField]
        protected KeyCode key_screenshot = KeyCode.P;

        [Space( 10 ), SerializeField]
        protected KeyCode key_fast_loading_hospital_road = KeyCode.F5;

        [SerializeField]
        protected KeyCode key_fast_loading_bukit_song = KeyCode.F6;

        [SerializeField]
        protected KeyCode key_fast_loading_tusan_a = KeyCode.F7;

        [SerializeField]
        protected KeyCode key_fast_loading_tusan_b = KeyCode.F8;

        [Space( 10 ), SerializeField]
        private bool use_keyboard_in_vr_mode = true;

        protected Vector3 hit_point = new Vector3( float.MinValue, float.MinValue, float.MinValue );

        protected Transform standalone_eraser_pointer;

        protected PaintingEraser painting_eraser;
        public PaintingEraser Painting_eraser { get { return painting_eraser; } }


        /// <summary>
        /// Use this for initialization.
        /// </summary>
        protected override void Awake() {
            
            Instance = this;

            base.Awake();
        }


        /// <summary>
        /// Use this for initialization.
        /// </summary>
        protected override void Start() {
		
            base.Start();

            if( !SteamVR.active && (SceneManager.GetActiveScene().name != ScenesManager.Instance.Start_scene_name) && (SceneManager.GetActiveScene().name != ScenesManager.Instance.Lobby_scene_name) ) {

                standalone_eraser_pointer = GameObject.CreatePrimitive( PrimitiveType.Sphere ).GetComponent<Transform>();
                standalone_eraser_pointer.name = "Standalone Eraser Pointer";
                standalone_eraser_pointer.transform.SetParent( (PaintingControl.Instance != null) ? PaintingControl.Instance.Painting_group_local.transform : null, false );
                standalone_eraser_pointer.localScale = new Vector3( PaintingControl.Instance.Eraser_cell_size, PaintingControl.Instance.Eraser_cell_size, PaintingControl.Instance.Eraser_cell_size );
                standalone_eraser_pointer.gameObject.SetActive( false );

                Rigidbody eraser_rigidbody = standalone_eraser_pointer.gameObject.AddComponent<Rigidbody>();

                if( eraser_rigidbody != null ) {
                
                    eraser_rigidbody.useGravity = false;
                    eraser_rigidbody.isKinematic = true;
                }

                Collider eraser_collider = standalone_eraser_pointer.gameObject.GetComponent<Collider>();

                if( eraser_collider != null ) { 
                
                    eraser_collider.isTrigger = true;
                    eraser_collider.enabled = false;
                }

                painting_eraser = standalone_eraser_pointer.gameObject.AddComponent<PaintingEraser>();

                if( painting_eraser != null ) { 
                
                    painting_eraser.Eraser_transform = standalone_eraser_pointer.transform;
                    painting_eraser.SetRigidbody( eraser_rigidbody );
                    painting_eraser.SetCollider( eraser_collider );
                }
            }
	    }


        /// <summary>
        /// LateUpdate is called once per frame.
        /// </summary>
        protected override void LateUpdate() {

            if( SteamVR.active && enabled && !use_keyboard_in_vr_mode ) { 
            
                enabled = false;

                return;
            }

            if( !ScenesManager.Instance.Is_working ) {
                
                return;
            }

            if( use_keyboard_in_vr_mode || !SteamVR.active ) {

                //if( (LevelManagerMain.Instance != null) && (LevelManagerMain.Instance.Canvas_controller_mode == CanvasControllerMode.Hidden) ) { 
                if( LevelManagerMain.Instance != null ) { 

                    if( Input.GetKeyDown( key_fast_loading_hospital_road ) ) { 
                    
                        CanvasController.Instance.Loading_scene_name = ScenesManager.Instance.GetSceneNameByIndex( 3 );
                        CanvasController.Instance.LoadPredefinedSceneSingle();
                    }
            
                    if( Input.GetKeyDown( key_fast_loading_bukit_song ) ) { 
                    
                        CanvasController.Instance.Loading_scene_name = ScenesManager.Instance.GetSceneNameByIndex( 2 );
                        CanvasController.Instance.LoadPredefinedSceneSingle();
                    }

                    else if( Input.GetKeyDown( key_fast_loading_tusan_a ) ) { 
                    
                        CanvasController.Instance.Loading_scene_name = ScenesManager.Instance.GetSceneNameByIndex( 4 );
                        CanvasController.Instance.LoadPredefinedSceneSingle();
                    }

                    else if( Input.GetKeyDown( key_fast_loading_tusan_b ) ) { 
                    
                        CanvasController.Instance.Loading_scene_name = ScenesManager.Instance.GetSceneNameByIndex( 5 );
                        CanvasController.Instance.LoadPredefinedSceneSingle();
                    }
                }
            
                ProcessKeyboardInput();
                ProcessMouseInput();
            }
        }


        /// <summary>
        /// For non-VR-mode only: keyboard input processing.
        /// </summary>
        protected override void ProcessKeyboardInput() {

            base.ProcessKeyboardInput();

            if( Input.GetKeyDown( key_menu ) ) {

                CanvasController.Instance.gameObject.SetActive( !CanvasController.Instance.gameObject.activeSelf );
            }

            else if( Input.GetKeyDown( key_instructions ) ) {

                CanvasController.Instance.SwitchInstructionsPanel();
            }

            else if( Input.GetKeyDown( key_screenshot ) ) {

                CanvasController.Instance.MakeScreenshot();
            }
        }


        /// <summary>
        /// For non-VR mode only: mouse input processing.
        /// </summary>
        protected override void ProcessMouseInput() {

            // Disable painting or erasing mode
            if( (CanvasController.Instance != null) && Input.GetMouseButtonUp( 2 ) ) {

                if( CanvasController.Instance.Current_work_mode == ActiveMode.Painting ) { 

                    PaintingControl.Instance.PausePaint();

                    Resources.UnloadUnusedAssets();
                }

                if( (standalone_eraser_pointer != null) && standalone_eraser_pointer.gameObject.activeSelf ) { 
                    
                    standalone_eraser_pointer.gameObject.SetActive( false );
                }
            }

            // Enable painting or erasing mode
            else if( (CanvasController.Instance != null) && Input.GetMouseButton( 2 ) ) {

                RaycastHit hit;

                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

                if( Physics.Raycast( ray, out hit, 100f, ProjectData.Instance.Player_motion_check_mask, QueryTriggerInteraction.Ignore ) ) {

                    hit_point = hit.point;

                    // Painting
                    if( CanvasController.Instance.Current_work_mode == ActiveMode.Painting ) { 
                
                        PaintingControl.Instance.PaintLine( hit_point );
                    }

                    // Erasing
                    else if( CanvasController.Instance.Current_work_mode == ActiveMode.Erasing ) { 

                        standalone_eraser_pointer.position = hit_point;               

                        if( !standalone_eraser_pointer.gameObject.activeSelf ) { 
                    
                            standalone_eraser_pointer.gameObject.SetActive( true );
                        }

                        PaintingControl.Instance.ErasePixels();
                    }
                }
            }

            if( !use_rotation ) { 
            
                return;
            }

            if( Input.GetMouseButtonDown( 0 ) ) {
                
                mouse_position = Input.mousePosition;
            }

            mouse_position -= Input.mousePosition;

            if( Input.GetMouseButton( 0 ) && (mouse_position != Vector3.zero ) ) {

                ViveInteractionsManager.Instance.Camera_rig_transform.Rotate( 0f, (- mouse_position.x * mouse_rotation_speed * Time.deltaTime), 0f, Space.Self );
                ViveInteractionsManager.Instance.Eye_camera_transform.Rotate( (mouse_position.y * mouse_rotation_speed * Time.deltaTime), 0f, 0f, Space.Self );
            }

            mouse_position = Input.mousePosition;
        }
    }
}