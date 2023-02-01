using Connect4.Game;
using Connect4.Rendering;

namespace Connect4.Game.Player
{
	public class AIPlayer : PlayerBase
	{
		public AIPlayer()
			: base()
		{
		}

		public override bool TakeTurn()
		{
			bool result;
			do { result = Program.Game.Board.TryPlaceToken(Program.Random.Next(0, Program.Game.Board.Width), Sprite);  } while (!result);
			return true;
		}
	}
}
