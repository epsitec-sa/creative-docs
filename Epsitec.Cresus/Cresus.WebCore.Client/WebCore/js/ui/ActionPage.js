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
      
      this.registerActionStore('mailing');

      config = {
        title: 'Outils Actions Aider',
        iconCls: 'epsitec-cresus-core-images-data-specialcontroller-icon16',
        bodyCls: 'tile',
        margins: '0 5 5 5',
        layout: 'column',
        items: [this.createMailingActionPanel(),this.createGroupActionPanel(),this.createHouseholdActionPanel(),this.createContactsMergeActionPanel()],
        listeners: {
          score: this
        } 
      };

      this.callParent([config]);

      this.initMailingActionPanel();
      
      return this;
    },

    /* Methods */
    
    registerActionStore: function (action) {
      this.actionStores[action] = Ext.create('Ext.data.Store', {
          model: 'Bag',
          data: [],
        });
    },

    initMailingActionPanel : function()
    {
      var addMailingDropZone = Ext.create('Epsitec.DropZone', 'addmailingzoneid','Ajouter un publipostage', this.addMailing, this);
      Epsitec.Cresus.Core.app.entityBag.registerDropZone(addMailingDropZone);
    },

    createMailingActionPanel: function()
    {
      return Ext.create('Ext.panel.Panel', {
          columnWidth: 1/5,
          title: 'Création de publipostage',
          height: 500,
          bodyCls: 'tile',
          items: [this.createMailingEntityView(),this.createMailingActionButton()]
        });
    },
    createMailingActionButton: function () {
      var button = {};
          button.xtype = 'button';
          button.text = 'Créer le publipostage';
          button.width = 400;
          button.cls = 'tile-button';
          button.overCls = 'tile-button-over';
          button.textAlign = 'left';
          button.handler = this.createMailing;
          button.scope = this;
      return button;
    },

    createMailingEntityView: function() {
      return Ext.create('Ext.view.View', {
        cls: 'entitybag-view',
        tpl: '<tpl for=".">' +
                '<div class="entitybag-source">' +
                    '<tr><span class="entitybag-label">{entityType}</span>{summary}' +
                '</div>' +
             '</tpl>',
        itemSelector: 'div.entitybag-source',
        overItemCls: 'entitybag-over',
        selectedItemClass: 'entitybag-selected',
        singleSelect: true,
        entityBag: this,
        store: this.actionStores['mailing'],
        listeners: {
            scope: this
        }
    });      
    },

    addMailing: function(mailing) {
      var store = this.actionStores['mailing'];
      var index = store.indexOfId(mailing.id);
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
          entityIds: this.bagStore.data.items.map(function(e) { return e.internalId; }).join(';')
        },
        callback: function ()
        {
          this.setLoading(false);
          Epsitec.Cresus.Core.app.reloadCurrentTile(null);
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
