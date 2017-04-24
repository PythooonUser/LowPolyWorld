using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

	[Header("Random Generator")]
	public int seed;
	public bool randomizeSeed;
	private System.Random randomGenerator;

	[Header("World Parameters")]
	public int chunkSize;
	public int chunkDistance;
	public GameObject chunkObject;

	[Header("Terrain Parameters")]
	public float scale;
	public float amplitude;
	public int octaves;
	[Range(0f,1f)]
	public float persistance;
	public float lacunarity;

	[Header("Editor")]
	public bool autoUpdate;

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

		float[,] terrainHeightMap = TerrainGenerator.GenerateHeightMap (chunkIndexX, chunkIndexY, chunkSize, scale, amplitude, octaves, persistance, lacunarity, offset);
		Mesh terrainMesh = TerrainGenerator.GenerateMesh (terrainHeightMap);

		chunk.GetComponent<MeshFilter> ().sharedMesh = terrainMesh;
	}
}