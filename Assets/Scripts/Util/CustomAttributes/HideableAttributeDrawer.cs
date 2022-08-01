// --------------------------------
// <copyright file="HideableAttributeDrawer.cs" company="Rumor Games">
//     Copyright (C) Rumor Games, LLC.  All rights reserved.
// </copyright>
// --------------------------------

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// HideableAttributeDrawer class.
/// </summary>
public abstract class HideableAttributeDrawer : PropertyDrawer
{
	/// <summary>
	/// Draws the GUI for the property.
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the property GUI.</param>
	/// <param name="property">The SerializedProperty to make the custom GUI for.</param>
	/// <param name="label">The label of this property.</param>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (!this.IsHidden(property))
		{
			EditorGUI.PropertyField(position, property, label);
		}
	}
	
	/// <summary>
	/// Get the property height in pixels of the given property.
	/// </summary>
	/// <param name="property">The SerializedProperty to get height for.</param>
	/// <param name="label">The label of this property.</param>
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return this.IsHidden(property) ? 0f : base.GetPropertyHeight(property, label);
	}
	
	/// <summary>
	/// Checks whether the property is supposed to be hidden.
	/// </summary>
	/// <param name="property">The SerializedProperty to test.</param>
	/// <returns>True if the property should be hidden.</returns>
	protected abstract bool IsHidden(SerializedProperty property);
}
#endif