using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VertexStudio.Networking;

namespace VostokVR.Geo {

    public class NetworkTransactionsControlMeasurers : NetworkTransactionsControl {

        public static NetworkTransactionsControlMeasurers Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private NetworkMeasurer network_measurer_prefab;
        public NetworkMeasurer Network_measurer_prefab { get { return network_measurer_prefab; } }

        private List<NetworkMeasurer> measurers = new List<NetworkMeasurer>();

	    // Use this for initialization #########################################################################################################################################################################################################################
	    protected override void Awake() {
		
            Instance = this;

            base.Awake();
	    }
	
	    // Use this for initialization #########################################################################################################################################################################################################################
	    protected override void Start() {
		
            base.Start();
	    }

        // Creates measurer ####################################################################################################################################################################################################################################
        public void CreateMeasurer( int view_ID, string scene_name ) {
            
            photonView.RPC( "RPC_CreateMeasurer", PhotonTargets.Others, view_ID, scene_name );
        }

        // Creates measurer ####################################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_CreateMeasurer( int view_ID, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                NetworkMeasurer network_measurer = Instantiate( network_measurer_prefab, MeasuringControl.Instance.transform );

                network_measurer.View_ID = view_ID;

                network_measurer.name = network_measurer_prefab.name + "; client ID: " + view_ID;
                network_measurer.transform.localPosition = Vector3.zero;
                network_measurer.transform.localRotation = Quaternion.identity;
                network_measurer.transform.localScale = Vector3.one;

                measurers.Add( network_measurer );
            }
        }

        // Destroys measurer ###################################################################################################################################################################################################################################
        public void DestroyMeasurer( int view_ID, string scene_name ) {
            
            photonView.RPC( "RPC_DestroyMeasurer", PhotonTargets.Others, view_ID, scene_name );
        }

        // Destroys measurer ###################################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_DestroyMeasurer( int view_ID, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                for( int i = 0; i < measurers.Count; i++ ) { 
            
                    if( measurers[i].View_ID == view_ID ) { 
                
                        Destroy( measurers[i].gameObject );

                        measurers.RemoveAt( i );

                        break;
                    }
                }
            }
        }

        // Shows measurer ######################################################################################################################################################################################################################################
        public void ShowMeasurer( int view_ID, string scene_name, Vector3 start_point, Vector3 end_point, Vector3 text_position, Vector3 text_rotation, string distance_text ) {

            #if( UNITY_EDITOR )
            //Debug.Log( "start_point: " + start_point + "; end_point: " + end_point + "; text_position: " + text_position + "; distance_text: " + distance_text );
            #endif

            photonView.RPC( "RPC_ShowMeasurerGeometry", PhotonTargets.Others, 
                
                view_ID, 
                scene_name, 
                start_point.x, 
                start_point.y, 
                start_point.z, 
                end_point.x, 
                end_point.y, 
                end_point.z 
            );

            photonView.RPC( "RPC_ShowMeasurerText", PhotonTargets.Others, 
                
                view_ID, 
                scene_name, 
                text_position.x, 
                text_position.y, 
                text_position.z, 
                text_rotation.x,
                text_rotation.y,
                text_rotation.z,
                distance_text 
            );
        }

        // Shows measurer geometry ##############################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_ShowMeasurerGeometry( int view_ID, string scene_name, float start_point_x, float start_point_y, float start_point_z, float end_point_x, float end_point_y, float end_point_z ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                for( int i = 0; i < measurers.Count; i++ ) { 
            
                    if( measurers[i].View_ID == view_ID ) { 
                
                        NetworkMeasurer measurer = measurers[i];

                        #if( UNITY_EDITOR )
                        //Debug.Log( "start_point: " + new Vector3( start_point_x, start_point_y, start_point_z ) + "; end_point: " + new Vector3( end_point_x, end_point_y, end_point_z ) );
                        #endif

                        measurer.Start_sphere_transform.position = new Vector3( start_point_x, start_point_y, start_point_z );
                        measurer.End_sphere_transform.position = new Vector3( end_point_x, end_point_y, end_point_z );

                        measurer.Measurer_line_renderer.SetPosition( 0, measurer.Start_sphere_transform.position );
                        measurer.Measurer_line_renderer.SetPosition( 1, measurer.End_sphere_transform.position );

                        break;
                    }
                }
            }
        }

        // Shows measurer text ##################################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_ShowMeasurerText( int view_ID, string scene_name, float text_position_x, float text_position_y, float text_position_z, float text_rotation_x, float text_rotation_y, float text_rotation_z, string distance_text ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                for( int i = 0; i < measurers.Count; i++ ) { 
            
                    if( measurers[i].View_ID == view_ID ) { 
                
                        NetworkMeasurer measurer = measurers[i];

                        #if( UNITY_EDITOR )
                        //Debug.Log( "text_position: " + new Vector3( text_position_x, text_position_y, text_position_z ) + "; distance_text: " + distance_text );
                        #endif

                        measurer.Measurer_text_transform.position = new Vector3( text_position_x, text_position_y, text_position_z );
                        measurer.Measurer_text_transform.eulerAngles = new Vector3( text_rotation_x, text_rotation_y, text_rotation_z );

                        measurer.Measurer_text.text = distance_text;

                        break;
                    }
                }
            }
        }
    }
}