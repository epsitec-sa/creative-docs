//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		public BlackList()
		{
		}

		
		public bool IsEmpty
		{
			get
			{
				return this.properties == null;
			}
		}

		
		public void Add(DependencyProperty property)
		{
			if (this.properties == null)
			{
				this.properties = new Dictionary<DependencyProperty, bool> ();
			}
			
			this.properties[property] = true;
		}

		public void Clear(DependencyProperty property)
		{
			this.properties.Remove (property);

			if (this.properties.Count == 0)
			{
				this.properties = null;
			}
		}

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

		
		public static BlackList GetSerializationBlackList(DependencyObject o)
		{
			return (BlackList) o.GetValue (BlackList.SerializationBlackListProperty);
		}

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
