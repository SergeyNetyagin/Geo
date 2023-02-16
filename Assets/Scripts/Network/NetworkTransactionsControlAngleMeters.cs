using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VertexStudio.Networking;

namespace VostokVR.Geo {

    public class NetworkTransactionsControlAngleMeters : NetworkTransactionsControl {

        public static NetworkTransactionsControlAngleMeters Instance { get; private set; }

        [SerializeField]
        private NetworkAngleMeter network_angle_meter_prefab;
        public NetworkAngleMeter Network_angle_meter_prefab { get { return network_angle_meter_prefab; } }

        private List<NetworkAngleMeter> angle_meters = new List<NetworkAngleMeter>();

	    // Use this for initialization #########################################################################################################################################################################################################################
	    protected override void Awake() {
		
            Instance = this;

            base.Awake();
	    }
	
	    // Use this for initialization #########################################################################################################################################################################################################################
	    protected override void Start() {
		
            base.Start();
	    }

        // Creates angle meter #################################################################################################################################################################################################################################
        public void CreateAngleMeter( int view_ID, string scene_name ) {
            
            photonView.RPC( "RPC_CreateAngleMeter", PhotonTargets.Others, view_ID, scene_name );
        }

        // Creates angle meter #################################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_CreateAngleMeter( int view_ID, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                NetworkAngleMeter angle_meter = Instantiate( network_angle_meter_prefab, MeasuringControl.Instance.transform );

                angle_meter.View_ID = view_ID;

                angle_meter.name = network_angle_meter_prefab.name + "; client ID: " + view_ID;
                angle_meter.transform.localPosition = Vector3.zero;
                angle_meter.transform.localRotation = Quaternion.identity;
                angle_meter.transform.localScale = Vector3.one;

                angle_meter.Section_line_renderer.gameObject.SetActive( true );
                angle_meter.Angle_ray_line_renderer.gameObject.SetActive( false );

