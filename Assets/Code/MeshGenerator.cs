using System.Collections.Generic;
using UnityEngine;

namespace Battleplan
{
    public class MeshGenerator : MonoBehaviour
    {
        public Texture2D ProvinceMap;
        public GameObject ProvinceMesh;

        public void Start()
        {
            GenerateQuadMap();
        }

        private void GenerateSmoothMap()
        {

        }

        private void GenerateQuadMap()
        {
            var width = ProvinceMap.width;
            var height = ProvinceMap.height;

            var provinceMeshes = new Dictionary<string, (GameObject ProvinceMesh, Color32 Color)>();

            for (var y = 0; y < 400; y++)
            {
                var x = 0;
                var centerY = y + 0.5f;

                while (x < width)
                {
                    var color = (Color32)ProvinceMap.GetPixel(x, y);
                    var colorHex = ColorUtility.ToHtmlStringRGB(color);
                    var voxelWidth = 0;
                    var startX = x;

                    while (x < width && AreEqual(color, ProvinceMap.GetPixel(x, y)))
                    {
                        voxelWidth++;
                        x++;
                    }

                    if (!provinceMeshes.TryGetValue(colorHex, out var pair))
                    {
                        var provinceMesh = Instantiate(ProvinceMesh);
                        provinceMesh.transform.SetParent(transform);
                        provinceMesh.name = $"R{color.r}G{color.g}B{color.b}";
                        provinceMesh.GetComponent<Renderer>().material.color = color;

                        pair = (provinceMesh, color);
                        provinceMeshes[colorHex] = pair;
                    }

                    AggregateQuadMesh(pair.ProvinceMesh, startX, centerY, voxelWidth);
                }
            }

            foreach (var pair in provinceMeshes.Values)
            {
                pair.ProvinceMesh.GetComponent<MeshCollider>().sharedMesh = pair.ProvinceMesh.GetComponent<MeshFilter>().mesh;
            }
        }

        private static void AggregateQuadMesh(GameObject province, float startX, float centerY, float width)
        {
            var matrix = Matrix4x4.TRS(new Vector3(startX, centerY, 0), province.transform.rotation, new Vector3(1, 1, 1));
            var combine = new CombineInstance[2];
            var meshFilter = province.GetComponent<MeshFilter>();

            combine[0].mesh = GetQuadMesh(width, 1);
            combine[0].transform = matrix;
            combine[1].mesh = meshFilter.mesh;
            combine[1].transform = province.transform.localToWorldMatrix;

            meshFilter.mesh = new Mesh();
            meshFilter.mesh.CombineMeshes(combine);
        }

        private static Mesh GetQuadMesh(float width, float height)
        {
            var mesh = new Mesh();
            var vertices = new Vector3[4];
     
            vertices[0] = new Vector3(0, 0, 0);
            vertices[1] = new Vector3(width, 0, 0);
            vertices[2] = new Vector3(0, height, 0);
            vertices[3] = new Vector3(width, height, 0);
     
            mesh.vertices = vertices;
     
            var tri = new int[6];
 
            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;
     
            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;
     
            mesh.triangles = tri;
     
            var normals = new Vector3[4];
     
            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;
     
            mesh.normals = normals;
     
            var uv = new Vector2[4];
 
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            return mesh;
        }

        private static bool AreEqual(Color32 a, Color32 b)
        {
            return Mathf.Approximately(a.r, b.r)
                && Mathf.Approximately(a.g, b.g)
                && Mathf.Approximately(a.b, b.b);
        }
    }
}
