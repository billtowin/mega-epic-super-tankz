if (!isObject(SniperShotBehavior))
{
   %template = new BehaviorTemplate(SniperShotBehavior);

   %template.friendlyName = "Tank Charge Shot";
   %template.behaviorType = "Combat";
   %template.description  = "Tank Cannon Charge Shot projectile";

   %template.addBehaviorField(duration, "Duration of Powerup/Ability (negative number for infinite) (in ms)", int, 15000);
   %template.addBehaviorField(baseDamage, "Base Damage per shot", int, 5);
   
   %template.addBehaviorField(damageModifier, "Damage added per timeDelta", int, 9);
   %template.addBehaviorField(timeDelta, "timeDelta (ms)", int, 50);
   
   %template.addBehaviorField(speed, "Normal Speed", float, 100);
   
   %template.addBehaviorField(reloadTime, "Reload time (in ms)", int, 3000);
}

function SniperShotBehavior::onBehaviorAdd(%this)
{
   %this.isLoaded = true;
}

function SniperShotBehavior::onBehaviorRemove(%this)
{
   %this.stopSounds();
}

function SniperShotBehavior::stopSounds(%this)
{
   alxStop(%this.reloadSound);
   alxStop(%this.shotSound);
}

function SniperShotBehavior::loadShot(%this)
{
   %this.reloadSound = alxPlay("MyModule:chargeShotReloadSound");
   %this.isLoaded = true;
}

function SniperShotBehavior::snipe(%this)
{
   if(%this.isLoaded)
   {
      %this.createSniperShot();
      %this.shotSound = alxPlay("MyModule:chargeShotSound");
      %this.isLoaded = false;
      %this.reloadSchedule = %this.schedule(%this.reloadTime, loadShot);
   }
}

function SniperShotBehavior::createSniperShot(%this)
{   
   %shot = new Sprite()
   {
      class = SniperShot;
      Image = "ToyAssets:BlankCircle";
      BlendColor = "Black";
      BodyType = dynamic;
      Bullet = true;
      Size = 0.8;
      SceneLayer = 1;
      SceneGroup = 2;
      CollisionCallback = true;
      
      sender = %this;
   };
   %shot.Position = %this.owner.getWorldPoint(0 SPC %this.owner.Size.height * 0.5 + %shot.Size.height * 0.6);
   
   %shot.setLinearVelocityPolar(%this.owner.Angle - 180, %this.speed);
   %shot.createCircleCollisionShape( %shot.Size.height / 2);
   %shot.setCollisionGroups("0 1 4");
   
   %dealDmgBehavior = DealsDamageBehavior.createInstance();
   %dealDmgBehavior.strength = %this.baseDamage;
   %shot.addBehavior(%dealDmgBehavior);
   
   %shot.addDamageSchedule = %shot.schedule(%this.timeDelta, addDamage);
   // Add the sprite to the scene.
   %this.owner.getScene().add( %shot );
}

function SniperShot::addDamage(%this)
{
   %dealsDamage = %this.getBehavior("DealsDamageBehavior");
   if (isObject(%dealsDamage)) {
      %dealsDamage.strength += %this.sender.damageModifier;
   }
   %this.addDamageSchedule = %this.schedule(%this.sender.timeDelta, addDamage);
}

function SniperShot::onCollision(%this, %object, %details)
{
   cancel(%this.addDamageSchedule);
   if(%object.type $= "Scenery" && %object.isBreakable) {
      %object.safeDelete();
   }
   %this.onDeath();
}

function SniperShot::onDeath(%this)
{
   %this.getScene().add(createExplosion(%this.Position.x SPC (%this.Position.y + 2), 5));
   alxPlay("MyModule:shotExplosionSound");
   %this.safeDelete();
}