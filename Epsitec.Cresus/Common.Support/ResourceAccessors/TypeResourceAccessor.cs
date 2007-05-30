//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

#if false
namespace Epsitec.Common.Support.ResourceAccessors
{
	using CultureInfo=System.Globalization.CultureInfo;
	
	/// <summary>
	/// The <c>TypeResourceAccessor</c> is used to access caption resources,
	/// stored in the <c>Captions</c> resource bundle and which have a field
	/// name prefixed with <c>"Typ."</c>.
	/// </summary>
	public class TypeResourceAccessor : AbstractCaptionResourceAccessor
	{
		public TypeResourceAccessor()
		{
		}

		protected virtual string Prefix
		{
			get
			{
				return "Typ.";
			}
		}

		protected override IStructuredType GetStructuredType()
		{
			return Res.Types.ResourceCaption;
		}

		protected override string GetNameFromFieldName(string fieldName)
		{
			System.Diagnostics.Debug.Assert (fieldName.StartsWith (this.Prefix));
			return fieldName.Substring (this.Prefix.Length);
		}

		protected override string GetFieldNameFromName(Types.StructuredData data, string name)
		{
			return this.Prefix + name;
		}

		protected override Caption GetCaptionFromData(Types.StructuredData data, string name)
		{
			string description = data.GetValue (Res.Fields.ResourceCaption.Description) as string;
			string icon = data.GetValue (Res.Fields.ResourceCaption.Icon) as string;

			Caption caption = new Caption ();

			IEnumerable<string> labels = data.GetValue (Res.Fields.ResourceCaption.Labels) as IEnumerable<string>;

			if (labels != null)
			{
				foreach (string label in labels)
				{
					caption.Labels.Add (label);
				}
			}

			if (name != null)
			{
				caption.Name = name;
			}

			if (description != null)
			{
				caption.Description = description;
			}

			if (icon != null)
			{
				caption.Icon = icon;
			}

			return caption;
		}

		protected override void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption)
		{
			Types.Collections.ObservableList<string> labels = new Epsitec.Common.Types.Collections.ObservableList<string> ();
			labels.AddRange (caption.Labels);

			//	The labels property can be accessed as an IList<string> and any modification
			//	will be reported to the listener.
			
			data.SetValue (Res.Fields.ResourceCaption.Labels, labels);
			data.LockValue (Res.Fields.ResourceCaption.Labels);
			labels.CollectionChanged += new Listener (this, item).HandleCollectionChanged;

			if (caption.Description != null)
			{
				data.SetValue (Res.Fields.ResourceCaption.Description, caption.Description);
			}
			if (caption.Icon != null)
			{
				data.SetValue (Res.Fields.ResourceCaption.Icon, caption.Icon);
			}
		}

		protected override bool FilterField(ResourceBundle.Field field)
		{
			return (!string.IsNullOrEmpty (field.Name))
				&& (field.Name.StartsWith (this.Prefix));
		}

		#region Listener Class

		protected class Listener
		{
			public Listener(TypeResourceAccessor accessor, CultureMap item)
			{
				this.accessor = accessor;
				this.item = item;
			}

			public void HandleCollectionChanged(object sender, CollectionChangedEventArgs e)
			{
				switch (e.Action)
				{
					case CollectionChangedAction.Add:
						this.HandleCollectionAdd (e.NewItems);
						break;
					
					case CollectionChangedAction.Remove:
						this.HandleCollectionRemove (e.OldItems);
						break;
					
					case CollectionChangedAction.Replace:
						this.HandleCollectionRemove (e.OldItems);
						this.HandleCollectionAdd (e.NewItems);
						break;
				}
				
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}: index {1} -> {2}", e.Action, e.OldStartingIndex, e.NewStartingIndex));
				this.accessor.NotifyItemChanged (this.item);
			}

			private void HandleCollectionAdd(System.Collections.IEnumerable list)
			{
				foreach (object item in list)
				{
					StructuredData data = item as StructuredData;
					
					if (data != null)
					{
						this.item.NotifyDataAdded (data);
					}
				}
			}

			private void HandleCollectionRemove(System.Collections.IEnumerable list)
			{
				foreach (object item in list)
				{
					StructuredData data = item as StructuredData;

					if (data != null)
					{
						this.item.NotifyDataRemoved (data);
					}
				}
			}

			private TypeResourceAccessor accessor;
			private CultureMap item;
		}

		#endregion
	}
}
#endif
