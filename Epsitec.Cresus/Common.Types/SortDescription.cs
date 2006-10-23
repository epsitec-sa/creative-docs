//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>SortDescription</c> structure defines the direction and the property
	/// name to be used as the criteria for sorting a collection or view.
	/// </summary>
	[SerializationConverter (typeof (SortDescription.SerializationConverter))]
	public struct SortDescription : System.IEquatable<SortDescription>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SortDescription"/> class
		/// using the ascending sort by default.
		/// </summary>
		/// <param name="propertyName">Name of the property used for sorting.</param>
		public SortDescription(string propertyName)
			: this (ListSortDirection.Ascending, propertyName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SortDescription"/> class.
		/// </summary>
		/// <param name="direction">The sort direction.</param>
		/// <param name="propertyName">Name of the property used for sorting.</param>
		public SortDescription(ListSortDirection direction, string propertyName)
		{
			if ((direction != ListSortDirection.Ascending) &&
				(direction != ListSortDirection.Descending))
			{
				throw new System.ComponentModel.InvalidEnumArgumentException ("direction", (int) direction, typeof (ListSortDirection));
			}

			if (propertyName == null)
			{
				throw new System.ArgumentNullException ("propertyName");
			}
			if (propertyName.Length == 0)
			{
				throw new System.ArgumentException ("Empty property name");
			}

			this.direction = direction;
			this.propertyName = propertyName;
		}

		/// <summary>
		/// Gets the name of the property used for sorting.
		/// </summary>
		/// <value>The name of the property.</value>
		public string							PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		/// <summary>
		/// Gets the sort direction.
		/// </summary>
		/// <value>The sort direction.</value>
		public ListSortDirection				Direction
		{
			get
			{
				return this.direction;
			}
		}

		#region IEquatable<SortDescription> Members

		public bool Equals(SortDescription other)
		{
			return this == other;
		}

		#endregion

		/// <summary>
		/// Creates a comparison object based on the sort description which
		/// can be used to sort items of a given type.
		/// </summary>
		/// <param name="type">The item type for which to create the comparison.</param>
		/// <returns>The <c>System.Comparison&lt;object&gt;</c> which can be used
		/// to sort items.</returns>
		public System.Comparison<object> CreateComparison(System.Type type)
		{
			string propertyName = this.PropertyName;

			Support.PropertyComparer comparer = Support.DynamicCodeFactory.CreatePropertyComparer (type, propertyName);

			switch (this.Direction)
			{
				case ListSortDirection.Ascending:
					return delegate (object x, object y)
					{
						return comparer (x, y);
					};

				case ListSortDirection.Descending:
					return delegate (object x, object y)
					{
						return -comparer (x, y);
					};
			}

			throw new System.ArgumentException ("Invalid sort direction");
		}

		public override bool Equals(object obj)
		{
			if (obj is SortDescription)
			{
				return this == (SortDescription) obj;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.propertyName.GetHashCode () ^ this.direction.GetHashCode ();
		}

		public static bool operator==(SortDescription a, SortDescription b)
		{
			return (a.direction == b.direction)
				&& (a.propertyName == b.propertyName);
		}

		public static bool operator!=(SortDescription a, SortDescription b)
		{
			return (a.direction != b.direction)
				|| (a.propertyName != b.propertyName);
		}

		#region SerializationConverter Class

		public class SerializationConverter : ISerializationConverter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, IContextResolver context)
			{
				SortDescription sort = (SortDescription) value;

				switch (sort.direction)
				{
					case ListSortDirection.Ascending:
						return string.Concat ("A;", sort.propertyName);
					case ListSortDirection.Descending:
						return string.Concat ("D;", sort.propertyName);
				}

				throw new System.InvalidOperationException (string.Format ("ListSortDirection.{0} not supported", sort.direction));
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				System.Diagnostics.Debug.Assert (value[1] == ';');

				switch (value[0])
				{
					case 'A':
						return new SortDescription (ListSortDirection.Ascending, value.Substring (2));
					case 'D':
						return new SortDescription (ListSortDirection.Descending, value.Substring (2));
				}

				throw new System.FormatException ("Unknown format or ListSortDirection");
			}

			#endregion
		}

		#endregion

		private string							propertyName;
		private ListSortDirection				direction;
	}
}
