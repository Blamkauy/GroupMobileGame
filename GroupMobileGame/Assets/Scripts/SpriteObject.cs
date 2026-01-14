using System.Collections;
using UnityEngine;

public class SpriteObject : MonoBehaviour
{
    public Vector3 position;//X and Z for baseic 2D directions, Y for vertical. Assume 3D math for all transformations and scene queries
    public bool setPosition = true;
    public bool setSprite = true;
    public bool setSpriteDepth = true;

    public Sprite[] sprites;//All frames of every angle in order of Angle1Frame1, Angle1Frame2,Angle1Frame3,Angle2Frame1,Angle2Frame2... etc.
    public int rotationCount=1;//How many different angles are there?
    public int currentFrame;//Curent animation frame
    public Sprite overrideSprite;//normall null, otherwise, will always set the spriterenderer to this sprite.
    public SpriteRenderer shadow;
    public SpriteRenderer sr;
    public float angle;//Verticle angle.
    IEnumerator hitFlashAnim()
    {
        sr.material = GameManager.main.avaliableMaterials[1];
        yield return new WaitForSeconds(0.1f);
        sr.material = GameManager.main.avaliableMaterials[0];

    }
    public virtual void HitFlash()
    {
        StartCoroutine(hitFlashAnim());
    }
    public virtual void OnDrawGizmos()
    {
        UpdateSprite();
    }
    public const float verticalSquash = 0.422f;//sin 25° ~= 0.422;
    public static Vector3 rasterize3DPosition(Vector3 position)
    {
        return new Vector3(position.x, position.z + position.y * verticalSquash, 0);
    }
    public virtual void UpdateSprite()
    {
        if(setPosition)
        {
            transform.position = rasterize3DPosition(position);
            if(shadow!=null)
            shadow.transform.position = new Vector3(position.x, position.z-verticalSquash*1.5f, 0);
        }

        if (sr == null||!setSprite) return;
        if(setSpriteDepth)
        {
            sr.sortingOrder = Mathf.RoundToInt(-position.z*10);
        }
        if (overrideSprite != null)
        {
            sr.sprite = overrideSprite;
        }
        else
        {
            if (sprites == null || sprites.Length <= 0) return;
            float truncatedAngle = angle / 360f % 1;
            while (truncatedAngle < 0) truncatedAngle++;
            sr.sprite = rotationCount <=1||sprites.Length<rotationCount ? sprites[currentFrame] : sprites[(Mathf.RoundToInt(truncatedAngle * rotationCount) * sprites.Length / rotationCount + currentFrame)%sprites.Length];
        }
    }
    public virtual void Update()
    {
        UpdateSprite();
    }
}
