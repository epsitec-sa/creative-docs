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
	/// The <c>CaptionResourceAccessor</c> is used to access caption resources,
	/// stored in the <c>Captions</c> resource bundle and which have a field
	/// name prefixed with <c>"Cap."</c>.
	/// </summary>
	public class CaptionResourceAccessor : AbstractCaptionResourceAccessor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CaptionResourceAccessor"/> class.
		/// </summary>
		public CaptionResourceAccessor()
		{
		}

		/// <summary>
		/// Gets the caption prefix for this accessor.
		/// Note: several resource types are stored as captions; the prefix of
		/// the field name is used to differentiate them.
		/// </summary>
		/// <value>The caption <c>"Cap."</c> prefix.</value>
		protected virtual string Prefix
		{
			get
			{
				return "Cap.";
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
			return Res.Types.ResourceCaption;
		}

		/// <summary>
		/// Gets the pure caption name from a field name. This simply strips of
		/// the field name prefix.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="fieldName">The field name.</param>
		/// <returns>The pure caption name.</returns>
		protected override string GetNameFromFieldName(CultureMap item, string fieldName)
		{
			System.Diagnostics.Debug.Assert (fieldName.StartsWith (this.Prefix));
			
			return fieldName.Substring (this.Prefix.Length);
		}

		/// <summary>
		/// Gets the resource field name of the resource based on the caption
		/// name.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="data">The data record.</param>
		/// <returns>The resource field name.</returns>
		protected override string GetFieldNameFromName(CultureMap item, Types.StructuredData data)
		{
			return string.Concat (this.Prefix, item.Name);
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
			string description = data.GetValue (Res.Fields.ResourceCaption.Description) as string;
			string icon        = data.GetValue (Res.Fields.ResourceCaption.Icon) as string;

			if (!this.BasedOnPatchModule)
			{
				//	Never store an empty string as the icon specification in the
				//	reference module; storing null instead is more compact !

				if (icon == "")
				{
					icon = null;
				}
			}

			Caption caption = new Caption ();

			IList<string> labels = data.GetValue (Res.Fields.ResourceCaption.Labels) as IList<string>;

			if ((labels != null) &&
				(labels.Count > 0) &&
				(!((labels.Count == 1) && (ResourceBundle.Field.IsNullString (labels[0])))))
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

			caption.DefineId (captionId);

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
			bool recordLabels = false;
			
			ObservableList<string> labels = data.GetValue (Res.Fields.ResourceCaption.Labels) as ObservableList<string>;

			//	If the caption contains labels, they will overwrite the data
			//	record labels collection. Otherwise, the previously defined
			//	labels will remain unmodified in the data record.

			//	Note: a patch module can only replace the Labels list, but not
			//	alter it in any other more subtle way.
			
			if (labels == null)
			{
				labels = new ObservableList<string> ();
				recordLabels = true;
			}
			else if (caption.HasLabels)
			{
				labels.Clear ();
			}

			//	TODO: always overwrite the labels list

			if (caption.HasLabels)
			{
				labels.AddRange (caption.Labels);
			}

			if (recordLabels)
			{
				//	The labels property can be accessed as an IList<string> and
				//	any modification will be reported to the listener. We must
				//	set up an event handler for that purpose :

				data.SetValue (Res.Fields.ResourceCaption.Labels, labels);
				data.LockValue (Res.Fields.ResourceCaption.Labels);

				if (mode == DataCreationMode.Public)
				{
					LabelListener listener = new LabelListener (this, item, data);

					labels.CollectionChanging += listener.HandleCollectionChanging;
					labels.CollectionChanged  += listener.HandleCollectionChanged;
				}
			}

			if (caption.HasDescription)
			{
				data.SetValue (Res.Fields.ResourceCaption.Description, caption.Description);
			}

			string icon = caption.Icon;

			if (item.Source == CultureMapSource.ReferenceModule)
			{
				//	If an icon is null in a reference module, replace with an
				//	empty string, which for the Designer means "no icon", whereas
				//	a null string would mean "default from reference", which is
				//	meaningless in the context :

				if (icon == null)
				{
					icon = "";
				}
			}

			if (UndefinedValue.IsUndefinedValue (data.GetValue (Res.Fields.ResourceCaption.Icon)))
			{
				data.SetValue (Res.Fields.ResourceCaption.Icon, icon);
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
			IList<string> labels = data.GetValue (Res.Fields.ResourceCaption.Labels) as IList<string>;

			string description = data.GetValue (Res.Fields.ResourceCaption.Description) as string;
			string icon        = data.GetValue (Res.Fields.ResourceCaption.Icon) as string;

			return ((labels == null) || (labels.Count == 0) || ((labels.Count == 1) && (ResourceBundle.Field.IsNullString (labels[0]))))
				&& (ResourceBundle.Field.IsNullString (description))
				&& (ResourceBundle.Field.IsNullString (icon));
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
#if true
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceCaption.Description);
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceCaption.Icon);
			AbstractCaptionResourceAccessor.CopyDeltaValue (rawData, patchData, Res.Fields.ResourceCaption.Labels);
#else
			string        rawDesc   = rawData.GetValue (Res.Fields.ResourceCaption.Description) as string;
			string        rawIcon   = rawData.GetValue (Res.Fields.ResourceCaption.Icon) as string;
			IList<string> rawLabels = rawData.GetValue (Res.Fields.ResourceCaption.Labels) as IList<string>;

			string        refDesc   = refData.GetValue (Res.Fields.ResourceCaption.Description) as string;
			string        refIcon   = refData.GetValue (Res.Fields.ResourceCaption.Icon) as string;
			IList<string> refLabels = refData.GetValue (Res.Fields.ResourceCaption.Labels) as IList<string>;

			if (!ResourceBundle.Field.IsNullString (rawDesc) &&
				(rawDesc != refDesc))
			{
				patchData.SetValue (Res.Fields.ResourceCaption.Description, rawDesc);
			}
			if (!ResourceBundle.Field.IsNullString (rawIcon) &&
				(rawIcon != refIcon))
			{
				patchData.SetValue (Res.Fields.ResourceCaption.Icon, rawIcon);
			}
			if ((rawLabels != null) &&
				(rawLabels.Count > 0) &&
				(!Types.Collection.CompareEqual (rawLabels, refLabels)))
			{
				patchData.SetValue (Res.Fields.ResourceCaption.Labels, new List<string> (rawLabels));
			}
#endif
		}

		/// <summary>
		/// Checks if the data stored in the field matches this accessor. This
		/// can be used to filter out specific fields.
		/// </summary>
		/// <param name="field">The field to check.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns>
		/// 	<c>true</c> if data should be loaded from the field; otherwise, <c>false</c>.
		/// </returns>
		protected override bool FilterField(ResourceBundle.Field field, string fieldName)
		{
			return (!string.IsNullOrEmpty (fieldName))
				&& (fieldName.StartsWith (this.Prefix));
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
			if (fieldId == Res.Fields.ResourceCaption.Labels)
			{
				LabelListener listener = Listener.FindListener<LabelListener> (container, fieldId);

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

		#region Listener Class

		/// <summary>
		/// The <c>Listener</c> class is used to handle changes to fields which
		/// contain collection of items and for which the automatic notification
		/// mechanism implemented by <c>StructuredData</c> does not work.
		/// </summary>
		protected abstract class Listener
		{
			public Listener(CaptionResourceAccessor accessor, CultureMap item, StructuredData data)
			{
				this.accessor = accessor;
				this.item = item;
				this.data = data;
			}

			public CultureMap Item
			{
				get
				{
					return this.item;
				}
			}

			public StructuredData Data
			{
				get
				{
					return this.data;
				}
			}

			public CaptionResourceAccessor Accessor
			{
				get
				{
					return this.accessor;
				}
			}

			public abstract void HandleCollectionChanging(object sender);

			public abstract void ResetToOriginalValue();

			public virtual void HandleCollectionChanged(object sender, CollectionChangedEventArgs e)
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
				
				this.accessor.NotifyItemChanged (this.item, this.data, null);
			}

			/// <summary>
			/// Finds the listener associated with a given value in the data record.
			/// </summary>
			/// <param name="data">The data record.</param>
			/// <param name="id">The field id.</param>
			/// <returns>The <c>Listener</c> instance or <c>null</c>.</returns>
			public static T FindListener<T>(StructuredData data, Druid id) where T : Listener
			{
				return Listener.FindListener<T> (data.GetValue (id) as AbstractObservableList);
			}

			public static T FindListener<T>(AbstractObservableList list) where T : Listener
			{
				if (list != null)
				{
					return list.GetCollectionChangingTarget (0) as T;
				}

				return null;
			}

			/// <summary>
			/// Checks whether the specified field contains an original value.
			/// </summary>
			/// <param name="id">The field id.</param>
			/// <returns><c>true</c> if the field contains an original value; otherwise, <c>false</c>.</returns>
			protected bool UsesOriginalValue(Druid id)
			{
				if (this.accessor.BasedOnPatchModule)
				{
					bool usesOriginalData;
					
					this.data.GetValue (id, out usesOriginalData);
					
					return usesOriginalData;
				}
				else
				{
					return false;
				}
			}

			protected bool SaveField(Druid id)
			{
				if (this.UsesOriginalValue (id))
				{
					this.data.UnlockValue (id);
					this.data.CopyOriginalToCurrentValue (id);
					this.data.LockValue (id);
					
					return true;
				}
				else
				{
					return false;
				}
			}

			protected void RestoreField(Druid id)
			{
				this.data.UnlockValue (id);
				this.data.ResetToOriginalValue (id);
				this.data.LockValue (id);
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
			private StructuredData data;
		}

		#endregion

		#region LabelListener Class

		protected class LabelListener : Listener
		{
			public LabelListener(CaptionResourceAccessor accessor, CultureMap item, StructuredData data)
				: base (accessor, item, data)
			{
			}

			public override void HandleCollectionChanging(object sender)
			{
				if (this.SaveField (Res.Fields.ResourceCaption.Labels))
				{
					this.originalLabels = new List<string> (this.Data.GetValue (Res.Fields.ResourceCaption.Labels) as IList<string>);
				}
			}

			public override void ResetToOriginalValue()
			{
				if (this.originalLabels != null)
				{
					this.RestoreField (Res.Fields.ResourceCaption.Labels);

					ObservableList<string> labels = this.Data.GetValue (Res.Fields.ResourceCaption.Labels) as ObservableList<string>;

					using (labels.DisableNotifications ())
					{
						labels.Clear ();
						labels.AddRange (this.originalLabels);
					}
				}
			}

			private List<string> originalLabels;
		}

		#endregion
	}
}
