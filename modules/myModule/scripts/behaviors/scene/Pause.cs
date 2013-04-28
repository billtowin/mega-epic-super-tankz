if (!isObject(PauseBehavior))  
{  
   %template = new BehaviorTemplate(PauseBehavior);  
     
   %template.friendlyName = "Pause Game";  
   %template.behaviorType = "Scene";  
   %template.description  = "Pause and unpause the game";  
  
   %template.addBehaviorField(pauseKey, "The button to pause the game", keybind, "keyboard escape");  
}  
  
function PauseBehavior::onBehaviorAdd(%this)  
{  
   if (!isObject(GlobalActionMap))  
      return;  
        
   GlobalActionMap.bindCmd( getWord(%this.pauseKey, 0), getWord(%this.pauseKey, 1), "pauseGame();", "");  
  
   $isPaused = false;  
}  
  
function pauseGame()  
{
   $timescale = $isPaused ? 1 : 0;     
   $isPaused = !$isPaused;  
}  