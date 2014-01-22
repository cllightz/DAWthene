using System;
using System.Collections.Generic;

namespace DATA
{
	class LinearPCM : List<Stereo>
	{
		private int pos; //現在処理中のサンプルの位置

		public LinearPCM()
		{
			pos = 0;
		}

		public void Marge( Stereo Arg )
		{
			if ( pos == Count ) {
				Add( Arg );
			} else {
				this[pos] += Arg;
			}

			Next();
		}

		public void Next()
		{
			if ( pos == Count ) {
				Add( new Stereo() );
			}

			++pos;
		}

		public void Rewind()
		{
			pos = 0;
		}
	}
}