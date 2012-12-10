//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>DataSetMetadata</c> class defines metadata for a data set (which is what the
	/// user understands as a database, such as "customers", "invoices", etc.).
	/// </summary>
	public class DataSetMetadata : CoreMetadata
	{
		public DataSetMetadata(Druid entityId, string name, bool isDefault)
		{
			this.entityId = entityId;
			this.name = name;
			this.isDefault = isDefault;

			this.dataSetName = EntityInfo.GetStructuredType (entityId).Caption.Name;
			this.dataSetEntityType = DataSetGetter.GetRootEntityType (this.dataSetName);
			this.baseShowCommand   = DataSetMetadata.ResolveShowCommand (this.dataSetName);

			this.userRoles = new List<string> ();
		}

		public DataSetMetadata(IDictionary<string, string> data)
			: this (Druid.Parse (data[Strings.EntityId]), data[Strings.Name], bool.Parse (data[Strings.IsDefault]))
		{
			this.DefineDisplayGroup (Druid.Parse (data[Strings.DisplayGroup]));
		}

		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public Druid							EntityId
		{
			get
			{
				return this.entityId;
			}
		}

		public bool								IsDefault
		{
			get
			{
				return this.isDefault;
			}
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

		public IList<string>					UserRoles
		{
			get
			{
				return this.userRoles;
			}
		}

		public EntityFilter						Filter
		{
			get
			{
				return this.filter;
			}
		}

		public EntityTableMetadata				EntityTableMetadata
		{
			get
			{
				return this.entityTableMetadata;
			}
		}


		public bool MatchesUserRole(string role)
		{
			return this.userRoles.Contains (role);
		}

		public bool MatchesAnyUserRole(IEnumerable<string> roles)
		{
			foreach (var role in roles)
			{
				if (this.MatchesUserRole (role))
				{
					return true;
				}
			}

			return false;
		}


		public void DefineDisplayGroup(Druid captionId)
		{
			this.displayGroupCaptionId = captionId;
		}

		public void DefineFilter(EntityFilter filter)
		{
			this.filter = filter;
		}

		public void DefineEntityTableMetadata(EntityTableMetadata entityTableMetadata)
		{
			this.entityTableMetadata = entityTableMetadata;
		}

		public override void Add(CoreMetadata metadata)
		{
			throw new System.NotImplementedException ();
		}
		
		public XElement Save(string xmlNodeName)
		{
			List<XAttribute> attributes = new List<XAttribute> ();

			this.Serialize (attributes);

			var roles = new XElement (Strings.UserRoles,
				from role in this.userRoles
				select new XElement (Strings.UserRole, new XText (role)));

			var filter = this.filter.Save (Strings.Filter);

			return new XElement (xmlNodeName, attributes, roles, filter);
		}

		public static DataSetMetadata Restore(XElement xml, IEnumerable<EntityTableMetadata> tables)
		{
			var data     = Xml.GetAttributeBag (xml);
			var roles    = xml.Element (Strings.UserRoles).Elements (Strings.UserRole).Select (x => x.Value);
			var filter   = xml.Element (Strings.Filter);
			var metadata = new DataSetMetadata (data);

			metadata.userRoles.AddRange (roles);
			metadata.DefineFilter (EntityFilter.Restore (filter));
			metadata.DefineEntityTableMetadata (tables.Single (t => t.Name == metadata.Name && t.EntityId == metadata.EntityId));

			return metadata;
		}

		private void Serialize(List<XAttribute> attributes)
		{
			attributes.Add (new XAttribute (Strings.EntityId, this.entityId.ToCompactString ()));
			attributes.Add (new XAttribute (Strings.Name, this.name));
			attributes.Add (new XAttribute (Strings.IsDefault, this.isDefault.ToString ()));
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
			public static readonly string		EntityId = "eid";
			public static readonly string		Name = "n";
			public static readonly string		IsDefault = "df";
			public static readonly string		DisplayGroup = "dg";
			public static readonly string		UserRoles = "R";
			public static readonly string		UserRole = "r";
			public static readonly string		Filter = "f";
		}

		#endregion


		private readonly Druid					entityId;
		private readonly string					name;
		private readonly bool					isDefault;
		private readonly string					dataSetName;
		private readonly System.Type			dataSetEntityType;
		private readonly Command				baseShowCommand;
		private readonly List<string>			userRoles;

		private Druid							displayGroupCaptionId;
		private EntityFilter					filter;
		private EntityTableMetadata				entityTableMetadata;
	}
}
