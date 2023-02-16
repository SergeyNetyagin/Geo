using UnityEngine;
using VertexStudio.VirtualRealty;

namespace VostokVR.Geo {

    public class PlayAreaMarker : MonoBehaviour {

        public static PlayAreaMarker Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private GameObject play_area_marker_prefab;

        private BoxCollider play_area_box_collider;

        private Transform play_area_marker_transform;

        private SteamVR_PlayArea steam_play_area;

        // Use this for initialization #####################################################################################################################################################################################################################
	    private void Awake() {
        
            Instance = this;
        }

        // Use this for initialization #####################################################################################################################################################################################################################
	    private void Start() {
		
        }

        // Adaptates area collider to the existing play area size ##########################################################################################################################################################################################
        public void Create() {

            if( play_area_marker_transform != null ) return;

            //play_area_marker_transform = Instantiate( play_area_marker_prefab, ControllerData.Instance.Camera_rig_transform ).GetComponent<Transform>();
            //play_area_marker_transform.name = play_area_marker_prefab.name;

            steam_play_area = ViveInteractionsManager.Instance.GetComponent<SteamVR_PlayArea>();

            play_area_box_collider = play_area_marker_transform.GetComponent<BoxCollider>();

            if( play_area_box_collider != null ) {

                Vector3 size = new Vector3( GetSizeByX(), play_area_box_collider.size.y, GetSizeByZ() );

                play_area_box_collider.size = size;
                play_area_box_collider.enabled = true;
            }
        }

        // Returns maximal size of paly area on X axle #####################################################################################################################################################################################################
        private float GetSizeByX() {

            if( !SteamVR.active ) return 0f;

            float min = float.MaxValue;
            float max = float.MinValue;

            float size = 0f;

            for( int i = 0; i < steam_play_area.vertices.Length; i++ ) {

                if( steam_play_area.vertices[i].x < min ) min = steam_play_area.vertices[i].x;
                if( steam_play_area.vertices[i].x > max ) max = steam_play_area.vertices[i].x;
            }

            size = Mathf.Abs( max - min );

            return size;
        }

        // Returns maximal size of paly area on Z axle #####################################################################################################################################################################################################
        private float GetSizeByZ() {

            if( !SteamVR.active ) return 0f;

            SteamVR_PlayArea steam_play_area = ViveInteractionsManager.Instance.GetComponent<SteamVR_PlayArea>();

            float min = float.MaxValue;
            float max = float.MinValue;

            float size = 0f;

            for( int i = 0; i < steam_play_area.vertices.Length; i++ ) {

                if( steam_play_area.vertices[i].z < min ) min = steam_play_area.vertices[i].z;
                if( steam_play_area.vertices[i].z > max ) max = steam_play_area.vertices[i].z;
            }

            size = Mathf.Abs( max - min );

            return size;
        }
    }
}