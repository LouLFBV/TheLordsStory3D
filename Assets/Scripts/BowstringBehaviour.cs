using UnityEngine;

public class BowstringBehaviour : MonoBehaviour
{
    [Header("Bowstring settings")]
    public Transform bowTop;
    public Transform bowBottom;
    public Transform stringHandPoint;

    [HideInInspector ]public LineRenderer line;
    private void Start()
    {
        line = GetComponent<LineRenderer>();
        if (line != null)
        {
            line.positionCount = 3;
            line.startWidth = 0.01f;
            line.endWidth = 0.01f;
            line.useWorldSpace = true;
        }
    }

    private void OnEnable()
    {
        BowBehaviour.instance.bowstringBehaviour = this;
    }

    private void OnDisable()
    {
        BowBehaviour.instance.bowstringBehaviour = null;
    }
}
