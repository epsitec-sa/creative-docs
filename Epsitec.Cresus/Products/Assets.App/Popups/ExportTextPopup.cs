//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir les paramètres pour l'exportation au format texte.
	/// </summary>
	public class ExportTextPopup : StackedPopup
	{
		public ExportTextPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Exportation des données au format texte";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = "Exporter les noms des colonnes",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = "Transposer les lignes et les colonnes",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = "Colonnes séparées par",
				Width                 = 200,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = "Colonnes entourées par",
				Width                 = 200,
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = "Caractère d'échappement",
				Width                 = 200,
			});

			list.Add (new StackedControllerDescription  // 5
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = "Lignes terminées par",
				Width                 = 200,
			});

			list.Add (new StackedControllerDescription  // 6
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = "Encodage",
				MultiLabels           = EncodingHelpers.Labels,
				Width                 = 200,
			});

			this.SetDescriptions (list);
		}


		public TextExportProfile				Profile
		{
			get
			{
				string		columnSeparator;
				string		columnBracket;
				string		escape;
				string		endOfLine;
				bool		hasHeader;
				bool		inverted;
				Encoding	encoding;

				{
					var controller = this.GetController (0) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					hasHeader = controller.Value;
				}

				{
					var controller = this.GetController (1) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					inverted = controller.Value;
				}

				{
					var controller = this.GetController (2) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					columnSeparator = Converters.EditableToInternal (controller.Value);
				}

				{
					var controller = this.GetController (3) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					columnBracket = Converters.EditableToInternal (controller.Value);
				}

				{
					var controller = this.GetController (4) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					escape = Converters.EditableToInternal (controller.Value);
				}

				{
					var controller = this.GetController (5) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					endOfLine = Converters.EditableToInternal (controller.Value);
				}

				{
					var controller = this.GetController (6) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					encoding = EncodingHelpers.IntToEncoding (controller.Value);
				}

				return new TextExportProfile (columnSeparator, columnBracket, escape, endOfLine, hasHeader, inverted, encoding);
			}
			set
			{
				{
					var controller = this.GetController (0) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.HasHeader;
				}

				{
					var controller = this.GetController (1) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.Inverted;
				}

				{
					var controller = this.GetController (2) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = Converters.InternalToEditable (value.ColumnSeparator);
				}

				{
					var controller = this.GetController (3) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = Converters.InternalToEditable (value.ColumnBracket);
				}

				{
					var controller = this.GetController (4) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = Converters.InternalToEditable (value.Escape);
				}

				{
					var controller = this.GetController (5) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = Converters.InternalToEditable (value.EndOfLine);
				}

				{
					var controller = this.GetController (6) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = EncodingHelpers.EncodingToInt (value.Encoding);
				}
			}
		}


		protected override void UpdateWidgets()
		{
			this.okButton.Text = "Exporter";
		}
	}
}