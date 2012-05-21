//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	using UidSlot = DataLayer.Infrastructure.UidSlot;

	/// <summary>
	/// The <c>RefIdGenerator</c> gives access to numeric monotonic and unique ids.
	/// </summary>
	public class RefIdGenerator
	{
		internal RefIdGenerator(string name, RefIdGeneratorPool pool, long firstId = 1000L)
		{
			RefIdGenerator.AssertCaller<RefIdGeneratorPool> ();

			this.name = name;
			this.pool = pool;

			var infrastructure = this.pool.Host.DataInfrastructure;

			if (infrastructure.DoesUidGeneratorExists (this.name))
			{
				this.uidGenerator = infrastructure.GetUidGenerator (this.name);
			}
			else
			{
				List<UidSlot> slots = new List<UidSlot> ();
				slots.Add (new UidSlot (firstId, 999999999999L));
				this.uidGenerator = infrastructure.CreateUidGenerator (this.name, slots);
			}
		}


		public long GetNextId()
		{
			return this.uidGenerator.GetNextUid ();
		}


		[System.Diagnostics.Conditional ("DEBUG")]
		private static void AssertCaller<T>()
		{
			var trace = new System.Diagnostics.StackTrace (skipFrames: 2);
			var frame = trace.GetFrame (0);
			var method = frame.GetMethod ();

			if (method.DeclaringType != typeof (T))
			{
				throw new System.InvalidOperationException ("UidGenerator cannot be created directly; incorrect caller");
			}
		}

		private readonly string name;
		private readonly RefIdGeneratorPool pool;
		private readonly DataLayer.Infrastructure.UidGenerator uidGenerator;
	}
}