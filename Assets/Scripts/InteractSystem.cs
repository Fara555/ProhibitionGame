using System.Collections.Generic;
using UnityEngine;
using TMPro;

interface IInteractable
{
    void Interact();
    bool destroyable { get; }

    static Transform playerTransform { get; set; }
}

public class InteractSystem : MonoBehaviour
{
    [SerializeField] private float pickupRange = 5f;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private TMP_Text itemInDistanceText;
    private List<Transform> targetsInView = new List<Transform>();
    private Camera mainCamera;

    [SerializeField] private Transform itemContainer;


    private void Awake()
    {
        mainCamera = Camera.main;
        IInteractable.playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        CleanUpTargetsInView();
        if (targetsInView.Count > 0) CheckIfInDistance();
    }

    void CheckIfInDistance()
    {
        RaycastHit hit;
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(ray, out hit, pickupRange, pickupLayer))
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
                    else if (hit.collider.gameObject.TryGetComponent(out ThrowableItem throwable))
                    {
                        hit.transform.SetParent(itemContainer);
                        hit.transform.localPosition = Vector3.zero;
                        hit.transform.localRotation = Quaternion.Euler(Vector3.zero);
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