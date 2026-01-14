using UnityEngine;

public class Bat : Enemy
{
    public float speed = 1f;
    public bool chasing = true;
    float tte = 0f;
    public override void Update()
    {
        currentFrame = tte>=0f?tte>5f?3:Mathf.RoundToInt(Time.time * 4) % 2 : 2;
        base.Update();
        if (!chasing) return;
        Vector3 dir = PlayerController.main.position - position;
        if(tte<0)
        {
            tte += Time.deltaTime;
            if(dir.magnitude<1f)
            {
                PlayerController.main.GetHit(new DamageReason(1, EntityTeam.Enemy));
                tte = 0f;
            }
            if (position.y < -.8f)
                tte = 0f;
            return;
        }

        velocity = new Vector3(dir.x, 0, dir.z)*speed;
        float upward = 3f - position.y;
        velocity = velocity.normalized + Vector3.up*Mathf.Clamp01(Mathf.Abs(upward))*Mathf.Sign(upward);
        angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg - 90f;
        tte += Time.deltaTime;

        if (tte > 6f)
        {
            tte = -4f;
            velocity = dir.normalized * 10;
        }
    }
}
