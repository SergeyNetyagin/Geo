using UnityEngine;

namespace VostokVR.Geo {

    public class SkyboxManager : MonoBehaviour {

        public static SkyboxManager Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private GameObject render_texture_skybox_videos;
        public GameObject Render_texture_skybox_videos { get { return render_texture_skybox_videos; } }

        [Space( 10 ), SerializeField]
        private GameObject exterior_skybox_pictures;
        public GameObject Exterior_skybox_pictures { get { return exterior_skybox_pictures; } }

        [SerializeField]
        private GameObject exterior_skybox_videos;
        public GameObject Exterior_skybox_videos { get { return exterior_skybox_videos; } }

        [SerializeField]
        private GameObject interior_skybox_pictures;
        public GameObject Interior_skybox_pictures { get { return interior_skybox_pictures; } }

        [SerializeField]
        private GameObject interior_skybox_videos;
        public GameObject Interior_skybox_videos { get { return interior_skybox_videos; } }
    
        // Use this for initialization #####################################################################################################################################################################################################################
	    private void Awake() {

            Instance = this;
        }

        // Use this for initialization #####################################################################################################################################################################################################################
	    private void Start() {
		
	    }
    }
}