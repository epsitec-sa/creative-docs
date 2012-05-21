//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>PropertyValuePair</c> structure is used by <see cref="T:DependencyObject"/>
	/// to store its property/value pairs in a dictionary, and to enumerate them.
	/// </summary>
	public struct PropertyValuePair
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:PropertyValuePair"/> class.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		public PropertyValuePair(DependencyProperty property, object value)
		{
			this.property = property;
			this.value    = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:PropertyValuePair"/> class.
		/// </summary>
		/// <param name="pair">The property/value pair.</param>
		public PropertyValuePair(KeyValuePair<DependencyProperty, object> pair)
		{
			this.property = pair.Key;
			this.value = pair.Value;
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <value>The property.</value>
		public DependencyProperty				Property
		{
			get
			{
				return this.property;
			}
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public object							Value
		{
			get
			{
				return this.value;
			}
		}
		
		private DependencyProperty				property;
		private object							value;
	}
}
