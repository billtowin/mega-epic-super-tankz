function createBarrel(%x_pos, %y_pos)
{
   // Create the sprite.
   %barrel = new Sprite()
   {
      class = Barrel;
      type = Scenery;
      isBreakable = true;
      Image = "MyModule:woodenBarrelImage";
      BodyType = dynamic;
      Position = %x_pos SPC %y_pos;
      Size = 3;
      LinearDamping = 0.8;
      AngularDamping = 0.8;
      SceneLayer = 2;
      SceneGroup = 4;
   };
   %barrel.createCircleCollisionShape( %barrel.Size.x / 2.4);
   %barrel.setCollisionGroups("0 1 2 4");
   
   return %barrel;
}
