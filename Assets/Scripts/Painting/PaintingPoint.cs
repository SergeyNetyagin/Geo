using UnityEngine;

namespace VostokVR.Geo {

    public class PaintingPoint : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private Transform point_transform;
        public Transform Point_transform { get { return point_transform; } }

        [SerializeField]
        private Renderer point_renderer;
        public Renderer Point_renderer { get { return point_renderer; } }

        [SerializeField]
        private PaintingColor point_color = PaintingColor.Unknown;
        public PaintingColor Point_color { get { return point_color; } set { point_color = value; } }

        public PaintingGroup Parent_painting_group { get; set; }

        public int Point_ID { get; set; } = -1;
    }
}