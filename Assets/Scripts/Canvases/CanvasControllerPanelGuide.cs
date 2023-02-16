using UnityEngine;
using UnityEngine.UI;

namespace VostokVR.Geo {

    public class CanvasControllerPanelGuide : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private GameObject arrow_left;

        [SerializeField]
        private GameObject arrow_right;

        [SerializeField]
        private Text text_pages;

        [Space( 10 ), SerializeField]
        private CanvasGuidePage[] guide_pages;

        private int current_page_index = 0;
        public int Current_page_index { get { return current_page_index; } }

        // OnEnable is called before the first frame update ###############################################################################################################################################################################################
        private void OnEnable() {

            if( (guide_pages == null) || (guide_pages.Length == 0) ) {
            
                guide_pages = GetComponentsInChildren<CanvasGuidePage>( true );
            }

            RefreshCurrentPage();
        }

        // Goes to the specified page ####################################################################################################################################################################################################################
        public void GoToPage( int page_index ) { 

            if( page_index >= guide_pages.Length ) { 
            
                return;
            }

            current_page_index = page_index;

            RefreshCurrentPage();
        }

        // Refreshes current page #########################################################################################################################################################################################################################
        public void RefreshCurrentPage() { 

            for( int i = 0; i < guide_pages.Length; i++ ) { 
            
                if( current_page_index == i ) { 
                    
                    guide_pages[i].gameObject.SetActive( true );
                }

                else { 
                
                    guide_pages[i].gameObject.SetActive( false );
                }
            }

            if( current_page_index == 0 ) { 
            
                arrow_left.SetActive( false );
                arrow_right.SetActive( true );
            }

            else if( current_page_index == (guide_pages.Length - 1) ) {

                arrow_left.SetActive( true );
                arrow_right.SetActive( false );
            }

            else { 

                arrow_left.SetActive( true );
                arrow_right.SetActive( true );
            }

            text_pages.text = (current_page_index + 1).ToString() + "/" + guide_pages.Length;
        }

        // Moves page left ################################################################################################################################################################################################################################
        public void PageLeft() { 

            current_page_index--;

            if( current_page_index < 0 ) { 
            
                current_page_index = 0;
            }

            RefreshCurrentPage();
        }

        // Moves page right ###############################################################################################################################################################################################################################
        public void PageRight() { 

            current_page_index++;

            if( current_page_index >= guide_pages.Length ) { 
            
                current_page_index = guide_pages.Length - 1;
            }

            RefreshCurrentPage();
        }
    }
}