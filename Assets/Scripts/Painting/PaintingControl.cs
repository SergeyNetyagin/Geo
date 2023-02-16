using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using VertexStudio.Generic;
using VertexStudio.VirtualRealty;
using VertexStudio.Networking;

namespace VostokVR.Geo {

    public enum PaintingColor { 
    
        Unknown,
        Red,
        Green,
        Blue,
        Cyan,
        Orange
    }

    [System.Serializable]
    public struct PaintingPointFeatures { 
        
        public float Point_position_x;
        public float Point_position_y;
        public float Point_position_z;
        
        public int Color_index;
    }

    [System.Serializable]
    public class Paint { 
    
        [SerializeField]
        public PaintingColor Painting_color = PaintingColor.Red;

        [SerializeField]
        public Material Painting_material;
    }

    public class PaintingControl : MonoBehaviour {

        public static PaintingControl Instance { get; private set; }

        [Space( 10 ), SerializeField]
        private PaintingPoint painting_point_prefab;
        public PaintingPoint Painting_point_prefab { get { return painting_point_prefab; } }

        [Space( 10 ), SerializeField, Range( 0f, 0.1f )]
        private float paint_line_length = 0.01f;
        public float Paint_line_length { get { return paint_line_length; } }

        [SerializeField, Range( 0f, 0.1f )]
        private float paint_line_width = 0.05f;
        public float Paint_line_width { get { return paint_line_width; } }

        [SerializeField, Range( 0f, 0.005f )]
        private float paint_cell_offset = 0.001f;
        public float Paint_cell_offset { get { return paint_cell_offset; } }

        [SerializeField, Range( 0f, 0.1f )]
        private float eraser_cell_size = 0.05f;
        public float Eraser_cell_size { get { return eraser_cell_size; } }

        [Space( 10 ), SerializeField]
        private int remote_points_sending_quantity = 100;

        [Space( 10 ), SerializeField]
        private PaintingGroup painting_group_local;
        public PaintingGroup Painting_group_local { get { return painting_group_local; } }

        [Space( 10 ), SerializeField]
        private Paint[] paints;

        private Paint current_paint;
        public Paint Current_paint { get { return current_paint; } set { current_paint = value; } }

        private List<PaintingGroup> painting_groups = new List<PaintingGroup>();

        private List<Collider> touched_pixels = new List<Collider>();

        private Transform painting_point_transform;

        // Awake is called before the first frame update ##################################################################################################################################################################################################
        private void Awake() {
        
            Instance = this;

            painting_point_transform = new GameObject( "Painting Point Transform" ).GetComponent<Transform>();
            painting_point_transform.SetParent( transform, true );
            painting_point_transform.position = Vector3.zero;

            current_paint = ((paints == null) || (paints.Length == 0)) ? null : paints[0];
        }

        // OnEnable is called before the first frame update ###############################################################################################################################################################################################
        private void OnEnable() {

            if( current_paint == null ) { 
            
                current_paint = ((paints == null) || (paints.Length == 0)) ? null : paints[0];                
            }
        }

        // OnDisable is called when the object is disabling ###############################################################################################################################################################################################
        private void OnDisable() {

        }

        // Start is called before the first frame update ##################################################################################################################################################################################################
        private void Start() {

            if( painting_group_local != null ) { 
            
                painting_groups.Add( painting_group_local );
            }
        }

        // Registers painting group with the specified ID #################################################################################################################################################################################################
        public void RegisterPaintingGroup( PaintingGroup painting_group ) {

            if( !painting_groups.Contains( painting_group ) ) {
            
                painting_groups.Add( painting_group );
            }
        }

        // Returns painting group by the specified ID #####################################################################################################################################################################################################
        public PaintingGroup GetPaintingGroup( int view_ID ) { 
        
            PaintingGroup painting_group = null;

            for( int i = 0; i < painting_groups.Count; i++ ) { 
                
                if( painting_groups[i].Network_player_view_ID == view_ID ) { 
                
                    painting_group = painting_groups[i];

                    break;
                }
            }

            return painting_group;
        }

