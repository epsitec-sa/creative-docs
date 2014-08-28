﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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
	public class NewMandatPopup : StackedPopup
	{
		public NewMandatPopup(DataAccessor accessor)
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
				Width                 = DateController.controllerWidth - 4,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Free,
				Label                 = Res.Strings.Popup.NewMandat.Date.ToString (),
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Create.ToString ();
			this.defaultControllerRankFocus = 2;
		}


		public string							MandatFactoryName
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

		public string							MandatName
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

		public System.DateTime					MandatStartDate
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

		public bool								MandatWithSamples
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
	}
}