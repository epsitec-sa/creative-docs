
Ext.require([],
  function() {
    Ext.define('Epsitec.cresus.webcore.ui.querybuilder.QueryBuilderPanel', {
      extend: 'Ext.Panel',
      alternateClassName: ['Epsitec.QueryBuilderPanel'],

      /* Properties */

      columnDefinitions: null,
      itemIdGenerator: null,
      /* Constructor */

      constructor: function(columnDefinitions) {
        this.columnDefinitions = columnDefinitions;
        this.itemIdGenerator = 1;
        var config = {
          region: 'center',
          title: 'Editeur',
          items: [this.createElement ()],
          layout: {
            type: 'vbox'
          }
        };
        this.callParent([config]);

        return this;
      },

      /* Methods */
      createElement: function (values) {
        return Ext.create(
          'Epsitec.QueryElement', {
            builder: this,
            values: values,
            columnDefinitions: this.columnDefinitions,
            isFirstElement: function (itemId) {
              return itemId === 1 ? true : false;
            },
            itemId: this.itemIdGenerator
          });
      },

      getElements: function () {
        var elements = [];
        Ext.each(this.items.items, function (item) {
          var element =  {
            op: item.getOperator(),
            condition: item.getCondition()
          };
          elements.push(element);
        });

        return elements;
      },

      closeElements: function ()
      {
        //remove all conditions
        Ext.each(this.items.items, function (item) {
          item.close();
        });
        this.itemIdGenerator = 1;
      },

      resetElements: function ()
      {
        //remove all conditions
        this.closeElements();
        this.insert (this.itemIdGenerator, this.createElement());
        this.doLayout();
      },

      loadElements: function (query) {
        this.closeElements();
        this.itemIdGenerator = 0;

        var scope = this;
        var elementCount = query.length;
        Ext.each(query, function(element){
          scope.itemIdGenerator++;
          scope.insert (scope.itemIdGenerator, scope.createElement(element));
          if(scope.itemIdGenerator < elementCount)
          {
            var panel = scope.items.items[scope.itemIdGenerator-1];
            panel.tools[0].setVisible(false);
          }
        });

        this.doLayout();
      },

      onAddElement: function(event, toolEl, panel) {
        //hide add button
        panel.tools[0].setVisible(false);
        this.itemIdGenerator++;
        this.insert (this.itemIdGenerator, this.createElement());
        this.doLayout();
      },

      onRemoveElement: function(panel, e) {
        if(this.itemIdGenerator>1)
        {
          this.itemIdGenerator--;
          this.items.items[this.itemIdGenerator-1].tools[0].setVisible(true);
          this.doLayout();
        }
      }
    });
  });
