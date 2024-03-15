using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WireManager : MonoBehaviour
{
    [SerializeField] private Tilemap wireMap;

    [SerializeField] private TileBase[] tiles;

    private void Start()
    {
        tiles = wireMap.GetTilesBlock(wireMap.cellBounds);
    }
}
