//System
using System.Collections;
using System.Collections.Generic;

//UnityEngine
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Movement2D Movement2D;

    private void Start()
    {
        Movement2D = GetComponent<Movement2D>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        var xx = Input.GetAxisRaw("Horizontal");
        var yy = Input.GetAxisRaw("Vertical");

        Movement2D.MoveTo(new Vector3(xx, yy, 0));
    }
}
