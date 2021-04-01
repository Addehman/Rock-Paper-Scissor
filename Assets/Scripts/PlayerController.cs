using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Sprite rockSprite;
	[SerializeField] private Sprite paperSprite;
	[SerializeField] private Sprite scissorSprite;
	[SerializeField] private AttackAlternativeHandler attackHandler;
	[SerializeField] private SpriteRenderer spriteRenderer;

	public PlayerAttackChoice choice = PlayerAttackChoice.Rock;
	public int attackChoiceNumber = 0;


	private void Start()
	{
		choice = PlayerAttackChoice.Rock;
		attackHandler.Rock += SetSpriteToRock;
		attackHandler.Paper += SetSpriteToPaper;
		attackHandler.Scissor += SetSpriteToScissor;
	}

	private void SetSpriteToRock()
	{
		attackChoiceNumber = 0;
		choice = PlayerAttackChoice.Rock;
		spriteRenderer.sprite = rockSprite;
	}

	private void SetSpriteToPaper()
	{
		attackChoiceNumber = 1;
		choice = PlayerAttackChoice.Paper;
		spriteRenderer.sprite = paperSprite;
	}

	private void SetSpriteToScissor()
	{
		attackChoiceNumber = 2;
		choice = PlayerAttackChoice.Scissor;
		spriteRenderer.sprite = scissorSprite;
	}
}

 public enum PlayerAttackChoice {
	 Rock,
	 Paper,
	 Scissor,
 }