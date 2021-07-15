using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 10f;
    public Transform player1;
    public GameObject player1Flag;
    public Player2Movement player2Flag;

    float prevAngle = 0;
    Rigidbody rigidBody;
    Vector3 velocity;
    
    private bool inPossession = false;
    private float speedReduction = .7f;
    private int score = 0;
    private bool onHomeBase = false;

    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        player1 = Instantiate(player1, new Vector3(-44.4f, 2f, -49.4f), Quaternion.identity);
        //player2 = Instantiate(player2, new Vector3(41f, 2f, 43f), Quaternion.identity);
        player1.rotation = Quaternion.Euler(0,30,0);
        //player1.rotation = Quaternion.Euler(0,-140,0);
        player1Flag = Instantiate(player1Flag, new Vector3(-46f, 1.5f, -52f), Quaternion.identity);
        player2Flag = FindObjectOfType(typeof(Player2Movement)) as Player2Movement;
        //player2Flag = Instantiate(player2Flag, new Vector3(50f, 1.5f, 45f), Quaternion.identity);
    }

    void Update() {
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow)) {
            MovePlayer();
        } else {
            velocity = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) {
            RotatePlayer();
        }
        Captured();
    }

    void MovePlayer() {
        velocity = transform.forward * Input.GetAxisRaw("Vertical") * speed;
    }

    void RotatePlayer() {
        Vector3 inputDir = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            0).normalized;

        prevAngle += Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
        float inputAngle = prevAngle;

        transform.rotation = Quaternion.Euler(0, inputAngle / 70f, 0);
    }

    void FixedUpdate() {
        rigidBody.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider trigger) {
        if (trigger.gameObject.tag == "Flag2") {
            trigger.transform.parent = transform;
            trigger.transform.localPosition = new Vector3(0,0,-1.2f);//.4f,-.45f,.3f);
            inPossession = true;
            speed = speedReduction * speed;
        }
        if (trigger.gameObject.tag == "Flag") {
            respawnFlag();
        }
        if (trigger.gameObject.tag == "Base1") {
            onHomeBase = true;
        } else {
            onHomeBase = false;
        }
    }

    void Captured() {
        if (inPossession && onHomeBase) {
                inPossession = false;
                score++;
                speed = speed / speedReduction;
                player2Flag.respawnFlag();
            }
    }

    public void respawnFlag() {
        Destroy(player1Flag);
        player1Flag = Instantiate(player1Flag, new Vector3(-46f, 1.5f, -52f), Quaternion.identity);
    }
}