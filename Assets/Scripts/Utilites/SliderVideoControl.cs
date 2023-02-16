using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace VostokVR.Geo {

    public class SliderVideoControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private Slider slider;

        [SerializeField]
        private VideoPlayer video_player;

        #if( UNITY_EDITOR )
        [SerializeField, Range( 0.1f, 100f )]
        private float video_speed = 1f;
        private float previous_video_speed = 1f;
        #endif

        // Start is called before the first frame update ###########################################################################################################################################################################################################
        private void Start() {
        
            if( video_player != null ) { 
                
                video_player.prepareCompleted += CheckForAllowSpeedControl;
            }
        }

        // Update is called once per frame #########################################################################################################################################################################################################################
        private void Update() {
            
            #if( UNITY_EDITOR )
            if( previous_video_speed != video_speed ) { 
            
                if( (slider != null) && (video_player != null) ) { 
                    
                    if( video_player.canSetPlaybackSpeed ) {

                        video_player.playbackSpeed = video_speed;
                    }
                }

                previous_video_speed = video_speed;
            }
            #endif
        }

        /// <summary>
        /// Lets do alternative way to control slider using usual unity UI tools.
        /// </summary>
        /// ########################################################################################################################################################################################################################################################
        public void ChangeVideoTime( VideoPlayer video_player ) { 
        
            if( (slider != null) && (video_player != null) ) { 
            
                //video_player.time = slider.value * frame_rate;
                //video_player.playbackSpeed = 10f;
            }
        }

        /// <summary>
        /// Subscribes when video file is prepared.
        /// </summary>
        /// ########################################################################################################################################################################################################################################################
        public void CheckForAllowSpeedControl( VideoPlayer video_player ) { 
        
            if( video_player != null ) { 
            
                Debug.Log( "video_player.canSetPlaybackSpeed: " + video_player.canSetPlaybackSpeed );
            }
        }
    }
}