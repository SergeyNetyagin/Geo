using UnityEngine;

namespace VostokVR.Geo {

    public class PaintingEraser : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private Transform eraser_transform;
        public Transform Eraser_transform { get { return eraser_transform; } set { eraser_transform = value; } }

        [SerializeField]
        private Rigidbody eraser_rigidbody;

        [SerializeField]
        private Collider eraser_collider;

        public void EnableCollider() { if( eraser_collider != null ) eraser_collider.enabled = true; }
        public void DisableCollider() { if( eraser_collider != null ) eraser_collider.enabled = false; }

        public void SetRigidbody( Rigidbody rigidbody ) { eraser_rigidbody = rigidbody; }
        public void SetCollider( Collider collider ) { eraser_collider = collider; }

        // Use this for initialization #####################################################################################################################################################################################################################
	    private void Start() {
		
	    }

        // On trigger enter ################################################################################################################################################################################################################################
        private void OnTriggerEnter( Collider other ) {

            if( other.CompareTag( "Pixel" ) ) { 

                PaintingControl.Instance.AddContactedPixelToList( other );
            }
        }

        // On trigger exit #################################################################################################################################################################################################################################
        private void OnTriggerExit( Collider other ) {

        }
    }
}