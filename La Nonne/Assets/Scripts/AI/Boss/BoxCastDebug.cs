using UnityEngine;

public class BoxCastDebug : MonoBehaviour
{
    //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
    public static void DrawBoxCast2D(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, Color color)
    {
        var angleZ = Quaternion.Euler(0f, 0f, angle);
        DrawBoxCastBox(origin, size / 2f, angleZ, direction, distance, color);
    }

    private static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
     {
         direction.Normalize();
         var bottomBox = new Box(origin, halfExtents, orientation);
         var topBox = new Box(origin + (direction * distance), halfExtents, orientation);
             
         Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft,    color);
         Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
         Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
         Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight,    color);
         Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft,    color);
         Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
         Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
         Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight,    color);
     
         DrawBox(bottomBox, color);
         DrawBox(topBox, color);
     }
     
     /*public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
     {
         DrawBox(new Box(origin, halfExtents, orientation), color);
     }*/

     private static void DrawBox(Box box, Color color)
     {
         Debug.DrawLine(box.frontTopLeft,     box.frontTopRight,    color);
         Debug.DrawLine(box.frontTopRight,     box.frontBottomRight, color);
         Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
         Debug.DrawLine(box.frontBottomLeft,     box.frontTopLeft, color);
                                                  
         Debug.DrawLine(box.backTopLeft,         box.backTopRight, color);
         Debug.DrawLine(box.backTopRight,     box.backBottomRight, color);
         Debug.DrawLine(box.backBottomRight,     box.backBottomLeft, color);
         Debug.DrawLine(box.backBottomLeft,     box.backTopLeft, color);
                                                  
         Debug.DrawLine(box.frontTopLeft,     box.backTopLeft, color);
         Debug.DrawLine(box.frontTopRight,     box.backTopRight, color);
         Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
         Debug.DrawLine(box.frontBottomLeft,     box.backBottomLeft, color);
     }

     private struct Box
     {
         private Vector3 localFrontTopLeft     {get; set;}
         private Vector3 localFrontTopRight    {get; set;}
         private Vector3 localFrontBottomLeft  {get; set;}
         private Vector3 localFrontBottomRight {get; set;}
         private Vector3 localBackTopLeft => -localFrontBottomRight;
         private Vector3 localBackTopRight => -localFrontBottomLeft;
         private Vector3 localBackBottomLeft => -localFrontTopRight;
         private Vector3 localBackBottomRight => -localFrontTopLeft;

         public Vector3 frontTopLeft => localFrontTopLeft + origin;
         public Vector3 frontTopRight => localFrontTopRight + origin;
         public Vector3 frontBottomLeft => localFrontBottomLeft + origin;
         public Vector3 frontBottomRight => localFrontBottomRight + origin;
         public Vector3 backTopLeft => localBackTopLeft + origin;
         public Vector3 backTopRight => localBackTopRight + origin;
         public Vector3 backBottomLeft => localBackBottomLeft + origin;
         public Vector3 backBottomRight => localBackBottomRight + origin;

         private Vector3 origin {get;}
 
         public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
         {
             Rotate(orientation);
         }

         private Box(Vector3 origin, Vector3 halfExtents)
         {
             halfExtents /= 2;
             
             this.localFrontTopLeft     = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
             this.localFrontTopRight    = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
             this.localFrontBottomLeft  = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
             this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
 
             this.origin = origin;
         }


         private void Rotate(Quaternion orientation)
         {
             localFrontTopLeft     = RotatePointAroundPivot(localFrontTopLeft    , Vector3.zero, orientation);
             localFrontTopRight    = RotatePointAroundPivot(localFrontTopRight   , Vector3.zero, orientation);
             localFrontBottomLeft  = RotatePointAroundPivot(localFrontBottomLeft , Vector3.zero, orientation);
             localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
         }
     }

     private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
     {
         var direction = point - pivot;
         return pivot + rotation * direction;
     }
}
