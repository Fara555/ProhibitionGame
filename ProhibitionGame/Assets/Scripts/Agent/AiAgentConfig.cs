using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AiAgentConfig : ScriptableObject
{
    [Header("ChasePlayer")]
    public float chaseSpeed = 8f;
    public float maxTime = 1.0f;
    public float maxDistance = 1.0f;
    public float chaseTime = 5f;

    [Header("Patrol")]
    public float patrolSpeed = 5f;
    public float patrolWaitTime = 3.0f;
    public float patrolRadius = 10f;

    [Header("Attack")]
    public float attackForce = 20f;
    public float laughTime = 2f;

    [Header("Stunned")]
    public float stunTime = 1f;
}
