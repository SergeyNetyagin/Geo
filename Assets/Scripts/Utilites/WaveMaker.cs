using UnityEngine;

namespace VostokVR.Geo {

    public class WaveMaker : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private Vector2 waves_max_speed = new Vector2( 0.01f, 0.01f );

        private MeshRenderer mesh_renderer;

        private Vector2 current_offset = Vector2.zero;

        // Use this for initialization #####################################################################################################################################################################################################################
        private void Awake() {

            mesh_renderer = GetComponent<MeshRenderer>();
        }

        // Use this for initialization #####################################################################################################################################################################################################################
        private void Start() {

            current_offset.x = mesh_renderer.material.mainTextureOffset.x;
            current_offset.y = mesh_renderer.material.mainTextureOffset.y;
        }

        // Update is called once per frame #################################################################################################################################################################################################################
        private void Update() {

            current_offset.x = (
                
                mesh_renderer.material.mainTextureOffset.x + 
                //((System.DateTime.Now.Millisecond > 500) ? 1f : -1f) *
                //Random.Range( 0f, waves_max_speed.x ) * 
                waves_max_speed.x * 
                Time.deltaTime
            );

            current_offset.y = (
                
                mesh_renderer.material.mainTextureOffset.y + 
                //((System.DateTime.Now.Millisecond > 500) ? 1f : -1f) *
                //Random.Range( 0f, waves_max_speed.y ) * 
                waves_max_speed.y * 
                Time.deltaTime
            );

            mesh_renderer.material.mainTextureOffset = current_offset;
        }
    }
}
