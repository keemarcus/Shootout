using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    // reference to target prefab
    public GameObject target;
    // variable to reference the active target
    private GameObject currentTarget = null;
    // boundries for where targets can spawn
    public Vector2 spawnBoundTopLeft;
    public Vector2 spawnBoundBotRight;
    // time delay before the first target spawns
    public float spawnDelay;
    // bool to track if the first target has been spawned
    private bool firstTargetSpawned = false;
    // int to track how many times the player has clicked
    private int clickCount = 0;
    // int to track how many targets we've hit
    private int targetsHit = 0;

    void Update()
    {
        if(!firstTargetSpawned){
            if(Time.realtimeSinceStartup >= spawnDelay){
                SpawnTarget();
                firstTargetSpawned = true;
            }
        } else{
            if(Input.GetMouseButtonDown(0)){
                Click(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        
        
    }
    
    private void Click(Vector2 mousePos){
        clickCount += 1;

        // see if he hit the target
        if(Vector2.Distance(currentTarget.transform.position, mousePos) <= 0.75f){targetsHit += 1;}

        if(clickCount >= 3){
            EndGame();
        } else{
           // destroy the current target and spawn a new one
            SpawnTarget(); 
        }
    }

    private void SpawnTarget(){
        // destroy the current target if there is one
        if(currentTarget != null){Destroy(currentTarget);}
        
        // generate random spawn point for new target
        Vector2 spawnPoint = new Vector2(Random.Range(spawnBoundTopLeft.x, spawnBoundBotRight.x),Random.Range(spawnBoundBotRight.y, spawnBoundTopLeft.y));

        // insatantiate the new target at that point
        currentTarget = Instantiate(target, (Vector3)spawnPoint, Quaternion.identity);
    }

    private void EndGame(){
        // print our results to the console
        Debug.Log("Targets Hit: " + targetsHit + "\n" + "Time: " + (Time.realtimeSinceStartup - spawnDelay).ToString("f3"));

        // print the outcome to the console
        if(DetermineOutcome()){
            Debug.Log("HIT");
        } else{
            Debug.Log("MISS");
        }
        
        // exit game mode
        UnityEditor.EditorApplication.isPlaying = false;
    }

    private bool DetermineOutcome(){
        if(targetsHit >= Random.Range(0, 4)){
            // it's a hit
            return true;
        } else{
            // it's a miss
            return false;
        }
    }
}
