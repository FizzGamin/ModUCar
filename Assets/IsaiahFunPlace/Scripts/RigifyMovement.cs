using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigifyMovement : MonoBehaviour
{
    private Animator animator;
    private string currentState;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move forward animation
        if (Input.GetKey(KeyCode.W))
            animator.SetBool("isWalkForward", true);
        else
            animator.SetBool("isWalkForward", false);

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 20);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * Time.deltaTime * 20);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Time.deltaTime * 20);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Time.deltaTime * 20);
        }

    }
}
