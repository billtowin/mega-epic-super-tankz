if (!isObject(TeleportBehavior))
{
   %template = new BehaviorTemplate(TeleportBehavior);

   %template.friendlyName = "Tank Teleport Ability";
   %template.behaviorType = "Combat";
   %template.description  = "Tank teleport ability";

   %template.addBehaviorField(duration, "Duration of Powerup/Ability (negative number for infinite) (in ms)", int, 15000);

   %template.addBehaviorField(teleportDistance, "Teleport Distance", int, 15);
   
   %template.addBehaviorField(reloadTime, "Reload time (in ms)", int, 2000);
}

function TeleportBehavior::onBehaviorAdd(%this)
{
   %this.isLoaded = true;
}

function TeleportBehavior::onBehaviorRemove(%this)
{
   %this.stopSounds();
}

function TeleportBehavior::stopSounds(%this)
{
   alxStop(%this.reloadSound);
   alxStop(%this.teleportSound);
}

function TeleportBehavior::loadTeleport(%this)
{
   %this.reloadSound = alxPlay("MyModule:teleportReloadSound");
   %this.isLoaded = true;
}

function TeleportBehavior::teleport(%this)
{
   if(%this.isLoaded)
   {
      if(%this.owner.AngularVelocity > 0) {
         %this.owner.Position = %this.owner.getWorldPoint((-%this.teleportDistance) SPC 0);
      } else if (%this.owner.AngularVelocity < 0) {
         %this.owner.Position = %this.owner.getWorldPoint( %this.teleportDistance SPC 0);      
      } else {
         %this.owner.Position = %this.owner.getWorldPoint(0 SPC %this.teleportDistance);      
      }
      
      %this.teleportSound = alxPlay("MyModule:teleportSound");
      %this.isLoaded = false;
      %this.reloadSchedule = %this.schedule(%this.reloadTime, loadTeleport);
   }
}