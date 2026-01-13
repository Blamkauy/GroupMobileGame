using UnityEngine;

public class BouncyBall : Entity
{
    public override void Update()
    {
        velocity += Physics.gravity * Time.deltaTime;
        base.Update();
    }
    public override void OnCollision(Vector3 direction, float distance, Vector3 normal)
    {
        velocity = Vector3.Reflect(velocity, normal);
    }
}
