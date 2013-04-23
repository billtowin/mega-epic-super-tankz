if (!isObject(SpreadShotBehavior))
{
   %template = new BehaviorTemplate(SpreadShotBehavior);

   %template.friendlyName = "Tank Spread Shot";
   %template.behaviorType = "Combat";
   %template.description  = "Tank Shotgun/Spread Shot ability";

   %template.addBehaviorField(powerKey, "Key to bind to Shoot", keybind, "keyboard j");
   %template.addBehaviorField(duration, "Duration of Powerup/Ability (negative number for infinite) (in ms)", int, 15000);
   %template.addBehaviorField(damage, "Damage per shot", int, 12);

   %template.addBehaviorField(speed, "Projectile Speed", float, 70);
   
   %template.addBehaviorField(reloadTime, "Reload time (in ms)", int, 1500);
   %template.addBehaviorField(spreadShotLifespan, "LifeSpan of Shot (in ms)", int, 400);
}

function SpreadShotBehavior::onBehaviorAdd(%this)
{
   if (!isObject(GlobalActionMap))
      return;
   GlobalActionMap.bindObj(getWord(%this.powerKey, 0), getWord(%this.powerKey, 1), "spreadShot", %this);
   
   %this.isLoaded = true;
}

function SpreadShotBehavior::onBehaviorRemove(%this)
{
   if (!isObject(GlobalActionMap))
      return;
   
   %this.stopSounds();
   GlobalActionMap.unbind(getWord(%this.powerKey, 0), getWord(%this.powerKey, 1));
}

function SpreadShotBehavior::stopSounds(%this)
{
   alxStop(%this.reloadSound);
   alxStop(%this.shotSound);
}

function SpreadShotBehavior::loadShot(%this)
{
   %this.reloadSound = alxPlay("MyModule:tankSpreadShotReloadSound");
   %this.isLoaded = true;
}

function SpreadShotBehavior::spreadShot(%this)
{
   if(%this.isLoaded)
   {
      %this.createSpreadShot();
      %this.shotSound = alxPlay("MyModule:tankSpreadShotSound");
      %this.isLoaded = false;
      %this.reloadSchedule = %this.schedule(%this.reloadTime, loadShot);
   }
}

