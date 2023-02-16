using UnityEngine;
using VertexStudio.VirtualRealty;

namespace VostokVR.Geo {

    public class TeleportSurfaceSimple : TeleportSurface {

        protected override void Start() {

            //mesh_renderer = GetComponent<MeshRenderer>();
        }

        public override void ShowTeleportGrid() { 

            //mesh_renderer.enabled = true;
        }

        public override void HideTeleportGrid() { 

            //mesh_renderer.enabled = false;
        }
    }
}