//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public class ExpressionSimulation : System.IDisposable
	{
		public ExpressionSimulation(DataAccessor accessor)
		{
			this.accessor = accessor;
		}

		public void Dispose()
		{
		}


		public List<ExpressionSimulationNode> ComputeSimulation(string expression,
			IEnumerable<Guid> argumentGuids, ExpressionSimulationParams simulationParams)
		{
			//	Lance la simulation d'une expression et retourne tous les noeuds correspondants,
			//	qui pourront être donnés à ExpressionSimulationTreeTableFiller.
			this.expression       = expression;
			this.argumentGuids    = argumentGuids;
			this.simulationParams = simulationParams;

			var a = new Amortizations (this.accessor);

			var method = this.CreateMethod ();
			var asset  = this.CreateAsset  (method);

			if (this.simulationParams.HasExtra)
			{
				//	On génère d'abord tous les amortissements pour permettre de récupérer la valeur
				//	finale au moment de l'amortissement extraordinaire.
				this.CreateAdjust (asset);
				a.Preview (this.simulationParams.Range, asset.Guid);

				var timestamp = new Timestamp (this.simulationParams.ExtraDate.Value, 0);
				var aa = ObjectProperties.GetObjectPropertyAmortizedAmount (asset, timestamp, ObjectField.MainValue);

				var finalAmount = 0.0m;
				if (aa.HasValue && aa.Value.FinalAmount.HasValue)
				{
					finalAmount = aa.Value.FinalAmount.Value;
				}

				//	Supprime les amortissements automatiques.
				a.Delete (this.simulationParams.Range.IncludeFrom, asset.Guid);

				this.CreateExtra (asset, finalAmount, this.simulationParams.AmortizationSuppl);
			}
			else
			{
				this.CreateAdjust (asset);
			}

			//	Génère tous les amortissements.
			var amortizationDetails = a.Preview (this.simulationParams.Range, asset.Guid);

			var nodes = ExpressionSimulation.GetNodes (asset, amortizationDetails);

			//	Supprime les objets bidons.
			this.accessor.RemoveObject (BaseType.Methods, method);
			this.accessor.RemoveObject (BaseType.Assets, asset);

			return nodes;
		}


		private DataObject CreateMethod()
		{
			//	Crée une méthode bidon, pour refléter les modifications effectuées dans EditorPageMethod,
			//	sans qu'il soit nécessaire de valider.
			var properties = new List<AbstractDataProperty> ();

			properties.Add (new DataStringProperty (ObjectField.Expression, this.expression));

			foreach (var guid in this.argumentGuids)
			{
				var field = ArgumentsLogic.GetObjectField (this.accessor, guid);
				properties.Add (new DataGuidProperty (field, guid));
			}

			var methodGuid = this.accessor.CreateSimulationObject (BaseType.Methods,
				this.simulationParams.Range.IncludeFrom, Guid.Empty, properties.ToArray ());

			return this.accessor.GetObject (BaseType.Methods, methodGuid);
		}

		private DataObject CreateAsset(DataObject method)
		{
			//	Crée un objet bidon.
			var properties = new List<AbstractDataProperty> ();

			var mainProperty = new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (this.simulationParams.InitialAmount));
			properties.Add (new DataGuidProperty (ObjectField.MethodGuid, method.Guid));
			properties.Add (new DataIntProperty (ObjectField.Periodicity, (int) this.simulationParams.Periodicity));

			foreach (var pair in this.simulationParams.Arguments)
			{
				if (pair.Value == null)
				{
					continue;
				}

				var field = pair.Key;
				var value = pair.Value;

				this.AddArgument (properties, field, value);
			}

			var assetGuid = this.accessor.CreateSimulationObject (BaseType.Assets,
				this.simulationParams.Range.IncludeFrom, Guid.Empty, properties.ToArray ());

			var asset = this.accessor.GetObject (BaseType.Assets, assetGuid);

			var ie = asset.GetInputEvent ();
			ie.AddProperty (mainProperty);

			return asset;
		}

		private void AddArgument(List<AbstractDataProperty> properties, ObjectField field, object value)
		{
			var type = ArgumentsLogic.GetArgumentType (this.accessor, field);

			switch (type)
			{
				case ArgumentType.Decimal:
				case ArgumentType.Amount:
				case ArgumentType.Rate:
				case ArgumentType.Years:
					properties.Add (new DataDecimalProperty (field, (decimal) value));
					break;

				case ArgumentType.Int:
					properties.Add (new DataIntProperty (field, (int) value));
					break;

				case ArgumentType.Bool:
					properties.Add (new DataIntProperty (field, (bool) value ? 1:0));
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

		private void CreateExtra(DataObject asset, decimal finalAmount, bool amortizationSuppl)
		{
			//	Crée l'éventuel événement exceptionnel d'amortissement.
			if (this.simulationParams.HasExtra)
			{
				var timestamp = new Timestamp (this.simulationParams.ExtraDate.Value, 0);
				var e = new DataEvent (null, timestamp, amortizationSuppl ? EventType.AmortizationSuppl : EventType.AmortizationExtra);
				asset.AddEvent (e);

				var aa = new AmortizedAmount (finalAmount - this.simulationParams.ExtraAmount);
				var p = new DataAmortizedAmountProperty (ObjectField.MainValue, aa);
				e.AddProperty (p);
			}
		}

		private void CreateAdjust(DataObject asset)
		{
			//	Crée l'éventuel événement exceptionnel d'ajustement.
			if (this.simulationParams.HasAdjust)
			{
				var timestamp = new Timestamp (this.simulationParams.AdjustDate.Value, 0);
				var e = new DataEvent (null, timestamp, EventType.Adjust);
				asset.AddEvent (e);

				var aa = new AmortizedAmount (this.simulationParams.AdjustAmount);
				var p = new DataAmortizedAmountProperty (ObjectField.MainValue, aa);
				e.AddProperty (p);
			}
		}

		private static List<ExpressionSimulationNode> GetNodes(DataObject asset, List<AmortizationDetails> amortizationDetails)
		{
			//	Récupère tous les événements d'amortissement dans les noeuds.
			var nodes = new List<ExpressionSimulationNode> ();

			int i = 0;
			int j = 0;
			foreach (var e in asset.Events)
			{
				var property = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;

				var initial = property.Value.InitialAmount;
				var final   = property.Value.FinalAmount;
				var trace   = property.Value.Trace;
				var error   = property.Value.Error;

				int? rank = null;
				if (e.Type == EventType.AmortizationPreview ||
					e.Type == EventType.AmortizationExtra)
				{
					rank = i++;
				}

				var details = AmortizationDetails.Empty;
				if (e.Type == EventType.AmortizationPreview)
				{
					details = amortizationDetails[j++];
				}

				var mode = AssetsLogic.IsAmortizationEnded (asset, e);
				var node = new ExpressionSimulationNode (rank, e.Timestamp.Date, e.Type, mode, initial, final, trace, error, details);
				nodes.Add (node);
			}

			return nodes;
		}


		private readonly DataAccessor			accessor;

		private string							expression;
		private IEnumerable<Guid>				argumentGuids;
		private ExpressionSimulationParams		simulationParams;
	}
}
