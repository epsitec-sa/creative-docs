//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Bricks.DynamicFactories
{
	public abstract class StaticFactory
	{
		protected StaticFactory(BusinessContext businessContext, ExpandoObject settings)
		{
			this.businessContext = businessContext;
			this.settings        = settings;
		}
		
		public abstract void CreateUI(FrameBox container, UIBuilder builder);

		protected readonly BusinessContext businessContext;
		protected readonly dynamic settings;
	}

	public sealed class ButtonFactory : StaticFactory
	{
		public ButtonFactory(BusinessContext businessContext, ExpandoObject settings)
			: base (businessContext, settings)
		{
		}

		public override void CreateUI(FrameBox container, UIBuilder builder)
		{
			FormattedText buttonTitle = this.settings.ButtonTitle;
			FormattedText buttonDescription = this.settings.ButtonDescription;
			System.Action buttonAction = this.settings.ButtonAction;

			var button = new ConfirmationButton
			{
				FormattedText = ConfirmationButton.FormatContent (buttonTitle, buttonDescription),
				PreferredHeight = 52,
			};

			button.Clicked += delegate
			{
				buttonAction ();
			};

			builder.Add (container, button);
		}
	}
	
	public sealed class SearchPanelFactory : StaticFactory
	{
		public SearchPanelFactory(BusinessContext businessContext, ExpandoObject settings)
			: base (businessContext, settings)
		{
		}

		public override void CreateUI(FrameBox container, UIBuilder builder)
		{
		}
	}
}
