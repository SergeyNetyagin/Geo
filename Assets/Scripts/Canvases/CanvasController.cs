using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VertexStudio.VirtualRealty;
using VertexStudio.Generic;
using VertexStudio.Networking;

namespace VostokVR.Geo {

    public enum ActiveMode { 
    
        None,
        Movement,
        Painting,
        Measuring,
        Scaling,
        Erasing,
        Shooting,
        Angle,
        Liquid,
        Hotspots,
        Minimap,
        Guide,
        Panorama
    }

    public enum MenuMode {

        Invisible,
        Visible
    }

    public enum MinimapMode {

        Hidden,
        Local,
        Global
    }

    [System.Serializable]
    public class ButtonControl { 

        public Button Button;
        public RectTransform Outline;
        public ActiveMode Button_mode = ActiveMode.None;
    }

    public class CanvasController : MonoBehaviour {

        public static CanvasController Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private RectTransform canvas_rect_transform;

        [SerializeField]
        private Transform canvas_transform;

        [Space( 10 ), SerializeField]
        private GameObject panel_menu;

        [SerializeField]
        private GameObject panel_main_buttons;

        [SerializeField]
        private GameObject panel_minimaps;

        [SerializeField]
        private GameObject panel_painting;

        [SerializeField]
        private GameObject panel_dialog_load_scene;

        [SerializeField]
        private GameObject panel_dialog_load_sketch;

        [SerializeField]
        private GameObject panel_instructions;

        [Space( 10 ), SerializeField]
        private Button button_load_group;

        [SerializeField]
        private Text text_load_single;

        [Space( 10 ), SerializeField]
        private GameObject panel_pc_menu;

        [SerializeField]
        private Button button_pc_menu_collapse;

        [SerializeField]
        private Button button_pc_menu_expand;

        [Space( 10 ), SerializeField]
        private Button button_sketch_group;

        [SerializeField]
        private Text text_sketch_single;

        [Space( 10 ), SerializeField]
        private GameObject image_player;

        [SerializeField]
        private GameObject image_global_map;

        [SerializeField]
        private GameObject image_local_map;

        [Space( 10 ), SerializeField]
        private MenuMode starting_menu_mode = MenuMode.Visible;

        [SerializeField]
        private MinimapMode starting_minimap_mode = MinimapMode.Hidden;
        private MinimapMode current_minimap_mode = MinimapMode.Hidden;

        [Space( 10 ), SerializeField]
        private ButtonControl[] main_buttons;

        [Space( 10 ), SerializeField]
        private Image image_make_me_master_client;

        [SerializeField]
        private Image image_i_am_master_client;

        [Space( 10 ), SerializeField]
        private ButtonControl[] color_buttons;

        [Space( 10 ), SerializeField]
        private ButtonControl button_eraser;

        [Space( 10 ), SerializeField]
        private ButtonControl button_screenshot;

        [Space( 10 ), SerializeField]
        private Vector3 global_minimap_start_position = new Vector3( 0f, 0.0255f, 0f );

        [SerializeField]
        private Vector3 global_minimap_common_position = new Vector3( 0f, 0.077f, 0f );

        [Space( 10 ), SerializeField]
        private Button button_measuring;

        [SerializeField]
        private Button button_angle_meter;

        [Space( 10 ), SerializeField]
        private Image instruction_htc_vive;

        [SerializeField]
        private Image instruction_oculus_rift;

        [SerializeField]
        private Image instruction_pc_desktop;

        [SerializeField]
        private Image instruction_menu_vr;

        [SerializeField]
        private Image instruction_menu_pc;

        [Space( 10 ), SerializeField]
        private Vector3 non_vr_mode_local_position = new Vector3( -0.1f, 1.6f, 0.12f );

        [SerializeField]
        private Vector3 non_vr_mode_local_rotation = new Vector3( 0f, 0f, 0f );

        [SerializeField]
        private Vector3 non_vr_mode_local_scale = new Vector3( 1f, 1f, 1f );

        [Space( 10 ), SerializeField]
        private Vector3 vr_mode_local_position = new Vector3( 0f, 0.1f, 0.05f );

        [SerializeField]
        private Vector3 vr_mode_local_rotation = new Vector3( 45f, 0f, 0f );

        [SerializeField]
        private Vector3 vr_mode_local_scale = new Vector3( 1f, 1f, 1f );

        [Space( 10 ), SerializeField]
        private Vector3 small_mode_local_position = new Vector3( -8.185f, 1.6025f, 2.4045f );

        [SerializeField]
        private Vector3 small_mode_local_rotation = new Vector3( 0f, 115f, 0f );

        [SerializeField]
        private Vector3 small_mode_local_scale = new Vector3( 0.5f, 0.5f, 0.5f );

        [Space( 10 ), SerializeField]
        private CanvasGroup panel_message;

        [SerializeField]
        private Text text_message;

        [SerializeField, Range( 1, 10 )]
        private float message_timeout = 5;

        [SerializeField]
        private bool use_messages = true;

        private Coroutine message_coroutine = null;

        private ActiveMode current_work_mode = ActiveMode.Movement;
        public ActiveMode Current_work_mode { get { return current_work_mode; } set { current_work_mode = value; } }

        public ButtonControl Selected_main_button { get; private set; }
        public ButtonControl Selected_color_button { get; private set; }
        
        private Vector3 position = Vector3.zero;        
        private Vector3 rotation = Vector3.zero;        

        private Vector2 milestone_scale_ratio = Vector2.zero;

        private string loading_scene_name = string.Empty;
        public string Loading_scene_name { get { return loading_scene_name; } set { loading_scene_name = value; } }

        private MinimapControl minimap_control;
        private RealmapControl realmap_control;

        private Transform first_milestone_transform;
        private Transform second_milestone_transform;
        private Transform player_transform;

        private Transform real_space_transform;

        public bool PC_mode { get; private set; } = false;


        // Awake is called before the first frame update ##################################################################################################################################################################################################
        private void Awake() {

            Instance = this;

            current_minimap_mode = starting_minimap_mode;

            panel_minimaps.SetActive( starting_minimap_mode == MinimapMode.Global );

            panel_main_buttons.SetActive( starting_menu_mode == MenuMode.Visible );
            panel_dialog_load_scene.SetActive( false );
            panel_painting.SetActive( false );
            panel_pc_menu.SetActive( false );

            image_global_map.SetActive( starting_minimap_mode == MinimapMode.Local );
            image_local_map.SetActive( starting_minimap_mode == MinimapMode.Global );

            panel_message.gameObject.SetActive( false );
            panel_message.alpha = 0;
        }

