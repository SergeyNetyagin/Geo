using UnityEngine;

namespace VostokVR.Geo {

    public class MinimapControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private RectTransform first_milestone_transform;
        public RectTransform First_milestone_transform { get { return first_milestone_transform; } }

        [SerializeField]
        private RectTransform second_milestone_transform;
        public RectTransform Second_milestone_transform { get { return second_milestone_transform; } }

        [SerializeField]
        private RectTransform player_position_transform;
        public RectTransform Player_position_transform { get { return player_position_transform; } }

        [SerializeField]
        private RectTransform player_rotation_transform;
        public RectTransform Player_rotation_transform { get { return player_rotation_transform; } }

        // Start is called before the first frame update ##################################################################################################################################################################################################
        private void Start() {
        
        }
    }
}