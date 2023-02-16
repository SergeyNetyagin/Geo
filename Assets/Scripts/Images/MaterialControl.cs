using System.IO;
using System.Collections;
using UnityEngine;
using VertexStudio.Generic;

namespace VostokVR.Geo {

    public enum MaterialResolution { 

        Auto,
        Low,
        High
    }

    [ExecuteInEditMode]
    public class MaterialControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private MeshRenderer mesh_renderer;

        [SerializeField]
        private MaterialResolution material_resolution = MaterialResolution.Auto;
        public MaterialResolution Material_resolution { get { return material_resolution; } set { material_resolution = SetMaterialResolution( value ); } }

        [Space( 10 ), SerializeField]
        private Material low_quality_material;

        [SerializeField]
        private Material high_quality_material;

        [SerializeField]
        private string high_quality_material_folder = string.Empty;

        [Space(10), SerializeField]
        private string asset_bundle_name = string.Empty;
        public string Asset_bundle_name { get { return asset_bundle_name; } }

        [SerializeField]
        private string asset_bundle_folder = string.Empty;
        public string Asset_bundle_folder { get { return asset_bundle_folder; } }

        public MeshCollider Rock_collider { get; private set; }

        #if (UNITY_EDITOR)
        private MaterialResolution previous_material_resolution = MaterialResolution.Auto;
        #endif

        private Coroutine loading_material_coroutine = null;

        private AssetBundleRequest material_loading_request = null;

        // Awake is called before the first frame update ##################################################################################################################################################################################################
        private void Awake() {
        
            #if( UNITY_EDITOR )
            previous_material_resolution = material_resolution;
            #endif
        }

        // OnEnable is called before the first frame update ###############################################################################################################################################################################################
        private void OnEnable() {

            if( Application.isPlaying ) {

                if( Rock_collider == null ) { 

                    Rock_collider = GetComponent<MeshCollider>();
                
                    if( Rock_collider == null ) {
                    
                        Rock_collider = gameObject.AddComponent<MeshCollider>();
                    }
                }
            }
        }

        // OnDisable is called when the object is disabling ###############################################################################################################################################################################################
        private void OnDisable() {

        }

        // Start is called before the first frame update ##################################################################################################################################################################################################
        private void Start() {
        
            #if( UNITY_EDITOR )
            if( mesh_renderer.sharedMaterial == low_quality_material ) { 

                if( material_resolution != MaterialResolution.Low ) {
                
                    material_resolution = MaterialResolution.Low;

                    ScenesManager.MarkCurrentSceneDirty();
                }
            }

            else if( mesh_renderer.sharedMaterial == high_quality_material ) { 
                
                if( material_resolution != MaterialResolution.High ) {

                    material_resolution = MaterialResolution.High;

                    ScenesManager.MarkCurrentSceneDirty();
                }
            }

            else { 
                
                if( material_resolution != MaterialResolution.Auto ) {
                
                    material_resolution = MaterialResolution.Auto;

                    ScenesManager.MarkCurrentSceneDirty();
                }
            }
            #endif
        }

        #if( UNITY_EDITOR )
        // Update is called once per a frame ##############################################################################################################################################################################################################
        private void Update() {
            
            if( previous_material_resolution != material_resolution ) { 
                
                SetMaterialResolution( material_resolution );
            }

            previous_material_resolution = material_resolution;
        }
        #endif