        // OnEnable is called before the first frame update ###############################################################################################################################################################################################
        private void OnEnable() {

            Activate();

            if( gameObject.activeInHierarchy ) {
            
                StartCoroutine( CheckForMasterClient() );
            }
        }

        // Start is called before the first frame update ##################################################################################################################################################################################################
        private void Start() {

            SceneAttribute[] mm_scene_attributes = panel_minimaps.GetComponentsInChildren<SceneAttribute>( true );
            SceneAttribute[] rm_scene_attributes = (SceneManager.GetActiveScene().name == ScenesManager.Instance.Start_scene_name) ? null : NavigationControl.Instance.GetComponentsInChildren<SceneAttribute>( true );

            MinimapControl mm_control = null;
            RealmapControl rm_control = null;

            if( SceneManager.GetActiveScene().name != ScenesManager.Instance.Start_scene_name ) {

                for( int i = 0; i < mm_scene_attributes.Length; i++ ) {
            
                    if( SceneManager.GetActiveScene().name == mm_scene_attributes[i].Scene_name ) { 
                 
                        mm_control = mm_scene_attributes[i].GetComponent<MinimapControl>();

                        break;
                    }
                }

                for( int i = 0; i < rm_scene_attributes.Length; i++ ) {
            
                    if( SceneManager.GetActiveScene().name == rm_scene_attributes[i].Scene_name ) { 
                 
                        rm_control = rm_scene_attributes[i].GetComponent<RealmapControl>();

                        break;
                    }
                }
            }

            if( starting_menu_mode == MenuMode.Visible ) {

                DeselectAllMainButtons();

                SetButtonState( ActiveMode.Hotspots, CanvasHotspots.Instance.Panel_all_scenes_hotspots.gameObject.activeSelf, ActiveMode.Movement );
                SetButtonState( ActiveMode.Minimap, panel_minimaps.activeSelf, ActiveMode.Movement );
                SetButtonState( ActiveMode.Guide, CanvasFiledGuide.Instance.Panel_guide.activeSelf, ActiveMode.Movement );

                for( int i = 0; i < color_buttons.Length; i++ ) { 
            
                    if( i == 0 ) { 
                
                        color_buttons[i].Outline.gameObject.SetActive( true );

                        Selected_color_button = color_buttons[i];
                    }

                    else { 

                        color_buttons[i].Outline.gameObject.SetActive( false );
                    }
                }

                if( PaintingControl.Instance != null ) { 
                
                    PaintingControl.Instance.Current_paint = PaintingControl.Instance.GetPaint( Selected_color_button.Button.GetComponent<PaintingColorButton>().Selected_color );
                }
            }

            Current_work_mode = ActiveMode.Movement;

            if( SceneManager.GetActiveScene().name == ScenesManager.Instance.Start_scene_name ) {

                panel_minimaps.GetComponent<RectTransform>().anchoredPosition3D = global_minimap_start_position;

                image_player.SetActive( false );
            }

            else {

                panel_minimaps.GetComponent<RectTransform>().anchoredPosition3D = global_minimap_common_position;
                            
                PrepareNavigationSystem( mm_control, rm_control );
            }

            if( SteamVR.active ) { 
                
                VivePointerLeft.Instance.Touch_pointer_trigger.GetComponent<PaintingEraser>().DisableCollider();
                VivePointerRight.Instance.Touch_pointer_trigger.GetComponent<PaintingEraser>().DisableCollider();
            }
        }

        // Update is called once per frame ################################################################################################################################################################################################################
        private void Update() {

            if( image_player.activeInHierarchy ) {

                if( realmap_control == null ) { 
                
                    #if( UNITY_EDITOR )
                    Debug.LogError( "Realmap_control is null!" );
                    #endif

                    return;
                }

                if( first_milestone_transform == null ) { 
                
                    #if( UNITY_EDITOR )
                    Debug.LogError( "First_milestone_transform is null!" );
                    #endif

                    return;
                }

                if( player_transform == null ) { 
                
                    #if( UNITY_EDITOR )
                    Debug.LogError( "Player_transform is null!" );
                    #endif

                    return;
                }

                #if( UNITY_EDITOR )
                PrepareNavigationSystem( minimap_control, realmap_control );
                #endif
                
                Vector2 map_distance_from_first_milestone_to_player = Vector2.zero;

                if( SteamVR.active ) {

                    map_distance_from_first_milestone_to_player.x = (
                
                        (ViveInteractionsManager.Instance.Eye_camera_transform.position.x - realmap_control.Real_first_milestone_transform.position.x) * 
                        milestone_scale_ratio.x
                    );

                    map_distance_from_first_milestone_to_player.y = (
                
                        (ViveInteractionsManager.Instance.Eye_camera_transform.position.z - realmap_control.Real_first_milestone_transform.position.z) * 
                        milestone_scale_ratio.y
                    );
                }

                else { 
                    
                    map_distance_from_first_milestone_to_player.x = (
                
                        (ViveInteractionsManager.Instance.Camera_rig_transform.position.x - realmap_control.Real_first_milestone_transform.position.x) * 
                        milestone_scale_ratio.x
                    );

                    map_distance_from_first_milestone_to_player.y = (
                
                        (ViveInteractionsManager.Instance.Camera_rig_transform.position.z - realmap_control.Real_first_milestone_transform.position.z) * 
                        milestone_scale_ratio.y
                    );
                }

                position.x = first_milestone_transform.localPosition.x + map_distance_from_first_milestone_to_player.x;
                position.y = first_milestone_transform.localPosition.y + map_distance_from_first_milestone_to_player.y;
                position.z = 0f;

                player_transform.localPosition = position;

                Vector3 view_marker_rotation = Vector3.zero;

                if( SteamVR.active ) {

                    view_marker_rotation.z = (
                        
                        (- ViveInteractionsManager.Instance.Eye_camera_transform.localEulerAngles.y) -
                        (ViveInteractionsManager.Instance.Camera_rig_transform.localEulerAngles.y)
                    );
                }

                else { 
                    
                    view_marker_rotation.z = (

                        (- ViveInteractionsManager.Instance.Eye_camera_transform.localEulerAngles.y) -
                        (ViveInteractionsManager.Instance.Camera_rig_transform.localEulerAngles.y)
                    );
                }

                player_transform.localEulerAngles = view_marker_rotation;
            }       
        }

