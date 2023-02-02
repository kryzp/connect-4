using Connect4.Game.Player;
using Connect4.Rendering;
using Connect4.UI;
using Connect4.UI.Elements;

namespace Connect4
{
	public static class Program
	{
		private enum GameState
		{
			START,
			PLAYING,
		}

		private static GameState curState;
		
		private static Menu startMenu;
		private static InputBox endlessModeInputBox;
		
		private static InputBox player1NameInputBox;
		private static InputBox player1TokenSymbolInputBox;
		private static InputBox player1ColourInputBox;
		
		private static InputBox player2NameInputBox;
		private static InputBox player2TokenSymbolInputBox;
		private static InputBox player2ColourInputBox;

		private static InputBox shouldP2beAI;

		public static bool ProgramRunning = true;
		
		public const int WINDOW_WIDTH = 64;
		public const int WINDOW_HEIGHT = 32;
		
		public static Renderer Renderer;
		public static Random Random;

		public static MainGame Game;

		private static bool firstFrame = false;
		
		public static string PopupPrompt(string prompt)
		{
			var coords = new Coordinates(
				(WINDOW_WIDTH / 2) - 2 - (prompt.Length / 2),
				(WINDOW_HEIGHT / 2) - 2
			);
			
			Renderer.PushImage(new TextImage()
					.DrawBox(
						Coordinates.ORIGIN, 
						new Coordinates(prompt.Length + 3, 3),
						ConsoleColor.DarkGray
					).DrawText(
						prompt, ConsoleColor.Cyan,
						new Coordinates(2, 1)
					),
				coords, 10, true
			);
			
			Renderer.Render();
			
			Console.SetCursorPosition(coords.X + 2, coords.Y + 2);
			Console.Write("> ");
			return Console.ReadLine() ?? "";
		}

		public static void PopupMessage(string prompt, ConsoleColor col = ConsoleColor.Cyan, int yoffset = 0)
		{
			var coords = new Coordinates(
				(WINDOW_WIDTH / 2) - 2 - (prompt.Length / 2),
				(WINDOW_HEIGHT / 2) - 2 + yoffset
			);
			
			Renderer.PushImage(new TextImage()
					.DrawBox(
						new Coordinates(0, 0), new Coordinates(prompt.Length + 3, 2),
						ConsoleColor.DarkGray
					).DrawText(
						prompt, col, new Coordinates(2, 1)
					),
				coords, 10, true
			);
			
			Renderer.Render();
			Console.ReadKey();
		}
		
		public static void PopupMessageColouredName(string prompt, string name, ConsoleColor nameColour, ConsoleColor col = ConsoleColor.Cyan, int yoffset = 0)
		{
			var coords = new Coordinates(
				(WINDOW_WIDTH / 2) - 2 - (prompt.Length / 2),
				(WINDOW_HEIGHT / 2) - 2 + yoffset
			);

			prompt = prompt.Replace("[[NAME]]", name);
			
			Renderer.PushImage(new TextImage()
					.DrawBox(
						new Coordinates(0, 0), new Coordinates(prompt.Length + 3, 2),
						ConsoleColor.DarkGray
					).DrawText(
						prompt, col, new Coordinates(2, 1)
					).DrawText(
						name, nameColour, new Coordinates(2 + prompt.IndexOf(name), 1)
					),
				coords, 10, true
			);

			Renderer.Render();
			Console.ReadKey();
		}

		public static void PopupError(string error)
		{ 
			PopupMessage(error, ConsoleColor.Red);
		}

		private static ConsoleColor DarkerColour(ConsoleColor lighter)
		{
			return lighter switch
			{
				ConsoleColor.Blue => ConsoleColor.DarkBlue,
				ConsoleColor.Cyan => ConsoleColor.DarkCyan,
				ConsoleColor.Gray => ConsoleColor.DarkGray,
				ConsoleColor.Magenta => ConsoleColor.DarkMagenta,
				ConsoleColor.Red => ConsoleColor.DarkRed,
				ConsoleColor.Green => ConsoleColor.DarkGreen,
				_ => ConsoleColor.Black
			};
		}
		
