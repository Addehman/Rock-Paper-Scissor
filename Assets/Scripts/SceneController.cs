using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
	private static SceneController sceneController;
	public static SceneController Instance
	{
		get
		{
			if (sceneController == null)
				Debug.LogError("The SceneController is NULL.");
			return sceneController;
		}
	}


	private void Awake()
	{
		sceneController = this;
	}

	public void LoadScene(int number)
	{
		SceneManager.LoadScene(number);
	}

	public void LoadScene(string name)
	{
		SceneManager.LoadScene(name);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
