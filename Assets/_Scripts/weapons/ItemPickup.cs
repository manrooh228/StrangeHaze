using Assets.Scripts.weapons;
using Assets.Scripts.Needs;
using StrangeHaze.Bootstrap;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum ItemType { Ammo, Weapon, Food }
    public ItemType type;

    public string itemName = "�������";
    public int amount = 1;
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
                ReloadManager.Instance.AddAmmo(amount);
            }
            else if (type == ItemType.Weapon)
            {
                ReloadManager.Instance.EquipWeapon();
            }
            else if (type == ItemType.Food)
            {
                ServiceLocator.Get<IPlayerNeedsService>().Eat(amount);
            }

            Destroy(gameObject);
        }
    }
}
