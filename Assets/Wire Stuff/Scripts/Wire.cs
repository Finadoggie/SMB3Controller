using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static PresetContactFilters;

public class Wire : MonoBehaviour
{
    public enum State { off, on }

    [SerializeField] private Wire[] connectedWires;
    [SerializeField] private Collider2D[] colliders;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public State myState;

    [SerializeField] private Collider2D[] touchedColliders;

    public static Color[] wireColors = {
        new Color(0x88 / 255f, 0f, 0f),
        new Color(1f, 0.5f, 0.5f)
    };

    public static List<Wire> wireList;
    public static List<LinkedList<Wire>> wireGroups;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        colliders = GetComponents<Collider2D>();
        connectedWires = new Wire[colliders.Length];

        touchedColliders = new Collider2D[colliders.Length];

        #region Get Connected Wires

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].OverlapCollider(WIRE_FILTER, touchedColliders);

            connectedWires[i] = touchedColliders[0].gameObject.GetComponent<Wire>();
        }

        #endregion

        wireList.Add(this);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void GroupWires()
    {
        Debug.Log(wireList.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        #region Set color

        spriteRenderer.color = wireColors[(int)myState];

        #endregion

        
    }

    public void SetState(State state, Wire origin = null)
    {
        myState = state;
    }
}
