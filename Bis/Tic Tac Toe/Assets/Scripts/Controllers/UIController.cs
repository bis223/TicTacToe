using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UIController : MonoBehaviour 
{
	private List<Text> txtScores;
	public FSM	uiFSM;
	public static event Action<int> OnClickPlayButton;
	public static event Action OnClickNewGameButton;
	public static event Action OnClickMenuButton;
	public static event Action OnClickContinueChallengeGameButton;
	public Text gameOverInfo;
	public Text challengerInfo;
	public GameObject hud;
	public GameObject scoreTextPrefab;
	public GameObject gameOverDialog;
	public GameObject challengerDialog;
	public Sprite board_3X3;
	public Sprite board_4X4;

	public void Init(int numOfPlayers)
	{
		CreateScorePanels(numOfPlayers);
	}

	void CreateScorePanels(int numOfPlayers)
	{
		txtScores = new List<Text>();
		for(int i = 0; i < numOfPlayers; i++)
		{
			GameObject goTemp = Instantiate(scoreTextPrefab);
			goTemp.transform.SetParent(hud.transform, false);
			txtScores.Add(goTemp.GetComponentInChildren<Text>());
			UpdateScore(i, 0);
		}
	}

	public void RemoveScorePanels()
	{
		for(int i = 0; i < txtScores.Count; i++)
		{
			Destroy(txtScores[i].transform.parent.gameObject);
		}
	}

	public void ClickPlayButton(int gameMode)
	{
		if(OnClickPlayButton != null)
		{
			OnClickPlayButton(gameMode);
		}
	}

	public void ClickNewGameButton()
	{
		if(OnClickNewGameButton != null)
		{
			OnClickNewGameButton();
		}
	}

	public void ClickContinueChallengeGameButton()
	{
		if(OnClickContinueChallengeGameButton != null)
		{
			OnClickContinueChallengeGameButton();
		}
	}

	public void ClickMenuButton()
	{
		if(OnClickMenuButton != null)
		{
			OnClickMenuButton();
		}
	}

	public void UpdateScore(int playerIndex, int score)
	{
		txtScores[playerIndex].text = "Player " + (CellState)(playerIndex + 1) + ": " + score;
	}

	public void ShowGameOver(CellState? winner = null)
	{
		uiFSM.ChangeState(FiniteStateList.GameOverState);
		gameOverDialog.SetActive(true);
		challengerDialog.SetActive(false);
		if(winner != null)
		{
			gameOverInfo.text = "Player " + winner + " Wins!...";
		}
		else
		{
			gameOverInfo.text = "Draw!!.";
		}
	}

	public void ShowChallengerProgress(int remainingGames, CellState? winner = null)
	{
		uiFSM.ChangeState(FiniteStateList.GameOverState);
		gameOverDialog.SetActive(false);
		challengerDialog.SetActive(true);
		if(winner != null)
		{
			challengerInfo.text = "Player " + winner + " Wins!...\n" + remainingGames +" more to go!!.";
		}
		else
		{
			challengerInfo.text = "Draw!!..\n" + remainingGames + " more to go!!.";
		}
	}
}
