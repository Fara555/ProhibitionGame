using System.Collections;
using UnityEngine;

public class AiLaughingState : AiState
{
    private Coroutine laughCoroutine;

    public void Enter(AiAgent agent)
    {
        laughCoroutine = agent.StartCoroutine(Laugh(agent));
    }

    public void Exit(AiAgent agent)
    {
        agent.StopCoroutine(laughCoroutine);
    }

    public AiStateId GetId()
    {
        return AiStateId.Laugh;
    }

    public void Update(AiAgent agent)
    {

    }

    private IEnumerator Laugh(AiAgent agent)
    {
        yield return new WaitForSeconds(1.5f); // Time for player to 'recover' after enemy attack;
        ResetPlayerControl(agent); // Give player abilities back
        agent.animator.SetBool("Laughing", true);
        yield return new WaitForSeconds(agent.config.laughTime); // Duration of the laughing animation
        agent.animator.SetBool("Laughing", false);

        // Check if player still close enough for another attack
        if (agent.attackSensor.IsInSight(agent.playerTransform.gameObject) || agent.backSensor.IsInSight(agent.playerTransform.gameObject)) agent.stateMachine.ChangeState(AiStateId.Attack);
        else if (agent.mainSensor.IsInSight(agent.playerTransform.gameObject)) agent.stateMachine.ChangeState(AiStateId.ChasePlayer); // Then check if player close enough to immediately start chasing 
        else agent.stateMachine.ChangeState(AiStateId.Patrol); // If not, start patroling
    }

    private void ResetPlayerControl(AiAgent agent)
    {
        //Reset player ability to move and turn camera around;
        agent.playerCamera.enabled = true;
        agent.playerMovement.enabled = true;
        agent.playerFootStepSystem.enabled = true;
        agent.navMeshAgent.stoppingDistance = 5f; //Reset player stop distance to normal after attack;

    }
}
