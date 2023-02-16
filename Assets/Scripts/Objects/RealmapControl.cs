using UnityEngine;

namespace VostokVR.Geo {

    public class RealmapControl : MonoBehaviour {

        [Space( 10  ), SerializeField]
        private Transform real_space_transform;
        public Transform Real_space_transform { get { return real_space_transform; } }

        [SerializeField]
        private Transform real_first_milestone_transform;
        public Transform Real_first_milestone_transform { get { return real_first_milestone_transform; } }

        [SerializeField]
        private Transform real_second_milestone_transform;
        public Transform Real_second_milestone_transform { get { return real_second_milestone_transform; } }

        // Start is called before the first frame update ##################################################################################################################################################################################################
        private void Start() {
        
        }
    }
}