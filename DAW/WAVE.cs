using System;
using System.Collections.Generic;
using System.IO;

namespace DAW
{
	class WAVE
	{
		#region メンバ
		private	byte	Bytes;

		private	byte[]	H_RIFF;				//RIFFヘッダ
		private	uint	H_FileSize;			//ファイルサイズ[byte]
		private	byte[]	H_WAVE;				//WAVEヘッダ
		private	byte[]	H_fmt;				//fmtチャンク
		private	uint	H_fmt_Size	= 16;	//fmtチャンクのサイズ[byte]
		private	ushort	H_fmt_ID	= 1;	//フォーマットID
		private	ushort	H_fmt_Ch;			//チャンネル数
		private	uint	H_fmt_Sam;			//サンプリングレート[Hz]
		private	uint	H_fmt_BySe;			//データ速度(Sam*Blo)[Byte]
		private	ushort	H_fmt_Blo;			//ブロックサイズ(Bisa/8*Ch)[Byte]
		private	ushort	H_fmt_BiSa;			//サンプルごとのビット数
		private	byte[]	H_data;				//dataチャンク
		private	uint	H_data_Size;		//dataチャンクのサイズ[byte]

		private BinaryWriter file;		//出力先のファイル

		private List< Stereo > data;	//マージ先のデータ

		private int pos;				//現在処理中のサンプルの位置

		public Tone tone;			//音色(デフォルトはsine)
		public double tempo;		//テンポ(デフォルトは120BPM)
		public double panpot;		//パンポット(デフォルトは0)
		public double volume;		//ボリューム(デフォルトは1)
		public double expression;	//エクスプレッション(デフォルトは1)
		public double delay_vol;	//ディレイの音量(デフォルトは0)、未実装
		public double delay_tim;	//ディレイの時間(デフォルトは0)、未実装
		public double chorus_vol;	//コーラスの音量(デフォルトは0)
		public double chorus_wid;	//コーラスの豊かさ(デフォルトは0)
		public double vib_dep;		//ビブラートの深さ(デフォルトは0)
		public double vib_fre;		//ビブラートの周波数(デフォルトは0)

		private const double A = 6.875;	//なんの係数だっけ(忘れた)
		private const int root = -3;	//根音……？

		private double high;	//コーラスの上の音
		private double mid;		//コーラスの真ん中の音
		private double low;		//コーラスの下の音

		private int NoiseDone;		//ノイズ関連
		private double NoiseLast;	//前のサンプルでの量子
		#endregion

		public WAVE(string Arg)
		{
			Bytes = sizeof( short );	//16[bit]

			H_RIFF = new byte[4] { 0x52, 0x49, 0x46, 0x46 };	//"RIFF"
			H_FileSize = 0;
			H_WAVE = new byte[4] { 0x57, 0x41, 0x56, 0x45 };	//"WAVE"
			H_fmt = new byte[4] { 0x66, 0x6d, 0x74, 0x20 };		//"fmt "
			H_fmt_Ch = 2;										//ステレオ
			H_fmt_Sam = 44100;									//44100[Hz]
			H_fmt_BySe = (uint)(Bytes * H_fmt_Ch * H_fmt_Sam);
			H_fmt_Blo = (ushort)(Bytes * H_fmt_Ch);
			H_fmt_BiSa = (ushort)(Bytes * 8);
			H_data = new byte[4] { 0x64, 0x61, 0x74, 0x61 };	//"data"
			H_data_Size = 0;

			//出力先ファイルを開く
			file = new BinaryWriter( new FileStream( Arg + ".wav", FileMode.Create, FileAccess.Write ) );

			//マージ先のデータ
			data = new List< Stereo >();

			//現在処理中のサンプルの位置
			pos = 0;

			//音符関連
			tone = Tone.Sine;
			tempo = 120.0;
			panpot = 0;
			volume = 1;
			expression = 10;
			delay_vol = 0.0;
			delay_tim = 0.0;
			chorus_vol = 0.0;
			chorus_wid = 0.0;
			vib_dep = 0.0;
			vib_fre = 0.0;

			//コーラス関連
			high = 0.0;
			mid = 0.0;
			low = 0.0;

			//ノイズ関連
			NoiseDone = 0;
			NoiseLast = 0.0;
		}

		private void Marge(Stereo Arg)
		{
			if ( pos == data.Count )
				data.Add( Arg );
			else
			{
				data[pos].Left += Arg.Left;
				data[pos].Right += Arg.Right;
			}

			Next();
		}

		public void NewTrack()
		{
			pos = 0;
			high = 0.0;
			mid = 0.0;
			low = 0.0;
		}

