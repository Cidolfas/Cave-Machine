using UnityEngine;
using System.Collections;

public class AppStuff : MonoBehaviour {

	// Public inspector variables
	public KeyCode exit;
	public KeyCode toggleMouse;

	void Update ()
	{
		// Cursor lock
		if (Input.GetKeyDown (toggleMouse)) {
			Screen.lockCursor = !Screen.lockCursor;
		}

		// Exit
		if (Input.GetKeyDown(exit)) {
			Application.Quit();
		}
	}
}
