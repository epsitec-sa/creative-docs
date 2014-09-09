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
    currentPage: 0,
    pageSize: 5,
    /* Constructor */

    constructor: function(menu) {
      var config, purgeButton, navButtons;

      this.initStores();
      this.dropZones = [];

      purgeButton = {
          xtype    : 'button',
          text     : 'Vider le panier',
          width    : 200,
          cls      : 'tile-button',
          overCls  : 'tile-button-over',
          textAlign: 'left',
          handler  : this.purgeEntityBag,
          scope    : this
      };
      navButtons = [{
          xtype    : 'button',
          text     : '<-',
          width    : 60,
          cls      : 'tile-button',
          overCls  : 'tile-button-over',
          textAlign: 'left',
          handler  : function () {
            Epsitec.Cresus.Core.app.viewport.setLoading();
            this.setLoading();
            this.changePage (true);
            Epsitec.Cresus.Core.app.viewport.setLoading(false);
            this.setLoading(false);
          },
          scope    : this
      }, {
          xtype: 'button',
          text: '->',
          width: 60,
          cls: 'tile-button',
          overCls: 'tile-button-over',
          textAlign: 'left',
          handler: function () {
            Epsitec.Cresus.Core.app.viewport.setLoading();
            this.setLoading();
            this.changePage (false);
            Epsitec.Cresus.Core.app.viewport.setLoading(false);
            this.setLoading(false);
          },
          scope: this
      }];

      config = {
        headerPosition: 'left',
        title: 'Panier',
        cls: 'entitybag-window',
        iconCls: 'epsitec-aider-images-general-bag-icon16',
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
          items: purgeButton
        },
        {
          xtype: 'toolbar',
          dock: 'top',
          items: navButtons
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
      for(var d in this.dropZones)
      {
        this.dropZones[d].show();
      }
    },

    hideRegistredDropZone: function (){
      for(var d in this.dropZones)
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

    addEntityToClientBag: function (entity) {
      var count = this.bagStore.count();
      var index = this.bagStore.indexOfId(entity.id);


      if(index===-1)
      {
        this.bagStore.add(entity);
        if (count < this.pageSize || this.checkForJob(entity.id)) {
         var tile = this.createEntityTile(entity);
         this.items.items[0].add(tile);
        }
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

      this.setTitle('Panier (' +this.bagStore.count() + ')');
      this.setSizeAndPosition();
      Epsitec.Cresus.Core.app.viewport.setLoading(false);
      this.setLoading(false);
    },

    changePage: function (reverse) {

      var count = this.bagStore.count();
      var page  = this.currentPage * this.pageSize;
      var tilesToClose = [];

      if(reverse && this.currentPage > 0) {
        this.currentPage--;
      }

      if(!reverse && page + this.pageSize <= count)
      {
        this.currentPage++;
      }

      //extract tiles to close
      var scope = this;
      Ext.Array.each(this.items.items[0].items.items, function (item) {
          if (Ext.isDefined(item)) {
            if (!scope.checkForJob(item.entityId)) {
              tilesToClose.push(item);
            }
          }
      });
      //close all tiles
      Ext.Array.each(tilesToClose, function (tile) {
        tile.close();
      });

      //add new tiles for page
      for(var i=page;i<page+this.pageSize;i++)
      {
        if(this.bagStore.data.items[i] !== undefined) {
          var entity = this.bagStore.data.items[i].raw;
          if(!this.checkForJob(entity.id)) {
            var tile = this.createEntityTile(entity);
            this.items.items[0].add(tile);
          }
        }
      }
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
      this.setTitle('Panier');
    },

    removeEntityFromBag: function(entity) {
      var record = this.bagStore.getById(entity.id);
      var hub = Epsitec.Cresus.Core.app.hubs.getHubByName('entitybag');

      if (this.checkForJob (entity.id)) {
        Epsitec.Cresus.Core.app.deleteJobAndFile(entity.id);
      }

      hub.RemoveFromMyBag(entity.id);
    },

    removeEntityFromClientBag: function(entity) {
      var record = this.bagStore.getById(entity.id);
      var entityBag = this;

      if (record !== null) {
          this.bagStore.remove(record);
      }

      var count = this.bagStore.count();
      this.setTitle('Panier (' + count + ')');
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

      if(this.bagStore.count()===0)
      {
        Epsitec.Cresus.Core.app.viewport.setLoading(false);
        this.setLoading(false);
        this.hide();
      }


    },

    checkForJob: function (entityId) {
      if (entityId.substring(0, 3) === "JOB") {
        return true;
      }
      else {
        return false;
      }
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
          html: '<div class="entitybag-source">' + Ext.util.Format.htmlDecode(entity.summary) + '</div>',
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
