function createCrate(%x_pos, %y_pos)
{
   // Create the sprite.
   %crate = new Sprite()
   {
      class = Scenery;
      isBreakable = false;
      Image = "ToyAssets:tiles";
      Frame = 4;
      BodyType = dynamic;
      Position = %x_pos SPC %y_pos;
      Size = 3;
      LinearDamping = 0.8;
      AngularDamping = 0.8;
      SceneLayer = 2;
      SceneGroup = 4;
   };
   %crate.createPolygonBoxCollisionShape();
   %crate.setCollisionGroups("0 1 2 4");
   
   return %crate;
}