using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace VostokVR.Geo {

    public class TeleportPointsControl : MonoBehaviour {

	    // Use this for initialization #####################################################################################################################################################################################################################
	    void Start() {
		
	    }

        // Shows telepot points ############################################################################################################################################################################################################################
        [ContextMenu( "SHOW TELEPOR POINTS" )]
        private void ShowTeleportPoints() {

            TeleportPoint[] points = GetComponentsInChildren<TeleportPoint>( true );

            for( int i = 0; i < points.Length; i++ ) points[i].GetComponent<MeshRenderer>().enabled = true;

            #if UNITY_EDITOR
            if( !Application.isPlaying ) EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene() );
            #endif
        }

        // Hides telepot points ############################################################################################################################################################################################################################
        [ContextMenu( "HIDE TELEPOR POINTS" )]
        private void HideTeleportPoints() {

            TeleportPoint[] points = GetComponentsInChildren<TeleportPoint>( true );

            for( int i = 0; i < points.Length; i++ ) points[i].GetComponent<MeshRenderer>().enabled = false;

            #if UNITY_EDITOR
            if( !Application.isPlaying ) EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene() );
            #endif
        }
    }
}