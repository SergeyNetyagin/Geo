using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Utilite class to make outlines for Image component.
/// </summary>
public class OutlineMaker : MonoBehaviour {

    [Space( 10 ), SerializeField]
    private string parent_outline_object_name = "Image Outline";

    [SerializeField]
    private float outline_thickness = 0.0002f;

    [Space( 10 ), SerializeField]
    private Image prefab_bar_left;

    [SerializeField]
    private Image prefab_bar_right;

    [SerializeField]
    private Image prefab_bar_top;

    [SerializeField]
    private Image prefab_bar_bottom;


    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start() {
        
    }


    /// <summary>
    /// Makes an outline.
    /// </summary>
    #if( UNITY_EDITOR )
    [ContextMenu( "Make outlines" )]
    private void MakeOultiles() {

        OutlineParent[] outline_parent_transforms = GetComponentsInChildren<OutlineParent>( true );

        for( int i = 0; i < outline_parent_transforms.Length; i++ ) { 
        
            RectTransform parent_transform = outline_parent_transforms[i].GetComponent<RectTransform>();

            Image image = parent_transform.GetComponent<Image>();

            if( image != null ) { 

                if( Application.isPlaying ) {
                
                    Destroy( image );
                }

                else { 
                    
                    DestroyImmediate( image );
                }
            }

            parent_transform.anchoredPosition3D = Vector3.zero;
            parent_transform.sizeDelta = Vector2.zero;
            parent_transform.anchorMin = Vector2.zero;
            parent_transform.anchorMax = Vector2.one;
            parent_transform.pivot = new Vector2( 0.5f, 0.5f );

            OutlineBar[] existing_bars = parent_transform.GetComponentsInChildren<OutlineBar>( true );

            for( int existing_bar_index = 0; existing_bar_index < existing_bars.Length; existing_bar_index ++ ) { 

                if( Application.isPlaying ) {
                
                    Destroy( existing_bars[ existing_bar_index ].gameObject );
                }

                else { 
                    
                    DestroyImmediate( existing_bars[ existing_bar_index ].gameObject );
                }                
            }

            Image bar_left   = Instantiate( prefab_bar_left, parent_transform, false );
            Image bar_right  = Instantiate( prefab_bar_right, parent_transform, false );
            Image bar_top    = Instantiate( prefab_bar_top, parent_transform, false );
            Image bar_bottom = Instantiate( prefab_bar_bottom, parent_transform, false );

            bar_left.name   = prefab_bar_left.name;
            bar_right.name  = prefab_bar_right.name;
            bar_top.name    = prefab_bar_top.name;
            bar_bottom.name = prefab_bar_bottom.name;

            if( parent_transform.gameObject.activeSelf ) { 
                
                parent_transform.gameObject.SetActive( false );
            }
        }

    }
    #endif
}