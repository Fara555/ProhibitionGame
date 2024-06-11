using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageContainer
{
    public string itemName;
    public int itemID;
    public float price;
    public float quality;

    public StorageContainer(string itemName, int itemID, float price, float quality)
    {
        this.itemName = itemName;
        this.itemID = itemID;
        this.price = price;
        this.quality = quality;
    }
}

public class Storage : MonoBehaviour, IInteractable
{
    [SerializeField] private List<StorageContainer> storageContainer = new List<StorageContainer>();
    private InteractSystem interactSystem;

    public bool destroyable => false;

    public void Interact()
    {

    }

    private void Start()
    {
        interactSystem = InteractSystem.instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && interactSystem.equieped)
        {
            AddItemToStorage();
        }
    }

    private void AddItemToStorage()
    {
        RaycastHit hit;
        Ray ray = new Ray(interactSystem.mainCamera.transform.position, interactSystem.mainCamera.transform.forward);

        if (Physics.Raycast(ray, out hit, interactSystem.interactRange, interactSystem.interactLayer))
        {
            Storage storageObject = hit.collider.gameObject.GetComponent<Storage>();

            if (hit.collider.CompareTag("Storage"))
            {
                GameObject objectInHand = interactSystem.objectInHand;
                if (objectInHand != null)
                {
                    IItem item = objectInHand.GetComponent<IItem>();
                    if (item != null)
                    {
                        storageObject.storageContainer.Add(new StorageContainer(item.ItemName, item.ItemID, item.Price, item.Quality));
                        interactSystem.DropItem();
                        Destroy(objectInHand);
                        Debug.Log("Item added to storage: " + item.ItemName);
                    }
                }
            }
        }
    }
}