		public void Add(Note Arg)
		{
			double Key = Arg.Key - root;

			Arg.GT = (Arg.GT > 100) ? 100 : Arg.GT;

			double x=0, y=0, z=0, vib=0, prev=0, left=0, center=0, right=0;

			for ( int i=0; i<GetGT( Arg.ST, Arg.GT ); ++i )
			{
				const double SiV=2.0, SqV=0.7, TrV=2.0, SaV=0.8, NoV=0.5;
				if ( tone == Tone.Sine )
				{
					high = Sine( x ) * SiV;
					mid = Sine( y ) * SiV;
					low = Sine( z ) * SiV;
				}
				else if ( tone == Tone.Square )
				{
					high = Square( x ) * SqV;
					mid = Square( y ) * SqV;
					low = Square( z ) * SqV;
				}
				else if ( tone == Tone.Tri )
				{
					high = Tri( x ) * TrV;
					mid = Tri( y ) * TrV;
					low = Tri( z ) * TrV;
				}
				else if ( tone == Tone.Saw )
				{
					high = Saw( x ) * SaV;
					mid = Saw( y ) * SaV;
					low = Saw( z ) * SaV;
				}
				else if ( tone == Tone.Noise )
				{
					high = 0.0;
					mid = Noise( Key ) / (short.MaxValue * 0.5) * NoV;
					low = 0.0;
				}

				high *= chorus_vol;
				low *= chorus_vol;

				center = (high + mid + low) * volume * expression * Arg.Vel;
				prev = center;

				left = center * (panpot - 100) * -1;
				right = center * (panpot + 100);
				Marge( new Stereo( left, right ) );

				x += Freq( Key, Math.Sin( vib ) * vib_dep + chorus_wid ) / (double)H_fmt_Sam * (Math.PI * 2.0);
				y += Freq( Key, Math.Sin( vib ) * vib_dep ) / (double)H_fmt_Sam * (Math.PI * 2.0);
				z += Freq( Key, Math.Sin( vib ) * vib_dep - chorus_wid ) / (double)H_fmt_Sam * (Math.PI * 2.0);

				vib += vib_fre * Math.PI*2.0 / (double)H_fmt_Sam;
			}

			for ( int i=0; i<(GetGT( Arg.ST, 100 ) - GetGT( Arg.ST, Arg.GT )); ++i )
				Next();
		}

		private double Freq(double Key, double Pitch)
		{
			return A * Math.Pow( 2.0, (Key + Pitch) / 12.0 );
		}

		public void Rest(int ST)
		{
			for ( int i=0; i<GetGT( ST, 100 ); ++i )
				Next();
		}

		private double GetGT(int ST, double GT)
		{
			return ((double)ST * GT * (double)H_fmt_Sam) / (100.0 * 480.0 * (tempo / 60.0));
		}

		private double Sine(double Arg)
		{
			return Math.Sin( Arg );
		}

		private double Square(double Arg)
		{
			return ((Math.Sin( Arg ) >= 0.0) ? 1.0 : -1.0);
		}

		private double Tri(double Arg)
		{
			double a = Arg - Math.Floor( Arg / (Math.PI * 2.0) ) * Math.PI * 2.0;

			if ( a < Math.PI * 0.5 )
				return a / (Math.PI * 0.5);

			if ( a < Math.PI * 1.5 )
				return -1 * a / (Math.PI * 0.5) + 2.0;

			return a / (Math.PI * 0.5) - 4.0;
		}

		private double Saw(double Arg)
		{
			double a = Arg - Math.Floor( Arg / (Math.PI * 2.0) ) * Math.PI * 2.0;

			return (a / (Math.PI * 2.0) - 0.5) * 2.0;
		}

		private double Noise(double Key)
		{
			if ( Key==0 || NoiseDone%Key==0 )
			{
				byte[] a = new byte[1];
				System.Security.Cryptography.RNGCryptoServiceProvider b = new System.Security.Cryptography.RNGCryptoServiceProvider();
				b.GetBytes( a );
				++NoiseDone;
				return (100.0 * (double)Convert.ToInt16( a[0] ) - short.MaxValue * 0.375);
			}

			++NoiseDone;
			return NoiseLast;
		}

		private void Next()
		{
			if ( pos == data.Count )
			{
				data.Add( new Stereo( 0.0, 0.0 ) );
				++pos;
			}
			else
			{
				++pos;
			}

			high = 0.0;
			mid = 0.0;
			low = 0.0;
		}

		public void Close()
		{
			H_data_Size = (uint)((data.Count) * H_fmt_Ch * Bytes);
			H_FileSize = H_data_Size + H_fmt_Size + 20;

			//			Console.WriteLine(H_FileSize.ToString());
			//			Console.WriteLine(H_data_Size.ToString());

			file.Write( H_RIFF );
			file.Write( H_FileSize );
			file.Write( H_WAVE );
			file.Write( H_fmt );
			file.Write( H_fmt_Size );
			file.Write( H_fmt_ID );
			file.Write( H_fmt_Ch );
			file.Write( H_fmt_Sam );
			file.Write( H_fmt_BySe );
			file.Write( H_fmt_Blo );
			file.Write( H_fmt_BiSa );
			file.Write( H_data );
			file.Write( H_data_Size );

			foreach ( var i in data )
			{
				Write( i.Left );
				Write( i.Right );
			}
		}

		private void Write(double Arg)
		{
			short New;

			if ( Arg > short.MaxValue )
			{
				//				Console.WriteLine(Arg.ToString());
				New = short.MaxValue;
			}
			else if ( Arg < short.MinValue )
			{
				//				Console.WriteLine(Arg.ToString());
				New = short.MinValue;
			}
			else
				New = (short)Arg;

			file.Write( New );
		}
	}
}
