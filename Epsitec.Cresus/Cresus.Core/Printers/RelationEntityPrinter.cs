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

	public class RelationEntityPrinter : AbstractEntityPrinter<RelationEntity>
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
				text = UIBuilder.FormatText ("N°", this.entity.Id, "<br/>", x.Title.Name, "<br/>", x.Firstname, x.Lastname, "(", x.Gender.Name, ")", "<br/>", x.BirthDate).ToString ();
			}

			if (entity.Person is LegalPersonEntity)
			{
				var x = this.entity.Person as LegalPersonEntity;
				text = UIBuilder.FormatText (x.Name).ToSimpleText ();
			}

			//?AbstractEntityPrinter.PaintText (port, text, new Rectangle (this.PageMargins.Left, this.PageSize.Height-this.PageMargins.Top-150, 150, 150), Font.DefaultFont, fontSize);

#if true
			string t = "Ceci est un texte bidon mais <b>assez long</b>, pour permettre de tester le découpage en plusieurs pavés distincts, qui seront dessinés sur plusieurs pages.<br/><br/><i>Et voilà la suite et la fin de ce chef d'œuvre littéraire sur une toute nouvelle ligne.</i>";
			Point pos;
			int firstLine;

#if true
			pos = new Point (10, 200);
			firstLine = 0;
			while (true)
			{
				Rectangle b = new Rectangle (pos.X, pos.Y, 50, 25);
				firstLine = AbstractEntityPrinter.PaintText (port, t, firstLine, b, Font.DefaultFont, fontSize);

				port.LineWidth = 0.1;
				port.PaintOutline (Path.FromRectangle (b));

				if (firstLine == -1)
				{
					break;
				}

				pos.X += 50+1;
			}

			pos = new Point (10, 200-25-2);
			firstLine = 0;
			while (true)
			{
				Rectangle b = new Rectangle (pos.X, pos.Y, 30, 25);
				firstLine = AbstractEntityPrinter.PaintText (port, t, firstLine, b, Font.DefaultFont, fontSize);

				port.LineWidth = 0.1;
				port.PaintOutline (Path.FromRectangle (b));

				if (firstLine == -1)
				{
					break;
				}

				pos.X += 30+1;
			}

			pos = new Point (10, 200-25-2-25-2);
			firstLine = 0;
			while (true)
			{
				Rectangle b = new Rectangle (pos.X, pos.Y, 20, 25);
				firstLine = AbstractEntityPrinter.PaintText (port, t, firstLine, b, Font.DefaultFont, fontSize);

				port.LineWidth = 0.1;
				port.PaintOutline (Path.FromRectangle (b));

				if (firstLine == -1)
				{
					break;
				}

				pos.X += 20+1;
			}
#endif

			pos = new Point (10, 200-25-2-25-2-25-2);
			firstLine = 0;
			while (true)
			{
				Rectangle b = new Rectangle (pos.X, pos.Y, 50, 25);
				firstLine = AbstractEntityPrinter.PaintText (port, t, firstLine, b, Font.DefaultFont, fontSize, ContentAlignment.TopLeft, TextJustifMode.AllButLast, TextBreakMode.Hyphenate);

				port.LineWidth = 0.1;
				port.PaintOutline (Path.FromRectangle (b));

				if (firstLine == -1)
				{
					break;
				}

				pos.X += 50+1;
			}
#endif
		}


		private static readonly double pageWidth  = 210;
		private static readonly double pageHeight = 297;  // A4 vertical

		private static readonly double fontSize = 4;
	}
}
