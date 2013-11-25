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
    dropZones: null,
    /* Constructor */

    constructor: function(menu) {
      var config;

      this.initStores();
      this.dropZones = [];

      var button = {};
          button.xtype = 'button';
          button.text = 'Vider l\'arche';
          button.width = 400;
          button.width = 200;
          button.cls = 'tile-button';
          button.overCls = 'tile-button-over';
          button.textAlign = 'left';
          button.handler = this.purgeEntityBag;
          button.scope = this;

      config = {
        headerPosition: 'left',
        title: 'Arche',
        cls: 'entitybag-window',
        iconCls: 'epsitec-aider-images-general-ark-icon16',
        draggable: true,
        resizable: true,
        closable: false,
        autoScroll: true,
        layout: {
            type: 'column'
        },
        dockedItems: [{
          xtype: 'toolbar',
          dock: 'top',
          items: button
        }],
        items: [{
          xtype: 'panel',
          region: 'center',
          border: false,
          layout: 'vbox',
          autoHeight: true,
          autoScroll: true
        }],
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
    registerDropZone: function (dropzone) {
      this.dropZones[dropzone.dropZoneId] = dropzone;
    },

    showRegistredDropZone: function (){
      for(d in this.dropZones)
      {
        this.dropZones[d].show();
      }       
    },

    hideRegistredDropZone: function (){
      for(d in this.dropZones)
      {
        this.dropZones[d].hide();
      }  
    },

    resizeEntityBagHandler: function () {
        this.setSizeAndPosition();   
    },

    setSizeAndPosition: function() {
      var viewport = Epsitec.Cresus.Core.app.viewport, 
          menu = Epsitec.Cresus.Core.app.menu;
      if(Ext.isDefined(viewport))
      {
        this.width = 270;
        this.height = 500;
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
          data: [],
      });
    },

    showEntity: function(entity)
    {
      var path = {};
      path.entityId = entity.id;

      alert(entity.id);
    },

    addEntityToBag: function(title,summary,entityId) {
      var hub = Epsitec.Cresus.Core.app.hubs.getHubByName('entitybag');
      Epsitec.Cresus.Core.app.viewport.setLoading();
      this.setLoading();
      hub.AddToMyBag(title,summary,entityId);
    },

    addEntityToClientBag: function(entity) {
      var index = this.bagStore.indexOfId(entity.id);
      if(index===-1)
      {
        this.bagStore.add(entity);
        var tile = this.createEntityTile(entity);
        var entityBag = this;
        this.items.items[0].add(tile);
      }
      else
      {
        this.bagStore.removeAt(index);
        this.bagStore.insert(index,entity);
      }
      
      if(!this.isVisible())
      {
        this.show();
      }

      this.setSizeAndPosition();
      Epsitec.Cresus.Core.app.viewport.setLoading(false);
      this.setLoading(false);
    },

    purgeEntityBag: function () {
      var entityBag = this;
      if(this.bagStore.count()>0)
      {
        Epsitec.Cresus.Core.app.viewport.setLoading();
        this.setLoading();
        this.bagStore.each(function(record,id){
            entityBag.removeEntityFromBag(record.data);
        });
      }
    },

    removeEntityFromBag: function(entity) {
      var record = this.bagStore.getById(entity.id);
      var hub = Epsitec.Cresus.Core.app.hubs.getHubByName('entitybag');

      hub.RemoveFromMyBag(entity.id);
    },

    removeEntityFromClientBag: function(entity) {
      var record = this.bagStore.getById(entity.id);
      var entityBag = this;
      this.bagStore.remove(record);

      Ext.Array.each(this.items.items[0].items.items, function(item) {
          if(Ext.isDefined(item))
          {
            if(item.entityId == entity.id)
            {
              item.close();
              
            }
          }         
      });

      this.setSizeAndPosition();

      if(this.bagStore.count()==0)
      {
        this.hide();
      }
      Epsitec.Cresus.Core.app.viewport.setLoading(false);
      this.setLoading(false);

    },

    createToolbar: function() {
      return Ext.create('Ext.Toolbar', {
        dock: 'top',
        items: this.createButtons()
      });
    },

    createEntityTile: function(entity) {
      return Ext.create('Ext.panel.Panel', {
          entityData : entity,
          entityBag: this,
          title: entity.entityType,
          entityId: entity.id,
          minHeight: 50,
          minWidth: 200,
          maxWidth: 400,
          border: false,
          tools: [{
                type: 'close',
                handler: function(e, t, o) { this.removeEntityFromBag(o.up().entityData); },
                scope: this
          }],
          style: {
            borderRight: '1px solid #99BCE8',
            borderBottom: '1px solid #99BCE8',
            borderLeft: '1px solid #99BCE8'
          },
          height: 'auto',
          bodyCls: 'tile',
          itemSelector: 'div.entitybag-source',
          overItemCls: 'entitybag-over',
          selectedItemClass: 'entitybag-selected',
          html: '<div class="entitybag-source">' + entity.summary + '</div>',
          listeners: {
            render: this.initializeEntityDragZone,
            destroy: this.unregEntityDragZone,
            scope: this
          }
        });
    },

    unregEntityDragZone: function (v) {
      if(v.dragZone)
      {
        v.dragZone.unreg();
      }
    },

    initializeEntityDragZone: function (v) {
        if(!Ext.isDefined(v.dragZone))
        {
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
                        entityData: v.entityData
                    });
                }
            },

  //      Provide coordinates for the proxy to slide back to on failed drag.
  //      This is the original XY coordinates of the draggable element.
            getRepairXY: function() {
                return this.dragData.repairXY;
            },

            onBeforeDrag: function () {
              v.entityBag.showRegistredDropZone();
            },

            afterInvalidDrop: function () {
              v.entityBag.hideRegistredDropZone();
            },

            afterDragDrop: function () {
              v.entityBag.hideRegistredDropZone();
            }
        });
      }
    },

    statics: {
       getUrl: function(prefix, viewMode, viewId, entityId) {
        var url = prefix + '/' + viewMode + '/' + viewId + '/' + entityId + '/list';
        return url;
      }
    }
  });
});
