//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextStyle définit un style de texte de haut niveau. Il s'agit
	/// d'une collection de propriétés.
	/// </summary>
	public sealed class TextStyle : Styles.PropertyContainer, IContentsComparer
	{
		internal TextStyle(string name, TextStyleClass text_style_class)
		{
			this.name             = name;
			this.text_style_class = text_style_class;
		}
		
		internal TextStyle(string name, TextStyleClass text_style_class, System.Collections.ICollection properties) : base (properties)
		{
			this.name             = name;
			this.text_style_class = text_style_class;
		}
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public TextStyleClass					TextStyleClass
		{
			get
			{
				return this.text_style_class;
			}
		}
		
		
		public static System.Collections.IComparer	Comparer
		{
			get
			{
				return new TextStyleComparer ();
			}
		}
		
		
		public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
		{
			//	Ignore le nom dans le calcul de la signature. C'est voulu !
			
			checksum.Update ((int) this.text_style_class);
			
			base.UpdateContentsSignature (checksum);
		}
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			foreach (Property property in this)
			{
				buffer.Append (property.GetType ().Name);
				buffer.Append (":");
				property.SerializeToText (buffer);
				buffer.Append ("\n");
			}
			
			return buffer.ToString ();
		}
		
		
		#region IContentsComparer Members
		public bool CompareEqualContents(object value)
		{
			return TextStyle.CompareEqualContents (this, value as TextStyle);
		}
		#endregion
		
		public static bool CompareEqualContents(TextStyle a, TextStyle b)
		{
			if ((a.text_style_class == b.text_style_class) &&
				(Styles.PropertyContainer.CompareEqualContents (a, b)))
			{
				//	Ignore le nom dans la comparaison du contenu. C'est voulu !
				
				//	TODO: compléter
				
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
		#region TextStyleComparer Class
		private class TextStyleComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				TextStyle px = x as TextStyle;
				TextStyle py = y as TextStyle;
				
				if (px.text_style_class < py.text_style_class)
				{
					return -1;
				}
				if (px.text_style_class > py.text_style_class)
				{
					return 1;
				}
				
				return string.Compare (px.Name, py.Name);
			}
			#endregion
		}
		#endregion
		
		private string							name;
		private TextStyleClass					text_style_class;
	}
}
