using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("인벤토리 UI")]
    [SerializeField] private GameObject inventoryCanvas; // 인벤토리 전체 UI
    private bool isOpen;

    void Start()
    {
        if (inventoryCanvas) inventoryCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        if (inventoryCanvas) inventoryCanvas.SetActive(isOpen);

        if (isOpen)
            InputLockManager.Acquire("Inventory"); // 잠금
        else
            InputLockManager.Release("Inventory"); // 해제
    }
}