        // Activates menu #################################################################################################################################################################################################################################
        public void Activate() { 

            if( !gameObject.activeSelf ) {

                gameObject.SetActive( true );
            }

            LevelManagerMain level_manager_main = LevelManagerMain.Instance;

            if( LevelManagerMain.Instance == null ) {

                level_manager_main = FindObjectOfType<LevelManagerMain>();
            }

            bool activate = SteamVR.active ? true : (level_manager_main.Starting_controller_mode != CanvasControllerMode.Hidden);
            
            PC_mode = !SteamVR.active;

            panel_menu.SetActive( activate && !PC_mode );
            panel_dialog_load_scene.gameObject.SetActive( false );
            panel_dialog_load_sketch.gameObject.SetActive( false );
            panel_pc_menu.SetActive( PC_mode );

            button_pc_menu_collapse.gameObject.SetActive( panel_menu.activeSelf );
            button_pc_menu_expand.gameObject.SetActive( !panel_menu.activeSelf );

            AdaptCanvasForRemote();
        }

        // Deactivates menu ###############################################################################################################################################################################################################################
        public void Deactivate() { 

            panel_menu.SetActive( false );
            panel_dialog_load_scene.SetActive( false );
            panel_dialog_load_sketch.SetActive( false );
            panel_pc_menu.SetActive( false );
        }

        // ################################################################################################################################################################################################################################################
        public void OnClickPCMenuCollapse( Button button ) { 
        
            if( button != null ) { 
            
                button.interactable = false;
                button.interactable = true;
            }

            panel_menu.SetActive( false );

            button_pc_menu_expand.gameObject.SetActive( true );
            button_pc_menu_collapse.gameObject.SetActive( false );
        }

        // ################################################################################################################################################################################################################################################
        public void OnClickPCMenuExpand( Button button ) { 
        
            if( button != null ) { 
            
                button.interactable = false;
                button.interactable = true;
            }

            panel_menu.SetActive( true );

            button_pc_menu_collapse.gameObject.SetActive( true );
            button_pc_menu_expand.gameObject.SetActive( false );
        }

        // Checks for virtual realty presents and adjust menu #############################################################################################################################################################################################
        private IEnumerator CheckForMasterClient() { 

            while( enabled ) { 
            
                if( VertexStudio.Networking.NetworkPlayer.Instance == null ) { 
                    
                    image_make_me_master_client.gameObject.SetActive( false );
                    image_i_am_master_client.gameObject.SetActive( false );
                }

                else if( (NetworkConnectionManager.Instance != null) && panel_main_buttons.activeInHierarchy ) { 
                
                    if( NetworkConnectionManager.Instance.Is_master_client ) { 
                        
                        image_make_me_master_client.gameObject.SetActive( false );
                    }

                    else { 
                        
                        image_make_me_master_client.gameObject.SetActive( true );
                    }

                    image_i_am_master_client.gameObject.SetActive( !image_make_me_master_client.gameObject.activeSelf );
                }

                button_load_group.gameObject.SetActive( image_i_am_master_client.gameObject.activeSelf );
                text_load_single.text = image_i_am_master_client.gameObject.activeSelf ? "SINGLE" : "LOAD";

                button_sketch_group.gameObject.SetActive( image_i_am_master_client.gameObject.activeSelf );
                text_sketch_single.text = image_i_am_master_client.gameObject.activeSelf ? "SINGLE" : "LOAD";

                #if( UNITY_EDITOR )
                //Debug.Log( "Am I master client? " + image_i_am_master_client.gameObject.activeSelf );
                #endif

                yield return new WaitForSeconds( 1f );
            }

            yield break;
        }

        // Sets the necessary instructions page depending on device #######################################################################################################################################################################################
        public void AssignInstructionsDependingOnDevice() { 

            button_measuring.gameObject.SetActive( ViveInteractionsManager.Instance.Device_model != DeviceModel.Standalone );
            button_angle_meter.gameObject.SetActive( ViveInteractionsManager.Instance.Device_model != DeviceModel.Standalone );

            if( ViveInteractionsManager.Instance.Device_model == DeviceModel.HTC_Vive ) { 
             
                instruction_htc_vive.gameObject.SetActive( true );
                instruction_oculus_rift.gameObject.SetActive( false );
                instruction_pc_desktop.gameObject.SetActive( false );
                instruction_menu_vr.gameObject.SetActive( true );
                instruction_menu_pc.gameObject.SetActive( false );
            }

            else if( ViveInteractionsManager.Instance.Device_model == DeviceModel.Oculus_Rift ) { 
                
                instruction_htc_vive.gameObject.SetActive( false );
                instruction_oculus_rift.gameObject.SetActive( true );
                instruction_pc_desktop.gameObject.SetActive( false );
                instruction_menu_vr.gameObject.SetActive( true );
                instruction_menu_pc.gameObject.SetActive( false );
            }

            else if( ViveInteractionsManager.Instance.Device_model == DeviceModel.Oculus_Quest ) { 
                
                instruction_htc_vive.gameObject.SetActive( false );
                instruction_oculus_rift.gameObject.SetActive( true );
                instruction_pc_desktop.gameObject.SetActive( false );
                instruction_menu_vr.gameObject.SetActive( true );
                instruction_menu_pc.gameObject.SetActive( false );
            }

            else { 
            
                instruction_htc_vive.gameObject.SetActive( false );
                instruction_oculus_rift.gameObject.SetActive( false );
                instruction_pc_desktop.gameObject.SetActive( true );
                instruction_menu_vr.gameObject.SetActive( false );
                instruction_menu_pc.gameObject.SetActive( true );
            }
        }

        // Prepares navigation system #####################################################################################################################################################################################################################
        private void PrepareNavigationSystem( MinimapControl mm_control, RealmapControl rm_control ) { 

            if( (mm_control == null) || (rm_control == null) ) { 
            
                return;
            }

            else { 
            
                minimap_control = mm_control;
                realmap_control = rm_control;
            }

            image_player.SetActive( true );

            first_milestone_transform = minimap_control.First_milestone_transform;
            second_milestone_transform = minimap_control.Second_milestone_transform;
            player_transform = minimap_control.Player_position_transform;

            real_space_transform = realmap_control.Real_space_transform;

            first_milestone_transform.gameObject.SetActive( false );
            second_milestone_transform.gameObject.SetActive( false );

            real_space_transform.eulerAngles = Vector3.zero;

            // Get ratio on X
            milestone_scale_ratio.x = Mathf.Abs(
                
                (first_milestone_transform.localPosition.x - second_milestone_transform.localPosition.x) / 
                (realmap_control.Real_first_milestone_transform.position.x - realmap_control.Real_second_milestone_transform.position.x)
            );

            // Get ratio on Y
            milestone_scale_ratio.y = Mathf.Abs(
                
                (first_milestone_transform.localPosition.y - second_milestone_transform.localPosition.y) / 
                (realmap_control.Real_first_milestone_transform.position.z - realmap_control.Real_second_milestone_transform.position.z)
            );

            position = player_transform.localPosition;
            rotation = player_transform.localEulerAngles;
        }

