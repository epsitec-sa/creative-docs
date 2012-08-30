//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	public abstract class ColumnRef
	{
		public ColumnRef(string columnId)
		{
			this.columnId = columnId;
		}

		public string Id
		{
			get
			{
				return this.columnId;
			}
		}


		public XElement Save(string xmlNodeName)
		{
			var attributes = new List<XAttribute> ();

			this.Serialize (attributes);
			
			var xml = new XElement (xmlNodeName,
									attributes,
									this.SaveValue (Strings.Value));

			return xml;
		}

		public static ColumnRef<T> Restore<T>(XElement xml)
		{
			var value    = ColumnRef<T>.RestoreValue (xml.Element (Strings.Value));
			var instance = new ColumnRef<T> (xml.Attribute (Strings.Id).Value, value);

			return instance;
		}


		protected abstract XElement SaveValue(string xmlNodeName);

		protected virtual void Serialize(List<XAttribute> attributes)
		{
			attributes.Add (new XAttribute (Strings.Id, this.columnId));
		}

		#region Strings Class

		private static class Strings
		{
			public static readonly string		Id = "id";
			public static readonly string		Value = "v";
		}

		#endregion

		private readonly string columnId;
	}
}
