using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;

    [Header("Jump")]
    public float jumpAccel;

    private bool isJumping;
    private bool isOnGround;

    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;

    //Scoring
    [Header("Scoring")]
    public ScoreController score;
    public float scoringRatio;
    private float lastPositionX;

    //Game Over
    [Header("GameOver")]
    public GameObject gameOverScreen;
    public float fallPositionY;

    [Header("Camera")]
    public CameraMoveController gameCamera;

    //Get RigidBody2D Char
    private Rigidbody2D rig;

    //create Animator
    private Animator anim;   

    //Get charactersoundcontroller
    private CharacterSoundController sound;

    // Start is called before the first frame update
    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundController>();
    }

    // Update is called once per frame
    private void Update()
    {
        
        //Read Input
        if (Input.GetMouseButtonDown(0))
        {
            if (isOnGround)
            {
                isJumping = true;

                sound.PlayJump();
            }
        }

        anim.SetBool("isOnGround", isOnGround);

        //Kalkulasi Score
        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

        if (scoreIncrement > 0)
        {
            score.IncreaseCurrentScore(scoreIncrement);
            lastPositionX += distancePassed;
        }

        //Game Over
        if (transform.position.y < fallPositionY)
        {
            GameOver();
        }

    }

    //Move Char to Right
    private void FixedUpdate() 
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, groundLayerMask);

        if (hit)
        {

            if (!isOnGround && rig.velocity.y <+ 0)
            {
                isOnGround = true;
            }

        }else
        {
            isOnGround = false;
        }

        Vector2 velocityVector = rig.velocity;

        if (isJumping)
        {
            velocityVector.y += jumpAccel;
            isJumping = false;
        }

        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);

        rig.velocity = velocityVector;
    }

    private void GameOver()
    {
        //Get High Score
        score.FinishScoring();

        //Camera Stop
        gameCamera.enabled = false;

        //Show UI Game Over
        gameOverScreen.SetActive(true);

        this.enabled = false;
    }

    private void OnDrawGizmos() 
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.white);
    }
}
