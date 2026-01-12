using System.Collections.Generic;
using UnityEngine;


public class Entity : SpriteObject
{
    public int health;
    public Rect hitbox;//defines the XZ flat profile, assumes a height of 1.0 in 3D
    public static List<Entity> allEntities;
    public EntityTeam team;
    public List<Effect> effects;
    public virtual void Start()
    {
        if (allEntities == null)
            allEntities = new List<Entity>();
        allEntities.Add(this);
        effects = new List<Effect>();
    }
    public override void Update()
    {
        foreach(Effect ef in effects)
        {
            ef.Affect(this);
        }
        base.Update();
    }
    private void OnDestroy()
    {
        foreach (Effect ef in effects)
            ef.Destroy();
        if (gameObject.scene.isLoaded)
            allEntities.Remove(this);
    }
    public virtual void Die()
    {
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + new Vector3(hitbox.x, hitbox.y, 0), new Vector3(hitbox.width, hitbox.height, 1f));
    }
    public virtual bool GetHit(DamageReason source)
    {
        if (source.team != EntityTeam.Neutral && source.team == team) return false;
        health -= source.Damage;
        if (health <= 0) Die();
        return true;
    }
    public bool intersectsHitbox(HitBox form)
    {
        return HitBox.HitboxIntersect(new HitBox(position, hitbox, 1f), form);
    }
    public void AddEffect<T>(float duration) where T : Effect
    {
        foreach(Effect ef in effects)
        {
            if (ef.GetType() == typeof(T))
            {
                ef.timeDestroy = Time.time+ duration;
                break;
            }
        }
        effects.Add((Effect)System.Activator.CreateInstance(typeof(T),new object[] {duration}));
    }
    public void AddEffect(Effect effect)
    {

        foreach (Effect ef in effects)
        {
            if (ef.GetType() == effect.GetType())
            {
                ef.timeDestroy = effect.timeDestroy;
                break;
            }
        }
        effects.Add(effect);
    }
    public static Entity[] OverlapHitbox(HitBox form)//returns a list of entities that are intersecting the form.
    {
        List<Entity> foundEntities = new List<Entity>();
        DrawHitBox(form);
        foreach (Entity entity in allEntities)
        {
            if (entity.intersectsHitbox(form))
                foundEntities.Add(entity);
        }
        return foundEntities.ToArray();
    }
    public static void DrawHitBox(HitBox form)
    {
        Vector3 orig = rasterize3DPosition(form.position);
        Debug.DrawLine(orig + new Vector3(-form.profile.width, form.profile.height,0)*.5f, orig + new Vector3(form.profile.width, form.profile.height, 0) * .5f, Color.red,.5f);
        Debug.DrawLine(orig + new Vector3(form.profile.width, form.profile.height, 0) * .5f, orig + new Vector3(form.profile.width, -form.profile.height, 0) * .5f, Color.red, .5f);
        Debug.DrawLine(orig + new Vector3(-form.profile.width, -form.profile.height, 0) * .5f, orig + new Vector3(form.profile.width, -form.profile.height, 0) * .5f, Color.red, .5f);
        Debug.DrawLine(orig + new Vector3(-form.profile.width, -form.profile.height, 0) * .5f, orig + new Vector3(-form.profile.width, form.profile.height, 0) * .5f, Color.red, .5f);
    }
}
public enum EntityTeam { Player, Neutral, Enemy }
[System.Serializable]
public struct HitBox
{
    public Vector3 position;
    public float height;
    public Rect profile;
    public HitBox(Vector3 pos, Rect p, float h)
    {
        position = pos; profile = p; height = h;
    }
    public static bool HitboxIntersect(HitBox formA, HitBox formB)
    {
        return Mathf.Abs(formA.position.y - formB.position.y) < (formA.height + formB.height)*.5f && Mathf.Abs(formA.position.x + formA.profile.x - formB.position.x - formB.profile.x) < (formA.profile.width + formB.profile.width)*.5f && Mathf.Abs(formA.position.z + formA.profile.y - formB.position.z - formB.profile.y) < (formA.profile.height + formB.profile.height)*.5f;
    }
}
public struct DamageReason
{
    public int Damage;
    public EntityTeam team;
    public DamageReason(int d, EntityTeam t)
    {
        Damage = d; team = t;
    }
}
public class Effect
{
    public float timeDestroy = 0f;
    public ParticleSystem particles;
    public Effect(float Duration)
    {
        timeDestroy = Time.time+Duration;
    }
    public virtual void Destroy()
    {
        if (particles != null)
            GameObject.Destroy(particles.gameObject);

    }
    public virtual void Affect(Entity host)
    {
        if (particles != null)
            particles.transform.position = host.transform.position-Vector3.forward;
        if(Time.time>timeDestroy)
        {
            Destroy();
            host.effects.Remove(this);

        }
    }
}
public class DebugFireEffect : Effect
{
    float time = 0f;

    public DebugFireEffect(float Duration) : base(Duration)
    {
        timeDestroy = Time.time + Duration;
        particles = GameManager.main.SpawnParticles(0);
    }

    public override void Affect(Entity host)
    {
        if (Time.time - time > .5f)
        {
            time = Time.time;
            host.GetHit(new DamageReason(1, EntityTeam.Neutral));
        }
        base.Affect(host);
    }
}