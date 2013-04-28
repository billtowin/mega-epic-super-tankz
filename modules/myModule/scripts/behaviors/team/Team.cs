if (!isObject(TeamBehavior))
{
   %template = new BehaviorTemplate(TeamBehavior);

   %template.friendlyName = "Team Behavior";
   %template.behaviorType = "Team";
   %template.description  = "Behavior meant for assigning teams to various objects for team gamemodes and for AI sub-routines";

   %template.addBehaviorField(teamID, "Team Identification number", int, 0);
   %template.addBehaviorField(teamColor, "Team Color", string, "Green");
}

function TeamBehavior::onBehaviorAdd(%this)
{   
}

function TeamBehavior::onBehaviorRemove(%this)
{
}

function TeamBehavior::isEnemy(%this, %possibleEnemy)
{
   %team = %possibleEnemy.getBehavior("TeamBehavior");
   //If entity isn't on a team, then it's automatically an enemy
   %returnVal = !isObject(%team) ? true : (%this.teamID != %team.teamID);
   return %returnVal;
}