//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Epsitec.Common.Drawing;

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
			FormattedText searchTitle  = this.settings.SearchTitle;
			FormattedText buttonTitle  = this.settings.ButtonTitle;
			System.Action buttonAction = this.settings.ButtonAction;

			var search = new SearchPicker ()
			{
				Margins = new Margins (4, 4, 4, 0)
			};

			var messageEmpty = new StaticText ()
			{
				Parent = search.MessageContainerStateEmpty,
				Dock = DockStyle.Fill,
				ContentAlignment = ContentAlignment.MiddleCenter,
				FormattedText = new FormattedText ("<i>Aucun résultat</i>"),
			};

			var messageError = new StaticText ()
			{
				Parent = search.MessageContainerStateError,
				Dock = DockStyle.Fill,
				ContentAlignment = ContentAlignment.MiddleCenter,
				FormattedText = new FormattedText ("<i>Trop de résultats</i>"),
			};
			
			var button = new Button ()
			{
				FormattedText = buttonTitle,
			};

			button.Clicked += delegate
			{
				buttonAction ();
			};

			search.SearchTextChanged += _ => search.State = SearchPickerState.Busy;
			search.SearchClicked     += _ => search.State = SearchPickerState.Error;
			
			search.State = SearchPickerState.Empty;

			builder.Add (container, search);
			builder.Add (container, button);
		}
	}
}
