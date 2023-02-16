using UnityEngine;
using UnityEngine.UI;

namespace VertexStudio.Generic {

    public class AdaptiveCompanyColorArrow : MonoBehaviour {

        // Start is called before the first frame update
        private void OnEnable() {
        
            MaskableGraphic item = GetComponent<MaskableGraphic>();

            if( item == null ) { 
            
                Debug.LogError( "Cannot find an item to use color for adaptation!" );

                return;
            }

            else { 
            
                item.color = ProjectManager.Instance.GetCompanyColor( ColorType.ArrowColor );
            }
        }
    }
}