function createBarrier(%x_pos, %y_pos, %width, %height, %angle)
{
   // Create the sprite.
   %barrier = new Sprite()
   {
      class = Barrier;
      type = Wall;
      Image = "MyModule:barrierImage";
      BodyType = static;
      Position = %x_pos SPC %y_pos;
      Size = %width SPC %height;
      Angle = %angle;
      SceneLayer = 1;
      SceneGroup = 0;
   };
   %barrier.createPolygonBoxCollisionShape();
   
   return %barrier;
}