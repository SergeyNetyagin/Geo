using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VertexStudio.Generic;

namespace VostokVR.Geo {

    public class SpectrumAudioPlayer : MonoBehaviour {

        [Space( 10 ), SerializeField]
        private AudioClip audio_clip;
        public AudioClip Audio_clip { get { return audio_clip; } }

        [Space( 10 ), SerializeField]
        private SpectrumAudioItem spectrum_item_prefab;

        [SerializeField]
        private RectTransform player_rect_transform;

        [SerializeField]
        private GridLayoutGroup grid_layout_group;

        private float[] samples;

        private SpectrumAudioItem[] spectrum_audio_items;

        private Coroutine samples_coroutine = null;
        private Coroutine volume_coroutine = null;

        // Use this for initialization ####################################################################################################################################################################################################################
        private void OnEnable() {

        }

        // Use this for initialization ####################################################################################################################################################################################################################
        private void OnDisable() {

        }

        /// <summary>
        /// Starts playing audio file.
        /// </summary>
        /// ###############################################################################################################################################################################################################################################
        public void PlayAudio() {

            SpectrumAudioControl.Instance.Audio_source.GetComponent<Transform>().position = player_rect_transform.position;
            SpectrumAudioControl.Instance.Audio_source.clip = audio_clip;

            if( !gameObject.activeSelf ) { 
                
                gameObject.SetActive( true );
            }

            if( samples_coroutine != null ) { 
                
                StopCoroutine( samples_coroutine );

                samples_coroutine = null;
            }

            if( volume_coroutine != null ) { 
                
                StopCoroutine( volume_coroutine );

                volume_coroutine = null;
            }

            if( spectrum_audio_items == null ) { 
                    
                CreateSpectrumItems();
            }

            GetComponent<Image>().enabled = true;
            
            volume_coroutine = StartCoroutine( ChangeVolumeSmoothly( 0f, 1f ) );
        }

        /// <summary>
        /// Stops playing audio file.
        /// </summary>
        /// ###############################################################################################################################################################################################################################################
        public void StopAudio( System.Action final_action = null ) {

            if( SpectrumAudioControl.Instance.Audio_source.clip == audio_clip ) {

                if( samples_coroutine != null ) { 
                
                    StopCoroutine( samples_coroutine );

                    samples_coroutine = null;
                }

                if( volume_coroutine != null ) { 
                
                    StopCoroutine( volume_coroutine );

                    volume_coroutine = null;
                }

                if( gameObject.activeInHierarchy ) {

                    DestroySpectrumItems();

                    GetComponent<Image>().enabled = false;

                    volume_coroutine = StartCoroutine( ChangeVolumeSmoothly( 1f, 0f, final_action ) );
                }
            }
        }

