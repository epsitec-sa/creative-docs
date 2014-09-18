//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	/// <summary>
	/// La toolbar s'adapte en fonction de la largeur disponible. Certains
	/// boutons non indispensables disparaissent s'il manque de la place.
	/// </summary>
	public abstract class AbstractTreeTableToolbar : AbstractCommandToolbar
	{
		public AbstractTreeTableToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


	}
}
