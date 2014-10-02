//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir les options pour la recherche.
	/// </summary>
	public class SearchOptionsPopup : StackedPopup
	{
		public SearchOptionsPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.SearchController.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.SearchController.Options.IgnoreCase.ToString (),
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.SearchController.Options.IgnoreDiacritic.ToString (),
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.SearchController.Options.Phonetic.ToString (),
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.SearchController.Options.WholeWords.ToString (),
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.SearchController.Options.FullText.ToString (),
			});

			list.Add (new StackedControllerDescription  // 5
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.SearchController.Options.Regex.ToString (),
			});

			this.SetDescriptions (list);
		}


		public SearchOptions Options
		{
			get
			{
				var options = SearchOptions.Unknown;

				if (this.IgnoreCase     )  options |= SearchOptions.IgnoreCase;
				if (this.IgnoreDiacritic)  options |= SearchOptions.IgnoreDiacritic;
				if (this.Phonetic       )  options |= SearchOptions.Phonetic;
				if (this.WholeWords     )  options |= SearchOptions.WholeWords;
				if (this.FullText       )  options |= SearchOptions.FullText;
				if (this.Regex          )  options |= SearchOptions.Regex;

				return options;
			}
			set
			{
				this.IgnoreCase      = (value & SearchOptions.IgnoreCase     ) != 0;
				this.IgnoreDiacritic = (value & SearchOptions.IgnoreDiacritic) != 0;
				this.Phonetic        = (value & SearchOptions.Phonetic       ) != 0;
				this.WholeWords      = (value & SearchOptions.WholeWords     ) != 0;
				this.FullText        = (value & SearchOptions.FullText       ) != 0;
				this.Regex           = (value & SearchOptions.Regex          ) != 0;
			}
		}

		private bool IgnoreCase
		{
			get
			{
				var controller = this.GetController (0) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (0) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private bool IgnoreDiacritic
		{
			get
			{
				var controller = this.GetController (1) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (1) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private bool Phonetic
		{
			get
			{
				var controller = this.GetController (2) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (2) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private bool WholeWords
		{
			get
			{
				var controller = this.GetController (3) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (3) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private bool FullText
		{
			get
			{
				var controller = this.GetController (4) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (4) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private bool Regex
		{
			get
			{
				var controller = this.GetController (5) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (5) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}



	}
}