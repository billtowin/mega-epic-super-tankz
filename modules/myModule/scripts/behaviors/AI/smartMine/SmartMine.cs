if (!isObject(SmartMineBehavior))
{
   %template = new BehaviorTemplate(SmartMineBehavior);

   %template.friendlyName = "SmartMine Behavior";
   %template.behaviorType = "AI";
   %template.description  = "Smart Mine AI whichs renders the owner invisible when Tank /or/ Vehicle is near and reveals itself when they are";
   
   %template.addBehaviorField(scanUpdateTime, "Scan for targets update time", int, 100);
   %template.addBehaviorField(rangeRadius, "Range at which to turn visible", int, 12);
   
}

function SmartMineBehavior::onBehaviorAdd(%this)
{
   %this.scanSchedule = %this.schedule(%this.scanUpdateTime, scanForTargets);
}

function SmartMineBehavior::onBehaviorRemove(%this)
{
}

function SmartMineBehavior::scanForTargets(%this)
{
   %picked = %this.owner.getScene().pickCircle(%this.owner.Position,%this.rangeRadius, -1, -1, "collision");
   %isTankNear = false;
   for(%i = 0; %i < getWordCount(%picked) ; %i++)
   {
      %obj = getWord(%picked,%i);
      if(%obj.class $= Tank) 
      {
         %isTankNear = true;
         
         //Check if there is a wall blocking the path
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
      }
   }
   
   if(%isTankNear && !%isWallBlockingPath) {
      %this.owner.Visible = true;   
   } else {
      %this.owner.Visible = false;
   }
   %this.scanSchedule = %this.schedule(%this.scanUpdateTime, scanForTargets);
}