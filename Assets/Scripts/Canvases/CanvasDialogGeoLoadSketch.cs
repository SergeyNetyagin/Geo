using UnityEngine;
using UnityEngine.UI;
using VertexStudio.Generic;
using VertexStudio.Networking;

namespace VostokVR.Geo {

    /// <summary>
    /// Shows confirming dialog window for loading sketch.
    /// </summary>
    public class CanvasDialogGeoLoadSketch : CanvasDialog {

        public static new CanvasDialogGeoLoadSketch Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private Button button_ok;

        [SerializeField]
        private Button button_cancel;

        [SerializeField]
        private Button button_group;

        // Use this for initialization #####################################################################################################################################################################################################################
        protected override void Awake() {

            Instance = this;

            base.Awake();
	    }

        // Use this for initialization #####################################################################################################################################################################################################################
	    protected override void Start() {
		
            base.Start();
        }

        // Use this for initialization #####################################################################################################################################################################################################################
	    protected override void OnEnable() {

            base.OnEnable();
        }

        // Use this for initialization #####################################################################################################################################################################################################################
	    protected override void OnDisable() {

            base.OnDisable();
        }

        /// <summary>
        /// Shows actions' message and dialog window.
        /// </summary>
        /// ################################################################################################################################################################################################################################################
        public override void Show( DialogType dialog_type ) {

            if( (NetworkConnectionManager.Instance != null) && NetworkConnectionManager.Instance.Is_master_client ) {
            
                button_group.gameObject.SetActive( true );
            }

            else { 
                
                button_group.gameObject.SetActive( false );
            }

            base.Show( dialog_type );
        }
    }
}