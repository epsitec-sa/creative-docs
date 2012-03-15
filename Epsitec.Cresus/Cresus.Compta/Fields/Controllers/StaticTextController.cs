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
	public class StaticTextController : AbstractFieldController
	{
		public StaticTextController(AbstractController controller, int line, ColumnMapper columnMapper, System.Action<int, ColumnType> setFocusAction = null, System.Action<int, ColumnType> contentChangedAction = null)
			: base (controller, line, columnMapper, setFocusAction, contentChangedAction)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateBoxUI (parent);
			this.box.DrawFullFrame = false;

			int rightMargin = (columnMapper.Alignment == ContentAlignment.MiddleRight) ? 5 : 0;

			new StaticText
			{
				Parent           = this.box,
				FormattedText    = columnMapper.Description,
				ContentAlignment = columnMapper.Alignment,
				PreferredHeight  = 20,
				Dock             = DockStyle.Fill,
				Margins          = new Margins (0, rightMargin, 0, 2),
			};
		}
	}
}
