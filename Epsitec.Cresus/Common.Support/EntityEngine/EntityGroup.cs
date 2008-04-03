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
	/// <typeparam name="T">The entity type, derived from <see cref="AbstractEntity"/>.</typeparam>
	public class EntityGroup<T> : EntityTable<T>, IGrouping<object, T> where T : AbstractEntity, new ()
	{
		public EntityGroup()
		{
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
		/// Gets or sets the title. The title may contain formatting (this is
		/// a full fledged tagged text).
		/// </summary>
		/// <value>The title encoded as a tagged text.</value>
		public override string Title
		{
			get
			{
				string title = base.Title;
				
				if (title == null)
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
					return title;
				}
			}
			set
			{
				base.Title = value;
			}
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

		private object key;
	}
}
