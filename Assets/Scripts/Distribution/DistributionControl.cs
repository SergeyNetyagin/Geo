using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexStudio.Generic;

namespace VostokVR.Geo {

    public enum ZipCompression { 

        None    = 0,
        Ratio_1 = 1,
        Ratio_2 = 2,
        Ratio_3 = 3,
        Ratio_4 = 4,
        Ratio_5 = 5,
        Ratio_6 = 6,
        Ratio_7 = 7,
        Ratio_8 = 8,
        Ratio_9 = 9
    }

    public class DistributionControl : MonoBehaviour {

        public static DistributionControl Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private string distribution_file_folder = "Data";

        [SerializeField]
        private string source_filename = @"source.config";

        [SerializeField]
        private string destination_filename = @"destination.config";

        [SerializeField]
        private ZipCompression zip_compression = ZipCompression.None;

        [SerializeField, Range( 0f, 10f )]
        private float delay_when_success = 2f;

        [SerializeField]
        private float zip_progress_time_rate = 0.5f;

        [Space( 10 ), SerializeField]
        private int app_files_processing_time_in_mins = 2;
        public int App_files_processing_time_in_mins { get { return app_files_processing_time_in_mins; } }

        [SerializeField]
        private int asset_bundles_processing_time_in_mins = 5;
        public int Asset_bundles_processing_time_in_mins { get { return asset_bundles_processing_time_in_mins; } }

        [Space( 10 ), SerializeField]
        private string[] especial_folders;

        private int current_available_space = 0;
        private int current_required_space = 0;

        private string[] source_files = null;
        private string[] app_source_files = null;
        private string[] asset_bundles_source_files = null;

        private FileInfo[] files_info;

        private Coroutine zip_making_coroutine = null;

        // #################################################################################################################################################################################################################################################
        private void Awake() {
        
            Instance = this;

            if( ExpirationControl.Instance.enabled ) { 
                
                ExpirationControl.Instance.enabled = false;
            }
        }

        // #################################################################################################################################################################################################################################################
        private void Start() {
        

            ExpirationControl.Instance.ReadApplicationConfigFile();
        }

        // #################################################################################################################################################################################################################################################
        private void OnApplicationQuit() {
        
            PlayerPrefs.SetString( "Geo Distribution", "Geo Distribution" );
        }

        // #################################################################################################################################################################################################################################################
        public bool IsFirstUsing() {
        
            return !PlayerPrefs.HasKey( "Geo Distribution" );
        }

        // #################################################################################################################################################################################################################################################
        public string GetDrive( string path ) {

            string default_drive = Application.streamingAssetsPath.Substring( 0, (Application.streamingAssetsPath.IndexOf( ":" ) > 0) ? Application.streamingAssetsPath.IndexOf( ":" ) + 1 : 0 );

            string[] drives = Directory.GetLogicalDrives();

            string current_drive = path.Contains( ":" ) ? path.Substring( 0, (path.IndexOf( ":" ) > 0) ? path.IndexOf( ":" ) + 1 : 0 ) : default_drive;

            for( int i = 0; i < drives.Length; i++ ) { 
                
                if( drives[i].Contains( current_drive ) ) { 
                
                    current_drive = (current_drive.Contains( @"\" ) || current_drive.Contains( @"/" )) ? current_drive : (current_drive + @"\");

                    break;
                }
            }

            return current_drive;
        }

        /// <summary>
        /// Returns free space on the specified drive.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        public int GetDriveAvailableSpaceInGygabytes( string path ) {

            if( string.IsNullOrEmpty( path ) ) { 
            
                return 0;
            }

            int available_space = 0;
            
            try {
            
                available_space = SimpleDiskUtils.DiskUtils.CheckAvailableSpace( GetDrive( path ) );
            }

            catch( Exception exception ) { 
                
                CanvasDashboard.Instance.ShowMessage( 
                    
                    "DRIVE CHECKING ERROR", 
                    "Cannot process drive or folder name!" + Environment.NewLine + 
                    exception
                );
            }

            available_space = (int) (available_space * 0.001f);

            current_available_space = available_space;

            return current_available_space;
        }

        /// <summary>
        /// Returns free space on the specified drive.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        public int GetRequiredSpaceInGygabytes( string app_files_source_folder, string asset_bundle_files_source_folder ) {

            source_files = null;
            app_source_files = null;
            asset_bundles_source_files = null;

            double required_space = 0d;

            if( Directory.Exists( app_files_source_folder ) ) {

                app_source_files = Directory.GetFiles( app_files_source_folder, "*.*", SearchOption.AllDirectories );
                asset_bundles_source_files = (asset_bundle_files_source_folder != null) ? Directory.GetFiles( asset_bundle_files_source_folder, "*.*", SearchOption.AllDirectories ) : null;

                // Collect app source files and asset bundles source files together
                if( asset_bundles_source_files != null ) {

                    List<string> files = new List<string>();

                    for( int i = 0; i < app_source_files.Length; i++ ) { 
                
                        files.Add( app_source_files[i] );
                    }

                    for( int i = 0; i < asset_bundles_source_files.Length; i++ ) { 
                 
                        files.Add( asset_bundles_source_files[i] );
                    }

                    source_files = files.ToArray();
                }

                else { 
                
                    source_files = app_source_files;
                }

                // Get info about the files...

                files_info = new FileInfo[ source_files.Length ];

                if( source_files != null ) {

                    for( int i = 0; i < source_files.Length; i++ ) { 
            
                        files_info[i] = new FileInfo( source_files[i] );

                        if( files_info[i] != null ) { 
                    
                            required_space += files_info[i].Length * 0.000000001d;
                        }
                    }
                }
            }

            current_required_space = (int) required_space;

            return current_required_space;
        }

