using UnityEngine;

namespace VostokVR.Geo {

    public class VideoSettings : MonoBehaviour {

        public static VideoSettings Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private Material screen_video_material;
        public Material Screen_video_material { get { return screen_video_material; } }

        [SerializeField]
        private string screen_material_property = "_EmissionMap";
        public string Screen_material_property { get { return screen_material_property; } }

        // Use this for initialization #########################################################################################################################################################################################################################
	    void Awake() {
		
            Instance = this;
	    }

        // Use this for initialization #########################################################################################################################################################################################################################
        void Start() {

        }
    }
}