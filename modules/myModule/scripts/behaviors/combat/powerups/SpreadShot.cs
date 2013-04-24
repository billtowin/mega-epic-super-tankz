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
   %shotOffset = Vector2Direction(%adjustedAngle,%this.owner.Size.height * 0.7);
   
   %angles = "0 -8 8 -16 16";
   for(%i = 0; %i < getWordCount(%angles); %i++)
   {
      %shot = new Sprite()
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
      %shot.setLinearVelocityPolar(%this.owner.Angle - 180 + getWord(%angles, %i),%this.speed);
      %shot.createCircleCollisionShape( 0.7, "-0.1 0.6" );
      %shot.setCollisionGroups("0 1 4");
      
      %dealDmgBehavior = DealsDamageBehavior.createInstance();
      %dealDmgBehavior.strength = %this.damage;
      %shot.addBehavior(%dealDmgBehavior);
      
      %shot.dieSchedule = %shot.schedule(%this.spreadShotLifespan,destroy);
      %this.owner.getScene().add( %shot );
   }
}

function SpreadShot::onCollision(%this, %object, %details)
{
   if(%object.class $= "Scenery" && %object.isBreakable)
   {
      %object.safeDelete();
   }
}

function SpreadShot::destroy(%this)
{
   %currentScene = %this.getScene();
   addExplosion(%currentScene, %this.Position.x SPC (%this.Position.y + 2), 3);
   alxPlay("MyModule:tankShotExplosionSound");
   %this.safeDelete();
}