namespace DATA
{
	class Stereo
	{
		//標本化されたサンプルごとのステレオ音声を保持
		//WAVEファイルでは左→右の順に記録される
		public double Left;
		public double Right;

		public Stereo()
		{
			Left = 0;
			Right = 0;
		}

		public Stereo( double left, double right )
		{
			Left = left;
			Right = right;
		}

		public static Stereo operator+( Stereo L, Stereo R ) {
			return new Stereo( L.Left + R.Left, L.Right + R.Right );
		}

		/*
		public static Stereo operator*( double L, Stereo R )
		{
			return new Stereo( L * R.Left, L * R.Right );
		}

		public static Stereo operator*( Stereo L, double R )
		{
			return new Stereo( L.Left * R, L.Right * R );
		}
		*/
	}
}