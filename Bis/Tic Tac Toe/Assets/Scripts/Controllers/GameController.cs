using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GameController : MonoBehaviour 
{
	public int numberOfPlayers = 2;
	private List<PlayerStats> players;
	private int challengerGamesTotal = 3;
	private int challengerHighScore = 0;
	private event Action onComplete;

	public GameMode gameMode = GameMode.TWO_PLAYER;
	public Board board;
	public UIController uiController;
	public GameObject goGame;
	public CellBundleItem[] cellBundlesData = new CellBundleItem[2];


	void Awake()
	{
		int randomId = UnityEngine.Random.Range(0, cellBundlesData.Length);
		StartCoroutine(DownloadCellBundle(cellBundlesData[randomId].cellBundleName, cellBundlesData[randomId].cellName));
	}


	IEnumerator DownloadCellBundle(string bundleName, string itemName)
	{
		WWW www = WWW.LoadFromCacheOrDownload("file:///"+ Application.dataPath + "/AssetBundles/" + bundleName, 1);
		yield return www;

		AssetBundle bundle = www.assetBundle;
		AssetBundleRequest request = bundle.LoadAssetAsync<GameObject>(itemName);

		yield return request;

		GameObject obj = request.asset as GameObject;
		if(obj != null)
		{
			board.cellPrefab  = obj;
			onComplete = () => bundle.Unload(true);
		}
		else
		{
			bundle.Unload(true);
		}
	}
		

	void Start()
	{
		UIController.OnClickPlayButton += StartGame;
		UIController.OnClickNewGameButton += Restart;
		UIController.OnClickMenuButton += GoToMenu;
		UIController.OnClickContinueChallengeGameButton += ContinueChallenge;
	}

	void OnDestroy()
	{
		UIController.OnClickPlayButton -= StartGame;
		UIController.OnClickNewGameButton -= Restart;
		UIController.OnClickMenuButton -= GoToMenu;
		UIController.OnClickContinueChallengeGameButton -= ContinueChallenge;
		onComplete();

	}

	void StartGame(int gameModeIndex)
	{
		SetGameMode((GameMode)gameModeIndex);
		goGame.SetActive(true);
		board.Init(OnBoardChange);
		board.SetPlayer(CellState.X);
		InitPlayers();
		uiController.Init(numberOfPlayers);
		uiController.uiFSM.ChangeState(FiniteStateList.GamePlayState);
	}

	public void SetGameMode(GameMode gm)
	{
		gameMode = gm;
		switch (gm)
		{
		case GameMode.TWO_PLAYER:
			numberOfPlayers = 2;
			board.rows = 3;
			board.columns = 3;
			board.SetBoardSprite(uiController.board_3X3);
			break;
		case GameMode.THREE_PLAYER:
			numberOfPlayers = 3;
			board.rows = 4;
			board.columns = 4;
			board.SetBoardSprite(uiController.board_4X4);
			break;
		case GameMode.CHALLENGER:
			numberOfPlayers = 2;
			board.rows = 3;
			board.columns = 3;
			board.SetBoardSprite(uiController.board_3X3);
			challengerGamesTotal = UnityEngine.Random.Range(3, 6);
			break;
		}
	}
	void Restart()
	{
		board.Clear();
		board.SetPlayer(CellState.X);

		if(gameMode == GameMode.CHALLENGER)
		{
			challengerGamesTotal = UnityEngine.Random.Range(3, 6);
			InitPlayers();
			for(int i = 0; i < players.Count; i++)
			{
				uiController.UpdateScore(i, 0);
			}
		}
		uiController.uiFSM.ChangeState(FiniteStateList.GamePlayState);
	}

	void ContinueChallenge()
	{
		board.Clear();
		board.SetPlayer(CellState.X);
	
		uiController.uiFSM.ChangeState(FiniteStateList.GamePlayState);
	}

	void GoToMenu()
	{
		uiController.uiFSM.ChangeState(FiniteStateList.MainMenuState);
		board.Destroy();
		uiController.RemoveScorePanels();
	}

	void InitPlayers()
	{
		players = new List<PlayerStats>(numberOfPlayers);
		for(int i = 0; i < numberOfPlayers; i++)
		{
			players.Add(new PlayerStats("Player " + (CellState)(i+1), 0, (CellState)(i+1)));
		}
	}

	/// <summary>
	/// Raises the board change event and checks game over
	/// </summary>
	private void OnBoardChange(CellState currentState, int row, int col)
	{
		CellState nextState = CellState.NULL;
		challengerHighScore = players.Max(r => r.Score);

		if (board.HasWon(currentState))
		{
			
			int winnerIndex = (int)currentState - 1;
			players[winnerIndex].Score += 1;
			uiController.UpdateScore(winnerIndex, players[winnerIndex].Score);

			if(gameMode == GameMode.CHALLENGER)
			{
				challengerGamesTotal--;
				if(challengerGamesTotal == 0)
				{	
					uiController.ShowGameOver(GetChallengeWinner());
				}
				else
				{
					uiController.ShowChallengerProgress(challengerGamesTotal, currentState);
				}
			}
			else
			{
				uiController.ShowGameOver(currentState);
			}
		}
		else if (board.IsDraw())
		{
			if(gameMode == GameMode.CHALLENGER)
			{
				challengerGamesTotal--;
				if(challengerGamesTotal == 0)
				{	
					uiController.ShowGameOver(GetChallengeWinner());
				}
				else
				{
					uiController.ShowChallengerProgress(challengerGamesTotal);
				}
			}
			else
			{
				uiController.ShowGameOver();
			}
		}
		else
		{
			int nextStateIndex = (int)currentState + 1;
			if(nextStateIndex >  players.Count)
			{ 
				nextStateIndex = 1; 
			}
			nextState = (CellState)nextStateIndex;
		}
		board.SetPlayer(nextState);
	}

	/// <summary>
	/// Gets the challenge winner if there is a single high scorer, else returns null for a draw.
	/// </summary>
	/// <returns>The challenge winner.</returns>
    CellState? GetChallengeWinner()
	{
		int totalPlayersWithHighScore = 0;
		int winnerIndex = 0;
		for(int i = 0; i < players.Count; i++)
		{
			if(challengerHighScore == players[i].Score)
			{
				totalPlayersWithHighScore++;
				winnerIndex = i;
			}
		}
		if(totalPlayersWithHighScore == 1)
		{
			return (CellState)(winnerIndex + 1);
		}
		return null;
	}
		
	public enum GameMode
	{
		TWO_PLAYER,
		THREE_PLAYER,
		CHALLENGER
	}
}
