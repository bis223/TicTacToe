using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour 
{
	public string Name { get; set; }

	public int Score { get; set; }

	public CellState playerType { get; set; }

	public PlayerStats(string name, int score, CellState cs)
	{
		Name = name;
		Score = score;
		playerType = cs;
	}

}
