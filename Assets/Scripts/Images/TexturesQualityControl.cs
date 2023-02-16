using System.IO;
using System.Collections;
using UnityEngine;
using VertexStudio.Generic;

namespace VostokVR.Geo {

    public enum TexturesVisualizationMode {

        Variable,
        Permanent
    }

    [System.Serializable]
    public class AssetBundleControl {

        public string Asset_bundle_name = string.Empty;

        public AssetBundle Asset_bundle;

        public AssetBundleCreateRequest Loading_request;
    }

    //[ExecuteInEditMode]
    public class TexturesQualityControl : MonoBehaviour {

        public static TexturesQualityControl Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private TexturesVisualizationMode textures_visualization_mode = TexturesVisualizationMode.Variable;
        public TexturesVisualizationMode Textures_visualization_mode { get { return textures_visualization_mode; } }

        [SerializeField, Range( 0.1f, 10f )]
        private float change_texture_distance = 3f;
        public float Change_texture_distance { get { return change_texture_distance; } }

        [Space( 10 ), SerializeField]
        private AssetBundleControl[] asset_bundle_controls;

        #if( UNITY_EDITOR )
        [Space( 10 )]
        public bool load_asset_bundle_in_editor = false;
        private bool previous_value_load_asset_bundle_in_editor = false;

        [Space( 10 )]
        public bool use_material_references = true;
        private bool previous_value_use_material_references = true;

        private MaterialControl[] material_controls;
        #endif

        [Space( 10 ), SerializeField]
        private GameObject[] uncontrollable_rocks;

        private TeleportSurfaceRock[] rocks;
         
        public bool Is_visible { get; private set; }

        // Awake is called before the first frame update ##################################################################################################################################################################################################
        private void Awake() {
        
            Instance = this;

            #if( UNITY_EDITOR )
            previous_value_use_material_references = use_material_references;
            #endif

            Is_visible = true;

            rocks = GetComponentsInChildren<TeleportSurfaceRock>( true );
        }

        // Use this for initialization ####################################################################################################################################################################################################################
        private void OnEnable() {

            if( Application.isPlaying ) {

                for (int i = 0; i < asset_bundle_controls.Length; i++ ) {

                    if( asset_bundle_controls[i].Asset_bundle != null ) {

                        asset_bundle_controls[i].Asset_bundle.Unload( true );

                        asset_bundle_controls[i].Asset_bundle = null;
                        asset_bundle_controls[i].Loading_request = null;
                    }
                }
            }
        }

        // Use this for deinitialization ##################################################################################################################################################################################################################
        private void OnDisable() {
            
            if( Application.isPlaying ) {

                for (int i = 0; i < asset_bundle_controls.Length; i++ ) {

                    if( asset_bundle_controls[i].Asset_bundle != null ) {

                        asset_bundle_controls[i].Asset_bundle.Unload( true );

                        asset_bundle_controls[i].Asset_bundle = null;
                        asset_bundle_controls[i].Loading_request = null;
                    }
                }
            }
        }

        // Start is called before the first frame update ##################################################################################################################################################################################################
        private void Start() {

            if( !Application.isPlaying ) { 
            
                return;
            }
           
            StartCoroutine( LoadAssetBundles() );
        }

        #if( UNITY_EDITOR )
        // Update is called once per a frame ##############################################################################################################################################################################################################
        private void Update() {
            
            if( Application.isPlaying ) { 
            
                return;
            }

            if( previous_value_load_asset_bundle_in_editor != load_asset_bundle_in_editor ) { 
             
                for (int i = 0; i < asset_bundle_controls.Length; i++ ) {

                    if( asset_bundle_controls[i].Asset_bundle != null ) {

                        asset_bundle_controls[i].Asset_bundle.Unload( true );

                        asset_bundle_controls[i].Asset_bundle = null;
                        asset_bundle_controls[i].Loading_request = null;
                    }
                }

                if( load_asset_bundle_in_editor ) {
                
                    StartCoroutine( LoadAssetBundles() );
                }

                else { 
                
                    ScenesManager.MarkCurrentSceneDirty();
                }

                previous_value_load_asset_bundle_in_editor = load_asset_bundle_in_editor;
            }

            if( previous_value_use_material_references != use_material_references ) { 
            
                material_controls = GetComponentsInChildren<MaterialControl>( true );

                if( use_material_references ) { 
                    
                    for( int i = 0; i < material_controls.Length; i++ ) { 
                        
                        material_controls[i].AssignMaterialReference();
                    }
                }

                else { 
                 
                    for( int i = 0; i < material_controls.Length; i++ ) { 
                    
                        material_controls[i].ClearMaterialReference();
                    }
                }

                ScenesManager.MarkCurrentSceneDirty();
            }

            previous_value_use_material_references = use_material_references;
        }
        #endif

        // Shows rocks ####################################################################################################################################################################################################################################
        public void ShowRocks() {

            for( int i = 0; i < rocks.Length; i++ ) { 
            
                //rocks[i].Mesh_renderer.enabled = true;
                rocks[i].gameObject.SetActive( true );
            }

            for( int i = 0; (uncontrollable_rocks != null) && (i < uncontrollable_rocks.Length); i++ ) { 
            
                uncontrollable_rocks[i].SetActive( true );
            }

            Is_visible = true;
        }

