//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Epsitec.Cresus.Core.Bricks.Factories
{
	public sealed class SearchPanelFactory : StaticFactory
	{
		public SearchPanelFactory(BusinessContext businessContext, ExpandoObject settings)
			: base (businessContext, settings)
		{
		}

		public override void CreateUI(FrameBox container, UIBuilder builder)
		{
#if false
			FormattedText searchTitle  = this.settings.SearchTitle;
			FormattedText buttonTitle  = this.settings.ButtonTitle;
			System.Action buttonAction = this.settings.ButtonAction;

			var search = new SearchBox ()
			{
			};
			
			var button = new Button ()
			{
				FormattedText = buttonTitle,
			};

			button.Clicked += delegate
			{
				buttonAction ();
			};

			builder.Add (container, search);
			builder.Add (container, button);
#endif
		}
	}
}
