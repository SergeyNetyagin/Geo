using UnityEngine;

namespace VostokVR.Geo {

    public class AnimationSpeedControl : MonoBehaviour {

        [Space( 10 ), SerializeField, Range( 0f, 10f )]
        private float animation_speed = 1f;

        private Animation current_animation;

        private string clip_name = string.Empty;

	    // Use this for initialization
	    void Start () {
		
            current_animation = GetComponent<Animation>();

            clip_name = current_animation.clip.name;

            if( current_animation != null ) {

                current_animation[ clip_name ].speed = animation_speed;
            }
	    }
    }
}