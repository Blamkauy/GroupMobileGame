using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlayerController : Entity
{
    public static PlayerController main;
    public float Movespeed;
    public InputActionReference moveAction;
    public InputActionReference shootAction;
    public GameObject mobileControls;
    public Weapon holdingWeapon = null;//Static so that the holding weapon persists upon level changes
    public Image WeaponUIIcon;
    public TMPro.TextMeshProUGUI WeaponUIText;
    public static int SpawnWithWeaponID = -1;
    public static int SpawnWithWeaponSeed = 0;
    private void Awake()
    {
        main = this;
    }
    public void EquipWeapon(int ID,int seed)
    {
        if (ID<0) DropHeldItem();
        holdingWeapon = GameManager.main.SpawnWeapon(ID, seed, position);
        SpawnWithWeaponID = ID; SpawnWithWeaponSeed = seed;

    }
    public override void Start()
    {
        base.Start();
        if(SpawnWithWeaponID>=0)
        {
            EquipWeapon(SpawnWithWeaponID, SpawnWithWeaponSeed);
        }
        mobileControls.gameObject.SetActive(GameManager.main.controlScheme==GameControlScheme.Mobile);
    }
    public override void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        Debug.Log("The player has died.");
    }
    public void SendPauseInput()
    {
        GameManager.main.SetPaused(!GameManager.main.gamePaused);
    }
    public void pickupInputOverride()
    {
        pickUpInputThisFrame = true;
    }
    public void DropHeldItem()
    {
        SpawnWithWeaponID = -1;

        if (holdingWeapon != null) return;
        GameManager.main.SpawnDroppedItem(holdingWeapon.SpawnID, holdingWeapon.Seed, holdingWeapon.position).Fling();
        Destroy(holdingWeapon.gameObject);
        holdingWeapon = null;
    }
    bool pickUpInputThisFrame = false;
    public override void Update()
    {
        if (GameManager.main.controlScheme == GameControlScheme.PC)
        {
            pickUpInputThisFrame = Input.GetKeyDown(KeyCode.E);
            if (Input.GetKeyDown(KeyCode.Escape))
                SendPauseInput();
        }

        if (GameManager.main.gamePaused)return;

        base.Update();
        Vector2 move = GameManager.main.controlScheme == GameControlScheme.Mobile?moveAction.action.ReadValue<Vector2>():Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical")),1);
        Vector3 cameraMousePos = Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.one - Vector3.forward);
        bool firingInput = Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Space);
        Vector2 shoot = GameManager.main.controlScheme == GameControlScheme.Mobile?shootAction.action.ReadValue<Vector2>(): new Vector2(cameraMousePos.x-transform.position.x,cameraMousePos.y-transform.position.y).normalized*(firingInput?1:0);
        firingInput = shoot.sqrMagnitude > 0;
        velocity = new Vector3(move.x, 0, move.y) * Movespeed;
        angle = Mathf.MoveTowardsAngle(angle, firingInput ? Mathf.Atan2(shoot.y, shoot.x) * Mathf.Rad2Deg-90f : move.sqrMagnitude > 0f ? Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg - 90f : angle,Time.deltaTime*720);


        if (pickUpInputThisFrame)
        {
            foreach(Collider2D col in Physics2D.OverlapCircleAll(transform.position,0.5f,1<<6))
            {
                DroppedItem di; if (di = col.gameObject.GetComponent<DroppedItem>()) { di.Pickup(); break; }
            }
        }

        pickUpInputThisFrame = false;

        WeaponUIIcon.transform.parent.gameObject.SetActive(holdingWeapon != null);
        if(holdingWeapon!=null)
        {
            WeaponUIIcon.sprite = GameManager.main.AllAvailiableWeapons[holdingWeapon.SpawnID].DroppedSprite;
            WeaponUIText.text = holdingWeapon.Name;
            if (holdingWeapon.Cooldown <= 0f)
            {
                if (firingInput)
                {
                    holdingWeapon.UseWeapon(shoot);
                }
            }
            else
                holdingWeapon.Cooldown -= Time.deltaTime;
            
        }
    }
}