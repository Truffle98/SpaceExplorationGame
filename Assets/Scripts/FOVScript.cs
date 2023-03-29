using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVScript : MonoBehaviour
{
    public GameObject player;
    private PlayerScript playerScript;
    private Mesh mesh;

    //Configuration for mesh
    public int rayCount;
    public float viewDistance, FOV;

    private void Start() 
    {
        playerScript = player.GetComponent<PlayerScript>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        FOV *= Mathf.Deg2Rad;
    }

    public void UpdateFOV()
    {

        float startAngle = playerScript.angle - (FOV / 2f);
        float angleIncrease = FOV / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2];
        vertices[0] = player.transform.position;
        Vector2[] uv = new Vector2[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        int vertexIdx = 1, triangleIdx = 0;

        float curAngle = startAngle;
        Vector3 vertex;
        Vector2 rayDirection;
        RaycastHit2D[] hits;
        for (int i = 0; i <= rayCount; i++)
        {
            rayDirection = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle));
            hits = Physics2D.RaycastAll(player.transform.position, rayDirection, viewDistance);

            vertex = player.transform.position + new Vector3(rayDirection.x * viewDistance, rayDirection.y * viewDistance, 0);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.tag == "Wall" || hit.collider.tag == "Door")
                {
                    vertex = hit.point;
                    break;
                }
            }
            vertices[vertexIdx] = vertex;

            if (i > 0)
            {
                triangles[triangleIdx] = 0;
                triangles[triangleIdx + 1] = vertexIdx - 1;
                triangles[triangleIdx + 2] = vertexIdx;

                triangleIdx += 3;
            }

            vertexIdx++;
            curAngle += angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(player.transform.position, Vector3.one * 1000f);
        mesh.RecalculateBounds();
    }
}
