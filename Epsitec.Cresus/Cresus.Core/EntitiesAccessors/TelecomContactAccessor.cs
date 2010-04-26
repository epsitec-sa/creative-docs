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
	public class TelecomContactAccessor : AbstractContactAccessor
	{
		public TelecomContactAccessor(AbstractEntity entity, bool grouped)
			: base (entity, grouped)
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
				if (this.Grouped)
				{
					return "Téléphone";
				}
				else
				{
					var builder = new StringBuilder ();

					builder.Append ("Téléphone");
					builder.Append (Misc.Encapsulate (" ", this.Roles, ""));

					return Misc.RemoveLastBreakLine (builder.ToString ());
				}
			}
		}

		public override string Summary
		{
			get
			{
				var builder = new StringBuilder ();

				builder.Append (this.TelecomContact.Number);
				
				if (this.Grouped)
				{
					builder.Append (Misc.Encapsulate (" (", this.Roles, ")"));
				}
				
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
