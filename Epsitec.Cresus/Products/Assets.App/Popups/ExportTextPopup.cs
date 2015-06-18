//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir les paramètres pour l'exportation au format texte.
	/// </summary>
	public class ExportTextPopup : AbstractStackedPopup
	{
		private ExportTextPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.ExportText.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.Popup.ExportText.HasHeader.ToString (),
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.Popup.ExportText.Inverted.ToString (),
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.ExportText.ColumnSeparator.ToString (),
				Width                 = 240,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.ExportText.ColumnBracket.ToString (),
				Width                 = 240,
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.ExportText.Escape.ToString (),
				Width                 = 240,
			});

			list.Add (new StackedControllerDescription  // 5
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.Export.EndOfLines.ToString (),
				Width                 = 240,
			});

			list.Add (new StackedControllerDescription  // 6
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = Res.Strings.Popup.Export.Encoding.ToString (),
				MultiLabels           = EncodingHelpers.Labels,
				Width                 = 240,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Export.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		private TextExportProfile				Profile
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
					columnSeparator = controller.Value;
				}

				{
					var controller = this.GetController (3) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					columnBracket = controller.Value;
				}

				{
					var controller = this.GetController (4) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					escape = controller.Value;
				}

				{
					var controller = this.GetController (5) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					endOfLine = controller.Value;
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
					controller.Value = value.ColumnSeparator;
				}

				{
					var controller = this.GetController (3) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.ColumnBracket;
				}

				{
					var controller = this.GetController (4) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.Escape;
				}

				{
					var controller = this.GetController (5) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.EndOfLine;
				}

				{
					var controller = this.GetController (6) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = EncodingHelpers.EncodingToInt (value.Encoding);
				}
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, TextExportProfile profile, System.Action<TextExportProfile> action)
		{
			//	Affiche le Popup.
			var popup = new ExportTextPopup (accessor)
			{
				Profile = profile,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					action (popup.Profile);
				}
			};
		}
		#endregion
	}
}