using UnityEngine;

namespace VostokVR.Geo {

    public class PlayAreaTrigger : MonoBehaviour {

        // Use this for initialization #####################################################################################################################################################################################################################
	    private void Awake() {
        
        }

        // Use this for initialization #####################################################################################################################################################################################################################
	    private void Start() {
		
        }

        // Detects right position of player ################################################################################################################################################################################################################
        private void OnTriggerEnter( Collider other ) {
            
            if( other.GetComponent<LocalPlayerTrigger>() == null ) return;

            //ProjectManager.Instance.Play_area_is_adapted = true;
        }
    }
}