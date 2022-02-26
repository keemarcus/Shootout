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
    // int to track how many times the player has clicked
    private int clickCount = 0;
    // int to track how many targets we've hit
    private int targetsHit = 0;
    // status indicator
    public Text status;
    private GameState gameState = GameState.Idle;
    private CharacterState playerState = CharacterState.Healthy;
    private CharacterState enemyState = CharacterState.Healthy;
    public Animator playerAnim;
    public Animator enemyAnim;

    void Update()
    {
        switch(gameState){
            case GameState.Idle:
                // status.text = null;
                break;
            case GameState.Active:
                UpdateTimer(firstTargetSpawned);
                if(firstTargetSpawned && Input.GetMouseButtonDown(0)){Click(Camera.main.ScreenToWorldPoint(Input.mousePosition));}
                else if(!firstTargetSpawned && (Time.realtimeSinceStartup - startTime >= spawnDelay)){
                    SpawnTarget();
                    firstTargetSpawned = true;
                }
                break;
            default:
                // status.text = null;
                break;
        }

        switch(playerState){
            case CharacterState.Healthy:
                playerAnim.SetFloat("State", 0f);
                break;
            case CharacterState.Wounded:
                playerAnim.SetFloat("State", .5f);
                break;
            case CharacterState.Dead:
                playerAnim.SetFloat("State", 1f);
                break;
        }

        switch(enemyState){
            case CharacterState.Healthy:
                enemyAnim.SetFloat("State", 0f);
                break;
            case CharacterState.Wounded:
                enemyAnim.SetFloat("State", .5f);
                break;
            case CharacterState.Dead:
                enemyAnim.SetFloat("State", 1f);
                break;
        }
    }
    private void UpdateTimer(bool active){
        if(active){
            status.text = "GO";
        } else{
            status.text = Mathf.CeilToInt((spawnDelay + startTime) - Time.realtimeSinceStartup).ToString();
        }
    }
    
    private void Click(Vector2 mousePos){
        clickCount += 1;

        // see if he hit the target
        if(currentTarget != null && currentTarget.GetComponent<Collider2D>().bounds.Contains(mousePos)){targetsHit += 1;}

        if(clickCount >= 3){
            DetermineOutcome();
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
        gameState = GameState.Active;
        startTime = Time.realtimeSinceStartup;

        // reset all the relevant variables
        targetsHit = 0;
        clickCount = 0;
        playerState = CharacterState.Healthy;
        enemyState = CharacterState.Healthy;
        firstTargetSpawned = false;
    }

    private void DetermineOutcome(){
       float playerTime = Time.realtimeSinceStartup - (startTime+spawnDelay);
        int enemyHits = GenerateEnemyHits();
        float enemyTime = GenerateEnemyTime();

        bool playerOutcome;
        bool enemyOutcome;

        // destroy the current target
        if(currentTarget != null){Destroy(currentTarget);}

        // print our results to the console
        // Debug.Log("Targets Hit: " + targetsHit + "\n" + "Time: " + playerTime.ToString("f3"));

        // print enemy results
        // Debug.Log("Enemy Hits: " + enemyHits + "\n" + "Enemy Time: " + enemyTime.ToString("f3"));

        // first, generate the outcome of the person who shot first (player will win if they tied)
        //  then, if they hit, subtract 1 from the other person's hits and generate their outcome
        if(enemyTime < playerTime){
            // determine the enemy outcome
            enemyOutcome = CheckForHit(enemyHits);
            // injure the player if they hit
            if(enemyOutcome){
                // if they were killed then the game is over
                if(InjureCharacter("Player")){
                    return;
                }
            }
            
            // determine the player outcome
            playerOutcome = CheckForHit(targetsHit);
            // injure the enemy if they hit
            if(playerOutcome){
                // if they were killed then the game is over
                if(InjureCharacter("Enemy")){
                    return;
                }
            }
        } else{
            // determine the enemy outcome
            enemyOutcome = CheckForHit(enemyHits);
            // injure the player if they hit
            if(enemyOutcome){
                // if they were killed then the game is over
                if(InjureCharacter("Player")){
                    return;
                }
            }
            
            // determine the player outcome
            playerOutcome = CheckForHit(targetsHit);
            // injure the enemy if they hit
            if(playerOutcome){
                // if they were killed then the game is over
                if(InjureCharacter("Enemy")){
                    return;
                }
            }
        } 

        // subtract one from the characters hit totals if they're wounded
        if(playerState == CharacterState.Wounded && targetsHit > 0){targetsHit --;}
        if(enemyState == CharacterState.Wounded && enemyHits > 0){enemyHits --;}

        // print the final hit totals
        // Debug.Log("Player Hits: " + targetsHit + "\n" + "Enemy Hits: " + enemyHits);

        // print the outcome to the console
        // Debug.Log("Player Hit? - " + playerOutcome + "\n" + "Enemy Hit? - " + enemyOutcome);

        // if we made it to this point then neither character is dead, so restart
        Restart();
    }

    private void Restart(){
        gameState = GameState.Active;
        startTime = Time.realtimeSinceStartup;

        // reset all the relevant variables
        targetsHit = 0;
        clickCount = 0;
        firstTargetSpawned = false;
    }

    private void EndGame(string winner){
        // print the final outcome
        // Debug.Log(winner + " Won");
        status.text = winner + " Won";

        gameState = GameState.Idle;
    }

    public void Exit(){
        // exit game mode
        UnityEditor.EditorApplication.isPlaying = false;
    }

    private bool CheckForHit(int hits){
        if(hits >= Random.Range(0, 4)){
            // it's a hit
            return true;
        } else{
            // it's a miss
            return false;
        }
    }

    private bool InjureCharacter(string character){
        // determine if the shot is fatal
        if(Random.Range(0,2) > 0){
            // end the game if it was fatal
            if(character == "Player"){
                playerState = CharacterState.Dead;
                Debug.Log("Player is dead");
                EndGame("Enemy");
            } else if(character == "Enemy"){
                enemyState = CharacterState.Dead;
                Debug.Log("Enemy is dead");
                EndGame("Player");
            } 
            return true;
        } else{
            // change the character's state to wounded if it wasn't fatal
            if(character == "Player"){
                playerState = CharacterState.Wounded;
                Debug.Log("Player is wounded");
            } else if(character == "Enemy"){
                enemyState = CharacterState.Wounded;
                Debug.Log("Enemy is wounded");
            }
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

    private enum GameState{
        Idle,
        Active
    }
    private enum CharacterState{
        Healthy,
        Wounded,
        Dead
    }
}
