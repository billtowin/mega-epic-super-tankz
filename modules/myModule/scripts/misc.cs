function getPositiveAngle(%object)
{
   %adjustedAngle = %object.Angle;
   
   //Make sure that the angle is always between 0 and 360 degrees
   if(%adjustedAngle < 0) %adjustedAngle *= -1;
   else if(%adjustedAngle > 0) %adjustedAngle = 360-%adjustedAngle;

   %adjustedAngle %= 360;
   
   return %adjustedAngle;
}