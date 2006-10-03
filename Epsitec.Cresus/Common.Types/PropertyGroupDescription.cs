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
			if (item == null)
			{
				return new string[0];
			}

			object value = this.GetGroupValue (item);

			//	TODO: implement

			throw new System.NotImplementedException ();
		}
		
		public override bool NamesMatch(string groupName, string itemName)
		{
			return string.Equals (groupName, itemName, this.stringComparison);
		}

		private object GetGroupValue(object item)
		{
			if (string.IsNullOrEmpty (this.propertyName))
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
