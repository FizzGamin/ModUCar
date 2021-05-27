using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DistToFinish : MonoBehaviour
{
    TMP_Text totalDist;
    TMP_Text distTraveled;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        totalDist = gameObject.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        distTraveled = gameObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();

        player = GameObject.FindGameObjectWithTag("Player");

        totalDist.text = 35000 + "";
    }

    // Update is called once per frame
    void Update()
    {
        distTraveled.text = Vector3.Distance(player.transform.position, Vector3.zero) + "";
    }
}
