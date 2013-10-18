//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditorPageCompta : AbstractObjectEditorPage
	{
		public ObjectEditorPageCompta(DataAccessor accessor)
			: base (accessor)
		{
		}


		protected override void CreateUI(Widget parent)
		{
			if (this.properties != null)
			{
			}
		}
	}
}
