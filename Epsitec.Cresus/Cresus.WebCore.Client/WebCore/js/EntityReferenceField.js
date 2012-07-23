Ext.define('Epsitec.cresus.webcore.EntityReferenceField', {
  extend: 'Ext.container.Container',
  alternateClassName: ['EntityReferenceField.Callback'],
  alias: 'widget.epsitec.entityreferencefield',

  /* Config */

  layout: 'column',

  /* Constructor */

  constructor: function(options)
  {
    options.columnWidth = 1;

    var combo = Ext.create('Epsitec.EntityReferenceComboBox', options);

    var button = Ext.create('Ext.Button', {
      text: '>',
      renderTo: Ext.getBody(),
      handler: function()  {
        var title = 'Cannot edit this list';
        var content = 'You cannot directly edit this list. You will need to ' +
                      'save the current changes, click the header menu to ' +
                      'edit the corresponding list, and come back to this ' +
                      'entity to edit it.';
        Ext.Msg.alert(title, content);
      },
      margin: '19 0 0 5'
    });

    this.items = this.items || [];
    this.items.push(combo);
    this.items.push(button);

    this.callParent();

    return this;
  }
});

