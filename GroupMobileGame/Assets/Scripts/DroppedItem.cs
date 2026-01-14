using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class DroppedItem : Entity
{
    public int ItemID;
    public int randomSeed;
    public virtual void Fling()
    {
        float angle = Random.Range(0f, 2 * Mathf.PI);
        velocity = new Vector3(Mathf.Cos(angle), 4f, Mathf.Sin(angle));
    }
    public override void Start()
    {
        overrideSprite = GameManager.main.AllAvailiableWeapons[ItemID].DroppedSprite;
        base.Start();
    }
    public override void Update()
    {
        velocity += Physics.gravity * Time.deltaTime;
        velocity *= Mathf.Pow(0.6f, Time.deltaTime);
        
        base.Update();
    }
    public override void OnCollision(Vector3 direction, float distance, Vector3 normal)
    {
        velocity = Vector3.Reflect(velocity, normal)*.5f;
    }
    public override bool GetHit(DamageReason source)
    {
        return false;
    }
    public void Pickup()
    {
        if(PlayerController.holdingWeapon!=null)
        {
            PlayerController.main.DropHeldItem();
        }
        PlayerController.holdingWeapon = GameManager.main.SpawnWeapon(ItemID, randomSeed,PlayerController.main.position);
        Destroy(gameObject);
    }
}
