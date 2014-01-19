namespace DAW
{
	class Note
	{
		public double	Key	{ get; set; } //音程
		public int		ST	{ get; set; } //音符の長さ
		public double	GT	{ get; set; } //発音時間
		public double	Vel	{ get; set; } //音量

		public Note(double key, int st, double gt, double vel)
		{
			//音符
			Key = key;
			ST = st;
			GT = gt;
			Vel = vel;
		}
		
		public Note(int st)
		{
			//休符
			Key = -1;
			ST = st;
			GT = 0.0;
			Vel = 0.0;
		}
	}

	class Stereo
	{
		//標本化されたサンプルごとのステレオ音声を保持
		//WAVEファイルでは左→右の順に記録される
		public double Left;
		public double Right;

		public Stereo(double left, double right)
		{
			Left = left;
			Right = right;
		}
	}

	public enum Tone
	{
		Sine,	//正弦波
		Square,	//矩形波
		Tri,	//三角波
		Saw,	//ノコギリ波
		Noise	//ホワイトノイズ
	}
}