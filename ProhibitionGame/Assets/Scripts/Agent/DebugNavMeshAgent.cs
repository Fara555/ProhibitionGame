using UnityEngine;
using UnityEngine.AI;

public class DebugNavMeshAgent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private AiAgent agentAI;
    [SerializeField] private Transform player;
    [SerializeField] private bool velocity;
    [SerializeField] private bool desiredVelocity;
    [SerializeField] private bool path;
    [SerializeField] private bool patrolZone;

    private void OnDrawGizmos()
    {
        if (velocity) //visualize current velocity of the entity
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + agent.velocity);
        }
        if (desiredVelocity) //visualize desired velocity of the entity
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + agent.desiredVelocity);
        }

        if (path) //visualize navmesh path of the entity
        {
            Gizmos.color = Color.red;   
            var agentPath = agent.path;
            Vector3 prevCorner = transform.position;
            foreach(var corner in agentPath.corners)
            {
                Gizmos.DrawLine(prevCorner, corner);
                Gizmos.DrawSphere(corner, 0.1f);
                prevCorner = corner;    
            }
        }
        
        if (patrolZone) // visualize zone where entity should be most of the time
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(player.position,agentAI.config.patrolRadius);
        }
    }
}
