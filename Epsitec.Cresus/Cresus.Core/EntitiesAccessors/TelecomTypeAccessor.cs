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
	public class TelecomTypeAccessor : AbstractAccessor
	{
		public TelecomTypeAccessor(object parentEntities, AbstractEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
		}


		public Entities.TelecomContactEntity TelecomContact
		{
			get
			{
				return this.Entity as Entities.TelecomContactEntity;
			}
		}


		public override string IconUri
		{
			get
			{
				return "Data.Type";
			}
		}

		public override string Title
		{
			get
			{
				return "Type";
			}
		}

		public override string Summary
		{
			get
			{
				var builder = new StringBuilder ();

				builder.Append (this.TelecomType);
				builder.Append ("<br/>");

				return AbstractAccessor.SummaryPostprocess (builder.ToString ());
			}
		}


		public ComboInitializer TelecomTypeInitializer
		{
			get
			{
				ComboInitializer initializer = new ComboInitializer ();

				initializer.Content.Add ("fixnet", "Fixe");
				initializer.Content.Add ("mobile", "Mobile");
				initializer.Content.Add ("fax",    "Fax");

				return initializer;
			}
		}


		public string TelecomType
		{
			get
			{
				if (this.TelecomContact.TelecomType != null)
				{
					return this.TelecomTypeInitializer.ConvertInternalToEdition (this.TelecomContact.TelecomType.Code);
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

				this.TelecomContact.TelecomType.Code = this.TelecomTypeInitializer.ConvertEditionToInternal(value);
			}
		}
	}
}
