using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Transform player;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = new Vector3(2.75f, 3.5f, -10f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(player.position.x + offset.x,offset.y, offset.z);
    }
}
