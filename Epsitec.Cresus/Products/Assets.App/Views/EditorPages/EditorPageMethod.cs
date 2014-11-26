//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Expression;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public class EditorPageMethod : AbstractEditorPage
	{
		public EditorPageMethod(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent, hasColorsExplanation: false);

			this.nameController       = this.CreateStringController (parent, ObjectField.Name);
			this.methodController     = this.CreateEnumController   (parent, ObjectField.AmortizationMethod, EnumDictionaries.DictAmortizationMethods);
			this.expressionController = this.CreateStringController (parent, ObjectField.Expression, lineCount: 25, maxLength: 10000);

			this.CreateButtons (parent);
			this.CreateOutputConsole (parent);

			this.methodController.ValueEdited += delegate
			{
				this.UpdateControllers ();
			};
		}

		public override void SetObject(Guid objectGuid, Timestamp timestamp)
		{
			base.SetObject (objectGuid, timestamp);
			this.UpdateControllers ();
		}

		private void CreateButtons(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 21,
				Dock            = DockStyle.Top,
				Margins         = new Margins (110, 0, 5, 0),
			};

			this.compileButton = new Button
			{
				Parent          = frame,
				PreferredWidth  = 90,
				Text            = "Compile",  // anglais, ne pas traduire
				ButtonStyle     = ButtonStyle.Icon,
				AutoFocus       = false,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 5, 0, 0),
			};

			this.debugButton = new Button
			{
				Parent          = frame,
				PreferredWidth  = 90,
				Text            = "Debug",  // anglais, ne pas traduire
				ButtonStyle     = ButtonStyle.Icon,
				AutoFocus       = false,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 5, 0, 0),
			};

			this.runButton = new Button
			{
				Parent          = frame,
				PreferredWidth  = 90,
				Text            = "Run",  // anglais, ne pas traduire
				ButtonStyle     = ButtonStyle.Icon,
				AutoFocus       = false,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 5, 0, 0),
			};

			this.compileButton.Clicked += delegate
			{
				this.Compile ();
			};

			this.debugButton.Clicked += delegate
			{
				this.Debug ();
			};

			this.runButton.Clicked += delegate
			{
				this.Run ();
			};
		}

		private void CreateOutputConsole(Widget parent)
		{
			this.outputConsole = new TextFieldMulti
			{
				Parent          = parent,
				IsReadOnly      = true,
				PreferredHeight = 100,
				Dock            = DockStyle.Top,
				Margins         = new Margins (110, 40, 5, 0),
			};
		}

		private void UpdateControllers()
		{
			bool expressionEnable = (this.CurrentMethod == AmortizationMethod.Custom);

			this.expressionController.IsReadOnly = !expressionEnable;
			this.compileButton.Visibility = expressionEnable;
			this.debugButton.Visibility = expressionEnable;
			this.runButton.Visibility = expressionEnable;
			this.outputConsole.Visibility = expressionEnable;

			if (expressionEnable)
			{
				if (string.IsNullOrEmpty (this.expressionController.Value))
				{
					this.expressionController.Value = AmortizationExpression.DefaultExpression;
				}
			}
			else
			{
				this.expressionController.Value = null;
			}

			this.outputConsole.Text = null;  // efface le message précédent
		}

		private void Compile()
		{
			this.outputConsole.Text = null;  // efface le message précédent
			this.outputConsole.Window.ForceLayout ();

			var startTime = System.DateTime.Now;

			using (var e = new AmortizationExpression (this.expressionController.Value))
			{
				var elapsedTime = System.DateTime.Now - startTime;
				var message = string.Format ("Time elapsed {0}", elapsedTime.ToString ());

				if (string.IsNullOrEmpty (e.Error))
				{
					message += "<br/>";
					message += "Compile succeeded";  // anglais, ne pas traduire
				}
				else
				{
					message += "<br/>";
					message += e.Error;
				}

				this.outputConsole.Text = message;  // affiche le nouveau message
			}
		}

		private void Debug()
		{
		}

		private void Run()
		{
		}

		private AmortizationMethod CurrentMethod
		{
			get
			{
				if (this.methodController.Value.HasValue)
				{
					return (AmortizationMethod) this.methodController.Value.Value;
				}
				else
				{
					return AmortizationMethod.Unknown;
				}
			}
		}


		private StringFieldController			nameController;
		private EnumFieldController				methodController;
		private StringFieldController			expressionController;
		private Button							compileButton;
		private Button							debugButton;
		private Button							runButton;
		private TextFieldMulti					outputConsole;
	}
}
