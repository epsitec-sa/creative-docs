//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.StructuredCommand))]

namespace Epsitec.Common.Widgets
{
	public class StructuredCommand : DependencyObject
	{
		private StructuredCommand()
		{
		}
		
#if false
		public void AddField(string name, INamedType type)
		{
			this.type.AddField (name, type);
		}
#endif

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
	
		internal class StructuredState : CommandState, IStructuredData
		{
			public StructuredState()
			{
			}

			protected override void OverrideDefineCommand()
			{
				base.OverrideDefineCommand ();

				this.data = new StructuredData (this.Command.StructuredType);
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

			IEnumerable<string> IStructuredData.GetValueNames()
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

			#endregion

			private Types.StructuredData data;
		}

		public static StructuredType GetStructuredType(DependencyObject obj)
		{
			return obj.GetValue (StructuredCommand.StructuredTypeProperty) as StructuredType;
		}

		private static object GetStructuredTypeValue(DependencyObject obj)
		{
			StructuredType data = obj.GetValueBase (StructuredCommand.StructuredTypeProperty) as StructuredType;

			if (data == null)
			{
				data = new StructuredType ();
				obj.SetLocalValue (StructuredCommand.StructuredTypeProperty, data);
				obj.InvalidateProperty (StructuredCommand.StructuredTypeProperty, null, data);
			}

			return data;
		}
		
		public static readonly DependencyProperty StructuredTypeProperty = DependencyProperty.RegisterAttached ("StructuredType", typeof (StructuredType), typeof (StructuredCommand), new DependencyPropertyMetadata (StructuredCommand.GetStructuredTypeValue));
	}
}
