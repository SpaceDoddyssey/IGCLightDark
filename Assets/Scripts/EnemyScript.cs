using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    int hp = 3;
    public SpriteRenderer spriteRender;
    public Sprite defaultSprite, outlineSprite;

    void Start() {

        GameObject spritechild = gameObject.transform.GetChild(0).gameObject;
        spriteRender = spritechild.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        // var outline = gameObject.AddComponent<Outline>();

        // outline.OutlineMode = Outline.Mode.OutlineAll;
        // outline.OutlineColor = Color.red;
        // outline.OutlineWidth = 30f;
    }

    public void Outline(bool outl){
        if(outl) {
            spriteRender.sprite = outlineSprite;
        } else {
            spriteRender.sprite = defaultSprite;
        }
    }

    public void TakeDamage(int amount){
        hp -= amount;
        Debug.Log("The imp takes " + amount + " damage!");
        if (hp < 0){
            Debug.Log("The imp dies!");
            Destroy(this.gameObject);
        }
    }
}
