using System.Collections;
using UnityEngine;

public class AiChasePlayerState : AiState
{
    private float timer = 0f;
    private Coroutine lostPlayerCoroutine;

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.speed = agent.config.chaseSpeed; // Increase speed when agent chasing
    }

    public void Exit(AiAgent agent)
    {
        if (lostPlayerCoroutine != null) agent.StopCoroutine(lostPlayerCoroutine); // Stop coroutine on exit of a state
    }

    public AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public void Update(AiAgent agent)
    {

        if (agent.attackSensor.IsInSight(agent.playerTransform.gameObject)) agent.stateMachine.ChangeState(AiStateId.Attack); // change to the attack state when player close enough

        if (!agent.mainSensor.IsInSight(agent.playerTransform.gameObject)) // If player leave agent sight zone, chase him a few more second before completly lost him
        {
            if (lostPlayerCoroutine == null) lostPlayerCoroutine = agent.StartCoroutine(LostPlayer(agent));
        }
        else
        {
            if (lostPlayerCoroutine != null) // Stop lost coroutine if player again enter sight zone 
            {
                agent.StopCoroutine(lostPlayerCoroutine);
                lostPlayerCoroutine = null;
            }
        }

        timer -= Time.deltaTime;
        if (!agent.navMeshAgent.hasPath) agent.navMeshAgent.destination = agent.playerTransform.position; // Set destination to player if agent doesnt have path

        //Update path every few milisecond instead of every frame, and when player move, for optimization purpose
        if (timer < 0f)
        {
            float distance = (agent.playerTransform.position - agent.navMeshAgent.destination).magnitude;
            if (distance > agent.config.maxDistance)
            {
                agent.navMeshAgent.destination = agent.playerTransform.position;
            }
            timer = agent.config.maxTime;
        }
    }

    private IEnumerator LostPlayer(AiAgent agent) // Wait few second before change to the patrol state
    {
        yield return new WaitForSeconds(agent.config.chaseTime);
        agent.stateMachine.ChangeState(AiStateId.Patrol);
    }
}