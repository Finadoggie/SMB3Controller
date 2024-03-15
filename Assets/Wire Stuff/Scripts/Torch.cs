using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PresetContactFilters;

public class Torch : MonoBehaviour
{
    [SerializeField] private int children;

    [SerializeField] private Wire[] connectedWires;

    public Wire.State state;

    // Start is called before the first frame update
    void Awake()
    {
        connectedWires = transform.parent.GetComponentsInChildren<Wire>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var child in connectedWires)
        {
            child.SetState(state);
        }
    }
}
