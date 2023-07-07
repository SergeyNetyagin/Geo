using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if( UNITY_EDITOR || DEBUG_MODE )
using System.Runtime.CompilerServices;
#endif

namespace VostokVR.Geo {

    /// <summary>
    /// Shows testing info about application.
    /// </summary>
    [ExecuteInEditMode]
    public class DebugManager : MonoBehaviour {

        private static DebugManager instance;
        public static DebugManager Instance => (instance != null) ? instance : (instance = FindObjectOfType<DebugManager>());

        [Space( 10 ), SerializeField]
        private const string define_debug_mode = "DEBUG_MODE";

        [Space( 10 ), SerializeField]
        private bool show_console = true;

        [SerializeField]
        private bool show_messages = true;

        [SerializeField]
        private bool show_warnings = true;

        [SerializeField]
        private bool show_errors = true;

        [SerializeField]
        private bool show_frame_rate = true;

        [Space( 10 ), SerializeField]
        private GameObject panel_debug;

        [SerializeField]
        private Text text_message;

        [SerializeField]
        private Text text_fps;

        [Space( 10 ), SerializeField]
        private Text text_video_state;

        [SerializeField]
        private Text text_video_data;

        [SerializeField]
        private Text text_video_rate;

        [Space( 10 ), SerializeField]
        private GameObject panel_debug_console;

        [SerializeField]
        private Text console_field;

        [Space( 10 ), SerializeField]
        private bool test_notch_in_editor = false;
        public bool Test_notch_in_editor { get { return test_notch_in_editor; } }

        [SerializeField]
        private bool force_virtual_notch_on_device = false;
        public bool Force_virtual_notch_on_device { get { return force_virtual_notch_on_device; } }

        [SerializeField]
        private bool test_screen_resolution_in_editor = false;
        public bool Test_screen_resolution_in_editor { get { return test_screen_resolution_in_editor; } }

        [SerializeField]
        private bool system_memory_imitation_mode = false;

        [SerializeField, Range( 10, 100 )]
        private int new_line_count_limit = 33;

        [SerializeField, Range( 0.1f, 2f )]
        private float fps_check_period = 0.5f;

        [SerializeField, Range( 500, 10000 )]
        private int log_characters_limit = 5000;

        [SerializeField, Range( 512, 16384 )]
        private int system_memory_in_megabytes = 2048;
        public int System_memory_in_megabytes { get { return (system_memory_imitation_mode ? system_memory_in_megabytes : SystemInfo.systemMemorySize); } }

        [Space( 10 ), SerializeField]
        private UnityEvent onAwake;

        [SerializeField]
        private bool run_on_awake_events = false;

        [SerializeField]
        private bool hide_android_java_object_messages = true;

        [SerializeField]
        private bool hide_urp_multisampling_messages = true;

        [SerializeField]
        private bool hide_reduce_lighting_messages = true;

        [SerializeField]
        private bool hide_curved_ui_messages = true;

        [SerializeField]
        private bool hide_avpro_info_messages = true;

        [SerializeField]
        private bool hide_unexpected_timestamp_messages = true;

        public string GetMessage() { return (text_message == null) ? string.Empty : text_message.text; }

        private int calls_counter = 0;

        private float starting_time = 0f;

        private int current_frame_rate = 0;

        private string log = string.Empty;