        // Switch minmap panel ############################################################################################################################################################################################################################
        public void SwitchMinimapPanel() { 

            if( panel_minimaps.activeSelf ) { 
                
                panel_minimaps.SetActive( false );
            }

            else { 

                panel_minimaps.SetActive( true );
                panel_minimaps.GetComponent<SceneAttributeControl>().ActivateObject( SceneManager.GetActiveScene().name );

                image_global_map.SetActive( current_minimap_mode == MinimapMode.Local );
                image_local_map.SetActive( current_minimap_mode == MinimapMode.Global );
            }
                
            PrepareNavigationSystem( 
                
                panel_minimaps.GetComponentInChildren<MinimapControl>(), 
                NavigationControl.Instance.GetComponentInChildren<RealmapControl>() 
            );

            SetButtonState( ActiveMode.Minimap, panel_minimaps.activeSelf, ActiveMode.Movement );
        }

        // Switch instructions panel ######################################################################################################################################################################################################################
        public void SwitchInstructionsPanel() { 

            if( panel_instructions.activeSelf ) { 
                
                panel_instructions.SetActive( false );
            }

            else { 

                panel_instructions.SetActive( true );
            }
        }

        // Switch rocks ###################################################################################################################################################################################################################################
        public void SwitchRocks() {

            if( LevelManagerMain.Instance.Rocks.Is_visible ) {

                ShowPanorama();
            }

            else {

                ShowRocks();
            }

            if( (NetworkProjectManager.Instance != null) && PhotonNetwork.isMasterClient ) { 
            
                if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                    NetworkTransactionsControlTools.Instance.SetEnvironment( 
                        
                        VertexStudio.Networking.NetworkPlayer.Instance.View_ID, 
                        LevelManagerMain.Instance.Rocks.Is_visible ? EnvironmentType.Rocks : EnvironmentType.Panorama, 
                        SceneManager.GetActiveScene().name 
                    );
                }
            }
        }

        // Switch panorama ################################################################################################################################################################################################################################
        [System.Obsolete( "This is obsolete method! Do not use this one!" )]
        public void SwitchPanorama() { 

            #if( UNITY_EDITOR )
            Debug.LogError( "This is obsolete method! Do not use this one!" );
            #endif

            if( LevelManagerMain.Instance.Rocks.Is_visible ) {

                ShowRocks();
            }

            else {

                ShowPanorama();
            }
        }

        // Shows rocks ####################################################################################################################################################################################################################################
        public void ShowRocks() { 

            CanvasHotspots.Instance.Panel_all_scenes_hotspots.gameObject.SetActive( true );

            LevelManagerMain.Instance.Rocks.ShowRocks();
            LevelManagerMain.Instance.Ground_mesh_renderer.enabled = true;
            LevelManagerMain.Instance.Panorama.gameObject.SetActive( false );

            PaintingControl.Instance.ShowAllPaintings();

            SetButtonState( ActiveMode.Hotspots, CanvasHotspots.Instance.Scene_hotspots_control.gameObject.activeSelf );
            SetButtonState( ActiveMode.Panorama, !LevelManagerMain.Instance.Rocks.Is_visible, ActiveMode.Movement );
        }

        // Shows panorama #################################################################################################################################################################################################################################
        public void ShowPanorama() { 
         
            CanvasHotspots.Instance.Panel_all_scenes_hotspots.gameObject.SetActive( false );

            LevelManagerMain.Instance.Rocks.HideRocks();
            LevelManagerMain.Instance.Ground_mesh_renderer.enabled = (LevelManagerMain.Instance.Panorama is PanoramaPictureControl);
            LevelManagerMain.Instance.Panorama.gameObject.SetActive( true );

            StopScaler( true );
            StopMeasurer( true );
            StopAngleMeter( true );

            PaintingControl.Instance.HideAllPaintings();

            DeactivatePaintingPanel();

            SetButtonState( ActiveMode.Hotspots, CanvasHotspots.Instance.Scene_hotspots_control.gameObject.activeSelf );
            SetButtonState( ActiveMode.Panorama, !LevelManagerMain.Instance.Rocks.Is_visible, ActiveMode.Movement );
        }

