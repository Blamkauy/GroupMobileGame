using UnityEngine;

public class Weapon : SpriteObject
{
    public string Name;
    [HideInInspector]
    public int SpawnID = -1;
    public int Seed = 0;
    public float Cooldown = 0f;
    public virtual void Start()
    {

    }
    public virtual void Randomize(int Seed)
    {
        this.Seed = Seed;
    }
    public virtual void UseWeapon(Vector2 direction)
    {

    }

}
[System.Serializable]
public struct WeaponEntry
{
    public Weapon prefabReference;
    public Sprite DroppedSprite;
}