		public static void Main()
		{
			Renderer = new Renderer();
			Random = new Random();
			Game = new MainGame();
			
			curState = GameState.START;

			startMenu = new Menu();
			{
				startMenu.Add(new Button(new Coordinates(1, 1), "Begin Game!", () =>
				{
					if (endlessModeInputBox.Text.Length <= 0 ||
					    player1NameInputBox.Text.Length <= 0 ||
					    player1TokenSymbolInputBox.Text.Length <= 0 ||
					    player1ColourInputBox.Text.Length <= 0 ||
					    player2NameInputBox.Text.Length <= 0 ||
					    player2TokenSymbolInputBox.Text.Length <= 0 ||
					    player2ColourInputBox.Text.Length <= 0 ||
					    shouldP2beAI.Text.Length <= 0)
					{
						PopupError("One of the input boxes is left empty!");
						return;
					}
					
					Game.EndlessMode = endlessModeInputBox.Text.ToUpper()[0] == 'Y';
					
					ConsoleColor player1Colour = player1ColourInputBox.Text.ToUpper()[0] switch
					{
						'B' => ConsoleColor.Blue,
						'G' => ConsoleColor.Green,
						_ => ConsoleColor.Blue
					};
					
					ConsoleColor player2Colour = player2ColourInputBox.Text.ToUpper()[0] switch
					{
						'R' => ConsoleColor.Red,
						'M' => ConsoleColor.Magenta,
						_ => ConsoleColor.Red
					};
					
					Game.Players = new PlayerBase[2]
					{
						new RealPlayer(),
						(shouldP2beAI.Text.ToUpper()[0] == 'Y') ? new AIPlayer() : new RealPlayer()
					};
					
					Game.Players[0].Name = player1NameInputBox.Text;
					Game.Players[0].Sprite = new ColouredChar(player1TokenSymbolInputBox.Text[0], player1Colour);
					Game.Players[0].WinColour = DarkerColour(player1Colour);
					
					Game.Players[1].Name = player2NameInputBox.Text;
					Game.Players[1].Sprite = new ColouredChar(player2TokenSymbolInputBox.Text[0], player2Colour);
					Game.Players[1].WinColour = DarkerColour(player2Colour);
					
					curState = GameState.PLAYING;
					firstFrame = true;

					Game.Tick = 0;
				})
				{
					ID = 0,
					DownID = 1
				});

				endlessModeInputBox = new InputBox(new Coordinates(1, 4), (WINDOW_WIDTH - 2, 4), "Play on endless mode? (Y/n)")
				{
					ID = 1,
					UpID = 0,
					DownID = 2
				};
				
				player1NameInputBox = new InputBox(new Coordinates(1, 8), ((WINDOW_WIDTH - 2) / 2, 4), "Player 1 Name")
				{
					ID = 2,
					UpID = 1,
					DownID = 4,
					RightID = 3
				};
				
				player2NameInputBox = new InputBox(new Coordinates(WINDOW_WIDTH / 2, 8), ((WINDOW_WIDTH - 2) / 2, 4), "Player 2 Name")
				{
					ID = 3,
					UpID = 2,
					DownID = 5,
					LeftID = 2
				};
				
				player1TokenSymbolInputBox = new InputBox(new Coordinates(1, 12), ((WINDOW_WIDTH - 2) / 2, 4), "Player 1 Token Symbol")
				{
					ID = 4,
					UpID = 2,
					DownID = 6,
					RightID = 5
				};
				
				player2TokenSymbolInputBox = new InputBox(new Coordinates(WINDOW_WIDTH / 2, 12), ((WINDOW_WIDTH - 2) / 2, 4), "Player 2 Token Symbol")
				{
					ID = 5,
					UpID = 3,
					DownID = 6,
					LeftID = 4
				};
				
				player1ColourInputBox = new InputBox(new Coordinates(1, 16), (WINDOW_WIDTH - 2, 4), "Player 1 Token Colour: [B]lue, [G]reen")
				{
					ID = 6,
					UpID = 4,
					DownID = 7,
				};
				
				player2ColourInputBox = new InputBox(new Coordinates(1, 20), (WINDOW_WIDTH - 2, 4), "Player 2 Token Colour: [R]ed, [M]agenta")
				{
					ID = 7,
					UpID = 6,
				};
				
				player2ColourInputBox = new InputBox(new Coordinates(1, 20), (WINDOW_WIDTH - 2, 4), "Player 2 Token Colour: [R]ed, [M]agenta")
				{
					ID = 7,
					UpID = 6,
					DownID = 8
				};

				shouldP2beAI = new InputBox(new Coordinates(1, 24), (WINDOW_WIDTH - 2, 4), "Should player 2 be an AI? (Y/n)")
				{
					ID = 8,
					UpID = 7
				};
				
				startMenu.Add(endlessModeInputBox);
				
				startMenu.Add(player1NameInputBox);
				startMenu.Add(player1TokenSymbolInputBox);
				startMenu.Add(player1ColourInputBox);
				
				startMenu.Add(player2NameInputBox);
				startMenu.Add(player2TokenSymbolInputBox);
				startMenu.Add(player2ColourInputBox);

				startMenu.Add(shouldP2beAI);
				
				startMenu.SetSelectedElement(0);
			}

			firstFrame = true;
			
			while (ProgramRunning)
			{
				if (!firstFrame)
					Input.PollNewKey();
				
				firstFrame = false;
				
				switch (curState)
				{
					case GameState.START:
						startMenu.Update();
						startMenu.Draw();
						Renderer.PushImage(new TextImage().DrawText("Use the arrow keys to move around the menu!", ConsoleColor.Cyan), new Coordinates(2, WINDOW_HEIGHT - 3), 0.9f, true);
						Renderer.PushImage(new TextImage().DrawText("Use the enter key to press the begin game button!", ConsoleColor.Cyan), new Coordinates(2, WINDOW_HEIGHT - 2), 0.9f, true);
						break;
					
					case GameState.PLAYING:
						Game.Update();
						Game.Draw();
						break;
				}
				
				Renderer.Clear();
				Renderer.Render();

				Console.SetCursorPosition(0, 0);
			}

			Renderer.Clear();
		}
	}
}
