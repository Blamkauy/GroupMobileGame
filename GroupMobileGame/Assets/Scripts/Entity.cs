using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Entity : SpriteObject
{
    public int health;
    public Rect hitbox;//defines the XZ flat profile, assumes a height of 1.0 in 3D
    public static List<Entity> allEntities;
    public EntityTeam team;
    public List<Effect> effects;
    public Vector3 velocity;
    public bool usePhysics = true;
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

        if (usePhysics && velocity.magnitude > 0)
        {
            Vector2 fakeVel = new Vector2(velocity.x, velocity.z);
            RaycastHit2D collideOnMove = Physics2D.Raycast(new Vector2(position.x,position.z)+fakeVel.normalized*0.05f, fakeVel, fakeVel.magnitude * Time.deltaTime, 1 << 7);
            if (collideOnMove.collider != null)
            {
                OnCollision(velocity.normalized, collideOnMove.distance,new Vector3(collideOnMove.normal.x,0,collideOnMove.normal.y));
            }
            if (position.y + velocity.y * Time.deltaTime < -1f)
            {

                OnCollision(velocity.normalized, position.y + 1f,Vector3.up);


            }
        }
        position += velocity* Time.deltaTime;

        base.Update();

    }
    public virtual void OnCollision(Vector3 direction,float distance,Vector3 normal)
    {
        //Debug.DrawLine(rasterize3DPosition(position), rasterize3DPosition(direction * distance + position),Color.yellow,.1f);
        //Debug.DrawLine(rasterize3DPosition(direction * distance + position), rasterize3DPosition(direction * distance + position + normal),Color.blue,.1f);
        //Debug.DrawLine(rasterize3DPosition(position), rasterize3DPosition(direction+position),Color.red,.1f);
        if (Vector3.Dot(normal, velocity) > 0f) return;
        if(normal == Vector3.up)
        {
            velocity = new Vector3(velocity.x, 0, velocity.y);

        }
        else
        {
            Vector2 perp = new Vector2(-normal.z, normal.x);
            perp = Vector2.Dot(perp, new Vector2(velocity.x,velocity.z)) * perp;
            velocity = new Vector3(perp.x, velocity.y, perp.y);
        }
    }
    public virtual void OnDestroy()
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
                ef.timeDestroy = Mathf.Max(ef.timeDestroy, Time.time+duration);
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
                ef.timeDestroy = Mathf.Max(ef.timeDestroy,effect.timeDestroy);
                effect.Destroy();
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