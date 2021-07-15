using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Movement : MonoBehaviour {

    public float speed = 10f;
    public Transform player2;
    public GameObject player2Flag;
    public PlayerMovement player1Flag;

    float prevAngle = 0;
    Rigidbody rigidBody;
    Vector3 velocity;
    
    private bool inPossession = false;
    private float speedReduction = .7f;
    private int score = 0;
    private bool onHomeBase = false;

    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        player2 = Instantiate(player2, new Vector3(41f, 2f, 43f), Quaternion.identity);
        player2.rotation = Quaternion.Euler(0,-140,0);
        player2Flag = Instantiate(player2Flag, new Vector3(50f, 1.5f, 45f), Quaternion.identity);
        player1Flag = FindObjectOfType(typeof(PlayerMovement)) as PlayerMovement;
    }

    void Update() {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W)) {
            MovePlayer();
        } else {
            velocity = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
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
        if (trigger.gameObject.tag == "Flag") {
            trigger.transform.parent = transform;
            trigger.transform.localPosition = new Vector3(0,0,-1.2f);//.4f,-.45f,.3f);
            inPossession = true;
            speed = speedReduction * speed;
        }
        if (trigger.gameObject.tag == "Flag2") {
            respawnFlag();
        }
        if (trigger.gameObject.tag == "Base2") {
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
                player1Flag.respawnFlag();
            }
    }

    public void respawnFlag() {
        Destroy(player2Flag);
        player2Flag = Instantiate(player2Flag, new Vector3(-46f, 1.5f, -52f), Quaternion.identity);
    }
}