namespace DAW
{
	class Note
	{
		//音符
		public int		Key	{ get; set; } //音程
		public int		ST	{ get; set; } //音符の長さ
		public double	GT	{ get; set; } //発音時間
		public double	Vel	{ get; set; } //音量

		public Note()
		{

		}

		public Note(int key, int st, double gt, double vel)
		{
			Key = key;
			ST = st;
			GT = gt;
			Vel = vel;
		}
	}

	class Rest : Note
	{
		//休符
		public Rest(int st)
		{
			Key = -1;
			ST = st;
			GT = 0.0;
			Vel = 0.0;
		}
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

	public enum Tone
	{
		Sine,
		Square,
		Tri,
		Saw,
		Noise
	}
}