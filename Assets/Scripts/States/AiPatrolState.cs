using Hearing;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AiPatrolState : AiState
{
    private bool isPatrolling;
    private Coroutine patrolCoroutine;
    private Vector3 lastPatrolPoint;

    public void Enter(AiAgent agent)
    {
        agent.isListening = true;
        agent.animator.SetBool("Patroling", true);
        agent.navMeshAgent.speed = agent.config.patrolSpeed;
        lastPatrolPoint = agent.transform.position;  // Initialize the last patrol point
        isPatrolling = true;
        patrolCoroutine = agent.StartCoroutine(Patrol(agent));
    }

    public void Exit(AiAgent agent)
    {
        agent.isListening = false;  
        if (patrolCoroutine != null) agent.StopCoroutine(patrolCoroutine);
        agent.animator.SetBool("Patroling", false);
    }

    public AiStateId GetId()
    {
        return AiStateId.Patrol;
    }

    public void Update(AiAgent agent)
    {
        if (agent.mainSensor.IsInSight(agent.playerTransform.gameObject)) agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
        if (agent.backSensor.IsInSight(agent.playerTransform.gameObject)) agent.stateMachine.ChangeState(AiStateId.Attack);
    }

    #region PatrolSettings

    IEnumerator Patrol(AiAgent agent)
    {
        while (isPatrolling)
        {
            Vector3 patrolPoint = ChoosePatrolPoint(agent.playerTransform.position, agent.config.patrolRadius, lastPatrolPoint);
            lastPatrolPoint = patrolPoint;  // Update last patrol point
            agent.navMeshAgent.SetDestination(patrolPoint);

            while (!HasReachedDestination(agent))
            {
                yield return null;
            }

            yield return new WaitForSeconds(agent.config.patrolWaitTime);
        }
    }

    Vector3 ChoosePatrolPoint(Vector3 center, float radius, Vector3 lastPoint)
    {
        Vector3 directionToLastPoint = (lastPoint - center).normalized;
        Vector3 newDirection = Quaternion.Euler(0, 180, 0) * directionToLastPoint;  // Compute the opposite direction
        Vector3 randomPointInOppositeDirection = center + newDirection * radius;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPointInOppositeDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return center;  // Return center if no valid point is found
    }

    bool HasReachedDestination(AiAgent agent)
    {
        return agent.navMeshAgent.remainingDistance <= agent.navMeshAgent.stoppingDistance && !agent.navMeshAgent.pathPending;
    }


    #endregion
}
