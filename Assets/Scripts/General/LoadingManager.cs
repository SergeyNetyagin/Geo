using System.IO;
using System.Collections;
using UnityEngine;
using Valve.VR;

namespace VostokVR.Geo {

    public class LoadingManager : MonoBehaviour {

        public static LoadingManager Instance { get; private set; }
  
        [Space( 10 ), HideInInspector, SerializeField]
        private bool use_async_loading = true;

        [HideInInspector, SerializeField]
        private bool use_mediator_scene = true;

        [HideInInspector, SerializeField]
        private bool use_steam_loading_system = false;

        //[Space( 10 ), SerializeField]
        //private GameObject bundle_loading_skybox_prefab;
        //private GameObject bundle_loading_skybox;

        //[SerializeField]
        //private VideoPlayer video_loading_skybox_prefab;
        //private VideoPlayer video_loading_skybox;

        //[SerializeField]
        //private SteamVR_LoadLevel steam_loading_skybox_prefab;
        //private SteamVR_LoadLevel steam_loading_skybox;

        [Space( 10 ), SerializeField]
        private string login_scene_name = "Login";
        public string Login_scene_name { get { return login_scene_name; } }

        [SerializeField]
        private string installer_scene_name = "Installer";
        public string Installer_scene_name { get { return installer_scene_name; } }

        [SerializeField]
        private string analytics_scene_name = "Analytics";
        public string Analytics_scene_name { get { return analytics_scene_name; } }

        [Space( 10 ), SerializeField]
        private string mediator_scene_name = "Loading";
        public string Mediator_scene_name { get { return mediator_scene_name; } }

        [Space( 10 ), SerializeField]
        private string intro_scene_name = "Intro";
        public string Intro_scene_name { get { return intro_scene_name; } }

        [SerializeField]
        private string adaptation_scene_name = "Adaptation";
        public string Adaptation_scene_name { get { return adaptation_scene_name; } }

        [SerializeField]
        private string tutorial_scene_name = "Tutorial";
        public string Tutorial_scene_name { get { return tutorial_scene_name; } }

        [SerializeField]
        private string lobby_scene_name = "Lobby";
        public string Lobby_scene_name { get { return lobby_scene_name; } }

        [Space( 10 ), SerializeField, Tooltip( "The very light scenes that loading fast and should load in the dark (without showing of progress)" )]
        private string[] fast_load_scenes;
        public string[] Fast_load_scenes { get { return fast_load_scenes; } }

        [Space( 10 ), HideInInspector, SerializeField]
        private bool skip_adaptation_scene = false;
        public bool Skip_adaptation_scene { get { return skip_adaptation_scene; } }

        [HideInInspector, SerializeField]
        private bool skip_tutorial_scene = true;
        public bool Skip_tutorial_scene { get { return skip_tutorial_scene; } }

        public string Loading_scene_name { get; private set; }
        public AssetBundle Loading_asset_bundle { get; set; }
           
        private bool is_application_just_started = true;         
        public bool Is_application_just_started { get { return is_application_just_started; } }

        private CVROverlay overlay;
        private CVRCompositor compositor;

        // Use this for initialization #####################################################################################################################################################################################################################
        private void Awake() {

            if( Instance == null ) {

                Instance = this;

                string current_scene_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

                if( current_scene_name == mediator_scene_name ) Loading_scene_name = intro_scene_name;
//                else if( (current_scene_name == intro_scene_name) && (!skip_adaptation_scene && !ProjectManager.Instance.Play_area_is_adapted) ) Loading_scene_name = adaptation_scene_name;
                else if( (current_scene_name == adaptation_scene_name) && !skip_tutorial_scene ) Loading_scene_name = tutorial_scene_name;
                else if( current_scene_name == tutorial_scene_name ) Loading_scene_name = lobby_scene_name;
                else Loading_scene_name = lobby_scene_name;
            }
        }

        // Use this for initialization #####################################################################################################################################################################################################################
        private void Start() {

        }

