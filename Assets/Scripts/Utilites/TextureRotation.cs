using UnityEngine;

namespace VostokVR.Geo {

    public class TextureRotation : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private Vector2 texture_offset_speed = new Vector2( 1f, 0f );

        private Vector2 texture_current_offset = Vector2.zero;

        private MeshRenderer mesh_renderer;

        private int main_texture_ID = -1;

	    // Use this for initialization
	    private void Start() {
		
            mesh_renderer = GetComponent<MeshRenderer>();

            main_texture_ID = Shader.PropertyToID( "_MainTex" );
	    }
	
	    // Update is called once per frame
	    private void Update() {
		
            texture_current_offset = mesh_renderer.material.GetTextureOffset( main_texture_ID );

            texture_current_offset.x += texture_offset_speed.x * Time.deltaTime;
            texture_current_offset.y += texture_offset_speed.y * Time.deltaTime;

            mesh_renderer.material.SetTextureOffset( main_texture_ID, texture_current_offset );
	    }
    }
}