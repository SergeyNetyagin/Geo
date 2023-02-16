using UnityEngine;

namespace VertexStudio.Generic {

    public class AdaptiveCompanyColorMaterial : MonoBehaviour {

        // Start is called before the first frame update
        private void Start() {
        
            Renderer renderer = GetComponentInChildren<Renderer>( true );

            if( renderer == null ) { 
            
                Debug.LogError( "Cannot find a renderer to use color for adaptation!" );

                return;
            }

            else { 
            
                renderer.sharedMaterial.color = ProjectManager.Instance.GetCompanyColor( ColorType.HotspotColor );
            }
        }
    }
}