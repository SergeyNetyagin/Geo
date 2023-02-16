﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexStudio.Generic;

namespace VostokVR.Geo {

    public enum PlayerLocation {

        Front,
        Back
    }

    public enum PlayerViewMode {

        Outside,
        Inside
    }

    public enum EnvironmentType { 
    
        Rocks    = 0,
        Panorama = 1
    }

    public enum CanvasHotspotsMode { 
    
        Visible,
        Invisible
    }

    public enum CanvasControllerMode { 
    
        Normal,
        Small,
        Hidden
    }

    public class LevelManagerMain : LevelManager {

        public static LevelManagerMain Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private CanvasControllerMode canvas_controller_mode = CanvasControllerMode.Normal;
        public CanvasControllerMode Canvas_controller_mode { get { return canvas_controller_mode; } }

        [SerializeField]
        private CanvasHotspotsMode canvas_hotspots_mode = CanvasHotspotsMode.Visible;
        public CanvasHotspotsMode Canvas_hotspots_mode { get { return canvas_hotspots_mode; } }

        [Space( 10 ), SerializeField]
        private GameObject[] helmet_invisible_objects;

        [Space( 10 ), SerializeField]
        protected Transform instantiating_group_parent;
        public Transform Instantiating_group_parent { get { return instantiating_group_parent; } }

        [Space( 10 ), SerializeField]
        private TexturesQualityControl rocks;
        public TexturesQualityControl Rocks { get { return rocks; } }

        [SerializeField]
        private PanoramaControl panorama;
        public PanoramaControl Panorama { get { return panorama; } }

        [SerializeField]
        private MeshRenderer ground_mesh_renderer;
        public MeshRenderer Ground_mesh_renderer { get { return ground_mesh_renderer; } }

        // Use this for initialization ####################################################################################################################################################################################################################
        protected override void Awake() {

            Instance = this;

            base.Awake();

            List<Collider> all_colliders_list = new List<Collider>();

            for( int i = 0; i < helmet_invisible_objects.Length; i++ ) { 
                
                Collider[] colliders = helmet_invisible_objects[i].GetComponentsInChildren<Collider>( true );

                for( int collider_index = 0; collider_index < colliders.Length; collider_index++ ) { 
                    
                    all_colliders_list.Add( colliders[ collider_index ] );
                }
            }

            for( int i = 0; i < excluded_helmet_colliders.Length; i++ ) { 
            
                all_colliders_list.Add( excluded_helmet_colliders[i] );
            }

            excluded_helmet_colliders = all_colliders_list.ToArray();
        }

        // Use this for initialization ####################################################################################################################################################################################################################
        protected override void Start() {

            base.Start();

            if( !ProjectData.Instance.Sphere_follow_player ) { 
                
                if( panorama != null ) {
                
                    panorama.enabled = (panorama is PanoramaPictureControl);
                }
            }

            if( panorama != null ) {
            
                panorama.gameObject.SetActive( false );
            }
        }

        // Activates canvas controller in the spedified intro is inactive #################################################################################################################################################################################
        public void ActivateCanvasController( Animator intro_animator ) {

            if( !intro_animator.gameObject.activeSelf || !intro_animator.enabled ) {
                
                CanvasController.Instance.Activate();
            }
        }

        // Activates canvas controller forcibly ###########################################################################################################################################################################################################
        public void ActivateCanvasController() {

            StartCoroutine( ActivateCanvasControllerForcibly() );
        }

        private IEnumerator ActivateCanvasControllerForcibly() {

            if( CanvasController.Instance != null ) { 
                
                CanvasController.Instance.Activate();
            }

            while( CanvasController.Instance != null ) { 
            
                if( !CanvasController.Instance.gameObject.activeSelf ) {
                
                    CanvasController.Instance.Activate();
                }

                yield return new WaitForSeconds( 1f );
            }

            yield break;
        }
    }
}