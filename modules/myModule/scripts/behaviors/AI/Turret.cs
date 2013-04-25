if (!isObject(TurretBehavior))
{
   %template = new BehaviorTemplate(TurretBehavior);

   %template.friendlyName = "Turret";
   %template.behaviorType = "AI";
   %template.description  = "Turret AI";
   
   %template.addBehaviorField(angularSpeed, "Angular/Swivel Speed", float, 70);
   %template.addBehaviorField(range, "Range of turret", int, 15);
   %template.addBehaviorField(scanUpdateTime, "Scan for targets update time", int, 100);
   
   %template.addBehaviorField(resetAngle, "Angle to reset to ", float, 0);
   %template.addBehaviorField(shouldResetAngle, "Should reset to resetAngle if no targets are available?", bool, true);
   
   %template.addBehaviorField(angleDelta, "Number of degrees to turn when target is not infront of turret", float, 7);
   %template.addBehaviorField(targetBuffer, "Number of meters that is good enough for target to acquired", float, 1.5);
   // put straight line through the turret's "mouth", 
   // if there is a perpendicular line formed by the Tank's Position and the mouth line less that the targetBuffer, shoot at target
}

function TurretBehavior::onBehaviorAdd(%this)
{
   %chargeShot = %this.owner.getBehavior("ChargeShotBehavior");
   if (!isObject(%chargeShot))
   {
      %chargeShot = ChargeShotBehavior.createInstance();
      %chargeShot.duration = -1;
      %chargeShot.damage = 20;
      %chargeShot.minSpeed = 25;
      %chargeShot.maxSpeed = 25;
      %chargeShot.maxChargeTime = 1000;
      %chargeShot.reloadTime = 3000;
      %chargeShot.chargeShotLifespan = 1000;
      %this.owner.addBehavior(%chargeShot);
   }
   %this.scanSchedule = %this.schedule(%this.scanUpdateTime, scanForTargets);
}

function TurretBehavior::onBehaviorRemove(%this)
{
   %chargeShot = %this.owner.getBehavior("ChargeShotBehavior");
   if (isObject(%chargeShot))
      %this.owner.removeBehavior(%chargeShot);
}

function TurretBehavior::scanForTargets(%this)
{
   %startX = %this.owner.Position.x - %this.range;
   %endX = %this.owner.Position.x + %this.range;
   %startY = %this.owner.Position.y - %this.range;
   %endY = %this.owner.Position.y + %this.range;
   %startPoint = %startX SPC %startY;
   %endPoint = %endX SPC %endY;
   %picked = %this.owner.getScene().pickArea(%startPoint, %endPoint, -1, -1);
   %isAnyTargetAvailable = false;
   for(%i = 0; %i < getWordCount(%picked) ; %i++)
   {
      %obj = getWord(%picked,%i);
      if(%obj.class $= Tank)
      {
         %rayPicked = %this.owner.getScene().pickRayCollision(%this.owner.Position, %obj.Position, -1, -1);
         %isWallBlockingPath = false;
         for(%j = 0; %j < getWordCount(%rayPicked) ; %j++)
         {
            %possibleWall = getWord(%rayPicked,%j);
            if(%possibleWall.class $= Wall) {
               %isWallBlockingPath = true;
               break;            
            }
         }
         if(!%isWallBlockingPath) {
            %isAnyTargetAvailable = true;
            %targetLocal = %this.owner.getLocalPoint(%obj.Position);
            %x = getWord(%targetLocal, 0);
            %y = getWord(%targetLocal, 1);
            
            %angle = %this.owner.Angle;
            if(%x > 0){
               %angle -= %this.angleDelta;         
            } else {
               %angle += %this.angleDelta;         
            }
            
            if(mAbs(%x) <= %this.targetBuffer) {
               %chargeShot = %this.owner.getBehavior("ChargeShotBehavior");
               if (isObject(%chargeShot))
                  %chargeShot.shoot();
            } else {
               %this.owner.rotateTo(%angle, %this.angularSpeed);
            }
         }
      }
   }
   if(!%isAnyTargetAvailable && %this.shouldResetAngle) {
      %this.owner.rotateTo(%this.resetAngle, %this.angularSpeed);
   }
   %this.scanSchedule = %this.schedule(%this.scanUpdateTime, scanForTargets);
}