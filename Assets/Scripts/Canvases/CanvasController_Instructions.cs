using UnityEngine;
using UnityEngine.UI;
using VertexStudio.VirtualRealty;

namespace VostokVR.Geo {

    public class CanvasController_Instructions : MonoBehaviour {

		[Space( 10 ), SerializeField]
		private Button button_collaplse_menu;


        private void Start() {
        
        }


		private void OnEnable() {
		
			if( ViveInteractionsManager.Instance.Device_model == DeviceModel.Standalone ) {

				button_collaplse_menu.gameObject.SetActive( false );
			}
		}


		private void OnDisable() {

			if( ViveInteractionsManager.Instance.Device_model == DeviceModel.Standalone ) {

				button_collaplse_menu.gameObject.SetActive( true );
			}			
		}
	}
}