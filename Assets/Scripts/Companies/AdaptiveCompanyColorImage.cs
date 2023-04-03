using UnityEngine;
using UnityEngine.UI;

namespace VertexStudio.Generic {

    public class AdaptiveCompanyColorImage : MonoBehaviour {

        // Start is called before the first frame update
        private void OnEnable() {
        
            MaskableGraphic item = GetComponent<MaskableGraphic>();

            if( item == null ) { 
            
                Debug.LogError( "Cannot find an item to use color for adaptation!" );

                return;
            }

            else { 
            
                item.color = ProjectManager.Instance.GetCompanyColor( ColorType.BasicColor );
            }

            ImageSelectedColor image_selected_color_component = GetComponent<ImageSelectedColor>();

            if( (image_selected_color_component != null) && (item is Image) ) { 
            
                image_selected_color_component.Normal_color = item.color;
            }
        }
    }
}