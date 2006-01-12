//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		public Attachment						Attachment
		{
			get
			{
				return this.attachment;
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
			
			this.attachment = Attachment.Text;
			
			this.UpdateAll ();
			this.NotifyChanged ();
		}
		
		public void Attach(Text.TextContext context, Text.TextStyle style)
		{
			this.InternalDetach ();
			
			this.context    = context;
			this.style      = style;
			this.style_list = this.context.StyleList;
			
			this.style_list.StyleAdded += new Common.Support.EventHandler (this.HandleStyleListStyleAdded);
			this.style_list.StyleRemoved += new Common.Support.EventHandler (this.HandleStyleListStyleRemoved);
			this.style_list.StyleRedefined += new Common.Support.EventHandler (this.HandleStyleListStyleRedefined);
			
			this.attachment = Attachment.Style;
			
			this.UpdateAll ();
			this.NotifyChanged ();
		}
		
		public void Detach()
		{
			this.InternalDetach ();
			
			this.attachment = Attachment.None;
			
			this.NotifyChanged ();
		}
		
		
		public void SuspendSynchronizations()
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
		
		public void ResumeSynchronizations()
		{
			System.Diagnostics.Debug.Assert (this.suspend_synchronisations > 0);
			
			this.suspend_synchronisations--;
			
			if (this.suspend_synchronisations == 0)
			{
				int id = this.change_id++;
				
				switch (this.attachment)
				{
					case Attachment.Text:
						this.SynchronizeText (id);
						break;
					
					case Attachment.Style:
						this.SynchronizeStyle (id);
						break;
				}
			}
		}
		
		
		public void UpdateAll()
		{
			this.UpdateState (false);
			this.UpdateState (true);
		}
		
		
		private void SynchronizeText(int id)
		{
			System.Diagnostics.Debug.Assert (this.navigator != null);
			System.Diagnostics.Debug.Assert (this.style_list != null);
			
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
		
		private void SynchronizeStyle(int id)
		{
			System.Diagnostics.Debug.Assert (this.style != null);
			System.Diagnostics.Debug.Assert (this.style_list != null);
			
			//	TODO: OpletQueue.BeginAction
			
			foreach (AbstractState state in this.states)
			{
				foreach (StateProperty property in state.GetPendingProperties ())
				{
					state.NotifyChanged (property, id);
					this.InternalSynchronize (state, property);
				}
				
				state.ClearPendingProperties ();
			}
			
			//	TODO: OpletQueue.ValidateAction
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
				this.style_list.StyleAdded -= new Common.Support.EventHandler (this.HandleStyleListStyleAdded);
				this.style_list.StyleRemoved -= new Common.Support.EventHandler (this.HandleStyleListStyleRemoved);
				this.style_list.StyleRedefined -= new Common.Support.EventHandler (this.HandleStyleListStyleRedefined);
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
		
		private void HandleStyleListStyleAdded(object sender)
		{
		}
		
		private void HandleStyleListStyleRemoved(object sender)
		{
		}
		
		private void HandleStyleListStyleRedefined(object sender)
		{
			this.UpdateAll ();
		}
		
		
		protected void DefineMetaProperty(string meta, int priority, params Property[] properties)
		{
			if (this.attachment == Attachment.Text)
			{
				TextStyle style = this.style_list.CreateOrGetMetaProperty (meta, priority, properties);
				
				if (this.navigator.IsSelectionActive)
				{
					this.navigator.EndSelection ();
				}
				
				this.navigator.SetMetaProperties (Properties.ApplyMode.Set, style);
			}
			else if (this.attachment == Attachment.Style)
			{
			}
		}
		
		protected void ClearMetaProperty(string meta)
		{
			if (this.attachment == Attachment.Text)
			{
				TextStyle style = this.style_list.CreateOrGetMetaProperty (meta, new Property[0]);
				
				if (this.navigator.IsSelectionActive)
				{
					this.navigator.EndSelection ();
				}
				
				this.navigator.SetMetaProperties (Properties.ApplyMode.Clear, style);
			}
			else if (this.attachment == Attachment.Style)
			{
			}
		}
		
		protected void ClearUniformMetaProperty(string meta)
		{
			if (this.attachment == Attachment.Text)
			{
				TextStyle style = this.style_list.CreateOrGetMetaProperty (meta, new Property[0]);
				
				if (this.navigator.IsSelectionActive)
				{
					this.navigator.EndSelection ();
				}
				
				this.navigator.SetMetaProperties (Properties.ApplyMode.ClearUniform, style);
			}
			else if (this.attachment == Attachment.Style)
			{
			}
		}
		
		protected Property ReadProperty(Properties.WellKnownType type)
		{
			Property[] properties = null;
			
			if (this.attachment == Attachment.Text)
			{
				properties = this.navigator.TextProperties;
			}
			else if (this.attachment == Attachment.Style)
			{
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
			
			if (this.attachment == Attachment.Text)
			{
				properties = this.navigator.AccumulatedTextProperties;
			}
			else if (this.attachment == Attachment.Style)
			{
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
			
			if (this.attachment == Attachment.Text)
			{
				properties = this.navigator.AccumulatedTextProperties;
			}
			else if (this.attachment == Attachment.Style)
			{
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
			
			if (this.attachment == Attachment.Text)
			{
				styles = this.navigator.TextStyles;
			}
			else if (this.attachment == Attachment.Style)
			{
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
			
			if (this.attachment == Attachment.Text)
			{
				styles = this.navigator.TextStyles;
			}
			else if (this.attachment == Attachment.Style)
			{
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
			
			if (this.attachment == Attachment.Text)
			{
				styles = this.navigator.TextStyles;
			}
			else if (this.attachment == Attachment.Style)
			{
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
					//	La logique de synchronisation se trouve dans ResumeSynchronizations
					//	et on ne veut pas la répliquer ici. On triche donc pour forcer une
					//	paire suspend/resume :
					
					this.SuspendSynchronizations ();
					state.AddPendingProperty (property);
					this.ResumeSynchronizations ();
				}
			}
		}
		
		
		internal abstract void InternalSynchronize(AbstractState state, StateProperty property);
		internal abstract void UpdateState(bool active);
		
		private Attachment						attachment;
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
