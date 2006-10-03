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
		public PropertyGroupDescription()
		{
		}

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
		
		public override bool NamesMatch(string groupName, string itemName)
		{
			return string.Equals (groupName, itemName, this.stringComparison);
		}

		private object GetGroupValue(object item)
		{
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

		private System.StringComparison stringComparison;
		private IValueConverter converter;
		private string propertyName;
	}
}
