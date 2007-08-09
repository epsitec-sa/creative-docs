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
			this.systemTypeMap       = new Dictionary<Druid, System.Type> ();
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
			IStructuredType type = this.GetStructuredType (typeId, delegate { this.systemTypeMap[typeId] = entity.GetType (); });

			if (type == null)
			{
				throw new System.NotSupportedException (string.Format ("Type {0} is not supported", typeId));
			}
			
			return new StructuredData (type);
		}


		public IStructuredType GetStructuredType(Druid id)
		{
			return this.GetStructuredType (id, delegate { });
		}

		private IStructuredType GetStructuredType(Druid id, System.Action<IStructuredType> setupAction)
		{
			this.EnsureCorrectThread ();

			IStructuredType type;

			if (!this.structuredTypeMap.TryGetValue (id, out type))
			{
				Caption caption = this.resourceManager.GetCaption (id);
				type = TypeRosetta.GetTypeObject (caption) as IStructuredType;
				
				this.structuredTypeMap[id] = type;
				setupAction (type);
			}
			
			return type;
		}

		public System.Type FindEntityType(Druid id)
		{
			System.Type value;
			this.systemTypeMap.TryGetValue (id, out value);
			return value;
		}


		private void EnsureCorrectThread()
		{
			if (this.associatedThread == System.Threading.Thread.CurrentThread)
			{
				return;
			}

			throw new System.InvalidOperationException ("Invalid thread calling into EntityContext");
		}

		private class Data : StructuredData
		{
			public Data(IStructuredType type)
				: base (type)
			{
			}

			protected override bool CheckValueValidity(StructuredTypeField field, object value)
			{
				if (field.Type is IStructuredType)
				{
					System.Type type = EntityContext.current.FindEntityType (field.Type.CaptionId);
					
					switch (field.Relation)
					{
						case FieldRelation.None:
						case FieldRelation.Reference:
							return TypeRosetta.IsValidValue (value, type);

						case FieldRelation.Collection:
							return TypeRosetta.IsValidValueForCollectionOfType (value, type);

						case FieldRelation.Inclusion:
						//	TODO: handle inclusion

						default:
							throw new System.NotImplementedException (string.Format ("Support for Relation.{0} not implemented", field.Relation));
					}
				}
				else
				{
					return base.CheckValueValidity (field, value);
				}
			}
		}

		[System.ThreadStatic]
		private static EntityContext current;

		private readonly ResourceManagerPool resourceManagerPool;
		private readonly ResourceManager resourceManager;
		private readonly System.Threading.Thread associatedThread;
		private readonly Dictionary<Druid, IStructuredType> structuredTypeMap;
		private readonly Dictionary<Druid, System.Type> systemTypeMap;
	}
}
