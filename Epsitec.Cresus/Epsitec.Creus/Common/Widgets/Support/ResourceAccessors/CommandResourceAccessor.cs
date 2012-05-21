//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandResourceAccessor"/> class.
		/// </summary>
		public CommandResourceAccessor()
		{
		}

		/// <summary>
		/// Gets the data broker associated with the specified field. Usually,
		/// this is only meaningful if the field defines a collection of
		/// <see cref="StructuredData"/> items.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="fieldId">The id for the field in the specified container.</param>
		/// <returns>The data broker or <c>null</c>.</returns>
		public override IDataBroker GetDataBroker(StructuredData container, string fieldId)
		{
			if (fieldId == Res.Fields.ResourceCommand.Shortcuts.ToString ())
			{
				return new ShortcutBroker ();
			}
			else
			{
				return base.GetDataBroker (container, fieldId);
			}
		}

		/// <summary>
		/// Gets the caption prefix for this accessor.
		/// Note: several resource types are stored as captions; the prefix of
		/// the field name is used to differentiate them.
		/// </summary>
		/// <value>The caption <c>"Cmd."</c> prefix.</value>
		protected override string Prefix
		{
			get
			{
				return "Cmd.";
			}
		}

		/// <summary>
		/// Gets the structured type which describes the caption data.
		/// </summary>
		/// <returns>
		/// The <see cref="StructuredType"/> instance.
		/// </returns>
		protected override IStructuredType GetStructuredType()
		{
			return Res.Types.ResourceCommand;
		}

		/// <summary>
		/// Creates a caption based on the definitions stored in a data record.
		/// </summary>
		/// <param name="captionId">The caption id.</param>
		/// <param name="sourceBundle">The source bundle.</param>
		/// <param name="data">The data record.</param>
		/// <param name="name">The name of the caption.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>A <see cref="Caption"/> instance.</returns>
		protected override Caption CreateCaptionFromData(Druid captionId, ResourceBundle sourceBundle, Types.StructuredData data, string name, string twoLetterISOLanguageName)
		{
			Caption caption = base.CreateCaptionFromData (captionId, sourceBundle, data, name, twoLetterISOLanguageName);

			if (!Types.UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceCommand.Statefull)))
			{
				Widgets.Command.SetStatefull (caption, (bool) data.GetValue (Res.Fields.ResourceCommand.Statefull));
			}
			if (!Types.UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceCommand.DefaultParameter)))
			{
				Widgets.Command.SetDefaultParameter (caption, (string) data.GetValue (Res.Fields.ResourceCommand.DefaultParameter));
			}
			if (!Types.UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceCommand.Group)))
			{
				Widgets.Command.SetGroup (caption, (string) data.GetValue (Res.Fields.ResourceCommand.Group));
			}

			IList<StructuredData> shortcuts = data.GetValue (Res.Fields.ResourceCommand.Shortcuts) as IList<StructuredData>;

			if (shortcuts != null)
			{
				IList<Widgets.Shortcut> target = Widgets.Shortcut.GetShortcuts (caption);
				target.Clear ();

				foreach (StructuredData item in shortcuts)
				{
					Widgets.Shortcut shortcut = new Widgets.Shortcut ();
					shortcut.SetValue (Widgets.Shortcut.KeyCodeProperty, item.GetValue (Res.Fields.Shortcut.KeyCode) as string);
					target.Add (shortcut);
				}
			}
			
			return caption;
		}

		/// <summary>
		/// Fills the data record from a given caption.
		/// </summary>
		/// <param name="item">The item associated with the data record.</param>
		/// <param name="data">The data record.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="mode">The creation mode for the data record.</param>
		protected override void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption, DataCreationMode mode)
		{
			base.FillDataFromCaption (item, data, caption, mode);

			bool recordShortcuts = false;
			bool containsShortcuts = Widgets.Shortcut.HasShortcuts (caption);

			ObservableList<StructuredData> shortcuts = data.GetValue (Res.Fields.ResourceCommand.Shortcuts) as ObservableList<StructuredData>;

			if (shortcuts == null)
			{
				shortcuts = new ObservableList<StructuredData> ();
				recordShortcuts = true;
			}
			else if (containsShortcuts)
			{
				shortcuts.Clear ();
			}

			if (containsShortcuts)
			{
				IList<Widgets.Shortcut> captionShortcuts = Widgets.Shortcut.GetShortcuts (caption);

				foreach (Widgets.Shortcut captionShortcut in captionShortcuts)
				{
					StructuredData shortcut = new StructuredData (Res.Types.Shortcut);
					
					shortcut.SetValue (Res.Fields.Shortcut.KeyCode, captionShortcut.GetValue (Widgets.Shortcut.KeyCodeProperty));
					shortcuts.Add (shortcut);
					
					if (mode == DataCreationMode.Public)
					{
						item.NotifyDataAdded (shortcut);
					}
				}
			}

			if (recordShortcuts)
			{
				data.SetValue (Res.Fields.ResourceCommand.Shortcuts, shortcuts);
				data.LockValue (Res.Fields.ResourceCommand.Shortcuts);

				if (mode == DataCreationMode.Public)
				{
					ShortcutListener listener = new ShortcutListener (this, item, data);

					shortcuts.CollectionChanging += listener.HandleCollectionChanging;
					shortcuts.CollectionChanged  += listener.HandleCollectionChanged;
				}
			}

			object statefullValue        = caption.GetValue (Widgets.Command.StatefullProperty);
			object defaultParameterValue = caption.GetValue (Widgets.Command.DefaultParameterProperty);
			object groupValue            = caption.GetValue (Widgets.Command.GroupProperty);

			if (!Types.UndefinedValue.IsUndefinedValue (statefullValue))
			{
				data.SetValue (Res.Fields.ResourceCommand.Statefull, statefullValue);
			}
			if (!Types.UndefinedValue.IsUndefinedValue (defaultParameterValue))
			{
				data.SetValue (Res.Fields.ResourceCommand.DefaultParameter, defaultParameterValue);
			}
			if (!Types.UndefinedValue.IsUndefinedValue (groupValue))
			{
				data.SetValue (Res.Fields.ResourceCommand.Group, groupValue);
			}
		}

		/// <summary>
		/// Determines whether the specified data record describes an empty
		/// caption.
		/// </summary>
		/// <param name="data">The data record.</param>
		/// <returns>
		/// 	<c>true</c> if this is an empty caption; otherwise, <c>false</c>.
		/// </returns>
		protected override bool IsEmptyCaption(StructuredData data)
		{
			if (base.IsEmptyCaption (data))
			{
				object statefull        = data.GetValue (Res.Fields.ResourceCommand.Statefull);
				string defaultParameter = data.GetValue (Res.Fields.ResourceCommand.DefaultParameter) as string;
				string group            = data.GetValue (Res.Fields.ResourceCommand.Group) as string;

				IList<StructuredData> shortcuts = data.GetValue (Res.Fields.ResourceCommand.Shortcuts) as IList<StructuredData>;

				if ((UndefinedValue.IsUndefinedValue (statefull) || ((bool)statefull == false)) &&
					(ResourceBundle.Field.IsNullString (defaultParameter)) &&
					(ResourceBundle.Field.IsNullString (group)) &&
					((shortcuts == null) || (shortcuts.Count == 0)))
				{
					return true;
				}
			}
			
			return false;
		}

		/// <summary>
		/// Computes the difference between a raw data record and a reference
		/// data record and fills the patch data record with the resulting
		/// delta.
		/// </summary>
		/// <param name="rawData">The raw data record.</param>
		/// <param name="refData">The reference data record.</param>
		/// <param name="patchData">The patch data, which will be filled with the delta.</param>
		protected override void ComputeDataDelta(StructuredData rawData, StructuredData refData, StructuredData patchData)
		{
			base.ComputeDataDelta (rawData, refData, patchData);

#if true
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceCommand.DefaultParameter);
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceCommand.Group);
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceCommand.Shortcuts);
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceCommand.Statefull);
#else
			object refStatefull        = refData.GetValue (Res.Fields.ResourceCommand.Statefull);
			string refDefaultParameter = refData.GetValue (Res.Fields.ResourceCommand.DefaultParameter) as string;
			string refGroup            = refData.GetValue (Res.Fields.ResourceCommand.Group) as string;
			
			IList<StructuredData> refShortcuts = refData.GetValue (Res.Fields.ResourceCommand.Shortcuts) as IList<StructuredData>;
			
			object rawStatefull        = rawData.GetValue (Res.Fields.ResourceCommand.Statefull);
			string rawDefaultParameter = rawData.GetValue (Res.Fields.ResourceCommand.DefaultParameter) as string;
			string rawGroup            = rawData.GetValue (Res.Fields.ResourceCommand.Group) as string;
			
			IList<StructuredData> rawShortcuts = rawData.GetValue (Res.Fields.ResourceCommand.Shortcuts) as IList<StructuredData>;

			if ((!UndefinedValue.IsUndefinedValue (rawStatefull)) &&
				(refStatefull != rawStatefull))
			{
				patchData.SetValue (Res.Fields.ResourceCommand.Statefull, rawStatefull);
			}
			if ((!ResourceBundle.Field.IsNullString (rawDefaultParameter)) &&
				(refDefaultParameter != rawDefaultParameter))
			{
				patchData.SetValue (Res.Fields.ResourceCommand.DefaultParameter, rawDefaultParameter);
			}
			if ((!ResourceBundle.Field.IsNullString (rawGroup)) &
				(refGroup != rawGroup))
			{
				patchData.SetValue (Res.Fields.ResourceCommand.Group, rawGroup);
			}

			if ((rawShortcuts != null) &&
				(rawShortcuts.Count > 0) &&
				(!Types.Collection.CompareEqual (rawShortcuts, refShortcuts)))
			{
				patchData.SetValue (Res.Fields.ResourceCommand.Shortcuts, new List<StructuredData> (rawShortcuts));
			}
