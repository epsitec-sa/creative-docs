using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Cette classe contient toutes les informations relatives à une ligne, c'est-à-dire à un champ.
	/// </summary>
	public class Field
	{
		public enum RouteType
		{
			// Cas A:
			// o----->
			// 
			// Cas Bt:
			//      ^
			//      |
			// o----|
			// 
			// Cas Bb:
			// o----|
			//      |
			//      V
			// 
			// Cas C:
			// o----|
			//      x
			//      |---->
			// 
			// Cas D:
			// o----|
			//      x
			//   <--|
			// 
			// Les cas A et B ont un routage toujours automatique.
			// Les cas C et D ont une poignée 'x' pour personnaliser le routage.

			Close,		// connection fermée
			Himself,	// connection sur soi-même
			A,			// connection de type A
			Bt,			// connection de type B vers le haut
			Bb,			// connection de type B vers le bas
			C,			// connection de type C
			D,			// connection de type C
		}

		
		public Field(Editor editor)
		{
			this.editor = editor;

			this.textLayoutField = new TextLayout();
			this.textLayoutField.DefaultFontSize = 10;
			this.textLayoutField.Alignment = ContentAlignment.MiddleLeft;
			this.textLayoutField.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.textLayoutType = new TextLayout();
			this.textLayoutType.DefaultFontSize = 10;
			this.textLayoutType.Alignment = ContentAlignment.MiddleLeft;
			this.textLayoutType.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.relation = FieldRelation.None;
			this.destination = Druid.Empty;
			this.rank = -1;
			this.isExplored = false;
			this.isSourceExpanded = false;
			this.routeType = RouteType.Close;
			this.middleRelativeC = 0.5;
			this.positionAbsoluteD = 0.0;
		}

		public string FieldName
		{
			//	Nom du champ.
			get
			{
				return this.textLayoutField.Text;
			}
			set
			{
				this.textLayoutField.Text = value;
			}
		}

		public string TypeName
		{
			//	Nom du type.
			get
			{
				return this.textLayoutType.Text;
			}
			set
			{
				this.textLayoutType.Text = value;
			}
		}

		public TextLayout TextLayoutField
		{
			//	Retourne le TextLayout utilisé pour le nom du champ.
			get
			{
				return this.textLayoutField;
			}
		}

		public TextLayout TextLayoutType
		{
			//	Retourne le TextLayout utilisé pour le type du champ.
			get
			{
				return this.textLayoutType;
			}
		}

		public FieldRelation Relation
		{
			//	Type de la relation éventuelle du champ.
			get
			{
				return this.relation;
			}
			set
			{
				this.relation = value;
			}
		}

		public Druid Destination
		{
			//	Destination de la relation éventuelle du champ.
			get
			{
				return this.destination;
			}
			set
			{
				this.destination = value;
			}
		}

		public int Rank
		{
			//	Rang du champ dans le tableau (numéro de la ligne, 0..n)
			get
			{
				return this.rank;
			}
			set
			{
				this.rank = value;
			}
		}

		public ObjectBox SrcBox
		{
			//	Objet source de la connection
			get
			{
				return this.srcBox;
			}
			set
			{
				this.srcBox = value;
			}
		}

		public ObjectBox DstBox
		{
			//	Objet destination de la connection.
			get
			{
				return this.dstBox;
			}
			set
			{
				this.dstBox = value;
			}
		}

		public ObjectConnection Connection
		{
			//	Objet connection.
			get
			{
				return this.connection;
			}
			set
			{
				this.connection = value;
			}
		}

		public bool IsExplored
		{
			//	Indique si une relation est explorée, c'est-à-dire si l'entité destination est visible.
			get
			{
				return this.isExplored;
			}
			set
			{
				this.isExplored = value;
			}
		}

		public bool IsSourceExpanded
		{
			//	Indique si la boîte source d'une relation est étendue.
			get
			{
				return this.isSourceExpanded;
			}
			set
			{
				this.isSourceExpanded = value;
			}
		}

		public bool IsAttachToRight
		{
			//	Indique si la boîte source est attachée à droite ou à gauche.
			get
			{
				return this.isAttachToRight;
			}
			set
			{
				this.isAttachToRight = value;
			}
		}


		public void RouteClear()
		{
			//	Force un routage standard pour la connection.
			this.Route = RouteType.Close;
		}

		public RouteType Route
		{
			//	Type de routage de la connection.
			get
			{
				return this.routeType;
			}
			set
			{
				if (this.routeType != value)
				{
					this.routeType = value;
					this.connection.UpdateRoute();
				}
			}
		}

		public double MiddleRelativeC
		{
			//	Position intermédiaire de la connection (cas C).
			//	0.5 correspond au milieu (valeur par défaut).
			get
			{
				return this.middleRelativeC;
			}
			set
			{
				value = System.Math.Max(value, 0.1);
				value = System.Math.Min(value, 0.9);

				if (this.middleRelativeC != value)
				{
					this.middleRelativeC = value;

					this.connection.UpdateRoute();
					this.editor.Invalidate();
				}
			}
		}

		public double PositionAbsoluteD
		{
			//	Position intermédiaire de la connection (cas D).
			//	0.0 correspond à la boucle la plus serrée (valeur par défaut).
			get
			{
				return this.positionAbsoluteD;
			}
			set
			{
				value = System.Math.Max(value, 0.0);

				if (this.positionAbsoluteD != value)
				{
					this.positionAbsoluteD = value;

					this.connection.UpdateRoute();
					this.editor.Invalidate();
				}
			}
		}

		
		protected Editor editor;
		protected TextLayout textLayoutField;
		protected TextLayout textLayoutType;
		protected FieldRelation relation;
		protected Druid destination;
		protected int rank;
		protected ObjectBox srcBox;
		protected ObjectBox dstBox;
		protected ObjectConnection connection;
		protected bool isExplored;
		protected bool isSourceExpanded;
		protected bool isAttachToRight;
		protected RouteType routeType;
		protected double middleRelativeC;
		protected double positionAbsoluteD;
	}
}