        // Draws one point or whole line if distance is too long ##########################################################################################################################################################################################
        public void PaintLine( Vector3 destination, PaintingGroup painting_group = null ) { 
        
            if( painting_group == null) { 
                
                painting_group = painting_group_local;
            }

            if( Vector3.Distance( painting_point_transform.position, destination ) < paint_line_length ) { 

                return;
            }

            InstantiatePointInsideLine( destination, painting_group );

            if( painting_point_transform.position == Vector3.zero ) { 
            
                painting_point_transform.position = destination;

                return;
            }

            float distance = Vector3.Distance( painting_point_transform.position, destination );

            float line_length = paint_line_length * 0.5f;

            int steps = (int) (distance / line_length);

            // Paint at least one pixel or more
            for( int i = 0; i < steps; i++ ) {

                painting_point_transform.LookAt( destination );
                painting_point_transform.Translate( 0f, 0f, paint_line_length, Space.Self );

                InstantiatePointInsideLine( painting_point_transform.position, painting_group );
            }
        }

        // Creates and adds the new pixel to the list #####################################################################################################################################################################################################
        public PaintingPoint InstantiatePoint( Vector3 point_position, PaintingGroup painting_group ) { 

            PaintingPoint painting_point = Instantiate( 
                    
                painting_point_prefab, 
                point_position, 
                Quaternion.identity, 
                painting_group.Group_transform
            );

            if( current_paint == null ) { 
            
                current_paint = paints[0];
            }

            if( painting_point.Point_renderer is SpriteRenderer ) {
                    
                (painting_point.Point_renderer as SpriteRenderer).color = current_paint.Painting_material.color;
            }

            else if( painting_point.Point_renderer is MeshRenderer ) {
                    
                painting_point.Point_renderer.sharedMaterial = current_paint.Painting_material;
            }


            painting_point.Point_ID = painting_group.GetNextPointID();
            painting_point.Parent_painting_group = painting_group;
            painting_point.Point_transform.localScale = new Vector3( paint_line_width, paint_line_width, paint_line_width );
            painting_point.Point_color = current_paint.Painting_color;

            painting_point.name = painting_point_prefab.name + " (" + painting_point.Point_ID + ")";

            return painting_point;
        }