                angle_meters.Add( angle_meter );
            }
        }

        // Shows whole angle meter #############################################################################################################################################################################################################################
        public void ShowEntireAngleMeter( int view_ID, string scene_name ) {
            
            photonView.RPC( "RPC_ShowEntireAngleMeter", PhotonTargets.Others, view_ID, scene_name );
        }

        // Shows whole angle meter #############################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_ShowEntireAngleMeter( int view_ID, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                for( int i = 0; i < angle_meters.Count; i++ ) { 
            
                    if( angle_meters[i].View_ID == view_ID ) { 
                
                        NetworkAngleMeter angle_meter = angle_meters[i];

                        angle_meter.Section_line_renderer.gameObject.SetActive( true );
                        angle_meter.Angle_ray_line_renderer.gameObject.SetActive( true );

                        break;
                    }
                }
            }
        }

        // Destroys angle meter ################################################################################################################################################################################################################################
        public void DestroyAngleMeter( int view_ID, string scene_name ) {
            
            photonView.RPC( "RPC_AngleMeter", PhotonTargets.Others, view_ID, scene_name );
        }

        // Destroys angle meter ################################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_AngleMeter( int view_ID, string scene_name ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                for( int i = 0; i < angle_meters.Count; i++ ) { 
            
                    if( angle_meters[i].View_ID == view_ID ) { 
                
                        Destroy( angle_meters[i].gameObject );

                        angle_meters.RemoveAt( i );

                        break;
                    }
                }
            }
        }

        // Shows angle meter ###################################################################################################################################################################################################################################
        public void ShowAngleMeter( int view_ID, string scene_name, bool show_angle_ray, bool show_angle_value, Vector3 corner_point, Vector3 end_section_point, Vector3 end_ray_point, Vector3 text_position, Vector3 text_rotation, string angle_text ) {

            #if( UNITY_EDITOR )
            //Debug.Log( "start_point: " + start_point + "; end_point: " + end_point + "; text_position: " + text_position + "; distance_text: " + distance_text );
            #endif

            if( !show_angle_ray ) {

                photonView.RPC( "RPC_ShowAngleMeterSection", PhotonTargets.Others, 
                
                    view_ID, 
                    scene_name, 
                    corner_point.x, 
                    corner_point.y, 
                    corner_point.z, 
                    end_section_point.x, 
                    end_section_point.y, 
                    end_section_point.z 
                );
            }

            else {

                photonView.RPC( "RPC_ShowShowAngleMeterRay", PhotonTargets.Others, 
                
                    view_ID, 
                    scene_name, 
                    corner_point.x, 
                    corner_point.y, 
                    corner_point.z, 
                    end_ray_point.x, 
                    end_ray_point.y, 
                    end_ray_point.z 
                );
            }

            if( show_angle_value ) {

                photonView.RPC( "RPC_ShowAngleMeterText", PhotonTargets.Others, 
                
                    view_ID, 
                    scene_name, 
                    text_position.x, 
                    text_position.y, 
                    text_position.z, 
                    text_rotation.x,
                    text_rotation.y,
                    text_rotation.z,
                    angle_text 
                );
            }
        }

        // Shows angle meter ray section ########################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_ShowAngleMeterSection( int view_ID, string scene_name, float start_point_x, float start_point_y, float start_point_z, float end_point_x, float end_point_y, float end_point_z ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                for( int i = 0; i < angle_meters.Count; i++ ) { 
            
                    if( angle_meters[i].View_ID == view_ID ) { 
                
                        NetworkAngleMeter angle_meter = angle_meters[i];

                        #if( UNITY_EDITOR )
                        //Debug.Log( "start_point: " + new Vector3( start_point_x, start_point_y, start_point_z ) + "; end_point: " + new Vector3( end_point_x, end_point_y, end_point_z ) );
                        #endif

                        angle_meter.Corner_point_transform.position = new Vector3( start_point_x, start_point_y, start_point_z );
                        angle_meter.Section_end_point_transform.position = new Vector3( end_point_x, end_point_y, end_point_z );

                        if( angle_meter.Ray_end_point_transform.gameObject.activeSelf ) { 

                            angle_meter.Ray_end_point_transform.gameObject.SetActive( false );
                        }

                        angle_meter.Section_line_renderer.SetPosition( 0, new Vector3( start_point_x, start_point_y, start_point_z ) );
                        angle_meter.Section_line_renderer.SetPosition( 1, new Vector3( end_point_x, end_point_y, end_point_z ) );

                        break;
                    }
                }
            }
        }

        // Shows angle meter ray angle ##########################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_ShowShowAngleMeterRay( int view_ID, string scene_name, float start_point_x, float start_point_y, float start_point_z, float end_point_x, float end_point_y, float end_point_z ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                for( int i = 0; i < angle_meters.Count; i++ ) { 
            
                    if( angle_meters[i].View_ID == view_ID ) { 
                
                        NetworkAngleMeter angle_meter = angle_meters[i];

                        #if( UNITY_EDITOR )
                        //Debug.Log( "start_point: " + new Vector3( start_point_x, start_point_y, start_point_z ) + "; end_point: " + new Vector3( end_point_x, end_point_y, end_point_z ) );
                        #endif

                        angle_meter.Ray_end_point_transform.position = new Vector3( end_point_x, end_point_y, end_point_z );

                        if( !angle_meter.Ray_end_point_transform.gameObject.activeSelf ) { 

                            angle_meter.Ray_end_point_transform.gameObject.SetActive( true );
                        }

                        angle_meter.Angle_ray_line_renderer.SetPosition( 0, new Vector3( start_point_x, start_point_y, start_point_z ) );
                        angle_meter.Angle_ray_line_renderer.SetPosition( 1, new Vector3( end_point_x, end_point_y, end_point_z ) );

                        break;
                    }
                }
            }
        }

        // Shows angle meter text ###############################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_ShowAngleMeterText( int view_ID, string scene_name, float text_position_x, float text_position_y, float text_position_z, float text_rotation_x, float text_rotation_y, float text_rotation_z, string angle_text ) {

            if( SceneManager.GetActiveScene().name == scene_name ) {

                for( int i = 0; i < angle_meters.Count; i++ ) { 
            
                    if( angle_meters[i].View_ID == view_ID ) { 
                
                        NetworkAngleMeter angle_meter = angle_meters[i];

                        #if( UNITY_EDITOR )
                        //Debug.Log( "text_position: " + new Vector3( text_position_x, text_position_y, text_position_z ) + "; distance_text: " + distance_text );
                        #endif

                        if( !angle_meter.Angle_text.gameObject.activeSelf ) { 

                            angle_meter.Angle_text.gameObject.SetActive( true );
                        }

                        angle_meter.Angle_text_transform.position = new Vector3( text_position_x, text_position_y, text_position_z );
                        angle_meter.Angle_text_transform.eulerAngles = new Vector3( text_rotation_x, text_rotation_y, text_rotation_z );

                        angle_meter.Angle_text.text = angle_text;

                        break;
                    }
                }
            }
        }
    }
}