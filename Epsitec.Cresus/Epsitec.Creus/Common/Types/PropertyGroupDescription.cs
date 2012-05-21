//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>PropertyGroupDescription</c> class describes the grouping of
	/// items using a property name as the grouping criteria.
	/// </summary>
	public class PropertyGroupDescription : GroupDescription
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyGroupDescription"/> class.
		/// </summary>
		public PropertyGroupDescription()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyGroupDescription"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		public PropertyGroupDescription(string propertyName)
		{
			this.propertyName = propertyName;
		}

		/// <summary>
		/// Gets or sets the name of the property that is used to determine
		/// which group an item belongs to.
		/// </summary>
		/// <value>The name of the property.</value>
		public string							PropertyName
		{
			get
			{
				return this.propertyName;
			}
			set
			{
				if (this.propertyName != value)
				{
					this.propertyName = value;
					this.cachedType   = null;
					this.cachedGetter = null;
				}
			}
		}

		/// <summary>
		/// Gets the value used to derive the group(s).
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>The value used to derive the group(s) or <c>UnknownValue.Value</c> if
		/// no value can be found.</returns>
		protected override object GetGroupValue(object item)
		{
			//	Based on the property name, return either the property value or
			//	the item itself as the base value used to derive the group name.
			
			if ((item == null) ||
				(string.IsNullOrEmpty (this.propertyName)))
			{
				return item;
			}

			System.Type type = item.GetType ();

			if (this.cachedType != type)
			{
				this.cachedType = type;
				this.cachedGetter = this.CreateGetter (item);
			}

			if (this.cachedGetter == null)
			{
				return UnknownValue.Value;
			}
			else
			{
				return this.cachedGetter (item);
			}
		}

		private Support.PropertyGetter CreateGetter(object item)
		{
			if (item is IStructuredData)
			{
				return StructuredData.CreatePropertyGetter (this.propertyName);
			}
			else
			{
				return Support.DynamicCodeFactory.CreatePropertyGetter (this.cachedType, this.propertyName);
			}
		}

		private string							propertyName;
		private System.Type						cachedType;
		private Support.PropertyGetter			cachedGetter;
	}
}
