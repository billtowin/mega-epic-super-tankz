if (!isObject(SecondaryAbilityBehavior))
{
   %template = new BehaviorTemplate(SecondaryAbilityBehavior);

   %template.friendlyName = "Tank Secondary Ability";
   %template.behaviorType = "Controls";
   %template.description  = "Tank Secondary Ability";

   %template.addBehaviorField(key, "Key to bind to secondary weapon fire", keybind, "keyboard x");
}

function SecondaryAbilityBehavior::onBehaviorAdd(%this)
{
   if (!isObject(GlobalActionMap))
      return;

   %id = %this.getId();
   GlobalActionMap.bindCmd(getWord(%this.key, 0), getWord(%this.key, 1), %id @ ".startAbility();", %id @ ".endAbility();");
}

function SecondaryAbilityBehavior::onBehaviorRemove(%this)
{
    if (!isObject(GlobalActionMap))
       return;

    GlobalActionMap.unbind(getWord(%this.key, 0), getWord(%this.key, 1));
}

function SecondaryAbilityBehavior::startAbility(%this)
{
}

function SecondaryAbilityBehavior::endAbility(%this)
{
   %teleport = %this.owner.getBehavior("TeleportBehavior");
   if (isObject(%teleport)) {
      %teleport.teleport();
   }
   
   %spreadShot = %this.owner.getBehavior("SpreadShotBehavior");
   if (isObject(%spreadShot)) {
      %spreadShot.spreadShot();
   }
   
   %mineShot = %this.owner.getBehavior("MineShotBehavior");
   if (isObject(%mineShot)) {
      %mineShot.mineShot();
   }
      
   %laserBeam = %this.owner.getBehavior("LaserBeamBehavior");
   if (isObject(%laserBeam)) {
      %laserBeam.beam();
   }
   
   %sniperShot = %this.owner.getBehavior("SniperShotBehavior");
   if (isObject(%sniperShot)) {
      %sniperShot.snipe();
   }
}