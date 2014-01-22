using DATA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAW
{
	class DAW
	{
		#region フィールド
		private static WAVE		wave;
		private static string	FileName;	//出力先ファイル名・入力元ファイル名(Arg[0])
		private static int		FileNum;	//入力元ファイルの番号
		private static int		LineNum;	//処理中の行
		private static string	LineStr;	//処理中の行の文字列
		private static int		errors;		//総エラー数
		#endregion

		public static void Main( string[] Arg )
		{
			#region 開始
			errors = 0;

			if ( Arg.Length == 0 ) { //"daw"
				Console.WriteLine( new StreamReader( "info.txt" ).ReadToEnd() );
				return;
			} else if ( Arg[0] == "?" ) { //"daw ?"
				Console.WriteLine( new StreamReader( "format.txt" ).ReadToEnd() );
				return;
			}

			FileName = Arg[0];

			try {
				//インスタンスの生成
				wave = new WAVE( FileName );
			} catch ( IOException ) {
				//出力先ファイルのオープン失敗
				Console.WriteLine( "エクスプローラやWindows Media Playerで " + FileName + ".wav が開かれています。" );
				Console.WriteLine( "閉じてからもう1回実行してください。" );
				return;
			}
			#endregion

			for ( FileNum = 0; ; ++FileNum ) {
				#region 入力元ファイルのオープン
				//入力元ファイルが存在するかをチェック
				if ( File.Exists( FileName + FileNum.ToString() + ".txt" ) ) {
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine( '"' + FileName + FileNum.ToString() + ".txt" + '"' );
				} else if ( FileNum == 0 ) {
					Console.WriteLine( "入力ファイルが1つもありません。" );
					break;
				} else {
					break;
				}

				//入力元ファイルをオープン
				var file = new StreamReader( FileName + FileNum.ToString() + ".txt", Encoding.UTF8 );
				#endregion

				for ( LineNum = 1; ( LineStr = file.ReadLine() ) != null; ++LineNum ) {
					var token = Tokenize( LineStr );

					try {
						switch ( token.Count ) {
							case 0:
								continue;

							case 1:
							case 2:
								Func( token );
								break;

							case 3:
								Error( "引数に過不足がありませんか？" );
								break;

							case 4:
								#region 音符の処理
								double b;

								if ( double.TryParse( token[0], out b ) ) {
									wave.Add( new Note( b, int.Parse( token[1] ), double.Parse( token[2] ), double.Parse( token[3] ) ) );

									if ( LineNum % 10 == 1 ) {
										Console.Write( "." );
									}
								} else {
									Error( "不明な記述: \"" + token[0] + "\"" );
								}
								#endregion
								break;

							default:
								Error( "引数が多すぎませんか？" );
								break;
						}
					} catch ( FormatException ) {
						Error( "不明なエラーです。" );
					}
				}

				wave.NewTrack();
			}

			#region 終了
			Console.WriteLine();
			Console.WriteLine();

			if ( errors == 0 ) {
				Console.WriteLine( "正常に楽譜データを読み込みました。" );
				Console.WriteLine( "音声ファイルに出力します。" );
			} else {
				Console.WriteLine( "エラーが" + errors.ToString() + "個ありましたが、音声ファイルに出力します。" );
			}

			wave.Close();

			Console.WriteLine();
			Console.WriteLine( "音声ファイルへの出力が完了しました。" );
			Console.WriteLine( "「" + FileName +".wav」で再生できます。" );
			Console.WriteLine();
			#endregion
		}

		private static void Func( List<string> Arg )
		{
			try {
				switch ( Arg[0] ) {
					//引数なし
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

					//引数1つ
					case "//":
						Console.WriteLine( Arg[1] );
						break;
					case "r":
						wave.Rest( int.Parse( Arg[1] ) );
						break;
					case "t":
						wave.tempo = double.Parse( Arg[1] );
						Console.WriteLine( "テンポ: " + Arg[1] );
						break;
					case "p":
						wave.panpot = double.Parse( Arg[1] );
						Console.WriteLine( "パンポット: " + Arg[1] );
						break;
					case "v":
						wave.volume = double.Parse( Arg[1] );
						Console.WriteLine( "ボリューム: " + Arg[1] );
						break;
					case "e":
						wave.expression = double.Parse( Arg[1] );
						Console.WriteLine( "エクスプレッション: " + Arg[1] );
						break;
					case "dv":
						wave.delay_vol = double.Parse( Arg[1] );
						Console.WriteLine( "ディレイ音量: " + Arg[1] );
						break;
					case "ds":
						wave.delay_tim = double.Parse( Arg[1] );
						Console.WriteLine( "ディレイ速度: " + Arg[1] );
						break;
					case "cv":
						wave.chorus_vol = double.Parse( Arg[1] );
						Console.WriteLine( "コーラス音量: " + Arg[1] );
						break;
					case "cw":
						wave.chorus_wid = double.Parse( Arg[1] );
						Console.WriteLine( "コーラス幅: " + Arg[1] );
						break;
					case "vd":
						wave.vib_dep = double.Parse( Arg[1] );
						Console.WriteLine( "ビブラート幅: " + Arg[1] );
						break;
					case "vf":
						wave.vib_fre = double.Parse( Arg[1] );
						Console.WriteLine( "ビブラート速度: " + Arg[1] );
						break;
				}
			} catch ( IndexOutOfRangeException ) {

			}
		}

		private static void Error( string Arg )
		{
			Console.WriteLine();
			Console.WriteLine( "【エラー】以下の行で不明な記述があります。" );
			Console.WriteLine( FileName + FileNum.ToString() + ".txt" );
			Console.WriteLine( LineNum.ToString() + "行目: \"" + LineStr + "\"" );
			Console.WriteLine();
			Console.WriteLine( Arg );
			Console.WriteLine();
			++errors;
		}

		private static List<string> Tokenize( string Arg )
		{
			//引数の文字列を小文字に統一し、トークン化
			var split = new List<string>( Arg.ToLower().Split( new char[] { ' ', '\t' } ) );
			var res = new List<string>();

			//空のトークンを無視して、戻り値のリストに追加
			foreach ( var i in split ) {
				if ( i != "" ) {
					res.Add( i );
				}
			}

			return res;
		}
	}
}