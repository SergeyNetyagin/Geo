using UnityEngine;
using UnityEngine.UI;
using VertexStudio.Networking;

namespace VostokVR.Geo {

    public class NetworkAngleMeter : NetworkObject {

        [Space( 10 ), SerializeField]
        private LineRenderer section_line_renderer;
        public LineRenderer Section_line_renderer { get { return section_line_renderer; } }

        [SerializeField]
        private LineRenderer angle_ray_line_renderer;
        public LineRenderer Angle_ray_line_renderer { get { return angle_ray_line_renderer; } }

        [SerializeField]
        private Transform corner_point_transform;
        public Transform Corner_point_transform { get { return corner_point_transform; } }

        [SerializeField]
        private Transform section_end_point_transform;
        public Transform Section_end_point_transform { get { return section_end_point_transform; } }

        [SerializeField]
        private Transform ray_end_point_transform;
        public Transform Ray_end_point_transform { get { return ray_end_point_transform; } }

        [SerializeField]
        private Transform angle_text_transform;
        public Transform Angle_text_transform { get { return angle_text_transform; } }

        [SerializeField]
        private Transform angle_value_transform;
        public Transform Angle_value_transform { get { return angle_value_transform; } }

        [SerializeField]
        private Text angle_text;
        public Text Angle_text { get { return angle_text; } }

        [SerializeField]
        private Image angle_value_image;
        public Image Angle_value_image { get { return angle_value_image; } }
    }
}