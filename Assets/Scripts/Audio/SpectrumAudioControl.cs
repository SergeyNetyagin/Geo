using UnityEngine;

namespace VostokVR.Geo {

    public class SpectrumAudioControl : MonoBehaviour {

        public static SpectrumAudioControl Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private AudioSource audio_source;
        public AudioSource Audio_source { get { return audio_source; } }

        [SerializeField]
        private Transform audio_source_transform;
        public Transform Audio_source_transform { get { return audio_source_transform; } }

        [Space( 10 ), SerializeField, Range( 64, 512 )]
        private int audio_samples = 256;
        public int Audio_samples { get { return audio_samples; } }

        [SerializeField, Range( 0, 1 )]
        private int audio_channel = 0;
        public int Audio_channel { get { return audio_channel; } }

        [SerializeField, Range( 0f, 5f )]
        private float audio_gain_time = 3f;
        public float Audio_gain_time { get { return audio_gain_time; } }

        [SerializeField, Range( 0f, 5f )]
        private float audio_damping_time = 3f;
        public float Audio_damping_time { get { return audio_damping_time; } }

        [SerializeField]
        private FFTWindow audio_fft_window = FFTWindow.Rectangular;
        public FFTWindow Audio_fft_window { get { return audio_fft_window; } }

        [SerializeField, Range( 1f, 10f )]
        private float height_multiplier_right = 5.0f;
        public float Height_multiplier_right { get { return height_multiplier_right; } }

        [SerializeField, Range( 1f, 10f )]
        private float height_multiplier_left = 5.1f;
        public float Height_multiplier_left { get { return height_multiplier_left; } }

        // Use this for initialization ####################################################################################################################################################################################################################
        private void Awake() {
        
            if( Instance == null ) { 
            
                Instance = this;
            }
        }

        // Use this for initialization ####################################################################################################################################################################################################################
        private void Start() {
        
        }
    }
}