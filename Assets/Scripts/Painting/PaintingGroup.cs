using UnityEngine;

namespace VostokVR.Geo {

    public class PaintingGroup : MonoBehaviour {

        public Transform Group_transform { get; private set; }

        public int Network_player_view_ID { get; set; }

        public int GetNextPointID() { return current_point_ID ++ ; }

        private int current_point_ID = 0;

        // Awake is called before the first frame update ##################################################################################################################################################################################################
        private void Awake() {
        
            Group_transform = transform;
        }

        // ################################################################################################################################################################################################################################################
        private void OnEnable() {
         
        }

        // ################################################################################################################################################################################################################################################
        private void OnDisable() {
            
        }

        // Start is called before the first frame update ##################################################################################################################################################################################################
        private void Start() {

        }

        // Shows all pixels for the group #################################################################################################################################################################################################################
        public void ShowAllPoints() {

            PaintingPoint[] painting_points = GetComponentsInChildren<PaintingPoint>( true );

            if( painting_points != null ) {

                for( int i = 0; i < painting_points.Length; i++ ) { 
                
                    painting_points[i].gameObject.SetActive( true );
                }
            }
        }

        // Hides all pixels for the group #################################################################################################################################################################################################################
        public void HideAllPoints() {

            PaintingPoint[] painting_points = GetComponentsInChildren<PaintingPoint>( true );

            if( painting_points != null ) {

                for( int i = 0; i < painting_points.Length; i++ ) { 
                
                    painting_points[i].gameObject.SetActive( false );
                }
            }
        }

        // Clears all pixels for the group ################################################################################################################################################################################################################
        public void ClearAllPixels() {
           
            for( int i = Group_transform.childCount - 1; i >= 0; i-- ) { 

                Transform point_transform = Group_transform.GetChild( i );

                if( point_transform.GetComponent<PaintingEraser>() == null ) {
                
                    Destroy( point_transform.gameObject );
                }
            }

            current_point_ID = 0;

            Resources.UnloadUnusedAssets();
        }
    }
}