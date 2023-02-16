using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace VostokVR.Geo {

    public class PulsedImage : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private Image pulsing_imgae;

        [SerializeField]
        private AnimationCurve pulse_curve;

        [SerializeField, Range( 0f, 10f )]
        private float cycle_time = 2f;

        [Space( 10 ), SerializeField]
        private UnityEvent onEndCycle;

        private Color current_color = Color.clear;

        private float timer = 0f;
        private float time_rate = 1f;

	    // Use this for initialization #####################################################################################################################################################################################################################
	    private void OnEnable() {

            timer = 0f;

            current_color = pulsing_imgae.color;
            current_color.a = 0f;
	    }
	
	    // Use this for initialization #####################################################################################################################################################################################################################
	    private void Start() {
		
            time_rate = 1f / cycle_time;
	    }
        
        // Update is called once per frame #################################################################################################################################################################################################################
	    private void Update() {
		
            if( timer >= cycle_time ) {

                timer = 0f;
                current_color.a = 0f;
                pulsing_imgae.color = current_color;

                if( onEndCycle != null ) onEndCycle.Invoke();
            }

            else {

                current_color.a = pulse_curve.Evaluate( timer * time_rate );
                pulsing_imgae.color = current_color;

                timer += Time.deltaTime;
            }
	    }
    }
}