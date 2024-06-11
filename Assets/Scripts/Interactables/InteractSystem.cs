using System.Collections.Generic;
using UnityEngine;
using TMPro;

interface IInteractable
{
    void Interact();
    bool destroyable { get; }
}

public interface IItem
{
    string ItemName { get; }
    int ItemID { get; }
    float Quality { get; }
    float Price { get; }
    static Transform playerTransform { get; set; }
}

public class InteractSystem : MonoBehaviour
{
    public static InteractSystem instance;  

    public float interactRange = 5f;
    public LayerMask interactLayer;
    [SerializeField] private TMP_Text itemInDistanceText;
    private List<Transform> targetsInView = new List<Transform>();
    [HideInInspector] public Camera mainCamera;

    [SerializeField] private Transform itemContainer;
    [HideInInspector] public GameObject objectInHand = null;

    [HideInInspector] public bool equieped;
    [HideInInspector] public static bool slotFull;
    [HideInInspector] public bool dropped;

    [SerializeField] private float dropForwardForce, dropUpwardForce;
    [SerializeField] private float throwForwardForce, throwUpwardForce;

    [HideInInspector] public Rigidbody itemRB, playerRB;
    [HideInInspector] public Collider itemColl;
    [HideInInspector] public Transform cameraTransform;

    private List<Transform> enemies;


    private void Awake()
    {
        if (instance == null) instance = this;

        mainCamera = Camera.main;
        IItem.playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        playerRB = IItem.playerTransform.GetComponent<Rigidbody>();
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
    }

    private void Update()
    {
        CleanUpTargetsInView();
        if (targetsInView.Count > 0) CheckIfInDistance();

        if (Input.GetKeyDown(KeyCode.Q) && equieped) DropItem();

        if (Input.GetKeyDown(KeyCode.Mouse0) && equieped)
        {
            ThrowItem();
        }
    }

    private void CheckIfInDistance()
    {
        RaycastHit hit;
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(ray, out hit, interactRange, interactLayer))
        {
            IInteractable interactObject = hit.collider.gameObject.GetComponent<IInteractable>();
            if (interactObject != null)
            {
                itemInDistanceText.enabled = true;
                itemInDistanceText.text = hit.collider.gameObject.name;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactObject.Interact();
                    if (interactObject.destroyable)
                    {
                        targetsInView.Remove(hit.transform);
                        Destroy(hit.collider.gameObject);
                        itemInDistanceText.enabled = false;
                    }
                    else if (hit.collider.gameObject.TryGetComponent(out CraftingItem craftingItem) || (hit.collider.gameObject.TryGetComponent(out Storage storage)))
                    {
                        PickUp(hit.collider.gameObject);
                    }
                }
            }
            else
            {
                itemInDistanceText.enabled = false;
            }
        }
        else
        {
            itemInDistanceText.enabled = false;
        }
    }

    public void DropItem()
    {
        if (objectInHand != null)
        {
            equieped = false;
            slotFull = false;
            dropped = true;

            objectInHand.transform.SetParent(null);

            Collider coll = objectInHand.GetComponent<Collider>();
            Rigidbody rb = coll.GetComponent<Rigidbody>();
            coll.isTrigger = false;
            rb.isKinematic = false;
            rb.velocity = playerRB.velocity;

            rb.AddForce(cameraTransform.forward * dropForwardForce, ForceMode.Impulse);
            rb.AddForce(cameraTransform.up * dropUpwardForce, ForceMode.Impulse);

            float random = Random.Range(-1f, 1f);
            rb.AddTorque(new Vector3(random, random, random) * 10);

            objectInHand = null; // Обнулить objectInHand после выброса предмета
        }
    }



    private void ThrowItem()
    {
        equieped = false;
        slotFull = false;

        objectInHand.transform.SetParent(null);
        itemColl.isTrigger = false;
        itemRB.isKinematic = false;
        itemRB.velocity = playerRB.velocity;

        itemRB.AddForce(cameraTransform.forward * throwForwardForce, ForceMode.Impulse);
        itemRB.AddForce(cameraTransform.up * throwUpwardForce, ForceMode.Impulse);

        float random = Random.Range(-1f, 1f);
        itemRB.AddTorque(new Vector3(random, random, random) * 10);
        objectInHand = null;
    }

    public void PickUp(GameObject pickUpObject)
    {
        if (!equieped && !slotFull)
        {
            itemColl = pickUpObject.GetComponent<Collider>();
            itemRB = pickUpObject.GetComponent<Rigidbody>();
            objectInHand = pickUpObject;
            objectInHand.transform.SetParent(itemContainer);
            objectInHand.transform.localPosition = Vector3.zero;
            objectInHand.transform.localRotation = Quaternion.Euler(Vector3.zero);

            equieped = true;
            slotFull = true;
            dropped = false;

            itemRB.isKinematic = true;
            itemColl.isTrigger = true;
        }
    }


    void CleanUpTargetsInView()
    {
        targetsInView.RemoveAll(item => item == null || item.gameObject == null);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable")) targetsInView.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable")) targetsInView.Remove(other.transform);
    }
}