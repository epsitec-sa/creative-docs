//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	public class DataView
	{
		public DataView(DataViewContext context, AbstractEntity root)
		{
			this.context = context;
			this.root = root;
		}


		public IDataItem GetValue(string path)
		{
			if (string.IsNullOrEmpty (path))
			{
				return new DataItem (this.root);
			}

			string[] pathElements = path.Split ('.');

			IValueStore entity = this.root;
			object      value  = null;
			
			for (int i = 0; i < pathElements.Length; )
			{
				string id = pathElements[i++];

				if (entity == null)
				{
					//	Trying to get a value on something which does not implement
					//	the IValueStore interface is a fatal error.

					throw new System.InvalidOperationException (string.Format ("Path {0} contains an invalid node", path));
				}

				value = entity.GetValue (id);

				if ((UndefinedValue.IsUndefinedValue (value)) ||
					(UnknownValue.IsUnknownValue (value)) ||
					(value == null))
				{
					return new DataItem ();
				}

				System.Collections.IEnumerable collection = this.context.GetEnumerable (value);

				if (collection != null)
				{
					//	The value is a collection; we will have to map it to a
					//	(sorted/filtered/grouped) table, as required.

					//	TODO: ...
					throw new System.NotImplementedException ();
				}

				entity = value as IValueStore;
			}

			System.Diagnostics.Debug.Assert (value != null);
			
			AbstractEntity entityValue = value as AbstractEntity;

			if (entityValue == null)
			{
				//	TODO: ...

				return new DataItem ();
			}
			else
			{
				return new DataItem (entityValue);
			}
		}

		private readonly DataViewContext context;
		private readonly AbstractEntity root;
	}
}
