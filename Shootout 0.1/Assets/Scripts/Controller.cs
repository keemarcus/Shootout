using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    // time the game started
    private float startTime;
    // bool to track if the first target has been spawned
    private bool firstTargetSpawned = false;
    // bool to track if the game has started
    private bool started = false;
    // int to track how many times the player has clicked
    private int clickCount = 0;
    // int to track how many targets we've hit
    private int targetsHit = 0;
    // status indicator
    public Text status;

    void Update()
    {
        if(started){
            if(firstTargetSpawned){
                status.text = "GO";
            } else{
                status.text = Mathf.CeilToInt((spawnDelay + startTime) - Time.realtimeSinceStartup).ToString();
            }
            if(Input.GetMouseButtonDown(0)){Click(Camera.main.ScreenToWorldPoint(Input.mousePosition));}
            else if(!firstTargetSpawned && (Time.realtimeSinceStartup - startTime >= spawnDelay)){
                SpawnTarget();
                firstTargetSpawned = true;
            }
        }
        else{
            status.text = null;
        }
        // if(!started){
        //     if(Time.realtimeSinceStartup >= spawnDelay){
        //         SpawnTarget();
        //         started = true;
        //     }
        // } else{
        //     if(Input.GetMouseButtonDown(0)){
        //         Click(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //     }
        // } 
    }
    
    private void Click(Vector2 mousePos){
        clickCount += 1;

        // see if he hit the target
        if(currentTarget.GetComponent<Collider2D>().bounds.Contains(mousePos)){targetsHit += 1;}

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

    public void StartGame(){
        started = true;
        startTime = Time.realtimeSinceStartup;
    }

    private void EndGame(){
        float playerTime = Time.realtimeSinceStartup - (startTime+spawnDelay);
        int enemyHits = GenerateEnemyHits();
        float enemyTime = GenerateEnemyTime();

        bool playerOutcome;
        bool enemyOutcome;

        // destroy the current target
        if(currentTarget != null){Destroy(currentTarget);}

        // print our results to the console
        Debug.Log("Targets Hit: " + targetsHit + "\n" + "Time: " + playerTime.ToString("f3"));

        // print enemy results
        Debug.Log("Enemy Hits: " + enemyHits + "\n" + "Enemy Time: " + enemyTime.ToString("f3"));

        // first, generate the outcome of the person who shot first (player will win if they tied)
        //  then, if they hit, subtract 1 from the other person's hits and generate their outcome
        if(enemyTime < playerTime){
            enemyOutcome = DetermineOutcome(enemyHits);
            
            if(enemyOutcome && targetsHit > 0){targetsHit --;}
            playerOutcome = DetermineOutcome(targetsHit);
        } else{
            playerOutcome = DetermineOutcome(targetsHit);
            
            if(playerOutcome && enemyHits > 0){enemyHits --;}
            enemyOutcome = DetermineOutcome(enemyHits);
        }

        // print the new hit totals
        Debug.Log("Player Hits: " + targetsHit + "\n" + "Enemy Hits: " + enemyHits);

        // print the outcome to the console
        Debug.Log("Player Hit? - " + playerOutcome + "\n" + "Enemy Hit? - " + enemyOutcome);

        // reset all the relevant variables
        targetsHit = 0;
        clickCount = 0;
        started = false;
        firstTargetSpawned = false;
    }

    public void Exit(){
        // exit game mode
        UnityEditor.EditorApplication.isPlaying = false;
    }

    private bool DetermineOutcome(int hits){
        if(hits >= Random.Range(0, 4)){
            // it's a hit
            return true;
        } else{
            // it's a miss
            return false;
        }
    }

    private float GenerateEnemyTime(){
        return Random.Range(1f,2f);
    }

    private int GenerateEnemyHits(){
        // generate a random number 0-3, more weighted to 1-2
        int a = Random.Range(0,5);
        int b = Random.Range(0,5);
        int c = Random.Range(0,5);
        int d = Random.Range(0,5);
        int result = Mathf.CeilToInt((a + b + c + d)/4);
        if(result > 3){result = 3;}

        return result;
    }
    // private void Start() {
    //     // generate a bunch of enemyhits
    //     float total = 0;
    //     for(int i=0; i<100; i++){
    //         total += GenerateEnemyTime();
    //     }
    //     Debug.Log((total/100).ToString("f3"));
    // }
}
