using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using VertexStudio.VirtualRealty;

namespace VostokVR.Geo {

    public class IntroControl : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private Animator animator;

        [SerializeField]
        private AudioSource audio_source;

        [SerializeField]
        private SceneAttribute hotspots_scene_attribute;

        [SerializeField]
        bool freeze_player_position = true;

        [Space( 10 ), SerializeField]
        private Canvas canvas_annotation;

        [SerializeField]
        private Vector3 vr_annotation_player_offset = Vector3.zero;

        [SerializeField]
        private Vector3 non_vr_annotation_player_offset = Vector3.zero;

        [Space( 10 ), SerializeField]
        private string intro_key = string.Empty;

        [Space( 10 ), SerializeField]
        private UnityEvent onStartIntro;

        [Space( 10 ), SerializeField]
        private UnityEvent onStopIntro;

        private Vector3 start_player_position = Vector3.zero;

        private bool freeze_player = true;

        // OnEnable is called before the first frame update ##############################################################################################################################################################################################
        private void OnEnable() {

            if( animator != null ) { 
                
                animator.enabled = false;
            }
        }

        // Start is called before the first frame update #################################################################################################################################################################################################
        private void Start() {
        
            start_player_position = new Vector3( 
                
                ViveInteractionsManager.Instance.Eye_camera_transform.position.x,
                ViveInteractionsManager.Instance.Camera_rig_transform.position.y,
                ViveInteractionsManager.Instance.Eye_camera_transform.position.z
            );

            StartCoroutine( CheckForKey() );
        }

        // Update is called once per frame ##############################################################################################################################################################################################################
        private void Update() {
            
            if( freeze_player ) { 
            
                ViveInteractionsManager.Instance.Teleport( start_player_position );
            }
        }

        // Checks for existence of the key ###############################################################################################################################################################################################################
        private IEnumerator CheckForKey() { 

            yield return new WaitForEndOfFrame();

            if( !PlayerPrefs.HasKey( intro_key ) ) { 

                freeze_player = freeze_player_position;
                
                #if( UNITY_EDITOR )
                //Debug.Log( "Key " + intro_key + " does not exist in player settings: starting the introduction..." );
                #endif

                if( animator != null ) { 
                
                    animator.enabled = true;
                }

                if( hotspots_scene_attribute != null ) { 
                
                    hotspots_scene_attribute.gameObject.SetActive( false );
                }

                PlayerPrefs.SetString( intro_key, intro_key );
                PlayerPrefs.Save();

                if( onStartIntro != null ) {
                
                    onStartIntro.Invoke();
                }

                #if( UNITY_EDITOR )
                //Debug.Log( "Intro " + gameObject.name + " started..." );
                #endif
            }

            else {

                freeze_player = false;

                #if( UNITY_EDITOR )
                //Debug.Log( "Key " +  intro_key + " exists in player settings: introduction was skipped." );
                #endif
            }
        
            yield break;
        }

        // Adapts intro annotation position for player ###################################################################################################################################################################################################
        public void AdaptCanvasAnnotationForPlayer() { 
            
            canvas_annotation.transform.position = ViveInteractionsManager.Instance.Eye_camera_transform.position;
            
            canvas_annotation.transform.position = new Vector3( 
                
                canvas_annotation.transform.position.x, 
                ViveInteractionsManager.Instance.Camera_rig_transform.position.y, 
                canvas_annotation.transform.position.z 
            );

            if( SteamVR.active ) {
            
                canvas_annotation.transform.Translate( vr_annotation_player_offset, Space.Self );
            }

            else { 
                
                canvas_annotation.transform.Translate( non_vr_annotation_player_offset, Space.Self );
            }
        }

        // Stops intro ###################################################################################################################################################################################################################################
        public void StopIntro() { 

            if( onStopIntro != null ) {
                
                onStopIntro.Invoke();
            }

            #if( UNITY_EDITOR )
            //Debug.Log( "Intro " + gameObject.name + " stopped" );
            #endif

            if( animator != null ) { 
                
                animator.enabled = false;
            }

            if( hotspots_scene_attribute != null ) { 

                LevelManagerMain level_manager_main = LevelManagerMain.Instance;

                if( LevelManagerMain.Instance == null ) {

                    level_manager_main = FindObjectOfType<LevelManagerMain>();
                }

                bool activate = SteamVR.active ? true : (level_manager_main.Starting_controller_mode != CanvasControllerMode.Hidden);
                
                hotspots_scene_attribute.gameObject.SetActive( activate );
            }

            freeze_player = false;
        }

        // On application quit ###########################################################################################################################################################################################################################
        private void OnApplicationQuit() {
            
            if( PlayerPrefs.HasKey( intro_key ) ) { 
                
                PlayerPrefs.DeleteKey( intro_key );
                PlayerPrefs.Save();

                #if( UNITY_EDITOR )
                //Debug.Log( "Key " + intro_key + " was removed from player settings." );
                #endif
            }
        }

        // Sets annotation text to the specified ##########################################################################################################################################################################################################
        public void SetAnnotation( Text text ) { 

            if( text != null ) { 
                
                text.gameObject.SetActive( true );
            }
        }

        // Sets annotation text to the specified ##########################################################################################################################################################################################################
        public void SetAnnotation( string annotation_text ) { 

        }

        // Starts playing the new clip ####################################################################################################################################################################################################################
        public void StartPlaying( AudioClip clip ) { 

            if( audio_source == null ) {

                return;
            }
                
            audio_source.Stop();
            audio_source.clip = clip;
            audio_source.Play();
        }

        // Pauses playing the current clip ################################################################################################################################################################################################################
        public void PausePlaying() { 

            if( audio_source == null ) {

                return;
            }
                
            if( audio_source.isPlaying ) {
            
                audio_source.Pause();
            }
        }

        // Resumes playing the current clip ###############################################################################################################################################################################################################
        public void ResumePlaying() { 

            if( audio_source == null ) {

                return;
            }
                
            if( !audio_source.isPlaying ) {
            
                audio_source.Play();
            }
        }
    }
}