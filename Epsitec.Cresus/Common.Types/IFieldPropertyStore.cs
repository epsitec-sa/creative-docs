//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IFieldPropertyStore</c> interface gives access to field
	/// properties.
	/// </summary>
	public interface IFieldPropertyStore
	{
		/// <summary>
		/// Gets the value associated with the property of the specified field.
		/// </summary>
		/// <param name="fieldId">The field id.</param>
		/// <param name="property">The property.</param>
		/// <returns>The value or <c>UndefinedValue.Value</c> if it is not
		/// defined.</returns>
		object GetValue(string fieldId, DependencyProperty property);

		/// <summary>
		/// Sets the value associated with the property of the specified field.
		/// </summary>
		/// <param name="fieldId">The field id.</param>
		/// <param name="property">The property.</param>
		/// <param name="value">The value to set.</param>
		void SetValue(string fieldId, DependencyProperty property, object value);

		/// <summary>
		/// Determines whether the specified field contains a value for the
		/// specified property.
		/// </summary>
		/// <param name="fieldId">The field id.</param>
		/// <param name="property">The property.</param>
		/// <returns>
		/// 	<c>true</c> if the specified field contains a value for the
		/// 	specified property; otherwise, <c>false</c>.
		/// </returns>
		bool ContainsValue(string fieldId, DependencyProperty property);
	}
}
