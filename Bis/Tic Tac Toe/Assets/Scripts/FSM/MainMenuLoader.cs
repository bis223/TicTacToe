using UnityEngine;
using System.Collections;

public class MainMenuLoader : StateBase 
{
	public GameObject mainMenuPanel;

	protected override void Init()
	{
		base.Init();
		mainMenuPanel.SetActive (true);
	}

	protected override void DeInit()
	{
		base.DeInit();
		mainMenuPanel.SetActive (false);
	}

}
