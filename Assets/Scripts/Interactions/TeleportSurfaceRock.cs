using System.Collections;
using UnityEngine;
using VertexStudio.VirtualRealty;
using VertexStudio.Generic;

namespace VostokVR.Geo {

    public class TeleportSurfaceRock : TeleportSurface {

        [Space(10), SerializeField]
        private Transform root_sarch_transform;

        [SerializeField]
        private MeshRenderer mesh_renderer;
        public MeshRenderer Mesh_renderer { get { return mesh_renderer; } }

        [SerializeField]
        private MaterialControl material_control;
        public MaterialControl Material_control { get { return material_control; } }

        protected void Awake() {

            //StartCoroutine( CreateCollider() );
        }

        protected override void Start() {

            //mesh_renderer = GetComponent<MeshRenderer>();
        }

        public override void ShowTeleportGrid() { 

            //mesh_renderer.enabled = true;
        }

        public override void HideTeleportGrid() { 

            //mesh_renderer.enabled = false;
        }

        private IEnumerator CreateCollider() {

            yield return null;

            yield return null;

            yield return null;

            yield return null;

            yield return null;

            MeshCollider mesh_collider = gameObject.AddComponent<MeshCollider>();

            GetComponent<MeshCollider>().sharedMesh = mesh_renderer.GetComponent<MeshFilter>().mesh;

            yield break;
        }

        #if (UNITY_EDITOR)
        [ContextMenu( "Fill corresponding MeshRenderer" )]
        private void FillCorrespondingMeshRenderer() { 
            
            for( int i = 0; i < root_sarch_transform.childCount; i++ ) { 
            
                if( root_sarch_transform.GetChild( i ).GetComponent<MeshFilter>().sharedMesh.name == GetComponent<MeshCollider>().sharedMesh.name ) { 
                 
                    mesh_renderer = root_sarch_transform.GetChild(i).GetComponent<MeshRenderer>();
                    material_control = mesh_renderer.GetComponent<MaterialControl>();

                    ScenesManager.MarkCurrentSceneDirty();

                    break;
                }
            }
        }
        #endif
    }
}