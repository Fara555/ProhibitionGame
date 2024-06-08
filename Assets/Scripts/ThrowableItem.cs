using Hearing;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using static Hearing.Sound;

public class ThrowableItem : MonoBehaviour, IInteractable
{
    private Rigidbody rb, playerRB;
    private Collider coll;
    private Transform cam;

    [SerializeField] private float dropForwardForce, dropUpwardForce;
    [SerializeField] private float throwForwardForce, throwUpwardForce;
    private List<Transform> enemies;

    private bool equieped;
    public static bool slotFull;
    private bool dropped;


    //IInteractable
    public bool destroyable => false;
    
    private void Start()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = new List<Transform>();
        foreach (GameObject enemyObject in enemyObjects)
        {
            enemies.Add(enemyObject.transform);
        }

        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        playerRB = IInteractable.playerTransform.GetComponent<Rigidbody>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();

        //Setup
        if (!equieped)
        {
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        else
        {
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
        }

        if (!equieped) this.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && equieped) Drop();

        if (!equieped) this.enabled = false;

        if (Input.GetKeyDown(KeyCode.Mouse0) && equieped) 
        {
            Throw();

        }
    }

    //IInteractable
    public void Interact()
    {
        if (!equieped && !slotFull)
        {
            PickUp();
            this.enabled = true;
        }
    }

    public void PickUp()
    {
        equieped = true;
        slotFull = true;
        dropped = false;

        rb.isKinematic = true;

        coll.isTrigger = true;
    }

    private void Drop()
    {
        equieped = false;
        slotFull = false;
        dropped = true;

        transform.SetParent(null);

        rb.isKinematic = false;
        coll.isTrigger = false;

        rb.velocity = playerRB.velocity;

        rb.AddForce(cam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(cam.up * dropUpwardForce, ForceMode.Impulse);

        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);    
    }

    private void Throw()
    {
        equieped = false;
        slotFull = false;

        transform.SetParent(null);

        rb.isKinematic = false;
        coll.isTrigger = false;

        rb.velocity = playerRB.velocity;

        rb.AddForce(cam.forward * throwForwardForce, ForceMode.Impulse);
        rb.AddForce(cam.up * throwUpwardForce, ForceMode.Impulse);

        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);
    }

    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private bool destroyOnCollide = false;

    [Header("Sounds")]
    [SerializeField] private float soundRange;
    [SerializeField] private Sound.SoundType soundType = Sound.SoundType.Interesting;
    [SerializeField] private LayerMask soundListeners;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip throwSound;
    private AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionLayerMask == (collisionLayerMask | (1 << collision.gameObject.layer)) && !dropped)
        {
            if (destroyOnCollide) Destroy(this.gameObject);

            audioSource.clip = throwSound;
            audioSource.Play();

            var sound = new Sound(transform.position, soundRange, soundType);
            Sounds.MakeSound(sound, soundListeners);
        }
        if (enemyMask == (enemyMask | (1 << collision.gameObject.layer)) && !dropped)
        {
            AiAgent enemy =  collision.gameObject.GetComponentInParent<AiAgent>();
            if (enemy.stateMachine.currentState != AiStateId.Attack) enemy.stateMachine.ChangeState(AiStateId.Stunned);

        }
        if (dropped)
        {
            audioSource.clip = dropSound;
            audioSource.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, soundRange);
    }
}