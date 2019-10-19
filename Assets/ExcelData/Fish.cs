using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class Fish : ScriptableObject
{
	public List<FishEntity> Sheet1; // Replace 'EntityType' to an actual type that is serializable.
}
