//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EcrituresView : AbstractView
	{
		public EcrituresView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
		}

		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);
		}
	}
}
