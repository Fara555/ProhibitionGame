using UnityEngine;

public class CraftingItem : MonoBehaviour, IItem, IInteractable
{
    public bool destroyable => false;

    [SerializeField] private string itemName;
    [SerializeField] private int itemID;
    public string ItemName => itemName;
    public int ItemID => itemID;

    private InteractSystem interactSystem;
    private Rigidbody rb;
    private Collider coll;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();

        interactSystem = InteractSystem.instance;
        if (!interactSystem.equieped)
        {
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        else
        {
            rb.isKinematic = true;
            coll.isTrigger = true;
            InteractSystem.slotFull = true;
        }

        if (!interactSystem.equieped) this.enabled = false;
    }

    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private bool destroyOnCollide = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionLayerMask == (collisionLayerMask | (1 << collision.gameObject.layer)) && !interactSystem.dropped)
        {
            if (destroyOnCollide) Destroy(this.gameObject);
        }
        if (enemyMask == (enemyMask | (1 << collision.gameObject.layer)) && !interactSystem.dropped)
        {
            AiAgent enemy = collision.gameObject.GetComponentInParent<AiAgent>();
            if (enemy.stateMachine.currentState != AiStateId.Attack) enemy.stateMachine.ChangeState(AiStateId.Stunned);
        }
    }

    public void Interact()
    {

    }
}