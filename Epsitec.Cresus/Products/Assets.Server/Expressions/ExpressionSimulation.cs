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

			var methodGuid = accessor.CreateObject (BaseType.Methods, accessor.Mandat.StartDate, Guid.Empty, properties.ToArray ());
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

				var type = ArgumentsLogic.GetArgumentType (accessor, field);

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

			var assetGuid = accessor.CreateObject (BaseType.Assets, simulationParams.Range.IncludeFrom, Guid.Empty, properties.ToArray ());
			var asset = accessor.GetObject (BaseType.Assets, assetGuid);

			var ie = asset.GetInputEvent ();
			ie.AddProperty (mainProperty);

			//	Génère tous les amortissements.
			var a = new Amortizations (accessor);
			a.Preview (simulationParams.Range, assetGuid);

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
	}
}
