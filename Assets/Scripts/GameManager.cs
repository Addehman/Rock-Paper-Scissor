using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Database;
using Firebase.Auth;
using TMPro;

/*		NOTES
Problem: when logging in a second player, either nothing happens, or the first player loses its ID. 
The "games/" child in the database is overwritten with "p2moves" sometimes, seems that what writes "p2moves" at this point
writes it with a wrong in the path, forgetting to set in under

See line 441

*/


public class GameManager : MonoBehaviour
{
	private static GameManager gameManager;
	public static GameManager Instance 
	{
		get
		{
			if (gameManager == null)
				Debug.LogError("GameManager is NULL.");

			return gameManager;
		}
	}

	public event Action<int> OpponentChoice, UpdatePlayerScore, UpdateOpponentScore;
	public event Action ResetChoices;

	[SerializeField] private ButtonEmitter endTurnBtn, nextEmitter, rockEmitter, paperEmitter, scissorEmitter;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private OpponentController opponentController;
	[SerializeField] private TextMeshProUGUI player1NameUI, player2NameUI;

	public int playerScore, opponentScore, winScore;

	private PlayerAttackChoice playerCurrentChoice, playerRock = PlayerAttackChoice.Rock, playerPaper = PlayerAttackChoice.Paper, 
	playerScissor = PlayerAttackChoice.Scissor;
	private OpponentAttackChoice opponentCurrentChoice, opponentRock = OpponentAttackChoice.Rock, opponentPaper = OpponentAttackChoice.Paper, 
	opponentScissor = OpponentAttackChoice.Scissor;

	private string userID;

	private bool player2 = false;

	private UserInfo user;
	private GameInfo game;


	private void Awake()
	{
		gameManager = this;
	}

	private void Start()
	{
		playerScore = opponentScore = 0;
		winScore = 3;

		endTurnBtn.ChoiceSignal += EndTurn;
		nextEmitter.ChoiceSignal += NextRound;
		rockEmitter.AttackMoveSignal += MakeAttackMove;
		paperEmitter.AttackMoveSignal += MakeAttackMove;
		scissorEmitter.AttackMoveSignal += MakeAttackMove;

		//Ref for our userID
		userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

		//Tell the user what's happening
		HUDController.Instance.AnnounceThis("Loading data for: " + userID);

		//Load userInfo
		StartCoroutine(FirebaseManager.Instance.LoadData("users/" + userID, LoadedUser));
	}

	private void EndTurn()
	{
		endTurnBtn.gameObject.SetActive(false);
	}

	//process the user data
	private void LoadedUser(string jsonData)
	{
		HUDController.Instance.AnnounceThis("Processing user data: " + userID);
		//If we can't find any user data we need to create it
		if (jsonData == null || jsonData == "")
		{
			HUDController.Instance.AnnounceThis("No user data found, creating new user data...");

			user = new UserInfo();
			user.activeGame = "";
			StartCoroutine(FirebaseManager.Instance.SaveData("users/" + userID, JsonUtility.ToJson(user)));
		}
		else
		{	
			//we found user data
			user = JsonUtility.FromJson<UserInfo>(jsonData);
		}

		//We now have a user, let's check if our user has an active game.
		CheckedActiveGame();
	}

	//
	private void CheckedActiveGame()
	{
		//Does our user have an active game?
		if (user.activeGame == "" || user.activeGame == null)
		{
			//Start the new game process
			HUDController.Instance.AnnounceThis("No active game for the user, look for a game");
			StartCoroutine(FirebaseManager.Instance.CheckForGame("games/", NewGameLoaded));
		}
		else
		{
			//We already have a game, load it
			HUDController.Instance.AnnounceThis("Loading game: " + user.activeGame);
			StartCoroutine(FirebaseManager.Instance.CheckForGame("games/" + user.activeGame, GameLoaded));
		}
	}

	private void NewGameLoaded(string jsonData)
	{
		// We couldn't find a new game to join
		if (jsonData == "" || jsonData == null || jsonData == "{}")
		{
			//Create a unique ID for the new game
			string key = FirebaseDatabase.DefaultInstance.RootReference.Child("games/").Push().Key;
			string path = "games/" + key;

			//Create game structure
			var newGame = new GameInfo();
			newGame.player1 = userID;
			newGame.status = "new";
			newGame.gameID = key;

			//Save our new game
			StartCoroutine(FirebaseManager.Instance.SaveData(path, JsonUtility.ToJson(newGame)));

			HUDController.Instance.AnnounceThis("Creating new game: " + key);

			// Add the key to our active games
			user.activeGame = key;
			StartCoroutine(FirebaseManager.Instance.SaveData("users/" + userID, JsonUtility.ToJson(user)));

			GameLoaded(newGame);
		}
		else
		{
			//We found a game, lets join it
			print (jsonData);
			var game = JsonUtility.FromJson<GameInfo>(jsonData);

			//Update the game
			game.player2 = userID;
			game.status = "full";
			StartCoroutine(FirebaseManager.Instance.SaveData("games/" + game.gameID, JsonUtility.ToJson(game)));
			user.activeGame = game.gameID;
			StartCoroutine(FirebaseManager.Instance.SaveData("users/" + userID, JsonUtility.ToJson(user)));

			GameLoaded(game);
		}
	}

