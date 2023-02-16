using UnityEngine;

namespace VostokVR.Geo {

    public class CheckActivation : MonoBehaviour {

        private void OnEnable() {
        
            Debug.Log( gameObject.name + " is ENABLED ..." );
        }

        private void OnDisable() {
        
            Debug.Log( gameObject.name + " is disabled !!!" );
        }
    }
}