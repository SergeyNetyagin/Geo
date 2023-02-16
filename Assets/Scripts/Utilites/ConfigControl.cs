using System.IO;
using UnityEngine;

namespace VostokVR.Geo {

    public class ConfigControl : MonoBehaviour {

        public static ConfigControl Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private string asset_bundles_default_folder = "AssetBundles";
        public string Asset_bundles_default_folder { get { return asset_bundles_default_folder; } }

        [SerializeField]
        private string config_files_runtime_folder = @"Geo Next Generation Networking_Data\StreamingAssets";
        public string Config_files_runtime_folder { get { return config_files_runtime_folder; } }

        [Space( 10 ), SerializeField]
        private string screenshots_config_file_name = @"screenshots.config";
        public string Screenshots_config_file_name { get { return screenshots_config_file_name; } }

        [SerializeField]
        private string assetbundles_config_file_name = @"assetbundles.config";
        public string Assetbundles_config_file_name { get { return assetbundles_config_file_name; } }

        // Use this for initialization #####################################################################################################################################################################################################################
        private void Awake() {

            Instance = this;
        }

        // Use this for initialization #####################################################################################################################################################################################################################
        private void Start() {

        }

        /// <summary>
        /// Returns screenshots configuration full file name.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        private string GetScreenshotsConfigFileFullPath() {

            string screenshots_path = Path.Combine( Application.streamingAssetsPath, screenshots_config_file_name );

            return ArrangeSlashes( screenshots_path );
        }

        /// <summary>
        /// Returns asset bundles configuration full file name.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        private string GetAssetBundlesConfigFileFullPath() {

            string assetbundles_path = Path.Combine( Application.streamingAssetsPath, assetbundles_config_file_name );

            return ArrangeSlashes( assetbundles_path );
        }

        /// <summary>
        /// Return directory for screenshots files.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        public string GetScreenshotsFolder() {

            string screenshots_path = string.Empty;
            
            try {
            
                screenshots_path = File.ReadAllText( GetScreenshotsConfigFileFullPath() );
            }

            catch { 

                #if( UNITY_EDITOR )
                Debug.LogError( "Cannot found path " + GetScreenshotsConfigFileFullPath() + "!" );
                #endif
            }

            return (screenshots_path == null) ? string.Empty : screenshots_path;
        }

        /// <summary>
        /// Return directory for asset bundles files.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        public string GetAssetBundlesFolder() {

            string asset_bundles_config_file_path = GetAssetBundlesConfigFileFullPath();

            string asset_bundles_path = string.Empty;
            
            if( File.Exists( GetAssetBundlesConfigFileFullPath() ) ) {

                try {
            
                    asset_bundles_path = File.ReadAllText( asset_bundles_config_file_path );
                }

                catch { 

                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot found path " + GetAssetBundlesConfigFileFullPath() + "!" );
                    #endif
                }
            }

            if( string.IsNullOrEmpty( asset_bundles_path ) ) { 
                
                if( Application.platform == RuntimePlatform.WindowsEditor ) { 
                    
                    asset_bundles_path = Application.streamingAssetsPath + @"/../";
                }

                else if( Application.platform == RuntimePlatform.WindowsPlayer ) { 
                    
                    asset_bundles_path = Application.streamingAssetsPath + @"/../../";
                }

                else if( Application.platform == RuntimePlatform.OSXEditor ) { 
                    
                    asset_bundles_path = Application.streamingAssetsPath + @"/../../";
                }

                else if( Application.platform == RuntimePlatform.OSXPlayer ) { 
                    
                    asset_bundles_path = Application.streamingAssetsPath + @"/../../../";
                }

                asset_bundles_path = Path.Combine( asset_bundles_path, asset_bundles_default_folder );
            }

            return asset_bundles_path;
        }

        /// <summary>
        /// Return unique name of file.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
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
        /// ############################################################################################################################################################################################################################################
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