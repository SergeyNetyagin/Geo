using System.IO;
using UnityEngine;

namespace VostokVR.Geo {

    public enum ApplicationResourcesLocation { 
    
        UseConfigurationFiles,
        IncludeInTheApplicationData
    }

    public class ConfigControl : MonoBehaviour {

        public static ConfigControl Instance { get; private set; }

        [Space( 10 ), SerializeField, Tooltip( "Use <config files> to locate resources anywhere you want; use <include in...> to include resources inside the app data folder before zipping the app." )]
        private ApplicationResourcesLocation resources_location = ApplicationResourcesLocation.IncludeInTheApplicationData;

        [Space( 10 ), SerializeField]
        private string asset_bundles_default_folder = "AssetBundles";
        public string Asset_bundles_default_folder => asset_bundles_default_folder;

        [SerializeField]
        private string config_files_runtime_folder = "StreamingAssets";
        public string Config_files_runtime_folder => config_files_runtime_folder;

        [Space( 10 ), SerializeField]
        private string screenshots_config_file_name = @"screenshots.config";
        public string Screenshots_config_file_name => screenshots_config_file_name;

        [SerializeField]
        private string assetbundles_config_file_name = @"assetbundles.config";
        public string Assetbundles_config_file_name => assetbundles_config_file_name;

        [Space( 10 ), SerializeField]
        private string assetbundles_distribution_folder = "Runtime Asset Bundles";

        [SerializeField]
        private string screenshots_distribution_folder = "Runtime Screenshots";


        /// <summary>
        /// Use this for initialization.
        /// </summary>
        private void Awake() {

            Instance = this;
        }


        /// <summary>
        /// Use this for initialization.
        /// </summary>
        private void Start() {

        }


        /// <summary>
        /// Returns screenshots configuration full file name.
        /// </summary>
        private string GetScreenshotsConfigFileFullPath() {

            string screenshots_path = Path.Combine( Application.streamingAssetsPath, screenshots_config_file_name );

            return ArrangeSlashes( screenshots_path );
        }


        /// <summary>
        /// Returns asset bundles configuration full file name.
        /// </summary>
        private string GetAssetBundlesConfigFileFullPath() {

            string assetbundles_path = Path.Combine( Application.streamingAssetsPath, assetbundles_config_file_name );

            return ArrangeSlashes( assetbundles_path );
        }


        /// <summary>
        /// Return directory for screenshots files.
        /// </summary>
        public string GetScreenshotsFolder() {

            string screenshots_path = string.Empty;
            string application_path = string.Empty;
            
            if( resources_location == ApplicationResourcesLocation.IncludeInTheApplicationData ) {

                try {
                    
                    #if( UNITY_EDITOR )
                    application_path = Application.persistentDataPath;
                    #else
                    application_path = Application.dataPath.Replace( Path.GetFileNameWithoutExtension( Application.dataPath ), string.Empty );
                    #endif

                    if( string.IsNullOrEmpty( application_path ) ) { 
                    
                        application_path = Application.isEditor ? Application.persistentDataPath : Application.dataPath;
                    }

                    screenshots_path = ArrangeSlashes( Path.Combine( application_path, screenshots_distribution_folder ) );

                    if( !string.IsNullOrEmpty( screenshots_path ) && !Directory.Exists( screenshots_path ) ) { 
                    
                        Directory.CreateDirectory( screenshots_path );

                        #if( UNITY_EDITOR || DEBUG_MODE )
                        Debug.Log( "Try to create folder for screenshots: " + screenshots_path );
                        #endif
                    }
                }

                catch { 
                    
                    #if( UNITY_EDITOR || DEBUG_MODE )
                    Debug.LogError( "Cannot create folder for screenshots!" );
                    #endif
                }
            }

            else if( resources_location == ApplicationResourcesLocation.UseConfigurationFiles ) {

                try {
            
                    screenshots_path = ArrangeSlashes( File.ReadAllText( GetScreenshotsConfigFileFullPath() ) );
                }

                catch { 

                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot found path " + GetScreenshotsConfigFileFullPath() + "!" );
                    #endif
                }
            }

            return (screenshots_path == null) ? string.Empty : screenshots_path;
        }


