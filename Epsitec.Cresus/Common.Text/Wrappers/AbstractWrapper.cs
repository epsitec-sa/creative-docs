//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// La classe AbstractWrapper sert de base aux "wrappers" qui simplifient
	/// l'accès aux réglages internes d'un texte ou d'un style.
	/// </summary>
	public abstract class AbstractWrapper
	{
		protected AbstractWrapper()
		{
		}
		
		
		public void Attach(Text.TextNavigator navigator)
		{
			this.Detach ();
			
			this.navigator  = navigator;
			this.context    = this.navigator.TextContext;
			this.style_list = this.context.StyleList;
			
			//	TODO: attache au navigateur
		}
		
		public void Attach(Text.TextContext context, Text.TextStyle style)
		{
			this.Detach ();
			
			this.context    = context;
			this.style      = style;
			this.style_list = this.context.StyleList;
			
			//	TODO: attache au styliste
		}
		
		public void Detach()
		{
			if (this.navigator != null)
			{
				//	TODO: détache du navigateur
			}
			if (this.style_list != null)
			{
				//	TODO: détache du styliste
			}
			
			this.navigator  = null;
			this.style      = null;
			this.style_list = null;
			this.context    = null;
		}
		
		
		protected void DefineMeta(string meta, params Property[] properties)
		{
			TextStyle style = this.style_list.CreateOrGetMetaProperty (meta, properties);
			
			if (this.navigator != null)
			{
				this.navigator.SetMetaProperties (Properties.ApplyMode.Set, style);
			}
		}
		
		protected void ClearMeta(string meta)
		{
			TextStyle style = this.style_list.CreateOrGetMetaProperty (meta, new Property[0]);
			
			if (this.navigator != null)
			{
				this.navigator.SetMetaProperties (Properties.ApplyMode.Clear, style);
			}
		}
		
		protected Property ReadProperty(Properties.WellKnownType type)
		{
			Property[] properties = null;
			
			if (this.navigator != null)
			{
				properties = this.navigator.TextProperties;
			}
			
			if ((properties != null) &&
				(properties.Length > 0))
			{
				for (int i = 0; i < properties.Length; i++)
				{
					if (properties[i].WellKnownType == type)
					{
						return properties[i];
					}
				}
			}
			
			return null;
		}
		
		protected Property ReadAccumulatedProperty(Properties.WellKnownType type)
		{
			Property[] properties = null;
			
			if (this.navigator != null)
			{
				properties = this.navigator.AccumulatedTextProperties;
			}
			
			if ((properties != null) &&
				(properties.Length > 0))
			{
				for (int i = 0; i < properties.Length; i++)
				{
					if (properties[i].WellKnownType == type)
					{
						return properties[i];
					}
				}
			}
			
			return null;
		}
		
		protected Property ReadMeta(string meta, Properties.WellKnownType type)
		{
			TextStyle[] styles = null;
			
			if (this.navigator != null)
			{
				styles = this.navigator.TextStyles;
			}
			
			if ((styles != null) &&
				(styles.Length > 0))
			{
				foreach (TextStyle style in styles)
				{
					if (style.MetaId == meta)
					{
						return style[type];
					}
				}
			}
			
			return null;
		}
		
		protected Property[] ReadMetaMultiple(string meta, Properties.WellKnownType type)
		{
			return new Property[0];
		}
		
		
		internal abstract void Synchronise(AbstractState state, StateProperty property);
		internal abstract void Update(bool active);
		
		private TextContext						context;
		private TextNavigator					navigator;
		private StyleList						style_list;
		private TextStyle						style;
	}
}
