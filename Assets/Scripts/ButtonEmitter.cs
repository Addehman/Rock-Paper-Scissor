using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;


public class ButtonEmitter : MonoBehaviour
{
	public event Action ChoiceSignal, NextSignal;
	public event Action<int> AttackMoveSignal;
	
	[SerializeField] private UnityEvent OnEndTurn;
	[SerializeField] private ButtonTypes type = ButtonTypes.RockType;
	[SerializeField] private PlayerController playerController;

	private enum ButtonTypes
	{
		RockType, PaperType, ScissorType, RegretChoice, EndTurnChoice, NextBtn,
	}

	
	private void Start()
	{
		if (type == ButtonTypes.RegretChoice)
			gameObject.SetActive(false);
		else if (type == ButtonTypes.EndTurnChoice)
			gameObject.SetActive(false);
		else if (type == ButtonTypes.NextBtn)
			gameObject.SetActive(false);
	}

	private void OnMouseDown()
	{
		if (Input.GetMouseButtonDown(0) && type == ButtonTypes.RockType)
		{
			if (ChoiceSignal != null) ChoiceSignal();
		}

		if (Input.GetMouseButtonDown(0) && type == ButtonTypes.PaperType)
		{
			if (ChoiceSignal != null) ChoiceSignal();
		}

		if (Input.GetMouseButtonDown(0) && type == ButtonTypes.ScissorType)
		{
			if (ChoiceSignal != null) ChoiceSignal();
		}

		if (Input.GetMouseButtonDown(0) && type == ButtonTypes.RegretChoice)
		{
			if (ChoiceSignal != null) ChoiceSignal();
		}

		if (Input.GetMouseButtonDown(0) && type == ButtonTypes.EndTurnChoice)
		{
			if (ChoiceSignal != null) ChoiceSignal();
			GameManager.Instance.MakeAttackMove(playerController.attackChoiceNumber);
			OnEndTurn.Invoke();
		}

		if (Input.GetMouseButtonDown(0) && type == ButtonTypes.NextBtn)
		{
			if (ChoiceSignal != null) ChoiceSignal();
			if (NextSignal != null) NextSignal();
		}
	}
}
