//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	public class EntityContext
	{
		private EntityContext()
		{
			this.resourceManager     = Resources.DefaultManager;
			this.resourceManagerPool = this.resourceManager.Pool;
			this.associatedThread    = System.Threading.Thread.CurrentThread;
			this.structuredTypeMap   = new Dictionary<Druid, IStructuredType> ();
		}
		
		
		public static EntityContext				Current
		{
			get
			{
				if (EntityContext.current == null)
				{
					EntityContext.current = new EntityContext ();
				}
				
				return EntityContext.current;
			}
		}

		public IValueStore CreateValueStore(AbstractEntity entity)
		{
			Druid typeId = entity.GetStructuredTypeId ();
			IStructuredType type = this.GetStructuredType (typeId);

			if (type == null)
			{
				throw new System.NotSupportedException (string.Format ("Type {0} is not supported", typeId));
			}
			
			return new Data (type);
		}


		public IStructuredType GetStructuredType(Druid id)
		{
			this.EnsureCorrectThread ();

			IStructuredType type;

			if (!this.structuredTypeMap.TryGetValue (id, out type))
			{
				Caption caption = this.resourceManager.GetCaption (id);
				type = TypeRosetta.GetTypeObject (caption) as IStructuredType;
				this.structuredTypeMap[id] = type;
			}
			
			return type;
		}

		public IStructuredType GetStructuredType(AbstractEntity entity)
		{
			this.EnsureCorrectThread ();

			IStructuredTypeProvider provider = entity.GetStructuredTypeProvider ();

			if (provider == null)
			{
				return this.GetStructuredType (entity.GetStructuredTypeId ());
			}
			else
			{
				return provider.GetStructuredType ();
			}
		}


		private void EnsureCorrectThread()
		{
			if (this.associatedThread == System.Threading.Thread.CurrentThread)
			{
				return;
			}

			throw new System.InvalidOperationException ("Invalid thread calling into EntityContext");
		}

		private class Data : IValueStore, IStructuredTypeProvider
		{
			public Data(IStructuredType type)
			{
				this.type  = type;
				this.store = new Dictionary<string, object> ();
			}

			#region IValueStore Members

			public object GetValue(string id)
			{
				object value;
				
				if (this.store.TryGetValue (id, out value))
				{
					return value;
				}
				else
				{
					return UndefinedValue.Instance;
				}
			}

			public void SetValue(string id, object value)
			{
				if (UndefinedValue.IsUndefinedValue (value))
				{
					this.store.Remove (id);
				}
				else
				{
					this.store[id] = value;
				}
			}

			#endregion

			#region IStructuredTypeProvider Members

			public IStructuredType GetStructuredType()
			{
				return this.type;
			}

			#endregion

			IStructuredType type;
			Dictionary<string, object> store;
		}

		[System.ThreadStatic]
		private static EntityContext current;

		private readonly ResourceManagerPool resourceManagerPool;
		private readonly ResourceManager resourceManager;
		private readonly System.Threading.Thread associatedThread;
		private readonly Dictionary<Druid, IStructuredType> structuredTypeMap;
	}
}