        /// <summary>
        /// Use this for initialization.
        /// </summary>
        private void Awake() {
            
            #if( UNITY_EDITOR )
            if( !UnityEngine.Application.isPlaying ) { 

                text_message.text = "DEBUG INFO";
                console_field.text = "DEBUG CONSOLE";
                text_fps.text = "FPS: 0";

                #if( DEBUG_MODE )
                {
                    panel_debug.SetActive( true );
                    panel_debug_console.SetActive( show_console );
                    text_message.gameObject.SetActive( show_messages || show_warnings || show_errors );
                    text_fps.gameObject.SetActive( show_frame_rate );
                    console_field.gameObject.SetActive( true );
                }
                #else
                {
                    panel_debug.SetActive( false );
                    panel_debug_console.SetActive( false );
                }
                #endif

                if( !gameObject.activeSelf ) {
                
                    gameObject.SetActive( true );
                }

                return;
            }
            #endif

            instance = this;

            #if( DEBUG_MODE )
            {
                panel_debug.SetActive( true );
                panel_debug_console.SetActive( show_console );
                text_message.gameObject.SetActive( show_messages || show_warnings || show_errors );
                text_fps.gameObject.SetActive( show_frame_rate );
                console_field.gameObject.SetActive( true );
                gameObject.SetActive( true );

                if( run_on_awake_events && (onAwake != null) ) { 

                    onAwake.Invoke();
                }
            }
            #else
            {
                panel_debug.SetActive( false );
                panel_debug_console.SetActive( false );
            }
            #endif
        }


        /// <summary>
        /// Use this for initialization.
        /// </summary>
        #if( DEBUG_MODE )
        private void OnEnable() {

            text_message.text = "DEBUG INFO";
            console_field.text = "DEBUG CONSOLE";
            text_fps.text = "FPS: 0";

            starting_time = Time.unscaledTime;

            Application.logMessageReceived += DebugLog;
        }
        #endif


        /// <summary>
        /// Use this for initialization.
        /// </summary>
        #if( DEBUG_MODE )        
        private void OnDisable() {

            text_message.text = "DEBUG INFO";
            console_field.text = "DEBUG CONSOLE";
            text_fps.text = "FPS: 0";

            if( Application.isPlaying && show_console ) {

                Application.logMessageReceived -= DebugLog;
            }
        }
        #endif


        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        #if( DEBUG_MODE )        
        private void LateUpdate() { 

            // Activate/deactivate canvas
            #if( UNITY_EDITOR )
            if( !UnityEngine.Application.isPlaying ) { 

                #if( DEBUG_MODE )
                {
                    panel_debug.SetActive( true );
                    panel_debug_console.SetActive( show_console );
                    text_message.gameObject.SetActive( show_messages || show_warnings || show_errors );
                    console_field.gameObject.SetActive( true );

                    if( !gameObject.activeSelf ) {
                        
                        gameObject.SetActive( true );
                    }
                }
                #else
                {
                    if( panel_debug.activeSelf ) {
                    
                        panel_debug.SetActive( false );
                        panel_debug_console.SetActive( false );

                        ScenesManager.MarkCurrentSceneDirty();
                    }
                }
                #endif
            }
            #endif

            #if( UNITY_EDITOR )
            if( !UnityEngine.Application.isPlaying ) { 

                return;
            }
            #endif

            // Calculate FPS
            if( show_frame_rate ) {

                calls_counter++;

                if( (Time.unscaledTime - starting_time) >= fps_check_period ) {

                    current_frame_rate = Mathf.RoundToInt( calls_counter / (Time.unscaledTime - starting_time) );

                    starting_time = Time.unscaledTime;

                    calls_counter = 0;

                    #if( DEBUG_MODE )
                    ShowFPS( "FPS: " + current_frame_rate );
                    #endif
                }
            }
        }
        #endif


