using UnityEngine;

namespace VostokVR.Geo {

    public class Panorama360Control : PanoramaControl {

        [Space( 10 ), SerializeField]
        private Transform target_transform;

        [SerializeField]
        private Transform start_point_transform;

        private Transform sphere_transform;

        private Vector3 offset = Vector3.zero;

        // Start is called before the first frame update
        protected override void Start() {

            base.Start();
      
            sphere_transform = GetComponent<Transform>();

            offset = start_point_transform.position - sphere_transform.position;
        }

        // Update is called once per frame
        protected override void Update() {
        
            base.Update();

            sphere_transform.position = target_transform.position + offset;
        }
    }
}
