function createMenuItems(%x_pos, %y_pos, %offset)
{
   %p1Controls = new Sprite()
   {
      Image = "MyModule:player1ControlsImage";
      BodyType = static;
      Position = %x_pos - %offset SPC %y_pos - %offset * 1.1;
      Size = "45 10";
      SceneLayer = 1;
      SceneGroup = 1;
   };
   
   %p2Controls = new Sprite()
   {
      Image = "MyModule:player2ControlsImage";
      BodyType = static;
      Position = %x_pos + %offset SPC %y_pos - %offset * 1.1;
      Size = "45 10";
      SceneLayer = 1;
      SceneGroup = 1;
   };
   
   %changeColor = new Sprite()
   {
      Image = "MyModule:changeColorImage";
      BodyType = static;
      Position = %x_pos SPC %y_pos - %offset * 0.6;
      Size = "40 10";
      SceneLayer = 1;
      SceneGroup = 1;
   };
   
   %pressEnter = new Sprite()
   {
      Image = "MyModule:pressEnterImage";
      BodyType = static;
      Position = %x_pos + %offset SPC %y_pos - %offset * 1.6;
      Size = "45 10";
      SceneLayer = 1;
      SceneGroup = 1;
   };
   
   %pressEscape = new Sprite()
   {
      Image = "MyModule:pressEscapeImage";
      BodyType = static;
      Position = %x_pos - %offset SPC %y_pos - %offset * 1.6;
      Size = "45 10";
      SceneLayer = 1;
      SceneGroup = 1;
   };
   
   return (%p1Controls SPC %changeColor SPC %p2Controls SPC %pressEnter SPC %pressEscape);
}