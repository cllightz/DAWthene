using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAW
{
	class DAW
	{
		public static void Main(string[] Arg)
		{
			WAVE wave;

			if ( Arg.Length == 0 ) {
				Console.WriteLine( new StreamReader( "info.txt" ).ReadToEnd() );
				return;
			} else if ( Arg[0] == "?" ) {
				Console.WriteLine( new StreamReader( "format.txt" ).ReadToEnd() );
				return;
			}

			try {
				//インスタンスの生成
				wave = new WAVE( Arg[0] );
			} catch ( IOException ) {
				//出力先ファイルのオープン失敗
				Console.WriteLine( "エクスプローラやWindows Media Playerで " + Arg[0] + ".wav が開かれています。閉じてからもう1回実行してください。" );
				return;
			}

			for ( int i=0; ; ++i ) {
				if ( File.Exists( Arg[0] + i.ToString() + ".txt" ) ) {
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine( '"' + Arg[0] + i.ToString() + ".txt" + '"' );
				} else if ( i==0 ) {
					Console.WriteLine( "入力ファイルが1つもありません。" );
					break;
				} else
					break;

				var file = new StreamReader( Arg[0] + i.ToString() + ".txt", Encoding.UTF8 );

				for ( int loop = 1; ; ++loop ) {
					var tmp = file.ReadLine();

					if ( tmp == null )
						break;

					var str = ToString( tmp );

					if ( str[0].Length == 0 )
						continue;

					try {
						switch ( str[0] ) {
							case "//":
								Console.WriteLine( str[1] );
								break;
							case "r":
								wave.Rest( int.Parse( str[1] ) );
								break;
							case "t":
								wave.tempo = double.Parse( str[1] );
								Console.WriteLine( "テンポ: " + str[1] );
								break;
							case "p":
								wave.panpot = double.Parse( str[1] );
								Console.WriteLine( "パンポット: " + str[1] );
								break;
							case "v":
								wave.volume = double.Parse( str[1] );
								Console.WriteLine( "ボリューム: " + str[1] );
								break;
							case "e":
								wave.expression = double.Parse( str[1] );
								Console.WriteLine( "エクスプレッション: " + str[1] );
								break;
							case "dv":
								wave.delay_vol = double.Parse( str[1] );
								Console.WriteLine( "ディレイ音量: " + str[1] );
								break;
							case "ds":
								wave.delay_tim = double.Parse( str[1] );
								Console.WriteLine( "ディレイ速度: " + str[1] );
								break;
							case "cv":
								wave.chorus_vol = double.Parse( str[1] );
								Console.WriteLine( "コーラス音量: " + str[1] );
								break;
							case "cw":
								wave.chorus_wid = double.Parse( str[1] );
								Console.WriteLine( "コーラス幅: " + str[1] );
								break;
							case "vd":
								wave.vib_dep = double.Parse( str[1] );
								Console.WriteLine( "ビブラート幅: " + str[1] );
								break;
							case "vf":
								wave.vib_fre = double.Parse( str[1] );
								Console.WriteLine( "ビブラート速度: " + str[1] );
								break;
							case "sine":
								wave.tone = Tone.Sine;
								Console.WriteLine( "音色: 正弦波" );
								break;
							case "square":
								wave.tone = Tone.Square;
								Console.WriteLine( "音色: 矩形波" );
								break;
							case "tri":
								wave.tone = Tone.Tri;
								Console.WriteLine( "音色: 三角波" );
								break;
							case "saw":
								wave.tone = Tone.Saw;
								Console.WriteLine( "音色: のこぎり波" );
								break;
							case "noise":
								wave.tone = Tone.Noise;
								Console.WriteLine( "音色: ノイズ" );
								break;
							default:
								double b;
								if ( double.TryParse( str[0], out b ) ) {
									wave.Add( new Note( b, int.Parse( str[1] ), double.Parse( str[2] ), double.Parse( str[3] ) ) );
									if ( loop % 10 == 1 )
										Console.Write( "." );
								} else {
									Console.WriteLine( "【エラー】以下の行で書式が間違っています。" );
									Console.WriteLine( Arg[0] + i.ToString() + ".txt " + loop.ToString() + "行目: " + '"' + tmp + '"' );
								}
								break;
						}
					} catch ( FormatException ) {
						Console.WriteLine();
						Console.WriteLine( "【エラー】以下の行で書式が間違っています。" );
						Console.WriteLine( Arg[0] + i.ToString() + ".txt " + loop.ToString() + "行目: " + '"' + tmp + '"' );
						Console.WriteLine();
					}
				}

				wave.NewTrack();
			}

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine( "音声ファイルの作成が完了しました。" );
			Console.WriteLine( "「" + Arg[0] +".wav」で再生できます。" );

			wave.Close();
		}

		private static string[] ToString(string Arg)
		{
			var str = Arg.ToCharArray();
			string[] res = { "", "", "", "" };

			for ( int i=0, j=0; i<str.Length && j<4; ++i ) {
				if ( str[i]=='\t' || str[i]==' ' ) {
					if ( res[j]!="" )
						++j;
				} else if ( j==0 )
					res[j] += char.ToLower( str[i] );
				else
					res[j] += str[i];
			}

			return res;
		}
	}
}