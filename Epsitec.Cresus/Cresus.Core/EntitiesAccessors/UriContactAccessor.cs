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
	public class UriContactAccessor : AbstractAccessor
	{
		public UriContactAccessor(AbstractEntity entity)
			: base(entity)
		{
		}


		public Entities.UriContactEntity UriContact
		{
			get
			{
				return this.Entity as Entities.UriContactEntity;
			}
		}


		public override string Icon
		{
			get
			{
				return "Data.Uri";
			}
		}

		public override string Title
		{
			get
			{
				return "Mail";
			}
		}

		public override string Summary
		{
			get
			{
				var builder = new StringBuilder ();

				builder.Append (this.UriContact.Uri);
				AbstractAccessor.AppendRoles (builder, this.UriContact.Roles);
				builder.Append ("<br/>");

				return AbstractAccessor.SummaryPostprocess (builder.ToString ());
			}
		}


		public string UriScheme
		{
			get
			{
				if (this.UriContact.UriScheme != null)
				{
					return this.UriContact.UriScheme.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.UriContact.UriScheme == null)
				{
					this.UriContact.UriScheme = new Entities.UriSchemeEntity ();
				}

				this.UriContact.UriScheme.Name = value;
			}
		}
	}
}