        /// <summary>
        /// Shows console message.
        /// </summary>
        private void DebugLog( string message, string stack_trace, LogType log_type ) {

            if( panel_debug_console == null ) { 
            
                return;
            }

            if( console_field == null ) { 
            
                return;
            }

            if( hide_android_java_object_messages && message.Contains( "UnityEngine.AndroidJavaObject" ) ) { 
            
                return;
            }

            if( hide_urp_multisampling_messages && message.Contains( "non-multisampled texture being bound to a multisampled sampler" ) ) { 
            
                return;
            }

            if( hide_reduce_lighting_messages && message.Contains( "Reduced additional punctual light shadows resolution" ) ) { 
            
                return;
            }

            if( hide_curved_ui_messages && message.Contains( "CurvedUI" ) ) { 
            
                return;
            }

            if( hide_avpro_info_messages && message.Contains( "[AVProVideo]" ) && !message.ToLower().Contains( "error" ) ) { 
            
                return;
            }

            if( hide_unexpected_timestamp_messages && message.Contains( "Unexpected timestamp values detected" ) && !message.ToLower().Contains( "error" ) ) { 

                return;
            }

            //string bracket_start = @"<color=black>";
            //string bracket_end = @"</color>";

            if( instance != null ) { 
            
                #if( DEBUG_MODE )
                instance.ShowDebugInfo( message, log_type );
                #endif
            }

            /*if( log_type == LogType.Log ) { 
                
                bracket_start = @"<color=black>";
            }

            else if( log_type == LogType.Warning ) {

                bracket_start = @"<color=orange>";
            }

            else if( log_type == LogType.Error ) {

                bracket_start = @"<color=red>";
            }

            else if( log_type == LogType.Exception ) {

                bracket_start = @"<color=red>";
            }*/

            if( !show_console ) { 
            
                return;
            }

            if( !panel_debug_console.activeSelf ) {

                panel_debug_console.SetActive( true );
            }

            if( log.Length > log_characters_limit ) {

                log = log.Substring( 0, log_characters_limit );
                //log = log.Substring( 0, log.LastIndexOf( "<color" ) );
            }

            int new_line_count = Regex.Matches( log, Environment.NewLine ).Count;

            if( new_line_count > new_line_count_limit ) { 
            
                for( int i = (new_line_count - new_line_count_limit); i >= 0; i-- ) { 
                    
                    int index = log.IndexOf( Environment.NewLine );

                    if( index > 0 ) {
                    
                        log = log.Substring( index );
                    }
                }
            }

            log += Environment.NewLine /*+ bracket_start*/ + message /*+ bracket_end*/;

            console_field.text = log;
        }


