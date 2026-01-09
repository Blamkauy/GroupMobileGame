using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Entity
{
    public float Movespeed;
    public InputActionReference moveAction;
    public InputActionReference shootAction;
    public GameObject mobileControls;
    public override void Start()
    {
        base.Start();
        mobileControls.gameObject.SetActive(GameManager.main.controlScheme==GameControlScheme.Mobile);
    }
    public override void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        Debug.Log("The player has died.");
    }
    public void pickupInputOverride()
    {
        pickUpInputThisFrame = true;
    }
    bool pickUpInputThisFrame = false;
    public override void Update()
    {
        base.Update();
        Vector2 move = GameManager.main.controlScheme == GameControlScheme.Mobile?moveAction.action.ReadValue<Vector2>():Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical")),1);
        Vector3 cameraMousePos = Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.one - Vector3.forward);
        bool firingInput = Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Space);
        Vector2 shoot = GameManager.main.controlScheme == GameControlScheme.Mobile?shootAction.action.ReadValue<Vector2>(): new Vector2(cameraMousePos.x-transform.position.x,cameraMousePos.y-transform.position.y).normalized*(firingInput?1:0);
        firingInput = shoot.sqrMagnitude > 0;
        position += new Vector3(move.x,0,move.y)*Time.deltaTime*Movespeed;
        angle = firingInput ? Mathf.Atan2(shoot.y, shoot.x) * Mathf.Rad2Deg-90f : move.sqrMagnitude > 0f ? Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg - 90f : angle;
        if (GameManager.main.controlScheme == GameControlScheme.PC) pickUpInputThisFrame = Input.GetKeyDown(KeyCode.E);


        pickUpInputThisFrame = false;
    }
}
public class Entity : SpriteObject
{
    public int health;
    public Rect hitbox;//defines the XZ flat profile, assumes a height of 1.0 in 3D
    public static List<Entity> allEntities;
    public EntityTeam team;
    public virtual void Start()
    {
        if(allEntities == null)
        allEntities = new List<Entity>();
        allEntities.Add(this);
    }
    private void OnDestroy()
    {
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
        Gizmos.DrawWireCube(transform.position+new Vector3(hitbox.x,hitbox.y,0), new Vector3(hitbox.width, hitbox.height,1f));
    }
    public virtual void GetHit(DamageReason source)
    {
        if (source.team != EntityTeam.Neutral && source.team == team) return;
        health -= source.Damage;
        if (health <= 0) Die();
    }
    public bool intersectsHitbox(HitBox form)
    {
        return HitBox.HitboxIntersect(new HitBox(position, hitbox, 1f), form);
    }
    public static Entity[] OverlapHitbox(HitBox form)//returns a list of entities that are intersecting the form.
    {
        List<Entity> foundEntities = new List<Entity>();
        foreach (Entity entity in allEntities)
        {
            if(entity.intersectsHitbox(form))
                foundEntities.Add(entity);
        }
        return foundEntities.ToArray();
    }
}
public enum EntityTeam {Player, Neutral, Enemy}
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
        return Mathf.Abs(formA.position.y - formB.position.y) < formA.height + formB.height && Mathf.Abs(formA.position.x + formA.profile.x - formB.position.x -formB.profile.x) < formA.profile.width + formB.profile.width && Mathf.Abs(formA.position.z + formA.profile.y - formB.position.z - formB.profile.y) < formA.profile.height + formB.profile.height;
    }
}
public struct DamageReason
{
    public int Damage;
    public EntityTeam team;
}