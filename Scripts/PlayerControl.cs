using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
       private Rigidbody2D rb;
    private Vector2 direction; 

    public float moveSpeed =4f;
    public float jumpStrength = 10f;

    private bool grounded; //bool check for if on ground or the ladder
    private bool climbing; // bool check for climbing 


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
       
    }

    // Update is called once per frame
    private void Update()
    {
        
        directionCheck(); 
    }

    private void directionCheck()
    {
        if (climbing) {
            direction.y = Input.GetAxis("Vertical") * moveSpeed;   //If the player is on stairs, it will move upwards. 
        } 
        else if (Input.GetKeyDown(KeyCode.Space)) {
            direction = Vector2.up * jumpStrength;   //when on ground and pscae key pressed, the player will jump. 
        } 
        else {
            direction += Physics2D.gravity * Time.deltaTime;
        }

        direction.x = Input.GetAxis("Horizontal") * moveSpeed;   //Horizontal input

        

        //change the way player looks (left or right )
        if (direction.x > 0f) {
            transform.eulerAngles = Vector3.zero;   //if right, that is defualt so no need to change 
        } else if (direction.x < 0f) {
            transform.eulerAngles = new Vector3(0f, 180f, 0f); // if left, the player needs to turn 180
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            //Play Particle system 
        }
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * Time.fixedDeltaTime); //MovePosition to move the player every frame
    }
}
