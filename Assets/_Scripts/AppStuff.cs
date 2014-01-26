using UnityEngine;
using System.Collections;

public class AppStuff : MonoBehaviour {

	public KeyCode exit;
	public KeyCode toggleMouse;

	void Update ()
	{
		if (Input.GetKeyDown (toggleMouse)) {
			Screen.lockCursor = !Screen.lockCursor;
		}

		if (Input.GetKeyDown(exit)) {
			Application.Quit();
		}
	}
}