	private void GameLoaded(string jsonData)
	{
		Debug.Log(jsonData);

		if (jsonData == null || jsonData == "")
		{
			HUDController.Instance.AnnounceThis("No game data");
			Debug.LogError("Error while loading game data");
		}
		else 
		{
			GameLoaded(JsonUtility.FromJson<GameInfo>(jsonData));
		}
	}

	private void GameLoaded(GameInfo newGame)
	{
		HUDController.Instance.AnnounceThis("Game has been loaded");
		game = newGame;
		StartGame();
	}

	private void NextRound()
	{
		if (ResetChoices != null) ResetChoices();
		opponentController.SetSpriteToRock();
	}

	// private void PlayerWin()
	// {
	// 	print("Player wins!");
	// 	playerScore ++;
	// 	if (UpdatePlayerScore != null) UpdatePlayerScore(playerScore);
	// 	HUDController.Instance.AnnounceThis("Player Wins! \n"
	// 		+ playerCurrentChoice + " takes " + opponentCurrentChoice + "!");
	// }

	// private void PlayerLose()
	// {
	// 	print ("Player Lose!");
	// 	opponentScore ++;
	// 	if (UpdateOpponentScore != null) UpdateOpponentScore(opponentScore);
	// 	HUDController.Instance.AnnounceThis("Player Lose! \n"
	// 		+ playerCurrentChoice + " is taken by " + opponentCurrentChoice + "!");
	// }

	private void StartGame()
	{
		//Store our game in the class
		// game = newGame;

		//Check if we are player one or two
		if (game.player1 != userID)
			player2 = true;
		
		//Make sure we have move lists
		if (game.p1moves == null)
			game.p1moves = new List<Move>();
		
		if (game.p2moves == null)
			game.p2moves = new List<Move>();

		//Set player names
		player1NameUI.text = game.player1;
		player2NameUI.text = game.player2;

		//If we don't have a player 2
		if (game.player2 == "" || game.player2 == null)
			HUDController.Instance.AnnounceThis("Waiting for opponent to join...");
		
		//Listen to changes in our opponent moves:
		// if (player2)
		// 	FirebaseDatabase.DefaultInstance.GetReference("games/" + game.gameID).Child("p1moves").ValueChanged += UpdateGame;
		// else
		// 	FirebaseDatabase.DefaultInstance.GetReference("games/" + game.gameID).Child("p2moves").ValueChanged += UpdateGame;
		WaitForOpponentMove();
	}

	private void WaitForOpponentMove()
	{
		HUDController.Instance.AnnounceThis("Waiting for opponent to make a move...");
		//Listen to changes in our opponent moves:
		if (player2)
			FirebaseDatabase.DefaultInstance.GetReference("games/" + game.gameID).Child("p1moves").ValueChanged += UpdateGame;
		else
			FirebaseDatabase.DefaultInstance.GetReference("games/" + game.gameID).Child("p2moves").ValueChanged += UpdateGame;
	}

	//Will run when our game opponent makes a move
	public void UpdateGame(object sender, ValueChangedEventArgs args)
	{
		Debug.Log("Updating game");
		HUDController.Instance.AnnounceThis("Updating game..");

		if (args.DatabaseError != null)
		{
			Debug.LogError(args.DatabaseError.Message);
			return;
		}

		print ("args is this: " + args.Snapshot.GetRawJsonValue());  	//		<-----------   Snapshot gives nothing? There is an issue here!

		string jsonData = args.Snapshot.GetRawJsonValue();

		//// BEGIN UGLY HACK

		//convert our data to a list of moves
		//This can probably be done in a much better way

		if (jsonData == null || jsonData == "{}") return;

		print ("jsonData is this: " + jsonData);
		jsonData = jsonData.TrimStart('[');
		jsonData = jsonData.TrimEnd(']');
		var values = jsonData.Split(',');
		List<Move> moves = new List<Move>();
		foreach (var item in values)
			moves.Add((Move)int.Parse(item));
		print ("and jsonData becomes this: " + jsonData);

		//// END UGLY HACK

		//If we are player 2, we get the moves for player 1 and vice versa
		if (player2)
			game.p1moves = moves;
		else
			game.p2moves = moves;
		
		DisplayResults();
	}