function SpreadShotBehavior::createSpreadShot(%this)
{
   %adjustedAngle = getPositiveAngle(%this.owner);
   
   //Calculate a direction from an Angle and Magnitude
   %shotOffset= Vector2Direction(%adjustedAngle,%this.owner.Size.height * 0.7);
   
   // Create the sprite.
   %shot1 = new Sprite()
   {
      class = SpreadShot;
      Animation = "ToyAssets:Cannonball_Projectile_3Animation";
      BodyType = dynamic;
      Bullet = true;
      Position = (%this.owner.Position.x + %shotOffset.x) SPC (%this.owner.Position.y + %shotOffset.y);
      Size = 2;
      SceneLayer = 1;
      SceneGroup = 2;
      CollisionCallback = true;
   };
   
   %shot2 = new Sprite()
   {
      class = SpreadShot;
      Animation = "ToyAssets:Cannonball_Projectile_3Animation";
      BodyType = dynamic;
      Bullet = true;
      Position = (%this.owner.Position.x + %shotOffset.x) SPC (%this.owner.Position.y + %shotOffset.y);
      Size = 2;
      SceneLayer = 1;
      SceneGroup = 2;
      CollisionCallback = true;
   };
   
   %shot3 = new Sprite()
   {
      class = SpreadShot;
      Animation = "ToyAssets:Cannonball_Projectile_3Animation";
      BodyType = dynamic;
      Bullet = true;
      Position = (%this.owner.Position.x + %shotOffset.x) SPC (%this.owner.Position.y + %shotOffset.y);
      Size = 2;
      SceneLayer = 1;
      SceneGroup = 2;
      CollisionCallback = true;
   };
   
   %shot4 = new Sprite()
   {
      class = SpreadShot;
      Animation = "ToyAssets:Cannonball_Projectile_3Animation";
      BodyType = dynamic;
      Bullet = true;
      Position = (%this.owner.Position.x + %shotOffset.x) SPC (%this.owner.Position.y + %shotOffset.y);
      Size = 2;
      SceneLayer = 1;
      SceneGroup = 2;
      CollisionCallback = true;
   };
   
   %shot5 = new Sprite()
   {
      class = SpreadShot;
      Animation = "ToyAssets:Cannonball_Projectile_3Animation";
      BodyType = dynamic;
      Bullet = true;
      Position = (%this.owner.Position.x + %shotOffset.x) SPC (%this.owner.Position.y + %shotOffset.y);
      Size = 2;
      SceneLayer = 1;
      SceneGroup = 2;
      CollisionCallback = true;
   };
   %shot1.setLinearVelocityPolar(%this.owner.Angle - 180,%this.speed);
   %shot2.setLinearVelocityPolar(%this.owner.Angle - 180 + 8,%this.speed);
   %shot3.setLinearVelocityPolar(%this.owner.Angle - 180 - 8,%this.speed);
   %shot4.setLinearVelocityPolar(%this.owner.Angle - 180 + 16,%this.speed);
   %shot5.setLinearVelocityPolar(%this.owner.Angle - 180 - 16,%this.speed);

   //Sets the collision shape to a circle
   %shot1.createCircleCollisionShape( 0.7, "-0.1 0.6" );
   %shot1.setCollisionGroups("0 1 4");
   %shot2.createCircleCollisionShape( 0.7, "-0.1 0.6" );
   %shot2.setCollisionGroups("0 1 4");
   %shot3.createCircleCollisionShape( 0.7, "-0.1 0.6" );
   %shot3.setCollisionGroups("0 1 4");
   %shot4.createCircleCollisionShape( 0.7, "-0.1 0.6" );
   %shot4.setCollisionGroups("0 1 4");
   %shot5.createCircleCollisionShape( 0.7, "-0.1 0.6" );
   %shot5.setCollisionGroups("0 1 4");
   
   %dealDmgBehavior1 = DealsDamageBehavior.createInstance();
   %dealDmgBehavior1.strength = %this.damage;
   %shot1.addBehavior(%dealDmgBehavior1);
   
   %dealDmgBehavior2 = DealsDamageBehavior.createInstance();
   %dealDmgBehavior2.strength = %this.damage;
   %shot2.addBehavior(%dealDmgBehavior2);
   
   %dealDmgBehavior3 = DealsDamageBehavior.createInstance();
   %dealDmgBehavior3.strength = %this.damage;
   %shot3.addBehavior(%dealDmgBehavior3);
   
   %dealDmgBehavior4 = DealsDamageBehavior.createInstance();
   %dealDmgBehavior4.strength = %this.damage;
   %shot4.addBehavior(%dealDmgBehavior4);
   
   %dealDmgBehavior5 = DealsDamageBehavior.createInstance();
   %dealDmgBehavior5.strength = %this.damage;
   %shot5.addBehavior(%dealDmgBehavior5);
   
   %shot1.dieSchedule = %shot1.schedule(%this.spreadShotLifespan,destroy);
   %shot2.dieSchedule = %shot2.schedule(%this.spreadShotLifespan,destroy);
   %shot3.dieSchedule = %shot3.schedule(%this.spreadShotLifespan,destroy);
   %shot4.dieSchedule = %shot4.schedule(%this.spreadShotLifespan,destroy);
   %shot5.dieSchedule = %shot5.schedule(%this.spreadShotLifespan,destroy);
   
   // Add the sprite to the scene.
   %ownerScene = %this.owner.getScene();   
   %ownerScene.add( %shot1 );
   %ownerScene.add( %shot2 );
   %ownerScene.add( %shot3 );
   %ownerScene.add( %shot4 );
   %ownerScene.add( %shot5 );
}

function SpreadShot::onCollision(%this, %object, %details)
{
   if(%object.class $= "Scenery" && %object.isBreakable)
   {
      %object.safeDelete();
   }
   
   //FIXES WEIRD BUG, when ChargeShot collides with SpreadShot
   if(%object.class !$= "ChargeShot")
   {
      %this.destroy();
   }
}

function SpreadShot::destroy(%this)
{
   %currentScene = %this.getScene();
   addExplosion(%currentScene, %this.Position.x SPC (%this.Position.y + 2), 3);
   alxPlay("MyModule:tankShotExplosionSound");
   %this.safeDelete();
}