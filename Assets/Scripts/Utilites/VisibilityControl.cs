using UnityEngine;

namespace VostokVR.Geo {

    public class VisibilityControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private GameObject[] controlled_objects;

        [Space( 10 ), SerializeField]
        private Behaviour[] controlled_components;

	    // Use this for initialization #########################################################################################################################################################################################################################
	    private void Awake() {

        }

	    // Use this for initialization #########################################################################################################################################################################################################################
	    private void Start() {

        }
    
        // On became visible ###################################################################################################################################################################################################################################
        private void OnBecameVisible() {

            for( int i = 0; i < controlled_objects.Length; i++ ) controlled_objects[i].SetActive( true );

            for( int i = 0; i < controlled_components.Length; i++ ) controlled_components[i].enabled = true;
        }

        // On became invisible #################################################################################################################################################################################################################################
        private void OnBecameInvisible() {
        
            for( int i = 0; i < controlled_objects.Length; i++ ) controlled_objects[i].SetActive( false );

            for( int i = 0; i < controlled_components.Length; i++ ) controlled_components[i].enabled = false;
        }
    }
}