        // Assign material depending on specified resolution ##############################################################################################################################################################################################
        private MaterialResolution SetMaterialResolution( MaterialResolution resolution ) { 

            // Assigh LOW quality material
            if( resolution == MaterialResolution.Low ) {
        
                if( low_quality_material != null ) {

                    mesh_renderer.sharedMaterial = low_quality_material;

                    #if( UNITY_EDITOR )
                    //Debug.Log( "Material for " + mesh_renderer.name + " changed to the LOW quality." );
                    #endif

                    #if( DEBUG_MODE )
                    if( Application.isPlaying ) {
                    
                        CanvasController.Instance.ShowDebugInfo( this, "Material for " + mesh_renderer.name + " changed to the LOW quality." );
                    }
                    #endif

                    if( high_quality_material != null ) { 
                        
                        Resources.UnloadAsset( high_quality_material );

                        high_quality_material = null;
                    }

                    else if( loading_material_coroutine != null ) { 
                    
                        StopCoroutine( loading_material_coroutine );

                        loading_material_coroutine = null;
                    }

                    Resources.UnloadUnusedAssets();
                }

                else { 
                    
                    #if( UNITY_EDITOR )
                    Debug.LogError( "Low quality material for " + mesh_renderer.name + " cannot assigned because material is null!" );
                    #endif

                    #if( DEBUG_MODE )
                    if( Application.isPlaying ) {
                    
                        CanvasController.Instance.ShowDebugInfo( this, "Low quality material for " + mesh_renderer.name + " cannot assigned because material is null!" );
                    }
                    #endif
                }
            }

            // Assigh HIGH quality material
            else {
            
                if( high_quality_material != null ) {
                
                    mesh_renderer.sharedMaterial = high_quality_material;
                
                    #if( UNITY_EDITOR )
                    //Debug.Log( "Material for " + mesh_renderer.name + " changed to the HIGH quality." );
                    #endif

                    #if( DEBUG_MODE )
                    if( Application.isPlaying ) {
                    
                        CanvasController.Instance.ShowDebugInfo( this, "Material for " + mesh_renderer.name + " changed to the HIGH quality." );
                    }
                    #endif

                    Resources.UnloadUnusedAssets();
                }

                else if( !string.IsNullOrEmpty( high_quality_material_folder ) ) { 
                    
                    if( loading_material_coroutine != null ) { 
                    
                        StopCoroutine( loading_material_coroutine );
                    }

                    loading_material_coroutine = StartCoroutine( LoadMaterialFromAssetBundle( high_quality_material_folder ) );
                }

                else { 

                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot load HIGH qiality material for " + mesh_renderer.name + " because material is empty and path is empty!" );
                    #endif                    

                    #if( DEBUG_MODE )
                    if( Application.isPlaying ) {
                    
                        CanvasController.Instance.ShowDebugInfo( this, "Cannot load HIGH qiality material for " + mesh_renderer.name + " because material is empty and path is empty!" );
                    }
                    #endif
                }
            }

            return resolution;
        }

        // Loads necessary material form asset bundle #####################################################################################################################################################################################################
        private IEnumerator LoadMaterialFromAssetBundle( string internal_asset_path ) { 

            TexturesQualityControl textures_quality_control = (TexturesQualityControl.Instance == null) ? FindObjectOfType<TexturesQualityControl>() : TexturesQualityControl.Instance;

            AssetBundle asset_bundle = textures_quality_control.GetAssetBundle( asset_bundle_name );

            // Wait if asset bundle is loading by another MaterialControl
            if( asset_bundle == null ) {
                
                AssetBundleCreateRequest loading_request = textures_quality_control.GetAssetBundleRequest( asset_bundle_name );

                if( (loading_request != null) && !loading_request.isDone ) {
            
                    while( !loading_request.isDone ) { 

                        yield return null;
                    }
                }

                asset_bundle = textures_quality_control.GetAssetBundle( asset_bundle_name );
            }

            // Cannot load asset because asset bundle is null
            if( asset_bundle == null ) {

                #if( UNITY_EDITOR )
                Debug.LogError( "Cannot load " + internal_asset_path + " because asset bundle " + asset_bundle_name + " was not downloaded!" );
                #endif

                #if( DEBUG_MODE )
                if( Application.isPlaying ) {
                
                    CanvasController.Instance.ShowDebugInfo( this, "Cannot load " + internal_asset_path + " because asset bundle " + asset_bundle_name + " was not downloaded!" );
                }
                #endif
            }

            // Load new asset from asset bundle in async mode
            else { 

                material_loading_request = asset_bundle.LoadAssetAsync<Material>( internal_asset_path );

                yield return material_loading_request;

                if( (material_loading_request == null) || (material_loading_request.asset == null) ) {

                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot load " + internal_asset_path + " from loaded asset bundle " + asset_bundle_name + "!" );
                    #endif

                    #if( DEBUG_MODE )
                    if( Application.isPlaying ) {
                    
                        CanvasController.Instance.ShowDebugInfo( this, "Cannot load " + internal_asset_path + " from loaded asset bundle " + asset_bundle_name + "!" );
                    }
                    #endif
                }

                else { 
                    
                    mesh_renderer.sharedMaterial = (Material) material_loading_request.asset;

                    #if( UNITY_EDITOR )
                    //Debug.Log( "Material for " + mesh_renderer.name + " changed to the HIGH quality." );
                    #endif

                    #if( DEBUG_MODE )
                    if( Application.isPlaying ) {
                    
                        CanvasController.Instance.ShowDebugInfo( this, "Material for " + mesh_renderer.name + " changed to the HIGH quality." );
                    }
                    #endif
                }
            }

            loading_material_coroutine = null;

            yield break;
        }

