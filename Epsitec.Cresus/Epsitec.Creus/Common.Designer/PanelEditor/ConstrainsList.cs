using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.PanelEditor
{
	/// <summary>
	/// Gestion de la liste des contraintes pour PanelEditor.
	/// </summary>
	public class ConstrainsList
	{
		public ConstrainsList(Editor editor)
		{
			this.editor = editor;
			this.context = editor.Context;
		}


		public void Starting(Rectangle initialRectangle, bool isDot)
		{
			//	Début des contraintes.
			if (!this.context.ShowConstrain)
			{
				return;
			}

			this.isDot = isDot;
			this.initialRectangle = initialRectangle;
			this.isObjectLock = false;
			this.list.Clear();
			this.isStarted = true;
		}

		public void Ending()
		{
			//	Fin des contraintes.
			if (this.list.Count != 0)
			{
				this.list.Clear();
				this.editor.Invalidate();
			}

			this.isStarted = false;
		}

		public void InvertLock()
		{
			//	Vérouille ou dévérouille l'objet le plus proche.
			if (!this.isStarted)
			{
				return;
			}

			this.isObjectLock = !this.isObjectLock;

			if (!this.isObjectLock)
			{
				this.list.Clear();
				this.editor.Invalidate();
			}
		}

		public void Activate(Rectangle rect, double baseLine, params Widget[] excludes)
		{
			//	Active les contraintes pour un rectangle donné.
			if (!this.isStarted)
			{
				return;
			}

			if (!this.isObjectLock)
			{
				this.list.Clear();
				double minX, minY;
				if (!rect.IsEmpty)
				{
					this.NearestDistance(rect, this.editor.Panel, out minX, out minY, excludes);
					this.NearestObjects(rect, this.editor.Panel, minX, minY, excludes);
				}
			}

			foreach (Constrain constrain in this.list)
			{
				constrain.IsActivate = false;
			}

			List<Constrain> bestX, bestY;
			this.Best(rect, baseLine, out bestX, out bestY);

			foreach (Constrain best in bestX)
			{
				best.IsActivate = true;
			}

			foreach (Constrain best in bestY)
			{
				best.IsActivate = true;
			}
		}

		protected void NearestDistance(Rectangle rect, Widget parent, out double minX, out double minY, params Widget[] excludes)
		{
			//	Cherche la distance à l'objet le plus proche d'une position donnée.
			Point center = rect.Center;
			minX = 1000000;
			minY = 1000000;
			double distance;

			if (parent.HasChildren)
			{
				foreach (Widget obj in parent.Children)
				{
					if (!this.IsContained(excludes, obj))
					{
						Rectangle bounds = this.editor.GetObjectPreferredBounds(obj);

						distance = System.Math.Abs(bounds.Center.X-center.X);
						if (minX > distance)
						{
							minX = distance;
						}

						distance = System.Math.Abs(bounds.Center.Y-center.Y);
						if (minY > distance)
						{
							minY = distance;
						}
					}

					if (ObjectModifier.IsAbstractGroup(obj))
					{
						this.NearestDistance(rect, obj, out minX, out minY, excludes);
					}
				}
			}
		}

		protected void NearestObjects(Rectangle rect, Widget parent, double distanceX, double distanceY, params Widget[] excludes)
		{
			//	Initialise les contraintes pour tous les objets dont la distance est
			//	inférieure ou égale à une distance donnée.
			if (parent.IsEmbedded)
			{
				return;
			}

			Point center = rect.Center;
			double distance;
			Constrain constrain;

			//	Ajoute les contraintes du conteneur parent.
			Rectangle box = this.editor.RealBounds;
			if (!box.IsEmpty)
			{
				box.Deflate(this.editor.Panel.Padding);

				constrain = new Constrain(box.BottomLeft, Constrain.Type.Left, this.context.ConstrainMargin);
				constrain.IsLimit = true;
				this.Add(constrain);

				constrain = new Constrain(box.BottomRight, Constrain.Type.Right, this.context.ConstrainMargin);
				constrain.IsLimit = true;
				this.Add(constrain);

				constrain = new Constrain(box.BottomLeft, Constrain.Type.Bottom, this.context.ConstrainMargin);
				constrain.IsLimit = true;
				this.Add(constrain);

				constrain = new Constrain(box.TopLeft, Constrain.Type.Top, this.context.ConstrainMargin);
				constrain.IsLimit = true;
				this.Add(constrain);
			}

			if (!this.initialRectangle.IsEmpty)
			{
				//	Ajoute les contraintes correspondant au rectangle initial.
				if (System.Math.Abs(rect.Left-this.initialRectangle.Left) <= this.context.ConstrainMargin)
				{
					constrain = new Constrain(this.initialRectangle.BottomLeft, Constrain.Type.Left, this.context.ConstrainMargin);
					this.Add(constrain);

					constrain = new Constrain(this.initialRectangle.BottomRight, Constrain.Type.Right, this.context.ConstrainMargin);
					this.Add(constrain);
				}

				if (System.Math.Abs(rect.Bottom-this.initialRectangle.Bottom) <= this.context.ConstrainMargin)
				{
					constrain = new Constrain(this.initialRectangle.BottomLeft, Constrain.Type.Bottom, this.context.ConstrainMargin);
					this.Add(constrain);

					constrain = new Constrain(this.initialRectangle.TopLeft, Constrain.Type.Top, this.context.ConstrainMargin);
					this.Add(constrain);
				}
			}

			if (parent.HasChildren)
			{
				foreach (Widget obj in parent.Children)
				{
					if (!this.IsContained(excludes, obj) && !obj.IsEmbedded)
					{
						Rectangle bounds = this.editor.GetObjectPreferredBounds(obj);

						distance = System.Math.Abs(bounds.Center.X-center.X);
						if (distance <= distanceX)
						{
							this.Initialize(obj);
						}

						distance = System.Math.Abs(bounds.Center.Y-center.Y);
						if (distance <= distanceY)
						{
							this.Initialize(obj);
						}
					}

					if (ObjectModifier.IsAbstractGroup(obj))
					{
						this.NearestObjects(rect, obj, distanceX, distanceY, excludes);
					}
				}
			}
		}

		protected bool IsContained(Widget[] list, Widget searched)
		{
			if (list != null)
			{
				foreach (Widget obj in list)
				{
					if (obj == searched)
					{
						return true;
					}
				}
			}
			return false;
		}

		protected void Initialize(Widget obj)
		{
			//	Initialise les contraintes pour un objet.
			Rectangle bounds = this.editor.GetObjectPreferredBounds(obj);
			Constrain constrain;

			constrain = new Constrain(bounds.BottomLeft, Constrain.Type.Left, this.context.ConstrainMargin);
			this.Add(constrain);

			constrain = new Constrain(bounds.BottomLeft-this.context.ConstrainSpacing, Constrain.Type.Right, this.context.ConstrainMargin);
			this.Add(constrain);

			constrain = new Constrain(bounds.BottomRight, Constrain.Type.Right, this.context.ConstrainMargin);
			this.Add(constrain);

			constrain = new Constrain(bounds.BottomRight+this.context.ConstrainSpacing, Constrain.Type.Left, this.context.ConstrainMargin);
			this.Add(constrain);

			constrain = new Constrain(bounds.BottomLeft, Constrain.Type.Bottom, this.context.ConstrainMargin);
			this.Add(constrain);

			constrain = new Constrain(bounds.TopLeft, Constrain.Type.Top, this.context.ConstrainMargin);
			this.Add(constrain);

			if (ObjectModifier.IsAbstractGroup(obj))
			{
				constrain = new Constrain(bounds.BottomLeft-this.context.ConstrainSpacing, Constrain.Type.Top, this.context.ConstrainMargin);
				this.Add(constrain);

				constrain = new Constrain(bounds.TopLeft+this.context.ConstrainSpacing, Constrain.Type.Bottom, this.context.ConstrainMargin);
				this.Add(constrain);

				bounds.Deflate(this.context.ConstrainGroupMargins);

				constrain = new Constrain(bounds.BottomLeft, Constrain.Type.Left, this.context.ConstrainMargin);
				this.Add(constrain);

				constrain = new Constrain(bounds.BottomRight, Constrain.Type.Right, this.context.ConstrainMargin);
				this.Add(constrain);

				constrain = new Constrain(bounds.BottomLeft, Constrain.Type.Bottom, this.context.ConstrainMargin);
				this.Add(constrain);

				constrain = new Constrain(bounds.TopLeft, Constrain.Type.Top, this.context.ConstrainMargin);
				this.Add(constrain);
			}
			else
			{
				if (!this.isDot)
				{
					ObjectModifier.ObjectType type = ObjectModifier.GetObjectType(obj);
					if (type == ObjectModifier.ObjectType.Button)
					{
						constrain = new Constrain(bounds.BottomLeft, Constrain.Type.Right, this.context.ConstrainMargin);
						this.Add(constrain);

						constrain = new Constrain(bounds.BottomRight, Constrain.Type.Left, this.context.ConstrainMargin);
						this.Add(constrain);

						constrain = new Constrain(bounds.BottomLeft, Constrain.Type.Top, this.context.ConstrainMargin);
						this.Add(constrain);

						constrain = new Constrain(bounds.TopLeft, Constrain.Type.Bottom, this.context.ConstrainMargin);
						this.Add(constrain);
					}

					Point baseLine = bounds.BottomLeft;
					baseLine.Y += this.editor.GetObjectBaseLine(obj);
					constrain = new Constrain(baseLine, Constrain.Type.BaseLine, this.context.ConstrainMargin);
					this.Add(constrain);

					baseLine.Y += this.context.Leading;
					constrain = new Constrain(baseLine, Constrain.Type.BaseLine, this.context.ConstrainMargin);
					this.Add(constrain);

					baseLine.Y -= this.context.Leading*2;
					constrain = new Constrain(baseLine, Constrain.Type.BaseLine, this.context.ConstrainMargin);
					this.Add(constrain);
				}
			}

			this.editor.Invalidate();
		}

		protected void Add(Constrain toAdd)
		{
			//	Ajoute une contrainte dans une liste, si elle n'y est pas déjà.
			foreach (Constrain constrain in this.list)
			{
				if (constrain.IsEqualTo(toAdd))
				{
					return;
				}
			}

			this.list.Add(toAdd);
		}

		public Rectangle Snap(Rectangle rect, double baseLine)
		{
			//	Adapte un rectangle en fonction de l'ensemble des contraintes.
			if (this.isStarted)
			{
				List<Constrain> bestX, bestY;
				this.Best(rect, baseLine, out bestX, out bestY);

				if (bestX.Count > 0)
				{
					rect = bestX[0].Snap(rect, baseLine);
				}

				if (bestY.Count > 0)
				{
					rect = bestY[0].Snap(rect, baseLine);
				}
			}

			return rect;
		}

		public Point Snap(Point pos)
		{
			//	Adapte une position en fonction de l'ensemble des contraintes.
			if (this.isStarted)
			{
				List<Constrain> bestX, bestY;
				this.Best(new Rectangle(pos, pos), 0, out bestX, out bestY);

				if (bestX.Count > 0)
				{
					pos = bestX[0].Snap(pos);
				}

				if (bestY.Count > 0)
				{
					pos = bestY[0].Snap(pos);
				}
			}

			return pos;
		}

		protected void Best(Rectangle rect, double baseLine, out List<Constrain> bestListX, out List<Constrain> bestListY)
		{
			//	Cherches les contraintes les plus pertinentes parmi l'ensemble des contraintes.
			double adjust;
			double minX = 1000000;
			double minY = 1000000;

			foreach (Constrain constrain in this.list)
			{
				if (constrain.AdjustX(rect, out adjust))
				{
					adjust = System.Math.Abs(adjust);
					if (minX > adjust)
					{
						minX = adjust;
					}
				}

				if (constrain.AdjustY(rect, baseLine, out adjust))
				{
					adjust = System.Math.Abs(adjust);
					if (minY > adjust)
					{
						minY = adjust;
					}
				}
			}

			bestListX = new List<Constrain>();
			bestListY = new List<Constrain>();

			foreach (Constrain constrain in this.list)
			{
				if (constrain.AdjustX(rect, out adjust))
				{
					if (System.Math.Abs(adjust) <= minX)
					{
						bestListX.Add(constrain);
					}
				}

				if (constrain.AdjustY(rect, baseLine, out adjust))
				{
					if (System.Math.Abs(adjust) <= minY)
					{
						bestListY.Add(constrain);
					}
				}
			}
		}

		public void Draw(Graphics graphics, Rectangle box)
		{
			//	Dessine toutes les contraintes.
			if (this.isStarted)
			{
				foreach (Constrain constrain in this.list)
				{
					constrain.Draw(graphics, box);
				}
			}
		}


		protected Editor					editor;
		protected PanelsContext				context;
		protected bool						isStarted;
		protected bool						isObjectLock;
		protected bool						isDot;
		protected Rectangle					initialRectangle;
		protected List<Constrain>			list = new List<Constrain>();
	}
}
