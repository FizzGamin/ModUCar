using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private LayerMask playerMask;

    private bool jumpKeyWasPressed;
    private float horizontalInput;
    private Rigidbody rigidbodyComponent;
    private int superJumpsRemaining = 0;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpKeyWasPressed = true;
        }

        horizontalInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        rigidbodyComponent.velocity = new Vector3(horizontalInput, rigidbodyComponent.velocity.y, 0);

        if (jumpKeyWasPressed && Physics.OverlapSphere(groundCheckTransform.position, 0.1f, playerMask).Length != 0)
        {
            float jumpPower = superJumpsRemaining > 0 ? 8 : 5;
            if (jumpPower > 5) superJumpsRemaining--;
            rigidbodyComponent.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            jumpKeyWasPressed = false;
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
        {
            Destroy(other.gameObject);
            superJumpsRemaining++;
        }
    }

}
