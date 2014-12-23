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
	public static class ExpressionSimulation
	{
		public static List<ExpressionSimulationNode> ComputeSimulation(DataAccessor accessor,
			string expression, IEnumerable<Guid> argumentGuids, ExpressionSimulationParams simulationParams)
		{
			//	Lance la simulation d'une expression et retourne tous les noeuds correspondants,
			//	qui pourront être donnés à ExpressionSimulationTreeTableFiller.
			var a = new Amortizations (accessor);

			var method = ExpressionSimulation.CreateMethod (accessor, expression, argumentGuids, simulationParams);
			var asset  = ExpressionSimulation.CreateAsset  (accessor, simulationParams, method);

			if (simulationParams.HasExtra)
			{
				//	On génère d'abord tous les amortissements pour permettre de récupérer la valeur
				//	finale au moment de l'amortissement extraordinaire.
				ExpressionSimulation.CreateAdjust (simulationParams, asset);
				a.Preview (simulationParams.Range, asset.Guid);

				var timestamp = new Timestamp (simulationParams.ExtraDate.Value, 0);
				var aa = ObjectProperties.GetObjectPropertyAmortizedAmount (asset, timestamp, ObjectField.MainValue);

				var finalAmount = 0.0m;
				if (aa.HasValue && aa.Value.FinalAmount.HasValue)
				{
					finalAmount = aa.Value.FinalAmount.Value;
				}

				//	Supprime les amortissements automatiques.
				a.Delete (simulationParams.Range.IncludeFrom, asset.Guid);

				ExpressionSimulation.CreateExtra (simulationParams, asset, finalAmount, simulationParams.AmortizationSuppl);
			}
			else
			{
				ExpressionSimulation.CreateAdjust (simulationParams, asset);
			}

			//	Génère tous les amortissements.
			var amortizationDetails = a.Preview (simulationParams.Range, asset.Guid);

			var nodes = ExpressionSimulation.GetNodes (asset, amortizationDetails);

			//	Supprime les objets bidons.
			accessor.RemoveObject (BaseType.Methods, method);
			accessor.RemoveObject (BaseType.Assets, asset);

			return nodes;
		}


		private static DataObject CreateMethod(DataAccessor accessor,
			string expression, IEnumerable<Guid> argumentGuids, ExpressionSimulationParams simulationParams)
		{
			//	Crée une méthode bidon, pour refléter les modifications effectuées dans EditorPageMethod,
			//	sans qu'il soit nécessaire de valider.
			var properties = new List<AbstractDataProperty> ();

			properties.Add (new DataStringProperty (ObjectField.Expression, expression));

			foreach (var guid in argumentGuids)
			{
				var field = ArgumentsLogic.GetObjectField (accessor, guid);
				properties.Add (new DataGuidProperty (field, guid));
			}

			var methodGuid = accessor.CreateSimulationObject (BaseType.Methods,
				simulationParams.Range.IncludeFrom, Guid.Empty, properties.ToArray ());

			return accessor.GetObject (BaseType.Methods, methodGuid);
		}

		private static DataObject CreateAsset(DataAccessor accessor,
			ExpressionSimulationParams simulationParams, DataObject method)
		{
			//	Crée un objet bidon.
			var properties = new List<AbstractDataProperty> ();

			var mainProperty = new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (simulationParams.InitialAmount));
			properties.Add (new DataGuidProperty (ObjectField.MethodGuid, method.Guid));
			properties.Add (new DataIntProperty (ObjectField.Periodicity, (int) simulationParams.Periodicity));

			foreach (var pair in simulationParams.Arguments)
			{
				if (pair.Value == null)
				{
					continue;
				}

				var field = pair.Key;
				var value = pair.Value;

				ExpressionSimulation.AddArgument (properties, accessor, field, value);
			}

			var assetGuid = accessor.CreateSimulationObject (BaseType.Assets,
				simulationParams.Range.IncludeFrom, Guid.Empty, properties.ToArray ());

			var asset = accessor.GetObject (BaseType.Assets, assetGuid);

			var ie = asset.GetInputEvent ();
			ie.AddProperty (mainProperty);

			return asset;
		}

		private static void AddArgument(List<AbstractDataProperty> properties, DataAccessor accessor, ObjectField field, object value)
		{
			var type = ArgumentsLogic.GetArgumentType (accessor, field);

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

		private static void CreateExtra(ExpressionSimulationParams simulationParams, DataObject asset, decimal finalAmount,
			bool amortizationSuppl)
		{
			//	Crée l'éventuel événement exceptionnel d'amortissement.
			if (simulationParams.HasExtra)
			{
				var timestamp = new Timestamp (simulationParams.ExtraDate.Value, 0);
				var e = new DataEvent (null, timestamp, amortizationSuppl ? EventType.AmortizationSuppl : EventType.AmortizationExtra);
				asset.AddEvent (e);

				var aa = new AmortizedAmount (finalAmount - simulationParams.ExtraAmount);
				var p = new DataAmortizedAmountProperty (ObjectField.MainValue, aa);
				e.AddProperty (p);
			}
		}

		private static void CreateAdjust(ExpressionSimulationParams simulationParams, DataObject asset)
		{
			//	Crée l'éventuel événement exceptionnel d'ajustement.
			if (simulationParams.HasAdjust)
			{
				var timestamp = new Timestamp (simulationParams.AdjustDate.Value, 0);
				var e = new DataEvent (null, timestamp, EventType.Adjust);
				asset.AddEvent (e);

				var aa = new AmortizedAmount (simulationParams.AdjustAmount);
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
	}
}
