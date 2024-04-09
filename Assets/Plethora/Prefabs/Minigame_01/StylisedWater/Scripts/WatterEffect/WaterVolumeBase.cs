#region Using statements

using System.Collections.Generic;
using UnityEngine;

#endregion

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class WaterVolumeBase : MonoBehaviour
{
    #region Constants

    public const int MAX_TILES_X = 100;
    public const int MAX_TILES_Y = 50;
    public const int MAX_TILES_Z = 100;

    #endregion

    #region Flag lists

    [System.Flags]
    public enum TileFace : int
    {
        NegX = 1,
        PosX = 2,
        NegZ = 4,
        PosZ = 8
    }

    #endregion

    #region Private fields

    protected bool isDirty = true;

    private Mesh mesh = null;
    private MeshFilter meshFilter = null;

    private bool[,,] tiles = null;

    #endregion

    #region Public fields

    [Range(0.1f, 100f)]
    public float TileSize = 1f;

    #endregion

    #region Private methods

    private void EnsureReferences()
    {
        if (meshFilter == null)
        {
            mesh = null;
            meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }
        }

        if (mesh == null)
        {
            mesh = meshFilter.sharedMesh;
            if (mesh == null || mesh.name != "WaterVolume-" + gameObject.GetInstanceID())
            {
                mesh = new UnityEngine.Mesh();
                mesh.name = "WaterVolume-" + gameObject.GetInstanceID();
            }
        }

        meshFilter.sharedMesh = mesh;
    }

    #endregion

    #region Public methods

    public float? GetHeight(Vector3 _position)
    {
        var x = Mathf.FloorToInt((_position.x - transform.position.x + 0.5f) / TileSize);
        var z = Mathf.FloorToInt((_position.z - transform.position.z + 0.5f) / TileSize);

        if (x < 0 || x >= MAX_TILES_X || z < 0 || z >= MAX_TILES_Z)
        {
            return null;
        }

        for (var y = MAX_TILES_Y - 1; y >= 0; y--)
        {
            if (tiles[x, y, z])
            {
                return transform.position.y + y * TileSize;
            }
        }

        return null;
    }

    public void Rebuild()
    {
        Debug.Log("rebuilding water volume \"" + gameObject.name + "\"");

        EnsureReferences();

        mesh.Clear();

        tiles = new bool[MAX_TILES_X, MAX_TILES_Y, MAX_TILES_Z];
        GenerateTiles(ref tiles);

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();
        var colors = new List<Color>();
        var indices = new List<int>();

        for (var x = 0; x < MAX_TILES_X; x++)
        {
            for (var y = 0; y < MAX_TILES_Y; y++)
            {
                for (var z = 0; z < MAX_TILES_Z; z++)
                {
                    if (!tiles[x, y, z])
                    {
                        continue;
                    }

                    var x0 = x * TileSize - 0.5f;
                    var x1 = x0 + TileSize;
                    var y0 = y * TileSize - 0.5f;
                    var y1 = y0 + TileSize;
                    var z0 = z * TileSize - 0.5f;
                    var z1 = z0 + TileSize;
                    var ux0 = x0 + transform.position.x;
                    var ux1 = x1 + transform.position.x;
                    var uy0 = y0 + transform.position.y;
                    var uy1 = y1 + transform.position.y;
                    var uz0 = z0 + transform.position.z;
                    var uz1 = z1 + transform.position.z;

                    var posY = y == MAX_TILES_Y - 1 || !tiles[x, y + 1, z];
                    var faceNegX = false;
                    var facePosX = false;
                    var faceNegZ = false;
                    var facePosZ = false;
                    var foamNegX = false;
                    var foamPosX = false;
                    var foamNegZ = false;
                    var foamPosZ = false;

                    if (y == MAX_TILES_Y - 1 || !tiles[x, y + 1, z])
                    {
                        vertices.Add(new Vector3(x0, y1, z0));
                        vertices.Add(new Vector3(x0, y1, z1));
                        vertices.Add(new Vector3(x1, y1, z1));
                        vertices.Add(new Vector3(x1, y1, z0));
                        normals.Add(new Vector3(0, 1, 0));
                        normals.Add(new Vector3(0, 1, 0));
                        normals.Add(new Vector3(0, 1, 0));
                        normals.Add(new Vector3(0, 1, 0));
                        uvs.Add(new Vector2(ux0, uz0));
                        uvs.Add(new Vector2(ux0, uz1));
                        uvs.Add(new Vector2(ux1, uz1));
                        uvs.Add(new Vector2(ux1, uz0));
                        colors.Add(Color.black);
                        colors.Add(Color.black);
                        colors.Add(Color.black);
                        colors.Add(Color.black);
                        var v = vertices.Count - 4;
                        if (foamNegX && foamPosZ || foamPosX && foamNegZ)
                        {
                            indices.Add(v + 1);
                            indices.Add(v + 2);
                            indices.Add(v + 3);
                            indices.Add(v + 3);
                            indices.Add(v);
                            indices.Add(v + 1);
                        }
                        else
                        {
                            indices.Add(v);
                            indices.Add(v + 1);
                            indices.Add(v + 2);
                            indices.Add(v + 2);
                            indices.Add(v + 3);
                            indices.Add(v);
                        }
                    }

                    if (faceNegX)
                    {
                        vertices.Add(new Vector3(x0, y0, z1));
                        vertices.Add(new Vector3(x0, y1, z1));
                        vertices.Add(new Vector3(x0, y1, z0));
                        vertices.Add(new Vector3(x0, y0, z0));
                        normals.Add(new Vector3(-1, 0, 0));
                        normals.Add(new Vector3(-1, 0, 0));
                        normals.Add(new Vector3(-1, 0, 0));
                        normals.Add(new Vector3(-1, 0, 0));
                        uvs.Add(new Vector2(uz1, uy0));
                        uvs.Add(new Vector2(uz1, uy1));
                        uvs.Add(new Vector2(uz0, uy1));
                        uvs.Add(new Vector2(uz0, uy0));
                        colors.Add(Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(Color.black);
                        var v = vertices.Count - 4;
                        indices.Add(v);
                        indices.Add(v + 1);
                        indices.Add(v + 2);
                        indices.Add(v + 2);
                        indices.Add(v + 3);
                        indices.Add(v);
                    }
                    if (facePosX)
                    {
                        vertices.Add(new Vector3(x1, y0, z0));
                        vertices.Add(new Vector3(x1, y1, z0));
                        vertices.Add(new Vector3(x1, y1, z1));
                        vertices.Add(new Vector3(x1, y0, z1));
                        normals.Add(new Vector3(1, 0, 0));
                        normals.Add(new Vector3(1, 0, 0));
                        normals.Add(new Vector3(1, 0, 0));
                        normals.Add(new Vector3(1, 0, 0));
                        uvs.Add(new Vector2(uz0, uy0));
                        uvs.Add(new Vector2(uz0, uy1));
                        uvs.Add(new Vector2(uz1, uy1));
                        uvs.Add(new Vector2(uz1, uy0));
                        colors.Add(Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(Color.black);
                        var v = vertices.Count - 4;
                        indices.Add(v);
                        indices.Add(v + 1);
                        indices.Add(v + 2);
                        indices.Add(v + 2);
                        indices.Add(v + 3);
                        indices.Add(v);
                    }
                    if (faceNegZ)
                    {
                        vertices.Add(new Vector3(x0, y0, z0));
                        vertices.Add(new Vector3(x0, y1, z0));
                        vertices.Add(new Vector3(x1, y1, z0));
                        vertices.Add(new Vector3(x1, y0, z0));
                        normals.Add(new Vector3(0, 0, -1));
                        normals.Add(new Vector3(0, 0, -1));
                        normals.Add(new Vector3(0, 0, -1));
                        normals.Add(new Vector3(0, 0, -1));
                        uvs.Add(new Vector2(ux0, uy0));
                        uvs.Add(new Vector2(ux0, uy1));
                        uvs.Add(new Vector2(ux1, uy1));
                        uvs.Add(new Vector2(ux1, uy0));
                        colors.Add(Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(Color.black);
                        var v = vertices.Count - 4;
                        indices.Add(v);
                        indices.Add(v + 1);
                        indices.Add(v + 2);
                        indices.Add(v + 2);
                        indices.Add(v + 3);
                        indices.Add(v);
                    }
                    if (facePosZ)
                    {
                        vertices.Add(new Vector3(x1, y0, z1));
                        vertices.Add(new Vector3(x1, y1, z1));
                        vertices.Add(new Vector3(x0, y1, z1));
                        vertices.Add(new Vector3(x0, y0, z1));
                        normals.Add(new Vector3(0, 0, 1));
                        normals.Add(new Vector3(0, 0, 1));
                        normals.Add(new Vector3(0, 0, 1));
                        normals.Add(new Vector3(0, 0, 1));
                        uvs.Add(new Vector2(ux1, uy0));
                        uvs.Add(new Vector2(ux1, uy1));
                        uvs.Add(new Vector2(ux0, uy1));
                        uvs.Add(new Vector2(ux0, uy0));
                        colors.Add(Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(Color.black);
                        var v = vertices.Count - 4;
                        indices.Add(v);
                        indices.Add(v + 1);
                        indices.Add(v + 2);
                        indices.Add(v + 2);
                        indices.Add(v + 3);
                        indices.Add(v);
                    }
                }
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetColors(colors);
        mesh.SetTriangles(indices, 0);

        mesh.RecalculateBounds();

        mesh.RecalculateTangents();

        meshFilter.sharedMesh = mesh;

        isDirty = false;
    }

    #endregion

    #region Virtual methods

    protected virtual void GenerateTiles(ref bool[,,] _tiles) { }
    public virtual void Validate() { }

    #endregion

    #region MonoBehaviour events

    void OnValidate()
    {

        TileSize = Mathf.Clamp(TileSize, 0.1f, 100f);

        Validate();

        isDirty = true;
    }

    void Update()
    {
        if (isDirty || (!Application.isPlaying && false))
        {
            Rebuild();
        }
    }

    #endregion
}