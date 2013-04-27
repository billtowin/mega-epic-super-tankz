function createBasicTank(%initialFrame, %x_pos, %y_pos)
{
   // Create the sprite.
   %tank = new Sprite()
   {
      class = Tank;
      Image = "myModule:manyTanksImage";
      Frame = %initialFrame;      //Valid Frames: 0 to 7
      FlipY = true;
      FixedAngle = true;
      BodyType = dynamic;
      Position = %x_pos SPC %y_pos;
      Size = "5 6";
      LinearDamping = 2.0;
      AngularDamping = 1.0;
      SceneLayer = 1;
      SceneGroup = 1;
      CollisionCallback = true;
      
      initialX = %x_pos;
      initialY = %y_pos;
      initialFrame = %initialFrame;
      
      secondaryWeapon = "";
   };
   %tank.createPolygonBoxCollisionShape(%tank.Size.width * 0.82, %tank.Size.height * 0.85);
   %tank.setCollisionGroups("0 1 2 3");
   
   return %tank;
}

function createTank(%initialFrame, %x_pos, %y_pos)
{
   %tank = createBasicTank(%initialFrame, %x_pos, %y_pos);
   
   //Tank Movement Behavior
   %tank.addBehavior(TankMovementBehavior.createInstance());
   //Takes Damage Behavior
   %tank.addBehavior(TakesDamageBehavior.createInstance());
   //Slow On Damage Behavior
   %tank.addBehavior(SlowOnDamageBehavior.createInstance());
   //Fixed Health Bar Behavior
   %healthBar = %tank.createHealthBar();
   %healthBarBehavior = FixedHealthBarBehavior.createInstance();
   %healthBarBehavior.healthObject = %tank;
   %healthBar.addBehavior(%healthBarBehavior);
   %tank.healthBar = %healthBar;
   
   return %tank;
}

function Tank::createHealthBar(%this)
{
   // Create the sprite.
   %healthBar = new Sprite()
   {
      class = "HealthBar";
      Image = "ToyAssets:Blank";
      BlendColor = "Green";
      BodyType = dynamic;
      Position = %this.Position.x SPC %this.Position.y;
      Size = %this.Size.x SPC 1;
      SceneLayer = 0;
      SceneGroup = 31;
      
      CollisionGroups = "";
   };
   %healthBar.setBlendAlpha(0.6);
   return %healthBar;
}

function Tank::onCollision(%this, %object, %collisiondetails)
{
   if(%object.class $= "Powerup")
   {
      %object.powerupSpawner.onPickup();
      %this.onPowerupPickup(%object);
      %object.safeDelete();
   }
}

function Tank::onPowerupPickup(%this, %pickup)
{
   %this.powerupSound = alxPlay("MyModule:powerupSound");
   %temp = %pickup.powerupBehavior;
   if(%temp $= "LaserBeamBehavior" || %temp $= "TeleportBehavior" || %temp $= "SpreadShotBehavior" || %temp $= "MineShotBehavior")
   {
      if(isObject(%this.secondaryWeapon))
      {
         cancel(%this.powerdownSchedule);
         %this.secondaryWeapon.stopSounds();
         %this.removeBehavior(%this.secondaryWeapon);
         %this.secondaryWeapon = "";
      }
   }
   
   %powerupInstance = %pickup.powerupBehavior.createInstance();
   %this.addBehavior(%powerupInstance);
   if(%temp $= "LaserBeamBehavior" || %temp $= "TeleportBehavior" || %temp $= "SpreadShotBehavior"|| %temp $= "MineShotBehavior")
   {
      %this.secondaryWeapon = %powerupInstance;
   }
   if(%powerupInstance.duration > 0)
   {
      %this.powerdownSchedule = %this.schedule(%powerupInstance.duration, powerDown, %powerupInstance); 
   }
}

function Tank::powerDown(%this, %powerupInstance)
{
   if(isObject(%powerupInstance))
   {
      alxPlay("MyModule:powerdownSound");
      %this.removeBehavior(%powerupInstance);
   }
}

function Tank::onDeath(%this)
{
   %this.deathSound = alxPlay("MyModule:tankDeathSound");
   %tankControls = %this.getBehavior("TankControlsBehavior");
   if (isObject(%tankControls)) {
      %this.stopSounds();
   }
   %this.getScene().add(createExplosion(%this.Position, %this.Size.y));
   %this.healthBar.safeDelete();
   %this.safeDelete();
}