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
		
		public EnumValue(System.Enum value, int rank, bool hidden, string name)
		{
			this.DefineName (name);
			this.DefineValue (value);
			this.DefineRank (rank);
			this.DefineHidden (hidden);
		}

		public EnumValue(System.Enum value, int rank, bool hidden, Support.Druid captionId)
		{
			this.DefineCaptionId (captionId);
			this.DefineValue (value);
			this.DefineRank (rank);
			this.DefineHidden (hidden);
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

		public class SerializationConverter : Types.ISerializationConverter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, Types.IContextResolver context)
			{
				EnumValue enumValue = (EnumValue) value;

				string rank      = enumValue.Rank.ToString (System.Globalization.CultureInfo.InvariantCulture);
				string captionId = enumValue.CaptionId.ToString ();

				return string.Concat (rank, " ", captionId);
			}

			public object ConvertFromString(string value, Types.IContextResolver context)
			{
				string[]      args  = value.Split (' ');

				System.Diagnostics.Debug.Assert (args.Length == 2);
				
				int           rank  = System.Int32.Parse (args[0], System.Globalization.CultureInfo.InvariantCulture);
				Support.Druid druid = Support.Druid.Parse (args[1]);
				
				EnumValue enumValue = new EnumValue (druid);

				enumValue.rank = rank;
				
				return enumValue;
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
		
		protected virtual void OnCaptionDefined()
		{
		}

		private string name;
		private bool isReadOnly;
		private bool isHidden;
		private Support.Druid captionId;
		private Caption caption;
		private System.Enum value;
		private int rank;

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
	}
}
