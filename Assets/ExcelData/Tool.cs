using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class Tool : ScriptableObject
{
	public List<ToolEntity> Sheet1; // Replace 'EntityType' to an actual type that is serializable.
}
