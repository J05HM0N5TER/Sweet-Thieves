using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XboxCtrlrInput;

public class DefaultButton : MonoBehaviour
{
	//this to make sure that there is always a button selected and 
	// so when the player presses the back button it defaults to the play button
	//

	// What the timer is currently at
	float controlTimer = 0f;
	// How long the wait before input works is
	public float controlCooldown = 0.2f;
	// How far the player has to move the stick before it is recognised
	public float stickDeadzone = 0.5f;
	// What button the player has to press to press a button
	public XboxButton selectButton = XboxButton.A;

	public GameObject defaultButton = null;

	// When the scrip is loaded it selects the default button
	private void OnEnable()
	{
		if (EventSystem.current != null)
		{
			EventSystem.current.SetSelectedGameObject(defaultButton);
		}
	}

	// Update is called once per frame
	void Update()
	{
		controlTimer += Time.unscaledDeltaTime;

		// This make sure that if a player always has a button selected
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			EventSystem.current.SetSelectedGameObject(defaultButton);
		}

		// Deal with menu navigation with controller
		if (controlTimer > controlCooldown)
		{
			float stickInputY = XCI.GetAxis(XboxAxis.LeftStickY);

			// If the player has moved the stick down
			if (stickInputY < -stickDeadzone)
			{
				MoveSelected(false);
			}
			// If the player has moved the stick up
			if (stickInputY > stickDeadzone)
			{
				MoveSelected(true);
			}
		}

		// If the player has pressed down the select button
		if (XCI.GetButtonDown(selectButton))
		{
			GameObject selected = EventSystem.current.currentSelectedGameObject;
			if (selected)
			{
				Button selectable = selected.GetComponent<Button>();
				if (selectable)
				{
					// Click on button
					selectable.onClick.Invoke();
				}
			}
		}
	}

	private void MoveSelected(bool isMovingUP)
	{
		GameObject selected = EventSystem.current.currentSelectedGameObject;
		// If what is currently selected is valid
		if (selected)
		{
			Selectable selectable = selected.GetComponent<Selectable>();
			// If what is currently selected has a selectable component
			if (selectable)
			{
				Navigation nav = selectable.navigation;
				// If it has a navigation
				if (nav.selectOnUp != null)
				{
					// Move up a button
					if (isMovingUP)
					{
						EventSystem.current.SetSelectedGameObject(nav.selectOnUp.gameObject);
					}
					// Move down a button
					else
					{
						EventSystem.current.SetSelectedGameObject(nav.selectOnDown.gameObject);
					}
					// Reset timer
					controlTimer = 0.0f;
				}
			}
		}
	}
}
