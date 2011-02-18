//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization
{
	/// <summary>
	/// The <c>BlackList</c> class is used to mark properties as black-listed
	/// for the serialization; the black-listed properties may not be serialized.
	/// </summary>
	public class BlackList : DependencyObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BlackList"/> class.
		/// </summary>
		public BlackList()
		{
		}


		/// <summary>
		/// Gets a value indicating whether the black list is empty.
		/// </summary>
		/// <value><c>true</c> if this black list is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get
			{
				return this.properties == null;
			}
		}


		/// <summary>
		/// Adds the specified property to the black list. Adding a property more
		/// than once has no further effect.
		/// </summary>
		/// <param name="property">The property.</param>
		public void Add(DependencyProperty property)
		{
			if (this.properties == null)
			{
				this.properties = new Dictionary<DependencyProperty, bool> ();
			}
			
			this.properties[property] = true;
		}

		/// <summary>
		/// Removes the specified property from the black list.
		/// </summary>
		/// <param name="property">The property.</param>
		public void Clear(DependencyProperty property)
		{
			this.properties.Remove (property);

			if (this.properties.Count == 0)
			{
				this.properties = null;
			}
		}

		/// <summary>
		/// Determines whether the black list contains the specified property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>
		/// 	<c>true</c> if the black list contains the specified property; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(DependencyProperty property)
		{
			if ((this.properties != null) &&
				(this.properties.ContainsKey (property)))
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// Adds the specified property to the object's black list. Creates a
		/// black list if needed.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <param name="property">The property.</param>
		public static void Add(DependencyObject o, DependencyProperty property)
		{
			BlackList blackList = BlackList.GetSerializationBlackList (o);

			if (blackList == null)
			{
				blackList = new BlackList ();
				BlackList.SetSerializationBlackList (o, blackList);
			}
			
			blackList.Add (property);
		}

		/// <summary>
		/// Removes the specified property from the object's black list. If the
		/// black list becomes empty, it is deleted.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <param name="property">The property.</param>
		public static void Clear(DependencyObject o, DependencyProperty property)
		{
			BlackList blackList = BlackList.GetSerializationBlackList (o);

			if (blackList != null)
			{
				blackList.Clear (property);

				if (blackList.IsEmpty)
				{
					BlackList.SetSerializationBlackList (o, null);
				}
			}
		}

		/// <summary>
		/// Determines whether the object's black list contains the specified property.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <param name="property">The property.</param>
		/// <returns>
		/// 	<c>true</c> if the object's black list contains the specified property; otherwise, <c>false</c>.
		/// </returns>
		public static bool Contains(DependencyObject o, DependencyProperty property)
		{
			BlackList blackList = BlackList.GetSerializationBlackList (o);
			
			if (blackList != null)
			{
				return blackList.Contains (property);
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// Gets the black list.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <returns>The black list or <c>null</c>.</returns>
		public static BlackList GetSerializationBlackList(DependencyObject o)
		{
			return (BlackList) o.GetValue (BlackList.SerializationBlackListProperty);
		}

		/// <summary>
		/// Sets the black list.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <param name="value">The black list.</param>
		public static void SetSerializationBlackList(DependencyObject o, BlackList value)
		{
			if (value == null)
			{
				o.ClearValue (BlackList.SerializationBlackListProperty);
			}
			else
			{
				o.SetValue (BlackList.SerializationBlackListProperty, value);
			}
		}
		
		
		public static readonly DependencyProperty SerializationBlackListProperty = DependencyProperty.RegisterAttached ("SerializationBlackList", typeof (BlackList), typeof (BlackList), new DependencyPropertyMetadata ().MakeNotSerializable ());

		private Dictionary<DependencyProperty, bool> properties;
	}
}
