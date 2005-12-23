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
		
		
		public bool								IsAttached
		{
			get
			{
				return this.context != null;
			}
		}
		
		
		public void Attach(Text.TextNavigator navigator)
		{
			this.InternalDetach ();
			
			this.navigator  = navigator;
			this.context    = this.navigator.TextContext;
			this.style_list = this.context.StyleList;
			
			this.navigator.TextChanged += new Epsitec.Common.Support.EventHandler(this.HandleNavigatorTextChanged);
			this.navigator.CursorMoved += new Epsitec.Common.Support.EventHandler(this.HandleNavigatorCursorMoved);
			
			this.UpdateAll ();
			this.NotifyChanged ();
		}
		
		public void Attach(Text.TextContext context, Text.TextStyle style)
		{
			this.InternalDetach ();
			
			this.context    = context;
			this.style      = style;
			this.style_list = this.context.StyleList;
			
			//	TODO: attache au styliste
			
			this.UpdateAll ();
			this.NotifyChanged ();
		}
		
		public void Detach()
		{
			this.InternalDetach ();
			
			this.NotifyChanged ();
		}
		
		
		public void SuspendSynchronisations()
		{
			this.suspend_synchronisations++;
		}
		
		public void DefineOperationName(string name, string caption)
		{
			//	Définit un nom d'opération (name = nom interne, caption = nom qui
			//	sera attaché à l'oplet dans OpletQueue, lequel sera visible par
			//	l'utilisateur).
			
			if ((this.navigator != null) &&
				(this.navigator.TextStory != null) &&
				(this.navigator.TextStory.OpletQueue != null))
			{
				Common.Support.OpletQueue queue = this.navigator.TextStory.OpletQueue;
				
				if ((this.last_op_name != name) &&
					(name != null) &&
					(name.Length > 0))
				{
					queue.DisableMerge ();
				}
			}
			
			this.last_op_name = name;
			this.last_op_caption = caption;
		}
		
		public void ResumeSynchronisations()
		{
			System.Diagnostics.Debug.Assert (this.suspend_synchronisations > 0);
			
			this.suspend_synchronisations--;
			int id = this.change_id++;
			
			if (this.suspend_synchronisations == 0)
			{
				if (this.navigator != null)
				{
					this.navigator.SuspendNotifications ();
					
					using (this.navigator.TextStory.BeginAction (this.last_op_caption))
					{
						foreach (AbstractState state in this.states)
						{
							foreach (StateProperty property in state.GetPendingProperties ())
							{
								state.NotifyChanged (property, id);
								this.InternalSynchronize (state, property);
							}
							
							state.ClearPendingProperties ();
						}
						
						this.navigator.TextStory.ValidateAction ();
					}
					
					this.navigator.ResumeNotifications ();
				}
			}
		}
		
		
		public void UpdateAll()
		{
			this.UpdateState (false);
			this.UpdateState (true);
		}
		
		private void InternalDetach()
		{
			if (this.navigator != null)
			{
				this.navigator.CursorMoved -= new Epsitec.Common.Support.EventHandler(this.HandleNavigatorCursorMoved);
				this.navigator.TextChanged -= new Epsitec.Common.Support.EventHandler(this.HandleNavigatorTextChanged);
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
		
		
		private void HandleNavigatorTextChanged(object sender)
		{
			this.UpdateAll ();
		}

		private void HandleNavigatorCursorMoved(object sender)
		{
			this.UpdateAll ();
		}
		
		
		protected void DefineMetaProperty(string meta, int priority, params Property[] properties)
		{
			TextStyle style = this.style_list.CreateOrGetMetaProperty (meta, priority, properties);
			
			if (this.navigator != null)
			{
				if (this.navigator.IsSelectionActive)
				{
					this.navigator.EndSelection ();
				}
				
				this.navigator.SetMetaProperties (Properties.ApplyMode.Set, style);
			}
		}
		
		protected void ClearMetaProperty(string meta)
		{
			TextStyle style = this.style_list.CreateOrGetMetaProperty (meta, new Property[0]);
			
			if (this.navigator != null)
			{
				if (this.navigator.IsSelectionActive)
				{
					this.navigator.EndSelection ();
				}
				
				this.navigator.SetMetaProperties (Properties.ApplyMode.Clear, style);
			}
		}
		
		protected void ClearMetaProperty(string meta, Property template_property)
		{
			TextStyle style = this.style_list.CreateOrGetMetaProperty (meta, template_property);
			
			if (this.navigator != null)
			{
				if (this.navigator.IsSelectionActive)
				{
					this.navigator.EndSelection ();
				}
				
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
		
		protected Property[] ReadAccumulatedProperties(Properties.WellKnownType type)
		{
			Property[] properties = null;
			
			if (this.navigator != null)
			{
				properties = this.navigator.AccumulatedTextProperties;
			}
			
			if ((properties != null) &&
				(properties.Length > 0))
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				for (int i = 0; i < properties.Length; i++)
				{
					if (properties[i].WellKnownType == type)
					{
						list.Add (properties[i]);
					}
				}
				
				return (Property[]) list.ToArray (typeof (Property));
			}
			
			return null;
		}
		
		protected Property ReadUnderlyingProperty(Properties.WellKnownType type)
		{
			TextStyle[] styles = null;
			
			if (this.navigator != null)
			{
				styles = this.navigator.TextStyles;
			}
			
			if ((styles != null) &&
				(styles.Length > 0))
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				foreach (TextStyle style in styles)
				{
					if (style.TextStyleClass != TextStyleClass.MetaProperty)
					{
						list.Add (style);
					}
				}
				
				if (list.Count > 0)
				{
					Property[] properties;
					
					this.context.GetFlatProperties (list, out styles, out properties);
					this.context.AccumulateProperties (properties, out properties);
					
					for (int i = 0; i < properties.Length; i++)
					{
						if (properties[i].WellKnownType == type)
						{
							return properties[i];
						}
					}
				}
			}
			
			return null;
		}
		
		protected Property ReadMetaProperty(string meta, Properties.WellKnownType type)
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
		
		protected Property[] ReadMetaProperties(string meta, Properties.WellKnownType type)
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
						return style.FindProperties (type);
					}
				}
			}
			
			return null;
		}
		
		
		internal void Register(AbstractState state)
		{
			if (this.states == null)
			{
				this.states = new System.Collections.ArrayList (2);
			}
			
			this.states.Add (state);
		}
		
		internal void NotifyChanged()
		{
			foreach (AbstractState state in this.states)
			{
				state.NotifyWrapperChanged ();
			}
		}
		
		
		internal void Synchronize(AbstractState state, StateProperty property)
		{
			int id = this.change_id++;
			
			if (state.AccessMode == AccessMode.ReadOnly)
			{
				//	Si les réglages sont définis en lecture seule, la synchronisation
				//	ne doit peut pas affecter le texte sous-jacent :
				
				state.NotifyChanged (property, id);
			}
			else
			{
				if (this.suspend_synchronisations > 0)
				{
					state.AddPendingProperty (property);
				}
				else
				{
					state.NotifyChanged (property, id);
					
					if (this.navigator != null)
					{
						this.navigator.SuspendNotifications ();
						this.InternalSynchronize (state, property);
						this.navigator.ResumeNotifications ();
					}
				}
			}
		}
		
		
		internal abstract void InternalSynchronize(AbstractState state, StateProperty property);
		internal abstract void UpdateState(bool active);
		
		private TextContext						context;
		private TextNavigator					navigator;
		private StyleList						style_list;
		private TextStyle						style;
		private System.Collections.ArrayList	states;
		
		private string							last_op_name;
		private string							last_op_caption;
		
		private int								suspend_synchronisations;
		private int								change_id = 1;
	}
}
