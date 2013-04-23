function createPowerupSpawner(%x_pos, %y_pos)
{
   // Create the sprite.
   %spawn = new Sprite()
   {
      class = Spawner;
      BodyType = static;
      Position = %x_pos SPC %y_pos;
      Size = 3;
      SceneLayer = 2;
      SceneGroup = 31;
   };
   %spawn.addBehavior(PowerupSpawnerBehavior.createInstance());
   
   return %spawn;
}