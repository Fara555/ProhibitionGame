using System.Collections;
using UnityEngine;

public class AiStunned : AiState
{
    private bool stunned;
    private Coroutine stunCoroutine;

    public void Enter(AiAgent agent)
    {
        stunCoroutine = agent.StartCoroutine(Stunned(agent));
        Debug.Log("enter stun");
    }

    public void Exit(AiAgent agent)
    {
        agent.StopCoroutine(stunCoroutine);
    }

    public AiStateId GetId()
    {
        return AiStateId.Stunned;
    }

    public void Update(AiAgent agent)
    {
        if (!stunned)
        {
            if (agent.backSensor.IsInSight(agent.playerTransform.gameObject)) agent.stateMachine.ChangeState(AiStateId.Attack);
            if (agent.mainSensor.IsInSight(agent.playerTransform.gameObject)) agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
            else agent.stateMachine.ChangeState(AiStateId.Patrol);
        }
    }
   
    private IEnumerator Stunned(AiAgent agent)
    {
        agent.animator.SetBool("Stunned", true);
        stunned = true;
        yield return new WaitForSeconds(1.3f);
        agent.animator.SetBool("Stunned", false);
        yield return new WaitForSeconds(agent.config.stunTime);
        stunned = false;
    }
}