using System.Collections;
using UnityEngine;

namespace VostokVR.Geo {

    public class NavMeshControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private bool use_navigation_grid = false;

        [SerializeField]
        private bool use_navigation_border = true;

        [Space( 10 ), SerializeField]
        private AnimationCurve alpha_curve;

        [SerializeField]
        private MeshRenderer nav_mesh_renderer;

        [SerializeField]
        private MeshRenderer border_mesh_renderer;

        [SerializeField, Range( 1f, 10f )]
        private float blinking_duration = 5f;

        private bool is_shown = false;
        public bool Is_shown { get { return is_shown; } }

        private int shader_property_ID = 0;

        private Color original_color = Color.green;
        private Color current_color = Color.clear;

        // Use this for initialization #####################################################################################################################################################################################################################
        private void Awake() {

            shader_property_ID = Shader.PropertyToID( "_TintColor" );

            original_color = nav_mesh_renderer.sharedMaterial.GetColor( shader_property_ID );
            original_color.a = 0f;

            nav_mesh_renderer.sharedMaterial.SetColor( shader_property_ID, original_color );
        }

        // Use this for initialization #####################################################################################################################################################################################################################
        private void Start() {

            nav_mesh_renderer.sharedMaterial.SetColor( shader_property_ID, original_color );

            if( use_navigation_grid ) {

                if( !nav_mesh_renderer.enabled ) nav_mesh_renderer.enabled = true;
            }

            else {

                if( nav_mesh_renderer.enabled ) nav_mesh_renderer.enabled = false;
            }

            if( use_navigation_border ) {

                if( !border_mesh_renderer.enabled ) border_mesh_renderer.enabled = true;
            }

            else {

                if( border_mesh_renderer.enabled ) border_mesh_renderer.enabled = false;
            }
        }

        // Shows teleport area #############################################################################################################################################################################################################################
        public void ShowTeleportArea() {

            if( (alpha_curve != null) && (alpha_curve.keys != null) && (alpha_curve.keys.Length != 0) ) {

                is_shown = true;

                StartCoroutine( ShowTeleportArea( alpha_curve.keys[ alpha_curve.keys.Length - 1 ].time / blinking_duration ) );
            }
        }

        // Hides teleport area #############################################################################################################################################################################################################################
        public void HideTeleportArea() {

            is_shown = false;

            nav_mesh_renderer.sharedMaterial.SetColor( shader_property_ID, original_color );
        }

        // Makes teleport's area blinking ##################################################################################################################################################################################################################
        private IEnumerator ShowTeleportArea( float time_rate ) {

            if( shader_property_ID == 0 ) shader_property_ID = Shader.PropertyToID( "_TintColor" );

            current_color = original_color;

            for( float time = 0f; is_shown; time += Time.deltaTime ) {

                #if UNITY_EDITOR
                time_rate = alpha_curve.keys[ alpha_curve.keys.Length - 1 ].time / blinking_duration;
                #endif

                current_color.a = alpha_curve.Evaluate( Mathf.PingPong( time * time_rate, blinking_duration * time_rate ) );

                nav_mesh_renderer.sharedMaterial.SetColor( shader_property_ID, current_color );

                yield return null;
            }

            nav_mesh_renderer.sharedMaterial.SetColor( shader_property_ID, original_color );

            yield break;
        }

        // Restores original material's color ##############################################################################################################################################################################################################
        private void OnApplicationQuit() {

            if( nav_mesh_renderer != null ) nav_mesh_renderer.sharedMaterial.SetColor( shader_property_ID, original_color );
        }
    }
}