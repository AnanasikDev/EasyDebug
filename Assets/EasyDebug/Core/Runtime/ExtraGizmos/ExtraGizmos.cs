using UnityEngine;
// adapted from http://wiki.unity3d.com/index.php/DrawArrow 
// https://gist.github.com/MatthewMaker/5293052

namespace EasyDebug
{
    public enum ArrowType
    {
        Default,
        Thin,
        Double,
        Triple,
        Solid,
        Fat,
        ThreeD,
    }

    public class RuntimeGizmos
    {
        public static void DrawWireCube(Vector3 position, float size, Color color, float duration = 0.2f)
        {
            float h = size * 0.5f;
            Vector3[] p = {
                position + new Vector3(-h, -h, -h), position + new Vector3(h, -h, -h),
                position + new Vector3(h, -h, h), position + new Vector3(-h, -h, h),
                position + new Vector3(-h, h, -h), position + new Vector3(h, h, -h),
                position + new Vector3(h, h, h), position + new Vector3(-h, h, h)
            };

            Debug.DrawRay(p[0], p[1] - p[0], color, duration); Debug.DrawRay(p[1], p[2] - p[1], color, duration);
            Debug.DrawRay(p[2], p[3] - p[2], color, duration); Debug.DrawRay(p[3], p[0] - p[3], color, duration);
            Debug.DrawRay(p[4], p[5] - p[4], color, duration); Debug.DrawRay(p[5], p[6] - p[5], color, duration);
            Debug.DrawRay(p[6], p[7] - p[6], color, duration); Debug.DrawRay(p[7], p[4] - p[7], color, duration);
            Debug.DrawRay(p[0], p[4] - p[0], color, duration); Debug.DrawRay(p[1], p[5] - p[1], color, duration);
            Debug.DrawRay(p[2], p[6] - p[2], color, duration); Debug.DrawRay(p[3], p[7] - p[3], color, duration);
        }

        public static void DrawCircle(Vector3 position, Vector3 axis1, Vector3 axis2, float radius, Color color, float duration = 0.2f, int segments = 16)
        {
            float angleStep = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                float a1 = Mathf.Deg2Rad * (i * angleStep);
                float a2 = Mathf.Deg2Rad * ((i + 1) * angleStep);
                Vector3 p1 = position + (axis1 * Mathf.Cos(a1) + axis2 * Mathf.Sin(a1)) * radius;
                Vector3 p2 = position + (axis1 * Mathf.Cos(a2) + axis2 * Mathf.Sin(a2)) * radius;
                Debug.DrawRay(p1, p2 - p1, color, duration);
            }
        }

        public static void DrawWireSphere(Vector3 position, float radius, Color color, float duration = 0.2f, int segments = 16)
        {
            DrawCircle(position, Vector3.right, Vector3.up, radius, color, duration, segments);
            DrawCircle(position, Vector3.right, Vector3.forward, radius, color, duration, segments);
            DrawCircle(position, Vector3.up, Vector3.forward, radius, color, duration, segments);
        }


        /// <summary>
        /// Runtime function to draw an arrow in scene space. Uses Debug class for visualization.
        /// </summary>
        public static void DrawArrow(Vector3 pos, Vector3 direction, float duration = 0.2f, Color? color = null, ArrowType type = ArrowType.Default, float arrowHeadLength = 0.2f, float arrowHeadAngle = 30.0f, bool sceneCamFollows = false)
        {
            Color actualColor = color ?? Color.white;
            duration = duration / Time.timeScale;

            float width = 0.01f;

            Vector3 directlyRight = Vector3.zero;
            Vector3 directlyLeft = Vector3.zero;
            Vector3 directlyBack = Vector3.zero;
            Vector3 headRight = Vector3.zero;
            Vector3 headLeft = Vector3.zero;

            if (direction != Vector3.zero)
            {
                directlyRight = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 90, 0) * new Vector3(0, 0, 1);
                directlyLeft = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 90, 0) * new Vector3(0, 0, 1);
                directlyBack = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0) * new Vector3(0, 0, 1);
                headRight = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                headLeft = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            }

            //draw arrow head
            Debug.DrawRay(pos + direction, headRight * arrowHeadLength, actualColor, duration);
            Debug.DrawRay(pos + direction, headLeft * arrowHeadLength, actualColor, duration);

            switch (type)
            {
                case ArrowType.Default:
                    Debug.DrawRay(pos, direction, actualColor, duration); //draw center line
                    break;
                case ArrowType.Double:
                    Debug.DrawRay(pos + directlyRight * width, direction * (1 - width), actualColor, duration); //draw line slightly to right
                    Debug.DrawRay(pos + directlyLeft * width, direction * (1 - width), actualColor, duration); //draw line slightly to left

                    //draw second arrow head
                    Debug.DrawRay(pos + directlyBack * width + direction, headRight * arrowHeadLength, actualColor, duration);
                    Debug.DrawRay(pos + directlyBack * width + direction, headLeft * arrowHeadLength, actualColor, duration);

                    break;
                case ArrowType.Triple:
                    Debug.DrawRay(pos, direction, actualColor, duration); //draw center line
                    Debug.DrawRay(pos + directlyRight * width, direction * (1 - width), actualColor, duration); //draw line slightly to right
                    Debug.DrawRay(pos + directlyLeft * width, direction * (1 - width), actualColor, duration); //draw line slightly to left
                    break;
                case ArrowType.Fat:
                    break;
                case ArrowType.Solid:
                    int increments = 20;
                    for (int i = 0; i < increments; i++)
                    {
                        float displacement = Mathf.Lerp(-width, +width, i / (float)increments);
                        //draw arrow body
                        Debug.DrawRay(pos + directlyRight * displacement, direction, actualColor, duration); //draw line slightly to right
                        Debug.DrawRay(pos + directlyLeft * displacement, direction, actualColor, duration); //draw line slightly to left
                                                                                                            //draw arrow head
                        Debug.DrawRay((pos + direction) + directlyRight * displacement, headRight * arrowHeadLength, actualColor, duration);
                        Debug.DrawRay((pos + direction) + directlyRight * displacement, headLeft * arrowHeadLength, actualColor, duration);
                    }
                    break;
                case ArrowType.Thin:
                    Debug.DrawRay(pos, direction, actualColor, duration); //draw center line
                    break;
                case ArrowType.ThreeD:
                    break;
            }
        }
    }
}