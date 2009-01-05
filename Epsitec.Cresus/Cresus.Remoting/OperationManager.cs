//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>OperationManager</c> class should only be used on the server side.
	/// It provides the bidirectionnal mapping between operations and their IDs.
	/// <remarks>
	/// Operations don't travel across the network as objects, but just as IDs,
	/// and on the server side, the <c>OperationManager</c> resolves them back
	/// to the original objects, if they are still alive.
	/// </remarks>
	/// </summary>
	public class OperationManager
	{
		public OperationManager()
		{
			this.operations = new Dictionary<long, AbstractOperation> ();
		}

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

		internal static void Register(AbstractOperation operation, System.Action<long> setOperationId)
		{
			OperationManager.instance.RegisterOperation (operation, setOperationId);
		}

		internal static void Unregister(AbstractOperation operation, long operationId)
		{
			OperationManager.instance.UnregisterOperation (operation, operationId);
		}

		private void RegisterOperation(AbstractOperation operation, System.Action<long> setOperationId)
		{
			long operationId;

			lock (this.exclusion)
			{
				operationId = ++this.nextOperationId;
				setOperationId (operationId);
				this.operations[operationId] = operation;
			}
		}


		private static readonly OperationManager instance = new OperationManager ();

		private readonly object exclusion = new object ();
		private readonly Dictionary<long, AbstractOperation> operations;
		private long nextOperationId;

		private void UnregisterOperation(AbstractOperation operation, long operationId)
		{
			lock (this.exclusion)
			{
				System.Diagnostics.Debug.Assert (this.operations.ContainsKey (operationId));
				System.Diagnostics.Debug.Assert (this.operations[operationId] == operation);
				this.operations.Remove (operationId);
			}
		}

		internal static bool WaitForProgress(long operationId, int percent, System.TimeSpan timeout)
		{
			if (operationId == 0)
			{
				return true;
			}

			OperationManager manager = OperationManager.instance;
			AbstractOperation operation;

			lock (manager.exclusion)
			{
				if (operationId >= manager.nextOperationId)
				{
					throw new System.ArgumentException ("Invalid operation ID");
				}

				manager.operations.TryGetValue (operationId, out operation);
			}

			if (operation == null)
			{
				//	Operation finished a long time ago... no need to wait.

				return true;
			}

			return operation.WaitForProgress (percent, timeout);
		}

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
	}
}
