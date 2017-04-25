using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour {

	public float walkSpeed;
	public float turnSpeed;
	public float gravity;

	private Vector3 moveDirection = Vector3.zero;

	private Camera mainCamera;
	private CharacterController characterController;

	void Start() {
		Cursor.visible = false;
		mainCamera = Camera.main;
		characterController = GetComponent<CharacterController>();
	}

	void Update() {
		MoveCharacterController();
	}

	void MoveCharacterController() {
		// Turn head and body.
		float xMouse = Input.GetAxis("Mouse X");
		float yMouse = -Input.GetAxis("Mouse Y");
		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, xMouse, 0f) * turnSpeed);
		mainCamera.transform.rotation = Quaternion.Euler(mainCamera.transform.rotation.eulerAngles + new Vector3(yMouse, 0f, 0f) * turnSpeed);
		
		// Move body.
		if (characterController.isGrounded) {
			moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			moveDirection = transform.TransformDirection (moveDirection);
			moveDirection *= walkSpeed;
		}
		moveDirection.y += gravity * Time.deltaTime;

		characterController.Move(moveDirection * Time.deltaTime);
	}
}