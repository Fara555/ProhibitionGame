using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CraftingRecipe
{
    public List<string> requiredItemNames;
    public GameObject resultPrefab;
    public float craftTime;
}

public class Machine : MonoBehaviour, IInteractable
{
    public bool destroyable => false;
    [HideInInspector] public bool working = false;
    [HideInInspector] public List<IItem> itemsInMachine = new List<IItem>();
    [HideInInspector] public Transform spawnPoint;

    [SerializeField] private List<CraftingRecipe> craftingRecipes;

    private InteractSystem interactSystem;

    protected virtual void Start()
    {
        interactSystem = InteractSystem.instance;
        spawnPoint = GetComponentInChildren<Transform>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            ClearMachine();
        }
    }

    public void Interact()
    {
        UseMachine();
    }

    protected virtual void UseMachine()
    {
        if (!working)
        {
            PutItemInMachine();
        }
    }

    private void PutItemInMachine()
    {
        GameObject objectInHand = InteractSystem.instance?.objectInHand;
        if (objectInHand != null)
        {
            IItem itemInHand = objectInHand.GetComponent<IItem>();
            if (itemInHand != null)
            {
                itemsInMachine.Add(itemInHand);
                InteractSystem.instance.DropItem();
                Destroy(objectInHand);

                CheckForCrafting();
            }
        }
    }

    private void ClearMachine()
    {
        RaycastHit hit;
        Ray ray = new Ray(interactSystem.mainCamera.transform.position, interactSystem.mainCamera.transform.forward);

        if (Physics.Raycast(ray, out hit, interactSystem.interactRange, interactSystem.interactLayer))
        {
            if (hit.collider.gameObject.CompareTag("Machine"))
            {
                Machine machine = hit.collider.GetComponent<Machine>();
                if (machine != null)
                {
                    machine.ClearItemsInMachine();
                    Debug.Log(hit.collider.gameObject.name + " has been cleared");
                }
            }
        }
    }

    public void ClearItemsInMachine()
    {
        if (itemsInMachine.Count > 0)
        {
            itemsInMachine.Clear();
        }
    }

    private void CheckForCrafting()
    {
        CraftingRecipe recipe = GetMatchingRecipe();
        if (recipe != null)
        {
            StartCoroutine(StartCrafting(recipe));
        }
    }

    private CraftingRecipe GetMatchingRecipe()
    {
        foreach (CraftingRecipe recipe in craftingRecipes)
        {
            if (HasRequiredItems(recipe))
            {
                return recipe;
            }
        }
        return null;
    }

    private bool HasRequiredItems(CraftingRecipe recipe)
    {
        List<string> itemNamesInMachine = itemsInMachine.Select(item => item.ItemName).ToList();
        return !recipe.requiredItemNames.Except(itemNamesInMachine).Any();
    }

    private IEnumerator StartCrafting(CraftingRecipe recipe)
    {
        working = true;
        Debug.Log("Crafting started!");

        yield return new WaitForSeconds(recipe.craftTime);

        itemsInMachine.Clear();
        CreateCraftedItem(recipe);
        working = false;
    }

    protected virtual void CreateCraftedItem(CraftingRecipe recipe)
    {
        if (spawnPoint != null)
        {
            Instantiate(recipe.resultPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
