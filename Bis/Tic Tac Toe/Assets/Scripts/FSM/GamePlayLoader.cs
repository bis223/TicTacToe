using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GamePlayLoader : StateBase
{
	public GameObject gamePlayPanel;

	protected override void Init()
	{
		base.Init();
		gamePlayPanel.SetActive(true);
	}


	protected override void DeInit()
	{
		base.DeInit();
		gamePlayPanel.SetActive (false);
	}
}
