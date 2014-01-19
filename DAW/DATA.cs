namespace DAW
{
	class Note
	{

	}

	class Stereo<T, U>
	{
		public Stereo(T left, U right)
		{
			this.Left = left;
			this.Right = right;
		}

		public T Left { get; set; }
		public U Right { get; set; }
	}
}