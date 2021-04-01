using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
	private static HUDController hudController;
	public static HUDController Instance
	{
		get
		{
			if (hudController == null)
				Debug.LogError("The HUDController is NULL.");
			
			return hudController;
		}
	}

	[SerializeField] private ButtonEmitter rockEmitter, paperEmitter, 
	scissorEmitter;

	public TMP_Text player1ScoreTxt, player2ScoreTxt, announcementsTxt;


	private void Awake()
	{
		hudController = this;
	}

	private void Start()
	{
		GameManager.Instance.UpdatePlayerScore += UpdatePlayerScore;
		GameManager.Instance.UpdateOpponentScore += UpdateOpponentScore;
		GameManager.Instance.ResetChoices += ClearAnnouncementTxt;
		rockEmitter.ChoiceSignal += ClearAnnouncementTxt;
		paperEmitter.ChoiceSignal += ClearAnnouncementTxt;
		scissorEmitter.ChoiceSignal += ClearAnnouncementTxt;

		StartCoroutine(CoRemoveLetsPlayMsgAfterAFewSeconds());
	}

	private IEnumerator CoRemoveLetsPlayMsgAfterAFewSeconds()
	{
		yield return new WaitForSeconds(2);
		AnnounceThis("Choose your Weapon!\nRock, Paper or Scissor!");
	}

	public void ClearAnnouncementTxt()
	{
		AnnounceThis("");
	}

	private void UpdatePlayerScore(int score)
	{
		player1ScoreTxt.text = score.ToString();
	}

	private void UpdateOpponentScore(int score)
	{
		player2ScoreTxt.text = score.ToString();
	}

	public void AnnounceThis(string announcement)
	{
		announcementsTxt.text = announcement;
	}
}
