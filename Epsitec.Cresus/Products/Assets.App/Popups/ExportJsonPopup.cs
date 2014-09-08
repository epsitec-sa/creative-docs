﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir les paramètres pour l'exportation au format json.
	/// </summary>
	public class ExportJsonPopup : StackedPopup
	{
		public ExportJsonPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.ExportJson.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.Popup.Export.CamelCase.ToString (),
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.Export.EndOfLines.ToString (),
				Width                 = 200,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = Res.Strings.Popup.Export.Encoding.ToString (),
				MultiLabels           = EncodingHelpers.Labels,
				Width                 = 200,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Export.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		public JsonExportProfile				Profile
		{
			get
			{
				string		endOfLine;
				bool		camelCase;
				Encoding	encoding;

				{
					var controller = this.GetController (0) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					camelCase = controller.Value;
				}

				{
					var controller = this.GetController (1) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					endOfLine = controller.Value;
				}

				{
					var controller = this.GetController (2) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					encoding = EncodingHelpers.IntToEncoding (controller.Value);
				}

				return new JsonExportProfile (endOfLine, camelCase, encoding);
			}
			set
			{
				{
					var controller = this.GetController (0) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.CamelCase;
				}

				{
					var controller = this.GetController (1) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.EndOfLine;
				}

				{
					var controller = this.GetController (2) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = EncodingHelpers.EncodingToInt (value.Encoding);
				}
			}
		}
	}
}