        // Loads the new specific scene ####################################################################################################################################################################################################################
        public void LoadScene( string scene_name ) {

            // Check for quitting
//            if( ProjectManager.Instance.State == ApplicationState.Quitting ) return;

            // Check for empty name
            if( (scene_name == null) || (scene_name.Length == 0) ) {

                #if UNITY_EDITOR
                Debug.Log( "Scene's name is empty: scene has not been loaded!" );
                #endif

                return;
            }

            // Set states before loading
            Loading_scene_name = scene_name;

            if( (scene_name == intro_scene_name) ||
                (scene_name == tutorial_scene_name) ||
                (scene_name == adaptation_scene_name) ||
                (scene_name == lobby_scene_name) ) {
            }

            // Prepare Steam skybox
            if( SteamVR.active && use_steam_loading_system ) {

                //steam_loading_skybox = Instantiate( steam_loading_skybox_prefab, Vector3.zero, Quaternion.identity );
                //steam_loading_skybox.levelName = Loading_scene_name;
                //steam_loading_skybox.loadAsync = use_async_loading;

                compositor = SteamVR.instance.compositor;
                compositor.FadeGrid( 0f, false );
                compositor.FadeToColor( 0f, 1f, 0f, 0f, 0f, false );

                overlay = Valve.VR.OpenVR.Overlay;

                ReplaceSteamSplashScreen();
            }

            if( (Loading_scene_name != login_scene_name) && 
                (Loading_scene_name != intro_scene_name) && 
                (Loading_scene_name != tutorial_scene_name) && 
                (Loading_scene_name != lobby_scene_name) ) {
                
                is_application_just_started = false;
            }

            // Loading scene or bundle
            if( use_async_loading && !use_steam_loading_system ) StartCoroutine( LoadMediatorSceneAsync() );
            //else if( is_asset_bundle && use_async_loading ) StartCoroutine( LoadAssetBundleAsync() );
            //else if( is_asset_bundle && !use_async_loading ) StartCoroutine( LoadAssetBundle() );
            //else if( !is_asset_bundle && SteamVR.active && use_steam_loading_system && (steam_loading_skybox != null) ) steam_loading_skybox.Trigger();         
            //else if( !is_asset_bundle && use_mediator_scene ) UnityEngine.SceneManagement.SceneManager.LoadScene( mediator_scene_name );
            //else if( !is_asset_bundle ) UnityEngine.SceneManagement.SceneManager.LoadScene( Loading_scene_name );
        }
        
        // Loads scene in async mode #######################################################################################################################################################################################################################
        private IEnumerator LoadMediatorSceneAsync() {

            AsyncOperation loading = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync( mediator_scene_name );

            loading.allowSceneActivation = false;

            while( loading.progress < 0.9f ) yield return null;

            loading.allowSceneActivation = true; 

            while( !loading.isDone ) yield return null;

            yield break;
        }   

        // Loads AssetBundle in usual mode #################################################################################################################################################################################################################
        private IEnumerator LoadAssetBundle() {

            //if( LobbyManager.Instance != null ) CanvasFade.Instance.FadeIn();

            #if UNITY_EDITOR
            Debug.LogError( "CHECK FOR USING THIS MODULE!" );
            #endif

            if( Loading_asset_bundle == null ) {

                Loading_asset_bundle = AssetBundle.LoadFromFile( 
                    
                    //PropertyExchangeManager.Instance.Local_import_path + @"/" + 
                    Loading_scene_name + "." //+ 
                    //PropertyExchangeManager.Instance.Scene_extension 
                );

                if( Loading_asset_bundle == null ) {

                    #if UNITY_EDITOR
                    Debug.Log( "AssetBundle " + Loading_scene_name + " not found: scene has not been loaded!" );
                    #endif
                }
            }

            if( Loading_scene_name != null ) {
                
                UnityEngine.SceneManagement.SceneManager.LoadScene( Loading_scene_name );
            }

            yield break;
        }   
        
        // Replaces Steam's splash screen ##################################################################################################################################################################################################################
        private void ReplaceSteamSplashScreen() {

            ulong error_handle = Valve.VR.OpenVR.k_ulOverlayHandleInvalid;

            Valve.VR.CVROverlay overlay = Valve.VR.OpenVR.Overlay;

            string overlay_name = "Loading Screen";

            SteamVR.instance.compositor.FadeToColor( 0.1f, 0f, 0f, 0f, 1f, true );

		    if( overlay != null ) {

		        string key = SteamVR_Overlay.key + "." + overlay_name;

                Valve.VR.EVROverlayError error = overlay.FindOverlay( key, ref error_handle );

		        if( error != Valve.VR.EVROverlayError.None ) {

			        error = overlay.CreateOverlay( key, overlay_name, ref error_handle);

                    if( error != Valve.VR.EVROverlayError.None ) error = overlay.CreateOverlay( key, overlay_name, ref error_handle );
		            if( error == Valve.VR.EVROverlayError.None ) overlay.ShowOverlay( error_handle );
                }
            }
        }
    }
}