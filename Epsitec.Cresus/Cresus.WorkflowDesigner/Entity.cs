//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner
{
	public static class Entity
	{
		static public List<AbstractEntity> DeepSearch(WorkflowDefinitionEntity def)
		{
			//	Effectue une fouille profonde à la recherche de toutes les entités nodes et edges.
			//	Il faut prendre garde, car la structure peut contenir des boucles !
			var list = new List<AbstractEntity> ();
			var alreadyFound = new HashSet<AbstractEntity> ();

			Entity.DeepSearch (list, alreadyFound, def);

			return list;
		}

		static private void DeepSearch(List<AbstractEntity> list, HashSet<AbstractEntity> alreadyFound, WorkflowDefinitionEntity defEntity)
		{
			if (alreadyFound.Contains (defEntity))
			{
				return;
			}

			Entity.DeepSearch (list, alreadyFound, defEntity as WorkflowNodeEntity);

			defEntity.WorkflowNodes.ForEach (node => Entity.DeepSearch (list, alreadyFound, node));
		}

		static private void DeepSearch(List<AbstractEntity> list, HashSet<AbstractEntity> alreadyFound, WorkflowNodeEntity nodeEntity)
		{
			if (alreadyFound.Contains (nodeEntity))
			{
				return;
			}

			list.Add (nodeEntity);
			alreadyFound.Add (nodeEntity);

			foreach (var edgeEntity in nodeEntity.Edges)
			{
				Entity.DeepSearch (list, alreadyFound, edgeEntity);
			}
		}

		static private void DeepSearch(List<AbstractEntity> list, HashSet<AbstractEntity> alreadyFound, WorkflowEdgeEntity edgeEntity)
		{
			if (alreadyFound.Contains (edgeEntity))
			{
				return;
			}

			list.Add (edgeEntity);
			alreadyFound.Add (edgeEntity);

			if (edgeEntity.NextNode.IsNotNull ())
			{
				Entity.DeepSearch (list, alreadyFound, edgeEntity.NextNode);
			}

			if (edgeEntity.Continuation.IsNotNull ())
			{
				Entity.DeepSearch (list, alreadyFound, edgeEntity.Continuation);
			}
		}
	}
}
