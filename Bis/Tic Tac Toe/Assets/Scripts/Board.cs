using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Board : MonoBehaviour 
{
	private Cell[,] cells;
	private const float cellOffset = 1.5f;
	private const float xOffset = 2.15f;
	private const float fadeAlpha = 0.15f;
	private CellState currentPlayer;
	private event Action<CellState, int, int> onBoardChange;

	public int rows = 3;
	public int columns = 3;
	public GameObject cellPrefab;

	public void Init(Action<CellState, int, int> onBoardChangeAction)
	{
		cells = new Cell[rows, columns];
		SetPlayer(CellState.NULL);
		onBoardChange = onBoardChangeAction;
		CreateCells();
	}

	private void CreateCells()
	{
		Transform holder = (new GameObject("Cells")).transform;

		holder.transform.parent = transform;
		if(rows == 4)
			holder.transform.localPosition = new Vector3(-xOffset, -cellOffset, 0f);
		else
			holder.transform.localPosition = new Vector3(-cellOffset, -cellOffset, 0f);

		for (int r = 0; r < rows; r++)
		{
			for (int c = 0; c < columns; c++)
			{
				GameObject go = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				Cell cell = go.GetComponent<Cell>();

				cell.transform.parent = holder;
				cell.transform.localPosition = new Vector3(c * cellOffset, r * cellOffset, 0f);

				cell.Init(r, c, OnCellClick);

				cells[r, c] = cell;
			}
		}
	}

	public void Clear()
	{
		for (int r = 0; r < rows; r++)
		{
			for (int c = 0; c < columns; c++)
			{
				cells[r, c].Reset();
			}
		}
	}

	public void Destroy()
	{
		Destroy(cells[0, 0].transform.parent.gameObject);
	}

	private void OnCellClick(Cell cell)
	{
		if (currentPlayer != CellState.NULL)
		{
			cell.Set(currentPlayer);
			if (onBoardChange != null)
			{
				onBoardChange(currentPlayer, cell.RowNumber, cell.ColumnNumber);
			}
		}
	}
	public void SetPlayer(CellState cs)
	{
		currentPlayer = cs;

		if(currentPlayer != CellState.NULL)
		{
			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < columns; c++)
				{
					if(cells[r,c] != null && cells[r,c].IsEmpty)
					{
						cells[r,c].SetAlpha((int)cs - 1, fadeAlpha);
					}
				}
			}
		}
	}

	public void SetBoardSprite(Sprite spr)
	{
		GetComponent<SpriteRenderer>().sprite = spr;
	}

	public bool IsDraw()
	{
		for (int r = 0; r < rows; r++)
		{
			for (int c = 0; c < columns; c++)
			{
				if(cells[r, c].IsEmpty)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool HasWon(CellState cs)
	{
		return IsHorizontalLineComplete(cs) || IsVerticalLineComplete(cs) || IsDiagonalLineComplete(cs);
	}
		
	bool HasHorizontalTriplet(int rowNum, int columnNum, CellState cs)
	{
		return cells[rowNum, columnNum].CurrentState == cs 
			&& cells[rowNum, columnNum + 1].CurrentState == cs 
			&& cells[rowNum, columnNum + 2].CurrentState == cs;
	}


	bool IsHorizontalLineComplete(CellState cs)
	{
		for (int r = 0; r < rows; r++)
		{
			for (int c = 0; c < columns - 2; c++)
			{
				if(HasHorizontalTriplet(r, c, cs))
				{
					return true;
				}
			}
		}

		return false;
	}
		
	bool HasVerticalTriplet(int rowNum, int columnNum, CellState cs)
	{		
			return cells[rowNum, columnNum].CurrentState == cs 
				&& cells[rowNum + 1, columnNum].CurrentState == cs 
				&& cells[rowNum + 2, columnNum].CurrentState == cs;
		
	}

	bool IsVerticalLineComplete(CellState cs)
	{
		for (int r = 0; r < rows - 2; r++)
		{
			for (int c = 0; c < columns; c++)
			{
				if(HasVerticalTriplet(r, c, cs))
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Determines whether this instance has diagonal triplet the specified rowNum columnNum CellState and direction .
	/// </summary>
	/// <returns><c>true</c> if this instance has diagonal triplet the specified rowNum columnNum CellState in both dir ; otherwise, <c>false</c>.</returns>
	/// 
	bool HasDiagonalTriplet(int rowNum, int columnNum, CellState cs, int dir)
	{
		return  cells[rowNum, columnNum].CurrentState == cs 
			&& cells[rowNum + (1 * dir), columnNum + 1].CurrentState == cs 
			&& cells[rowNum +  (2 * dir), columnNum + 2].CurrentState == cs;
	}

	bool IsDiagonalLineComplete(CellState cs)
	{
		for (int r = 0; r < rows - 2; r++)
		{
			for (int c = 0; c < columns - 2; c++)
			{
				if(HasDiagonalTriplet(r, c, cs , 1))
				{
					return true;
				}
			}
		}

		for (int r = rows - 1 ; r > 1; r--)
		{
			for (int c = 0; c < columns - 2; c++)
			{
				if(HasDiagonalTriplet(r, c, cs , -1))
				{
					return true;
				}
			}
		}
		return false;
	}
}
