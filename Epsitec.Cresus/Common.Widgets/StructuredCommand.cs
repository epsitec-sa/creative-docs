//	Copyright � 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (StructuredCommand))]

namespace Epsitec.Common.Widgets
{
	public static class StructuredCommand
	{
		public static Command GetStructuredCommand(string commandName)
		{
			Command command = Command.Find (commandName);

			if ((command == null) || 
				(command.CommandType == CommandType.Standard))
			{
				command = Command.Get (commandName);
				Command.SetCommandType (command.Caption, CommandType.Structured);

				Types.StructuredType type = command.StructuredType;
				type.Fields.Add ("Name", Types.StringType.Default);
			}

			return command;
		}

		
		public static string GetNameFieldValue(this CommandState commandState)
		{
			return StructuredCommand.GetFieldValue (commandState, "Name") as string;
		}

		public static void SetNameFieldValue(this CommandState commandState, string value)
		{
			StructuredCommand.SetFieldValue (commandState, "Name", value);
		}

		
		public static object GetFieldValue(CommandState commandState, string id)
		{
			StructuredCommandState state = commandState as StructuredCommandState;
			IStructuredData data = state as IStructuredData;

			System.Diagnostics.Debug.Assert (state != null);
			System.Diagnostics.Debug.Assert (data != null);
			System.Diagnostics.Debug.Assert (id != null);

			return data.GetValue (id);
		}

		public static void SetFieldValue(CommandState commandState, string id, object value)
		{
			StructuredCommandState state = commandState as StructuredCommandState;
			IStructuredData data = state as IStructuredData;

			System.Diagnostics.Debug.Assert (state != null);
			System.Diagnostics.Debug.Assert (data != null);
			System.Diagnostics.Debug.Assert (id != null);

			data.SetValue (id, value);
		}


		public static void EnsureNameField(this Command command)
		{
			Types.StructuredType type = command.StructuredType;

			if (type.Fields.ContainsKey ("Name") == false)
			{
				type.Fields.Add ("Name", Types.StringType.Default);
			}
		}

		public static bool ContainsNameField(this Command command)
		{
			if (command.CommandType == CommandType.Structured)
			{
				INamedType typeObject = command.StructuredType.GetField ("Name").Type;

				return typeObject is Types.StringType;
			}
			else
			{
				return false;
			}
		}


		#region StructuredState Class

		internal class StructuredCommandState : CommandState, IStructuredData
		{
			public StructuredCommandState()
			{
			}

			protected override void OverrideDefineCommand()
			{
				base.OverrideDefineCommand ();

				this.data = new StructuredData (this.Command.StructuredType);
			}

			#region IStructuredData Members

			void IStructuredData.AttachListener(string id, EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				this.data.AttachListener (id, handler);
			}

			void IStructuredData.DetachListener(string id, EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				this.data.DetachListener (id, handler);
			}

			void IStructuredData.SetValue(string id, object value)
			{
				this.data.SetValue (id, value);
			}

			IEnumerable<string> IStructuredData.GetValueIds()
			{
				return this.data.GetValueIds ();
			}

			object IValueStore.GetValue(string id)
			{
				return this.data.GetValue (id);
			}

			void IValueStore.SetValue(string id, object value, ValueStoreSetMode mode)
			{
				this.data.SetValue (id, value);
			}

			#endregion

			private Types.StructuredData data;
		}

		#endregion

		internal static StructuredType GetStructuredType(DependencyObject obj)
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
