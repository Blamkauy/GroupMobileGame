using UnityEngine;

public class TestStabbySword : Weapon
{
    public override void Update()
    {
        base.Update();
        if(Cooldown<=0f)
        {
            transform.rotation = Quaternion.Euler(0, 0, PlayerController.main.angle);
            position = PlayerController.main.position + new Vector3(transform.up.x, 0f, transform.up.y)*.5f;
        }else
        {

            position = PlayerController.main.position + new Vector3(transform.up.x, 0f, transform.up.y)*(.5f+Cooldown*Cooldown*0.4f);
        }
        
    }
    public override void Randomize(int Seed)
    {
        base.Randomize(Seed);
        Name = "Stabby Sword";
        //nothing yet
    }
    public override void UseWeapon(Vector2 direction)
    {
        //nothing yet
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg-90f);

        Cooldown = 1f;
        position = PlayerController.main.position + new Vector3(transform.up.x, 0f, transform.up.y) * (.5f + Cooldown * Cooldown * 0.4f);

        foreach (Entity en in Entity.OverlapHitbox( new HitBox(position,new Rect(0,0,1f,1f),2f)))
        {
            en.GetHit(new DamageReason(1, EntityTeam.Player));
        }
    }
}