        // Hides rocks ####################################################################################################################################################################################################################################
        public void HideRocks() {

            for( int i = 0; i < rocks.Length; i++ ) {

                //rocks[i].Mesh_renderer.enabled = false;
                rocks[i].gameObject.SetActive( false );
            }

            for( int i = 0; (uncontrollable_rocks != null) && (i < uncontrollable_rocks.Length); i++ ) { 
            
                uncontrollable_rocks[i].SetActive( false );
            }

            Is_visible = false;
        }

        // Returns corresponding asset bundle #############################################################################################################################################################################################################
        public AssetBundle GetAssetBundle( string asset_bundle_name ) { 

            AssetBundle asset_bundle = null;

            for( int i = 0; i < asset_bundle_controls.Length; i++ ) {

                if( asset_bundle_name == asset_bundle_controls[i].Asset_bundle_name ) { 
                
                    asset_bundle = asset_bundle_controls[i].Asset_bundle;

                    break;
                }
            }

            return asset_bundle;
        }

        // Returns corresponding asset bundle #############################################################################################################################################################################################################
        public AssetBundleCreateRequest GetAssetBundleRequest( string asset_bundle_name ) {

            AssetBundleCreateRequest asset_bundle_request = null;

            for( int i = 0; i < asset_bundle_controls.Length; i++ ) {

                if( asset_bundle_name == asset_bundle_controls[i].Asset_bundle_name ) {

                    asset_bundle_request = asset_bundle_controls[i].Loading_request;

                    break;
                }
            }

            return asset_bundle_request;
        }

        // Loads asset bundle in async mode ###############################################################################################################################################################################################################
        private IEnumerator LoadAssetBundles() {

            // Wait for ConfigControl initialization complete
            yield return null;

            // Load all asset bundles
            for( int i = 0; i < asset_bundle_controls.Length; i++ ) { 

                AssetBundleControl asset_bundle_control = asset_bundle_controls[i];

                string asset_bundle_path = (
                
                    (!Application.isPlaying && Application.isEditor) ? 
                    Path.Combine( FindObjectOfType<ConfigControl>().GetAssetBundlesFolder(), asset_bundle_control.Asset_bundle_name ) : 
                    Path.Combine( ConfigControl.Instance.GetAssetBundlesFolder(), asset_bundle_control.Asset_bundle_name )
                );

                // Load asset bundle in async mode
                if( asset_bundle_control.Asset_bundle == null ) {

                    #if( DEBUG_MODE )
                    if( Application.isPlaying ) {
                
                        CanvasController.Instance.ShowDebugInfo( this, "Try to load asset bundle " + asset_bundle_path, true );
                    }
                    #endif

                    #if( UNITY_EDITOR )
                    if( Application.isPlaying ) {
                
                        //Debug.Log( "Try to load asset bundle " + asset_bundle_path );
                    }
                    #endif
                
                    asset_bundle_control.Loading_request = AssetBundle.LoadFromFileAsync( asset_bundle_path );

                    yield return asset_bundle_control.Loading_request;
                }

                // Initialize asset bundle
                if( (asset_bundle_control.Loading_request != null) && (asset_bundle_control.Loading_request.assetBundle != null)) {

                    asset_bundle_control.Asset_bundle = asset_bundle_control.Loading_request.assetBundle;

                    #if( DEBUG_MODE )
                    if( Application.isPlaying ) {
                    
                        CanvasController.Instance.ShowDebugInfo( this, "Asset bunlde " + asset_bundle_control.Asset_bundle.name + " loaded", true );
                    }
                    #endif

                    #if( UNITY_EDITOR )
                    if( Application.isPlaying ) {
                
                        //Debug.Log( "Asset bunlde " + asset_bundle_control.Asset_bundle.name + " loaded" );
                    }
                    #endif
                }

                else { 

                    #if( DEBUG_MODE )
                    if( Application.isPlaying ) {
                    
                        CanvasController.Instance.ShowDebugInfo( this, "Cannot load asset bunlde " + asset_bundle_path, true );
                    }
                    #endif

                    #if( UNITY_EDITOR )
                    if( Application.isPlaying ) {
                
                        Debug.LogError( "Cannot load asset bunlde " + asset_bundle_path );
                    }
                    #endif
                }
            }

            yield break;
        }

        // Service methods ################################################################################################################################################################################################################################
        #if( UNITY_EDITOR )
        [ContextMenu( "1) Set LOW quality materials to all rock meshes" )]
        private void SetLowQualityMaterials() { 

            material_controls = GetComponentsInChildren<MaterialControl>( true );

            StartCoroutine( ReduceQuality( material_controls ) );
        }

        // ################################################################################################################################################################################################################################################
        [ContextMenu( "2) Set HIGH quality materials to all rock meshes" )]
        private void SetHighQualityMaterials() { 

            material_controls = GetComponentsInChildren<MaterialControl>( true );

            StartCoroutine( RaiseQuality( material_controls ) );
        }

        // ################################################################################################################################################################################################################################################
        private IEnumerator ReduceQuality( MaterialControl[] material_controls ) { 

            for( int i = 0; i < material_controls.Length; i++ ) { 
                
                material_controls[i].Material_resolution = MaterialResolution.Low;
            }
            
            yield break;
        }

        // ################################################################################################################################################################################################################################################
        private IEnumerator RaiseQuality( MaterialControl[] material_controls ) { 

            ProjectData project_data = FindObjectOfType<ProjectData>();

            for( int i = 0; i < material_controls.Length; i++ ) { 
                
                material_controls[i].Material_resolution = MaterialResolution.High;
            }            

            yield break;
        }
        #endif
    }
}