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
using Epsitec.Cresus.Assets.Data.DataProperties;
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

			this.CreateStringController (parent, ObjectField.Name);

			this.CreateSepartor (parent);
			
			this.argumentsController =
				this.CreateArgumentsController (parent);
			
			this.CreateSepartor (parent);
			
			this.expressionController =
				this.CreateStringController (parent, ObjectField.Expression, lineCount: 20, maxLength: 10000);

			this.CreateOutputConsole (parent);

			CommandDispatcher.SetDispatcher (parent, this.commandDispatcher);  // nécesaire pour [Command (Res.CommandIds...)]
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

			foreach (var item in AmortizationExpressionCollection.Items)
			{
				popup.Items.Add (item.Description);
			}

			popup.Create (target, leftOrRight: true);

			popup.ItemClicked += delegate (object sender, int rank)
			{
				var expression = AmortizationExpressionCollection.Items
					.Select (x => x.Expression)
					.ElementAt (rank);

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

			var arguments = ArgumentsLogic.GetArgumentsDotNetCode (this.accessor, this.argumentsController.ArgumentGuids);

			using (var e = new AmortizationExpression (arguments, this.expressionController.Value))
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
			var arguments = ArgumentsLogic.GetArgumentsDotNetCode (this.accessor, this.argumentsController.ArgumentGuids);
			var expression = AmortizationExpression.GetDebugExpression (arguments, this.expressionController.Value);
			ShowExpressionPopup.Show (target, this.accessor, expression);
		}

		private void Simulation(Widget target)
		{
			//	Affiche le popup permettant de choisir les paramètres pour lancer la
			//	simulation de l'expression actuellement sélectionnée.
			ExpressionSimulationParamsPopup.Show (target, this.accessor, this.objectGuid, delegate
			{
				var nodes = this.ComputeSimulation (LocalSettings.ExpressionSimulationParams);
				ShowExpressionSimulationPopup.Show (target, this.accessor, nodes);
			});
		}

		private List<ExpressionSimulationNode> ComputeSimulation(ExpressionSimulationParams p)
		{
			//	Lance la simulation d'une expression et retourne tous les noeuds correspondants,
			//	qui pourront être donnés à ExpressionSimulationTreeTableFiller.
			var properties = new List<AbstractDataProperty> ();

			//	Crée une méthode bidon, pour refléter les modifications effectuées dans EditorPageMethod,
			//	sans qu'il soit nécessaire de valider.
			properties.Add (new DataStringProperty (ObjectField.Expression, this.expressionController.Value));

			foreach (var guid in this.argumentsController.ArgumentGuids)
			{
				var field = ArgumentsLogic.GetObjectField(this.accessor, guid);
				properties.Add (new DataGuidProperty (field, guid));
			}

			var methodGuid = accessor.CreateObject (BaseType.Methods, this.accessor.Mandat.StartDate, Guid.Empty, properties.ToArray ());
			var method = accessor.GetObject (BaseType.Methods, methodGuid);

			//	Crée un objet bidon.
			properties.Clear();
			var mainProperty = new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (p.InitialAmount));
			properties.Add (new DataGuidProperty (ObjectField.MethodGuid, methodGuid));
			properties.Add (new DataIntProperty (ObjectField.Periodicity, (int) p.Periodicity));

			foreach (var pair in p.Arguments)
			{
				if (pair.Value == null)
				{
					continue;
				}

				var field = pair.Key;
				var value = pair.Value;

				var type = ArgumentsLogic.GetArgumentType (this.accessor, field);

				switch (type)
				{
					case ArgumentType.Decimal:
					case ArgumentType.Amount:
					case ArgumentType.Rate:
						properties.Add (new DataDecimalProperty (field, (decimal) value));
						break;

					case ArgumentType.Int:
						properties.Add (new DataIntProperty (field, (int) value));
						break;

					case ArgumentType.Date:
						properties.Add (new DataDateProperty (field, (System.DateTime) value));
						break;

					case ArgumentType.String:
						properties.Add (new DataStringProperty (field, (string) value));
						break;

					default:
						throw new System.InvalidOperationException (string.Format ("Invalid ArgumentType {0}", type));
				}
			}

			var assetGuid = accessor.CreateObject (BaseType.Assets, p.Range.IncludeFrom, Guid.Empty, properties.ToArray ());
			var asset = accessor.GetObject(BaseType.Assets, assetGuid);

			var ie = asset.GetInputEvent ();
			ie.AddProperty (mainProperty);

			//	Génère tous les amortissements.
			var a = new Amortizations (accessor);
			a.Preview (p.Range, assetGuid);

			//	Récupère tous les événements d'amortissement dans les noeuds.
			var nodes = new List<ExpressionSimulationNode> ();

			int i = 0;
			foreach (var e in asset.Events.Where (x => x.Type == EventType.AmortizationPreview))
			{
				var property = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;

				var initial = property.Value.InitialAmount.GetValueOrDefault ();
				var final   = property.Value.FinalAmount.GetValueOrDefault ();
				var trace   = property.Value.Trace;

				var node = new ExpressionSimulationNode (i++, e.Timestamp.Date, initial, final, trace);
				nodes.Add (node);
			}

			//	Supprime les objets bidons.
			accessor.RemoveObject (BaseType.Methods, method);
			accessor.RemoveObject (BaseType.Assets, asset);

			return nodes;
		}


		private void UpdateControllers()
		{
			this.expressionController.SetFont (Font.GetFont ("Courier New", "Regular"));  // bof

			if (string.IsNullOrEmpty (this.expressionController.Value))
			{
				this.expressionController.Value = AmortizationExpressionCollection.GetExpression (AmortizationExpressionType.RateLinear);
			}

			this.outputConsole.Text = null;  // efface le message précédent

			this.UpdateCommands ();
		}

		private void UpdateCommands()
		{
			bool enable = (this.accessor.EditionAccessor.EditedObject != null);

			this.SetEnable (Res.Commands.Methods.Library,    enable);
			this.SetEnable (Res.Commands.Methods.Compile,    enable);
			this.SetEnable (Res.Commands.Methods.Show,       enable);
			this.SetEnable (Res.Commands.Methods.Simulation, enable);
		}

		private void SetEnable(Command command, bool enable)
		{
			this.commandContext.GetCommandState (command).Enable = enable;
		}


		private readonly CommandDispatcher		commandDispatcher;

		private ArgumentToUseFieldsController	argumentsController;
		private StringFieldController			expressionController;
		private TextFieldMulti					outputConsole;
	}
}
