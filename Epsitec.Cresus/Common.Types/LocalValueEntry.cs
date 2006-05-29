//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>LocalValueEntry</c> structure is used by <see cref="T:DependencyObject"/>
	/// to store its property/value pairs in a dictionary, and to enumerate them.
	/// </summary>
	public struct LocalValueEntry
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:LocalValueEntry"/> class.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		public LocalValueEntry(DependencyProperty property, object value)
		{
			this.property = property;
			this.value    = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:LocalValueEntry"/> class.
		/// </summary>
		/// <param name="pair">The property/value pair.</param>
		public LocalValueEntry(KeyValuePair<DependencyProperty, object> pair)
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
