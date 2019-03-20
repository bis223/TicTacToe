using UnityEngine;
using System.Collections;

public class GameOverLoader : StateBase {

	public GameObject gameOverPanel;

	protected override void Init()
	{
		base.Init();
		gameOverPanel.SetActive (true);
	
	}

	protected override void DeInit()
	{
		base.DeInit();
		gameOverPanel.SetActive (false);
	}
}
