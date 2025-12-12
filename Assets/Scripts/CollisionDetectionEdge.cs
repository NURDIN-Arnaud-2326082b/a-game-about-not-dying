using UnityEngine;
using System.Linq;


public class CollisionDetectionEdge : MonoBehaviour
{
    [SerializeField]
    private float radius;

    private Collider[] hitColliders;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private PointDetectionEdge[] detectionPoints;

    public MeshRenderer meshRenderer;

    public bool CheckConnection()
    {

        hitColliders = Physics.OverlapSphere(transform.position + offset, radius);
        if (hitColliders.Length > 0)
        {
            if (hitColliders.Any(col => col.CompareTag(transform.tag)))
            {
                return false;
            }
            else if (hitColliders.Any(col =>col.CompareTag("ground")))
            {
                return true;
            }
        }
        
        foreach (PointDetectionEdge point in detectionPoints)
        {
            point.CheckOverlap();
            if (point.connected)
            {
                return true;
            }
        }
        return false;
    }
}
