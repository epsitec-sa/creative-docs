//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Fields.Controllers
{
	/// <summary>
	/// Contrôleur générique affichant un texte fixe.
	/// </summary>
	public class FixedTextController : AbstractFieldController
	{
		public FixedTextController(AbstractController controller, int line, ColumnMapper columnMapper, System.Action<int, ColumnType> clearFocusAction, System.Action<int, ColumnType> setFocusAction, System.Action<int, ColumnType> contentChangedAction)
			: base (controller, line, columnMapper, clearFocusAction, setFocusAction, contentChangedAction)
		{
		}


		public System.Func<FormattedText, FormattedText> TextConverter
		{
			get
			{
				return this.textConverter;
			}
			set
			{
				this.textConverter = value;
			}
		}

		public override void CreateUI(Widget parent)
		{
			this.CreateBoxUI (parent);
			this.box.DrawFullFrame = false;

			int rightMargin = (columnMapper.Alignment == ContentAlignment.MiddleRight) ? 5 : 0;

			this.editWidget = new StaticText
			{
				Parent           = this.box,
				ContentAlignment = columnMapper.Alignment,
				PreferredHeight  = 20,
				Dock             = DockStyle.Fill,
				Margins          = new Margins (0, rightMargin, 0, 2),
			};
		}

		public override void EditionDataToWidget()
		{
			if (this.editionData != null)
			{
				if (this.textConverter == null)
				{
					this.InternalField.FormattedText = this.editionData.Text;
				}
				else
				{
					this.InternalField.FormattedText = this.textConverter (this.editionData.Text);
				}
			}
		}


		private StaticText InternalField
		{
			get
			{
				return this.editWidget as StaticText;
			}
		}


		private System.Func<FormattedText, FormattedText> textConverter;
	}
}
