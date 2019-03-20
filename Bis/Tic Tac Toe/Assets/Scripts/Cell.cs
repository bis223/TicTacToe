using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cell : MonoBehaviour
{
	private const float flashDuration = 0.28f;
	private event Action<Cell> onCellClick;
	private CellState nextState;
	private int flashingSubCellIndex;
	private bool isFlashing;

	public GameObject[] subCells;
	public int RowNumber { get; private set; }
	public int ColumnNumber { get; private set; }
	public CellState CurrentState { get; private set; }
	public bool IsEmpty { get { return CurrentState == CellState.NULL; } }

	public void Init(int row, int col, Action<Cell> onCellClickAction)
	{
		RowNumber = row;
		ColumnNumber = col;
		this.name = string.Format("Cell [{0}, {1}]", row, col);

		onCellClick = onCellClickAction;

		Reset();
		StartCoroutine(FlashingCell());
	}

	IEnumerator FlashingCell()
	{
		bool active = false;
		isFlashing = true;
		while(IsEmpty)
		{
			active = !active;
			subCells[flashingSubCellIndex].SetActive(active);
			yield return new WaitForSeconds(flashDuration);
		}
		isFlashing = false;
	}

	void OnDisable()
	{
		StopAllCoroutines();
	}

	public void Reset()
	{
		Set(CellState.NULL);
		StopAllCoroutines();
		StartCoroutine(FlashingCell());

	}

	public void Set(CellState cs)
	{
		CurrentState = cs;
		if(cs == CellState.NULL)
		{
			UpdateCells();
		}
		else
		{
			UpdateCells(CurrentState);
		}
	}

	void UpdateCells(CellState? activeCell = null)
	{
		for(int i = 0; i < subCells.Length; i++)
		{
			subCells[i].SetActive(false);
		}

		if(activeCell != null)
		{
			subCells[(int)activeCell - 1].SetActive(true);	
			SetAlpha((int)activeCell - 1, 1.0f); //resetting alpha of clicked cell
		}
	}

	public void SetAlpha(int index, float alpha)	
	{
		flashingSubCellIndex = index;
		for(int i = 0; i < subCells.Length; i++)
		{
			if(index == i)
			{
				Color tempColor = subCells[i].GetComponent<SpriteRenderer>().color;	
				tempColor.a = alpha;
				subCells[i].GetComponent<SpriteRenderer>().color = tempColor;	
				subCells[i].SetActive(true);;
			}
			else
			{
				subCells[i].SetActive(false);
			}
		}
	}

	void OnMouseDown()
	{
		if(!this.IsEmpty)
		{
			return;
		}

	if (onCellClick != null)
		{
			onCellClick(this);
		}
	}


}

/// <summary>
/// Cell state.
/// </summary>
public enum CellState
{
	NULL,
	X,
	O,
	Y
}