        /// <summary>
        /// Shows message with .NET detail source info.
        /// </summary>
        #if( UNITY_EDITOR || DEBUG_MODE )
        public static void ReportMessage( string message, [CallerFilePath] string calling_file_path = "", [CallerMemberName] string calling_method = "", [CallerLineNumber] int calling_file_line_number = 0 ) {
        #else
        public static void ReportMessage( string message ) {
        #endif

            if( instance != null ) { 
            
                #if( DEBUG_MODE )
                instance.ShowDebugInfo( message, LogType.Log, calling_file_path, calling_method, calling_file_line_number );
                #endif
            }
        }


        /// <summary>
        /// Shows warning with .NET detail source info.
        /// </summary>
        #if( UNITY_EDITOR || DEBUG_MODE )
        public static void ReportWarning( string message, [CallerFilePath] string calling_file_path = "", [CallerMemberName] string calling_method = "", [CallerLineNumber] int calling_file_line_number = 0 ) {
        #else
        public static void ReportWarning( string message ) {
        #endif

            if( instance != null ) { 
            
                #if( DEBUG_MODE )
                instance.ShowDebugInfo( message, LogType.Warning, calling_file_path, calling_method, calling_file_line_number );
                #endif
            }
        }


        /// <summary>
        /// Shows error with .NET detail source info.
        /// </summary>
        #if( UNITY_EDITOR || DEBUG_MODE )
        public static void ReportError( string message, [CallerFilePath] string calling_file_path = "", [CallerMemberName] string calling_method = "", [CallerLineNumber] int calling_file_line_number = 0 ) {
        #else
        public static void ReportError(string message ) {
        #endif

            if( instance != null ) { 

                #if( DEBUG_MODE )
                instance.ShowDebugInfo( message, LogType.Error, calling_file_path, calling_method, calling_file_line_number );
                #endif
            }
        }


        /// <summary>
        /// Shows error with .NET detail source info.
        /// </summary>
        #if( UNITY_EDITOR || DEBUG_MODE )
        public static void ReportException( string message, [CallerFilePath] string calling_file_path = "", [CallerMemberName] string calling_method = "", [CallerLineNumber] int calling_file_line_number = 0 ) {
        #else
        public static void ReportException(string message ) {
        #endif

            if( instance != null ) { 

                #if( DEBUG_MODE )
                instance.ShowDebugInfo( message, LogType.Exception, calling_file_path, calling_method, calling_file_line_number );
                #endif
            }
        }


        /// <summary>
        /// Shows FPS.
        /// </summary>
        public void ShowFPS( string fps ) {

            if( show_frame_rate ) { 
            
                text_fps.text = fps;
            }

            else {

                text_fps.text = string.Empty;
            }
        }


        /// <summary>
        /// Shows the player state.
        /// </summary>
        public void ShowPlayerState( string player_state ) {

            //text_video_state.text = "Video state: " + player_state;
        }


        /// <summary>
        /// Shows the player data.
        /// </summary>
        public void ShowPlayerData( string player_data ) {

            //text_video_data.text = "Video stream position: " + player_data;
        }


        /// <summary>
        /// Shows video real frame rate.
        /// </summary>
        public void ShowFrameRate( float frame_rate ) {

            if( frame_rate < 0 ) { 
            
                frame_rate = 0;
            }

            //text_video_rate.text = "Streaming frame rate: " + frame_rate;
        }


        /// <summary>
        /// Shows message with .NET detail source info.
        /// </summary>
        #if( DEBUG_MODE )
        private void ShowDebugInfo( string message, LogType message_type = LogType.Log, [CallerFilePath] string calling_file_path = "", [CallerMemberName] string calling_method = "", [CallerLineNumber] int calling_file_line_number = 0 ) {

            string sender = string.Empty;// string.IsNullOrEmpty( calling_file_path ) ? string.Empty : StreamHelper.ArrangeSlashes( System.IO.Path.GetFileNameWithoutExtension( calling_file_path ) );

            if( !string.IsNullOrEmpty( sender ) ) {

                if( sender.LastIndexOf( "." ) > 0 ) { 
                
                    sender = sender.Substring( 0, sender.LastIndexOf( "." ) );
                }

                if( sender.LastIndexOf( @"/" ) > 0 ) { 
                
                    sender = sender.Substring( sender.LastIndexOf( @"/" ) + 1 );
                }

                if( sender.LastIndexOf( @"\" ) > 0 ) { 
                
                    sender = sender.Substring( sender.LastIndexOf( @"\" ) + 1 );
                }

                if( !string.IsNullOrEmpty( sender ) && !string.IsNullOrEmpty( calling_method ) ) { 
            
                    sender += "." + calling_method;
                }

                if( !string.IsNullOrEmpty( sender ) && (calling_file_line_number != 0) ) { 
            
                    sender += ", line #" + calling_file_line_number;
                }

                if( !string.IsNullOrEmpty( sender ) ) {
            
                    sender += " >>> ";
                }

                else { 
            
                    sender = string.Empty;
                }
            }

            if( !panel_debug.gameObject.activeSelf ) {

                panel_debug.gameObject.SetActive( true );
            }

            if( !text_message.gameObject.activeSelf && (show_messages || show_warnings || show_errors ) ) {

                text_message.gameObject.SetActive( true );
            }

            string report = string.IsNullOrEmpty( message ) ? Environment.NewLine : (sender + message);
            //string bracket_start = @"<color=gray>";
            //string bracket_end = @"</color>";

            /*if( (message_type == LogType.Log) && show_messages ) {

                bracket_start = @"<color=gray>";                
            }

            else if( (message_type == LogType.Warning) && show_warnings ) {
                
                bracket_start = @"<color=orange>";
            }

            else if( (message_type == LogType.Error) && show_errors ) {
                
                bracket_start = @"<color=red>";
            }

            else if( (message_type == LogType.Exception) && show_errors ) {

                bracket_start = @"<color=red>";
            }*/

            //report = bracket_start + report + bracket_end;                
            text_message.text = report;
        }
        #endif


        /// <summary>
        /// Creates checking LineRenderer in order to make the specified points visible.
        /// </summary>
        #if( DEBUG_MODE )
        [ContextMenu( "Clear debug console" )]
        public void ClearDebugConsole() { 
            
            log = string.Empty;

            console_field.text = "DEBUG CONSOLE";
        }
        #endif
    }
}