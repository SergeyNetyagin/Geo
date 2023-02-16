using UnityEngine;
using VertexStudio.Networking;

namespace VostokVR.Geo {

    public class NetworkTransactionsControlPainting : NetworkTransactionsControl {

        public static NetworkTransactionsControlPainting Instance { get; private set; }

	    // Use this for initialization #########################################################################################################################################################################################################################
	    protected override void Awake() {
		
            Instance = this;

            base.Awake();
	    }
	
	    // Use this for initialization #########################################################################################################################################################################################################################
	    protected override void Start() {
		
            base.Start();
	    }

        // Draws pixel for other players #######################################################################################################################################################################################################################
        public void InstantiatePoint( int view_ID, Vector3 position, int color_index ) {
            
            photonView.RPC( "RPC_InstantiatePoint", PhotonTargets.OthersBuffered, view_ID, position.x, position.y, position.z, color_index );
        }

        // Instantiates pixle using current parameters #########################################################################################################################################################################################################
        [PunRPC]
        private void RPC_InstantiatePoint( int view_ID, float position_x, float position_y, float position_z, int color_index ) {
            
            if( PaintingControl.Instance != null ) { 
            
                PaintingGroup painting_group = PaintingControl.Instance.GetPaintingGroup( view_ID );

                if( painting_group != null ) { 
               
                    PaintingControl.Instance.Current_paint = PaintingControl.Instance.GetPaint( (PaintingColor) color_index );

                    Vector3 point_position = new Vector3( position_x, position_y, position_z );

                    PaintingPoint painting_point = PaintingControl.Instance.InstantiatePoint( point_position, painting_group );
                }
            }
        }

        // Clear all pixels for other players ##################################################################################################################################################################################################################
        public void ClearAllPixels( int view_ID ) {
            
            photonView.RPC( "RPC_ClearAllPixels", PhotonTargets.Others, view_ID );
        }

        // Clear all pixels for other players ###############################################################################################################################################################################################################
        [PunRPC]
        private void RPC_ClearAllPixels( int view_ID ) {

            if( PaintingControl.Instance != null ) { 
            
                PaintingGroup painting_group = PaintingControl.Instance.GetPaintingGroup( view_ID );

                if( painting_group != null ) { 
                
                    painting_group.ClearAllPixels();
                }
            }
        }

        // Creates new painting group for network player ##################################################################################################################################################################################################
        public void CreatePaintingGroup() { 
            
            if( VertexStudio.Networking.NetworkPlayer.Instance == null ) { 
            
                return;
            } 

            VertexStudio.Networking.NetworkPlayer network_player = VertexStudio.Networking.NetworkPlayer.Last_instantiated_network_player;

            if( network_player.Is_local_player ) { 
            
                PaintingControl.Instance.Painting_group_local.Network_player_view_ID = network_player.View_ID;

                photonView.RPC( "RPC_CreatePaintingGroup", PhotonTargets.OthersBuffered, network_player.View_ID );
            }
        }

        // Clear all pixels for other players ###############################################################################################################################################################################################################
        [PunRPC]
        private void RPC_CreatePaintingGroup( int view_ID ) {

            PaintingGroup painting_group = new GameObject( "Painting Group Remote; view_ID = " + view_ID.ToString() ).AddComponent<PaintingGroup>();

            painting_group.Network_player_view_ID = view_ID;

            painting_group.transform.SetParent( PaintingControl.Instance.transform, true );
            painting_group.transform.localPosition = Vector3.zero;
            painting_group.transform.localRotation = Quaternion.identity;
            painting_group.transform.localScale = Vector3.one;

            PaintingControl.Instance.RegisterPaintingGroup( painting_group );
        }
    }
}