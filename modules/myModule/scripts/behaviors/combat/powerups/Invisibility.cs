if (!isObject(InvisibilityBehavior))
{
   %template = new BehaviorTemplate(InvisibilityBehavior);

   %template.friendlyName = "SmartMine Behavior";
   %template.behaviorType = "AI";
   %template.description  = "Smart Mine AI whichs renders the owner invisible when Tank /or/ Vehicle is near and reveals itself when they are";
   
   %template.addBehaviorField(scanUpdateTime, "Scan for targets update time", int, 100);
   %template.addBehaviorField(rangeRadius, "Range at which to turn visible", int, 12);
   
}

function InvisibilityBehavior::onBehaviorAdd(%this)
{
   %this.scanSchedule = %this.schedule(%this.scanUpdateTime, scanForTargets);
}

function InvisibilityBehavior::onBehaviorRemove(%this)
{
}

function InvisibilityBehavior::scanForTargets(%this)
{
   %picked = %this.owner.getScene().pickCircle(%this.owner.Position,%this.rangeRadius, -1, -1, "collision");
   %isVehicleNear = false;
   for(%i = 0; %i < getWordCount(%picked) ; %i++)
   {
      %obj = getWord(%picked,%i);
      if(%obj.type $= Vehicle)
      {
         %isVehicleNear = true;
         //Check if there is a wall blocking the path
         %rayPicked = %this.owner.getScene().pickRay(%this.owner.Position, %obj.Position, -1, -1);
         %isWallBlockingPath = false;
         for(%j = 0; %j < getWordCount(%rayPicked) ; %j++)
         {
            %possibleWall = getWord(%rayPicked,%j);
            if(%possibleWall.type $= Wall) {
               %isWallBlockingPath = true;
               break;            
            }
         }
      }
   }
   if(%isVehicleNear && !%isWallBlockingPath) {
      %this.owner.onHide();   
   } else {
      %this.owner.onReveal();   
   }
   %this.scanSchedule = %this.schedule(%this.scanUpdateTime, scanForTargets);
}