using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Firebase.Auth;

public class FirebaseManager : MonoBehaviour
{
	private static FirebaseManager firebaseManager;
	public static FirebaseManager Instance
	{
		get
		{
			if (firebaseManager == null)
				Debug.LogError("The FirebaseManager is NULL.");
			return firebaseManager;
		}
	}

	public delegate void OnLoadedDelegate(string jsonData);
	public delegate void OnSaveDelegate();
	
	[SerializeField] private PlayerController playerController;
	private ButtonEmitter endTurnBtn;

	FirebaseDatabase db;


	private void Awake()
	{
		firebaseManager = this;

		db = FirebaseDatabase.DefaultInstance;
	}

	// public void StartSendPlayerAttackChoice()
	// {
	// 	StartCoroutine(CoSendPlayerAttackChoice(FirebaseAuth.DefaultInstance.CurrentUser.UserId, playerController.choice.ToString()));
	// }

	// public IEnumerator CoSendPlayerAttackChoice(string userID, string attackChoice)
	// {
	// 	Debug.Log("Sending Player's AttackChoice to Database");
	// 	HUDController.Instance.announcementsTxt.text = "Sending Player's Move to Database";

	// 	var _db = FirebaseDatabase.DefaultInstance;
	// 	var sendAttackTask = _db.RootReference.Child("users").Child(userID).Child("AttackChoice").SetValueAsync(attackChoice);
	// 	yield return new WaitUntil(() => sendAttackTask.IsCompleted);

	// 	if (sendAttackTask.Exception != null)
	// 	{
	// 		Debug.LogWarning(sendAttackTask.Exception);
	// 		HUDController.Instance.AnnounceThis(sendAttackTask.Exception.InnerExceptions[0].InnerException.Message);
	// 	}
	// 	else
	// 	{
	// 		Debug.LogWarning("Database received Player's AttackChoice");
	// 		HUDController.Instance.AnnounceThis("Database received Player's AttackChoice");
	// 	}
	// }

	//Loads the data at "path" then returns json result to the delegate/callback function
	public IEnumerator LoadData(string path, OnLoadedDelegate onLoadedDelegate)
	{
		var dataTask = db.RootReference.Child(path).GetValueAsync();
		yield return new WaitUntil(() => dataTask.IsCompleted);

		if (dataTask.Exception != null)
		{
			Debug.LogWarning(dataTask.Exception);
			HUDController.Instance.AnnounceThis(dataTask.Exception.InnerExceptions[0].InnerException.Message);
		}
		
		string jsonData = dataTask.Result.GetRawJsonValue();

		onLoadedDelegate(jsonData);
	}

	//Save data to our database
	public IEnumerator SaveData(string path, string data, OnSaveDelegate onSaveDelegate = null)
	{
		var dataTask = db.RootReference.Child(path).SetRawJsonValueAsync(data);
		yield return new WaitUntil(() => dataTask.IsCompleted);

		if (dataTask.Exception != null)
		{
			Debug.LogWarning(dataTask.Exception);
			HUDController.Instance.AnnounceThis(dataTask.Exception.InnerExceptions[0].InnerException.Message);
		}

		if (onSaveDelegate != null)
			onSaveDelegate();
	}

	public IEnumerator CheckForGame(string path, OnLoadedDelegate onLoadedDelegate = null)
	{
		Debug.Log("Checking for game");
		HUDController.Instance.AnnounceThis("Checking for game");

		var dataTask = db.GetReference("games").OrderByChild("status").EqualTo("new").GetValueAsync();
		yield return new WaitUntil(() => dataTask.IsCompleted);

		string jsonData = dataTask.Result.GetRawJsonValue();

		Debug.Log("game data: " + jsonData);
		HUDController.Instance.AnnounceThis("game data: " + jsonData);

		if (dataTask.Exception != null)
		{
			Debug.LogWarning(dataTask.Exception);
			HUDController.Instance.AnnounceThis(dataTask.Exception.InnerExceptions[0].InnerException.Message);
		}

		if (dataTask.Result.ChildrenCount > 0)
		{
			foreach (var item in dataTask.Result.Children)
			{
				Debug.Log("multiple data found: " + item.GetRawJsonValue());
				HUDController.Instance.AnnounceThis("multiple data found: " + item.GetRawJsonValue());

				onLoadedDelegate(item.GetRawJsonValue());
				break;
			}
		}
		else
		{
			onLoadedDelegate(jsonData);
		}
	}
}