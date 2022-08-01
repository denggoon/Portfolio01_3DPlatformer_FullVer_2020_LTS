// --------------------------------
// <copyright file="HideInEditModeAttributeDrawer.cs" company="Rumor Games">
//     Copyright (C) Rumor Games, LLC.  All rights reserved.
// </copyright>
// --------------------------------

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// HideInEditModeAttributeDrawer class.
/// </summary>
[CustomPropertyDrawer(typeof(HideInPlayModeAttribute))]
public class HideInPlayModeAttributeDrawer : HideableAttributeDrawer
{
	/// <summary>
	/// Checks whether the property is supposed to be hidden.
	/// </summary>
	/// <param name="property">The SerializedProperty to test.</param>
	/// <returns>True if the property should be hidden.</returns>
	protected override bool IsHidden(SerializedProperty property)
	{
		return Application.isPlaying;
	}
}
#endif