//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractView
	{
		public virtual void CreateUI(Widget parent)
		{
		}


		public static AbstractView CreateView(ViewType viewType)
		{
			switch (viewType)
			{
				case ViewType.Objects:
					return new ObjectsView ();

				default:
					return null;
			}
		}
	}
}
