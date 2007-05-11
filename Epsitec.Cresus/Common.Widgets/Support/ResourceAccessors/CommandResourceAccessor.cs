//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	using CultureInfo=System.Globalization.CultureInfo;
	
	/// <summary>
	/// The <c>CommandResourceAccessor</c> is used to access command resources,
	/// stored in the <c>Captions</c> resource bundle and which have a field
	/// name prefixed with <c>"Cmd."</c>.
	/// </summary>
	public class CommandResourceAccessor : CaptionResourceAccessor
	{
		public CommandResourceAccessor()
		{
		}

		protected override string Prefix
		{
			get
			{
				return "Cmd.";
			}
		}

		protected override IStructuredType GetStructuredType()
		{
			return Res.Types.ResourceCommand;
		}

		protected override Caption GetCaptionFromData(Types.StructuredData data, string name)
		{
			Caption caption = base.GetCaptionFromData (data, name);

			if (!Types.UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceCommand.Statefull)))
			{
				Widgets.Command.SetStatefull (caption, (bool) data.GetValue (Res.Fields.ResourceCommand.Statefull));
			}

			IList<StructuredData> shortcuts = data.GetValue (Res.Fields.ResourceCommand.Shortcuts) as IList<StructuredData>;

			if (shortcuts != null)
			{
				IList<Widgets.Shortcut> target = Widgets.Shortcut.GetShortcuts (caption);

				foreach (StructuredData item in shortcuts)
				{
					Widgets.Shortcut shortcut = new Widgets.Shortcut ();
					shortcut.SetValue (Widgets.Shortcut.KeyCodeProperty, item.GetValue (Res.Fields.Shortcut.KeyCode) as string);
				}
			}
			
			return caption;
		}

		protected override void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption)
		{
			base.FillDataFromCaption (item, data, caption);

			ObservableList<StructuredData> shortcuts = new ObservableList<StructuredData> ();

			foreach (Widgets.Shortcut shortcut in Widgets.Shortcut.GetShortcuts (caption))
			{
				StructuredData x = new StructuredData (Res.Types.Shortcut);
				x.SetValue (Res.Fields.Shortcut.KeyCode, shortcut.GetValue (Widgets.Shortcut.KeyCodeProperty));
				shortcuts.Add (x);
			}
			
			data.SetValue (Res.Fields.ResourceCommand.Shortcuts, shortcuts);
			data.LockValue (Res.Fields.ResourceCommand.Shortcuts);
			shortcuts.CollectionChanged += new Listener (this, item).HandleCollectionChanged;

			if (!Types.UndefinedValue.IsUndefinedValue (caption.GetValue (Widgets.Command.StatefullProperty)))
			{
				data.SetValue (Res.Fields.ResourceCommand.Statefull, caption.GetValue (Widgets.Command.StatefullProperty));
			}
		}
	}
}
