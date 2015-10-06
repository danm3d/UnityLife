using System.Collections;
using UnityEngine;

public enum LifeRules
{
	Conway = 0,
	Maze = 1
}

public class GameOfLife : MonoBehaviour
{

	public LifeRules Rules = new LifeRules ();
	private bool maze = false;
	[Range(0.1f, 2f)]
	public float
		speed = .25f;
	public int width = 128, height = 128;
	private bool[,] grid;
	private bool wrapEdges = true;
	private Rect textureRect;
	private Texture2D golTexture;
	#region MonoBehaviours

	private void OnGUI ()
	{
		speed = GUI.VerticalSlider (new Rect (0, 0, 64, Screen.height), speed, 0f, .5f);
		GUI.DrawTexture (textureRect, golTexture, ScaleMode.StretchToFill);
		if (GUI.Button (new Rect (32, 70, 128, 64), "Randomise")) {
			RandomiseGrid (ref grid);
		}
		if (GUI.Button (new Rect (32, 150, 128, 64), "Clear")) {
			ClearGrid (ref grid);
		}
		maze = GUI.Toggle (new Rect (32, 210, 128, 64), maze, maze ? "Conway" : "Maze Rules");
	}

	private void Start ()
	{
		golTexture = new Texture2D (width, height);
		golTexture.filterMode = FilterMode.Point;
		textureRect = new Rect (Screen.width / 2 - width, Screen.height / 2 - width, width * 2, height * 2);
		grid = new bool[width, height];
		RandomiseGrid (ref grid);
		Camera.main.transform.position = new Vector3 (grid.GetLength (0) / 2, grid.GetLength (1) / 2, -100);
		StartCoroutine ("Epoch");
	}

	#endregion MonoBehaviours

	#region GameOfLife

	private int CountLivingNeighbours (int x, int y, bool[,] grid)
	{
		int count = 0;

		for (int j = -1; j <= 1; j++) {
			if (!wrapEdges && y + j < 0 || y + j >= grid.GetLength (1)) {
				continue;
			}

			int k = (y + j + grid.GetLength (1)) % grid.GetLength (1);

			for (int i = -1; i <= 1; i++) {
				if (!wrapEdges && x + i < 0 || x + i >= grid.GetLength (0)) {
					continue;
				}
				int h = (x + i + grid.GetLength (0)) % grid.GetLength (0);

				count += grid [h, k] ? 1 : 0;
			}
		}
		return count - (grid [x, y] ? 1 : 0);

	}

	private void ClearGrid (ref bool[,] grid)
	{
		grid = new bool[grid.GetLength (0), grid.GetLength (1)];
	}

	private IEnumerator Epoch ()
	{
		while (true) {
			NextGeneration (ref grid);
			yield return new WaitForSeconds (speed);
		}
	}

	private void NextGeneration (ref bool[,] grid)
	{
		bool[,] nextGrid = new bool[grid.GetLength (0), grid.GetLength (1)];

		for (int row = 0; row < grid.GetLength(0); row++) {
			for (int col = 0; col < grid.GetLength(1); col++) {
				bool living = grid [row, col];
				int neighbours = CountLivingNeighbours (row, col, grid);

				if (living) {
					if (maze) {
						nextGrid [row, col] = (neighbours >= 1 && neighbours <= 5);
					} else {
						nextGrid [row, col] = (neighbours == 2 || neighbours == 3);
					}
				} else {
					nextGrid [row, col] = neighbours == 3;
				}
				golTexture.SetPixel (row, col, nextGrid [row, col] ? Color.black : Color.white);
			}
		}
		golTexture.Apply ();
		grid = nextGrid;
	}

	private void RandomiseGrid (ref bool[,] grid)
	{
		for (int row = 0; row < grid.GetLength(0); row++) {
			for (int col = 0; col < grid.GetLength(1); col++) {
				grid [row, col] = Random.Range (0, 10) == 1;
			}
		}
	}

	#endregion GameOfLife
}