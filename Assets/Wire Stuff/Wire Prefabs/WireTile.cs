using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class WireTile : RuleTile<WireTile.Neighbor> {
    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Any = 1;
        public const int None = 2;
        public const int Wire = 3;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) 
    {
        switch (neighbor)
        {
            case Neighbor.None:
                return tile == null;
            case Neighbor.Any: 
                return tile != null;
            case Neighbor.Wire: 
                return tile == this;
        }
        return base.RuleMatch(neighbor, tile);
    }
}