        // Add the new pixel to the list during line drawing ##############################################################################################################################################################################################
        private void InstantiatePointInsideLine( Vector3 point_position, PaintingGroup painting_group ) { 
            
            if( painting_group == null) { 
                
                painting_group = painting_group_local;
            }

            PaintingPoint painting_point = InstantiatePoint( point_position, painting_group );

            painting_point.Point_transform.LookAt( ViveInteractionsManager.Instance.Eye_camera_transform );
            painting_point.Point_transform.Translate( 0f, 0f, paint_cell_offset, Space.Self );
            painting_point.Point_transform.localScale = new Vector3( paint_line_width, paint_line_width, paint_line_width );

            if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkProjectManager.Instance != null) && (NetworkTransactionsControlPainting.Instance != null ) ) { 
                
                NetworkTransactionsControlPainting.Instance.InstantiatePoint( 
                    
                    VertexStudio.Networking.NetworkPlayer.Instance.View_ID, 
                    painting_point.Point_transform.position, 
                    (int) current_paint.Painting_color
                );
            }
        }

        // Add the new pixel to the list ##################################################################################################################################################################################################################
        public void AddContactedPixelToList( Collider pixel_collider ) { 

            if( CanvasController.Instance.Current_work_mode == ActiveMode.Erasing ) {
            
                touched_pixels.Add( pixel_collider );
            }
        }

        // Destroys pixel #################################################################################################################################################################################################################################
        public void ErasePixels( PaintingGroup painting_group = null ) { 

            if( painting_group == null) { 
                
                painting_group = painting_group_local;
            }

            if( touched_pixels.Count != 0 ) { 

                for( int i = touched_pixels.Count - 1; i >= 0; i-- ) {

                    Destroy( touched_pixels[i].gameObject );
                }

                touched_pixels.Clear();

                Resources.UnloadUnusedAssets();
            }
        }

        // Shows all pixels in painting groups ############################################################################################################################################################################################################
        public void ShowAllPaintings() { 

            for( int i = 0; i < painting_groups.Count; i++ ) { 

                painting_groups[i].ShowAllPoints();
            }
        }

        // Hides all pixels in painting groups ############################################################################################################################################################################################################
        public void HideAllPaintings() { 

            for( int i = 0; i < painting_groups.Count; i++ ) { 

                painting_groups[i].HideAllPoints();
            }
        }

        // Destroys all pixels in painting ################################################################################################################################################################################################################
        public void ClearAllPixels( PaintingGroup painting_group = null ) { 

            if( painting_group == null) { 
                
                painting_group = painting_group_local;
            }

            touched_pixels.Clear();

            painting_group.ClearAllPixels();
        }

        // Pauses painting ################################################################################################################################################################################################################################
        public void PausePaint() {

            painting_point_transform.position = Vector3.zero;
        }

        // Saves painting to disk ########################################################################################################################################################################################################################
        public void SaveSketch() { 
            
            PaintingPointFeatures[] painting_point_features = GetPaintingPointFeatures();

            if( (painting_point_features == null) || (painting_point_features.Length == 0) ) { 
            
                return;
            }

            string path = GetSketchFilePath();

            if( File.Exists( path ) ) { 
            
                try { 
                
                    File.Delete( path );
                }

                catch( Exception exception ) { 
                    
                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot delete file " + path + ": " + exception.Message );
                    #endif
                }
            }

            if( !Directory.Exists( Path.GetDirectoryName( path ) ) ) { 
            
                try { 
                    
                    Directory.CreateDirectory( Path.GetDirectoryName( path ) );
                }

                catch( Exception exception ) { 

                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot create directory " + Path.GetDirectoryName( path ) + ": " + exception.Message );
                    #endif                    
                }
            }

            bool is_serialization_error = false;

            using( FileStream file_stream = File.Create( path ) ) {

                BinaryFormatter formatter = new BinaryFormatter();

                try {
                
                    formatter.Serialize( file_stream, painting_point_features );
                }

                catch( Exception exception ) { 

                    is_serialization_error = true;

                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot serialize pixels array to " + path + ": " + exception.Message );
                    #endif                    
                }
            }

            if( !is_serialization_error ) { 

                #if( UNITY_EDITOR )
                Debug.Log( "Pixels array was successfully serialized to " + path );
                #endif

                ViveInteractionsManager.Instance.GetComponent<AudioSource>().PlayOneShot( ProjectData.Instance.Save_sketch_clip );
            }
        }

        // Loads painting from disk ######################################################################################################################################################################################################################
        public void LoadSketch( bool load_for_group ) { 

            CanvasDialogGeoLoadSketch.Instance.Hide();
            
            string path = GetSketchFilePath();

            if( !File.Exists( path ) || string.IsNullOrEmpty( path ) ) { 
                
                #if( UNITY_EDITOR )
                Debug.LogError( "Cannot locate sketch file " + path );
                #endif

                return;
            }

            PaintingPointFeatures[] painting_point_features = null;

            bool is_deserialization_error = false;

            using( FileStream file_stream = File.OpenRead( path ) ) {

                BinaryFormatter formatter = new BinaryFormatter();

                try {
                
                    painting_point_features = (PaintingPointFeatures[]) formatter.Deserialize( file_stream );
                }

                catch( Exception exception ) { 

                    is_deserialization_error = true;

                    #if( UNITY_EDITOR )
                    Debug.LogError( "Cannot deserialize pixels array from " + path + ": " + exception.Message );
                    #endif
                }
            }

            if( is_deserialization_error ) { 

                return;
            }

            else if( ( painting_point_features == null) || (painting_point_features.Length == 0) ) { 

                #if( UNITY_EDITOR )
                Debug.LogError( "Deserialized pixels array from " + path + " is empty!" );
                #endif

                return;
            }

            else { 
            
                #if( UNITY_EDITOR )
                Debug.Log( "Pixels array was successfully deserialized from " + path );
                #endif

                ViveInteractionsManager.Instance.GetComponent<AudioSource>().PlayOneShot( ProjectData.Instance.Load_sketch_clip );
            }
            
            ClearAllPixels();

            for( int i = 0; i < painting_point_features.Length; i++ ) { 
                
                if( painting_point_features[i].Color_index > 0 ) {

                    if( current_paint.Painting_color != (PaintingColor) painting_point_features[i].Color_index ) {
                    
                        current_paint = GetPaint( (PaintingColor) painting_point_features[i].Color_index );
                    }

                    Vector3 point_position = new Vector3( painting_point_features[i].Point_position_x, painting_point_features[i].Point_position_y, painting_point_features[i].Point_position_z );

                    PaintingPoint painting_point = InstantiatePoint( point_position, painting_group_local );

                    #if( UNITY_EDITOR )
                    //Debug.Log( "Loading local sketch point #" + painting_point.Point_ID + " with color " + ((PaintingColor) painting_point_features[i].Color_index).ToString() + "; current_paint.Painting_color: " + current_paint.Painting_color );
                    #endif
                }
            }

            if( load_for_group ) { 
            
                if( (VertexStudio.Networking.NetworkPlayer.Instance != null) && (NetworkProjectManager.Instance != null) && (NetworkTransactionsControlPainting.Instance != null ) ) { 

                    NetworkTransactionsControlPainting.Instance.ClearAllPixels( VertexStudio.Networking.NetworkPlayer.Instance.View_ID );
                    
                    StartCoroutine( LoadSketchGroup( painting_point_features ) );
                }
            }
        }

        // Loads painting from disk for the group ########################################################################################################################################################################################################
        private IEnumerator LoadSketchGroup( PaintingPointFeatures[] painting_point_features ) { 
        
            int current_point_index = 0;

            while( current_point_index < painting_point_features.Length ) { 
            
                for( int i = 0; i < remote_points_sending_quantity; i++ ) { 
                
                    int current_index = i + current_point_index;

                    if( current_index >= painting_point_features.Length ) { 
                    
                        break;
                    }

                    #if( UNITY_EDITOR )
                    //Debug.Log( "Loading (sending) remote sketch point #" + current_index + " with color " + ((PaintingColor) painting_point_features[ current_index ].Color_index).ToString() );
                    #endif

                    Vector3 point_position = new Vector3( 
                        
                        painting_point_features[ current_index ].Point_position_x, 
                        painting_point_features[ current_index ].Point_position_y, 
                        painting_point_features[ current_index ].Point_position_z 
                    );

                    NetworkTransactionsControlPainting.Instance.InstantiatePoint( 
                        
                        VertexStudio.Networking.NetworkPlayer.Instance.View_ID, 
                        point_position, 
                        painting_point_features[ current_index ].Color_index 
                    );
                }

                current_point_index += remote_points_sending_quantity;

                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        // Returns full sketch file path depending on scene name #########################################################################################################################################################################################
        private string GetSketchFilePath() { 
            
            string path = string.Empty;

            path = Path.Combine( Application.persistentDataPath, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + " Sketch.geo" );

            return path;
        }

        // Returns painting pixels as array ##############################################################################################################################################################################################################
        private PaintingPointFeatures[] GetPaintingPointFeatures() {

            if( painting_group_local.transform.childCount == 0 ) { 
            
                return null;
            }

            Transform points_group_parent = painting_group_local.transform;

            PaintingPoint[] painting_points = points_group_parent.GetComponentsInChildren<PaintingPoint>();

            if( (painting_points == null) || (painting_points.Length == 0) ) { 
            
                return null;
            }

            PaintingPointFeatures[] painting_point_features = new PaintingPointFeatures[ painting_points.Length ];

            for( int i = 0; i < painting_points.Length; i++ ) { 

                Transform point_transform = painting_points[i].Point_transform;
                
                painting_point_features[i].Point_position_x = point_transform.position.x;
                painting_point_features[i].Point_position_y = point_transform.position.y;
                painting_point_features[i].Point_position_z = point_transform.position.z;

                painting_point_features[i].Color_index = (int) painting_points[i].Point_color;
            }

            return painting_point_features;
        }

        // Returns paint by its color name ###############################################################################################################################################################################################################
        public Paint GetPaint( PaintingColor painting_color ) { 
        
            Paint paint = null;

            for( int i = 0; i < paints.Length; i++ ) { 
            
                if( paints[i].Painting_color == painting_color ) { 
                
                    paint = paints[i];

                    break;
                }
            }

            return paint;
        }

        // Returns material by color name ################################################################################################################################################################################################################
        public Material GetMaterial( PaintingColor painting_color ) { 
        
            Material material = null;

            for( int i = 0; i < paints.Length; i++ ) { 
            
                if( paints[i].Painting_color == painting_color ) { 
                
                    material = paints[i].Painting_material;

                    break;
                }
            }

            return material;
        }
    }
}