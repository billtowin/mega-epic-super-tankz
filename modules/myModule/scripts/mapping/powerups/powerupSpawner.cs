function createPowerupSpawner(%x_pos, %y_pos, %choices)
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
   %powerupSpawner = PowerupSpawnerBehavior.createInstance();
   if(getWordCount(%choices) != 0) {
      %powerupSpawner.choices = %choices;
   }
   %spawn.addBehavior(%powerupSpawner);
   
   return %spawn;
}