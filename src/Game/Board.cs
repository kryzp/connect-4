using Connect4.Rendering;

namespace Connect4.Game
{
	public class Board
	{
		private List<Token> tokens;

		public int Width { get; private set; }
		public int Height { get; private set; }
		
		public List<Token> Tokens => tokens;

		public Coordinates DrawCoords => new Coordinates((Program.WINDOW_WIDTH - 2*Width - 2) / 2, (Program.WINDOW_HEIGHT - Height) / 2);
		
		public Board(int width, int height)
		{
			this.Width = width;
			this.Height = height + 1;

			this.tokens = new List<Token>();
		}

		public int HeightUntilToken(int column)
		{
			for (int i = Height - 2; i >= 0; i--)
			{
				bool freeSpot = true;
				
				foreach (var tok in tokens)
				{
					if (tok.Coords == new Coordinates(column, i))
					{
						freeSpot = false;
						break;
					}
				}

				if (freeSpot)
					return i;
			}

			return -1;
		}

		public bool TryPlaceToken(int column, ColouredChar sprite)
		{
			int row = HeightUntilToken(column);

			if (row < 0)
				return false;
			
			Token tk = new Token()
			{
				Sprite = sprite,
				Coords = new Coordinates(column, row)
			};

			tokens.Add(tk);
			return true;
		}
		
		public void Draw()
		{
			TextImage wrapper = GetBoardWrapper();
			TextImage boardImg = GetBoardImage();

			boardImg.DrawTo(wrapper, new Coordinates(2, 3));

			Program.Renderer.PushImage(wrapper, DrawCoords, 0.25f, true);
		}

		private TextImage GetBoardWrapper()
		{
			TextImage ret = new TextImage(2*Width + 2, Height + 2);

			ret.DrawBox(new Coordinates(0, 1), new Coordinates((Width + 1) * 2, Height + 2), ConsoleColor.Gray);
			ret.DrawBox(Coordinates.ORIGIN, new Coordinates(Width * 2 + 2, 2), ConsoleColor.Gray);
			ret.DrawLine(new Coordinates(1, 1), new Coordinates((Width + 1) * 2, 1), new ColouredChar(' ', ConsoleColor.Black));

			for (int i = 0; i < Program.Game.Board.Width; i++)
				ret.DrawChar(new Coordinates(2 * (1 + i), 1), new ColouredChar((i + 1).ToString()[0], ConsoleColor.Cyan));

			ret.DrawChar(new Coordinates(0, 2), new ColouredChar(Characters.SPLIT_RIGHT, ConsoleColor.Gray));
			ret.DrawChar(new Coordinates((Width + 1) * 2, 2), new ColouredChar(Characters.SPLIT_LEFT, ConsoleColor.Gray));

			return ret;
		}

		public Token? GetTokenAt(int x, int y)
		{
			if (x < 0 || y < 0 || x >= Width || y >= Height)
				return null;
			
			foreach (var t in Tokens)
			{
				if (t.Coords == new Coordinates(x, y))
					return t;
			}

			return null;
		}

		public void ReplaceTokenAtTokenPosition(Token token)
		{
			for (int i = 0; i < tokens.Count; i++)
			{
				if (tokens[i].Coords == token.Coords)
				{
					tokens[i] = token;
					return;
				}
			}
		}

		private TextImage GetBoardImage()
		{
			TextImage boardImage = new TextImage(2*Width, Height + 1);

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					Token? here = GetTokenAt(x, y);

					if (here != null)
					{
						boardImage.DrawChar(new Coordinates(2*x, y), here.Sprite);
					}
				}
			}

			return boardImage;
		}
	}
}
