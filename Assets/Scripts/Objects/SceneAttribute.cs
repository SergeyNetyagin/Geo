using UnityEngine;

namespace VostokVR.Geo {

    public class SceneAttribute : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private string scene_name = string.Empty;
        public string Scene_name { get { return scene_name; } }

        // Start is called before the first frame update ##################################################################################################################################################################################################
        private void Start() {
        
        }
    }
}