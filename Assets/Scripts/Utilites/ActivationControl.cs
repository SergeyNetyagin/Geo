using UnityEngine;
using UnityEngine.Events;

namespace VostokVR.Geo {

    public class ActivationControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private UnityEvent onEnable;

        [Space( 10 ), SerializeField]
        private UnityEvent onDisable;

        // ################################################################################################################################################################################################################################################
        private void OnEnable() {
        
            if( onEnable != null ) { 
            
                onEnable.Invoke();
            }
        }

        // ################################################################################################################################################################################################################################################
        private void OnDisable() {

            if( onDisable != null ) { 
            
                onDisable.Invoke();
            }
        }
    }
}