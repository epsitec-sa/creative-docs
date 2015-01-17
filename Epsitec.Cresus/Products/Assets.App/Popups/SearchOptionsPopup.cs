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
	public class SearchOptionsPopup : AbstractStackedPopup
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
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = Res.Strings.SearchController.Options.Radios.ToString (),
			});

			this.SetDescriptions (list);
		}


		public SearchOptions					Options
		{
			get
			{
				var options = SearchOptions.Unknown;

				if (this.IgnoreCase     )  options |= SearchOptions.IgnoreCase;
				if (this.IgnoreDiacritic)  options |= SearchOptions.IgnoreDiacritic;
				if (this.Phonetic       )  options |= SearchOptions.Phonetic;
				if (this.WholeWords     )  options |= SearchOptions.WholeWords;

				options |= this.Radios;

				return options;
			}
			set
			{
				this.IgnoreCase      = (value & SearchOptions.IgnoreCase     ) != 0;
				this.IgnoreDiacritic = (value & SearchOptions.IgnoreDiacritic) != 0;
				this.Phonetic        = (value & SearchOptions.Phonetic       ) != 0;
				this.WholeWords      = (value & SearchOptions.WholeWords     ) != 0;

				this.Radios = value;
			}
		}

		private bool							IgnoreCase
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

		private bool							IgnoreDiacritic
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

		private bool							Phonetic
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

		private bool							WholeWords
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

		private SearchOptions					Radios
		{
			get
			{
				var controller = this.GetController (4) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);

				switch (controller.Value)
				{
					case 1:
						return SearchOptions.Prefix;
					case 2:
						return SearchOptions.Sufffix;
					case 3:
						return SearchOptions.FullText;
					case 4:
						return SearchOptions.Regex;
					default:
						return SearchOptions.Unknown;
				}
			}
			set
			{
				var controller = this.GetController (4) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);

				if ((value & SearchOptions.Prefix) != 0)
				{
					controller.Value = 1;
				}
				else if ((value & SearchOptions.Sufffix) != 0)
				{
					controller.Value = 2;
				}
				else if ((value & SearchOptions.FullText) != 0)
				{
					controller.Value = 3;
				}
				else if ((value & SearchOptions.Regex) != 0)
				{
					controller.Value = 4;
				}
				else
				{
					controller.Value = 0;
				}
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			bool enable = (this.Radios & SearchOptions.FullText) == 0 &&
						  (this.Radios & SearchOptions.Regex   ) == 0;

			this.SetEnable (3, enable);
		}
	}
}