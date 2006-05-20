//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	public class StructuredCommand : Command, IStructuredType
	{
		public StructuredCommand(string name) : base (name)
		{
			this.StateObjectType = Types.DependencyObjectType.FromSystemType (typeof (StructuredState));
		}

		public void AddField(string name, INamedType type)
		{
			this.type.AddField (name, type);
		}

		public static void SetFieldValue(CommandState commandState, string name, object value)
		{
			StructuredState state = commandState as StructuredState;
			IStructuredData data = state as IStructuredData;

			System.Diagnostics.Debug.Assert (state != null);
			System.Diagnostics.Debug.Assert (data != null);
			System.Diagnostics.Debug.Assert (name != null);

			data.SetValue (name, value);
		}

		public static object GetFieldValue(CommandState commandState, string name)
		{
			StructuredState state = commandState as StructuredState;
			IStructuredData data = state as IStructuredData;

			System.Diagnostics.Debug.Assert (state != null);
			System.Diagnostics.Debug.Assert (data != null);
			System.Diagnostics.Debug.Assert (name != null);

			return data.GetValue (name);
		}
		
		#region IStructuredType Members

		string[] IStructuredType.GetFieldNames()
		{
			return this.type.GetFieldNames ();
		}

		object IStructuredType.GetFieldTypeObject(string name)
		{
			return this.type.GetFieldTypeObject (name);
		}
		
		#endregion

		private class StructuredState : CommandState, IStructuredData
		{
			public StructuredState()
			{
			}

			protected override void OverrideDefineCommand()
			{
				base.OverrideDefineCommand ();
				
				this.data = new StructuredData (this.Command as IStructuredType);
			}

			#region IStructuredData Members

			void IStructuredData.AttachListener(string name, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				this.data.AttachListener (name, handler);
			}

			void IStructuredData.DetachListener(string name, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				this.data.DetachListener (name, handler);
			}

			string[] IStructuredData.GetValueNames()
			{
				return this.data.GetValueNames ();
			}

			object IStructuredData.GetValue(string name)
			{
				return this.data.GetValue (name);
			}

			void IStructuredData.SetValue(string name, object value)
			{
				this.data.SetValue (name, value);
			}

			bool IStructuredData.HasImmutableRoots
			{
				get
				{
					return this.data.HasImmutableRoots;
				}
			}

			#endregion

			private Types.StructuredData data;
		}

		private StructuredType type = new StructuredType ();
	}
}
