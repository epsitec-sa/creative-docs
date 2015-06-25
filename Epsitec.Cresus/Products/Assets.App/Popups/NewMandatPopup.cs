//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.Engine;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisie des informations nécessaires à la création d'un
	/// nouveau mandat.
	/// </summary>
	public class NewMandatPopup : AbstractStackedPopup
	{
		private NewMandatPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.NewMandat.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = NewMandatPopup.FactoryNames,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.Popup.NewMandat.Sample.ToString (),
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.NewMandat.Name.ToString (),
				Width                 = DateController.controllerWidth,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Free,
				Label                 = Res.Strings.Popup.NewMandat.Date.ToString (),
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Create.ToString ();
			this.defaultControllerRankFocus = 2;
		}


		private string							MandatFactoryName
		{
			get
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return NewMandatPopup.GetFactoryName (controller.Value);
			}
			set
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = NewMandatPopup.GetFactoryIndex (value);
			}
		}

		private string							MandatName
		{
			get
			{
				var controller = this.GetController (2) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (2) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private System.DateTime					MandatStartDate
		{
			get
			{
				var controller = this.GetController (3) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (3) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private bool							MandatWithSamples
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


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.okButton.Enable = !string.IsNullOrEmpty (this.MandatName)
								&& !this.HasError;
		}


		#region Factories interface
		private static string GetFactoryName(int? index)
		{
			if (index.HasValue)
			{
				return MandatFactory.Factories.ToArray ()[index.Value].Name;
			}
			else
			{
				return null;
			}
		}

		private static int? GetFactoryIndex(string name)
		{
			int index = MandatFactory.Factories.Select (x => x.Name).ToList ().IndexOf (name);

			if (index == -1)
			{
				return null;
			}
			else
			{
				return index;
			}
		}

		private static string FactoryNames
		{
			get
			{
				var list = new List<string> ();

				foreach (var factory in MandatFactory.Factories)
				{
					list.Add (factory.Name);
				}

				return string.Join ("<br/>", list);
			}
		}
		#endregion
		
		
		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, string factoryName, bool withSamples, System.DateTime date,
			System.Action<string, string, bool, System.DateTime> action)
		{
			//	Affiche le Popup.
			var popup = new NewMandatPopup (accessor)
			{
				MandatFactoryName = factoryName,
				MandatWithSamples = withSamples,
				MandatStartDate   = date,
			};

			popup.Create (target, leftOrRight: false);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					action (popup.MandatFactoryName, popup.MandatName, popup.MandatWithSamples, popup.MandatStartDate);
				}
			};
		}
		#endregion
	}
}