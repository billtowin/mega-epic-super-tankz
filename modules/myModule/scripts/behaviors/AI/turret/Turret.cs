if (!isObject(TurretBehavior))
{
   %template = new BehaviorTemplate(TurretBehavior);

   %template.friendlyName = "Turret";
   %template.behaviorType = "AI";
   %template.description  = "Turret AI";
   
   %template.addBehaviorField(angularSpeed, "Angular/Swivel Speed", float, 70);
   %template.addBehaviorField(rangeRadius, "Range radius of turret", int, 15);
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
   %this.hasAbilityStarted = false;
   
   %turretAbility = %this.owner.getBehavior("TurretAbilityBehavior");
   if (!isObject(%turretAbility))
   {
      %turretAbility = TurretAbilityBehavior.createInstance();
      %this.owner.addBehavior(%turretAbility);
   }
   %this.scanSchedule = %this.schedule(%this.scanUpdateTime, scanForTargets);
}

function TurretBehavior::onBehaviorRemove(%this)
{
   %turretAbility = %this.owner.getBehavior("TurretAbilityBehavior");
   if (isObject(%turretAbility)){
      %this.owner.removeBehavior(%turretAbility);
   }
}

function TurretBehavior::scanForTargets(%this)
{
   %picked = %this.owner.getScene().pickCircle(%this.owner.Position, %this.rangeRadius, -1, -1, "collision");
   %isAnyTargetAvailable = false;
      
   //Gets possible turret targets
   for(%i = 0; %i < getWordCount(%picked) ; %i++)
   {
      %obj = getWord(%picked,%i);
      if(%obj.class $= Tank) {
         if(!%isAnyTargetAvailable) {
            %isAnyTargetAvailable = true;
            %possibleTargets = %obj;   
         } else {
            %possibleTargets = %possibleTargets SPC %obj;
         }
      }
   }
   
   //Finds target at minimum distance
   %minDistanceToTarget = -1;
   %minTargetIndex = -1;
   for(%i = 0 ; %i < getWordCount(%possibleTargets) ; %i++) 
   {
      %possibleTarget = getWord(%possibleTargets, %i);
      %distance = Vector2Distance(%this.owner.Position, %possibleTarget.Position);
      if(%distance < %minDistanceToTarget || %minDistanceToTarget == -1) 
      {
         %minDistanceToTarget = %distance;
         %minTargetIndex = %i;      
      }
   }
   
   //Rotates towards chosen target little by little and fires when pointing towards target
   if(%minTargetIndex < getWordCount(%possibleTargets) && %minTargetIndex >= 0)
   {
      %target = getWord(%possibleTargets, %minTargetIndex);
      %rayPicked = %this.owner.getScene().pickRay(%this.owner.Position, %target.Position, -1, -1);
      %isWallBlockingPath = false;
      for(%j = 0; %j < getWordCount(%rayPicked) ; %j++)
      {
         %possibleWall = getWord(%rayPicked,%j);
         if(%possibleWall.class $= Wall) {
            %isWallBlockingPath = true;
            break;            
         }
      }
      if(!%isWallBlockingPath) 
      {
         if(!%this.hasAbilityStarted) 
         {
            %turretAbility = %this.owner.getBehavior("TurretAbilityBehavior");
            if (isObject(%turretAbility))
            {
               %this.hasAbilityStarted = true;
               %turretAbility.startAbility();
            }
         }
         %targetLocalPoint = %this.owner.getLocalPoint(%target.Position);
         %targetLocalX = getWord(%targetLocalPoint, 0);
         
         %angle = %this.owner.Angle;
         if(%targetLocalX > 0){
            %angle -= %this.angleDelta;         
         } else {
            %angle += %this.angleDelta;         
         }
         
         //Either shoot at acquired target or rotate towards it
         if(mAbs(%targetLocalX) <= %this.targetBuffer) 
         {
            %turretAbility = %this.owner.getBehavior("TurretAbilityBehavior");
            if (isObject(%turretAbility) && %this.hasAbilityStarted)
            {
               %turretAbility.endAbility();
               %this.hasAbilityStarted = false;
            }
         } else {
            %this.owner.rotateTo(%angle, %this.angularSpeed);
         }
      }
   }
   //If no target is acquired and shouldResetAngle is set to true, then rotate toward original angle
   if(!%isAnyTargetAvailable && %this.shouldResetAngle) {
      %this.owner.rotateTo(%this.resetAngle, %this.angularSpeed);
   }
   %this.scanSchedule = %this.schedule(%this.scanUpdateTime, scanForTargets);
}