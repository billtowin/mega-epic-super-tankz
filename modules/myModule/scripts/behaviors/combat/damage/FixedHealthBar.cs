if (!isObject(FixedHealthBarBehavior))
{
   %template = new BehaviorTemplate(FixedHealthBarBehavior);
   
   %template.friendlyName = "Fixed Health Bar";
   %template.behaviorType = "Game";
   %template.description  = "Display current health with a health bar for an object using the Takes Damage behavior.";

   %template.addBehaviorField(healthObject, "The object which needs a health bar", SceneObject, "");
   
   %fixedEdges = "Top" TAB "Bottom" TAB "Left" TAB "Right" TAB "Center Horizontal" TAB "Center Vertical";
   %template.addBehaviorField(fixedEdge, "The edge of the health bar you want to be fixed in place", enum, "Center Horizontal", %fixedEdges);
}

function FixedHealthBarBehavior::onAddToScene(%this)
{
   %this.owner.setUpdateCallback(true);
   %this.maxHealth = %this.healthObject.getBehavior("TakesDamageBehavior").health;
   %this.maxSizeX = %this.owner.Size.x;
   %this.maxSizeY = %this.owner.Size.y;
   %this.startPosX = %this.healthObject.Position.x;
   %this.startPosY = %this.healthObject.Position.y - %this.healthObject.Size.y * 0.6;
   
   %hObject = %this.healthObject.getBehavior("TakesDamageBehavior");
   if (!isObject(%hObject))
      return;
   %this.maxHealth = %hObject.health;
}

function FixedHealthBarBehavior::onBehaviorRemove(%this)
{
}

function FixedHealthBarBehavior::onUpdate(%this)
{

   %hObject = %this.healthObject.getBehavior("TakesDamageBehavior");
   if (!isObject(%hObject))
      return;

   %currentHealth=%hObject.health;

   if (%currentHealth<0)
      %currentHealth = 0;
      
   %healthRatio = (%currentHealth) / (%this.maxHealth);
   
   %this.updateSizeAndPosition(%healthRatio);
}

function FixedHealthBarBehavior::updateSizeAndPosition(%this, %hRatio)
{
   %this.owner.Position.x = %this.healthObject.Position.x;
   %this.owner.Position.y = %this.healthObject.Position.y - %this.healthObject.Size.y * 0.6;
   
   %xPos = %this.owner.Position.x;
   %yPos = %this.owner.Position.y;
   %fixedEdge = %this.fixedEdge;
   
   switch$ (%fixedEdge)
   {
      case "Top":
         %newSize = (%this.maxSizeY * %hRatio);
         %sizeDiff = (%this.maxSizeY - %newSize);
         %this.owner.Size.y = %newSize;
         %this.owner.Position.y = %this.startPosY - (%sizeDiff / 2);
      case "Bottom":
         %newSize = (%this.maxSizeY * %hRatio);
         %sizeDiff = (%this.maxSizeY - %newSize);
         %this.owner.Size.y = %newSize;
         %this.owner.Position.y = %this.startPosY + (%sizeDiff / 2);
      case "Left":
         %newSize = (%this.maxSizeX * %hRatio);
         %sizeDiff = (%this.maxSizeX - %newSize);
         %this.owner.Size.x = %newSize;
         %this.owner.Position.x = %this.startPosX - (%sizeDiff / 2);
      case "Right":
         %newSize = (%this.maxSizeX * %hRatio);
         %sizeDiff = (%this.maxSizeX - %newSize);
         %this.owner.Size.x = %newSize;
         %this.owner.Position.x = %this.startPosX + (%sizeDiff / 2);
      case "Center Horizontal":
         %newSize = (%this.maxSizeX * %hRatio);
         %this.owner.Size.x = %newSize;
      case "Center Vertical":
         %newSize = (%this.maxSizeY * %hRatio);
         %this.owner.Size.y = %newSize;     
   }   

}