using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace VostokVR.Geo {

    public class GenerateMeshFromNavMesh : MonoBehaviour {

	    // Use this for initialization #########################################################################################################################################################################################################################
	    void Start() {
		
	    }

        // Creates the mesh based on NavMesh ###################################################################################################################################################################################################################
        public static void GenerateMesh() {

             NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
             Mesh mesh = new Mesh();
             mesh.vertices = triangles.vertices;
             mesh.triangles = triangles.indices;
        }

        // Exprots NavMesh to Mesh #############################################################################################################################################################################################################################
        // [MenuItem("Tools/Export NavMesh to mesh")]
        static void ExportNavMeshToMesh() {

            NavMeshTriangulation triangulatedNavMesh = NavMesh.CalculateTriangulation();
 
            Mesh mesh = new Mesh();

            mesh.name = "ExportedNavMesh";
            mesh.vertices = triangulatedNavMesh.vertices;
            mesh.triangles = triangulatedNavMesh.indices;

            #if UNITY_EDITOR
            string filename = Application.dataPath +"/" + Path.GetFileNameWithoutExtension( EditorSceneManager.GetActiveScene().name ) + " Exported NavMesh.obj";
            #else
            string filename = Application.dataPath +"/" + Path.GetFileNameWithoutExtension( SceneManager.GetActiveScene().name ) + " Exported NavMesh.obj";
            #endif

            MeshToFile( mesh, filename );

            print( "NavMesh exported as '" + filename + "'" );

            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
        }
 
        // #####################################################################################################################################################################################################################################################
        private static string MeshToString( Mesh mesh ) {

            StringBuilder sb = new StringBuilder();
 
            sb.Append( "g " ).Append(mesh.name).Append( "\n" );

            foreach (Vector3 v in mesh.vertices) {

                sb.Append(string.Format("v {0} {1} {2}\n",v.x,v.y,v.z));
            }

            sb.Append("\n");

            foreach (Vector3 v in mesh.normals) {

                sb.Append(string.Format("vn {0} {1} {2}\n",v.x,v.y,v.z));
            }

            sb.Append("\n");

            foreach (Vector3 v in mesh.uv) {

                sb.Append(string.Format("vt {0} {1}\n",v.x,v.y));
            }

            for (int material = 0; material < mesh.subMeshCount; material++) {

                sb.Append("\n");
 
                int[] triangles = mesh.GetTriangles(material);

                for (int i=0;i<triangles.Length;i+=3) {

                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[i]+1, triangles[i+1]+1, triangles[i+2]+1));
                }
            }

            return sb.ToString();
        }
 
        // #####################################################################################################################################################################################################################################################
        private static void MeshToFile( Mesh mesh, string file_name ) {

            using( StreamWriter sw = new StreamWriter( file_name ) ) {

                sw.Write( MeshToString( mesh ) );
            }
        }
    }
}