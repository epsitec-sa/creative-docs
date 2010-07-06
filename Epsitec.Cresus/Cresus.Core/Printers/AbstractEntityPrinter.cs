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


		public static void PaintText(IPaintPort port, string text, Rectangle bounds, Font font, double fontSize)
		{
			PaintText (port, text, 0, bounds, font, fontSize);
		}

		public static int PaintText(IPaintPort port, string text, int firstLine, Rectangle bounds, Font font, double fontSize)
		{
			if (firstLine == -1)
			{
				return -1;
			}

			var textLayout = new TextLayout ()
			{
				Alignment = ContentAlignment.TopLeft,
				JustifMode = TextJustifMode.None,
				BreakMode = TextBreakMode.Hyphenate,
				DefaultFont = font,
				DefaultFontSize = fontSize,
				LayoutSize = new Size (bounds.Width, double.MaxValue),
				DefaultRichColor = RichColor.FromBrightness (0),
				Text = text,
			};

			int[] index = textLayout.GetLinesIndex ();

			if (firstLine >= index.Length)
			{
				return -1;
			}

			textLayout.Text = text.Substring (index[firstLine]);
			textLayout.LayoutSize = bounds.Size;
			textLayout.Paint (bounds.BottomLeft, port);

			firstLine += textLayout.VisibleLineCount;

			if (firstLine >= index.Length)
			{
				return -1;
			}

			return firstLine;
		}
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
