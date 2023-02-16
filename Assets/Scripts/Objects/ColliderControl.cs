using UnityEngine;
using UnityEngine.Events;

namespace VostokVR.Geo {

    [RequireComponent( typeof( Collider ) )]
    public class ColliderControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private Collider controlled_collider;

        [Space( 10 ), SerializeField]
        private UnityEvent onTriggerEnter;

        [Space( 10 ), SerializeField]
        private UnityEvent onTriggerExit;

        private Collider own_collider;

        // Use this for initialization #####################################################################################################################################################################################################################
        private void Awake() {

            own_collider = GetComponent<Collider>();

            if( own_collider != null ) own_collider.isTrigger = true;
        }

        // Use this for initialization #####################################################################################################################################################################################################################
        private void Start() {

        }

        // On trigger enter ################################################################################################################################################################################################################################
        private void OnTriggerEnter( Collider other ) {
            
            if( other == controlled_collider ) {

                if( onTriggerEnter != null ) onTriggerEnter.Invoke();
            }
        }

        // On trigger exit #################################################################################################################################################################################################################################
        private void OnTriggerExit( Collider other ) {
            
            if( other == controlled_collider ) {

                if( onTriggerExit != null ) onTriggerExit.Invoke();
            }
        }
    }
}