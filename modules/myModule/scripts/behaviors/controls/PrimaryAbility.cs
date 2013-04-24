if (!isObject(PrimaryAbilityBehavior))
{
   %template = new BehaviorTemplate(PrimaryAbilityBehavior);

   %template.friendlyName = "Tank Primary Ability";
   %template.behaviorType = "Controls";
   %template.description  = "Tank Primary Ability";

   %template.addBehaviorField(key, "Key to bind to primary ability fire", keybind, "keyboard z");
}

function PrimaryAbilityBehavior::onBehaviorAdd(%this)
{
   if (!isObject(GlobalActionMap))
      return;

   %id = %this.getId();
   GlobalActionMap.bindCmd(getWord(%this.key, 0), getWord(%this.key, 1), %id @ ".startAbility();", %id @ ".endAbility();");
}

function PrimaryAbilityBehavior::onBehaviorRemove(%this)
{
    if (!isObject(GlobalActionMap))
       return;

    GlobalActionMap.unbind(getWord(%this.key, 0), getWord(%this.key, 1));
}

function PrimaryAbilityBehavior::startAbility(%this)
{
   %chargeShot = %this.owner.getBehavior("ChargeShotBehavior");
   if (isObject(%chargeShot))
      %chargeShot.startCharging();
}

function PrimaryAbilityBehavior::endAbility(%this)
{
   %chargeShot = %this.owner.getBehavior("ChargeShotBehavior");
   if (isObject(%chargeShot))
      %chargeShot.shoot();
}