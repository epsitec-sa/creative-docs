//	Copyright © 2006-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>GroupDescription</c> class provides an abstract class for
	/// types that describe how to divide the items in a collection into groups.
	/// </summary>
	public abstract class GroupDescription
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GroupDescription"/> class.
		/// </summary>
		protected GroupDescription()
		{
		}
		
		/// <summary>
		/// Gets zero or one group names for the item. This uses either the named property
		/// value, if any, or else the item itself.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="culture">The culture.</param>
		/// <returns>An array with the group names the item belongs to.</returns>
		public virtual string[] GetGroupNamesForItem(object item, System.Globalization.CultureInfo culture)
		{
			object value = this.GetGroupValue (item);

			if ((value == null) ||
				(UnknownValue.IsUnknownValue (value)) ||
				(UndefinedValue.IsUndefinedValue (value)))
			{
				return Epsitec.Common.Types.Collections.EmptyArray<string>.Instance;
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

			return string.IsNullOrEmpty (group) ? Epsitec.Common.Types.Collections.EmptyArray<string>.Instance : new string[] { group };
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
		/// Tests if the names match.
		/// </summary>
		/// <param name="groupName">Name of the group.</param>
		/// <param name="itemName">Name of the item.</param>
		/// <returns><c>true</c> if the names match; <c>false</c> otherwise.</returns>
		public virtual bool NamesMatch(string groupName, string itemName)
		{
			return string.Equals (groupName, itemName, this.stringComparison);
		}

		#region Abstract Methods

		/// <summary>
		/// Gets the value used to derive the group(s).
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>The value used to derive the group(s) or <c>UnknownValue.Value</c> if
		/// no value can be found.</returns>
		protected abstract object GetGroupValue(object item);

		#endregion

		private System.StringComparison			stringComparison = System.StringComparison.Ordinal;
		private IValueConverter					converter;
	}
}