        /// <summary>
        /// Return directory for asset bundles files.
        /// </summary>
        public string GetAssetBundlesFolder() {

            string assetbundles_path = string.Empty;
            string application_data_path = Application.dataPath;

            #if( !UNITY_EDITOR )
            if( resources_location == ApplicationResourcesLocation.IncludeInTheApplicationData ) {

                assetbundles_path = ArrangeSlashes( Path.Combine( application_data_path, assetbundles_distribution_folder ) );

                if( string.IsNullOrEmpty( assetbundles_path ) ) { 
                
                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot found path for asset bundles!" );
                    #endif                    
                }
            }

            else if( resources_location == ApplicationResourcesLocation.UseConfigurationFiles ) {
            #endif

                string asset_bundles_config_file_path = GetAssetBundlesConfigFileFullPath();
            
                if( File.Exists( GetAssetBundlesConfigFileFullPath() ) ) {

                    try {
            
                        assetbundles_path = ArrangeSlashes( File.ReadAllText( asset_bundles_config_file_path ) );
                    }

                    catch { 

                        #if( UNITY_EDITOR )
                        Debug.LogError( "Cannot found path " + GetAssetBundlesConfigFileFullPath() + "!" );
                        #endif
                    }
                }

                if( string.IsNullOrEmpty( assetbundles_path ) ) { 
                
                    if( Application.platform == RuntimePlatform.WindowsEditor ) { 
                    
                        assetbundles_path = Application.streamingAssetsPath + @"/../";
                    }

                    else if( Application.platform == RuntimePlatform.WindowsPlayer ) { 
                    
                        assetbundles_path = Application.streamingAssetsPath + @"/../../";
                    }

                    else if( Application.platform == RuntimePlatform.OSXEditor ) { 
                    
                        assetbundles_path = Application.streamingAssetsPath + @"/../../";
                    }

                    else if( Application.platform == RuntimePlatform.OSXPlayer ) { 
                    
                        assetbundles_path = Application.streamingAssetsPath + @"/../../../";
                    }

                    assetbundles_path = Path.Combine( assetbundles_path, asset_bundles_default_folder );
                }

            #if( !UNITY_EDITOR )
            }
            #endif

            return (assetbundles_path == null) ? string.Empty : assetbundles_path;
        }


        /// <summary>
        /// Return unique name of file.
        /// </summary>
        public string GetScreenshotsFilename( string scene_name ) {

            string filename = "Screenshot " + scene_name + " ";
            
            filename += System.DateTime.Now.Year.ToString() + "-";
            filename += System.DateTime.Now.Month.ToString() + "-";
            filename += System.DateTime.Now.Day.ToString() + "-";
            filename += System.DateTime.Now.Hour.ToString() + "-";
            filename += System.DateTime.Now.Minute.ToString() + "-";
            filename += System.DateTime.Now.Second.ToString() + "-";
            filename += System.DateTime.Now.Millisecond.ToString();
            
            return filename;
        }


        /// <summary>
        /// Arranges slashes between folders.
        /// </summary>
        public string ArrangeSlashes( string path ) {

            string full_filename = path;

            if( Application.platform == RuntimePlatform.Android ) {

                full_filename = full_filename.Replace( @"\", @"/" );
            }

            else if( Application.platform == RuntimePlatform.IPhonePlayer ) {

                full_filename = full_filename.Replace( @"\", @"/" );
            }

            else if( Application.platform == RuntimePlatform.WindowsPlayer ) {

                full_filename = full_filename.Replace( @"\", @"/" );
            }

            else if( Application.platform == RuntimePlatform.WindowsEditor ) {

                full_filename = full_filename.Replace( @"\", @"/" );
            }

            return full_filename;
        }
    }
}