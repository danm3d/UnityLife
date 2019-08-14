using UnityEngine;
using Random = UnityEngine.Random;

public enum LifeRules
{
	Conway = 0,
	Maze = 1
}

public class GameOfLife : MonoBehaviour
{
	protected Texture2D golTexture;

	protected bool[,] grid;

	[SerializeField, Header("Grid Settings")]
	protected int width = 128;

	[SerializeField]
	protected int height = 128;

	protected bool maze = false;

	[SerializeField]
	protected MeshRenderer meshRenderer;

	[SerializeField]
	protected Camera cam;

	[SerializeField]
	protected LifeRules Rules;

	protected Rect textureRect;


	[SerializeField, Range(0.05f, 1f)]
	protected float updateRate = .05f;
	protected float timer;

	[SerializeField]
	protected bool wrapEdges = true;

	protected void ClearGrid(ref bool[,] grid)
	{
		grid = new bool[width, height];
	}

	protected int CountLivingNeighbours(int xPos, int yPos, bool[,] grid)
	{
		int count = 0;

		for (int j = -1; j <= 1; j++)
		{
			if ((!wrapEdges && yPos + j < 0) || yPos + j >= height)
			{
				continue;
			}

			int k = (yPos + j + height) % height;

			for (int i = -1; i <= 1; i++)
			{
				if ((!wrapEdges && xPos + i < 0) || xPos + i >= width)
				{
					continue;
				}
				int h = (xPos + i + width) % width;

				count += grid[h, k] ? 1 : 0;
			}
		}
		return count - (grid[xPos, yPos] ? 1 : 0);
	}

	protected void DrawTexture()
	{
		meshRenderer.material.mainTexture = golTexture;
	}

	protected void LateUpdate()
	{
		DrawTexture();
	}

	protected void NextGeneration(ref bool[,] grid)
	{
		bool[,] nextGrid = new bool[width, height];

		for (int row = 0; row < width; row++)
		{
			for (int col = 0; col < height; col++)
			{
				bool living = grid[row, col];
				int neighbours = CountLivingNeighbours(row, col, grid);

				if (living)
				{
					if (maze)
					{
						nextGrid[row, col] = (neighbours >= 1 && neighbours <= 5);
					}
					else
					{
						nextGrid[row, col] = (neighbours == 2 || neighbours == 3);
					}
				}
				else
				{
					nextGrid[row, col] = neighbours == 3;
				}
				if (nextGrid[row, col] != grid[row, col])
				{
					golTexture.SetPixel(row, col, nextGrid[row, col] ? Color.white : Color.black);
				}
			}
		}
		golTexture.Apply();
		grid = nextGrid;
	}

	protected void RandomiseGrid(ref bool[,] grid)
	{
		for (int row = 0; row < width; row++)
		{
			for (int col = 0; col < height; col++)
			{
				grid[row, col] = Random.Range(0f, 1f) > .5f;
			}
		}
	}

	protected void Start()
	{
		float aspect = (float)width / (float)height;
		meshRenderer.transform.localScale = new Vector3(aspect, 1f);
		cam.orthographicSize = aspect;
		golTexture = new Texture2D(width, height);
		golTexture.filterMode = FilterMode.Point;
		textureRect = new Rect(Screen.width / 2 - width, Screen.height / 2 - height, width * 2, height * 2);
		grid = new bool[width, height];
		RandomiseGrid(ref grid);
		for (int row = 0; row < width; row++)
		{
			for (int col = 0; col < height; col++)
			{
				golTexture.SetPixel(row, col, grid[row, col] ? Color.white : Color.black);
			}
		}
	}

	protected void Update()
	{
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			NextGeneration(ref grid);
			timer = updateRate;
		}
	}

	public void Clear()
	{
		ClearGrid(ref grid);
	}

	public void Randomize()
	{
		RandomiseGrid(ref grid);
	}

	public void SetSpeed(float speed)
	{
		updateRate = speed;
	}

	public void SetIsMazeRule(bool isMaze)
	{
		maze = isMaze;
	}
}