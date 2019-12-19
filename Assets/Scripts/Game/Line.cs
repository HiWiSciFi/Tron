using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Line : MonoBehaviour
{
    public byte ID;
    public Controller parentController;
    private const float defaultOfset = 0;
    private const float defaultHeight = 3;

    public static Mesh GenerateMesh(List<Vector2> basePoints, float ofset, float height)
    {
        Mesh toReturn = new Mesh();
        toReturn.vertices = new Vector3[basePoints.Count*2];
        for(int i = 0; i < basePoints.Count; i++)
        {
            toReturn.vertices[i*2] = new Vector3(basePoints[i].x, basePoints[i].y, ofset);
            toReturn.vertices[(i*2)+1] = new Vector3(basePoints[i].x, basePoints[i].y, ofset + height);
        }
        
        List<int> tris = new List<int>();
        for( int i = 3; i < toReturn.vertices.Length/2; i++)
        {
            tris.Add(i-3);
            tris.Add(i-2);
            tris.Add(i-1);
            tris.Add(i-2);
            tris.Add(i-1);
            tris.Add(i);
        }

        toReturn.triangles = tris.ToArray();
        toReturn.RecalculateNormals();
        return null;
    }
    public static Mesh GenerateMesh(List<Vector2> basePoints)
    {
        return GenerateMesh(basePoints, defaultOfset, defaultHeight);
    }
}