//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Donne un accès en lecture à une liste d'erreurs ou de messages,
	/// sans tri ni filtre.
	/// </summary>
	public class ErrorNodesGetter : AbstractNodesGetter<Error>  // outputNodes
	{
		public ErrorNodesGetter(List<Error> errors)
		{
			this.errors = errors;
		}


		public override int Count
		{
			get
			{
				return this.errors.Count;
			}
		}

		public override Error this[int index]
		{
			get
			{
				if (index >= 0 && index < this.errors.Count)
				{
					return this.errors[index];
				}
				else
				{
					return Error.Empty;
				}
			}
		}


		private readonly List<Error> errors;
	}
}
