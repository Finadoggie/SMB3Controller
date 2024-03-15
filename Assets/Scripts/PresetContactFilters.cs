using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetContactFilters
{
    public static ContactFilter2D NO_FILTER, WIRE_FILTER;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void SetFilters()
    {
        NO_FILTER.NoFilter();

        WIRE_FILTER.SetLayerMask(LayerMask.GetMask("Wire"));
        WIRE_FILTER.useLayerMask = true;
        WIRE_FILTER.useTriggers = true;
    }
}