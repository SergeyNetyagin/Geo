using UnityEngine;

namespace VostokVR.Geo {

    public class LookAtTarget : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private Transform target_transform;

        [SerializeField]
        private Vector3 rotation_axles = new Vector3( 0f, 1f, 0f );

        [SerializeField]
        private Vector3 local_offset = new Vector3( 0f, 0f, 0.1f );

        [SerializeField]
        private bool invert_look = true;

        private Vector3 base_local_position = Vector3.zero;

        private Transform object_transform;

	    // Use this for initialization #####################################################################################################################################################################################################################
	    private void Awake() {

            if( target_transform == null ) { 
            
                target_transform = Camera.main.GetComponent<Transform>();
            }
        }

	    // Use this for initialization #####################################################################################################################################################################################################################
	    private void Start() {
		
            object_transform = GetComponent<Transform>();

            base_local_position = object_transform.localPosition;

            #if( UNITY_EDITOR )
            if( target_transform == null ) { 

                Debug.LogError( "Cannot create the reference to target_transform for " + gameObject.name );
            }
            #endif
	    }
	
	    // Update is called once per frame #################################################################################################################################################################################################################
	    private void Update() {
		
            object_transform.LookAt( target_transform );

            Vector3 angles = object_transform.eulerAngles;

            if( rotation_axles.x == 0f ) {
                
                angles.x = 0f;
            }

            if( rotation_axles.y == 0f ) {
                
                angles.y = 0f;
            }

            if( rotation_axles.z == 0f ) {
                
                angles.z = 0f;
            }

            if( invert_look ) {

                if( rotation_axles.x != 0f ) {
                    
                    angles.x += 180f;
                }

                if( rotation_axles.y != 0f ) {
                    
                    angles.y += 180f;
                }

                if( rotation_axles.z != 0f ) {
                    
                    angles.z += 180f;
                }
            }

            object_transform.eulerAngles = angles;

            if( local_offset != Vector3.zero ) {
            
                object_transform.localPosition = base_local_position;
                object_transform.Translate( local_offset, Space.Self );
            }
	    }
    }
}