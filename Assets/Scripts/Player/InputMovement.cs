using UnityEngine;
using System.Collections;

/// <summary>
/// Provides functionality to move a transform around with specified keyboard keys.
/// </summary>
public class InputMovement : MonoBehaviour 
{
	public Transform MovingObject;
	
	public KeyCode ForwardKey;
	public KeyCode BackwardKey;
	public KeyCode TurnLeftKey;
	public KeyCode TurnRightKey;
	public KeyCode StrafeLeftKey;
	public KeyCode StrafeRightKey;
	public KeyCode UpKey;
	public KeyCode DownKey;
	
	public float TranslationSpeed;
	public float RotationSpeed;
	
	void Update () 
	{
		Vector3 translation = Vector3.zero;
		
		if (Input.GetKey(ForwardKey))
			translation.z += TranslationSpeed * Time.deltaTime;		
		if (Input.GetKey(BackwardKey))
			translation.z -= TranslationSpeed * Time.deltaTime;
				
		if (Input.GetKey(StrafeRightKey))
			translation.x += TranslationSpeed * Time.deltaTime;
				if (Input.GetKey(StrafeLeftKey))
			translation.x -= TranslationSpeed * Time.deltaTime;
		
		if (Input.GetKey(UpKey))
			translation.y += TranslationSpeed * Time.deltaTime;		
		if (Input.GetKey(DownKey))
			translation.y -= TranslationSpeed * Time.deltaTime;
		
		MovingObject.Translate(translation);
		
		float rotation = 0;
		
		if (Input.GetKey(TurnRightKey))
			rotation += RotationSpeed * Time.deltaTime;
		if (Input.GetKey(TurnLeftKey))
			rotation -= RotationSpeed * Time.deltaTime;
		
		MovingObject.Rotate(MovingObject.up, rotation);
	}
}
