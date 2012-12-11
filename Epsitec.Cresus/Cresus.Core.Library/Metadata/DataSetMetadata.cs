//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

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
		public DataSetMetadata(Druid commandId, Druid tableEntityId, string tableName, bool isDefault, bool isDisplayed)
		{
			this.command = Command.Find (commandId);

			if (this.command == null)
			{
				var message = string.Format ("Show command '{0}' could not be located.", commandId);
				throw new System.ArgumentException (message);
			}

			this.tableEntityId = tableEntityId;
			this.tableName = tableName;
			this.isDefault = isDefault;
			this.isDisplayed = isDisplayed;

			this.userRoles = new List<string> ();
		}

		public DataSetMetadata(IDictionary<string, string> data)
			: this
		(
			Druid.Parse(data[Strings.CommandId]),
			Druid.Parse (data[Strings.TableEntityId]),
			data[Strings.TableName],
			bool.Parse (data[Strings.IsDefault]),
			bool.Parse (data[Strings.IsDisplayed])
		)
		{
			this.DefineDisplayGroup (Druid.Parse (data[Strings.DisplayGroup]));
		}




		public Command							Command
		{
			get
			{
				return this.command;
			}
		}

		public Druid							TableEntityId
		{
			get
			{
				return this.tableEntityId;
			}
		}
		
		public string							TableName
		{
			get
			{
				return this.tableName;
			}
		}

		public bool								IsDefault
		{
			get
			{
				return this.isDefault;
			}
		}

		public bool								IsDisplayed
		{
			get
			{
				return this.isDisplayed;
			}
		}

		public IList<string> UserRoles
		{
			get
			{
				return this.userRoles;
			}
		}

		public Druid							DisplayGroupId
		{
			get
			{
				return this.displayGroupCaptionId;
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
			metadata.DefineEntityTableMetadata (tables.Single (t => t.Name == metadata.TableName && t.EntityId == metadata.TableEntityId));

			return metadata;
		}

		private void Serialize(List<XAttribute> attributes)
		{
			attributes.Add (new XAttribute (Strings.CommandId, this.command.Caption.Id.ToCompactString ()));
			attributes.Add (new XAttribute (Strings.TableEntityId, this.tableEntityId.ToCompactString ()));
			attributes.Add (new XAttribute (Strings.TableName, this.tableName));
			attributes.Add (new XAttribute (Strings.IsDefault, this.isDefault.ToString ()));
			attributes.Add (new XAttribute (Strings.IsDisplayed, this.isDisplayed.ToString ()));
			attributes.Add (new XAttribute (Strings.DisplayGroup, this.displayGroupCaptionId.ToCompactString ()));
		}


		#region Strings Class

		private static class Strings
		{
			public static readonly string		CommandId = "cid";
			public static readonly string		TableEntityId = "teid";
			public static readonly string		TableName = "tn";
			public static readonly string		IsDefault = "df";
			public static readonly string		IsDisplayed = "d";
			public static readonly string		DisplayGroup = "dg";
			public static readonly string		UserRoles = "R";
			public static readonly string		UserRole = "r";
			public static readonly string		Filter = "f";
		}

		#endregion


		private readonly Command				command;
		private readonly Druid					tableEntityId;
		private readonly string					tableName;
		private readonly bool					isDefault;
		private readonly bool					isDisplayed;
		private readonly List<string>			userRoles;

		private Druid							displayGroupCaptionId;
		private EntityFilter					filter;
		private EntityTableMetadata				entityTableMetadata;
	}
}
