//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner
{
	public class Entity
	{
		static public List<AbstractEntity> DeepSearch(WorkflowDefinitionEntity def)
		{
			//	Effectue une fouille profonde à la recherche de toutes les entités nodes et edges.
			//	Il faut prendre garde, car la structure peut contenir des boucles !
			var list = new List<AbstractEntity> ();
			var alreadyFounded = new List<AbstractEntity> ();

			Entity.DeepSearch (list, alreadyFounded, def);

			return list;
		}

		static private void DeepSearch(List<AbstractEntity> list, List<AbstractEntity> alreadyFounded, WorkflowEdgeEntity edgeEntity)
		{
			if (alreadyFounded.Contains (edgeEntity))
			{
				return;
			}

			list.Add (edgeEntity);
			alreadyFounded.Add (edgeEntity);

			if (edgeEntity.NextNode.IsNotNull ())
			{
				Entity.DeepSearch (list, alreadyFounded, edgeEntity.NextNode);
			}

			if (edgeEntity.Continuation.IsNotNull ())
			{
				Entity.DeepSearch (list, alreadyFounded, edgeEntity.Continuation);
			}
		}

		static private void DeepSearch(List<AbstractEntity> list, List<AbstractEntity> alreadyFounded, WorkflowNodeEntity nodeEntity)
		{
			if (alreadyFounded.Contains (nodeEntity))
			{
				return;
			}

			list.Add (nodeEntity);
			alreadyFounded.Add (nodeEntity);

			foreach (var edgeEntity in nodeEntity.Edges)
			{
				Entity.DeepSearch (list, alreadyFounded, edgeEntity);
			}
		}
	}
}
