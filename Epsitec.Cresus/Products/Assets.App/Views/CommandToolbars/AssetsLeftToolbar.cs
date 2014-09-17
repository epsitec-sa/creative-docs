//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	/// <summary>
	/// Toolbar utilisée pour montrer les objets d'immobilisations en mode "plusieurs
	/// timelines", dans la partie gauche.
	/// </summary>
	public class AssetsLeftToolbar : AbstractTreeTableToolbar
	{
		public AssetsLeftToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.buttonFilter     = this.CreateCommandButton (DockStyle.None, Res.Commands.AssetsLeft.Filter);

			this.buttonFirst      = this.CreateCommandButton (DockStyle.None, Res.Commands.AssetsLeft.First);
			this.buttonPrev       = this.CreateCommandButton (DockStyle.None, Res.Commands.AssetsLeft.Prev);
			this.buttonNext       = this.CreateCommandButton (DockStyle.None, Res.Commands.AssetsLeft.Next);
			this.buttonLast       = this.CreateCommandButton (DockStyle.None, Res.Commands.AssetsLeft.Last);

			this.separator1       = this.CreateSeparator     (DockStyle.None);

			this.buttonCompactAll = this.CreateCommandButton (DockStyle.None, Res.Commands.AssetsLeft.CompactAll);
			this.buttonCompactOne = this.CreateCommandButton (DockStyle.None, Res.Commands.AssetsLeft.CompactOne);
			this.buttonExpandOne  = this.CreateCommandButton (DockStyle.None, Res.Commands.AssetsLeft.ExpandOne);
			this.buttonExpandAll  = this.CreateCommandButton (DockStyle.None, Res.Commands.AssetsLeft.ExpandAll);
			
			this.separator2       = this.CreateSeparator     (DockStyle.None);
			this.separator3       = this.CreateSeparator     (DockStyle.None);

			this.buttonNew        = this.CreateCommandButton (DockStyle.None, Res.Commands.AssetsLeft.New);
			this.buttonDelete     = this.CreateCommandButton (DockStyle.None, Res.Commands.AssetsLeft.Delete);
			this.buttonDeselect   = this.CreateCommandButton (DockStyle.None, Res.Commands.AssetsLeft.Deselect);

			this.separator4       = this.CreateSeparator     (DockStyle.None);
		}
	}
}