	private void DisplayResults() 											// Getting errors for the end of this function, have a feeling that it could be because there is no data, also check line 248.
	{
		HUDController.Instance.AnnounceThis("Displaying results");

		//Calculate the number of completed rounds, lowest common amount of rounds
		int rounds = Mathf.Min(game.p1moves.Count, game.p2moves.Count);
		
		if (rounds  == 0) return;

		int p1score = 0;
		int p2score = 0;

		//Calculate scores
		for (int i = 0; i < rounds; i++)
		{
			p1score += CheckRound(game.p1moves[i], game.p2moves[i]);
			p2score += CheckRound(game.p2moves[i], game.p1moves[i]);
		}

		//Show scores in the UI
		HUDController.Instance.player1ScoreTxt.text = p1score.ToString();
		HUDController.Instance.player2ScoreTxt.text = p2score.ToString();

		//Show what happened
		HUDController.Instance.AnnounceThis(game.p1moves[rounds - 1].ToString() + " vs " + game.p2moves[rounds - 1].ToString());

		//Tell the user who won
		//If they are the same , it's a draw
		if (game.p1moves[rounds - 1] == game.p2moves[rounds - 1])
		{
			//Add a new line to the announcement
			HUDController.Instance.announcementsTxt.text += "\n the round is a Draw!";
		}
		else
		{
			if (player2)
			{
				//Check last round, who won. If you didn't win and it's not a draw, you lost.
				if (CheckRound(game.p2moves[rounds - 1], game.p1moves[rounds - 1]) == 1)
					HUDController.Instance.announcementsTxt.text += "\n You won the round!";
				else
					HUDController.Instance.announcementsTxt.text += "\n You lost the round!";
			}
			else
			{
				if (CheckRound(game.p1moves[rounds - 1], game.p2moves[rounds - 1]) == 1)
					HUDController.Instance.announcementsTxt.text += "\n You won the round!";
				else 
					HUDController.Instance.announcementsTxt.text += "\n You lost the round!";
			}
		}

		//Show status for the next round.
		if (player2)
		{
			if (game.p1moves.Count > game.p2moves.Count)
			{
				HUDController.Instance.AnnounceThis("Opponent is waiting for your move");
			}
			else if (game.p1moves.Count == game.p2moves.Count)
			{
				HUDController.Instance.AnnounceThis("New Round, make your move!");
			}
			else
			{
				HUDController.Instance.AnnounceThis("Waiting for opponent...");
			}
		}
		else
		{
			if (game.p2moves.Count > game.p1moves.Count)
			{
				HUDController.Instance.AnnounceThis("Opponent is waiting for your move");
			}
			else if (game.p2moves.Count == game.p1moves.Count)
			{
				HUDController.Instance.AnnounceThis("New Round, make your move!");
			}
			else
			{
				HUDController.Instance.AnnounceThis("Waiting for opponent...");
			}
		}

		//Check for victory
		//These are mainly getting started comments for you to work with
		if (p1score >= winScore)
		{
			//player 1 wins the game!

			//Announce the game completed
			HUDController.Instance.AnnounceThis("Game Completed! " + player1NameUI.text + " wins!");

			//If the other person has marked game as completed, we can remove it!
		}
		else if (p2score >= winScore)
		{
			//Same here as above but for player 2

			HUDController.Instance.AnnounceThis("Game Completed! " + player2NameUI.text + " wins!");
		}

		if(ResetChoices != null) ResetChoices();
	}

	//Return 1 if we win, 0 if it's a draw or loss.
	//This can be done super cool with bit-shift stuff.
	private int CheckRound(Move move1, Move move2)
	{
		if (move1 == Move.Rock && move2 == Move.Scissor)
			return 1;
		else if (move1 == Move.Scissor && move2 == Move.Paper)
			return 1;
		else if (move1 == Move.Paper && move2 == Move.Rock)
			return 1;
		
		return 0;
	}

	public void MakeAttackMove(int input)
	{
		//convert to Move enum
		var newMove = (Move)input;

		//Add our move to the list
		if (player2)
			game.p2moves.Add(newMove);
		else
			game.p1moves.Add(newMove);

		//Update our game
		DisplayResults();

		SaveGame();
	}

	private void SaveGame()
	{
		//Save our game to the database (so our opponent gets new data)
		Debug.Log("gameID is this: " + game.gameID);
		StartCoroutine(FirebaseManager.Instance.SaveData("games/" + game.gameID, JsonUtility.ToJson(game)));  // Check this out, maybe this path is wrong? When is this method being called?
	}

	//Remove our listeners when we go back into the menu or exit the game
	//(otherwise we will get errors and strange behaviours)
	public void OnDestroy()
	{
		if (player2)
			FirebaseDatabase.DefaultInstance.GetReference("games/" + game.gameID).Child("p1moves").ValueChanged -= UpdateGame;
		else
			FirebaseDatabase.DefaultInstance.GetReference("games/" + game.gameID).Child("p2moves").ValueChanged -= UpdateGame;
	}
}
