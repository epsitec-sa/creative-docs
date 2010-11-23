﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	/// <summary>
	/// The <c>UidGenerator</c> class allows the generation of unique ids. Each <c>UidGenerator</c>
	/// contains one or more slots which are each defined by a minimum and a maximum value. An
	/// <c>UidGenerator</c> can thus be used to generate unique ids in those slots.
	/// </summary>
	public sealed class UidGenerator
	{
		/// <summary>
		/// Creates a new instance of <c>UidGenerator</c>.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to communicate with the database.</param>
		/// <param name="name">The name of this instance.</param>
		/// <param name="slots">The sequence of slots of this instance.</param>
		private UidGenerator(DbInfrastructure dbInfrastructure, string name, IEnumerable<UidSlot> slots)
		{
			this.dbInfrastructure = dbInfrastructure;
			this.slots = slots.ToList ();
			this.Name = name;	
		}

		/// <summary>
		/// The name of this instance.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}

		/// <summary>
		/// The sequence of slots of this instance.
		/// </summary>
		public IEnumerable<UidSlot> Slots
		{
			get
			{
				foreach (UidSlot slot in this.slots)
				{
					yield return slot;
				}
			}
		}

		/// <summary>
		/// The low level <see cref="UidManager"/> used to manage the values of the counters.
		/// </summary>
		private DbUidManager UidManager
		{
			get
			{
				return this.dbInfrastructure.UidManager;
			}
		}

		/// <summary>
		/// Gets the next unique id generated by this instance.
		/// </summary>
		/// <returns>The next unique id.</returns>
		/// <exception cref="System.Exception">If a new unique id cannot be generated.</exception>
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

		/// <summary>
		/// Gets the next unique id generated by this instance in a given slot.
		/// </summary>
		/// <param name="slotIndex">The index of the slot to use.</param>
		/// <returns>The next unique id of the given slot.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="slotIndex"/> is lower than zero.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slotIndex"/> is greater or equal to the number of slots of this instance.</exception>
		/// <exception cref="System.Exception">If a new unique id cannot be generated in the given slot.</exception>
		public long GetNextUidInSlot(int slotIndex)
		{
			slotIndex.ThrowIf (s => s < 0, "slotIndex cannot be lower than zero.");
			slotIndex.ThrowIf (s => s >= this.slots.Count, "slotIndex cannot be greater or equal to the number of slots.");

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

		/// <summary>
		/// Gets the next unique id in any slot, starting from the slot given by
		/// <paramref name="startSlotIndex"/>
		/// </summary>
		/// <param name="startSlotIndex">The index of the first slot in which to try to generate the next unique id.</param>
		/// <returns>The next unique id.</returns>
		private long? InternalGetNextUidInSlots(int startSlotIndex)
		{
			long? uid = null;

			for (int slotIndex = startSlotIndex; slotIndex < this.slots.Count && !uid.HasValue; slotIndex++)
			{
				uid = this.InternalGetNextUidInSlot (slotIndex);
			}

			return uid;
		}

		/// <summary>
		/// Gets the next unique id in the given slot.
		/// </summary>
		/// <param name="slotIndex">The index of the slot in which to try to generate the next unique id.</param>
		/// <returns>The next unique id.</returns>
		private long? InternalGetNextUidInSlot(int slotIndex)
		{
			return this.UidManager.GetUidCounterNextValue (this.Name, slotIndex);
		}

		/// <summary>
		/// Creates a new generator for unique ids in the database.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to communicate with the database.</param>
		/// <param name="name">The name of the generator.</param>
		/// <param name="slots">The definition of the slots of the generator.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="slots"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slots"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slots"/> contains overlapping slots.</exception>
		internal static void CreateUidGenerator(DbInfrastructure dbInfrastructure, string name, IEnumerable<UidSlot> slots)
		{
			List<UidSlot> slotsAsList = slots.OrderBy (s => s.MinValue).ToList ();

			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			name.ThrowIfNullOrEmpty ("name");
			slots.ThrowIfNull ("slots");

			if (!slotsAsList.Any ())
			{
				throw new System.ArgumentException ("No slots defined.");
			}

			for (int i = 0; i < slotsAsList.Count - 1; i++)
			{
				if (slotsAsList[i].MaxValue >= slotsAsList[i + 1].MinValue)
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
					dbInfrastructure.UidManager.CreateUidCounter (name, i, slotsAsList[i].MinValue, slotsAsList[i].MaxValue);
				}

				transaction.Commit ();
			}
		}

		/// <summary>
		/// Deletes a generator for unique ids from the database.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to communicate with the database.</param>
		/// <param name="name">The name of the generator.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		internal static void DeleteUidGenerator(DbInfrastructure dbInfrastructure, string name)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = UidGenerator.CreateWriteTransaction (dbInfrastructure))
			{
				foreach (DbUidSlot slot in UidGenerator.GetSlots (dbInfrastructure, name))
				{
					dbInfrastructure.UidManager.DeleteUidCounter (slot.Name, slot.SlotNumber);
				}

				transaction.Commit ();
			}
		}

		/// <summary>
		/// Tells whether a generator for unique ids exists in the database.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to communicate with the database.</param>
		/// <param name="name">The name of the generator.</param>
		/// <returns><c>true</c> if a generator with <paramref name="name"/> exists in the database, <c>false</c> if there aren't.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		internal static bool UidGeneratorExists(DbInfrastructure dbInfrastructure, string name)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = UidGenerator.CreateReadTransaction (dbInfrastructure))
			{
				bool exists = dbInfrastructure.UidManager.ExistsUidCounter (name, 0);

				transaction.Commit ();

				return exists;
			}
		}

		/// <summary>
		/// Gets the <see cref="UidGenerator"/> object used to manipulate a generator of unique ids
		/// in the database.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to communicate with the database.</param>
		/// <param name="name">The name of the generator.</param>
		/// <returns>The <see cref="UidGenerator"/> object or <c>null</c> if it does not exist.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		internal static UidGenerator GetUidGenerator(DbInfrastructure dbInfrastructure, string name)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = UidGenerator.CreateReadTransaction (dbInfrastructure))
			{
				var slots = new List<UidSlot>
				(
					from slot in UidGenerator.GetSlots (dbInfrastructure, name)
					select new UidSlot (slot.MinValue, slot.MaxValue)
				);

				transaction.Commit ();

				if (!slots.Any ())
				{
					return null;
				}

				return new UidGenerator (dbInfrastructure, name, slots);
			}
		}

		/// <summary>
		/// Gets the indexes of the slots as stored in the database for a given uid counter.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to communicate with the database.</param>
		/// <param name="name">The name of the counter to use.</param>
		/// <returns>The sequence of slot indexes.</returns>
		private static IEnumerable<DbUidSlot> GetSlots(DbInfrastructure dbInfrastructure, string name)
		{
			return from slot in dbInfrastructure.UidManager.GetUidCounterSlots (name)
				   orderby slot.SlotNumber
				   select slot;
		}

		/// <summary>
		/// Creates the <see cref="DbTransaction"/> object that must be used when reading data from
		/// the database.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to communicate with the database.</param>
		/// <returns>The <see cref="DbTransaction"/> that must be used.</returns>
		private static DbTransaction CreateReadTransaction(DbInfrastructure dbInfrastructure)
		{
			return dbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly);
		}

		/// <summary>
		/// Creates the <see cref="DbTransaction"/> object that must be used when writing data to
		/// the database.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to communicate with the database.</param>
		/// <returns>The <see cref="DbTransaction"/> that must be used.</returns>
		private static DbTransaction CreateWriteTransaction(DbInfrastructure dbInfrastructure)
		{
			List<DbTable> tablesToLock = new List<DbTable> ()
			{
				dbInfrastructure.ResolveDbTable (Tags.TableUid),
			};

			return dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite, tablesToLock);
		}

		/// <summary>
		/// The <see cref="DbInfrastructure"/> used to communicate with the database.
		/// </summary>
		private readonly DbInfrastructure dbInfrastructure;

		/// <summary>
		/// The sequence of slots.
		/// </summary>
		private readonly List<UidSlot> slots;
	}
}