        /// <summary>
        /// Returns directory for app source files.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        public string GetSourcePath() {

            string distribution_config_path = Path.Combine( Application.streamingAssetsPath, distribution_file_folder );
            
            distribution_config_path = Path.Combine( distribution_config_path, source_filename );

            string app_distribution_folder = string.Empty;

            try {
            
                app_distribution_folder = File.ReadAllText( distribution_config_path );
            }

            catch { 

                #if( UNITY_EDITOR )
                Debug.LogError( "Cannot read file from " + app_distribution_folder + "!" );
                #endif
            }

            return (app_distribution_folder == null) ? string.Empty : app_distribution_folder;
        }

        /// <summary>
        /// Returns directory for app destination files.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        public string GetDestinationPath() {

            string distribution_config_path = Path.Combine( Application.streamingAssetsPath, distribution_file_folder );
            
            distribution_config_path = Path.Combine( distribution_config_path, destination_filename );

            string app_distribution_folder = string.Empty;

            try {
            
                app_distribution_folder = File.ReadAllText( distribution_config_path );
            }

            catch { 

                #if( UNITY_EDITOR )
                Debug.LogError( "Cannot read file from " + app_distribution_folder + "!" );
                #endif
            }

            return (app_distribution_folder == null) ? string.Empty : app_distribution_folder;
        }

        /// <summary>
        /// Saves directory for app source files.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        public void SaveSourceFolder( string app_source_folder ) {

            string distribution_config_path = Path.Combine( Application.streamingAssetsPath, distribution_file_folder );
            
            distribution_config_path = Path.Combine( distribution_config_path, source_filename );

            bool directory_error = false;

            if( !Directory.Exists( Path.GetDirectoryName( distribution_config_path ) ) ) { 
            
                try {

                    Directory.CreateDirectory( Path.GetDirectoryName( distribution_config_path ) );
                }

                catch { 

                    directory_error = true;

                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot create directory " + Path.GetDirectoryName( distribution_config_path ) + "!" );
                    #endif                    
                }
            }

            if( !directory_error ) {

                try {
            
                    if( File.Exists( distribution_config_path ) ) { 
                    
                        File.Delete( distribution_config_path );
                    }

                    File.WriteAllText( distribution_config_path, app_source_folder );
                }

                catch { 

                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot save source folder to file " + distribution_config_path + "!" );
                    #endif
                }
            }
        }

        /// <summary>
        /// Saves directory for app destination files.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        public void SaveDestinationFolder( string app_destination_folder ) {

            string distribution_config_path = Path.Combine( Application.streamingAssetsPath, distribution_file_folder );
            
            distribution_config_path = Path.Combine( distribution_config_path, destination_filename );

            bool directory_error = false;

            if( !Directory.Exists( Path.GetDirectoryName( distribution_config_path ) ) ) { 
            
                try {

                    Directory.CreateDirectory( Path.GetDirectoryName( distribution_config_path ) );
                }

                catch { 

                    directory_error = true;

                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot create directory " + Path.GetDirectoryName( distribution_config_path ) + "!" );
                    #endif                    
                }
            }

            if( !directory_error ) {

                try {
            
                    if( File.Exists( distribution_config_path ) ) { 
                    
                        File.Delete( distribution_config_path );
                    }

                    File.WriteAllText( distribution_config_path, app_destination_folder );
                }

                catch { 

                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot save source folder to file " + distribution_config_path + "!" );
                    #endif
                }
            }
        }

        /// <summary>
        /// Stops making the zip app.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        private bool TheFolderIsSubfolder( string checking_folder, string parent_folder ) { 
        
            bool is_subfolder = false;

            /* Way #1 */ {

                //Uri parent_uri = new Uri( parent_folder );
                //Uri child_uri = new Uri( checking_folder );

                //is_subfolder = parent_uri.IsBaseOf( child_uri );
            }

            /* Way #2 */{

                Uri parent_uri = new Uri( parent_folder );
                DirectoryInfo child_uri = new DirectoryInfo( checking_folder ).Parent;

                while( child_uri != null) {

                    if( new Uri( child_uri.FullName ) == parent_uri ) {

                        is_subfolder = true;

                        break;
                    }
                    
                    child_uri = child_uri.Parent;
                }
            }

            return is_subfolder;
        }

        /// <summary>
        /// Stops making the zip app.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        public void StopMakingZipApplication() {

            zip_making_coroutine = null;
        }

        /// <summary>
        /// Makes the zip app for distribution.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        public void MakeZipApplication( bool delete_files_on_finish ) {

            // Check for insufficient disk space
            if( current_available_space < current_required_space ) {

                CanvasDashboard.Instance.ShowMessage( 
                    
                    "INSUFFICIENT DISK SPACE!", 
                    "Please try to free up enough disk space," + Environment.NewLine +
                    "or try to change destination drive."
                );

                return;
            }

            string source_foder = CanvasDashboard.Instance.GetSourceFolder();
            string destination_foder = CanvasDashboard.Instance.GetDestinationFolder();

            // Check for subfolder
            if( TheFolderIsSubfolder( destination_foder, source_foder ) ) {

                CanvasDashboard.Instance.ShowMessage( 
                    
                    "FOLDER NESTING ERROR!", 
                    "The destination folder must not be a child of the source folder." + Environment.NewLine +
                    "Please type a correct destination folder path."
                );

                return;
            }

            // Check for the same folder names
            if( destination_foder == source_foder ) {

                CanvasDashboard.Instance.ShowMessage( 
                    
                    "FOLDER COINCIDENCE ERROR!", 
                    "The destination folder must be different from the source folder." + Environment.NewLine +
                    "Please type a correct destination folder path."
                );

                return;
            }

            // If ok then go to zip file making
            zip_making_coroutine = StartCoroutine( MakeZipApplicationProcess( delete_files_on_finish ) );
        }

        /// <summary>
        /// Creates the zip app for distribution process.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        private IEnumerator MakeZipApplicationProcess( bool delete_files_on_finish ) {

            yield return new WaitForEndOfFrame();

            string application_config_file_path = string.Empty;

            List<string> destination_files_list = new List<string>();

            string[] destination_files = null;

            string zip_file_name = null;

            ulong[] zip_asset_progress = new ulong[1] { 0 };

            float progress = 0f;

            int zip_error_code = 0;

            bool is_error = false;
            bool is_cancel = false;

            string source_folder = GetSourcePath();
            string destination_folder = GetDestinationPath();

            int stage_index = 1;

            CanvasDashboard.Instance.ShowProgress( "STAGE " + stage_index + ": COPYING FILES TO DESTINATION", "Complete: 0 %" );

            // STEP #1: prepare source folder
            if( !Directory.Exists( source_folder ) ) { 
                
                CanvasDashboard.Instance.ShowMessage( 
                    
                    "SOURCE FOLDER ERROR", 
                    "Cannot find the specified source folder!" + Environment.NewLine + 
                    "Check the folder existance or correct the path."
                );

                is_error = true;

                goto LabelCopyFilesStepFinished;
            }

            // STEP #2: prepare destination folder
            try {
            
                if( !Directory.Exists( destination_folder ) ) { 
                    
                    Directory.CreateDirectory( destination_folder );
                }

                else { 
                    
                    CanvasDashboard.Instance.ShowMessage( 
                    
                        "DESTINATION FOLDER PROBLEM", 
                        "The specified destination folder already exist." + Environment.NewLine + 
                        "Perhaps it contains an important data." + Environment.NewLine + 
                        "Rename the existing folder or remove it before zipping action."
                    );

                    is_error = true;

                    goto LabelCopyFilesStepFinished;
                }
            }

            catch( Exception exception ) { 

                CanvasDashboard.Instance.ShowMessage( 
                    
                    "DESTINATION FOLDER ERROR", 
                    "Cannot prepare the specified destination folder!" + Environment.NewLine + 
                    "Please check and correct the folder name." + Environment.NewLine + 
                    "The reason: " + exception.Message
                );

                #if( UNITY_EDITOR )
                Debug.LogError( "Destination folder creation error: " + exception.Message );
                #endif

                is_error = true;

                goto LabelCopyFilesStepFinished;            
            }

            // STEP #3: create all of the directories and copy the files...

            string asset_bundles_source_folder = CanvasDashboard.Instance.GetAssetBundlesFolder();

            if( asset_bundles_source_folder == null ) { 
            
                asset_bundles_source_folder = string.Empty;
            }

            for( int i = 0; i < source_files.Length; i++ ) {

                string new_path = string.Empty;
                    
                if( CanvasDashboard.Instance.Include_asset_bundles && source_files[i].Contains( asset_bundles_source_folder ) ) {
                    
                    new_path = source_files[i].Replace( asset_bundles_source_folder, Path.Combine( destination_folder, ConfigControl.Instance.Asset_bundles_default_folder ) );
                }

                else { 
                    
                    new_path = source_files[i].Replace( source_folder, destination_folder );
                }

                string new_destination_folder = Path.GetDirectoryName( new_path );

                if( new_path.Contains( ExpirationControl.Instance.Expiration_filename ) ) { 
                    
                    application_config_file_path = new_path;
                }

                if( !Directory.Exists( new_destination_folder) ) {

                    try {
                
                        #if( UNITY_EDITOR )
                        //Debug.Log( "Create folder " + new_destination_folder );
                        #endif

                        Directory.CreateDirectory( new_destination_folder );
                    }

                    catch( Exception exception ) { 

                        CanvasDashboard.Instance.ShowMessage( 
                    
                            "FOLDER CREATION ERROR", 
                            "Cannot create some application folders!" + Environment.NewLine + 
                            "Please check free disk space." + Environment.NewLine + 
                            "The reason: " + exception.Message
                        );
            
                        #if( UNITY_EDITOR )
                        Debug.LogError( "Destination subfolders creation error: " + exception.Message );
                        #endif

                        is_error = true;

                        goto LabelCopyFilesStepFinished;
                    }
                }

                destination_files_list.Add( new_path );

                try {    
                
                    #if( UNITY_EDITOR )
                    //Debug.Log( i + ") Copy file " + source_files[i] + " to " + new_path );
                    #endif

                    File.Copy( source_files[i], new_path, true );
                }

                catch( Exception exception ) { 

                    CanvasDashboard.Instance.ShowMessage( 
                    
                        "FILE COPYING ERROR", 
                        "Cannot copy some files to destination folder!" + Environment.NewLine + 
                        "Please check free disk space." + Environment.NewLine + 
                        "The reason: " + exception.Message
                    );
            
                    #if( UNITY_EDITOR )
                    Debug.LogError( "File copying error: " + exception.Message );
                    #endif

                    is_error = true;

                    goto LabelCopyFilesStepFinished;
                }

                if( zip_making_coroutine == null ) { 
                
                    is_cancel = true;

                    goto LabelCopyFilesStepFinished;
                }

                if( (files_info != null) && (files_info.Length == source_files.Length) ) {
                
                    progress += ((float) (files_info[i].Length * 0.000000001d)) / current_required_space * 100f * (1f - zip_progress_time_rate);

                    if( progress > ((1f - zip_progress_time_rate) * 100f) ) { 
                            
                        progress = (1f - zip_progress_time_rate) * 100f;
                    }

                    CanvasDashboard.Instance.ShowProgress( "STAGE " + stage_index + ": COPYING FILES TO DESTINATION", "Complete: " + ((int) progress) + " %" );
                }

                else { 

                    CanvasDashboard.Instance.ShowProgress( "STAGE " + stage_index + ": COPYING FILES TO DESTINATION", "Processing..." );
                }

                yield return null;
            }

            LabelCopyFilesStepFinished: {

                if( is_cancel || is_error ) {
               
                    CanvasDashboard.Instance.HideProgress();                 

                    goto LabelDeleteFilesStep;
                }

                else {
            
                    CanvasDashboard.Instance.ShowProgress( "STAGE " + stage_index + ": COPYING FILES TO DESTINATION", "Complete: " + ((int) ((1f - zip_progress_time_rate) * 100f)) + " %" );

                    yield return new WaitForSeconds( delay_when_success * 0.5f );
                }
            }

            #if( UNITY_EDITOR )
            if( File.Exists( Path.Combine( @"D:\Geo Distribution Branch Office\Geo Next Generation Networking_Data\StreamingAssets\Data", ExpirationControl.Instance.Expiration_filename ) ) ) {

                destination_files = Directory.GetFiles( destination_folder, "*.*", SearchOption.AllDirectories );

                application_config_file_path = Path.Combine( @"D:\Geo Distribution Branch Office\Geo Next Generation Networking_Data\StreamingAssets\Data", ExpirationControl.Instance.Expiration_filename );
            }
            #endif

            // STEP #4: create config file
            if( !is_error && !string.IsNullOrEmpty( application_config_file_path ) && (ExpirationControl.Instance != null) ) { 
                
                ExpirationControl.Instance.SetLimitedExpirationDate( 
                
                    CanvasDashboard.Instance.GetYear(),
                    CanvasDashboard.Instance.GetMonth(),
                    CanvasDashboard.Instance.GetDay()
                );

                ExpirationControl.Instance.SaveApplicationConfigFile( false, application_config_file_path );
            }

            else { 
                
                is_error = true;

                CanvasDashboard.Instance.ShowMessage( 
                    
                    "CONFIG FILE CREATION ERROR", 
                    "Cannot copy config file to destination folder!"
                );
            
                #if( UNITY_EDITOR )
                Debug.LogError( "Config file creation error!" );
                #endif

                goto LabelEndCoroutine;
            }

            // STEP #4a BEFORE ZIPPING: make empty asset bundles config file if the app includes asset bundle files
            if( CanvasDashboard.Instance.Include_asset_bundles ) { 

                string destination_asset_bundles_config_path = destination_asset_bundles_config_path = Path.Combine( destination_folder, ConfigControl.Instance.Config_files_runtime_folder );

                destination_asset_bundles_config_path = Path.Combine( destination_asset_bundles_config_path, ConfigControl.Instance.Assetbundles_config_file_name );

                if( File.Exists( destination_asset_bundles_config_path ) ) {

                    try { 
                        
                        File.Delete( destination_asset_bundles_config_path );
                    }

                    catch { 
                        
                        #if( UNITY_EDITOR )
                        Debug.LogError( "Cannot delete file " + destination_asset_bundles_config_path );
                        #endif
                    }
                }

                if( !File.Exists( destination_asset_bundles_config_path ) ) {

                    using( FileStream file_stream = File.Create( destination_asset_bundles_config_path ) ) {

                        file_stream.Write( new byte[0] { }, 0, 0 );
                    }

                    #if( UNITY_EDITOR )
                    Debug.Log( "The empty asset bundles config file created especially to load asset bundles from the new internal app folder" );
                    #endif
                }
            }  

            progress = (1f - zip_progress_time_rate) * 100f;

            destination_files = destination_files_list.ToArray();

            // STEP #5: make zip archive
            if( !is_error && !string.IsNullOrEmpty( application_config_file_path ) && (ExpirationControl.Instance != null) ) { 
             
                CanvasDashboard.Instance.ShowProgress( "STAGE " + (++ stage_index) + ": CREATING APPLICATION ZIP FILE", "Complete: " + ((int) progress) + " %" );

                zip_file_name = Path.Combine( destination_folder, Path.GetFileNameWithoutExtension( destination_folder ) ) + ".zip";

                for( int i = 0; i < destination_files.Length; i++ ) {

                    string final_internal_file_name = destination_files[i].Replace( destination_folder, string.Empty );

                    #if( UNITY_EDITOR )
                    Debug.Log( "Compression of " + final_internal_file_name + " ..." );
                    #endif

                    try {

                        zip_error_code = lzip.compress_File(

                            (int) zip_compression,
                            zip_file_name,
                            destination_files[i],
                            true,
                            final_internal_file_name,
                            null,
                            null,
                            false,
                            0,
                            zip_asset_progress
                        );
                    }

                    catch( Exception exception ) {

                        #if( UNITY_EDITOR )
                        Debug.LogError( exception );
                        #endif

                        is_error = true;
                    }

                    finally {

                        if( zip_error_code != 1 ) {

                            is_error = true;
                        }
                    }

                    if( is_error ) { 

                        break;
                    }

                    if( (files_info != null) && (files_info.Length == destination_files.Length) ) {
                
                        progress += ((float) (files_info[i].Length * 0.000000001d)) / current_required_space * 100f * zip_progress_time_rate;

                        if( progress > 99f ) { 
                            
                            progress = 99f;
                        }

                        CanvasDashboard.Instance.ShowProgress( "STAGE " + stage_index + ": CREATING APPLICATION ZIP FILE", "Complete: " + ((int) progress) + " %" );
                    }

                    else { 

                        CanvasDashboard.Instance.ShowProgress( "STAGE " + stage_index + ": CREATING APPLICATION ZIP FILE", "Processing..." );
                    }

                    yield return null;
                }
            }

            if( is_error ) { 
                
                CanvasDashboard.Instance.ShowMessage( 
                    
                    "ZIP FILE CREATION ERROR", 
                    "Cannot create zip file in destination folder!" + Environment.NewLine +
                    "Please check free disk space."
                );
            
                #if( UNITY_EDITOR )
                Debug.LogError( "Zip file creation error!" );
                #endif

                goto LabelEndCoroutine;
            }

            // STEP #6: delete destination files
            LabelDeleteFilesStep: {            

                if( delete_files_on_finish && (destination_files != null) ) {

                    for( int i = 0; i < destination_files.Length; i++ ) { 
                
                        if( File.Exists( destination_files[i] ) ) { 

                            #if( UNITY_EDITOR )
                            Debug.Log( "Delete file " + destination_files[i] );
                            #endif
                   
                            try { 
                            
                                File.Delete( destination_files[i] );
                            }

                            catch { 

                                #if( UNITY_EDITOR )
                                Debug.LogError( "Cannot delete file " + destination_files[i] );
                                #endif
                            }
                        }
                    }
                   
                    for( int i = 0; i < destination_files.Length; i++ ) { 

                        string directory = Path.GetDirectoryName( destination_files[i] );
                      
                        if( (directory != null) && (directory != destination_folder) && Directory.Exists( directory ) ) {

                            #if( UNITY_EDITOR )
                            Debug.Log( "1-st stage of removing folder " + directory );
                            #endif

                            bool deletion_error = false;

                            try { 
                            
                                Directory.Delete( directory, true );
                            }

                            catch { 
                            
                                deletion_error = true;
                            }

                            if( deletion_error ) { 

                                try { 
                            
                                    File.Delete( directory );
                                }                                

                                catch { 

                                    #if( UNITY_EDITOR )
                                    Debug.LogError( "Cannot remove folder " + directory );
                                    #endif                                    
                                }
                            }
                        }
                    }

                    for( int i = 0; i < especial_folders.Length; i++ ) { 

                        string directory = Path.GetDirectoryName( Path.Combine( destination_folder, especial_folders[i] ) );
                      
                        if( (directory != null) && (directory != destination_folder) && Directory.Exists( directory ) ) {

                            #if( UNITY_EDITOR )
                            Debug.Log( "2-nd stage of removing folder " + directory );
                            #endif
                            
                            bool deletion_error = false;

                            try { 
                            
                                Directory.Delete( directory, true );
                            }

                            catch { 
                            
                                deletion_error = true;
                            }

                            if( deletion_error ) { 

                                try { 
                            
                                    File.Delete( directory );
                                }                                

                                catch { 

                                    #if( UNITY_EDITOR )
                                    Debug.LogError( "Cannot remove folder " + directory );
                                    #endif                                    
                                }
                            }
                        }
                    }
                }

                if( !is_error ) {

                    CanvasDashboard.Instance.ShowProgress( "STAGE " + stage_index + ": CREATING APPLICATION ZIP FILE", "Complete: 100 %" );

                    yield return new WaitForSeconds( delay_when_success );
                }
            }

            // STEP #7: final message if success
            if( !is_cancel && !is_error ) {
          
                CanvasDashboard.Instance.ShowMessage( 
                    
                    "ZIP APPLICATION FILE CREATED", 
                    "You can use the file for distribution." + Environment.NewLine + 
                    "Location: " + Path.Combine( destination_folder, Path.GetFileNameWithoutExtension( destination_folder ) + ".zip" )
                );
            }

            LabelEndCoroutine: {

                zip_making_coroutine = null;
            }

            yield break;
        }
    }
}