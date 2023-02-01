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
		private static InputBox nameInputBox;
		private static InputBox tokenSymbolInputBox;
		private static InputBox tokenColourInputBox;

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
					    nameInputBox.Text.Length <= 0 ||
					    tokenSymbolInputBox.Text.Length <= 0 ||
					    tokenColourInputBox.Text.Length <= 0)
					{
						PopupError("One of the input boxes is left empty!");
						return;
					}
					
					Game.EndlessMode = endlessModeInputBox.Text.ToUpper() == "Y";
					
					ConsoleColor playerColour = tokenColourInputBox.Text.ToUpper()[0] switch
					{
						'B' => ConsoleColor.Blue,
						'G' => ConsoleColor.Green,
						'M' => ConsoleColor.Magenta,
						'C' => ConsoleColor.Cyan,
						_ => ConsoleColor.Blue
					};
					
					Game.Players[0].Name = nameInputBox.Text;
					Game.Players[0].Sprite = new ColouredChar(tokenSymbolInputBox.Text[0], playerColour);
					Game.Players[0].WinColour = DarkerColour(playerColour);
					
					Game.Players[1].Name = "AI Player";
					Game.Players[1].Sprite = new ColouredChar('X', ConsoleColor.Red);
					Game.Players[1].WinColour = ConsoleColor.DarkRed;
					
					curState = GameState.PLAYING;
					firstFrame = true;
				})
				{
					ID = 0,
					DownID = 1
				});

				endlessModeInputBox = new InputBox(new Coordinates(1, 5), (WINDOW_WIDTH - 2, 4), "Play on endless mode? (Y/n)")
				{
					ID = 1,
					UpID = 0,
					DownID = 2
				};
				
				nameInputBox = new InputBox(new Coordinates(1, 10), (WINDOW_WIDTH - 2, 4), "Player Name")
				{
					ID = 2,
					UpID = 1,
					DownID = 3
				};
				
				tokenSymbolInputBox = new InputBox(new Coordinates(1, 15), (WINDOW_WIDTH - 2, 4), "Token Symbol")
				{
					ID = 3,
					UpID = 2,
					DownID = 4
				};
				
				tokenColourInputBox = new InputBox(new Coordinates(1, 20), (WINDOW_WIDTH - 2, 4), "Token Colour: [B]lue, [G]reen, [M]agenta, [C]yan")
				{
					ID = 4,
					UpID = 3,
				};
				
				startMenu.Add(endlessModeInputBox);
				startMenu.Add(nameInputBox);
				startMenu.Add(tokenSymbolInputBox);
				startMenu.Add(tokenColourInputBox);
				
				startMenu.SetSelectedElement(0);
			}

			Game.Tick = 0;
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
