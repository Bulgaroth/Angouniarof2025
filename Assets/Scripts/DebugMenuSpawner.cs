using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class DebugMenuSpawner : MonoBehaviour
{
    [SerializeField] private GameObject DebugMenu;
    public void OnToggleDebugMenu(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed) return;
        DebugMenu.SetActive(!DebugMenu.activeInHierarchy);
    }
}
