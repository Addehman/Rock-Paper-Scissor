using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentController : MonoBehaviour
{
	[SerializeField] private Sprite rockSprite, paperSprite, scissorSprite;
	[SerializeField] private ButtonEmitter endTurnEmitter;

	public SpriteRenderer spriteRenderer;
	public OpponentAttackChoice choice = OpponentAttackChoice.Rock;

	private int rockID = 0, paperID = 1, scissorID = 2;
	

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
			SetSpriteToRock();
		if (Input.GetKeyDown(KeyCode.Alpha2))
			SetSpriteToPaper();
		if (Input.GetKeyDown(KeyCode.Alpha3))
			SetSpriteToScissor();
	}

	public void OpponentRandomChoice()
	{
		int randomOpponentChoice = (int)Random.Range(0, 3);

		switch (randomOpponentChoice) 
		{
			case 0:
				choice = OpponentAttackChoice.Rock;
				SetSpriteToRock();
				break;

			case 1:
				choice = OpponentAttackChoice.Paper;
				SetSpriteToPaper();
				break;

			case 2:
				choice = OpponentAttackChoice.Scissor;
				SetSpriteToScissor();
				break;
		}
	}

	public void SetSpriteToRock()
	{
		spriteRenderer.sprite = rockSprite;
	}

	public void SetSpriteToPaper()
	{
		spriteRenderer.sprite = paperSprite;
	}

	public void SetSpriteToScissor()
	{
		spriteRenderer.sprite = scissorSprite;
	}
}

public enum OpponentAttackChoice
{
	Rock,
	Paper,
	Scissor,
}