        /// <summary>
        /// Instantiates spectrum items.
        /// </summary>
        /// ###############################################################################################################################################################################################################################################
        private void CreateSpectrumItems() {

            samples = new float[ SpectrumAudioControl.Instance.Audio_samples ];

            grid_layout_group.enabled = true;

            float panel_width = player_rect_transform.sizeDelta.x;

            spectrum_audio_items = new SpectrumAudioItem[ SpectrumAudioControl.Instance.Audio_samples * 2 ];

            grid_layout_group.cellSize = new Vector2( panel_width / spectrum_audio_items.Length, player_rect_transform.sizeDelta.y );

            for( int i = 0; i < spectrum_audio_items.Length; i++ ) {

                spectrum_audio_items[i] = Instantiate( spectrum_item_prefab, player_rect_transform );
                
                spectrum_audio_items[i].name = spectrum_item_prefab.name + " #" + i;

                spectrum_audio_items[i].Image_sample_upper.GetComponent<RectTransform>().sizeDelta = new Vector2( 0f, player_rect_transform.sizeDelta.y * 0.5f );
                spectrum_audio_items[i].Image_sample_upper.color = ProjectManager.Instance.GetCompanyColor( ColorType.SpectrumColorUpper);

                spectrum_audio_items[i].Image_sample_lower.GetComponent<RectTransform>().sizeDelta = new Vector2( 0f, player_rect_transform.sizeDelta.y * 0.5f );
                spectrum_audio_items[i].Image_sample_lower.color = ProjectManager.Instance.GetCompanyColor( ColorType.SpectrumColorLower);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate( player_rect_transform );

            grid_layout_group.enabled = false;
        }

        /// <summary>
        /// Destroys spectrum items.
        /// </summary>
        /// ###############################################################################################################################################################################################################################################
        private void DestroySpectrumItems() {

            for( int i = player_rect_transform.childCount - 1; i >= 0; i-- ) { 
                
                Destroy( player_rect_transform.GetChild( i ).gameObject );
            }

            samples = null;

            spectrum_audio_items = null;
        }

        // Update is called once per a frame ##############################################################################################################################################################################################################
        private IEnumerator UpdateSamples() {

            while( (SpectrumAudioControl.Instance.Audio_source.clip == audio_clip) && SpectrumAudioControl.Instance.Audio_source.isPlaying ) {

                AudioListener.GetSpectrumData( samples, SpectrumAudioControl.Instance.Audio_channel, SpectrumAudioControl.Instance.Audio_fft_window );

                for( int i = 0; i < spectrum_audio_items.Length; i++ ) {

                    // Left samples
                    if( i < SpectrumAudioControl.Instance.Audio_samples ) {
                
                        spectrum_audio_items[i].Image_sample_upper.fillAmount = samples[ samples.Length - 1 - i ] * SpectrumAudioControl.Instance.Height_multiplier_left;
                        spectrum_audio_items[i].Image_sample_lower.fillAmount = samples[ samples.Length - 1 - i ] * SpectrumAudioControl.Instance.Height_multiplier_left;
                    }

                    // Right samples
                    else { 
                    
                        spectrum_audio_items[i].Image_sample_upper.fillAmount = samples[ i - samples.Length ] * SpectrumAudioControl.Instance.Height_multiplier_right;
                        spectrum_audio_items[i].Image_sample_lower.fillAmount = samples[ i - samples.Length ] * SpectrumAudioControl.Instance.Height_multiplier_right;
                    }
                }

                yield return null;
            }

            if( SpectrumAudioControl.Instance.Audio_source.clip == audio_clip ) {
                
                SpectrumAudioControl.Instance.Audio_source.Stop();
                SpectrumAudioControl.Instance.Audio_source.clip = null;
            }

            if( gameObject.activeSelf ) { 
                
                gameObject.SetActive( false );
            }

            yield break;
        }

        /// <summary>
        /// Starts playing audio file smoothly.
        /// </summary>
        /// ###############################################################################################################################################################################################################################################
        private IEnumerator ChangeVolumeSmoothly( float start_volume, float end_volume, System.Action final_action = null ) {

            SpectrumAudioControl.Instance.Audio_source.volume = start_volume;

            yield return null;

            if( start_volume < end_volume ) { 
            
                SpectrumAudioControl.Instance.Audio_source.Play();

                samples_coroutine = StartCoroutine( UpdateSamples() );
            }

            float time_ratio = 1f / ((start_volume < end_volume) ? SpectrumAudioControl.Instance.Audio_gain_time : SpectrumAudioControl.Instance.Audio_damping_time);
            
            float current_volume = start_volume;

            for( float timer = 0f; gameObject.activeInHierarchy && SpectrumAudioControl.Instance.Audio_source.isPlaying && (timer < SpectrumAudioControl.Instance.Audio_gain_time); timer += Time.deltaTime ) { 
            
                current_volume += Time.deltaTime * time_ratio * (end_volume - start_volume);

                if( current_volume > 1f ) { 
                
                    current_volume = 1f;
                }

                else if( current_volume < 0f ) { 
                
                    current_volume = 0f;
                }

                SpectrumAudioControl.Instance.Audio_source.volume = current_volume;

                yield return null;
            }

            SpectrumAudioControl.Instance.Audio_source.volume = end_volume;

            if( start_volume > end_volume ) { 
            
                if( SpectrumAudioControl.Instance.Audio_source.clip == audio_clip ) {
                
                    SpectrumAudioControl.Instance.Audio_source.Stop();
                    SpectrumAudioControl.Instance.Audio_source.clip = null;
                }

                if( samples_coroutine != null ) { 
                    
                    StopCoroutine( samples_coroutine );

                    samples_coroutine = null;

                    if( spectrum_audio_items != null ) { 
                    
                        DestroySpectrumItems();
                    }

                    if( gameObject.activeSelf ) { 
                
                        gameObject.SetActive( false );
                    }
                }
            }

            if( final_action != null ) { 
            
                final_action.Invoke();
            }

            yield break;
        }
    }
}