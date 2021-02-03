using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tree : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetBigger();
        }
    }

    public void GetBigger()
    {      
        //transform.localScale = new Vector3(2, 2, 2);
        transform.localScale = Vector3.one * 2;
    }
}
