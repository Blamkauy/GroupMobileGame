using UnityEngine;

public class Dummy : Entity
{
    [SerializeField] TMPro.TextMeshPro hitNumber;
    int lastHitVal;
    float hte = 0f;
    [SerializeField] Gradient damageGradient;
    public override void Update()
    {
        base.Update();
        Color newCol = damageGradient.Evaluate((lastHitVal * .1f) / (lastHitVal * .1f + 1f));
        newCol.a = Mathf.Clamp01(1f/((Time.time-hte)*5+1f));
        hitNumber.color = newCol;
    }
    public override bool GetHit(DamageReason source)
    {
        if(base.GetHit(source))
        {
            hte = Time.time; lastHitVal = source.Damage;
            hitNumber.text = lastHitVal.ToString();
            Vector2 move = Random.insideUnitCircle*.1f;
            hitNumber.transform.localPosition = new Vector3(move.x-.7f, 0.87f+ move.y,0);
            return true;
        }else
        { return false; }
    }
}
