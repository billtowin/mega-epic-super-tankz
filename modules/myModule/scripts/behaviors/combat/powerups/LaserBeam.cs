if (!isObject(LaserBeamBehavior))
{
   %template = new BehaviorTemplate(LaserBeamBehavior);

   %template.friendlyName = "Tank Laser Shot";
   %template.behaviorType = "Combat";
   %template.description  = "Tank Laser Shot projectile";

   %template.addBehaviorField(duration, "Duration of Powerup/Ability (negative number for infinite) (in ms)", int, 15000);
   %template.addBehaviorField(damage, "Damage per laser beam", int, 5);

   %template.addBehaviorField(beamWidth, "Width of Laser beam", float, 0.8);
   %template.addBehaviorField(beamLength, "Range/Length of laser beam", float, 22);
   
   %template.addBehaviorField(reloadTime, "Reload time (in ms)", int, 2500);
   %template.addBehaviorField(maxBeamTime, "Max Beam Time", int, 600);
   %template.addBehaviorField(beamRefreshTime, "Time between beam refreshs (for positioning)", int, 50);
}

function LaserBeamBehavior::onBehaviorAdd(%this)
{
   %this.timeSpentBeaming = 0;
   %this.isLoaded = true;
}

function LaserBeamBehavior::onBehaviorRemove(%this)
{
   %this.stopSounds();
}

function LaserBeamBehavior::beam(%this)
{
   if(%this.isLoaded)
   {
      %this.isLoaded = false;
      %this.beamSound = alxPlay("MyModule:laserBeamSound");
      %this.beamingSchedule = %this.schedule(%this.beamRefreshTime, incrementBeamTime);
      %this.beamSchedule = %this.schedule(%this.beamRefreshTime, createNextBeam);
   }
}

function LaserBeamBehavior::incrementBeamTime(%this)
{
   %this.timeSpentBeaming = %this.timeSpentBeaming + %this.beamRefreshTime;
   %this.beamingSchedule = %this.schedule(%this.beamRefreshTime, incrementBeamTime);
}

function LaserBeamBehavior::loadBeam(%this)
{
   %this.reloadSound = alxPlay("MyModule:chargeShotReloadSound");
   %this.isLoaded = true;
}

function LaserBeamBehavior::createNextBeam(%this)
{
   if(%this.timeSpentBeaming <= %this.maxBeamTime)
   {
      %this.createBeam();
      %this.beamSchedule = %this.schedule(%this.beamRefreshTime, createNextBeam);
   } else {
      cancel(%this.beamingSchedule);
      cancel(%this.beamSchedule);
      %this.stopSounds();
      %this.timeSpentBeaming = 0;
      %this.reloadSchedule = %this.schedule(%this.reloadTime, loadBeam);
   }
}

function LaserBeamBehavior::stopSounds(%this)
{
   alxStop(%this.reloadSound);
   alxStop(%this.beamSound);
}

function LaserBeamBehavior::createBeam(%this)
{
   %adjustedAngle = getPositiveAngle(%this.owner);
   
   //Calculate a direction from an Angle and Magnitude
   %beamOffset= Vector2Direction(%adjustedAngle,%this.owner.Size.height / 2 + %this.beamLength / 1.9);
   
   // Create the sprite.
   %beam = new Sprite()
   {
      class = LaserBeam;
      Image = "ToyAssets:Blank";
      Angle = %this.owner.Angle;
      BodyType = static;
      BlendColor = "Red";
      Size = %this.beamWidth SPC %this.beamLength;
      SceneLayer = 2;
      SceneGroup = 2;
      Position = (%this.owner.Position.x + %beamOffset.x) SPC (%this.owner.Position.y + %beamOffset.y);
      CollisionCallback = true;
   };
   //Sets the collision shape to a circle
   %beam.createPolygonBoxCollisionShape();
   %beam.setCollisionGroups("0 1 2 5");
   
   %dealDmgBehavior = DealsDamageBehavior.createInstance();
   %dealDmgBehavior.strength = %this.damage;
   %dealDmgBehavior.killOnHit = true;
   %beam.addBehavior(%dealDmgBehavior);
   
   %beam.dieSchedule = %beam.schedule(%this.beamRefreshTime,safeDelete);
   // Add the sprite to the scene.
   %this.owner.getScene().add( %beam );
}

function LaserBeam::onCollision(%this, %object, %details)
{
   if(%object.class $= "Scenery" && %object.isBreakable)
   {
      %object.safeDelete();
   }
   if(%object.class $= "MineShot" || %object.class $= "SpreadShot" || %object.class $= "ChargeShot") {
      %object.onDeath();  
   }
}