using Connect4.Game;
using Connect4.Rendering;

namespace Connect4.Game.Player
{
	public abstract class PlayerBase
	{
		public ColouredChar Sprite { get; set; }
		public ConsoleColor WinColour { get; set; }
		public string Name { get; set; }
		public int Score { get; set; } // !! endless mode only !!

		public virtual bool TakeTurn() => false;

		public virtual void Draw()
		{
		}
	}
}
