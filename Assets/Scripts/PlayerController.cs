using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	
	//mouse variables
	private float rotY = 0f;
	private const float rotationStrength = 1.0f;
	// private Quaternion q = new Quaternion(0, 0, 0, Quaternion.Identity);
	
	private void Start() {
		//set rotX for mouse rotation
		rotY = transform.localRotation.eulerAngles.y;
	}
	
	private void Update() {
		
		//turn player with mouse
		float mouseX = Input.GetAxis("Mouse X");
		rotY += mouseX * PlayerSettings.mouseSpeed * Time.deltaTime;
		// q = Quaternion.Euler(0f, rotY, 0f);
		transform.rotation = Quaternion.Euler(0f, rotY, 0f);
	}
}