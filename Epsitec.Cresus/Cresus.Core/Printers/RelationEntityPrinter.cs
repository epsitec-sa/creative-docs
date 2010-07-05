//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{

	public class RelationEntityPrinter : AbstractEntityPrinter
	{
		public RelationEntityPrinter(AbstractEntity entity)
			: base (entity)
		{
		}

		public override string JobName
		{
			get
			{
				return string.Format("Client {0}", this.Entity.Id);
			}
		}

		public override Size PageSize
		{
			get
			{
				return new Size (210, 297);  // A4 vertical
			}
		}

		public override void Print(IPaintPort port, Rectangle bounds)
		{
			var entity = this.Entity;
			string text = "?";

			if (entity.Person is NaturalPersonEntity)
			{
				var naturalPerson = entity.Person as NaturalPersonEntity;

				text = naturalPerson.Firstname + " " + naturalPerson.Lastname;  // TODO: provisoire
			}

			if (entity.Person is LegalPersonEntity)
			{
				var legalPerson = entity.Person as LegalPersonEntity;

				text = legalPerson.Name;  // TODO: provisoire
			}

			port.PaintText (10, 10, text, Font.DefaultFont, Font.DefaultFontSize);
		}


		private RelationEntity Entity
		{
			get
			{
				return this.entity as RelationEntity;
			}
		}
	}
}
