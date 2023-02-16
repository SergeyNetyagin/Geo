using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VertexStudio.Generic;
using VertexStudio.Networking;

namespace VostokVR.Geo {

    public class NetworkTransactionsControlScenes : NetworkTransactionsControl {

        public static NetworkTransactionsControlScenes Instance { get; private set; }

	    // Use this for initialization #########################################################################################################################################################################################################################
	    protected override void Awake() {
		
            Instance = this;

            base.Awake();
	    }
	
	    // Use this for initialization #########################################################################################################################################################################################################################
	    protected override void Start() {
		
            base.Start();
	    }

        // Loads the specified scene by master client command ##################################################################################################################################################################################################
        public void LoadNetworkGroupScene( int scene_index ) {
            
            if( SceneManager.GetActiveScene().name != ScenesManager.Instance.GetSceneNameByIndex( scene_index ) ) {
            
                photonView.RPC( "RPC_LoadNetworkGroupScene", PhotonTargets.Others, scene_index );
            }
        }

        // Loads scene for remote players #####################################################################################################################################################################################################################
        [PunRPC]
        private void RPC_LoadNetworkGroupScene( int scene_index ) {

            if( PhotonNetwork.room != null ) {
            
                PhotonNetwork.LeaveRoom( false );
            }

            StartCoroutine( WaitForLeavingRoomAndLoadScene( scene_index ) );
        }

        // Waits for leaving room and load scene ###############################################################################################################################################################################################################
        private IEnumerator WaitForLeavingRoomAndLoadScene( int scene_index ) { 
            
            while( PhotonNetwork.room != null ) {

                yield return null;
            }

            ScenesManager.Instance.LoadScene( scene_index );

            yield break;
        }
    }
}