namespace DATA
{
	class Stereo
	{
		//標本化されたサンプルごとのステレオ音声を保持
		//WAVEファイルでは左→右の順に記録される
		public double Left;
		public double Right;

		public Stereo( double left, double right )
		{
			Left = left;
			Right = right;
		}
	}
}