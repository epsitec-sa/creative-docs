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
		
		public static void SetFieldValue(CommandState commandState, string id, object value)
		{
			StructuredState state = commandState as StructuredState;
			IStructuredData data = state as IStructuredData;

			System.Diagnostics.Debug.Assert (state != null);
			System.Diagnostics.Debug.Assert (data != null);
			System.Diagnostics.Debug.Assert (id != null);

			data.SetValue (id, value);
		}

		public static object GetFieldValue(CommandState commandState, string id)
		{
			StructuredState state = commandState as StructuredState;
			IStructuredData data = state as IStructuredData;

			System.Diagnostics.Debug.Assert (state != null);
			System.Diagnostics.Debug.Assert (data != null);
			System.Diagnostics.Debug.Assert (id != null);

			return data.GetValue (id);
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

			void IStructuredData.AttachListener(string id, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				this.data.AttachListener (id, handler);
			}

			void IStructuredData.DetachListener(string id, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				this.data.DetachListener (id, handler);
			}

			IEnumerable<string> IStructuredData.GetValueIds()
			{
				return this.data.GetValueIds ();
			}

			object IStructuredData.GetValue(string id)
			{
				return this.data.GetValue (id);
			}

			void IStructuredData.SetValue(string id, object value)
			{
				this.data.SetValue (id, value);
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
