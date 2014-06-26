﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// Popup permettant de choisir les paramètres pour l'exportation au format xml.
	/// </summary>
	public class ExportXmlPopup : StackedPopup
	{
		public ExportXmlPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Exportation des données au format XML";

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
				Label                 = "Balise globale",
				Width                 = 200,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = "Balises pour les enregistrements",
				Width                 = 200,
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = "Indentation avec",
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


		public XmlExportProfile				Profile
		{
			get
			{
				string		bodyTag;
				string		recordTag;
				string		indent;
				string		endOfLine;
				bool		camelCase;
				bool		compact;
				Encoding	encoding;

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
					bodyTag = Converters.EditableToInternal (controller.Value);
				}

				{
					var controller = this.GetController (3) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					recordTag = Converters.EditableToInternal (controller.Value);
				}

				{
					var controller = this.GetController (4) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					indent = Converters.EditableToInternal (controller.Value);
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

				return new XmlExportProfile (bodyTag, recordTag, indent, endOfLine, camelCase, compact, encoding);
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
					controller.Value = Converters.InternalToEditable (value.BodyTag);
				}

				{
					var controller = this.GetController (3) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = Converters.InternalToEditable (value.RecordTag);
				}

				{
					var controller = this.GetController (4) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = Converters.InternalToEditable (value.Indent);
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


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.okButton.Text = "Exporter";
		}
	}
}