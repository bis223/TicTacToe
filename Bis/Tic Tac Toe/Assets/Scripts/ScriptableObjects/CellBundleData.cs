using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName="New Cell Item")]
[Serializable]
public class CellBundleItem : ScriptableObject 
{
	public string cellName;
	public string cellBundleName;

}
