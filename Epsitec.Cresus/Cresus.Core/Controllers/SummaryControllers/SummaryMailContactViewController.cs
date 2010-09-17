﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryMailContactViewController : SummaryViewController<Entities.MailContactEntity>
	{
		public SummaryMailContactViewController(string name, Entities.MailContactEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIMail (data);
			}
		}

		private void CreateUIMail(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "MailContact",
					IconUri				= "Data.Mail",
					Title				= TextFormatter.FormatText ("Adresse", "(", string.Join (", ", this.Entity.Roles.Select (role => role.Name)), ")"),
					CompactTitle		= TextFormatter.FormatText ("Adresse"),
					TextAccessor		= this.CreateAccessor (x => SummaryMailContactViewController.GetMailContactSummary (x)),
					CompactTextAccessor = this.CreateAccessor (x => SummaryMailContactViewController.GetCompactMailContactSummary (x)),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}


		private static FormattedText GetMailContactSummary(MailContactEntity x)
		{
			return TextFormatter.FormatText (x.LegalPerson.Name, "\n",
										 x.LegalPerson.Complement, "\n",
										 string.Join (" ", x.NaturalPerson.Firstname, x.NaturalPerson.Lastname), "\n",
										 x.Complement, "\n",
										 x.Address.Street.StreetName, "\n",
										 x.Address.Street.Complement, "\n",
										 x.Address.PostBox.Number, "\n",
										 x.Address.Location.Country.Code, "~-", x.Address.Location.PostalCode, x.Address.Location.Name);
		}

		private static FormattedText GetCompactMailContactSummary(MailContactEntity x)
		{
			return TextFormatter.FormatText (x.Address.Street.StreetName, "~,",
										 string.Join (" ", x.NaturalPerson.Firstname, x.NaturalPerson.Lastname), "~,",
										 x.Address.Location.PostalCode, x.Address.Location.Name);
		}
	}
}
