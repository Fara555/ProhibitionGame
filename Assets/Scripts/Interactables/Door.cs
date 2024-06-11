using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    public bool destroyable => false;
    [SerializeField] private Transform spawnPoint, playerTransform;

    public void Interact()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        playerTransform.position = spawnPoint.position;
        playerTransform.rotation = spawnPoint.rotation;
    }
}