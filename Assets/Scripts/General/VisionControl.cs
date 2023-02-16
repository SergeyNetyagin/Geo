using UnityEngine;
using VertexStudio.VirtualRealty;

namespace VostokVR.Geo {

    public class VisionControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private GameObject[] controlled_objects;

        private Camera eye_camera;

	    // Use this for initialization #####################################################################################################################################################################################################################
	    private void Awake() {

            eye_camera = ViveInteractionsManager.Instance.Eye_camera_transform.GetComponent<Camera>();
        }

        // On will render ##################################################################################################################################################################################################################################
        private void OnWillRenderObject() {

            if( Camera.current == null ) return;
        }

        // On became visible ###############################################################################################################################################################################################################################
        private void OnBecameVisible() {

            for( int i = 0; i < controlled_objects.Length; i++ ) controlled_objects[i].SetActive( true );
        }

        // On became invisible #############################################################################################################################################################################################################################
        private void OnBecameInvisible() {
        
            for( int i = 0; i < controlled_objects.Length; i++ ) controlled_objects[i].SetActive( false );
        }

        // On render object ################################################################################################################################################################################################################################
        private void OnRenderObject() {
            
        }
    }
}