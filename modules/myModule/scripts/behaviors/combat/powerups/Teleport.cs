if (!isObject(TeleportBehavior))
{
   %template = new BehaviorTemplate(TeleportBehavior);

   %template.friendlyName = "Tank Teleport Ability";
   %template.behaviorType = "Combat";
   %template.description  = "Tank teleport ability";

   %template.addBehaviorField(powerKey, "Key to bind to Teleport", keybind, "keyboard k");
   %template.addBehaviorField(duration, "Duration of Powerup/Ability (negative number for infinite) (in ms)", int, 15000);

   %template.addBehaviorField(teleportDistance, "Teleport Distance", int, 12);
   
   %template.addBehaviorField(reloadTime, "Reload time (in ms)", int, 2500);
}

function TeleportBehavior::onBehaviorAdd(%this)
{
   if (!isObject(GlobalActionMap))
      return;
   GlobalActionMap.bindObj(getWord(%this.powerKey, 0), getWord(%this.powerKey, 1), "teleport", %this);
   
   %this.isLoaded = true;
}

function TeleportBehavior::onBehaviorRemove(%this)
{
   if (!isObject(GlobalActionMap))
      return;
   %this.stopSounds();
   GlobalActionMap.unbind(getWord(%this.powerKey, 0), getWord(%this.powerKey, 1));
}

function TeleportBehavior::stopSounds(%this)
{
   alxStop(%this.reloadSound);
   alxStop(%this.teleportSound);
}

function TeleportBehavior::loadTeleport(%this)
{
   %this.reloadSound = alxPlay("MyModule:tankTeleportReloadSound");
   %this.isLoaded = true;
}

function TeleportBehavior::teleport(%this)
{
   if(%this.isLoaded)
   {
      %adjustedAngle = getPositiveAngle(%this.owner);
      %teleportOffset = Vector2Direction(%adjustedAngle, %this.teleportDistance);
      %this.owner.Position = %this.owner.Position.x + %teleportOffset.x SPC %this.owner.Position.y + %teleportOffset.y;
      
      %this.teleportSound = alxPlay("MyModule:tankTeleportSound");
      %this.isLoaded = false;
      %this.reloadSchedule = %this.schedule(%this.reloadTime, loadTeleport);
   }
}