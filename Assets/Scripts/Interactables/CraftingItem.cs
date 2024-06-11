using UnityEngine;

public class CraftingItem : MonoBehaviour, IItem, IInteractable
{
    public bool destroyable => false;

    [SerializeField] private string itemName;
    [SerializeField] private int itemID;
    [SerializeField] private float quality;
    [SerializeField] private float price;
    public string ItemName => itemName;
    public int ItemID => itemID;
    public float Quality => quality;
    public float Price => price;

    private InteractSystem interactSystem;
    private Rigidbody rb;
    private Collider coll;

    private void Start()
    {
        interactSystem = InteractSystem.instance;
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();

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

    public void Interact()
    {

    }
}