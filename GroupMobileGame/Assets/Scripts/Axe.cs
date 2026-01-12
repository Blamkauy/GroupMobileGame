using UnityEngine;

public class Axe : Weapon
{
    float attackAngle = 0f;
    bool bufferAttack = false;
    static string[] NamePrefixes = new string[] { "Generic", "Flaming" };
    static System.Type[] effectList = new System.Type[] { null, typeof(DebugFireEffect) };
    int EffectID = 0;
    public override void Update()
    {
        base.Update();
        angle = 0f;
        if (Cooldown <= 0f)
        {
            angle = PlayerController.main.angle - 90f;
        sr.flipY = (angle + 810) % 360 > 180;
        }
        else
        {

            angle = Mathf.LerpAngle(attackAngle+Mathf.Clamp(22f-Cooldown*12,-1.5f,1f)*45*-Mathf.Sign((attackAngle+810)%360-180),PlayerController.main.angle - 90f,Mathf.Clamp01(1f-Cooldown));
            position = PlayerController.main.position + new Vector3(-transform.right.x, 0f, -transform.right.y) * .5f;
            sr.flipY = (attackAngle + 810) % 360 > 180;
            if(Cooldown<1.8f&&bufferAttack)
            {
                bufferAttack = false;


                foreach (Entity en in Entity.OverlapHitbox(new HitBox(position, new Rect(0, 0, 2f, 2f), 1f)))
                {
                    if (en.GetHit(new DamageReason(1, EntityTeam.Player))&&EffectID!=0) en.AddEffect((Effect)System.Activator.CreateInstance(effectList[EffectID], new object[] { 10f }));
                    
                }
            }
        }

        transform.rotation = Quaternion.Euler(0, 0, angle);
        position = PlayerController.main.position + new Vector3(-transform.right.x, 0f, -transform.right.y) * .5f;

    }
    public override void Randomize(int Seed)
    {
        base.Randomize(Seed);
        System.Random random = new System.Random(Seed);
        EffectID = random.Next(NamePrefixes.Length);
        Name = NamePrefixes[EffectID]+" Axe";
        //nothing yet
    }
    public override void UseWeapon(Vector2 direction)
    {
        //nothing yet
        attackAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 180f;
        transform.rotation = Quaternion.Euler(0, 0, attackAngle);

        Cooldown = 2f;
        position = PlayerController.main.position + new Vector3(-transform.right.x, 0f, -transform.right.y) * .5f;
        bufferAttack = true;
    }
}