        // Switch guide panel #############################################################################################################################################################################################################################
        public void SwitchGuidePanel() { 
           
            if( CanvasFiledGuide.Instance.Panel_guide.activeSelf ) { 
                
                HideFieldGuide();
            }

            else { 

                ShowFieldGuide();
            }

            if( (NetworkProjectManager.Instance != null) && PhotonNetwork.isMasterClient ) { 
            
                if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                    NetworkTransactionsControlTools.Instance.SetGuide( 

                        VertexStudio.Networking.NetworkPlayer.Instance.View_ID, 
                        CanvasFiledGuide.Instance.Panel_guide.activeSelf ? true : false, 
                        CanvasFiledGuide.Instance.Panel_guide.GetComponent<SceneAttributeControl>().GetSceneAttribute().GetComponent<CanvasControllerPanelGuide>().Current_page_index,
                        SceneManager.GetActiveScene().name 
                    );
                }
            }
        }

        // Shows field guide ##############################################################################################################################################################################################################################
        public void ShowFieldGuide( int page_index = -1 ) { 

            CanvasFiledGuide.Instance.Panel_guide.SetActive( true );
            CanvasFiledGuide.Instance.Panel_guide.GetComponent<SceneAttributeControl>().ActivateObject( SceneManager.GetActiveScene().name );

            if( page_index >= 0 ) { 
                
                CanvasFiledGuide.Instance.GoToPage( page_index );
            }

            SetButtonState( ActiveMode.Guide, true, ActiveMode.Movement );
        }

        // Hides field guide ##############################################################################################################################################################################################################################
        public void HideFieldGuide() { 

            CanvasFiledGuide.Instance.Panel_guide.SetActive( false );

            SetButtonState( ActiveMode.Guide, false, ActiveMode.Movement );
        }

        // Switch hotspots panel ##########################################################################################################################################################################################################################
        public void SwitchHotspotsPanel() { 

            if( CanvasHotspots.Instance.Scene_hotspots_control.gameObject.activeSelf ) {

                HideHotspotsPanel();
            }

            else { 

                ShowHotspotsPanel();
            }

            SetButtonState( ActiveMode.Hotspots, CanvasHotspots.Instance.Scene_hotspots_control.gameObject.activeSelf, ActiveMode.Movement );

            if( (NetworkProjectManager.Instance != null) && PhotonNetwork.isMasterClient ) {

                if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                    NetworkTransactionsControlTools.Instance.SetHotspotsPanelActive( 
                        
                        VertexStudio.Networking.NetworkPlayer.Instance.View_ID, 
                        CanvasHotspots.Instance.Scene_hotspots_control.gameObject.activeSelf, 
                        SceneManager.GetActiveScene().name 
                    );
                }
            }
        }

        // Shows hotspots panel ###########################################################################################################################################################################################################################
        public void ShowHotspotsPanel() { 

            CanvasHotspots.Instance.Scene_hotspots_control.gameObject.SetActive( true );
        }

        // Hides hotspots panel ###########################################################################################################################################################################################################################
        public void HideHotspotsPanel() { 

            CanvasHotspots.Instance.Scene_hotspots_control.gameObject.SetActive( false );
        }

        // Switch painting panel ##########################################################################################################################################################################################################################
        public void SwitchPaintingPanel() { 

            if( panel_painting.activeSelf ) { 
                
                DeactivatePaintingPanel();
            }

            else { 

                ActivatePaintingPanel();
            }
        }

        // Activates painting panel #######################################################################################################################################################################################################################
        public void ActivatePaintingPanel() { 
            
            panel_painting.SetActive( true );    

            for( int i = 0; i < main_buttons.Length; i++ ) { 

                if( main_buttons[i].Button_mode == ActiveMode.Painting ) {

                    main_buttons[i].Outline.gameObject.SetActive( true );

                    current_work_mode = ActiveMode.Painting;

                    break;
                }
            }

            button_eraser.Outline.gameObject.SetActive( false );

            if( Selected_color_button != null ) { 
                
                Selected_color_button.Outline.gameObject.SetActive( true );
            }
        }

        // Deactivates painting panel #####################################################################################################################################################################################################################
        public void DeactivatePaintingPanel() { 

            panel_painting.SetActive( false );

            for( int i = 0; i < main_buttons.Length; i++ ) { 

                if( main_buttons[i].Button_mode == ActiveMode.Painting ) {

                    main_buttons[i].Outline.gameObject.SetActive( false );

                    current_work_mode = ActiveMode.Movement;

                    break;
                }
            }
        }

        // Switches meter #################################################################################################################################################################################################################################
        public void SwitchScaler() { 

            if( MeasuringControl.Instance.Scaler_transform.gameObject.activeSelf ) { 
                
                HideStaticScalers();
            }

            else { 
                
                ShowStaticScalers();
            }

            if( (NetworkProjectManager.Instance != null) && PhotonNetwork.isMasterClient ) { 
            
                if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                    NetworkTransactionsControlTools.Instance.SetScaleToolActive( 
                        
                        VertexStudio.Networking.NetworkPlayer.Instance.View_ID, 
                        MeasuringControl.Instance.Scaler_transform.gameObject.activeSelf, 
                        SceneManager.GetActiveScene().name 
                    );
                }
            }
        }

        // Shows static scalers ###########################################################################################################################################################################################################################
        public void ShowStaticScalers() { 

            MeasuringControl.Instance.Scaler_transform.gameObject.SetActive( true );
            MeasuringControl.Instance.Scaler_transform.GetComponent<SceneAttributeControl>().ActivateObject( SceneManager.GetActiveScene().name );

            SetButtonState( ActiveMode.Scaling, true, ActiveMode.Movement );
        }

        // Hides static scalers ###########################################################################################################################################################################################################################
        public void HideStaticScalers() { 

            MeasuringControl.Instance.Scaler_transform.gameObject.SetActive( false );

            SetButtonState( ActiveMode.Scaling, false, ActiveMode.Movement );
        }

        // Starts scaler ##################################################################################################################################################################################################################################
        public void StartScaler() { 

            if( MeasuringControl.Instance.Measurer_line_renderer.gameObject.activeSelf ) {
            
                StopMeasurer( true );
            }

            if( MeasuringControl.Instance.Section_line_renderer.gameObject.activeSelf ) {
            
                StopAngleMeter( true );
            }

            // Set usual mode
            if( MeasuringControl.Instance.Scaler_transform.gameObject.activeSelf ) { 

                for( int i = 0; i < main_buttons.Length; i++ ) { 

                    if( main_buttons[i].Button_mode == ActiveMode.Scaling ) {

                        main_buttons[i].Outline.gameObject.SetActive( false );

                        current_work_mode = (current_work_mode == ActiveMode.Scaling) ? ActiveMode.Movement : current_work_mode;

                        break;
                    }
                }
            }

            // Set scale mode
            else { 

                for( int i = 0; i < main_buttons.Length; i++ ) { 

                    if( main_buttons[i].Button_mode == ActiveMode.Scaling ) {

                        main_buttons[i].Outline.gameObject.SetActive( true );

                        current_work_mode = ActiveMode.Scaling;

                        break;
                    }
                }
            }
        }

        // Stops scaler ###################################################################################################################################################################################################################################
        public void StopScaler( bool hide_scaler ) { 

            MeasuringControl.Instance.Scaler_transform.gameObject.SetActive( !hide_scaler );

            for( int i = 0; i < main_buttons.Length; i++ ) { 

                if( main_buttons[i].Button_mode == ActiveMode.Scaling ) {

                    main_buttons[i].Outline.gameObject.SetActive( false );

                    break;
                }
            }

            current_work_mode = ActiveMode.Movement;
        }

        // Starts measurer ################################################################################################################################################################################################################################
        public void StartMeasurer() { 

            if( MeasuringControl.Instance.Scaler_transform.gameObject.activeSelf ) {
            
                //StopScaler( true );
            }

            if( MeasuringControl.Instance.Section_line_renderer.gameObject.activeSelf  ) {
            
                StopAngleMeter( true );
            }

            // Set usual mode
            if( MeasuringControl.Instance.Measurer_line_renderer.gameObject.activeSelf ) {  

                for( int i = 0; i < main_buttons.Length; i++ ) { 

                    if( main_buttons[i].Button_mode == ActiveMode.Measuring ) {

                        main_buttons[i].Outline.gameObject.SetActive( false );

                        current_work_mode = (current_work_mode == ActiveMode.Measuring) ? ActiveMode.Movement : current_work_mode;

                        break;
                    }
                }
            }

            // Set measuring mode
            else {

                for( int i = 0; i < main_buttons.Length; i++ ) { 

                    if( main_buttons[i].Button_mode == ActiveMode.Measuring ) {

                        main_buttons[i].Outline.gameObject.SetActive( true );

                        current_work_mode = ActiveMode.Measuring;

                        break;
                    }
                }
            }
        }

        // Stops measurer ###### ###########################################################################################################################################################################################################################
        public void StopMeasurer( bool hide_measurer ) { 

            if( MeasuringControl.Instance.Measurer_line_renderer.gameObject.activeSelf ) { 

                MeasuringControl.Instance.DeactivateMeasurer( !hide_measurer );

                for( int i = 0; i < main_buttons.Length; i++ ) { 

                    if( main_buttons[i].Button_mode == ActiveMode.Measuring ) {

                        main_buttons[i].Outline.gameObject.SetActive( false );

                        break;
                    }
                }
            }

            current_work_mode = ActiveMode.Movement;
        }

        // Starts angle measuryng #########################################################################################################################################################################################################################
        public void StartAngleMeter() { 

            if( MeasuringControl.Instance.Scaler_transform.gameObject.activeSelf ) {
            
                //StopScaler( true );
            }

            if( MeasuringControl.Instance.Measurer_line_renderer.gameObject.activeSelf ) {
            
                StopMeasurer( true );
            }

            // Set usual mode
            if( MeasuringControl.Instance.Section_line_renderer.gameObject.activeSelf ) { 

                for( int i = 0; i < main_buttons.Length; i++ ) { 

                    if( main_buttons[i].Button_mode == ActiveMode.Angle ) {

                        main_buttons[i].Outline.gameObject.SetActive( false );

                        current_work_mode = (current_work_mode == ActiveMode.Angle) ? ActiveMode.Movement : current_work_mode;

                        break;
                    }
                }
            }

            // Set angle measuring mode
            else { 

                for( int i = 0; i < main_buttons.Length; i++ ) { 

                    if( main_buttons[i].Button_mode == ActiveMode.Angle ) {

                        main_buttons[i].Outline.gameObject.SetActive( true );

                        current_work_mode = ActiveMode.Angle;

                        break;
                    }
                }
            }
        }

        // Stops angle measurer ###########################################################################################################################################################################################################################
        public void StopAngleMeter( bool hide_meter ) { 

            if( MeasuringControl.Instance.Section_line_renderer.gameObject.activeSelf ) { 

                MeasuringControl.Instance.DeactivateAngleMeter( !hide_meter );

                for( int i = 0; i < main_buttons.Length; i++ ) { 

                    if( main_buttons[i].Button_mode == ActiveMode.Angle ) {

                        main_buttons[i].Outline.gameObject.SetActive( false );

                        break;
                    }
                }
            }

            current_work_mode = ActiveMode.Movement;
        }

        // Select specified main button ###################################################################################################################################################################################################################
        public void SelectMainButton( Button button ) { 
        
            Selected_main_button = null;

            for( int i = 0; i < main_buttons.Length; i++ ) { 
            
                if( main_buttons[i].Button == button ) { 
                
                    main_buttons[i].Outline.gameObject.SetActive( true );

                    Selected_main_button = main_buttons[i];

                    if( Selected_main_button.Button_mode != ActiveMode.None ) { 

                        current_work_mode = Selected_main_button.Button_mode;
                    }
                }

                else { 

                    main_buttons[i].Outline.gameObject.SetActive( false );
                }
            }
        }

        // Deselect all main buttons ######################################################################################################################################################################################################################
        public void DeselectAllMainButtons() { 

            Selected_main_button = null;

            current_work_mode = ActiveMode.Movement;

            for( int i = 0; i < main_buttons.Length; i++ ) { 
            
                main_buttons[i].Outline.gameObject.SetActive( false );
            }
        }

        // Selects specified color button #################################################################################################################################################################################################################
        public void SetPaintingColor( Button button ) { 

            Current_work_mode = ActiveMode.Painting;

            for( int i = 0; i < color_buttons.Length; i++ ) { 
            
                if( color_buttons[i].Button == button ) { 
                
                    color_buttons[i].Outline.gameObject.SetActive( true );

                    Selected_color_button = color_buttons[i];
                }

                else { 

                    color_buttons[i].Outline.gameObject.SetActive( false );
                }
            }

            if( Selected_color_button == null ) {

                color_buttons[0].Outline.gameObject.SetActive( true );

                Selected_color_button = color_buttons[0];
            }

            if( PaintingControl.Instance != null ) { 
                
                PaintingControl.Instance.Current_paint = PaintingControl.Instance.GetPaint( button.GetComponent<PaintingColorButton>().Selected_color );
            }

            button_eraser.Outline.gameObject.SetActive( false );
        }

        // Selects eraser button ##########################################################################################################################################################################################################################
        public void SelectEraser( Button button ) { 

            Current_work_mode = ActiveMode.Erasing;

            for( int i = 0; i < color_buttons.Length; i++ ) { 
            
                color_buttons[i].Outline.gameObject.SetActive( false );
            }

            button_eraser.Outline.gameObject.SetActive( true );

            if( SteamVR.active ) { 
                
                VivePointerLeft.Instance.Touch_pointer_trigger.GetComponent<PaintingEraser>().EnableCollider();
                VivePointerRight.Instance.Touch_pointer_trigger.GetComponent<PaintingEraser>().EnableCollider();
            }

            else if( GeoInputManager.Instance != null ) {
            
                if( GeoInputManager.Instance.Painting_eraser != null ) { 
                    
                    GeoInputManager.Instance.Painting_eraser.EnableCollider();
                }
            }
        }

        // Saves the current sketch #######################################################################################################################################################################################################################
        public void SaveSketch( Button button ) { 

            if( PaintingControl.Instance != null ) { 
                
                PaintingControl.Instance.SaveSketch();
            }
        }

        // Loads the current sketch single ################################################################################################################################################################################################################
        public void LoadSketchSingle( Button button ) { 

            if( PaintingControl.Instance != null ) { 
                
                PaintingControl.Instance.LoadSketch( false );
            }
        }

        // Loads the current sketch group #################################################################################################################################################################################################################
        public void LoadSketchGroup( Button button ) { 

            if( PaintingControl.Instance != null ) { 
                
                PaintingControl.Instance.LoadSketch( true );
            }
        }

        // Sets state for the specified button ############################################################################################################################################################################################################
        public void SetButtonState( ActiveMode button_mode, bool state, ActiveMode end_mode = ActiveMode.None ) {
        
            for( int i = 0; i < main_buttons.Length; i++ ) { 

                if( main_buttons[i].Button_mode == button_mode ) {

                    main_buttons[i].Outline.gameObject.SetActive( state );

                    if( end_mode != ActiveMode.None ) {
                    
                        current_work_mode = end_mode;
                    }

                    break;
                }
            }

            if( current_work_mode != ActiveMode.Erasing ) { 

                if( SteamVR.active ) { 
                
                    if( VivePointerLeft.Instance.Touch_pointer_trigger != null ) {
                    
                        VivePointerLeft.Instance.Touch_pointer_trigger.GetComponent<PaintingEraser>().DisableCollider();
                    }

                    if( VivePointerRight.Instance.Touch_pointer_trigger != null ) {
                    
                        VivePointerRight.Instance.Touch_pointer_trigger.GetComponent<PaintingEraser>().DisableCollider();
                    }
                }

                else if( GeoInputManager.Instance != null ) {
            
                    if( GeoInputManager.Instance.Painting_eraser != null ) { 
                    
                        GeoInputManager.Instance.Painting_eraser.DisableCollider();
                    }
                }                
            }
        }

        // Makes screenshot and save to disk ##############################################################################################################################################################################################################
        public void MakeScreenshot() {

            string path = Path.Combine( ConfigControl.Instance.GetScreenshotsFolder(), ConfigControl.Instance.GetScreenshotsFilename( SceneManager.GetActiveScene().name ) );

            ViveInteractionsManager.Instance.GetComponent<AudioSource>().PlayOneShot( ProjectData.Instance.Make_screenshot_clip );

            try {

                if( !Directory.Exists( Path.GetDirectoryName( path ) ) ) { 

                    Directory.CreateDirectory( Path.GetDirectoryName( path ) );
                }
            }

            catch { 
            
                #if( UNITY_EDITOR )
                Debug.LogError( "Cannot create directory " + Path.GetDirectoryName( path ) + "!" );
                #endif
            }

            finally { 
            
                if( Directory.Exists( Path.GetDirectoryName( path ) ) ) { 

                    SaveScreenshotToDisk( path );
                }
            }
        }

        // Saves screenshot to disk #######################################################################################################################################################################################################################
        private void SaveScreenshotToDisk( string path ) { 

            Camera main_camera = ViveInteractionsManager.Instance.Eye_camera_transform.GetComponent<Camera>();

            Camera shooting_camera = ViveInteractionsManager.Instance.Menu_camera_transform.GetComponent<Camera>();

            shooting_camera.fieldOfView = main_camera.fieldOfView;

            RenderTexture render_texture = new RenderTexture( ProjectData.Instance.Screenshot_width, ProjectData.Instance.Screenshot_height, 24 );

            bool success = true;

            try {
            
                shooting_camera.enabled = false;
                shooting_camera.gameObject.SetActive( true );
                shooting_camera.targetTexture = render_texture;

                Texture2D screenshot = new Texture2D( 
                    
                    ProjectData.Instance.Screenshot_width, 
                    ProjectData.Instance.Screenshot_height, 
                    UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, 
                    UnityEngine.Experimental.Rendering.TextureCreationFlags.None 
                );

                ViveInteractionsManager.Instance.Menu_camera_transform.position = ViveInteractionsManager.Instance.Eye_camera_transform.position;
                ViveInteractionsManager.Instance.Menu_camera_transform.rotation = ViveInteractionsManager.Instance.Eye_camera_transform.rotation;

                shooting_camera.Render();

                RenderTexture.active = render_texture;

                screenshot.ReadPixels( new Rect( 0, 0, ProjectData.Instance.Screenshot_width, ProjectData.Instance.Screenshot_height ), 0, 0 );
                
                shooting_camera.targetTexture = null;

                RenderTexture.active = null;
                
                byte[] bytes = screenshot.EncodeToPNG();

                path = ConfigControl.Instance.ArrangeSlashes( path + ".png" );

                File.WriteAllBytes( path, bytes );
          
                #if( UNITY_EDITOR )
                Debug.Log( "Success! Screenshot saved to file " + path + "." );
                #endif

                shooting_camera.gameObject.SetActive( false );
            }

            catch { 
            
                success = false;

                if( use_messages ) { 
                    
                    ShowMessage( "Cannot write screenshot to file " + path + "!" );
                }

                #if( UNITY_EDITOR )
                Debug.LogError( "Cannot write screenshot to file " + path + "!" );
                #endif
            }

            finally { 
            
                if( render_texture != null ) { 

                    Destroy( render_texture );
                }

                if( success && use_messages ) { 

                    ShowMessage( "Screenshot has been saved to " + path );
                }
            }

            void ShowMessage( string message ) { 
                
                if( message_coroutine != null ) { 
                    
                    StopCoroutine( message_coroutine );
                }

                message_coroutine = StartCoroutine( ShowTextMessage( message ) );
            }

            IEnumerator ShowTextMessage( string message ) { 

                text_message.text = message;

                panel_message.alpha = 1;
                panel_message.gameObject.SetActive( true );

                float fade_ratio = 1f / message_timeout;

                for( float timer = 0; timer < message_timeout; timer += Time.unscaledDeltaTime ) { 
      
                    float alpha = 1 - timer * fade_ratio;

                    if( alpha < 0 ) { 
                    
                        alpha = 0;
                    }

                    panel_message.alpha = alpha;

                    yield return null;
                }

                panel_message.alpha = 0;
                panel_message.gameObject.SetActive( false );

                message_coroutine = null;

                yield break;
            }
        }

        // Load specified scene ###########################################################################################################################################################################################################################
        public void LoadScene( string scene_name ) {

            loading_scene_name = scene_name;

            if( SceneManager.GetActiveScene().name != scene_name ) { 
            
                panel_dialog_load_scene.SetActive( true );
            }
        }

        // Makes current network client as master #########################################################################################################################################################################################################
        public void AssignMeAsNetworkMasterClient() {

            if( !NetworkConnectionManager.Instance.Is_master_client ) { 
                
                NetworkConnectionManager.Instance.AssignMeAsNetworkMasterClient();

                image_make_me_master_client.gameObject.SetActive( false );
                image_i_am_master_client.gameObject.SetActive( !image_make_me_master_client.gameObject.activeSelf );
            }
        }

        // Load predefined scene ##########################################################################################################################################################################################################################
        public void LoadPredefinedSceneSingle() {

            Deactivate();

            StartCoroutine( WaitAndLoadPredefinedScene() );
        }

        // Load predefined scene and send message for all network clients #################################################################################################################################################################################
        public void LoadPredefinedSceneGroup() {

            Deactivate();

            if( NetworkTransactionsControlScenes.Instance != null ) { 

                NetworkTransactionsControlScenes.Instance.LoadNetworkGroupScene( ScenesManager.Instance.GetSceneIndexByName( loading_scene_name ) );
            }

            StartCoroutine( WaitAndLoadPredefinedScene() );
        }

        // Waits for leaving room and load scene ###############################################################################################################################################################################################################
        private IEnumerator WaitAndLoadPredefinedScene() { 

            if( PhotonNetwork.room != null ) {
            
                PhotonNetwork.LeaveRoom( true );
            }            

            while( PhotonNetwork.room != null ) {

                yield return null;
            }

            ScenesManager.Instance.LoadScene( loading_scene_name );

            yield break;
        }

        // Hidess all specific objects ####################################################################################################################################################################################################################
        public void LoadNextScene() { 

            CanvasDialogGeoLoadScene.Instance.Show( DialogType.Scene );

            ScenesManager.Instance.LoadScene( ScenesManager.Instance.Loading_scene_name ); 
        }

        // Activates local minimap ########################################################################################################################################################################################################################
        public void ActivateLocalMinimap() {

            panel_minimaps.GetComponent<SceneAttributeControl>().DeactivateObject( ScenesManager.Instance.Start_scene_name );
            panel_minimaps.GetComponent<SceneAttributeControl>().ActivateObject( SceneManager.GetActiveScene().name );

            image_global_map.SetActive( true );
            image_local_map.SetActive( false );

            PrepareNavigationSystem( 
                
                panel_minimaps.GetComponentInChildren<MinimapControl>(), 
                NavigationControl.Instance.GetComponentInChildren<RealmapControl>() 
            );
        }

        // Activates global minimap #######################################################################################################################################################################################################################
        public void ActivateGlobalMinimap() {

            panel_minimaps.GetComponent<SceneAttributeControl>().DeactivateObject( SceneManager.GetActiveScene().name );
            panel_minimaps.GetComponent<SceneAttributeControl>().ActivateObject( ScenesManager.Instance.Start_scene_name );

            image_global_map.SetActive( false );
            image_local_map.SetActive( true );

            image_player.SetActive( false );
        }

        // Adapts menu for the player in PC or VR mode ####################################################################################################################################################################################################
        public void AdaptCanvasForRemote() { 

            if( SteamVR.active ) { 

                canvas_transform.SetParent( VivePointerLeft.Instance.Model_controller_transform );

                if( canvas_transform.localPosition != vr_mode_local_position ) {

                    canvas_transform.localPosition = vr_mode_local_position;
                }

                if( canvas_transform.localEulerAngles != vr_mode_local_rotation ) {

                    canvas_transform.localEulerAngles = vr_mode_local_rotation;
                }

                if( canvas_transform.localScale != vr_mode_local_scale ) {
                
                    canvas_transform.localScale = vr_mode_local_scale;
                }
            }

            else if( LevelManagerMain.Instance.Starting_controller_mode == CanvasControllerMode.Small ) { 

                canvas_transform.SetParent( ViveInteractionsManager.Instance.Eye_camera_transform );

                if( canvas_transform.localPosition != small_mode_local_position ) {

                    canvas_transform.localPosition = small_mode_local_position;
                }

                if( canvas_transform.localEulerAngles != small_mode_local_rotation ) {

                    canvas_transform.localEulerAngles = small_mode_local_rotation;
                }

                if( canvas_transform.localScale != small_mode_local_scale ) {
                
                    canvas_transform.localScale = small_mode_local_scale;
                }
            }

            else { 

                canvas_transform.SetParent( ViveInteractionsManager.Instance.Eye_camera_transform );

                if( canvas_transform.localPosition != non_vr_mode_local_position ) {

                    canvas_transform.localPosition = non_vr_mode_local_position;
                }

                if( canvas_transform.localEulerAngles != non_vr_mode_local_rotation ) {

                    canvas_transform.localEulerAngles = non_vr_mode_local_rotation;
                }

                if( canvas_transform.localScale != non_vr_mode_local_scale ) {
                
                    canvas_transform.localScale = non_vr_mode_local_scale;
                }            
            }
        }

        // Adapts menu for the player in PC or VR mode ####################################################################################################################################################################################################
        [System.Obsolete( "Obsolete method: Adapt on teleport! It is necessary to replace on more actual one!" )]
        private void AdaptOnTeleport() { 

            #if( UNITY_EDITOR ) 
            Debug.LogError( "Obsolete method: Adapt on teleport! It is necessary to replace on more actual one!" ); return;
            #endif

            if( SteamVR.active ) { 

                //canvas_transform.SetParent( ViveInteractionsManager.Instance.Vive_pointer_left.GetComponent<Transform>(), false );

                canvas_transform.localPosition = vr_mode_local_position;
                canvas_transform.localEulerAngles = vr_mode_local_rotation;

                if( canvas_transform.localScale != vr_mode_local_scale ) {
                
                    canvas_transform.localScale = vr_mode_local_scale;
                }
            }

            else { 

                //canvas_transform.SetParent( ViveInteractionsManager.Instance.Head_camera_transform, false );

                canvas_transform.localPosition = non_vr_mode_local_position;
                canvas_transform.localEulerAngles = non_vr_mode_local_rotation;

                if( canvas_transform.localScale != non_vr_mode_local_scale ) {
            
                    canvas_transform.localScale = non_vr_mode_local_scale;
                }
            }
        }

        // On click clear all paintings ###################################################################################################################################################################################################################
        public void OnClickClearAllPixels() {

            PaintingControl.Instance.ClearAllPixels();

            if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlPainting.Instance != null) ) { 

                NetworkTransactionsControlPainting.Instance.ClearAllPixels( VertexStudio.Networking.NetworkPlayer.Instance.View_ID );
            }
        }
    }
}