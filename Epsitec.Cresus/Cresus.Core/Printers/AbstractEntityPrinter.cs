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
	public abstract class AbstractEntityPrinter
	{
		public virtual string JobName
		{
			get
			{
				return null;
			}
		}

		public virtual Size PageSize
		{
			get
			{
				return Size.Zero;
			}
		}

		public virtual Margins PageMargins
		{
			get
			{
				return new Margins (10);
			}
		}

		public virtual void Print(IPaintPort port, Rectangle bounds)
		{
		}


		public static AbstractEntityPrinter CreateEntityPrinter(AbstractEntity entity)
		{
			var type = AbstractEntityPrinter.FindType (entity.GetType ());

			if (type == null)
			{
				return null;
			}

			return System.Activator.CreateInstance (type, new object[] { entity }) as AbstractEntityPrinter;
		}
		
		private static System.Type FindType(System.Type entityType)
		{
			var baseTypeName = "AbstractEntityPrinter`1";

			//	Find all concrete classes which use either the generic AbstractEntityPrinter base classes,
			//	which match the entity type (usually, there should be exactly one such type).

			var types = from type in typeof (AbstractEntityPrinter).Assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName) && baseType.GetGenericArguments ()[0] == entityType
						select type;

			return types.FirstOrDefault ();
		}


		#region Painting tools
		public static void PaintText(IPaintPort port, string text, Rectangle bounds, Font font, double fontSize)
		{
			PaintText (port, text, 0, bounds, font, fontSize);
		}

		public static int PaintText(IPaintPort port, string text, int firstLine, Rectangle bounds, Font font, double fontSize, ContentAlignment alignment = ContentAlignment.TopLeft, TextJustifMode justif = TextJustifMode.None, TextBreakMode breakMode = TextBreakMode.Hyphenate)
		{
			//	Dessine un texte dans un pavé à partir d'une ligne donnée. Retourne le numéro de la première ligne
			//	pour le pavé suivant, ou -1 s'il n'y en a pas.
			if (firstLine == -1)
			{
				return -1;
			}

			//	Crée un pavé à la bonne largeur mais de hauteur infinie, pour pouvoir calculer les hauteurs
			//	de toutes les lignes.
			var textLayout = new TextLayout ()
			{
				Alignment = alignment,
				JustifMode = justif,
				BreakMode = breakMode,
				DefaultFont = font,
				DefaultFontSize = fontSize,
				LayoutSize = new Size (bounds.Width, double.MaxValue),
				DefaultRichColor = RichColor.FromBrightness (0),
				Text = text,
			};

			int lineCount = textLayout.TotalLineCount;
			double[] heights = new double[lineCount];

			for (int i = 0; i < lineCount; i++)
			{
				heights[i] = textLayout.GetLineHeight (i);
			}

			//	Calcule la distance verticale correspondant aux lignes à ne pas afficher.
			double verticalOffset = 0;
			for (int i = 0; i < firstLine; i++)
			{
				verticalOffset += heights[i];
			}

			Rectangle clipRect = bounds;  // clipping sur le rectangle demandé
			bounds.Top += verticalOffset;  // remonte le début, qui sera clippé

			//	Adapte le pavé avec les données réelles et dessine-le.
			textLayout.LayoutSize = bounds.Size;
			textLayout.Paint (bounds.BottomLeft, port, clipRect, Color.FromBrightness (0), GlyphPaintStyle.Normal);

			//	Calcul l'index de la première ligne suivante.
			firstLine = textLayout.VisibleLineCount;

			if (firstLine >= lineCount)
			{
				return -1;
			}

			return firstLine;
		}
		#endregion
	}

	public class AbstractEntityPrinter<T> : AbstractEntityPrinter
		where T : AbstractEntity
	{
		public AbstractEntityPrinter(T entity)
		{
			this.entity = entity;
		}

		protected T entity;
	}
}
