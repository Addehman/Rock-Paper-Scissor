using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackAlternativeHandler : MonoBehaviour
{
	public event Action Rock, Paper, Scissor;

	[SerializeField] private ButtonEmitter rockEmitter, paperEmitter, 
	scissorEmitter, regretChoiceEmitter, endTurnEmitter, nextEmitter;
	[SerializeField] private GameManager gameManager;
	

	private void Start()
	{
		rockEmitter.ChoiceSignal += DoRock;
		paperEmitter.ChoiceSignal += DoPaper;
		scissorEmitter.ChoiceSignal += DoScissor;
		regretChoiceEmitter.ChoiceSignal += ResetChoices;
		endTurnEmitter.ChoiceSignal += DoEndTurn;
		nextEmitter.ChoiceSignal += ResetChoices;
		gameManager.ResetChoices += ResetChoices;
	}

	private void DoRock()
	{
		if (Rock != null) Rock();
		rockEmitter.gameObject.SetActive(false);
		paperEmitter.gameObject.SetActive(false);
		scissorEmitter.gameObject.SetActive(false);

		regretChoiceEmitter.gameObject.SetActive(true);
		endTurnEmitter.gameObject.SetActive(true);
	}

	private void DoPaper()
	{
		if (Paper != null) Paper();
		rockEmitter.gameObject.SetActive(false);
		paperEmitter.gameObject.SetActive(false);
		scissorEmitter.gameObject.SetActive(false);

		regretChoiceEmitter.gameObject.SetActive(true);
		endTurnEmitter.gameObject.SetActive(true);
	}

	private void DoScissor()
	{
		if (Scissor != null) Scissor();
		rockEmitter.gameObject.SetActive(false);
		paperEmitter.gameObject.SetActive(false);
		scissorEmitter.gameObject.SetActive(false);

		regretChoiceEmitter.gameObject.SetActive(true);
		endTurnEmitter.gameObject.SetActive(true);
	}

	public void ResetChoices()
	{
		if (Rock != null) Rock();
		rockEmitter.gameObject.SetActive(true);
		paperEmitter.gameObject.SetActive(true);
		scissorEmitter.gameObject.SetActive(true);

		regretChoiceEmitter.gameObject.SetActive(false);
		endTurnEmitter.gameObject.SetActive(false);
		nextEmitter.gameObject.SetActive(false);
	}

	private void DoEndTurn()
	{
		regretChoiceEmitter.gameObject.SetActive(false);
		endTurnEmitter.gameObject.SetActive(false);
	}
}
