if (!isObject(TankControlsBehavior))
{
   %template = new BehaviorTemplate(TankControlsBehavior);

   %template.friendlyName = "Tank Controls";
   %template.behaviorType = "Input";
   %template.description  = "Tank style movement control";

   %template.addBehaviorField(forwardKey, "Key to bind to forward movement", keybind, "keyboard up");
   %template.addBehaviorField(backwardKey, "Key to bind to backward movement", keybind, "keyboard down");
   %template.addBehaviorField(leftKey, "Key to bind to left movement", keybind, "keyboard left");
   %template.addBehaviorField(rightKey, "Key to bind to right movement", keybind, "keyboard right");
}

function TankControlsBehavior::onBehaviorAdd(%this)
{
   if (!isObject(GlobalActionMap))
      return;

   %id = %this.owner.getId();
   GlobalActionMap.bindCmd(getWord(%this.forwardKey, 0), getWord(%this.forwardKey, 1), %id @ ".moveForward();", %id @ ".stopMovement();");
   GlobalActionMap.bindCmd(getWord(%this.backwardKey, 0), getWord(%this.backwardKey, 1), %id @ ".moveBackward();", %id @ ".stopMovement();");
   GlobalActionMap.bindCmd(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1), %id @ ".turnLeft();", %id @ ".stopTurn();");
   GlobalActionMap.bindCmd(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1), %id @ ".turnRight();", %id @ ".stopTurn();");
}

function TankControlsBehavior::onBehaviorRemove(%this)
{
    if (!isObject(GlobalActionMap))
       return;

    GlobalActionMap.unbind(getWord(%this.forwardKey, 0), getWord(%this.forwardKey, 1));
    GlobalActionMap.unbind(getWord(%this.backwardKey, 0), getWord(%this.backwardKey, 1));
    GlobalActionMap.unbind(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1));
    GlobalActionMap.unbind(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1));
}