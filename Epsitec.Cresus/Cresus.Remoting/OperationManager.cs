//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>OperationManager</c> class should only be used on the server side.
	/// It provides the bidirectionnal mapping between operations and their IDs.
	/// There is one <c>OperationManager</c> in every AppDomain.
	/// <remarks>
	/// Operations don't travel across the network as objects, but just as IDs,
	/// and on the server side, the <c>OperationManager</c> resolves them back
	/// to the original objects, if they are still alive.
	/// </remarks>
	/// </summary>
	public sealed class OperationManager
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OperationManager"/> class.
		/// </summary>
		private OperationManager()
		{
			this.operations = new Dictionary<long, AbstractOperation> ();
		}


		/// <summary>
		/// Resolves the specified operation id to an operation.
		/// </summary>
		/// <typeparam name="T">The expected operation type.</typeparam>
		/// <param name="operationId">The operation id.</param>
		/// <returns>The operation or <c>null</c>.</returns>
		public static T Resolve<T>(long operationId) where T : AbstractOperation
		{
			OperationManager manager = OperationManager.instance;
			AbstractOperation operation;

			lock (manager.exclusion)
			{
				manager.operations.TryGetValue (operationId, out operation);
			}

			return operation as T;
		}


		/// <summary>
		/// Registers the specified operation with the operation manager. This gets
		/// called by the <see cref="AbstractOperation"/> constructor.
		/// </summary>
		/// <param name="operation">The operation.</param>
		/// <param name="setOperationId">The callback used to set the caller's operation id.</param>
		internal static void Register(AbstractOperation operation, System.Action<long> setOperationId)
		{
			OperationManager.instance.RegisterOperation (operation, setOperationId);
		}

		/// <summary>
		/// Unregisters the specified operation from the operation manager. This gets
		/// called by <see cref="AbstractOperation.Dispose"/>.
		/// </summary>
		/// <param name="operation">The operation.</param>
		/// <param name="operationId">The operation id.</param>
		internal static void Unregister(AbstractOperation operation, long operationId)
		{
			OperationManager.instance.UnregisterOperation (operation, operationId);
		}

		
		private void RegisterOperation(AbstractOperation operation, System.Action<long> setOperationIdCallback)
		{
			long operationId;

			lock (this.exclusion)
			{
				operationId = ++this.nextOperationId;
				
				//	The operation id must be set on the caller before we store it into
				//	our dictionary; we could have used a setter property...
				
				setOperationIdCallback (operationId);
				
				this.operations[operationId] = operation;
			}
		}

		private void UnregisterOperation(AbstractOperation operation, long operationId)
		{
			lock (this.exclusion)
			{
				System.Diagnostics.Debug.Assert (this.operations.ContainsKey (operationId));
				System.Diagnostics.Debug.Assert (this.operations[operationId] == operation);
				this.operations.Remove (operationId);
			}
		}


		/// <summary>
		/// Sets the AppDomain specific id. This gets called by the <c>Engine</c> class in
		/// the <c>Epsitec.Cresus.Services</c> assembly.
		/// </summary>
		/// <param name="id">The AppDomain id.</param>
		public static void SetAppDomainId(int id)
		{
			System.Diagnostics.Debug.Assert (id > 0);
			System.Diagnostics.Debug.Assert (id < 250);

			OperationManager manager = OperationManager.instance;

			long highOrderMask = 0xff << 56;
			long highOrderBits = id << 56;

			lock (manager.exclusion)
			{
				if (manager.nextOperationId == 0)
				{
					manager.nextOperationId = highOrderBits;
				}
				else if ((manager.nextOperationId & ~highOrderMask) == 0)
				{
					//	OK, never mind if we already had another id
				}
				else
				{
					throw new System.InvalidOperationException ("Cannot set AppDomain ID; operation IDs already allocated");
				}
			}
		}
		
		private static readonly OperationManager instance = new OperationManager ();

		private readonly object exclusion = new object ();
		private readonly Dictionary<long, AbstractOperation> operations;
		private long nextOperationId;
	}
}
