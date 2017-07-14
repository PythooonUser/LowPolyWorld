using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainGenerator {

	public static float[,] GenerateHeightMap(int chunkIndexX, int chunkIndexY, int chunkSize, float chunkUnit, float noiseScale, int noiseOctaves, float noisePersistance, float noiseLacunarity, float noiseAmplitude, float islandRadius, Vector2 offset) {
		float chunkCenterPositionX = chunkIndexX * (chunkSize - 1);
		float chunkCenterPositionY = chunkIndexY * (chunkSize - 1);

		int halfChunkSize = (chunkSize - 1) / 2;

		float[,] heightMap = new float[chunkSize, chunkSize];

		for (int y = 0; y < chunkSize; y++) {
			for (int x = 0; x < chunkSize; x++) {
				float amplitude = 1f;
				float frequency = 1f;
				float heightValue = 0f;

				float xTransform = x + chunkCenterPositionX - halfChunkSize;
				float yTransform = y + chunkCenterPositionY - halfChunkSize;

				// Perlin Noise
				for (int i = 0; i < noiseOctaves; i++) {
					float xSample = xTransform / noiseScale * frequency + offset.x;
					float ySample = yTransform / noiseScale * frequency + offset.y;

					float perlinNoiseValue = Mathf.PerlinNoise (xSample, ySample) * 2f - 1f;
					heightValue += perlinNoiseValue * amplitude;

					amplitude *= noisePersistance;
					frequency *= noiseLacunarity;
				}

				// Island Mask
				float distanceFromCenter = Mathf.Sqrt (xTransform * xTransform + yTransform * yTransform);
				float maskValue = (islandRadius - distanceFromCenter) / islandRadius;

				heightMap [x, y] = heightValue * noiseAmplitude + maskValue * noiseAmplitude;
			}
		}

		return heightMap;
	}

	public static Mesh GenerateMesh(float[,] heightMap, float chunkUnit) {
		Mesh mesh = new Mesh ();
		mesh.name = "Generated Mesh";

		int heightMapSize = heightMap.GetLength (0);
		int chunkSize = heightMapSize;
		int halfChunkSize = (chunkSize - 1) / 2;

		List<Vector3> vertices = new List<Vector3> ();
		List<int> triangles = new List<int> ();
		int verticeIndex = 0;

		for (int y = 0; y < heightMapSize - 1; y++) {
			for (int x = 0; x < heightMapSize - 1; x++) {
				float xTransform = x - halfChunkSize;
				float yTransform = y - halfChunkSize;

				vertices.Add (new Vector3 (xTransform * chunkUnit, heightMap [x, y], yTransform * chunkUnit));
				vertices.Add (new Vector3 ((xTransform + 1) * chunkUnit, heightMap [x + 1, y + 1], (yTransform + 1) * chunkUnit));
				vertices.Add (new Vector3 ((xTransform + 1) * chunkUnit, heightMap [x + 1, y], yTransform * chunkUnit));
				triangles.Add (verticeIndex);
				triangles.Add (verticeIndex + 1);
				triangles.Add (verticeIndex + 2);

				vertices.Add (new Vector3 (xTransform * chunkUnit, heightMap [x, y], yTransform * chunkUnit));
				vertices.Add (new Vector3 (xTransform * chunkUnit, heightMap [x, y + 1], (yTransform + 1) * chunkUnit));
				vertices.Add (new Vector3 ((xTransform + 1) * chunkUnit, heightMap [x + 1, y + 1], (yTransform + 1) * chunkUnit));
				triangles.Add (verticeIndex + 3);
				triangles.Add (verticeIndex + 4);
				triangles.Add (verticeIndex + 5);

				verticeIndex += 6;
			}
		}

		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles.ToArray ();

		mesh.RecalculateNormals ();

		return mesh;
	}
}