#endif
		}

		/// <summary>
		/// Resets the specified field to its original value. This is the
		/// internal implementation which can be overridden.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="container">The data record.</param>
		/// <param name="fieldId">The field id.</param>
		protected override void ResetToOriginal(CultureMap item, StructuredData container, Druid fieldId)
		{
			if (fieldId == Res.Fields.ResourceCommand.Shortcuts)
			{
				ShortcutListener listener = Listener.FindListener<ShortcutListener> (container, fieldId);

				System.Diagnostics.Debug.Assert (listener != null);
				System.Diagnostics.Debug.Assert (listener.Item == item);
				System.Diagnostics.Debug.Assert (listener.Data == container);

				listener.ResetToOriginalValue ();
			}
			else
			{
				base.ResetToOriginal (item, container, fieldId);
			}
		}

		#region Broker Class

		private class ShortcutBroker : IDataBroker
		{
			#region IDataBroker Members

			public StructuredData CreateData(CultureMap container)
			{
				return new StructuredData (Res.Types.Shortcut);
			}

			#endregion
		}

		#endregion

		#region ShortcutListener Class

		protected class ShortcutListener : Listener
		{
			public ShortcutListener(CommandResourceAccessor accessor, CultureMap item, StructuredData data)
				: base (accessor, item, data)
			{
			}

			public override void HandleCollectionChanging(object sender)
			{
				if (this.SaveField (Res.Fields.ResourceCommand.Shortcuts))
				{
					this.originalShortcuts = new List<StructuredData> ();

					IList<StructuredData> shortcuts = this.Data.GetValue (Res.Fields.ResourceCommand.Shortcuts) as IList<StructuredData>;

					foreach (StructuredData data in shortcuts)
					{
						this.originalShortcuts.Add (data.GetShallowCopy ());
					}
				}
			}

			public override void ResetToOriginalValue()
			{
				if (this.originalShortcuts != null)
				{
					this.RestoreField (Res.Fields.ResourceCommand.Shortcuts);

					ObservableList<StructuredData> shortcuts = this.Data.GetValue (Res.Fields.ResourceCommand.Shortcuts) as ObservableList<StructuredData>;

					using (shortcuts.DisableNotifications ())
					{
						int index = shortcuts.Count - 1;

						while (index >= 0)
						{
							StructuredData data = shortcuts[index];
							shortcuts.RemoveAt (index--);
							this.Item.NotifyDataRemoved (data);
						}

						System.Diagnostics.Debug.Assert (shortcuts.Count == 0);

						foreach (StructuredData data in this.originalShortcuts)
						{
							StructuredData copy = data.GetShallowCopy ();
							copy.PromoteToOriginal ();
							shortcuts.Add (copy);
							this.Item.NotifyDataAdded (copy);
						}
					}
				}
			}

			List<StructuredData> originalShortcuts;
		}

		#endregion
	}
}
