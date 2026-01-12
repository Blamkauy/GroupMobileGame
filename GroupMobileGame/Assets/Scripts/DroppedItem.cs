using UnityEngine;

public class DroppedItem : SpriteObject
{
    public int ItemID;
    public int randomSeed;
    private void Start()
    {
        sr.sprite = GameManager.main.AllAvailiableWeapons[ItemID].DroppedSprite;
    }
    public void Pickup()
    {
        if(PlayerController.main.holdingWeapon!=null)
        {
            GameManager.main.SpawnDroppedItem(PlayerController.main.holdingWeapon.SpawnID, PlayerController.main.holdingWeapon.Seed, PlayerController.main.holdingWeapon.position);
            PlayerController.main.holdingWeapon = null;
        }
        PlayerController.main.holdingWeapon = GameManager.main.SpawnWeapon(ItemID, randomSeed,PlayerController.main.position);
        Destroy(gameObject);
    }
}
