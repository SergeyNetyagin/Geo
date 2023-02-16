using UnityEngine;

namespace VostokVR.Geo {

    public class SubjectRotation : MonoBehaviour {

        [Space( 10 ), SerializeField]
	    private bool enable_if_active = true;

        [SerializeField]
	    private Vector3 rotation_axle = new Vector3( 0f, 1f, 0f );
        public Vector3 Rotation_axle { get { return rotation_axle; } }
        public void SetRotationAxle( float x, float y, float z ) { rotation_axle.x = x; rotation_axle.y = y; rotation_axle.z = z; }

        [SerializeField]
	    private Vector3 rotation_speed = new Vector3( 0f, 50f, 0f );
        public Vector3 Rotation_speed { get { return rotation_speed; } }
        public void SetRotationSpeed( float x, float y, float z ) { rotation_speed.x = x; rotation_speed.y = y; rotation_speed.z = z; }

        private Vector3 current_rotation = Vector3.zero;

        private Transform cached_transform;

	    // Use this for initialization #####################################################################################################################################################################################################################
	    private void Awake() {
		
            cached_transform = GetComponent<Transform>();

            if( !enabled && enable_if_active ) {
                
                enabled = true;
            }

            else if( enabled && !enable_if_active ) {
                
                enabled = false;
            }
        }
        
        // Use this for initialization #####################################################################################################################################################################################################################
	    private void Start() {
		
	    }
	
	    // Update is called once per frame #################################################################################################################################################################################################################
	    private void Update() {

            current_rotation.x = rotation_axle.x * rotation_speed.x * Time.deltaTime;
            current_rotation.y = rotation_axle.y * rotation_speed.y * Time.deltaTime;
            current_rotation.z = rotation_axle.z * rotation_speed.z * Time.deltaTime;

		    transform.Rotate( current_rotation, Space.Self );
	    }
    }
}