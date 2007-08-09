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
			
			return new StructuredData (type);
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


		private void EnsureCorrectThread()
		{
			if (this.associatedThread == System.Threading.Thread.CurrentThread)
			{
				return;
			}

			throw new System.InvalidOperationException ("Invalid thread calling into EntityContext");
		}

		[System.ThreadStatic]
		private static EntityContext current;

		private readonly ResourceManagerPool resourceManagerPool;
		private readonly ResourceManager resourceManager;
		private readonly System.Threading.Thread associatedThread;
		private readonly Dictionary<Druid, IStructuredType> structuredTypeMap;
	}
}
