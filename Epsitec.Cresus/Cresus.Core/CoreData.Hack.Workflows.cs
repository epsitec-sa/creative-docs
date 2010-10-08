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
		private WorkflowEntity CreateRelationWorkflow()
		{
			var workflow = this.DataContext.CreateEntity<WorkflowEntity> ();
			var thread   = this.DataContext.CreateEntity<WorkflowThreadEntity> ();

			workflow.Threads.Add (thread);

			thread.Status = WorkflowStatus.Pending;
			thread.Definition = this.DataContext.GetEntitiesOfType<WorkflowDefinitionEntity> (x => x.EnableCondition == "MasterEntity=[L0AB2]").First ();
			
			return workflow;
		}

		private IEnumerable<WorkflowDefinitionEntity> InsertWorkflowDefinitionsInDatabase()
		{
			var def = this.DataContext.CreateEntity<WorkflowDefinitionEntity> ();

			var nodeA = this.DataContext.CreateEntity<WorkflowNodeEntity> ();
			var nodeB = this.DataContext.CreateEntity<WorkflowNodeEntity> ();
			var nodeC = this.DataContext.CreateEntity<WorkflowNodeEntity> ();

			var edgeAB = this.DataContext.CreateEntity<WorkflowEdgeEntity> ();
			var edgeAC = this.DataContext.CreateEntity<WorkflowEdgeEntity> ();
			var edgeCA = this.DataContext.CreateEntity<WorkflowEdgeEntity> ();

			def.EnableCondition = "MasterEntity=[L0AB2]";
			def.Name = FormattedText.FromSimpleText ("Commande client");
			def.Description = FormattedText.FromSimpleText ("Workflow pour le traitement d'une commande client (offre, bon pour commande, confirmation de commande, production, livraison)");
			def.Edges.Add (edgeAB);
			def.Edges.Add (edgeAC);

			nodeA.Name = FormattedText.FromSimpleText ("Préparation de l'offre");
			nodeA.Edges.Add (edgeAB);
			nodeA.Edges.Add (edgeAC);

			edgeAB.Name = FormattedText.FromSimpleText ("Créer une nouvelle offre");
			edgeAB.Description = FormattedText.FromSimpleText ("Crée une nouvelle offre liée à une nouvelle affaire pour ce client.");
			edgeAB.TransitionAction = "WorkflowAction.NewAffair WorkflowAction.NewOffer";
			edgeAB.NextNode = nodeB;

			edgeAC.Name = FormattedText.FromSimpleText ("Créer une variante");
			edgeAC.Description = FormattedText.FromSimpleText ("Crée une variante d'une offre existante");
			edgeAC.TransitionAction = "WorkflowAction.NewOfferVariant";
			edgeAC.NextNode = nodeC;

			edgeCA.Name = FormattedText.FromSimpleText ("Editer la variante");
			edgeCA.Description = FormattedText.FromSimpleText ("...");
			edgeCA.TransitionAction = "...";
			edgeCA.NextNode = nodeA;

			nodeB.Name = "Offre envoyée";

			nodeC.Name = "Variante de l'offre";
			nodeC.Edges.Add (edgeCA);

			yield return def;
		}
	}
}
