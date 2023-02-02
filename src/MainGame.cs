using Connect4.Game;
using Connect4.Game.Player;
using Connect4.Rendering;

namespace Connect4
{
	public class MainGame
	{
		public Board Board;
		public PlayerBase? CurrentPlayer;
		public PlayerBase[] Players;

		public bool EndlessMode { get; set; }

		private int tick = 0;
		public int Tick
		{
			get => tick;
			set
			{
				tick = value;
				CurrentPlayer = Players[Tick % Players.Length];
			}
		}

		public MainGame()
		{
			Board = new Board(7, 6);
			EndlessMode = false;
		}
		
		public void Update()
		{
			bool didTakeTurn = CurrentPlayer?.TakeTurn() ?? false;

			ColouredChar? c = CheckForWinCondition();

			if (c != null && c == CurrentPlayer?.Sprite)
			{
				if (!EndlessMode)
				{
					Program.PopupMessageColouredName($"[[NAME]] has won the game!", CurrentPlayer.Name, CurrentPlayer.Sprite.Colour, ConsoleColor.White, 10);
					Program.ProgramRunning = false;
				}
				else
				{
					Program.PopupMessageColouredName($"[[NAME]] has gained a point!", CurrentPlayer.Name, CurrentPlayer.Sprite.Colour, ConsoleColor.White, 10);
				}
			}

			if (didTakeTurn)
				Tick++;

			if (EndlessMode && (Board.Tokens.Count >= (Board.Width * (Board.Height - 1))))
			{
				PlayerBase? winnerPlayer = null;
				bool isDraw = false;
				int maxScore = -1;
				
				foreach (var p in Players)
				{
					if (p.Score > maxScore)
					{
						winnerPlayer = p;
						maxScore = p.Score;
					}
					else if (p.Score == maxScore)
					{
						isDraw = true;
					}
				}
				
				if (winnerPlayer != null)
				{
					if (isDraw)
					{
						Program.PopupMessage($"It is a draw :/", ConsoleColor.White);
					}
					else
					{
						Program.PopupMessage($"{winnerPlayer.Name} has won!", ConsoleColor.Green);
					}
					
					Program.ProgramRunning = false;
				}
			}
		}
		
		public void Draw()
		{
			Board.Draw();
			CurrentPlayer?.Draw();
			
			DrawInfo();

			if (EndlessMode)
				DrawScores();
		}

		private void DrawInfo()
		{
			Program.Renderer.PushImage(
				new TextImage()
					.DrawBox(Coordinates.ORIGIN, new Coordinates(50, 5), ConsoleColor.Gray)
					.DrawText("[SPACE] to place your token", ConsoleColor.White, new Coordinates(2, 1))
					.DrawText("[LEFTARROW] to move your token to left", ConsoleColor.White, new Coordinates(2, 2))
					.DrawText("[RIGHTARROW] to move your token to the right", ConsoleColor.White, new Coordinates(2, 3))
					.DrawText($"It is currently {CurrentPlayer?.Name}'s turn", ConsoleColor.White, new Coordinates(2, 4))
					.DrawText(CurrentPlayer.Name, CurrentPlayer.Sprite.Colour, new Coordinates(18, 4)),
				new Coordinates((Program.WINDOW_WIDTH - 50) / 2, 1),
				0.9f,
				true
			);
		}
		
		private void DrawScores()
		{
			TextImage scoreImg = new TextImage();
			scoreImg.DrawText($"{Players[0].Name}'s Score: {Players[0].Score}", ConsoleColor.Cyan, new Coordinates(0, 0));
			scoreImg.DrawText($"{Players[1].Name}'s Score: {Players[1].Score}", ConsoleColor.Cyan, new Coordinates(0, 1));
			
			Program.Renderer.PushImage(scoreImg, new Coordinates(2, Program.WINDOW_HEIGHT - 3), 0.9f, true);
		}

		private ColouredChar? CheckForWinCondition()
		{
			for (int y = 0; y < Board.Height; y++)
			{
				for (int x = 0; x < Board.Width; x++)
				{
					Token? here = Board.GetTokenAt(x, y);

					if (here == null)
						continue;
					
					for (int dx = -1; dx <= 1; dx++)
					{
						for (int dy = -1; dy <= 1; dy++)
						{
							if (dx == 0 && dy == 0)
								continue;
							
							bool fullFour = true;
							List<Coordinates> positions = new List<Coordinates>() { new Coordinates(x, y) };

							for (int i = 1; i <= 3; i++)
							{
								int extendedX = x + i * dx;
								int extendedY = y + i * dy;
								Token? here2 = Board.GetTokenAt(extendedX, extendedY);
								positions.Add(new Coordinates(extendedX, extendedY));

								if (here2 == null || here.Sprite != here2.Sprite)
								{
									fullFour = false;
									break;
								}
							}

							if (fullFour)
							{
								PlayerBase associatedPlayer = new RealPlayer();

								for (int i = 0; i < Players.Length; i++)
								{
									var p = Players[i];

									if (p.Sprite == here.Sprite)
									{
										associatedPlayer = p;
										associatedPlayer.Score++;
										break;
									}
								}
								
								foreach (var pos in positions)
								{
									Token winnerToken = new Token()
									{
										Sprite = new ColouredChar('*', associatedPlayer.WinColour),
										Coords = pos
									};

									Board.ReplaceTokenAtTokenPosition(winnerToken);
								}

								return here.Sprite;
							}
						}
					}
				}
			}

			return null;
		}
	}
}
