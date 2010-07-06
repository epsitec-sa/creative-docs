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

	public class RelationEntityPrinter<T> : AbstractEntityPrinter<RelationEntity>
	{
		public RelationEntityPrinter(RelationEntity entity)
			: base (entity)
		{
		}

		public override string JobName
		{
			get
			{
				return UIBuilder.FormatText ("Client", this.entity.Id).ToSimpleText ();
			}
		}

		public override Size PageSize
		{
			get
			{
				return new Size (pageWidth, pageHeight);  // A4 vertical
			}
		}

		public override void Print(IPaintPort port, Rectangle bounds)
		{
			string text = "?";

			if (entity.Person is NaturalPersonEntity)
			{
				var x = this.entity.Person as NaturalPersonEntity;

				text = UIBuilder.FormatText ("N°", this.entity.Id, "\n", x.Title.Name, "\n", x.Firstname, x.Lastname, "(", x.Gender.Name, ")", "\n", x.BirthDate).ToSimpleText ();
			}

			if (entity.Person is LegalPersonEntity)
			{
				var x = this.entity.Person as LegalPersonEntity;

				text = UIBuilder.FormatText (x.Name).ToSimpleText ();
			}

			var textLayout = new TextLayout ();
			textLayout.JustifMode = TextJustifMode.AllButLast;
			textLayout.BreakMode = TextBreakMode.Ellipsis;
			textLayout.DefaultFont = Font.DefaultFont;
			textLayout.DefaultFontSize = fontSize;
			textLayout.LayoutSize = new Size (150, 150);
			textLayout.DefaultRichColor = RichColor.FromBrightness (0);
			textLayout.Text = text.Replace ("\n", "<br/>");

			textLayout.Paint (new Point (10, pageHeight-10-150), port);
		}


		private static readonly double pageWidth  = 210;
		private static readonly double pageHeight = 297;  // A4 vertical

		private static readonly double fontSize = 4;
	}
}
