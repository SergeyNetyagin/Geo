using UnityEngine;
using UnityEngine.UI;
using VertexStudio.Networking;

namespace VostokVR.Geo {

    public class Hotspot : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private int hotspot_ID = -1;
        public int Hotspot_ID { get { return hotspot_ID; } }

        [Space( 10 ), SerializeField]
        private GameObject content;

        [Space( 10 ), SerializeField]
        private Text text_tooltip;
        public Text Text_tooltip { get { return text_tooltip; } }

        [SerializeField]
        private RectTransform panel_presentation;
        public RectTransform Panel_presentation { get { return panel_presentation; } }

        [SerializeField]
        private SpectrumAudioPlayer spectrum_audio_player;
        public SpectrumAudioPlayer Spectrum_audio_player { get { return spectrum_audio_player; } }

        public void SetHotspotID( int ID ) { hotspot_ID = ID; }

	    // Use this for initialization ###################################################################################################################################################################################################################
	    private void OnEnable() {

            text_tooltip.gameObject.SetActive( false );

            if( panel_presentation != null ) {
            
                panel_presentation.gameObject.SetActive( false );
            }

            if( spectrum_audio_player != null ) {
                    
                spectrum_audio_player.gameObject.SetActive( false );
            }

            content.SetActive( false );
	    }

        // Shows or hides hotspot #######################################################################################################################################################################################################################
        public void SwitchHotspot() { 
            
            // Deactivate hotspot presentation
            if( content.activeSelf ) { 

                Deactivate();

                if( NetworkProjectManager.Instance != null ) { 
            
                    if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                        NetworkTransactionsControlTools.Instance.DeactivateHotspot( VertexStudio.Networking.NetworkPlayer.Instance.View_ID, hotspot_ID );
                    }
                }
            }

            // Activate hotspot presentation
            else { 
                
                Activate();

                if( NetworkProjectManager.Instance != null ) { 
            
                    if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkTransactionsControlTools.Instance != null) ) { 
                    
                        NetworkTransactionsControlTools.Instance.ActivateHotspot( VertexStudio.Networking.NetworkPlayer.Instance.View_ID, hotspot_ID );
                    }
                }
            }
        }

        // Shows hotspot ##############################################################################################################################################################################################################################
        public void Activate() { 
            
            content.SetActive( true );

            if( panel_presentation != null ) {

                panel_presentation.GetComponent<BoxCollider>().size = new Vector3( 
                
                    panel_presentation.GetComponent<RectTransform>().sizeDelta.x,
                    panel_presentation.GetComponent<RectTransform>().sizeDelta.y,
                    panel_presentation.GetComponent<BoxCollider>().size.z 
                );

                panel_presentation.gameObject.SetActive( true );
            }

            text_tooltip.text = CanvasHotspots.Instance.Text_to_deactivate;

            if( CanvasHotspots.Instance.Hotspot_opening_mode == HotspotOpeningMode.DeactivateAllAnother ) {
            
                CanvasHotspots.Instance.DeactivateAllHotspotsButSpecified( this );
            }

            if( (spectrum_audio_player != null) && (spectrum_audio_player.Audio_clip != null) ) {
                    
                spectrum_audio_player.PlayAudio();
            }
        }

        // Hidess hotspot ##############################################################################################################################################################################################################################
        public void Deactivate() { 
            
            if( panel_presentation != null ) {
                
                panel_presentation.gameObject.SetActive( false );
            }

            text_tooltip.text = CanvasHotspots.Instance.Text_to_activate;

            if( (spectrum_audio_player != null) && (spectrum_audio_player.Audio_clip != null) && (SpectrumAudioControl.Instance.Audio_source.clip == spectrum_audio_player.Audio_clip) ) {
                    
                spectrum_audio_player.StopAudio( () => { content.SetActive( false ); } );
            }

            else { 
                
                content.SetActive( false );
            }

            if( text_tooltip.gameObject.activeSelf ) { 

                text_tooltip.gameObject.SetActive( false );
            }
        }

        // Closes illustration #########################################################################################################################################################################################################################
        public void CloseIllustration() { 

        }
    }
}