if (!isObject(ProximitySensorBehavior))
{
   %template = new BehaviorTemplate(ProximitySensorBehavior);

   %template.friendlyName = "Proximity Sensor Behavior";
   %template.behaviorType = "Misc";
   %template.description  = "Smart Mine AI whichs renders the owner invisible when Tank /or/ Vehicle is near and reveals itself when they are";
   
   %template.addBehaviorField(scanUpdateTime, "Scan for targets update time", int, 100);
   %template.addBehaviorField(rangeRadius, "Range at which to turn visible", int, 12);
   %template.addBehaviorField(detectTypes, "Types of the SceneObjects to detect", string, "Vehicle");
   
   %template.addBehaviorField(sensePastWalls, "Should the sensor detect objects across walls?", bool, false);
   
   //TODO: Currently Sensor calls its update functions every 100ms.
   // should add an option to only call the functions when the proximity sensor's status changes
}

function ProximitySensorBehavior::onBehaviorAdd(%this)
{
   %this.scanSchedule = %this.schedule(%this.scanUpdateTime, scanForTargets);
}

function ProximitySensorBehavior::onBehaviorRemove(%this)
{
}

function ProximitySensorBehavior::isDetectionTarget(%this, %obj)
{
   for(%i=0; %i < getWordCount(%this.detectTypes); %i++)
   {
      %type = getWord(%this.detectTypes, %i);
      if(%type $= %obj.type) {
         return true;      
      }
   }
   return false;
}
function ProximitySensorBehavior::scanForTargets(%this)
{
   %picked = %this.owner.getScene().pickCircle(%this.owner.Position,%this.rangeRadius, -1, -1, "collision");
   %isDetectionTargetNear = false;
   for(%i = 0; %i < getWordCount(%picked) ; %i++)
   {
      %obj = getWord(%picked,%i);
      %isValidTarget = %this.isDetectionTarget(%obj);
      if(%isValidTarget)
      {
         %isDetectionTargetNear = true;
         //Check if there is a wall blocking the path
         %rayPicked = %this.owner.getScene().pickRay(%this.owner.Position, %obj.Position, -1, -1);
         %isWallBlockingPath = false;
         if(!%this.sensePastWalls)
         {
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
   }
   if(%isDetectionTargetNear && !%isWallBlockingPath) {
      %this.owner.onProximitySensorOn();   
   } else {
      %this.owner.onProximitySensorOff();   
   }
   %this.scanSchedule = %this.schedule(%this.scanUpdateTime, scanForTargets);
}