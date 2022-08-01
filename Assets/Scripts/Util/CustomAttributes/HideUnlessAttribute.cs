// --------------------------------
// <copyright file="HideUnlessAttribute.cs" company="Rumor Games">
//     Copyright (C) Rumor Games, LLC.  All rights reserved.
// </copyright>
// --------------------------------

using UnityEngine;

/// <summary>
/// HideUnlessAttribute class.
/// </summary>
public class HideUnlessAttribute : PropertyAttribute
{
	/// <summary>
	/// Initializes a new instance of the HideUnlessAttribute class.
	/// </summary>
	/// <param name="fieldName">Field that controls whether this property is hidden or visible.</param>
	public HideUnlessAttribute(string fieldName)
	{
		this.FieldName = fieldName;
	}
	
	/// <summary>
	/// Gets the name of the field this attribute's drawer will test to see whether to show in the inspector.
	/// </summary>
	public string FieldName { get; private set; }

}
