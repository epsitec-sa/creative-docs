//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir les paramètres pour l'exportation au format html.
	/// </summary>
	public class ExportHtmlPopup : StackedPopup
	{
		public ExportHtmlPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Exportation des données au format html";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = "Balises en mode CamelCase",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = "Fichier compact",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = "Balises pour les enregistrements",
				Width                 = 200,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = "Lignes terminées par",
				Width                 = 200,
			});

			this.SetDescriptions (list);
		}


		public HtmlExportProfile				Profile
		{
			get
			{
				string	recordTag;
				string	endOfLine;
				bool	camelCase;
				bool	compact;

				{
					var controller = this.GetController (0) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					camelCase = controller.Value;
				}

				{
					var controller = this.GetController (1) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					compact = controller.Value;
				}

				{
					var controller = this.GetController (2) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					recordTag = Converters.EditableToInternal (controller.Value);
				}

				{
					var controller = this.GetController (3) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					endOfLine = Converters.EditableToInternal (controller.Value);
				}

				return new HtmlExportProfile (recordTag, endOfLine, camelCase, compact);
			}
			set
			{
				{
					var controller = this.GetController (0) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.CamelCase;
				}

				{
					var controller = this.GetController (1) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.Compact;
				}

				{
					var controller = this.GetController (2) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = Converters.InternalToEditable (value.RecordTag);
				}

				{
					var controller = this.GetController (3) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = Converters.InternalToEditable (value.EndOfLine);
				}
			}
		}


		protected override void UpdateWidgets()
		{
			this.okButton.Text = "Exporter";
		}
	}
}