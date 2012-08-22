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
		public DataSetMetadata(string dataSetName)
		{
			this.dataSetName = dataSetName;
			this.dataSetEntityType = DataSetGetter.GetRootEntityType (this.dataSetName);
			this.baseShowCommand = DataSetMetadata.ResolveShowCommand (this.dataSetName);

		}

		public DataSetMetadata(IDictionary<string, string> data)
			: this (data[Strings.Name])
		{
			this.DefineDisplayGroup (Druid.Parse (data[Strings.DisplayGroup]));
		}

		
		public string							DataSetName
		{
			get
			{
				return this.dataSetName;
			}
		}

		public System.Type						DataSetEntityType
		{
			get
			{
				return this.dataSetEntityType;
			}
		}


		public Command							BaseShowCommand
		{
			get
			{
				return this.baseShowCommand;
			}
		}

		public string							EntityIconUri
		{
			get
			{
				return IconProvider.GetEntityIconUri ("Base." + this.DataSetName, this.DataSetEntityType);
			}
		}

		public Druid							DisplayGroupId
		{
			get
			{
				return this.displayGroupCaptionId;
			}
		}



		public void DefineDisplayGroup(Druid captionId)
		{
			this.displayGroupCaptionId = captionId;
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
			attributes.Add (new XAttribute (Strings.Name, this.dataSetName));
			attributes.Add (new XAttribute (Strings.DisplayGroup, this.displayGroupCaptionId.ToCompactString ()));
		}


		private static Command ResolveShowCommand(string dataSetName)
		{
			var showName = "Base.Show" + dataSetName;
			var showCmd  = Command.FindByName (showName);

			System.Diagnostics.Debug.Assert (showCmd != null, string.Format ("Show command could not be located for data set '{0}'", dataSetName));

			return showCmd;
		}


		#region Strings Class

		private static class Strings
		{
			public static readonly string		Name = "name";
			public static readonly string		DisplayGroup = "disp.grp";
		}

		#endregion


		private readonly string dataSetName;
		private readonly Command baseShowCommand;
		private readonly System.Type dataSetEntityType;

		private Druid displayGroupCaptionId;
	}
}
