using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

	[Header("Random Generator")]
	public int seed = 0;
	public bool randomizeSeed = false;
	private System.Random randomGenerator;

	[Header("World Parameters")]
	public int chunkSize = 49;
	public int chunkDistance = 4;
	public GameObject chunkObject;

	[Header("Terrain Parameters")]
	public float noiseScale = 80f;
	public int noiseOctaves = 5;
	[Range(0f,1f)]
	public float noisePersistance = 0.35f;
	public float noiseLacunarity = 3f;
	public float noiseAmplitude = 10f;

	[Header("Editor")]
	public bool autoUpdate = true;

	private void InitializeRandomGenerator() {
		if (randomizeSeed) {
			seed = Random.Range (-100000, 100000);
		}
		randomGenerator = new System.Random (seed);
	}

	public void GenerateChunks() {
		InitializeRandomGenerator ();

		for (int i = transform.childCount - 1; i >= 0; i--) {
			DestroyImmediate(transform.GetChild(i).gameObject);
		}

		Vector2 offset = new Vector2 (
			                 (float)randomGenerator.Next (-100000, 100000),
			                 (float)randomGenerator.Next (-100000, 100000)
		                 );

		for (int y = -chunkDistance; y < chunkDistance + 1; y++) {
			for (int x = -chunkDistance; x < chunkDistance + 1; x++) {
				GenerateChunk (x, y, offset);
			}
		}
	}

	private void GenerateChunk(int chunkIndexX, int chunkIndexY, Vector2 offset) {
		GameObject chunk = Instantiate(chunkObject);
		chunk.name = "Chunk_" + chunkIndexX.ToString () + "_" + chunkIndexY.ToString ();
		chunk.transform.position = new Vector3 (chunkIndexX * (chunkSize - 1), 0f, chunkIndexY * (chunkSize - 1));
		chunk.transform.SetParent (transform, true);

		float[,] terrainHeightMap = TerrainGenerator.GenerateHeightMap (chunkIndexX, chunkIndexY, chunkSize, noiseScale, noiseOctaves, noisePersistance, noiseLacunarity, noiseAmplitude, offset);
		Mesh terrainMesh = TerrainGenerator.GenerateMesh (terrainHeightMap);

		chunk.GetComponent<MeshFilter> ().sharedMesh = terrainMesh;
		chunk.GetComponent<MeshCollider> ().sharedMesh = terrainMesh;
	}

	private void OnValidate() {
		if (chunkSize < 2) {
			Debug.Log ("chunkSize is smaller than 2!");
			chunkSize = 2;
		}
		else if (chunkSize > 105) {
			Debug.Log("chunkSize is greater than 105!");
			chunkSize = 105;
		}

		if (chunkDistance < 0) {
			Debug.Log ("chunkDistance is smaller than 0!");
			chunkDistance = 0;
		}
	}
}