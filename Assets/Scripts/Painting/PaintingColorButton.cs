using UnityEngine;

namespace VostokVR.Geo {

    public class PaintingColorButton : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private PaintingColor selected_color = PaintingColor.Unknown;
        public PaintingColor Selected_color { get { return selected_color; } }

        private void Start() {
            
        }
    }
}