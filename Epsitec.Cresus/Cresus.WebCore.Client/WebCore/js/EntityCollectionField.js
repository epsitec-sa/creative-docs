Ext.define('Epsitec.cresus.webcore.EntityCollectionField', {
  extend: 'Ext.container.Container',
  alias: 'widget.epsitec.entitycollectionfield',

  /* Config */

  layout: 'column',

  /* Constructor */

  constructor: function(options) {
    options.columnWidth = 1;

    var combo = Ext.widget('fieldcontainer', options);

    var button = Ext.create('Ext.Button', {
      text: '>',
      renderTo: Ext.getBody(),
      handler: function() {
        var title = 'Cannot edit this enumeration';
        var content = 'You cannot directly edit this enumeration. You will ' +
                      'need to save the current changes, click the header ' +
                      'menu to edit the corresponding enumeration, and come ' +
                      'back to this entity to edit it.';
        Ext.Msg.alert(title, content);
      },
      margin: '0 0 0 5'
    });

    this.items = this.items || [];
    this.items.push(combo);
    this.items.push(button);

    this.callParent(arguments);

    return this;
  }
});
