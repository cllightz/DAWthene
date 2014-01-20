namespace DATA
{
	class Note
	{
		class Note
		{
			public double Key { get; set; } //音程
			public int ST { get; set; } //音符の長さ
			public double GT { get; set; } //発音時間
			public double Vel { get; set; } //音量

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
	}
}