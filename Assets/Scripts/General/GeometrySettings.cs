using UnityEngine;

namespace VostokVR.Geo {

    public class GeometrySettings : MonoBehaviour {

        public static GeometrySettings Instance { get; private set; }

        [Space( 10 ), SerializeField, Range( 0f, 2f )]
        private float head_altitude = 1.75f;
        public float Head_altitude { get { return head_altitude; } }

        [SerializeField, Range( 1f, 2f )]
        private float mouth_altitude = 1.65f;
        public float Mouth_altitude { get { return mouth_altitude; } }

        [Space( 10 ), SerializeField, Range( 0.1f, 1f )]
        private float private_space_radius = 0.65f;
        public float Private_space_radius { get { return private_space_radius; } }

        [SerializeField, Range( 0.05f, 0.5f )]
        private float player_collider_radius = 0.3f;
        public float Player_collider_radius { get { return player_collider_radius; } }

	    // Use this for initialization #####################################################################################################################################################################################################################
	    private void Awake() {

            if( Instance == null ) Instance = this;
	    }
    
	    // Use this for initialization #####################################################################################################################################################################################################################
	    private void Start() {
		
        }
    }
}