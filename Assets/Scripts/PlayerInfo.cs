using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserInfo
{
	public string name;
	public string activeGame;
}

[Serializable]
public class GameInfo
{
	public string status;
	public string player1;
	public string player2;
	public string gameID;

	public List<Move> p1moves;
	public List<Move> p2moves;
}


public enum Move
{
	Rock,
	Paper,
	Scissor,
}