namespace DATA
{
	class Chorus
	{
		public double high;	//コーラスの上の音
		public double mid;	//コーラスの真ん中の音
		public double low;	//コーラスの下の音

		public Chorus()
		{
			Reset();
		}

		public Chorus( double H, double M, double L )
		{
			Set( H, M, L );
		}

		public Chorus( double M )
		{
			Set( M );
		}

		public void Set( double M )
		{
			mid = M;
		}

		public void Set( double H, double M, double L )
		{
			high = H;
			mid = M;
			low = L;
		}

		public void Reset()
		{
			high = 0.0;
			mid = 0.0;
			low = 0.0;
		}

		public static Chorus operator+( Chorus L, Chorus R )
		{
			return new Chorus( L.high + R.high, L.mid + R.mid, L.low + R.low );
		}

		public static Chorus operator*( double L, Chorus R )
		{
			return new Chorus( L * R.high, L * R.mid, L * R.low );
		}

		public static Chorus operator*( Chorus L, double R )
		{
			return new Chorus( L.high * R, L.mid * R, L.low * R );
		}

		public static explicit operator double( Chorus HML ) {
			return HML.high + HML.mid + HML.low;
		}
	}
}