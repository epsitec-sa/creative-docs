//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Expression;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public class EditorPageMethod : AbstractEditorPage
	{
		public EditorPageMethod(DataAccessor accessor, CommandContext commandContext, BaseType baseType, bool isTimeless)
			: base (accessor, commandContext, baseType, isTimeless)
		{
			this.commandDispatcher = new CommandDispatcher (this.GetType ().FullName, CommandDispatcherLevel.Primary, CommandDispatcherOptions.AutoForwardCommands);
			this.commandDispatcher.RegisterController (this);  // nécesaire pour [Command (Res.CommandIds...)]
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent, hasColorsExplanation: false);

			this.nameController       = this.CreateStringController (parent, ObjectField.Name);
			this.methodController     = this.CreateEnumController   (parent, ObjectField.AmortizationMethod, EnumDictionaries.DictAmortizationMethods);
			this.expressionController = this.CreateStringController (parent, ObjectField.Expression, lineCount: 25, maxLength: 10000);

			this.CreateOutputConsole (parent);

			CommandDispatcher.SetDispatcher (parent, this.commandDispatcher);  // nécesaire pour [Command (Res.CommandIds...)]

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


		[Command (Res.CommandIds.Methods.Library)]
		protected void OnLibrary(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = AbstractCommandToolbar.GetTarget (this.commandDispatcher, e);
			this.ShowLibraryPopup (target);
		}

		[Command (Res.CommandIds.Methods.Compile)]
		protected void OnCompile(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Compile ();
		}

		[Command (Res.CommandIds.Methods.Show)]
		protected void OnShow(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = AbstractCommandToolbar.GetTarget (this.commandDispatcher, e);
			this.Show (target);
		}

		[Command (Res.CommandIds.Methods.Test)]
		protected void OnTest(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = AbstractCommandToolbar.GetTarget (this.commandDispatcher, e);
			this.Test (target);
		}

		[Command (Res.CommandIds.Methods.Simulation)]
		protected void OnSimulation(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = AbstractCommandToolbar.GetTarget (this.commandDispatcher, e);
			this.Simulation (target);
		}

		private void ShowLibraryPopup(Widget target)
		{
			//	Affiche le popup pour choisir une expression à "importer".
			var popup = new SimplePopup ();

			foreach (var sample in EditorPageMethod.Samples)
			{
				popup.Items.Add (sample.Description);
			}

			popup.Create (target, leftOrRight: true);

			popup.ItemClicked += delegate (object sender, int rank)
			{
				var sample = EditorPageMethod.Samples.ToArray ()[rank];
				var expression = AmortizationExpression.GetDefaultExpression (sample.Type);
				this.SetExpression (expression);
			};
		}

		private void SetExpression(string expression)
		{
			//	Modifie l'expression actuellement sélectionnée.
			this.accessor.EditionAccessor.SetField(ObjectField.Expression, expression);
			this.expressionController.Value = expression;
		}

		private string Compile()
		{
			//	Compile l'expression actuellement sélectionnée.
			this.outputConsole.Text = "Compile started";
			this.outputConsole.Window.ForceLayout ();

			var startTime = System.DateTime.Now;
			string err;

			using (var e = new AmortizationExpression (this.expressionController.Value))
			{
				var elapsedTime = System.DateTime.Now - startTime;
				var message = string.Format ("Time elapsed {0}", elapsedTime.ToString ());

				if (string.IsNullOrEmpty (e.Error))
				{
					message += "<br/>";
					message += "Compile succeeded";  // anglais, ne pas traduire

					err = null;  // ok
				}
				else
				{
					message += "<br/>";
					message += e.Error;

					err = e.Error;
				}

				this.outputConsole.Text = message;  // affiche le nouveau message
			}

			return err;
		}

		private void Show(Widget target)
		{
			//	Affiche le code C# de l'expression actuellement sélectionnée.
			var expression = AmortizationExpression.GetDebugExpression (this.expressionController.Value);
			ShowExpressionPopup.Show (target, this.accessor, expression);
		}

		private void Test(Widget target)
		{
			//	Affiche le popup permettant de tester l'expression actuellement sélectionnée.
			var err = this.Compile ();

			if (string.IsNullOrEmpty (err))  // ok ?
			{
				var expression = new AmortizationExpression (this.expressionController.Value);
				TestExpressionPopup.Show (target, this.accessor, expression);
			}
			else  // erreur ?
			{
				MessagePopup.ShowError (target, err);
			}
		}

		private void Simulation(Widget target)
		{
			//	Affiche le popup permettant de choisir les paramètres pour lancer la
			//	simulation de l'expression actuellement sélectionnée.
			var err = this.Compile ();

			if (string.IsNullOrEmpty (err))  // ok ?
			{
				AmountExpressionSimulationPopup.Show (target, this.accessor, delegate
				{
					var expression = new AmortizationExpression (this.expressionController.Value);
					var amount = LocalSettings.ExpressionSimulationAmount;

					var nodes = EditorPageMethod.ComputeSimulation (expression, amount);

					ShowExpressionSimulationPopup.Show (target, this.accessor, nodes);
				});
			}
			else  // erreur ?
			{
				MessagePopup.ShowError (target, err);
			}
		}

		private static List<ExpressionSimulationNode> ComputeSimulation(AmortizationExpression expression, AmortizedAmount amount)
		{
			//	Lance la simulation d'une expression et retourne tous les noeuds correspondants.
			var nodes = new List<ExpressionSimulationNode> ();
			var date = new System.DateTime (2000-1, 12, 31);
			int totalMonth = 0;

			var baseAmount        = amount.BaseAmount;
			var monthCount        = amount.PeriodMonthCount;
			var periodicityFactor = amount.PeriodicityFactor;

			amount = AmortizedAmount.SetAmortizedAmount    (amount, baseAmount, baseAmount);
			amount = AmortizedAmount.SetRank               (amount, 0.0m);
			amount = AmortizedAmount.SetProrataNumerator   (amount, null);
			amount = AmortizedAmount.SetProrataDenominator (amount, null);

			for (int i=0; i<100; i++)  // nombre d'itérations arbitraire
			{
				totalMonth += monthCount;

				var initial = amount.InitialAmount.GetValueOrDefault ();
				var final = expression.Evaluate (amount).Value;

				var node = new ExpressionSimulationNode (i, date.AddMonths (totalMonth), initial, final);
				nodes.Add (node);

				amount = AmortizedAmount.SetAmortizedAmount (amount, final, baseAmount);
				amount = AmortizedAmount.SetRank (amount, (i+1)*periodicityFactor);
			}

			return nodes;
		}


		private void UpdateControllers()
		{
			bool expressionEnable = (this.CurrentMethod == AmortizationMethod.Custom);

			this.expressionController.SetFont (Font.GetFont ("Courier New", "Regular"));  // bof

			this.expressionController.IsReadOnly = !expressionEnable;
			this.outputConsole.Visibility =  expressionEnable;

			if (expressionEnable)
			{
				if (string.IsNullOrEmpty (this.expressionController.Value))
				{
					this.expressionController.Value = AmortizationExpression.GetDefaultExpression (SampleType.RateLinear);
				}
			}
			else
			{
				this.expressionController.Value = null;
			}

			this.outputConsole.Text = null;  // efface le message précédent

			this.UpdateCommands (expressionEnable);
		}

		private void UpdateCommands(bool expressionEnable)
		{
			this.SetEnable (Res.Commands.Methods.Library, expressionEnable);
			this.SetEnable (Res.Commands.Methods.Compile, expressionEnable);
			this.SetEnable (Res.Commands.Methods.Show, expressionEnable);
			this.SetEnable (Res.Commands.Methods.Test, expressionEnable);
			this.SetEnable (Res.Commands.Methods.Simulation, expressionEnable);
		}

		private void SetEnable(Command command, bool enable)
		{
			this.commandContext.GetCommandState (command).Enable = enable;
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


		#region Samples
		private static IEnumerable<Sample> Samples
		{
			get
			{
				yield return new Sample
				{
					Type        = SampleType.RateLinear,
					Description = Res.Strings.EditorPages.Method.RateLinear.Description.ToString (),
				};

				yield return new Sample
				{
					Type        = SampleType.RateDegressive,
					Description = Res.Strings.EditorPages.Method.RateDegressive.Description.ToString (),
				};

				yield return new Sample
				{
					Type        = SampleType.YearsLinear,
					Description = Res.Strings.EditorPages.Method.YearsLinear.Description.ToString (),
				};

				yield return new Sample
				{
					Type        = SampleType.YearsDegressive,
					Description = Res.Strings.EditorPages.Method.YearsDegressive.Description.ToString (),
				};
			}
		}

		private struct Sample
		{
			public SampleType					Type;
			public string						Description;
		}
		#endregion


		private readonly CommandDispatcher		commandDispatcher;

		private StringFieldController			nameController;
		private EnumFieldController				methodController;
		private StringFieldController			expressionController;
		private TextFieldMulti					outputConsole;
	}
}