        // Assigns material reference #####################################################################################################################################################################################################################
        #if( UNITY_EDITOR )
        public void AssignMaterialReference() { 
        
            if( !string.IsNullOrEmpty( high_quality_material_folder ) ) { 
                
                high_quality_material = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>( high_quality_material_folder );

                if( high_quality_material != null ) {
                
                    Debug.Log( "Material is successful assigned from path: " + high_quality_material_folder );
                }

                else { 
                
                    Debug.LogError( "Cannot assign material from path: " + high_quality_material_folder );
                }
            }
        }
        #endif

        // Clears material reference ######################################################################################################################################################################################################################
        #if( UNITY_EDITOR )
        public void ClearMaterialReference() { 
        
            if( high_quality_material != null ) { 
                
                high_quality_material_folder = UnityEditor.AssetDatabase.GetAssetPath( high_quality_material );

                if( !string.IsNullOrEmpty( high_quality_material_folder ) ) {

                    high_quality_material = null;
                
                    Debug.Log( "Material is cleared; path: " + high_quality_material_folder );
                }

                else { 
                        
                    Debug.LogError( "Material is NOT cleared because cannot get path for material " + high_quality_material.name );
                }
            }
        }
        #endif

        // Service methods ################################################################################################################################################################################################################################
        #if( UNITY_EDITOR )
        [ContextMenu( "1) Fill mesh renderer", false, 1000000 )]
        private void FillMeshRenderer() { 

            mesh_renderer = GetComponent<MeshRenderer>();
        }

        [ContextMenu( "2) Fill low quality material material", false, 1000001 )]
        private void FillLowQualityMaterial() { 

            low_quality_material = GetComponent<MeshRenderer>().sharedMaterial;
        }

        [ContextMenu( "3) Fill high quality material material", false, 1000002 )]
        private void FillHighQualityMaterial() { 

            high_quality_material = GetComponent<MeshRenderer>().sharedMaterial;
        }

        [ContextMenu( "Set MATERIAL reference by folder", false, 3000000 )]
        private void SetMaterialReference() { 

            AssignMaterialReference();
        }

        [ContextMenu( "Set FOLDER by material reference", false, 3000001 )]
        private void SetMaterialFolder() { 

            ClearMaterialReference();
        }

        [ContextMenu("Expand material PATH using existing material path and folder", false, 4000001)]
        private void ExpandMaterialPath() {

            if( !string.IsNullOrEmpty( asset_bundle_folder ) && !string.IsNullOrEmpty( high_quality_material_folder ) ) { 
            
                high_quality_material_folder = Path.Combine(
                
                    Path.GetDirectoryName( high_quality_material_folder ),
                    asset_bundle_folder,
                    Path.GetFileName( high_quality_material_folder )
                );
            }
        }

        [ContextMenu("Reduce material PATH using existing material path and folder", false, 4000002)]
        private void ReduceMaterialPath() {

            if( !string.IsNullOrEmpty( asset_bundle_folder ) && !string.IsNullOrEmpty( high_quality_material_folder ) ) {

                high_quality_material_folder = Path.Combine(

                    high_quality_material_folder.Remove( high_quality_material_folder.IndexOf( asset_bundle_folder ) ),
                    Path.GetFileName( high_quality_material_folder )
                );
            }
        }
        #endif
    }
}