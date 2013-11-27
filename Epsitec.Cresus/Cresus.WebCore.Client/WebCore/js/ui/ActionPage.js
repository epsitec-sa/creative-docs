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
  Ext.define('Epsitec.cresus.webcore.ui.ActionPage', {
    extend: 'Ext.panel.Panel',
    alternateClassName: ['Epsitec.ActionPage'],

    /* Properties */
    actionStores: null,
    /* Constructor */

    constructor: function(menu) {
      var config;

      this.actionStores = [];
      
      
      this.registerActionStore('target');

      config = {
        title: 'Gestion de la cible',
        iconCls: 'epsitec-cresus-core-images-data-specialcontroller-icon16',
        bodyCls: 'tile',
        margins: '0 5 5 5',
        layout: 'column',
        items: [this.createTargetEntityToolContainer(),this.createTargetEntityContainer()],
        listeners: {
          score: this
        } 
      };

      this.callParent([config]);
      
      return this;
    },

    /* Methods */
    createEntityView: function(store) {
      return Ext.create('Ext.view.View', {
        cls: 'entitybag-view',
        tpl: '<tpl for=".">' +
                '<div class="entitybag-source">' +
                    '<div class="entitybag-label">{entityType}</div>{summary}' +
                '</div>' +
             '</tpl>',
        itemSelector: 'div.entitybag-source',
        selectedItemClass: 'entitybag-selected',
        singleSelect: true,
        entityBag: this,
        store: this.actionStores[store],
        listeners: {
            scope: this
        }
    });      
    },

    addTargetEntity: function(entity) {
      var store = this.actionStores['target'];
      var index = 0;
      store.removeAt(index);
      store.insert(index,entity);    

      var targetToolPanel = this.items.items[0];
      targetToolPanel.items.add(this.addDefaultEntityActionButtons());
      targetToolPanel.items.add(this.addEntityActionButtons());

      targetToolPanel.doLayout();

    },

    createTargetEntityToolContainer: function ()
    {
      return Ext.create('Ext.panel.Panel', {
          flex : 20 / 100,
          title: 'Outils Cible',
          height: 150,
          bodyCls: 'tile',
          layout: 'hbox',
          items: [this.addDefaultEntityActionButtons()]
        });
    },

    createTargetEntityContainer: function ()
    {
      return Ext.create('Ext.panel.Panel', {
          columnWidth: 80 / 100,
          title: 'Cible',
          height: 150,
          bodyCls: 'tile',
          items: [this.createEntityView('target')]
        });
    },

    registerActionStore: function (action) {
      this.actionStores[action] = Ext.create('Ext.data.Store', {
          model: 'Bag',
          data: [],
        });
    },

    getEntityStoreKey: function(entity)
    {
      if(Ext.isDefined(entity.id))
      {
        return entity.id.split('-')[0];
      }
      else
        return 'undefinedEntity';
    },

    createEntityContainer: function(entity)
    {
      var storeKey = this.getEntityStoreKey(entity);
      if(Ext.isDefined(this.actionStores[storeKey]))
      {
        return null;
      }
      else
      {
        this.registerActionStore(storeKey);
        return Ext.create('Ext.panel.Panel', {
          columnWidth: 1/5,
          title: entity.entityType.split(' ')[0],
          height: 'auto',
          bodyCls: 'tile',
          items: [this.createEntityView(storeKey)]
        });
      }      
    },

    addDefaultEntityActionButtons: function () {
      var button = {};
          button.xtype = 'button';
          button.text = 'Remplir avec le panier';
          button.width = 400;
          button.width = 200;
          button.cls = 'tile-button';
          button.overCls = 'tile-button-over';
          button.textAlign = 'left';
          button.handler = this.loadEntityBagContent;
          button.scope = this;
      return button;
    },

    addEntityActionButtons: function() {
      var store = this.actionStores['target'];
      var target = store.getAt(0);
      var storeKey = this.getEntityStoreKey(target.data);

      if(storeKey=='[LVOA03]')
      {
        var button = {};
          button.xtype = 'button';
          button.text = 'Ajouter au publipostage';
          button.width = 400;
          button.width = 200;
          button.cls = 'tile-button';
          button.overCls = 'tile-button-over';
          button.textAlign = 'left';
          button.handler = function () {
                            this.createMailing(target);
                          };
          button.scope = this;


      }

      return button;
    },

    loadEntityBagContent: function () {
      var bag = Epsitec.Cresus.Core.app.entityBag.bagStore;
      var me = this;
      bag.each(function(record,id){

          console.info(record);
          me.add(me.createEntityContainer(record.data));
          me.addEntityToStore(record.data);
          me.doLayout();
      });
    },

    addEntityToStore: function(entity)
    {
      var storeKey = this.getEntityStoreKey(entity);
      var store = this.actionStores[storeKey];
      var index = store.indexOfId(entity.id);
      if(index===-1)
      {
        store.add(entity);
      }
      else
      {
        store.removeAt(index);
        store.insert(index,entity);
      }
    },

    createMailing: function(mailing) {
      this.setLoading();
      Ext.Ajax.request({
        url: Epsitec.EntityBag.getUrl(
            'proxy/entity/action/entity', 6, 0, mailing.id
        ),
        method: 'POST',
        params: {
          entityIds: this.actionStores['[LVOA03]'].data.items.map(function(e) { return e.internalId; }).join(';')
        },
        callback: function ()
        {
          this.setLoading(false);
        },
        scope: this
      });
    },

    createGroupActionPanel: function()
    {
      return Ext.create('Ext.panel.Panel', {
          columnWidth: 1/5,
          title: 'Compléter un groupe',
          height: 500,
          bodyCls: 'tile',
        });
    },

    createHouseholdActionPanel: function()
    {
      return Ext.create('Ext.panel.Panel', {
          columnWidth: 1/5,
          title: 'Compléter un ménage',
          height: 500,
          bodyCls: 'tile',
        });
    },

    createContactsMergeActionPanel: function()
    {
      return Ext.create('Ext.panel.Panel', {
          columnWidth: 1/5,
          title: 'Fusionner des contacts',
          height: 500,
          bodyCls: 'tile',
        });
    }

  });
});
