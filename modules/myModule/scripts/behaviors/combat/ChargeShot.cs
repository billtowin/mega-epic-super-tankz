if (!isObject(ChargeShotBehavior))
{
   %template = new BehaviorTemplate(ChargeShotBehavior);

   %template.friendlyName = "Tank Charge Shot";
   %template.behaviorType = "Combat";
   %template.description  = "Tank Cannon Charge Shot projectile";

   %template.addBehaviorField(duration, "Duration of Powerup/Ability (negative number for infinite) (in ms)", int, -1);
   %template.addBehaviorField(damage, "Damage per shot", int, 25);

   %template.addBehaviorField(minSpeed, "Normal Speed", float, 45);
   %template.addBehaviorField(maxSpeed, "Speed when fully charged", float, 85);
   
   %template.addBehaviorField(maxChargeTime, "Charge Time (in ms)", int, 1000);
   %template.addBehaviorField(reloadTime, "Reload time (in ms)", int, 1000);
   %template.addBehaviorField(chargeShotLifespan, "LifeSpan of Shot (in ms)", int, 1500);
}

function ChargeShotBehavior::onBehaviorAdd(%this)
{
   %this.timeSpentCharging = 0;
   %this.isLoaded = true;
}

function ChargeShotBehavior::onBehaviorRemove(%this)
{
   %this.stopSounds();
}

function ChargeShotBehavior::stopSounds(%this)
{
   alxStop(%this.reloadSound);
   alxStop(%this.chargeSound);
   alxStop(%this.shotSound);
}

function ChargeShotBehavior::startCharging(%this)
{
   %this.chargeSound = alxPlay("MyModule:chargeShotChargingSound");
   %this.timeSpentCharging = 0;
   %this.chargingSchedule = %this.schedule(100, incrementCharge);
}

function ChargeShotBehavior::incrementCharge(%this)
{
   %this.timeSpentCharging = %this.timeSpentCharging + 100;
   if(%this.timeSpentCharging >= %this.maxChargeTime)
   {
      alxStop(%this.chargeSound);
   } 
   %this.chargingSchedule = %this.schedule(100, incrementCharge);
}

function ChargeShotBehavior::loadShot(%this)
{
   %this.reloadSound = alxPlay("MyModule:chargeShotReloadSound");
   %this.isLoaded = true;
}

function ChargeShotBehavior::shoot(%this)
{
   alxStop(%this.chargeSound);
   cancel(%this.chargingSchedule);
   if(%this.isLoaded)
   {      
      %shotSpeed = 0;
      if(%this.timeSpentCharging >= %this.maxChargeTime)
      {
         %this.timeSpentCharging = %this.maxChargeTime;
      }
      %speedDelta = %this.maxSpeed - %this.minSpeed;
      %speedAddition = (%speedDelta * %this.timeSpentCharging) / %this.maxChargeTime;
      %shotSpeed = %this.minSpeed + %speedAddition;
      %shotLevel =  %this.timeSpentCharging / %this.maxChargeTime; //from 0 to 1.0
      
      %this.createChargeShot(%shotSpeed, %shotLevel);
      %this.shotSound = alxPlay("MyModule:chargeShotSound");
      %this.isLoaded = false;
      %this.reloadSchedule = %this.schedule(%this.reloadTime, loadShot);
   }
   %this.timeSpentCharging = 0;
}

function ChargeShotBehavior::createChargeShot(%this, %shotSpeed, %shotLevel)
{   
   %shot = new Sprite()
   {
      class = ChargeShot;
      Image = "ToyAssets:WhiteSphere";
      BlendColor = "Black";
      BodyType = dynamic;
      Bullet = true;
      Size = 1.3;
      SceneLayer = 1;
      SceneGroup = 2;
      CollisionCallback = true;
      DefaultRestitution = 1;
      
      shotLevel = %shotLevel;
   };
   %adjustedAngle = getPositiveAngle(%this.owner); 
   %shotOffset = Vector2Direction(%adjustedAngle,%this.owner.Size.height * 0.5 + %shot.Size.height * 0.6);
   %shot.Position = (%this.owner.Position.x + %shotOffset.x) SPC (%this.owner.Position.y + %shotOffset.y);
   
   %shot.setLinearVelocityPolar(%this.owner.Angle - 180, %shotSpeed);
   %shot.createCircleCollisionShape( %shot.Size.height / 2);
   %shot.setCollisionGroups("0 1 4");
   
   %dealDmgBehavior = DealsDamageBehavior.createInstance();
   %dealDmgBehavior.strength = %this.damage;
   %shot.addBehavior(%dealDmgBehavior);
   
   %shot.dieSchedule = %shot.schedule(%this.chargeShotLifespan,onDeath);
   // Add the sprite to the scene.
   %this.owner.getScene().add( %shot );
}

function ChargeShot::onCollision(%this, %object, %details)
{
   if(%object.class $= "Scenery" && %object.isBreakable)
   {
      %object.safeDelete();
   }
   if(%object.class !$= "Wall" && %object.class !$= "Scenery")
   {
      %this.onDeath();
   }
}

function ChargeShot::onDeath(%this)
{
   %this.getScene().add(createExplosion(%this.Position.x SPC (%this.Position.y + 2), 5));
   alxPlay("MyModule:shotExplosionSound");
   %this.safeDelete();
}