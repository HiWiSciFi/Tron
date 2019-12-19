using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour
{
    public float currentMoveSpeed = 0.0f;
    public bool boostActive = false;
    public bool boostAvailable = false;

    private GameObject line;
    private Rigidbody rb;
    private List<Vector3> points = new List<Vector3>();
    private MeshFilter mf;
    private MeshCollider mc;

    public bool killed = false;
    public bool local = false;

    public int ID;

    protected void InitController()
    {
        line = new GameObject("Line");
        line.transform.position = new Vector3(0, 0, 0);
        mc = line.AddComponent<MeshCollider>();
        mf = line.AddComponent<MeshFilter>();
        Line l = line.AddComponent<Line>();
        line.layer = 9;

        MeshRenderer mr = line.AddComponent<MeshRenderer>();
        mr.sharedMaterial = Resources.Load<Material>("LineMaterial");

        rb = GetComponent<Rigidbody>();

        StartCoroutine(createBarrier());
        StartCoroutine(boostCountdown());
    }

    protected void updateController()
    {
        if (!killed)
        {
            if (!boostActive)
                currentMoveSpeed = GameSettings.moveSpeed;
            else
                currentMoveSpeed = GameSettings.moveSpeed * GameSettings.boostMultiplier;
            rb.velocity = transform.TransformDirection(new Vector3(0, 0, currentMoveSpeed));
        }
    }

    protected void turn(float value)
    {
        transform.Rotate(0, value, 0);
    }

    protected void boost()
    {
        boostActive = true;
        boostAvailable = false;
        StartCoroutine(activateBoost());
    }

    protected IEnumerator activateBoost()
    {
        yield return new WaitForSeconds(GameSettings.boostTimeSeconds);
        boostActive = false;
        StartCoroutine(boostCountdown());
    }

    private IEnumerator boostCountdown()
    {
        yield return new WaitForSeconds(GameSettings.boostCountdownSeconds);
        boostAvailable = true;
    }

    private IEnumerator createBarrier()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            points.Add(transform.position + new Vector3(0, 0.1f, 0));
            if (mf.sharedMesh == null)
                mf.sharedMesh = new Mesh();
            Mesh m = extrudeAlongPath(points, 0.1f);
            mf.sharedMesh = m;
            mc.sharedMesh = m;
        }
    }

    private Mesh extrudeAlongPath(List<Vector3> points, float width)
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();

        for (int i = 0; i < points.Count; i++)
        {
            verts.Add(points[i]);
            verts.Add(points[i] + new Vector3(0, 1, 0));
        }
        m.vertices = verts.ToArray();
        m.normals = norms.ToArray();

        List<int> tris = new List<int>();
        for (int i = 0; i < m.vertices.Length - 3; i++)
        {
            if (i % 2 == 0)
            {
                tris.Add(i + 2);
                tris.Add(i + 1);
                tris.Add(i);
            }
            else
            {
                tris.Add(i);
                tris.Add(i + 1);
                tris.Add(i + 2);
            }
        }
        m.triangles = tris.ToArray();

        m.name = "pathMesh";
        m.RecalculateNormals();
        m.RecalculateBounds();
        m.Optimize();
        return m;
    }

    public Controller[] getAllPlayers()
    {
        return FindObjectsOfType<Controller>();
    }
}