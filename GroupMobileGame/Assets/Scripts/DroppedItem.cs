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
            PlayerController.main.DropHeldItem();
        }
        PlayerController.main.holdingWeapon = GameManager.main.SpawnWeapon(ItemID, randomSeed,PlayerController.main.position);
        Destroy(gameObject);
    }
}
