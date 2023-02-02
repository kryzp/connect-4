using Connect4.Rendering;

namespace Connect4.Game.Player
{
	public class RealPlayer : PlayerBase
	{
		private int currCol = 0;
		
		public RealPlayer()
			: base()
		{
		}

		public override bool TakeTurn()
		{
			if (Input.IsPressed(ConsoleKey.Spacebar))
				return Program.Game.Board.TryPlaceToken(currCol, Sprite);

			if (Input.IsPressed(ConsoleKey.LeftArrow))
				currCol--;

			if (Input.IsPressed(ConsoleKey.RightArrow))
				currCol++;

			currCol = Math.Clamp(currCol, 0, Program.Game.Board.Width - 1);
			
			return false;
		}

		public override void Draw()
		{
			base.Draw();

			int h = Program.Game.Board.HeightUntilToken(currCol);

			TextImage selectionBeam = new TextImage();
			
			selectionBeam.DrawLine(
				Coordinates.ORIGIN, new Coordinates(0, h), new ColouredChar('|', ConsoleColor.Yellow)
			);

			selectionBeam.DrawChar(Coordinates.ORIGIN, new ColouredChar('V', ConsoleColor.Yellow));
			selectionBeam.DrawChar(new Coordinates(0, h), new ColouredChar('+', ConsoleColor.Yellow));

			Program.Renderer.PushImage(
				selectionBeam, Program.Game.Board.DrawCoords + new Coordinates(currCol*2 + 2, 3), 1.2f, true
			);
		}
	}
}
