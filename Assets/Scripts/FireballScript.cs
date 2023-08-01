using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public Vector3 dir; //This is set by ItemScript when it is instantiated
    [SerializeField] public float moveSpeed;
    [SerializeField] public float damageMult;

    void Start(){ Debug.Log("Fireball says: SPAWNING"); }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Fireball says: " + dir);
        transform.position += new Vector3(dir.x, dir.y, dir.z) * Time.deltaTime * moveSpeed;
    }

    void OnTriggerEnter(Collider other){
        Debug.Log("Fireball says: HIT SOMETHING CALLED " + other.gameObject.name);
        if(other.gameObject.layer == LayerMask.NameToLayer("Unwalkable")){
            Detonate();
        } else if(other.gameObject.tag == "Enemy"){
            Debug.Log("Hit an ENEMY");
            if (other.gameObject.GetComponent<EnemyScript>().inPlayerDimension){
                GameState gameState = GameObject.Find("Game World Manager").GetComponent<GameState>();
                float baseDamage = gameState.GetDamageToEnemy();
                other.gameObject.GetComponent<EnemyScript>().TakeDamage((int)(baseDamage * damageMult));
                Debug.Log("In dimension!");
                Detonate();
            }
        }
    }

    void Detonate(){
        //Code for playing an explosion animation goes here
        Debug.Log("Fireball says: BLOWING UP");
        GameObject.Destroy(gameObject);
    }
}
