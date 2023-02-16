using UnityEngine;
using UnityEngine.Video;

namespace VertexStudio.Networking {

    public class VideoPlayerCreator : MonoBehaviour {

        [Space( 10 ), SerializeField, Tooltip( "The movie that will play on the specific mesh" )]
        private VideoClip screen_video_clip;

        [SerializeField, Tooltip( "The screen's mesh renderer where will play the specific clip" )]
        private MeshRenderer screen_mesh_renderer;

        [SerializeField, Tooltip( "Use (or not use) clip's sound by playing the clip" )]
        private bool use_audio_channels = false;

        private VideoPlayer video_player;

        // Use this for initialization #########################################################################################################################################################################################################################
        private void Awake() {
		
            if( screen_mesh_renderer == null ) screen_mesh_renderer = GetComponent<MeshRenderer>();
	    }
    
        // Use this for initialization #########################################################################################################################################################################################################################
	    private void Start() {
    /*
            if( (screen_mesh_renderer != null) && (CommonVideoSettings.Instance.Screen_video_material != null) && (screen_video_clip != null) ) {

                if( screen_mesh_renderer.material != CommonVideoSettings.Instance.Screen_video_material ) screen_mesh_renderer.material = CommonVideoSettings.Instance.Screen_video_material;

                video_player = (
                
                    screen_mesh_renderer.GetComponent<VideoPlayer>() == null ?
                    screen_mesh_renderer.gameObject.AddComponent<VideoPlayer>() : 
                    screen_mesh_renderer.gameObject.GetComponent<VideoPlayer>()
                );

                video_player.source = VideoSource.VideoClip;
                video_player.clip = screen_video_clip;
                video_player.playOnAwake = true;
                video_player.waitForFirstFrame = true;
                video_player.isLooping = true;
                video_player.playbackSpeed = 1f;
                video_player.renderMode = VideoRenderMode.MaterialOverride;
                video_player.targetMaterialRenderer = screen_mesh_renderer;
                video_player.targetMaterialProperty = CommonVideoSettings.Instance.Screen_material_property;
                video_player.audioOutputMode = use_audio_channels ?  VideoAudioOutputMode.Direct : VideoAudioOutputMode.None;
                video_player.Play();

                if( video_player.GetComponent<VideoPlayerVisibilityController>() == null ) video_player.gameObject.AddComponent<VideoPlayerVisibilityController>();

                if( screen_mesh_renderer.isVisible ) video_player.Play();
                else video_player.Pause();
            }*/
	    }
    }
}