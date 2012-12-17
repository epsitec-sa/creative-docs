Ext.require([
  'Epsitec.cresus.webcore.Texts',
  'Epsitec.cresus.webcore.Tile',
  'Epsitec.cresus.webcore.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.GroupedSummaryTile', {
    extend: 'Epsitec.cresus.webcore.Tile',
    alternateClassName: ['Epsitec.GroupedSummaryTile'],
    alias: 'widget.epsitec.groupedsummarytile',

    /* Config */

    bodyCls: 'grouped-summary-tile',

    /* Properties */

    subViewMode: '2',
    subViewId: 'null',
    selectedItem: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions, items;

      newOptions = {
        tools: this.createCollectionTools(options)
      };

      // Here we clone options because later on we'll modify some of its child
      // elements and we don't want these modification to be made on the given
      // config object.
      Ext.applyIf(newOptions, Ext.clone(options));

      items = newOptions.items;

      if (items.length > 0)
      {
        Ext.Array.forEach(items,
            function(item) {
              item.groupedSummaryTile = this;
              item.isLast = false;
            },
            this
        );
        items[items.length - 1].isLast = true;
      }
      else {
        newOptions.html = Epsitec.Texts.getEmptySummaryText();
        newOptions.bodyPadding = 5;
      }
      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createCollectionTools: function(options) {
      var tools = Ext.Array.clone(options.tools || []);

      if (!options.hideAddButton) {
        tools.push({
          type: 'plus',
          tooltip: Epsitec.Texts.getAddTip(),
          handler: this.addEntityHandler,
          scope: this
        });
      }

      if (!options.hideRemoveButton) {
        tools.push({
          type: 'minus',
          tooltip: Epsitec.Texts.getRemoveTip(),
          handler: this.deleteEntityHandler,
          scope: this
        });
      }

      return tools;
    },

    addEntityHandler: function() {
      this.setLoading();
      Ext.Ajax.request({
        url: 'proxy/list/create',
        method: 'POST',
        params: {
          parentEntityId: this.column.entityId,
          propertyAccessorId: this.propertyAccessorId
        },
        callback: this.addEntityCallback,
        scope: this
      });
    },

    addEntityCallback: function(options, success, response) {
      var json, entityId;

      this.setLoading(false);

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      entityId = json.content.key;
      this.addEntityColumn(entityId, true);
    },

    deleteEntityHandler: function() {
      if (this.selectedItem === null) {
        return;
      }

      this.setLoading();
      Ext.Ajax.request({
        url: 'proxy/list/delete',
        method: 'POST',
        params: {
          parentEntityId: this.column.entityId,
          deletedEntityId: this.selectedItem.entityId,
          propertyAccessorId: this.propertyAccessorId
        },
        callback: this.deleteEntityCallback,
        scope: this
      });
    },

    deleteEntityCallback: function(options, success, response) {
      var json;

      this.setLoading(false);

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      this.column.removeToRight();
      this.column.refreshToLeft(true);
    },

    handleItemClick: function(item) {
      this.selectItem(item);
      this.addEntityColumn(item.entityId, false);
    },

    addEntityColumn: function(entityId, refresh) {
      this.column.selectTile(this);
      this.column.addEntityColumn(
          this.subViewMode, this.subViewId, entityId, refresh
      );
    },

    select: function(selected) {
      if (this.isSelected() !== selected) {
        this.setSelected(selected);
        if (!selected) {
          this.selectItem(null);
        }
      }
    },

    selectItem: function(item) {
      if (this.selectedItem !== null) {
        this.selectedItem.select(false);
      }

      this.selectedItem = item;

      if (this.selectedItem !== null) {
        this.selectedItem.select(true);
      }
    },

    getState: function() {
      var entityId, itemIndex;

      if (this.selectedItem !== null) {
        entityId = this.selectedItem.entityId;
        itemIndex = this.items.items.indexOf(this.selectedItem);
      }
      else {
        entityId = null;
        itemIndex = null;
      }

      return {
        type: 'groupedSummaryTile',
        propertyAccessorId: this.propertyAccessorId,
        entityId: entityId,
        itemIndex: itemIndex
      };
    },

    setState: function(state) {
      var item;
      if (state.itemIndex !== null && state.entityId !== null)
      {
        item = this.getItemWithEntityId(state.itemIndex, state.entityId);
        this.selectItem(item);
      }
    },

    getItemForState: function(itemIndex, entityId) {

      // Looks for an item that has the good entityId. Let i be the index of the
      // selected item in the previous tile, then the items are examined in the
      // following order: i, i-1, i+1, i-2, i+2, ... until it finds an item
      // with the good entity id or all items have been examined.

      var up, down, items, item;

      up = itemIndex;
      down = itemIndex - 1;
      items = this.items.items;

      while (down >= 0 || up < items.length) {
        if (up < items.length) {
          item = items[up];
          if (true) {
            return item;
          }
        }
        if (down >= 0) {
          item = items[down];
          if (true) {
            return items;
          }
        }
        up += 1;
        down -= 1;
      }

      return null;
    },

    isStateApplicable: function(state) {
      return state.type === 'groupedSummaryTile' &&
          state.propertyAccessorId === this.propertyAccessorId;
    }
  });
});
