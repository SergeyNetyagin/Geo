using UnityEngine;
using UnityEngine.SceneManagement;
using VertexStudio.VirtualRealty;
using VertexStudio.Networking;

namespace VostokVR.Geo {

    public class GuideControl : MonoBehaviour {

        private CanvasControllerPanelGuide current_guide;

        // Awake is called before the first frame update #################################################################################################################################################################################################
        private void Awake() {

        }

        // OnEnable is called before the first frame update ###############################################################################################################################################################################################
        private void OnEnable() {

            Transform panel_transform = GetComponent<Transform>();

            panel_transform.position = ViveInteractionsManager.Instance.Eye_camera_transform.position;
            panel_transform.eulerAngles = new Vector3( 0f, ViveInteractionsManager.Instance.Eye_camera_transform.eulerAngles.y, 0f );
            panel_transform.Translate( 0f, 0f, CanvasFiledGuide.Instance.Distance_to_player, Space.Self );
        }

        // Moves page left ################################################################################################################################################################################################################################
        public void PageLeft() { 

            current_guide = GetComponentInChildren<CanvasControllerPanelGuide>();

            if( current_guide != null ) {
            
                current_guide.PageLeft();
            }

            if( (NetworkProjectManager.Instance != null) && PhotonNetwork.isMasterClient ) { 
            
                if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                    NetworkTransactionsControlTools.Instance.SetGuide( 

                        VertexStudio.Networking.NetworkPlayer.Instance.View_ID, 
                        CanvasFiledGuide.Instance.Panel_guide.activeSelf ? true : false, 
                        CanvasFiledGuide.Instance.Panel_guide.GetComponent<SceneAttributeControl>().GetSceneAttribute().GetComponent<CanvasControllerPanelGuide>().Current_page_index,
                        SceneManager.GetActiveScene().name 
                    );
                }
            }
        }

        // Moves page right ###############################################################################################################################################################################################################################
        public void PageRight() { 

            current_guide = GetComponentInChildren<CanvasControllerPanelGuide>();

            if( current_guide != null ) {
            
                current_guide.PageRight();
            }

            if( (NetworkProjectManager.Instance != null) && PhotonNetwork.isMasterClient ) { 
            
                if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                    NetworkTransactionsControlTools.Instance.SetGuide( 

                        VertexStudio.Networking.NetworkPlayer.Instance.View_ID, 
                        CanvasFiledGuide.Instance.Panel_guide.activeSelf ? true : false, 
                        CanvasFiledGuide.Instance.Panel_guide.GetComponent<SceneAttributeControl>().GetSceneAttribute().GetComponent<CanvasControllerPanelGuide>().Current_page_index,
                        SceneManager.GetActiveScene().name 
                    );
                }
            }
        }
    }
}