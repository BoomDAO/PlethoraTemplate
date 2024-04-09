#region Using statements

using UnityEngine;

#endregion

public class WaterVolumeTransforms : WaterVolumeBase
{
    #region MonoBehaviour events

    private void OnTransformChildrenChanged()
    {
        Rebuild();
    }

    #endregion

    #region Public methods

    protected override void GenerateTiles(ref bool[,,] _tiles)
    {
        // iterate the chldren
        for (var i = 0; i < transform.childCount; i++)
        {
            // grab the local position/scale
            var pos = transform.GetChild(i).localPosition;
            var sca = transform.GetChild(i).localScale / TileSize;

            // fix to the grid
            var x = Mathf.RoundToInt(pos.x / TileSize);
            var y = Mathf.RoundToInt(pos.y / TileSize);
            var z = Mathf.RoundToInt(pos.z / TileSize);

            // iterate the size of the transform
            for (var ix = x; ix < x + Mathf.RoundToInt(sca.x); ix++)
            {
                for (var iy = y; iy < y + Mathf.RoundToInt(sca.y); iy++)
                {
                    for (var iz = z; iz < z + Mathf.RoundToInt(sca.z); iz++)
                    {
                        // validate
                        if (ix < 0 || ix >= MAX_TILES_X || iy < 0 | iy >= MAX_TILES_Y || iz < 0 || iz >= MAX_TILES_Z)
                        {
                            continue;
                        }

                        // add the tile
                        _tiles[ix, iy, iz] = true;
                    }
                }
            }
        }
    }

    #endregion
}