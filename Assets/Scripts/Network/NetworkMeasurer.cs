using UnityEngine;
using UnityEngine.UI;
using VertexStudio.Networking;

namespace VostokVR.Geo {

    public class NetworkMeasurer : NetworkObject {

        [Space( 10 ), SerializeField]
        private LineRenderer measurer_line_renderer;
        public LineRenderer Measurer_line_renderer { get { return measurer_line_renderer; } }

        [SerializeField]
        private Transform start_sphere_transform;
        public Transform Start_sphere_transform { get { return start_sphere_transform; } }

        [SerializeField]
        private Transform end_sphere_transform;
        public Transform End_sphere_transform { get { return end_sphere_transform; } }

        [SerializeField]
        private Transform measurer_text_transform;
        public Transform Measurer_text_transform { get { return measurer_text_transform; } }

        [SerializeField]
        private Text measurer_text;
        public Text Measurer_text { get { return measurer_text; } }
    }
}