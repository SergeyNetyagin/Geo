using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace VostokVR.Geo {

    public class NavigationPivotsControl : MonoBehaviour {

	    // Use this for initialization #####################################################################################################################################################################################################################
	    private void Start() {

	    }

        // Shows geometry pivots ###########################################################################################################################################################################################################################
        [ContextMenu( "SHOW NAVIGATION PIVOTS" )]
        private void ShowGeometryPivots() {

            GeometryPivot[] pivots = GetComponentsInChildren<GeometryPivot>( true );

            for( int i = 0; i < pivots.Length; i++ ) pivots[i].gameObject.SetActive( true );

            #if UNITY_EDITOR
            if( !Application.isPlaying ) EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene() );
            #endif
        }

        // Hides geometry pivots ###########################################################################################################################################################################################################################
        [ContextMenu( "HIDE NAVIGATION PIVOTS" )]
        private void HideGeometryPivots() {

            GeometryPivot[] pivots = GetComponentsInChildren<GeometryPivot>( true );

            for( int i = 0; i < pivots.Length; i++ ) pivots[i].gameObject.SetActive( false );

            #if UNITY_EDITOR
            if( !Application.isPlaying ) EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene() );
            #endif
        }
    }
}