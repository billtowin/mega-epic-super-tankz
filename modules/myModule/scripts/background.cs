function createBackground()
{
   // Create the sprite.
   %background = new Sprite()
   {
      class = Background;
      type = Wall;
      Image = "MyModule:desertBackgroundImage";
      BodyType = static;
      Position = "0 0";
      Size = 100;
      SceneLayer = 31;
      SceneGroup = 0;
      DefaultRestitution = 1;
   };
   // Create border collisions.
   %background.createEdgeCollisionShape( -50, -50, -50, 50 );
   %background.createEdgeCollisionShape( 50, -50, 50, 50 );
   %background.createEdgeCollisionShape( -50, 50, 50, 50 );
   %background.createEdgeCollisionShape( -50, -50, 50, -50 );
   
   return %background;
}