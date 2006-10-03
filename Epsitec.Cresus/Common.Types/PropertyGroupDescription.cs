//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>PropertyGroupDescription</c> class describes the grouping of
	/// items using a property name as the grouping criteria.
	/// </summary>
	public class PropertyGroupDescription : AbstractGroupDescription
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyGroupDescription"/> class.
		/// </summary>
		public PropertyGroupDescription()
		{
		}

		/// <summary>
		/// Gets or sets the string comparison mode.
		/// </summary>
		/// <value>The string comparison mode.</value>
		public System.StringComparison StringComparison
		{
			get
			{
				return this.stringComparison;
			}
			set
			{
				this.stringComparison = value;
			}
		}

		/// <summary>
		/// Gets or sets a converter to apply to the property value or to the
		/// item (if no property value is defined) in order to produce the
		/// final value used to produce the group name.
		/// </summary>
		/// <value>The converter.</value>
		public IValueConverter Converter
		{
			get
			{
				return this.converter;
			}
			set
			{
				this.converter = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the property that is used to determine
		/// which group an item belongs to.
		/// </summary>
		/// <value>The name of the property.</value>
		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
			set
			{
				this.propertyName = value;
			}
		}

		/// <summary>
		/// Gets zero or one group names for the item. This uses either the named property
		/// value, if any, or else the item itself.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="culture">The culture.</param>
		/// <returns>An array with the group names the item belongs to.</returns>
		public override string[] GetGroupNamesForItem(object item, System.Globalization.CultureInfo culture)
		{
			object value = this.GetGroupValue (item);

			if ((value == null) ||
				(UnknownValue.IsUnknownValue (value)) ||
				(UndefinedValue.IsUndefinedValue (value)))
			{
				return new string[0];
			}

			string group;

			if (this.converter != null)
			{
				group = this.converter.Convert (value, typeof (string), null, culture) as string;
			}
			else
			{
				group = value.ToString ();
			}

			return string.IsNullOrEmpty (group) ? new string[0] : new string[] { group };
		}

		/// <summary>
		/// Tests if the names match.
		/// </summary>
		/// <param name="groupName">Name of the group.</param>
		/// <param name="itemName">Name of the item.</param>
		/// <returns><c>true</c> if the names match; <c>false</c> otherwise.</returns>
		public override bool NamesMatch(string groupName, string itemName)
		{
			return string.Equals (groupName, itemName, this.stringComparison);
		}

		/// <summary>
		/// Gets the value used to derive the group(s).
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>The value used to derive the group(s) or <c>UnknownValue.Instance</c> if
		/// no value can be found.</returns>
		protected virtual object GetGroupValue(object item)
		{
			//	Based on the property name, return either the property value or
			//	the item itself as the base value used to derive the group name.
			
			if ((item == null) ||
				(string.IsNullOrEmpty (this.propertyName)))
			{
				return item;
			}

			IStructuredData data = item as IStructuredData;

			if (data != null)
			{
				return data.GetValue (this.propertyName);
			}
			else
			{
				return UnknownValue.Instance;
			}
		}

		private System.StringComparison			stringComparison = System.StringComparison.Ordinal;
		private IValueConverter					converter;
		private string							propertyName;
	}
}
