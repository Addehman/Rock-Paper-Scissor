using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Firebase.Auth;
using System.Collections;
using TMPro;

public class FirebaseLogin : MonoBehaviour
{
	[SerializeField] private TMP_InputField userEmailInput, userPasswordInput;
	[SerializeField] private TMP_Text announcementsText;

	FirebaseDatabase db;
	// Firebase.FirebaseException firebaseEx;


	private void Start()
	{
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
		{
			if (task.Exception != null)
			{
				Debug.LogError(task.Exception);
			}

			db = FirebaseDatabase.DefaultInstance;
			// db.RootReference.Child("Hello").SetValueAsync("World");
		});

		announcementsText.text = null;

		userEmailInput.text = "test@test.test";
		userPasswordInput.text = "12345678";
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab) && userEmailInput.isFocused)
		{
			userPasswordInput.Select();
		}

		if (Input.GetKeyDown(KeyCode.Return))
		{
			StartLoginUser();
		}
	}

	public void StartRegisterUser()
	{
		StartCoroutine(RegUser(userEmailInput.text, userPasswordInput.text));
	}

	public void StartLoginUser()
	{
		StartCoroutine(SignIn(userEmailInput.text, userPasswordInput.text));
	}

	private IEnumerator RegUser(string email, string password)
	{
		Debug.Log("Starting Registration");
		announcementsText.text = "Starting Registration";

		var auth = FirebaseAuth.DefaultInstance;
		var regTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
		yield return new WaitUntil(() => regTask.IsCompleted);

		if (regTask.Exception != null)
		{
			Debug.LogWarning(regTask.Exception);
			announcementsText.text = regTask.Exception.InnerExceptions[0].InnerException.Message;
		}
		else
		{
			Debug.Log("Registration Successful!");
			announcementsText.text = "Registration Successful!";
			yield return new WaitForSeconds(1);
			SceneController.Instance.LoadScene("Game");
		}
	}

	private IEnumerator SignIn(string email, string password)
	{
		Debug.Log("Attempting to log in...");
		announcementsText.text = "Attempting to log in...";

		var auth = FirebaseAuth.DefaultInstance;
		var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
		yield return new WaitUntil(() => loginTask.IsCompleted);

		if (loginTask.Exception != null)
		{
			Debug.LogWarning(loginTask.Exception);
			announcementsText.text = loginTask.Exception.InnerExceptions[0].InnerException.Message;
		}
		else
		{
			Debug.Log("Login Successful!");
			announcementsText.text = "Login Successful!";
			// yield return StartCoroutine(DataTest(FirebaseAuth.DefaultInstance.CurrentUser.UserId, "I'm a Player!"));
			yield return new WaitForSeconds(1);
			SceneController.Instance.LoadScene("Game");
		}
		
	}

	// private IEnumerator DataTest(string userID, string data)
	// {
	// 	Debug.Log("Trying to write data to Database");
	// 	announcementsText.text = "Trying to write data to Database";

	// 	var db = FirebaseDatabase.DefaultInstance;
	// 	var dataTask = db.RootReference.Child("users").Child(userID).SetValueAsync(data);
	// 	yield return new WaitUntil(() => dataTask.IsCompleted);

	// 	if (dataTask.Exception != null)
	// 	{	
	// 		Debug.LogWarning(dataTask.Exception);
	// 		announcementsText.text = dataTask.Exception.InnerExceptions[0].InnerException.Message;
	// 	}
	// 	else
	// 	{
	// 		Debug.Log("Data Test-Write: Successful!");
	// 	}
	// }
}