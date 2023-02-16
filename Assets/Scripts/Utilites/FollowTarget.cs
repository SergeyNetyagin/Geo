using UnityEngine;

namespace VostokVR.Geo {

    public class FollowTarget : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private Transform target_transform;

        [SerializeField]
        RigidbodyConstraints constraints = RigidbodyConstraints.FreezePositionY;

        private Transform cached_transform;

        private Vector3 starting_position = Vector3.zero;
        private Vector3 starting_rotation = Vector3.zero;

        private Vector3 current_position = Vector3.zero;
        private Vector3 current_rotation = Vector3.zero;

        // Use this for initialization #####################################################################################################################################################################################################################
	    private void OnEnable() {

            if( target_transform == null ) { 
            
                target_transform = Camera.main.GetComponent<Transform>();
            }
        }

        // Use this for initialization #####################################################################################################################################################################################################################
	    private void Start() {
		
            cached_transform = GetComponent<Transform>();

            starting_position = cached_transform.position;
            starting_rotation = cached_transform.eulerAngles;
	    }
	
	    // Update is called once per frame #################################################################################################################################################################################################################
	    private void Update() {
		
            current_position.x = target_transform.position.x;
            current_position.y = target_transform.position.y;
            current_position.z = target_transform.position.z;

            cached_transform.position = current_position;
	    }
    }
}