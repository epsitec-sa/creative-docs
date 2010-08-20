using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	// TODO Comment this class.
	// Marc
	

	public sealed class UidGenerator
	{


		private UidGenerator(DbInfrastructure dbInfrastructure, string name, IEnumerable<System.Tuple<long, long>> slots)
		{
			this.dbInfrastructure = dbInfrastructure;

			this.slotMinValues = new List<long> ();
			this.slotMaxValues = new List<long> ();

			foreach (var slot in slots)
			{
				this.slotMinValues.Add (slot.Item1);
				this.slotMaxValues.Add (slot.Item2);
			}

			this.Name = name;	
		}


		public string Name
		{
			get;
			private set;
		}


		public IEnumerable<System.Tuple<long, long>> Slots
		{
			get
			{
				for (int slot = 0; slot < this.slotMinValues.Count; slot++)
				{
					long min = this.slotMinValues[slot];
					long max = this.slotMaxValues[slot];

					yield return System.Tuple.Create (min, max);
				}
			}
		}


		private DbUidManager UidManager
		{
			get
			{
				return this.dbInfrastructure.UidManager;
			}
		}


		public long GetNextUid()
		{
			using (DbTransaction transaction = UidGenerator.CreateWriteTransaction (dbInfrastructure))
			{
				long? uid = this.InternalGetNextUidInSlots (0);

				transaction.Commit ();

				if (!uid.HasValue)
				{
					throw new System.Exception ("Could not create new unique id.");
				}

				return uid.Value;
			}
		}


		public long GetNextUidInSlot(int slotIndex)
		{
			slotIndex.ThrowIf (s => s < 0, "slotIndex cannot be lower than zero.");
			slotIndex.ThrowIf (s => s >= this.slotMinValues.Count, "slotIndex cannot be greater or equal to the number of slots.");

			using (DbTransaction transaction = UidGenerator.CreateWriteTransaction (dbInfrastructure))
			{
				long? uid = this.InternalGetNextUidInSlot (slotIndex);

				transaction.Commit ();

				if (!uid.HasValue)
				{
					throw new System.Exception ("Could not create new unique id.");
				}

				return uid.Value;
			}
		}


		private long? InternalGetNextUidInSlots(int startSlotIndex)
		{
			long? uid = null;

			for (int slotIndex = startSlotIndex; slotIndex < this.slotMinValues.Count && !uid.HasValue; slotIndex++)
			{
				uid = this.InternalGetNextUidInSlot (slotIndex);
			}

			return uid;
		}


		private long? InternalGetNextUidInSlot(int slotIndex)
		{
			long max = this.slotMaxValues[slotIndex];
			long next = this.UidManager.GetUidCounterNext (this.Name, slotIndex);

			bool isNextValidUid = (next <= max);

			if (isNextValidUid)
			{
				this.UidManager.SetUidCounterNext (this.Name, slotIndex, next + 1);
			}

			return isNextValidUid ? (long?) next : null;
		}


		internal static void CreateUidGenerator(DbInfrastructure dbInfrastructure, string name, IEnumerable<System.Tuple<long, long>> slots)
		{
			List<System.Tuple<long, long>> slotsAsList = slots.OrderBy (s => s.Item1).ToList ();

			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			name.ThrowIfNullOrEmpty ("name");
			slots.ThrowIfNull ("slots");

			if (!slotsAsList.Any ())
			{
				throw new System.ArgumentException ("No slots defined.");
			}

			if (slotsAsList.Any (s => s.Item1 < 0 || s.Item2 < 0))
			{
				throw new System.ArgumentException ("Min and max values cannot be lower than zero.");
			}

			if (slotsAsList.Any (s => s.Item1 > s.Item2))
			{
				throw new System.ArgumentException ("A min value cannot be greater than a max value.");
			}

			for (int i = 0; i < slotsAsList.Count - 1; i++)
			{
				if (slotsAsList[i].Item2 >= slotsAsList[i + 1].Item1)
				{
					throw new System.ArgumentException ("Slots cannot overlap each others.");
				}
			}

			using (DbTransaction transaction = UidGenerator.CreateWriteTransaction (dbInfrastructure))
			{
				if (dbInfrastructure.UidManager.ExistsUidCounter (name, 0))
				{
					throw new System.InvalidOperationException ("UidGenerator " + name + " already exists.");
				}

				for (int i = 0; i < slotsAsList.Count; i++)
				{
					dbInfrastructure.UidManager.CreateUidCounter (name, i, slotsAsList[i].Item1, slotsAsList[i].Item2);
				}

				transaction.Commit ();
			}
		}


		internal static void DeleteUidGenerator(DbInfrastructure dbInfrastructure, string name)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = UidGenerator.CreateWriteTransaction (dbInfrastructure))
			{
				foreach (int slot in UidGenerator.GetSlots (dbInfrastructure, name))
				{
					dbInfrastructure.UidManager.DeleteUidCounter (name, slot);
				}

				transaction.Commit ();
			}
		}


		internal static bool UidGeneratorExists(DbInfrastructure dbInfrastructure, string name)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = UidGenerator.CreateReadTransaction (dbInfrastructure))
			{
				return dbInfrastructure.UidManager.ExistsUidCounter (name, 0);

				transaction.Commit ();
			}
		}


		internal static UidGenerator GetUidGenerator(DbInfrastructure dbInfrastructure, string name)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = UidGenerator.CreateReadTransaction (dbInfrastructure))
			{
				var slots = new List<System.Tuple<long, long>>
				(
					from slot in UidGenerator.GetSlots (dbInfrastructure, name)
					let minValue = dbInfrastructure.UidManager.GetUidCounterMin (name, slot)
					let maxValue = dbInfrastructure.UidManager.GetUidCounterMax (name, slot)
					select System.Tuple.Create (minValue, maxValue)
				);

				transaction.Commit ();

				if (!slots.Any ())
				{
					throw new System.ArgumentException ("Uid generator does not exists.");
				}

				return new UidGenerator (dbInfrastructure, name, slots);
			}
		}


		private static IEnumerable<int> GetSlots(DbInfrastructure dbInfrastructure, string name)
		{
			return from data in dbInfrastructure.UidManager.GetUidCounterNamesAndSlots ()
				   where data.Item1 == name
				   orderby data.Item2
				   select data.Item2;
		}


		private static DbTransaction CreateReadTransaction(DbInfrastructure dbInfrastructure)
		{
			return dbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly);
		}


		private static DbTransaction CreateWriteTransaction(DbInfrastructure dbInfrastructure)
		{
			List<DbTable> tablesToLock = new List<DbTable> ()
			{
				dbInfrastructure.ResolveDbTable (Tags.TableUid),
			};

			return dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite, tablesToLock);
		}


		private DbInfrastructure dbInfrastructure;


        private List<long> slotMinValues;


		private List<long> slotMaxValues;
		

	}


}
