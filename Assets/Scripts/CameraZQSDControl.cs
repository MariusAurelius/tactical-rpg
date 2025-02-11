using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWASDMovement : MonoBehaviour 
{
  // This is expressed in "units per second".
  public float speed = 1f;

  void Update() 
{
    if (Input.GetKey(KeyCode.Q)) {
      transform.position += Vector3.left * Time.deltaTime * speed;
    }
    if (Input.GetKey(KeyCode.D)) {
      transform.position += Vector3.right * Time.deltaTime * speed;
    }
    if (Input.GetKey(KeyCode.Z)) {
      transform.position += Vector3.forward * Time.deltaTime * speed;
    }
    if (Input.GetKey(KeyCode.S)) {
      transform.position += Vector3.back * Time.deltaTime * speed;
    }
  }
}