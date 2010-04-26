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
	public class TelecomContactAccessor : AbstractAccessor
	{
		public TelecomContactAccessor(AbstractEntity entity)
			: base(entity)
		{
		}


		public Entities.TelecomContactEntity TelecomContact
		{
			get
			{
				return this.Entity as Entities.TelecomContactEntity;
			}
		}


		public override string Icon
		{
			get
			{
				return "Data.Telecom";
			}
		}

		public override string Title
		{
			get
			{
				return "Téléphone";
			}
		}

		public override string Summary
		{
			get
			{
				var builder = new StringBuilder ();

				builder.Append (this.TelecomContact.Number);
				AbstractAccessor.AppendRoles (builder, this.TelecomContact.Roles);
				builder.Append ("<br/>");

				return AbstractAccessor.SummaryPostprocess (builder.ToString ());
			}
		}


		public string TelecomType
		{
			get
			{
				if (this.TelecomContact.TelecomType != null)
				{
					return this.TelecomContact.TelecomType.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.TelecomContact.TelecomType == null)
				{
					this.TelecomContact.TelecomType = new Entities.TelecomTypeEntity ();
				}

				this.TelecomContact.TelecomType.Name = value;
			}
		}
	}
}
