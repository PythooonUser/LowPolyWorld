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
	public float chunkUnit = 2f;
	private Dictionary<Vector2, GameObject> loadedChunks;
	public Transform playerTransform;
	private Vector2 oldChunkIndex;

	[Header("Terrain Parameters")]
	public float noiseScale = 80f;
	public int noiseOctaves = 5;
	[Range(0f,1f)]
	public float noisePersistance = 0.35f;
	public float noiseLacunarity = 3f;
	public float noiseAmplitude = 10f;
	public float islandRadius = 60f;
	private Vector2 offset;

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

		offset = new Vector2 (
			(float)randomGenerator.Next (-100000, 100000),
			(float)randomGenerator.Next (-100000, 100000)
		);

		loadedChunks = new Dictionary<Vector2, GameObject> ();

		for (int y = -chunkDistance; y < chunkDistance + 1; y++) {
			for (int x = -chunkDistance; x < chunkDistance + 1; x++) {
				Vector2 chunkIndex = new Vector2 (x, y);
				GameObject chunk = GenerateChunk (x, y, offset);
				loadedChunks.Add (chunkIndex, chunk);
			}
		}
	}

	private GameObject GenerateChunk(int chunkIndexX, int chunkIndexY, Vector2 offset) {
		GameObject chunk = Instantiate(chunkObject);
		chunk.name = "Chunk_" + chunkIndexX.ToString () + "_" + chunkIndexY.ToString ();
		chunk.transform.position = new Vector3 (chunkIndexX * (chunkSize - 1), 0f, chunkIndexY * (chunkSize - 1)) * chunkUnit;
		chunk.transform.SetParent (transform, true);

		float[,] terrainHeightMap = TerrainGenerator.GenerateHeightMap (chunkIndexX, chunkIndexY, chunkSize, chunkUnit, noiseScale, noiseOctaves, noisePersistance, noiseLacunarity, noiseAmplitude, islandRadius, offset);
		Mesh terrainMesh = TerrainGenerator.GenerateMesh (terrainHeightMap, chunkUnit);

		chunk.GetComponent<MeshFilter> ().sharedMesh = terrainMesh;
		chunk.GetComponent<MeshCollider> ().sharedMesh = terrainMesh;

		return chunk;
	}

	private void Update() {
		Vector3 playerPosition = playerTransform.position;
		Vector2 currentChunkIndex = new Vector2 ((int)(playerPosition.x / chunkSize), (int)(playerPosition.z / chunkSize));

		if (currentChunkIndex != oldChunkIndex) {
			// Disable all chunks around oldChunk
			for (int y = -chunkDistance + (int)oldChunkIndex.y; y < chunkDistance + (int)oldChunkIndex.y + 1; y++) {
				for (int x = -chunkDistance + (int)oldChunkIndex.x; x < chunkDistance + (int)oldChunkIndex.x + 1; x++) {
					Vector2 chunkIndex = new Vector2 (x, y);
					GameObject chunk = GenerateChunk (x, y, offset);
					loadedChunks [chunkIndex].SetActive (false);
				}
			}

			// Enable or create new chunks around currentChunk
			for (int y = -chunkDistance + (int)currentChunkIndex.y; y < chunkDistance + (int)currentChunkIndex.y + 1; y++) {
				for (int x = -chunkDistance + (int)currentChunkIndex.x; x < chunkDistance + (int)currentChunkIndex.x + 1; x++) {
					Vector2 chunkIndex = new Vector2 (x, y);
					if (loadedChunks.ContainsKey (chunkIndex)) {
						loadedChunks [chunkIndex].SetActive (true);
					} else {
						GameObject chunk = GenerateChunk (x, y, offset);
					}
				}
			}
		}
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