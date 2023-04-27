using UnityEngine;

// https://gist.github.com/Siccity/5cbfd8dd931f03cd088df491ae24d0a8

public class DebugMesh : MonoBehaviour {
	private MeshFilter filter { get { return _filter != null ? _filter : _filter = GetComponent<MeshFilter>(); } }
	private MeshFilter _filter;

	public bool showNormals = true;
	public bool showTangents = true;
	public float renderDistance = 50.0f;

    public float length = 0.5f;

	[ContextMenu("Print mesh info")]
	public void PrintMeshInfo() {
		Debug.Log("Verts: " + filter.sharedMesh.vertexCount + ", Norms: " + filter.sharedMesh.normals.Length + " Uvs: " + filter.sharedMesh.uv.Length + " Tris: " + filter.sharedMesh.triangles.Length + " Submeshes: " + filter.sharedMesh.subMeshCount);
	}

	// Update is called once per frame
	void OnDrawGizmos() {
		if (!filter) return;

		Vector3[] verts = filter.sharedMesh.vertices;
		Vector4[] tangents = filter.sharedMesh.tangents;
		Vector3[] norms = filter.sharedMesh.normals;
		for (int i = 0; i < verts.Length; i++) {
			verts[i] = transform.TransformPoint(verts[i]);
			if (Vector3.Distance(verts[i], Camera.current.transform.position) > renderDistance) continue;

			if (showNormals) {
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(verts[i], verts[i] + transform.rotation * norms[i] * length);
			}

			if (showTangents) {
				if (tangents[i].w > 0) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawLine(verts[i], verts[i] + transform.rotation * tangents[i] * length);
				} else {
					Gizmos.color = Color.red;
					Gizmos.DrawLine(verts[i], verts[i] + transform.rotation * -tangents[i] * length);
				}
			}
		}
	}
}