//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à la création d'une
	/// nouvelle catégorie, à savoir le nom de la catégorie et son éventuel modèle.
	/// </summary>
	public class CreateCategoryPopup2 : StackedPopup
	{
		public CreateCategoryPopup2(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Création d'une nouvelle catégorie";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = "Nom",
				Width                 = 200 + (int) AbstractScroller.DefaultBreadth,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.CategoryGuid,
				Label                 = "Modèle",
				Width                 = 200 + (int) AbstractScroller.DefaultBreadth,
				Height                = 260,
			});

			this.SetDescriptions (list);
		}


		public string							ObjectName
		{
			get
			{
				var controller = this.GetController (0) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (0) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public Guid								ObjectModel
		{
			get
			{
				var controller = this.GetController (1) as CategoryGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (1) as CategoryGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}


		public override void CreateUI()
		{
			base.CreateUI ();

			var controller = this.GetController (0);
			controller.SetFocus ();
		}

		protected override void UpdateWidgets()
		{
			this.okButton.Text = "Créer";

			this.okButton.Enable = !string.IsNullOrEmpty (this.ObjectName) &&
								   !this.ObjectModel.IsEmpty;
		}
	}
}