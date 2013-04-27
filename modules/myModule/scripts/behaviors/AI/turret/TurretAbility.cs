if (!isObject(TurretAbilityBehavior))
{
   %template = new BehaviorTemplate(TurretAbilityBehavior);

   %template.friendlyName = "AI Ability 'Control'";
   %template.behaviorType = "AI";
   %template.description  = "AI Ability";
}

function TurretAbilityBehavior::onBehaviorAdd(%this)
{
}

function TurretAbilityBehavior::onBehaviorRemove(%this)
{
}

function TurretAbilityBehavior::startAbility(%this)
{
   %chargeShot = %this.owner.getBehavior("ChargeShotBehavior");
   if (isObject(%chargeShot)) {
      %chargeShot.startCharging();
   }
}

function TurretAbilityBehavior::endAbility(%this)
{
   %chargeShot = %this.owner.getBehavior("ChargeShotBehavior");
   if (isObject(%chargeShot)) {
      %chargeShot.shoot();
   }
   
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
}