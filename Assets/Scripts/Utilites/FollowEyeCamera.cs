using UnityEngine;

namespace VostokVR.Geo {

    public class FollowEyeCamera : MonoBehaviour {

        private Transform cached_transform;

        private Vector3 position = Vector3.zero;

	    // Use this for initialization
	    void Awake() {
		
            cached_transform = GetComponent<Transform>();
	    }

	    // Use this for initialization
	    void Start() {
		
            position = cached_transform.position;
	    }
	
	    // Update is called once per frame
	    void Update () {
		
            //position.x = ControllerData.Instance.Eye_camera_transform.position.x;
            //position.z = ControllerData.Instance.Eye_camera_transform.position.z;

            cached_transform.position = position;
	    }
    }
}