using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine.AI;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;
using System.Buffers;

public class AI_Behaviour : MonoBehaviour
{
    public Ai_stats stats;
    public Rigidbody body;
    Vector3 pos;
    public Health health;
    int cooldown;

    [Header("Ai Movement ranges")]
    public float XEnd, ZEnd, minPlayerDist, maxPlayerDist;
    float steps;

    [Header("AI hostility")]
    public bool hostile, melee;
    public GameObject Projectile, projectSpawnObj;
    public Collider Hurtbox;
    public float clownProjectForcefulness, clownProjectileUp, timeFrame;
    public Transform actorToHarm;


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        Hurtbox = GetComponent<Collider>();
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (hostile)
        {

            case false:
                {
                    randomMove();
                    break;
                }
            case true:
                {
                    if (melee)
                    {
                        MoveTowardsPlayer();
                    }
                    else
                    {
                            randomMove();
                        

                    }
                    break;

                }
        }
    }
    public void calculate_speed()
    {
        if (stats.health < 50)
        {
            steps = (stats.speed * stats.SprintMultiplier) * Time.deltaTime;
        }
        else
        {
            steps = stats.speed * Time.deltaTime;
        }
    }
    void randomMove()
    {

        calculate_speed();

        if (Vector3.Distance(transform.position, pos) <= maxPlayerDist)
        {
            if(!melee && hostile)
            { LaunchProjectile();  }

            pos.x = Random.Range(-XEnd, XEnd);
            pos.z = Random.Range(-ZEnd, ZEnd);
            transform.LookAt(pos);
        }
        else
        {
            transform.position += transform.forward * steps;
        }


    }

    private void LaunchProjectile()
    {
        if (cooldown != timeFrame)
        {
            cooldown++;
            Debug.Log(cooldown);
        }
        else
        {
            transform.LookAt(actorToHarm.position);
            GameObject Clownjectile = Instantiate(Projectile, projectSpawnObj.transform.position, projectSpawnObj.transform.rotation);
            Rigidbody clownBody = Clownjectile.GetComponent<Rigidbody>();
            Debug.Log(clownBody);
            Vector3 clownForce = (projectSpawnObj.transform.forward * clownProjectForcefulness) + (transform.up * clownProjectileUp);
            clownBody.AddForce(clownForce, ForceMode.Impulse);
            cooldown = 0;
        }
    }

 
    void MoveTowardsPlayer()
    {
        transform.LookAt(actorToHarm);
        calculate_speed();

        if (Vector3.Distance(transform.position, actorToHarm.position) >= minPlayerDist)
        {
            transform.position += transform.forward * steps;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && health.currentHealth != 0)
        {
            health.currentHealth = health.currentHealth - stats.meleeDamage;
            if (health.currentHealth <= 0)
            {
                Destroy(GameObject.FindWithTag("Player"));
            }

        }
        else
        {
            Physics.IgnoreCollision(collision.collider, Hurtbox);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("Collision detected");
        if (!other.gameObject.CompareTag("Projectile")) { return; }
        //Debug.Log("Projectile detected");
        if(!gameObject.GetComponent<Ai_stats>()) { return; }

        gameObject.GetComponent<Ai_stats>().health -= WhoopiePieCannon.instance.weaponDamage;
    }
}
