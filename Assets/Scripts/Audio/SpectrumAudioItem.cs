using UnityEngine;
using UnityEngine.UI;

namespace VostokVR.Geo {

    public class SpectrumAudioItem : MonoBehaviour {

	    [Space( 10 ), SerializeField]
        private Image image_sample_upper;
        public Image Image_sample_upper { get { return image_sample_upper; } }

	    [SerializeField]
        private Image image_sample_lower;
        public Image Image_sample_lower { get { return image_sample_lower; } }

        private void Start() {
            
        }
    }
}