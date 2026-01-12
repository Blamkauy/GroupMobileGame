using UnityEngine;

public class Dummy : Entity
{
    [SerializeField] TMPro.TextMeshPro hitNumber;
    int lastHitVal;
    float hte = 0f;
    [SerializeField] Gradient damageGradient;
    private void Update()
    {
        Color newCol = damageGradient.Evaluate((lastHitVal * .1f) / (lastHitVal * .1f + 1f));
        newCol.a = Mathf.Clamp01(hte - Time.time + 1f);
        hitNumber.color = newCol;
    }
    public override bool GetHit(DamageReason source)
    {
        if(base.GetHit(source))
        {
            hte = Time.time; lastHitVal = source.Damage;
            hitNumber.text = lastHitVal.ToString();
            return true;
        }else
        { return false; }
    }
}
