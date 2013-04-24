$powerupAnimations = "MyModule:greenGemAnim MyModule:redGemAnim MyModule:yellowGemAnim MyModule:greyGemAnim MyModule:pinkGemAnim MyModule:blueGemAnim";

function createPowerup(%behavior, %colorIndex, %x_pos, %y_pos)
{
   // Create the sprite.
   %powerup = new Sprite()
   {
      class = Powerup;
      Animation = getWord($powerupAnimations, %colorIndex);
      BodyType = dynamic;
      Position = %x_pos SPC %y_pos;
      Size = 3;
      LinearDamping = 0.5;
      AngularDamping = 0.5;
      SceneLayer = 1;
      SceneGroup = 3;
      
      powerupBehavior = %behavior;
   };
   %powerup.createCircleCollisionShape(%powerup.Size.x * 0.4);
   %powerup.setCollisionGroups("0");
   
   return %powerup;
}

function createTeleportPowerup(%x_pos, %y_pos)
{
   return createPowerup(TeleportBehavior, 5, %x_pos, %y_pos);
}

function createSpreadShotPowerup(%x_pos, %y_pos)
{
   return createPowerup(SpreadShotBehavior, 2, %x_pos, %y_pos);
}

function createLaserBeamPowerup(%x_pos, %y_pos)
{
   return createPowerup(LaserBeamBehavior, 1, %x_pos, %y_pos);
}

function createRegenerationPowerup(%x_pos, %y_pos)
{
   return createPowerup(RegenerationBehavior, 0, %x_pos, %y_pos);
}

function createMineShotPowerup(%x_pos, %y_pos)
{
   return createPowerup(MineShotBehavior, 3, %x_pos, %y_pos);
}

function createRandomPowerup(%x_pos, %y_pos, %choices)
{
   if(getWordCount(%choices) == 0) {
      %choices = "0 1 2 3 4";   
   }
   
   %rand = getWord(%choices, getRandom(0, getWordCount(%choices)-1));
   switch(%rand)
   {
      case 0:
         %powerup = createTeleportPowerup(%x_pos, %y_pos);
      case 1:
         %powerup = createSpreadShotPowerup(%x_pos, %y_pos);
      case 2:
         %powerup = createLaserBeamPowerup(%x_pos, %y_pos);
      case 3:
         %powerup = createRegenerationPowerup(%x_pos, %y_pos);
      case 4:
         %powerup = createMineShotPowerup(%x_pos, %y_pos);
      default:
         %powerup = createRandomPowerup(%x_pos, %y_pos, "0 1 2 3 4");
   }
   return %powerup;
}

function createUtilityPowerup(%x_pos, %y_pos)
{
   createRandomPowerup(%x_pos, %y_pos,"0 3");
}

function createWeaponPowerup(%x_pos, %y_pos)
{
   createRandomPowerup(%x_pos, %y_pos,"1 2 4");
}