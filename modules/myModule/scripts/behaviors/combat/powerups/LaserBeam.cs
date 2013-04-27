if (!isObject(LaserBeamBehavior))
{
   %template = new BehaviorTemplate(LaserBeamBehavior);

   %template.friendlyName = "Tank Laser Shot";
   %template.behaviorType = "Combat";
   %template.description  = "Tank Laser Shot projectile";

   %template.addBehaviorField(duration, "Duration of Powerup/Ability (negative number for infinite) (in ms)", int, 15000);
   %template.addBehaviorField(damage, "Damage per tick/refresh", int, 3);

   %template.addBehaviorField(beamWidth, "Width of Laser beam", float, 0.8);
   %template.addBehaviorField(beamLength, "Range/Length of laser beam", float, 22);
   
   %template.addBehaviorField(reloadTime, "Reload time (in ms)", int, 2500);
   %template.addBehaviorField(maxBeamTime, "Max Beam Time", int, 600);
   %template.addBehaviorField(beamRefreshTime, "Time between beam ticks/refreshes for damage and positioning", int, 40);
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
      %this.beamSchedule = %this.schedule(%this.beamRefreshTime, updateBeamPosition);
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

function LaserBeamBehavior::updateBeamPosition(%this)
{
   if(!isObject(%this.currentBeam)) {
      %this.createBeam();
   }
   if(%this.timeSpentBeaming <= %this.maxBeamTime)
   {
      %this.currentBeam.Position = %this.owner.getWorldPoint( 0 SPC (%this.owner.Size.height / 2 + %this.beamLength / 1.9));
      %this.currentBeam.Angle = %this.owner.Angle;
      %this.beamSchedule = %this.schedule(%this.beamRefreshTime, updateBeamPosition);
   } else {
      cancel(%this.beamingSchedule);
      cancel(%this.beamSchedule);
      cancel(%this.currentBeam.collisionCheckSchedule);
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
      Position = %this.owner.getWorldPoint( 0 SPC (%this.owner.Size.height / 2 + %this.beamLength / 1.6));
   };
   
   %beam.damage = %this.damage;
   %beam.beamRefreshTime = %this.beamRefreshTime;
   %beam.dieSchedule = %beam.schedule(%this.maxBeamTime,safeDelete);
   %beam.collisionCheckSchedule = %beam.schedule(%this.beamRefreshTime, checkForCollisionsManually);
   
   %this.owner.getScene().add( %beam );
   %this.currentBeam = %beam;
}

function LaserBeam::checkForCollisionsManually(%this)
{
   %startP = %this.getWorldPoint(0 SPC (- %this.Size.height / 2));
   %endP = %this.getWorldPoint(0 SPC %this.Size.height / 2);
   %rayPicked = %this.getScene().pickRayCollision(%startP, %endP, -1,-1);
   for(%i = 0 ; %i < getWordCount(%rayPicked); %i++)
   {
      %obj = getWord(%rayPicked, %i);
      if(isObject(%obj)) 
      {
         %takesDamage = %obj.getBehavior("TakesDamageBehavior");
         if (isObject(%takesDamage)) {
            %takesDamage.takeDamage(%this.damage);
         }
         if(%obj.class $= "MineShot" || %obj.class $= "SpreadShot" || %obj.class $= "ChargeShot") {
            %obj.onDeath();  
         }
         if(%obj.class $= "Scenery" && %obj.isBreakable) {
            %obj.safeDelete();
         }
      }
   }
   %this.collisionCheckSchedule = %this.schedule(%this.beamRefreshTime, checkForCollisionsManually);
}