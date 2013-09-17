Ext.define('Bag', {
        extend: 'Ext.data.Model',
        idProperty: 'id',
        fields: [{
            name: 'summary'
        }, {
            name: 'entityType'
        }, {
            name: 'data'
        }]
    });

Ext.require([
  'Epsitec.cresus.webcore.ui.DropZone'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.EntityBag', {
    extend: 'Ext.Window',
    alternateClassName: ['Epsitec.EntityBag'],

    /* Properties */
    bagStore: null,
    removeFromBagDropZone: null,
    /* Constructor */

    constructor: function(menu) {
      var config;

      this.initStores();

      this.removeFromBagDropZone = Ext.create('Epsitec.DropZone', 'Enlever du panier', this.removeEntityFromBag,this);
      this.removeFromBagDropZone.hide();
      config = {
        headerPosition: 'left',
        title: 'Panier',
        draggable: false,
        resizable: false,
        closable: false,
        margins: '0 5 5 5',
        layout: {
            type: 'vbox',       
            align: 'stretch',    
            padding: 5
        },
        items: [this.createEntityView(),this.removeFromBagDropZone],
        listeners: {
          beforerender: this.setSizeAndPosition,
          score: this
        } 
      };
      menu.on("resize", this.resizeEntityBagHandler, this);
      this.callParent([config]);


      return this;
    },

    /* Methods */
    resizeEntityBagHandler: function () {
        this.setSizeAndPosition();   
    },

    setSizeAndPosition: function() {
      var viewport = Epsitec.Cresus.Core.app.viewport, 
          menu = Epsitec.Cresus.Core.app.menu;
      if(Ext.isDefined(viewport))
      {
        this.width = 200;
        this.height = 150;
        this.x = viewport.width - this.width;
        this.y = menu.el.lastBox.height;
        if(this.isVisible())
        {
          this.setPosition(this.x,this.y);
        }
      } 
    },

    initStores: function(){
      this.bagStore = Ext.create('Ext.data.Store', {
          model: 'Bag',
          data: []
      });
    },

    addEntityToBag: function(entity) {
      this.bagStore.add(entity);
      if(!this.isVisible())
      {
        this.show();
      }
    },

    removeEntityFromBag: function(entity) {
      var record = this.bagStore.getById(entity.id);
      this.bagStore.remove(record);
    },

    createEntityView: function() {
      return Ext.create('Ext.view.View', {
        cls: 'entity-view',
        tpl: '<tpl for=".">' +
                '<div class="entitybag-source">' +
                    '<tr><span class="entitybag-label">{entityType}</span>{summary}' +
                '</div>' +
             '</tpl>',
        itemSelector: 'div.entitybag-source',
        overItemCls: 'entitybag-over',
        selectedItemClass: 'entitybag-selected',
        singleSelect: true,
        store: this.bagStore,
        listeners: {
            render: this.initializeEntityDragZone,
            itemmouseenter: this.removeFromBagDropZone.show,
            itemmouseleave: this.removeFromBagDropZone.hide,
            scope: this.removeFromBagDropZone
        }
    });      
    },

    initializeEntityDragZone: function (v) {
      v.dragZone = Ext.create('Ext.dd.DragZone', v.getEl(), {

//      On receipt of a mousedown event, see if it is within a draggable element.
//      Return a drag data object if so. The data object can contain arbitrary application
//      data, but it should also contain a DOM element in the ddel property to provide
//      a proxy to drag.
          getDragData: function(e) {
              var sourceEl = e.getTarget(v.itemSelector, 10), d;

              if (sourceEl) {
                  d = sourceEl.cloneNode(true);
                  d.id = Ext.id();
                  return (v.dragData = {
                      sourceEl: sourceEl,
                      repairXY: Ext.fly(sourceEl).getXY(),
                      ddel: d,
                      entityData: v.getRecord(sourceEl).data
                  });
              }
          },

//      Provide coordinates for the proxy to slide back to on failed drag.
//      This is the original XY coordinates of the draggable element.
          getRepairXY: function() {
              return this.dragData.repairXY;
          }
      });
    }
  });
});
