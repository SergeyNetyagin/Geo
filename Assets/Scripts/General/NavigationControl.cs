using UnityEngine;

namespace VostokVR.Geo {

    public class NavigationControl : MonoBehaviour {

        public static NavigationControl Instance { get; private set; }

        // Awake is called before the first frame update ##################################################################################################################################################################################################
        private void Awake() {
        
            Instance = this;
        }

        // Start is called before the first frame update ##################################################################################################################################################################################################
        private void Start() {
        
        }
    }
}