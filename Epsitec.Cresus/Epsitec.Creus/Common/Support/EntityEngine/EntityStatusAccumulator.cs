//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	public sealed class EntityStatusAccumulator : System.IDisposable
	{
		public EntityStatusAccumulator()
		{
		}

		
		public EntityStatus						EntityStatus
		{
			get
			{
				return this.currentStatus;
			}
		}



		public static EntityStatus Union(params EntityStatus[] collection)
		{
			EntityStatus union = EntityStatus.None;

			foreach (var item in collection)
			{
				union |= item;
			}

			return union;
		}

		public static EntityStatus Intersection(params EntityStatus[] collection)
		{
			if (collection.Length == 0)
			{
				return EntityStatus.None;
			}

			EntityStatus union = collection[0];

			foreach (var item in collection)
			{
				union &= item;
			}

			return union;
		}

		
		#region IDisposable Members

		void System.IDisposable.Dispose()
		{
		}

		#endregion

		public void Accumulate(IEnumerable<EntityStatus> collection, EntityStatusAccumulationMode mode = EntityStatusAccumulationMode.NoneIsValid)
		{
			EntityStatus[] values = collection.ToArray ();

			if (values.Length == 0)
			{
				this.AccumulateEmptyStatus (mode);
			}
			else
			{
				this.Accumulate (EntityStatusAccumulator.CombineStatus (values));
			}
		}

		public void Accumulate(EntityStatus newStatus)
		{
			if (this.currentStatusDefined)
			{
				this.currentStatus = EntityStatusAccumulator.CombineStatus (this.currentStatus, newStatus);
			}
			else
			{
				this.currentStatus        = newStatus;
				this.currentStatusDefined = true;
			}
		}

		public void Accumulate(AbstractEntity entity, EntityStatusAccumulationMode mode = EntityStatusAccumulationMode.NoneIsValid)
		{
			if (entity.UnwrapNullEntity () == null)
			{
				this.AccumulateEmptyStatus (mode);
			}
			else
			{
				this.Accumulate (entity.GetEntityStatus ());
			}
		}



		private void AccumulateEmptyStatus(EntityStatusAccumulationMode mode)
		{
			switch (mode)
			{
				case EntityStatusAccumulationMode.NoneIsInvalid:
					this.Accumulate (EntityStatus.Empty);
					break;

				case EntityStatusAccumulationMode.NoneIsValid:
					this.Accumulate (EntityStatus.Empty | EntityStatus.Valid);
					break;

				case EntityStatusAccumulationMode.NoneIsPartiallyCreated:
					this.Accumulate (EntityStatus.PartiallyCreated);
					break;

				default:
					throw new System.NotSupportedException ("Invalid mode specified");
			}
		}

		private static EntityStatus CombineStatus(params EntityStatus[] status)
		{
			if (status == null || status.Length == 0)
			{
				return EntityStatus.Empty;
			}

			var union        = EntityStatusAccumulator.Union (status);
			var intersection = EntityStatusAccumulator.Intersection (status);

			return
				(union & EntityStatus.PartiallyCreated) |
				(intersection & EntityStatus.Empty) |
				(intersection & EntityStatus.Valid);
		}

		private bool							currentStatusDefined;
		private EntityStatus					currentStatus;
	}
}