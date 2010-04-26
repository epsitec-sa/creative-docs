//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.EntitiesAccessors
{
	public class AbstractAccessor
	{
		public AbstractAccessor(AbstractEntity entity)
		{
			System.Diagnostics.Debug.Assert (entity != null);
			this.entity = entity;
		}


		public AbstractEntity Entity
		{
			get
			{
				return this.entity;
			}
		}


		public virtual string Icon
		{
			get
			{
				return "?";
			}
		}

		public virtual string Title
		{
			get
			{
				return "?";
			}
		}

		public virtual string Summary
		{
			get
			{
				return null;
			}
		}


		protected static string SummaryPostprocess(string summary)
		{
			summary = Misc.RemoveLastBreakLine (summary);

			if (string.IsNullOrEmpty (summary))
			{
				summary = "<i>(vide)</i>";
			}

			return summary;
		}

		protected static void AppendRoles(StringBuilder builder, IList<Entities.ContactRoleEntity> roles)
		{
			if (roles != null && roles.Count != 0)
			{
				builder.Append (" (");

				bool first = true;
				foreach (Entities.ContactRoleEntity role in roles)
				{
					if (!first)
					{
						builder.Append (", ");
					}

					builder.Append (role.Name);
					first = false;
				}

				builder.Append (")");

			}
		}


		private readonly AbstractEntity entity;
	}
}
