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
    removedStore: null,
    /* Constructor */

    constructor: function(menu) {
      var config;

      this.initStores();

      var removeFromBagDropZone = Ext.create('Epsitec.DropZone', 'enlever du panier', this.removeEntityFromBag);

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
        items: [removeFromBagDropZone,this.createEntityView()],
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
        this.height = viewport.height;
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
      this.removedStore = Ext.create('Ext.data.Store', {
          model: 'Bag',
          data: [{
          id: 1,
          summary: "---",
          entityType: "---",
          data: "---"
        }]
      });
    },

    addEntityToBag: function(entity) {
      this.bagStore.add(entity);
    },

    removeEntityFromBag: function(entity) {
      this.store.remove(entity);
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
            render: this.initializeEntityDragZone
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
    },

    initializeEntityDropZone: function(v) {
      

        v.dropZone = Ext.create('Ext.dd.DropZone', v.el, {

//      If the mouse is over a target node, return that node. This is
//      provided as the "target" parameter in all "onNodeXXXX" node event handling functions
        getTargetFromEvent: function(e) {
            return e.getTarget('.entitybag-target');
        },

//      On entry into a target node, highlight that node.
        onNodeEnter : function(target, dd, e, data){
            Ext.fly(target).addCls('entitybag-target-hover');
        },

//      On exit from a target node, unhighlight that node.
        onNodeOut : function(target, dd, e, data){
            Ext.fly(target).removeCls('entitybag-target-hover');
        },

//      While over a target node, return the default drop allowed class which
//      places a "tick" icon into the drag proxy.
        onNodeOver : function(target, dd, e, data){
            return Ext.dd.DropZone.prototype.dropAllowed;
        },

        onNodeDrop : function(target, dd, e, data){  
          var targetEl = Ext.get(target),
              html = targetEl.dom.innerHTML;

            if (html == 'Drop Entity Here') {
                html = data.entityData.summary
            } else {
                html = data.entityData.summary + ', ' + targetEl.dom.innerHTML;
            }

            targetEl.update(html);
            Ext.Msg.alert('Drop gesture', 'Dropped entity ' + data.entityData.summary);
            return true;
        }
      });
    }
  });
});
