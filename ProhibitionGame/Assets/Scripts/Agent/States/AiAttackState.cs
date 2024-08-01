using System.Collections;
using UnityEngine;
using EZCameraShake;

public class AiAttackState : AiState
{
    private Coroutine attackCoroutine;
    private CameraShaker cameraShaker;
    private Rigidbody playerRb;

    public void Enter(AiAgent agent)
    {
        SetPosition(agent); // Set position for enemy start attack
        attackCoroutine = agent.StartCoroutine(Attack(agent));
        if (cameraShaker == null) cameraShaker = agent.mainCamera.GetComponentInChildren<CameraShaker>(); // Get camera shaker and cash it to reuse;
    }

    public void Exit(AiAgent agent)
    {
        agent.StopCoroutine(attackCoroutine);
    }

    public AiStateId GetId()
    {
        return AiStateId.Attack;
    }

    public void Update(AiAgent agent)
    {

    }

    private IEnumerator Attack(AiAgent agent)
    {
        DisablePlayerControll(agent);
        agent.playerCamera.LookAtEnemy(agent.lookPoint);
        agent.animator.SetBool("Attacking", true);

        yield return new WaitForSeconds(0.4f); // Duration of the attack animation
        PushPlayerBack(agent);
        agent.playerHealth.TakeDamage(33f);
        agent.animator.SetBool("Attacking", false);
        agent.stateMachine.ChangeState(AiStateId.Laugh); //Change to the laughing state
    }

    private void DisablePlayerControll(AiAgent agent)
    {
        //Disable ability to move and turn camera while enemy attacking
        agent.playerCamera.enabled = false;
        agent.playerMovement.enabled = false;
        agent.playerFootStepSystem.enabled = false;
        agent.navMeshAgent.stoppingDistance = 8f; // Prevent enemy to get to close to the player
    }

    private void SetPosition(AiAgent agent)
    {
        Vector3 directionToPlayer = (agent.playerTransform.position - agent.transform.position).normalized; // Direction to the player
        Vector3 attackPosition = agent.playerTransform.position - directionToPlayer * 5f; // Position for the enemy
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer); // Orientaton to the player

        // Set position of the enemy
        agent.transform.rotation = lookRotation;
        agent.transform.position = attackPosition;
    }

    private void PushPlayerBack(AiAgent agent)
    {
        if (playerRb == null) playerRb = agent.playerMovement.GetComponent<Rigidbody>(); // Set player rb

        Vector3 forceDirection = (agent.playerTransform.position - agent.lookPoint.position).normalized; // Direction to apple force
        forceDirection.y = 0; // Delete vertical force
        float forceMultiplier = agent.playerFootStepSystem.isGrounded ? 1f : 0.1f; // Decrease attack force if player mid air

        playerRb.AddForce(forceDirection * agent.config.attackForce * forceMultiplier, ForceMode.Impulse);
        cameraShaker.ShakeOnce(30f, 12f, 0.5f, 2f); // Shake camera when enemy attack the player
    }
}
