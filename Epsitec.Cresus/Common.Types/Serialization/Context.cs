//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public class Context
	{
		public Context()
		{
			this.visitor = new GraphVisitor ();
			this.typeIds = new Dictionary<System.Type, int> ();
		}
		public Context(IO.AbstractReader reader)
			: this ()
		{
			this.reader = reader;
		}
		public Context(IO.AbstractWriter writer)
			: this ()
		{
			this.writer = writer;
		}
		
		public GraphVisitor						Visitor
		{
			get
			{
				return this.visitor;
			}
		}
		public Generic.Map<DependencyObject>	ObjectMap
		{
			get
			{
				return this.visitor.ObjectMap;
			}
		}
		
		public IO.AbstractReader				Reader
		{
			get
			{
				return this.reader;
			}
		}
		public IO.AbstractWriter				Writer
		{
			get
			{
				return this.writer;
			}
		}
		
		internal void DefineType(int id, System.Type type)
		{
			this.AssertWritable ();
			
			System.Diagnostics.Debug.Assert (this.typeIds.ContainsKey (type) == false);
			
			this.typeIds[type] = id;
			this.writer.WriteTypeDefinition (id, type.FullName);
		}

		internal void DefineObject(int id, DependencyObject obj)
		{
			this.AssertWritable ();
			
			System.Type type = obj.GetType ();
			int typeId = this.typeIds[type];

			this.writer.WriteObjectDefinition (id, typeId);
		}

		internal void StoreObject(int id, DependencyObject obj)
		{
			this.AssertWritable ();

			this.writer.WriteObjectData (id, obj);
		}

		private void AssertWritable()
		{
			if (this.writer == null)
			{
				throw new System.InvalidOperationException (string.Format ("No writer associated with serialization context"));
			}
		}
		private void AssertReadable()
		{
			if (this.reader == null)
			{
				throw new System.InvalidOperationException (string.Format ("No reader associated with serialization context"));
			}
		}
		
		
		private GraphVisitor visitor;
		private Dictionary<System.Type, int> typeIds;
		private IO.AbstractWriter writer;
		private IO.AbstractReader reader;
	}
}
