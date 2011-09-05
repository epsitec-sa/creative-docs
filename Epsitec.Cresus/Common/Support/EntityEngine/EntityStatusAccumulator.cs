//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.Entities;
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

		
		public EntityStatus						EntityStatus
		{
			get
			{
				return this.currentStatus;
			}
		}

		
		#region IDisposable Members

		void IDisposable.Dispose()
		{
		}

		#endregion

		public void Accumulate(IEnumerable<EntityStatus> newStatus)
		{
			EntityStatus[] values;

			if (this.currentStatusDefined)
			{
				values = Epsitec.Common.Types.Collections.Enumerable.FromItem (this.currentStatus).Concat (newStatus).ToArray ();
			}
			else
			{
				values = newStatus.ToArray ();
			}

			if (values.Length == 0)
			{
				System.Diagnostics.Debug.Assert (this.currentStatusDefined == false);
			}
			else
			{
				this.currentStatus      = EntityStatusAccumulator.CombineStatus (this.atLeastOne, values);
				this.currentStatusDefined = true;
			}
		}

		public void Accumulate(EntityStatus newStatus)
		{
			if (this.currentStatusDefined)
			{
				this.currentStatus = EntityStatusAccumulator.CombineStatus (this.atLeastOne, this.currentStatus, newStatus);
			}
			else
			{
				this.currentStatus        = newStatus;
				this.currentStatusDefined = true;
			}
		}

		public void Accumulate(AbstractEntity entity, EntityStatusAccumulationMode mode = EntityStatusAccumulationMode.NoneIsEmpty)
		{
			var realEntity = entity.UnwrapNullEntity ();

			if (realEntity == null)
			{
				this.Accumulate (mode == EntityStatusAccumulationMode.NoneIsEmpty ? EntityStatus.Empty : EntityStatus.None);
			}
			else
			{
				this.Accumulate (realEntity.GetEntityStatus ());
			}
		}

		public void Accumulate(IEnumerable<AbstractEntity> entities, EntityStatusAccumulationMode mode = EntityStatusAccumulationMode.NoneIsEmpty)
		{
			var array = entities.ToArray ();

			if (array.Length == 0)
			{
				this.Accumulate (mode == EntityStatusAccumulationMode.NoneIsEmpty ?	EntityStatus.Empty : EntityStatus.None);
			}
			else
			{
				this.Accumulate (entities.Select (x => x.GetEntityStatus ()));
			}
		}



		private static EntityStatus CombineStatus(bool atLeastOne, params EntityStatus[] newStatus)
		{
			if (newStatus == null || newStatus.Length == 0)
			{
				return EntityStatus.Empty;
			}

			//	S'il existe un seul invalide, tout est considéré comme invalide.
			if (newStatus.Any (x => x == EntityStatus.None))
			{
				return EntityStatus.None;  // invalide
			}

			//	Si tout est vide, on dit que c'est vide.
			if (newStatus.All (x => x.HasFlag (EntityStatus.Empty)))
			{
				return EntityStatus.Empty;
			}

			if (atLeastOne)
			{
				//	Si un seul est valide, on dit que c'est valide.
				if (newStatus.Any (x => x.HasFlag (EntityStatus.Valid)))
				{
					return EntityStatus.Valid;
				}
			}
			else
			{
				//	Si tout est valide, on dit que c'est valide.
				if (newStatus.All (x => x.HasFlag (EntityStatus.Valid)))
				{
					return EntityStatus.Valid;
				}
			}

			return EntityStatus.None;  // invalide
		}


		private readonly bool					atLeastOne;

		private bool							currentStatusDefined;
		private EntityStatus					currentStatus;
	}
}