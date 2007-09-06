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
	/// The <c>CaptionResourceAccessor</c> is used to access caption resources,
	/// stored in the <c>Captions</c> resource bundle and which have a field
	/// name prefixed with <c>"Cap."</c>.
	/// </summary>
	public class CaptionResourceAccessor : AbstractCaptionResourceAccessor
	{
		public CaptionResourceAccessor()
		{
		}

		protected virtual string Prefix
		{
			get
			{
				return "Cap.";
			}
		}

		protected override IStructuredType GetStructuredType()
		{
			return Res.Types.ResourceCaption;
		}

		protected override string GetNameFromFieldName(CultureMap item, string fieldName)
		{
			System.Diagnostics.Debug.Assert (fieldName.StartsWith (this.Prefix));
			
			return fieldName.Substring (this.Prefix.Length);
		}

		protected override string GetFieldNameFromName(CultureMap item, Types.StructuredData data)
		{
			return this.Prefix + item.Name;
		}

		/// <summary>
		/// Creates a caption based on the definitions stored in a data record.
		/// </summary>
		/// <param name="sourceBundle">The source bundle.</param>
		/// <param name="data">The data record.</param>
		/// <param name="name">The name of the caption.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>A <see cref="Caption"/> instance.</returns>
		protected override Caption CreateCaptionFromData(ResourceBundle sourceBundle, Types.StructuredData data, string name, string twoLetterISOLanguageName)
		{
			string description = data.GetValue (Res.Fields.ResourceCaption.Description) as string;
			string icon        = data.GetValue (Res.Fields.ResourceCaption.Icon) as string;

			Caption caption = new Caption ();

			IEnumerable<string> labels = data.GetValue (Res.Fields.ResourceCaption.Labels) as IEnumerable<string>;

			if (labels != null)
			{
				foreach (string label in labels)
				{
					caption.Labels.Add (label);
				}
			}

			if (!ResourceBundle.Field.IsNullString (name))
			{
				caption.Name = name;
			}

			if (!ResourceBundle.Field.IsNullString (description))
			{
				caption.Description = description;
			}

			if (!ResourceBundle.Field.IsNullString (icon))
			{
				caption.Icon = icon;
			}

			Support.ResourceManager.SetSourceBundle (caption, sourceBundle);

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
			ObservableList<string> labels = data.GetValue (Res.Fields.ResourceCaption.Labels) as ObservableList<string>;

			//	If the caption contains labels, they will overwrite the data
			//	record labels collection. Otherwise, the previously defined
			//	labels will remain unmodified in the data record.

			//	Note: a patch module can only replace the Labels list, but not
			//	alter it in any other more subtle way.
			
			if (labels == null)
			{
				labels = new ObservableList<string> ();
			}
			else if (caption.HasLabels)
			{
				labels.Clear ();
			}

			if (caption.HasLabels)
			{
				labels.AddRange (caption.Labels);
			}

			if (UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceCaption.Labels)))
			{
				//	The labels property can be accessed as an IList<string> and
				//	any modification will be reported to the listener. We must
				//	set up an event handler for that purpose :

				data.SetValue (Res.Fields.ResourceCaption.Labels, labels);
				data.LockValue (Res.Fields.ResourceCaption.Labels);

				if (mode == DataCreationMode.Public)
				{
					labels.CollectionChanged += new Listener (this, item).HandleCollectionChanged;
				}
			}

			if (caption.HasDescription)
			{
				data.SetValue (Res.Fields.ResourceCaption.Description, caption.Description);
			}
			if (caption.HasIcon)
			{
				data.SetValue (Res.Fields.ResourceCaption.Icon, caption.Icon);
			}
		}

		protected override bool FilterField(ResourceBundle.Field field, string fieldName)
		{
			return (!string.IsNullOrEmpty (fieldName))
				&& (fieldName.StartsWith (this.Prefix));
		}

		#region Listener Class

		protected class Listener
		{
			public Listener(CaptionResourceAccessor accessor, CultureMap item)
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

			private CaptionResourceAccessor accessor;
			private CultureMap item;
		}

		#endregion
	}
}
