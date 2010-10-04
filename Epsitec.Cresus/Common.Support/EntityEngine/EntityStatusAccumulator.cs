//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Epsitec.Common.Support.EntityEngine
{
    public class EntityStatusAccumulator : IDisposable
	{
		public EntityStatusAccumulator(bool atLeastOne = false)
		{
			this.atLeastOne = atLeastOne;
		}

		void IDisposable.Dispose()
		{
		}


		public void Accumulate(IEnumerable<EntityStatus> newStatus)
		{
			foreach (var s in newStatus)
			{
				this.Accumulate (s);
			}
		}

		public void Accumulate(EntityStatus newStatus)
		{
			if (!this.currentStatusValid)
			{
				this.currentStatus = newStatus;
				this.currentStatusValid = true;
			}
			else
			{
				this.currentStatus = this.CombineStatus (this.currentStatus, newStatus);
			}
		}


		public EntityStatus EntityStatus
		{
			get
			{
				return this.currentStatus;
			}
		}


		private EntityStatus CombineStatus(params EntityStatus[] newStatus)
		{
			if (newStatus == null || newStatus.Count () == 0)
			{
				return EntityStatus.Empty;
			}

			//	S'il existe un seul invalide, tout est considéré comme invalide.
			if (newStatus.Any (x => (x & EntityStatus.Empty) == 0 && (x & EntityStatus.Valid) == 0))
			{
				return EntityStatus.None;  // invalide
			}

			//	Si tout est vide, on dit que c'est vide.
			if (newStatus.All (x => (x & EntityStatus.Empty) != 0))
			{
				return EntityStatus.Empty;
			}

			if (this.atLeastOne)
			{
				//	Si un seul est valide, on dit que c'est valide.
				if (newStatus.Any (x => (x & EntityStatus.Valid) != 0))
				{
					return EntityStatus.Valid;
				}
			}
			else
			{
				//	Si tout est valide, on dit que c'est valide.
				if (newStatus.All (x => (x & EntityStatus.Valid) != 0))
				{
					return EntityStatus.Valid;
				}
			}

			return EntityStatus.None;  // invalide
		}


		private readonly bool atLeastOne;

		private bool currentStatusValid;
		private EntityStatus currentStatus;
	}
}
