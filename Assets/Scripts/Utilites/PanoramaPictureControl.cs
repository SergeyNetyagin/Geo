using UnityEngine;
using VertexStudio.VirtualRealty;

namespace VostokVR.Geo {

    public class PanoramaPictureControl : PanoramaControl {

        [Space( 10 ), SerializeField]
        private Transform player_transform;

        [SerializeField]
        private float distance_to_player = 5f;

        private Transform picture_transform;

        private Vector3 offset = Vector3.zero;

        // OnEnable is called before the first frame update
        private void OnEnable() {
            
            if( player_transform == null ) { 
                
                player_transform = ViveInteractionsManager.Instance.Eye_camera_transform;
            }

            if( picture_transform == null ) {
            
                picture_transform = GetComponent<Transform>();
            }

            picture_transform.position = new Vector3(
                
                player_transform.position.x,
                player_transform.position.y + 0.8f,
                player_transform.position.z
            );

            if( picture_transform.position.y < 2.55f ) { 
             
                picture_transform.position = new Vector3(
                
                    picture_transform.position.x,
                    2.55f,
                    picture_transform.position.z
                );                
            }

            picture_transform.Translate( picture_transform.forward * distance_to_player, Space.World );
        }

        // OnDisable is called before closing object
        private void OnDisable() {
            
        }

        // Start is called before the first frame update
        protected override void Start() {
      
            base.Start();
        }

        // Update is called once per frame
        protected override void Update() {
        
            base.Update();
        }
    }
}
