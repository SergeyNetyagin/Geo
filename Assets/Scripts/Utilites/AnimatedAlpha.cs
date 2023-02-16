using UnityEngine;
using UnityEngine.UI;

namespace VostokVR.Geo {

    public class AnimatedAlpha : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private Component blinking_component;

        [SerializeField]
        private AnimationCurve blinking_curve;

        private float current_time = 0f;

        private Color starting_color = Color.white;
        private Color current_color = Color.white;

        private Text blinking_text;
        private Image blinking_image;

	    // Use this for initialization
	    private void Awake() {

            if( blinking_component is Text ) {

                blinking_text = (Text) blinking_component;
                starting_color = blinking_text.color;
            }

            else if( blinking_component is Image ) {

                blinking_image = (Image) blinking_component;
                starting_color = blinking_image.color;
            }
        }

	    // Use this for initialization
	    private void OnEnable() {
		
            if( blinking_image != null ) {

                blinking_image.color = starting_color;
            }

            else if( blinking_text != null ) {

                blinking_text.color = starting_color;
            }

            current_color = starting_color;
	    }
	
	    // Use this for initialization
	    private void OnDisable() {
		
            if( blinking_image != null ) {

                blinking_image.color = starting_color;
            }

            else if( blinking_text != null ) {

                blinking_text.color = starting_color;
            }
	    }
        
        // Update is called once per frame
	    private void Update() {
		
            current_time += Time.deltaTime * 0.5f;

            if( current_time > 1f ) current_time = 0f;

            current_color.a = blinking_curve.Evaluate( current_time );

            if( blinking_image != null ) {

                blinking_image.color = current_color;
            }

            else if( blinking_text != null ) {

                blinking_text.color = current_color;
            }
	    }
    }
}