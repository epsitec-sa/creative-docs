//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>EnumValue</c> class describes a single value in an enumeration.
	/// See also <see cref="T:EnumType"/>.
	/// </summary>
	[Types.SerializationConverter (typeof (EnumValue.SerializationConverter))]
	public class EnumValue : IEnumValue
	{
		public EnumValue()
		{
		}

		public EnumValue(int rank, Support.Druid captionId)
			: this (NotAnEnum.Instance, rank, false, captionId)
		{
		}

		public EnumValue(int rank, Caption caption)
			: this (NotAnEnum.Instance, rank, false, caption)
		{
		}

		public EnumValue(System.Enum value, int rank, bool hidden, string name)
		{
			this.DefineName (name);
			this.DefineValue (value);
			this.DefineRank (rank);
			this.DefineHidden (hidden);
			this.DefineIsNative (value is NotAnEnum ? false : true);
		}

		public EnumValue(System.Enum value, int rank, bool hidden, Support.Druid captionId)
		{
			this.DefineCaptionId (captionId);
			this.DefineValue (value);
			this.DefineRank (rank);
			this.DefineHidden (hidden);
			this.DefineIsNative (value is NotAnEnum ? false : true);
		}

		public EnumValue(System.Enum value, int rank, bool hidden, Caption caption)
		{
			this.DefineCaption (caption);
			this.DefineValue (value);
			this.DefineRank (rank);
			this.DefineHidden (hidden);
			this.DefineIsNative (value is NotAnEnum ? false : true);
		}

		private EnumValue(Support.Druid captionId)
		{
			this.DefineCaptionId (captionId);
		}

		/// <summary>
		/// Gets the name of the object.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				if (this.name != null)
				{
					return this.name;
				}

				if ((this.caption == null) &&
					(this.captionId.IsEmpty))
				{
					return null;
				}

				return this.Caption.Name;
			}
		}

		/// <summary>
		/// Gets the caption of the object.
		/// </summary>
		/// <value>The caption.</value>
		public Caption Caption
		{
			get
			{
				if (this.caption == null)
				{
					if (this.captionId.IsEmpty)
					{
						this.DefineCaption (new Caption ());

						this.caption.Name = this.name;
						this.name = null;
					}
					else
					{
						this.DefineCaption (Support.Resources.DefaultManager.GetCaption (this.captionId));
					}
				}

				return this.caption;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this value belongs to a native <c>enum</c>.
		/// </summary>
		/// <value><c>true</c> if this value belongs to a native <c>enum</c>; otherwise, <c>false</c>.</value>
		public bool IsNative
		{
			get
			{
				return this.isNative;
			}
		}

		public void DefineValue(System.Enum value)
		{
			if (this.Value != value)
			{
				this.value = value;
			}
		}

		public void DefineRank(int rank)
		{
			if (this.Rank != rank)
			{
				this.rank = rank;
			}
		}

		public void DefineHidden(bool hide)
		{
			if (this.IsHidden != hide)
			{
				this.isHidden = hide;
			}
		}

		public void DefineIsNative(bool value)
		{
			if (this.IsNative != value)
			{
				this.isNative = value;
			}
			
			if ((this.isNative) &&
				(this.caption != null))
			{
				EnumType.SetIsNative (this.caption, this.isNative);
			}
		}


		#region IName Members

		string IName.Name
		{
			get
			{
				return this.Name;
			}
		}

		#endregion

		#region INameCaption Members

		public Support.Druid CaptionId
		{
			get
			{
				return this.captionId;
			}
		}

		#endregion

		#region IEnumValue Members

		public System.Enum Value
		{
			get
			{
				return this.value;
			}
		}
		
		public int Rank
		{
			get
			{
				return this.rank;
			}
		}

		public bool IsHidden
		{
			get
			{
				return this.isHidden;
			}
		}
		
		#endregion

		#region SerializationConverter Class

		public class SerializationConverter : Types.ISerializationConverter, Types.ISerializationConverterFilter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, Types.IContextResolver context)
			{
				EnumValue enumValue = (EnumValue) value;

				string        rank  = enumValue.Rank.ToString (System.Globalization.CultureInfo.InvariantCulture);
				Support.Druid druid = enumValue.CaptionId;
				
				return string.Concat (rank, " ", druid.ToString ());
			}

			public object ConvertFromString(string value, Types.IContextResolver context)
			{
				string[] args = value.Split (' ');

				int rank;
				Support.Druid druid;

				if (args.Length == 2)
				{
					rank  = System.Int32.Parse (args[0], System.Globalization.CultureInfo.InvariantCulture);
					druid = Support.Druid.Parse (args[1]);
				}
				else
				{
					throw new System.FormatException ();
				}
				
				//	We need to know the resource manager which is used for the
				//	deserialization, so that we can properly map the DRUIDs to
				//	their associated captions when working within the Designer :
				
				Support.ResourceManager manager = context == null ? null : context.ExternalMap.GetValue (Serialization.Context.WellKnownTagResourceManager) as Support.ResourceManager;

				if (manager == null)
				{
					EnumValue enumValue = new EnumValue (druid);
					enumValue.rank = rank;
					
					return enumValue;
				}
				else
				{
					Caption caption = manager.GetCaption (druid);

					if (caption == null)
					{
						throw new System.InvalidOperationException ("DRUID cannot map to a Caption; probably incorrect ResourceManager specified");
					}
					
					EnumValue enumValue = new EnumValue (rank, caption);

					return enumValue;
				}
			}

			#endregion

			#region ISerializationConverterFilter Members

			public bool IsSerializable(object value, IContextResolver context)
			{
				EnumValue enumValue = (EnumValue) value;

				return enumValue.CaptionId.IsValid;
			}

			#endregion
		}

		#endregion
		
		internal void Lock()
		{
			this.isReadOnly = true;
		}

		public void DefineName(string name)
		{
			if (this.isReadOnly)
			{
				throw new System.InvalidOperationException ("The name is locked and cannot be changed");
			}

			if (this.caption == null)
			{
				this.name = name;
			}
			else
			{
				this.caption.Name = name;
				this.name = null;
			}
		}

		public void DefineCaptionId(Support.Druid druid)
		{
			if (this.captionId.IsEmpty)
			{
				this.captionId = druid;
			}
			else
			{
				throw new System.InvalidOperationException ("The caption DRUID cannot be changed");
			}
		}

		public void DefineCaption(Caption caption)
		{
			if (caption == null)
			{
				throw new System.ArgumentNullException ("caption");
			}

			if ((this.caption != null) &&
				(this.captionId.IsEmpty))
			{
				this.caption = null;
			}
			
			if ((this.caption == null) &&
				(caption != null) &&
				(this.captionId.IsEmpty || (this.captionId == caption.Druid)))
			{
				this.caption = caption;
				this.captionId = caption.Druid;

				this.OnCaptionDefined ();
			}
			else
			{
				throw new System.InvalidOperationException ("The caption cannot be changed");
			}
		}

		internal static void CopyProperties(EnumValue srcValue, EnumValue dstValue)
		{
			if ((dstValue != null) &&
				(srcValue != null))
			{
				dstValue.captionId = srcValue.captionId;
				dstValue.caption   = srcValue.caption;
				dstValue.rank      = srcValue.rank;
			}
		}
		
		protected virtual void OnCaptionDefined()
		{
			if ((this.isNative) &&
				(this.caption != null))
			{
				EnumType.SetIsNative (this.caption, this.isNative);
			}
		}

		private string name;
		private bool isReadOnly;
		private bool isHidden;
		private bool isNative;
		private Support.Druid captionId;
		private Caption caption;
		private System.Enum value;
		private int rank;
	}
}
