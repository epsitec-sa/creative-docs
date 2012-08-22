//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Data.Metadata
{
	/// <summary>
	/// The <c>DataSetMetadata</c> class defines metadata for a data set (which is what the
	/// user understands as a database, such as "customers", "invoices", etc.).
	/// </summary>
	public class DataSetMetadata
	{
		public DataSetMetadata(string dataSetName, Druid baseShowCommandId)
		{
			this.name = dataSetName;
			this.baseShowCommandId = baseShowCommandId;
		}

		public DataSetMetadata(IDictionary<string, string> data)
		{
			this.name = data[Strings.Name];
			this.baseShowCommandId = Druid.Parse (data[Strings.BaseShowCommandId]);
		}

		
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public Command BaseShowCommand
		{
			get
			{
				return Command.Find (this.baseShowCommandId);
			}
		}

		public string IconOverlayUri
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}

		
		public XElement Save(string xmlNodeName)
		{
			List<XAttribute> attributes = new List<XAttribute> ();

			this.Serialize (attributes);

			return new XElement (xmlNodeName, attributes);
		}

		public static DataSetMetadata Restore(XElement xml)
		{
			var data     = xml.Attributes ().ToDictionary (x => x.Name.LocalName, x => x.Value);
			var metadata = new DataSetMetadata (data);

			return metadata;
		}

		private void Serialize(List<XAttribute> attributes)
		{
			attributes.Add (new XAttribute (Strings.Name, this.name));
			attributes.Add (new XAttribute (Strings.BaseShowCommandId, this.baseShowCommandId.ToCompactString ()));
		}


		#region Strings Class

		private static class Strings
		{
			public static readonly string		Name = "name";
			public static readonly string		BaseShowCommandId = "CMD.show";
		}

		#endregion


		private readonly string name;
		private readonly Druid baseShowCommandId;
	}
}
