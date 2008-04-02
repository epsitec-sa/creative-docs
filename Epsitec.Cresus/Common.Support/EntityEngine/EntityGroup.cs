//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityGroup&lt;T&gt;</c> class used to represent a group of entities
	/// which share a common key.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EntityGroup<T> : IGrouping<object, T> where T : AbstractEntity, new ()
	{
		public EntityGroup()
		{
			this.values = new List<T> ();
		}

		public int Count
		{
			get
			{
				return this.values.Count;
			}
		}

		public object Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		/// <summary>
		/// Gets or sets the key title. The title may contain formatting (this is
		/// a full fledged tagged text).
		/// </summary>
		/// <value>The key title encoded as a tagged text.</value>
		public string KeyTitle
		{
			get
			{
				if (this.keyTitle == null)
				{
					if (this.key == null)
					{
						return TextConverter.ConvertToTaggedText ("<null>");
					}
					else
					{
						return TextConverter.ConvertToTaggedText (this.key.ToString ());
					}
				}
				else
				{
					return this.keyTitle;
				}
			}
			set
			{
				this.keyTitle = value;
			}
		}

		public void Add(T item)
		{
			this.values.Add (item);
		}

		public void AddRange(IEnumerable<T> collection)
		{
			this.values.AddRange (collection);
		}

		#region IGrouping<object,T> Members

		object IGrouping<object, T>.Key
		{
			get
			{
				return this.Key;
			}
		}

		#endregion

		#region IEnumerable<T> Members

		public System.Collections.Generic.IEnumerator<T> GetEnumerator()
		{
			return this.values.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			System.Collections.IList list = this.values;
			return list.GetEnumerator ();
		}

		#endregion


		private readonly List<T> values;
		private object key;
		private string keyTitle;
	}
}
