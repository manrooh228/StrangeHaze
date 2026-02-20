using Assets.Scripts.weapons;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum ItemType { Ammo, Weapon }
    public ItemType type;

    public string itemName = "Предмет";
    public int amount = 6;
    public GameObject promptUI;

    private bool _canPickup;

    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) { _canPickup = true; promptUI.SetActive(true); } }
    void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) { _canPickup = false; promptUI.SetActive(false); } }

    void Update()
    {
        if (_canPickup && Input.GetKeyDown(KeyCode.E))
        {
            if (type == ItemType.Ammo)
            {
                InventoryManager.Instance.AddAmmo(amount);
            }
            else if (type == ItemType.Weapon)
            {
                InventoryManager.Instance.EquipWeapon();
                Destroy(FindAnyObjectByType<idle>());
            }

            Destroy(gameObject);
        }
    }
}
