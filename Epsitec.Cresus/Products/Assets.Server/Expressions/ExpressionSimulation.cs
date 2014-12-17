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
			var properties = new List<AbstractDataProperty> ();

			//	Crée une méthode bidon, pour refléter les modifications effectuées dans EditorPageMethod,
			//	sans qu'il soit nécessaire de valider.
			properties.Add (new DataStringProperty (ObjectField.Expression, expression));

			foreach (var guid in argumentGuids)
			{
				var field = ArgumentsLogic.GetObjectField (accessor, guid);
				properties.Add (new DataGuidProperty (field, guid));
			}

			var methodGuid = accessor.CreateSimulationObject (BaseType.Methods,
				simulationParams.Range.IncludeFrom, Guid.Empty, properties.ToArray ());

			var method = accessor.GetObject (BaseType.Methods, methodGuid);

			//	Crée un objet bidon.
			properties.Clear ();

			var mainProperty = new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (simulationParams.InitialAmount));
			properties.Add (new DataGuidProperty (ObjectField.MethodGuid, methodGuid));
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

			//	Crée les éventuels événements exceptionnels.
			if (simulationParams.ExtraDate.HasValue && simulationParams.ExtraAmount.HasValue)
			{
				var timestamp = new Timestamp (simulationParams.ExtraDate.Value, 0);
				var e = new DataEvent(null, timestamp, EventType.AmortizationExtra);
				asset.AddEvent (e);

				var aa = new AmortizedAmount (simulationParams.ExtraAmount);
				var p = new DataAmortizedAmountProperty (ObjectField.MainValue, aa);
				e.AddProperty (p);
			}

			if (simulationParams.AdjustDate.HasValue && simulationParams.AdjustAmount.HasValue)
			{
				var timestamp = new Timestamp (simulationParams.AdjustDate.Value, 0);
				var e = new DataEvent (null, timestamp, EventType.Adjust);
				asset.AddEvent (e);

				var aa = new AmortizedAmount (simulationParams.AdjustAmount);
				var p = new DataAmortizedAmountProperty (ObjectField.MainValue, aa);
				e.AddProperty (p);
			}

			//	Génère tous les amortissements.
			var a = new Amortizations (accessor);
			var list = a.Preview (simulationParams.Range, assetGuid);

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

				int? rank = null;
				if (e.Type == EventType.AmortizationPreview ||
					e.Type == EventType.AmortizationExtra)
				{
					rank = i++;
				}

				var details = AmortizationDetails.Empty;
				if (e.Type == EventType.AmortizationPreview)
				{
					details = list[j++];
				}

				var node = new ExpressionSimulationNode (rank, e.Timestamp.Date, e.Type, initial, final, trace, details);
				nodes.Add (node);
			}

			//	Supprime les objets bidons.
			accessor.RemoveObject (BaseType.Methods, method);
			accessor.RemoveObject (BaseType.Assets, asset);

			return nodes;
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
	}
}
