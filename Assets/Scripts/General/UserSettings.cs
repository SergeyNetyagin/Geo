using UnityEngine;

namespace VostokVR.Geo {

    public class UserSettings : MonoBehaviour {

        public static UserSettings Instance { get; private set; }

        public static string Config_path { get { return Application.persistentDataPath + "/"; } }
        public static string Config_full_name { get { return (Config_path + Instance.Config_file_name); } }

        private static string player_name_key = "Player_name";
        private static string player_avatar_key = "Player_avatar";
        private static string player_color_key_a = "Player_color_a";
        private static string player_color_key_r = "Player_color_r";
        private static string player_color_key_g = "Player_color_g";
        private static string player_color_key_b = "Player_color_b";

        [Space( 10 ), SerializeField]
        private string config_file_name = "User.cfg";
        public string Config_file_name { get { return Config_file_name; } }

        [Space( 10 ), SerializeField, Tooltip( "The preferred name of the player for the his personal build" )]
        private string preferred_player_name = string.Empty;
        public string Preferred_player_name { get { return preferred_player_name; } }

        [SerializeField, Tooltip( "The preferred color of the player for the his personal build" )]
        private Color preferred_player_color = Color.clear;
        public Color Preferred_player_color { get { return preferred_player_color; } }
   
        [Space( 10 ), SerializeField]
        private string[] player_names;
        public string[] Player_names { get { return player_names; } }

        [Space( 10 ), SerializeField]
        private Color[] player_colors;
        public Color[] Player_colors { get { return player_colors; } }

        private int current_name_index = 0;
        public int Current_name_index { get { return current_name_index; } }

        private int current_color_index = 0;
        public int Current_color_index { get { return current_color_index; } }

        private int current_avatar_index = 0;
        public int Current_avatar_index { get { return current_avatar_index; } }

        // Use this for initialization ##############################################################################################################################################
	    void Awake() {

            if( Instance != null ) return;
            else Instance = this;

            Instance.preferred_player_name = LoadPreferredPlayerName();
            Instance.preferred_player_color = LoadPreferredPlayerColor();
        }

        // Use this for initialization ##############################################################################################################################################
	    void Start() {
		
        }
    
        // Returns a player name ####################################################################################################################################################
        private string LoadPreferredPlayerName() {

            if( PlayerPrefs.HasKey( player_name_key ) ) preferred_player_name = PlayerPrefs.GetString( player_name_key );

            // Set the name if a preffered name is not defined
            if( (preferred_player_name == null) || (preferred_player_name.Length == 0) ) {

                current_name_index = Random.Range( 0, player_names.Length );
            }

            // Change the size of array of names if there is the unique preferred name of the player
            else if( !HasName( preferred_player_name ) ) {

                current_name_index = 0;

                string[] extended_names = new string[ player_names.Length + 1 ];
                extended_names[ current_name_index ] = preferred_player_name;
                for( int i = 0; i < player_names.Length; i++ ) extended_names[ i + 1 ] = player_names[i];

                player_names = extended_names;
            }

            // Use existing name as it was loaded from PlayerPrefs
            else {

                current_name_index = GetNameIndex( preferred_player_name );
            }

            return GetName( current_name_index );
        }

        private bool HasName( string name ) {

            for( int i = 0; i < player_names.Length; i++ ) {

                if( player_names[i] == name ) return true;
            }

            return false;
        }

        private int GetNameIndex( string name ) {

            for( int i = 0; i < player_names.Length; i++ ) {

                if( player_names[i] == name ) return i;
            }

            return 0;
        }
    
        public void SetSpecificName( int name_index ) {

            current_name_index = name_index;
            preferred_player_name = player_names[ current_name_index ];
        }

        // Returns a player color ###################################################################################################################################################
        private Color LoadPreferredPlayerColor() {

            if( PlayerPrefs.HasKey( player_color_key_a ) ) preferred_player_color.a = PlayerPrefs.GetFloat( player_color_key_a );
            if( PlayerPrefs.HasKey( player_color_key_r ) ) preferred_player_color.r = PlayerPrefs.GetFloat( player_color_key_r );
            if( PlayerPrefs.HasKey( player_color_key_g ) ) preferred_player_color.g = PlayerPrefs.GetFloat( player_color_key_g );
            if( PlayerPrefs.HasKey( player_color_key_b ) ) preferred_player_color.b = PlayerPrefs.GetFloat( player_color_key_b );

            // Set the color if a preffered color is not defined
            if( preferred_player_color == Color.clear ) {

                current_color_index = Random.Range( 0, player_colors.Length );
            }
        
            // Change the size of array of colors if there is the unique preferred color of the player
            else if( !HasColor( preferred_player_color ) ) {

                current_color_index = 0;

                Color[] extended_colors = new Color[ player_colors.Length + 1 ];
                extended_colors[ current_color_index ] = preferred_player_color;
                for( int i = 0; i < player_colors.Length; i++ ) extended_colors[ i + 1 ] = player_colors[i];

                player_colors = extended_colors;
            }

            // Use existing color as it was loaded from PlayerPrefs
            else {

                current_color_index = GetColorIndex( preferred_player_color );
            }

            return GetColor( current_color_index );
        }
    
        private bool HasColor( Color color ) {

            for( int i = 0; i < player_colors.Length; i++ ) {

                if( player_colors[i] == color ) return true;
            }

            return false;
        }

        private int GetColorIndex( Color color ) {

            for( int i = 0; i < player_colors.Length; i++ ) {

                if( player_colors[i] == color ) return i;
            }

            return 0;
        }
    
        public Color GetNextColor() {

            current_color_index++;

            if( current_color_index >= player_colors.Length ) current_color_index = 0;

            preferred_player_color = GetColor( current_color_index );

            return preferred_player_color;
        }

        // Sets a player name #######################################################################################################################################################
        private void SavePreferredPlayerName( string player_name ) {

            PlayerPrefs.SetString( player_name_key, player_name );
        }

        // Sets a player color ######################################################################################################################################################
        private void SavePreferredPlayerColor( Color color ) {

            PlayerPrefs.SetFloat( player_color_key_a, preferred_player_color.a );
            PlayerPrefs.SetFloat( player_color_key_r, preferred_player_color.r );
            PlayerPrefs.SetFloat( player_color_key_g, preferred_player_color.g );
            PlayerPrefs.SetFloat( player_color_key_b, preferred_player_color.b );
        }
    
        // Sets a player avatar ######################################################################################################################################################
        private void SavePreferredPlayerAvatar( int avatar_index ) {

            PlayerPrefs.SetInt( player_avatar_key, avatar_index );
        }
 
        // Returns a specified color ################################################################################################################################################
        private Color GetColor( int index ) {

            if( player_colors.Length > 0 ) index %= player_colors.Length;

            if( (player_colors.Length == 0) || (index >= player_colors.Length) ) return Color.black;
            else return player_colors[ index ];
        }

        // Returns a specified name #################################################################################################################################################
        private string GetName( int index ) {

            if( player_names.Length > 0 ) index %= player_names.Length;

            if( (player_names.Length == 0) || (index >= player_names.Length) ) return string.Empty;
            else return player_names[ index ];
        }

        // Сохранение конфигурации ##################################################################################################################################################
        public bool Save() {

            return false;
        }
  
        // Загрузка конфигурации ####################################################################################################################################################
        public bool Load() {

            return false;
        }
    }
}