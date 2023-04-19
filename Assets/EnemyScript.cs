using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    int hp = 3;

    public void takeDamage(int amount){
        hp -= amount;
        Debug.Log("The imp takes " + amount + " damage!");
        if (hp < 0){
            Debug.Log("The imp dies!");
            Destroy(this.gameObject);
        }
    }
}
