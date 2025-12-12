using UnityEngine;

public class PointDetectionEdge : MonoBehaviour
{
    public bool connected;
    public float radius = 0.6f;

    public Collider[] hitColliders;

    //méthode  appelée quand l'objet est désactivé
    private void OnDisable()
    {
        connected = false;
    }

    public void CheckOverlap()
    {
        connected = false;
        hitColliders = Physics.OverlapSphere(transform.position, radius);
        if (hitColliders.Length > 0)
        {
            foreach (Collider col in hitColliders)
            {
                if (col.gameObject != this.gameObject && !col.isTrigger)
                {
                    connected = true;
                    break;
                }
            }  
        } 
    }
        
    private void OnDrawGizmos()
    {
        Gizmos.color = connected ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
