//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Repositories;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core
{
	public sealed partial class CoreData
	{
		private IEnumerable<WorkflowDefinitionEntity> InsertWorkflowDefinitionsInDatabase()
		{
			var def = this.DataContext.CreateEntity<WorkflowDefinitionEntity> ();

			var nodeA = this.DataContext.CreateEntity<WorkflowNodeEntity> ();
			var nodeB = this.DataContext.CreateEntity<WorkflowNodeEntity> ();
			var nodeC = this.DataContext.CreateEntity<WorkflowNodeEntity> ();

			var edgeAB = this.DataContext.CreateEntity<WorkflowEdgeEntity> ();
			var edgeAC = this.DataContext.CreateEntity<WorkflowEdgeEntity> ();
			var edgeCA = this.DataContext.CreateEntity<WorkflowEdgeEntity> ();

			def.Code = "CUST-ORDER";
			def.EnableCondition = "MasterEntity=[L0AB2]";
			def.Name = FormattedText.FromSimpleText ("Commande client");
			def.Description = FormattedText.FromSimpleText ("Workflow pour le traitement d'une commande client (offre, bon pour commande, confirmation de commande, production, livraison)");
			def.StartingEdges.Add (edgeAB);

			nodeA.Code = "SALES-QUOTE(1)";
			nodeA.Name = FormattedText.FromSimpleText ("Préparation de l'offre");
			nodeA.Edges.Add (edgeAB);
			nodeA.Edges.Add (edgeAC);

			edgeAB.Name = FormattedText.FromSimpleText ("Créer une nouvelle offre");
			edgeAB.Description = FormattedText.FromSimpleText ("Crée une nouvelle offre liée à une nouvelle affaire pour ce client.");
			edgeAB.TransitionAction = "WorkflowAction.NewAffair";
			edgeAB.NextNode = nodeB;

			edgeAC.NextNode = nodeC;
			edgeCA.NextNode = nodeA;

			nodeB.Code = "SALES-QUOTE(2)";
			nodeB.Name = "Offre envoyée";

			nodeC.Code = "SALES-QUOTE(3)";
			nodeC.Name = "Variante de l'offre";
			nodeC.Edges.Add (edgeCA);

			yield return def;
		}
	}
}
