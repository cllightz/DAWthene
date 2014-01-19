using Algorithm;
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

			try
			{
				if (Arg[0] == "?")
				{
					Console.WriteLine("【フォーマット】");
					Console.WriteLine("1. 音色の変更");
					Console.WriteLine("\t以下のいずれかから選んでください。");
					Console.WriteLine("\tsine\t正弦波（デフォルト）");
					Console.WriteLine("\tsquare\t矩形波");
					Console.WriteLine("\ttri\t三角波");
					Console.WriteLine("\tsaw\tのこぎり波");
					Console.WriteLine();
					Console.WriteLine("2. テンポの変更");
					Console.WriteLine("\tt 小数\t数値は拍/分");
					Console.WriteLine();
					Console.WriteLine("3. ボリューム（大まかな音量の調整）の変更");
					Console.WriteLine("\tv 小数\tデフォルトは1");
					Console.WriteLine("\t\tトラックごとの音量の設定に使用するといいでしょう");
					Console.WriteLine();
					Console.WriteLine("4. エクスプレッション（音量の微調整）の変更");
					Console.WriteLine("\te 小数\tデフォルトは1");
					Console.WriteLine("\t\t曲中の音量の変化に使用するといいでしょう");
					Console.WriteLine();
					Console.WriteLine("5. ディレイ（音の遅れ具合）の変更");
					Console.WriteLine("\t未実装");
					Console.WriteLine();
					Console.WriteLine("6. コーラス（斉唱のように聞こえる具合）の変更");
					Console.WriteLine("\tcv 小数\tデフォルトは0");
					Console.WriteLine("\t\tコーラス部分の音量");
					Console.WriteLine("\tcw 小数\tデフォルトは0");
					Console.WriteLine("\t\tコーラス部分が本来の音の高さより上下に外れる具合");
					Console.WriteLine();
					Console.WriteLine("7. ビブラート（音の揺らぎ具合）の変更");
					Console.WriteLine("\tvd 小数\tデフォルトは0");
					Console.WriteLine("\t\tビブラートの幅（1で上下に1半音ずつ揺らぐ）");
					Console.WriteLine("\tvf 小数\tデフォルトは0");
					Console.WriteLine("\t\tビブラートの周波数");
					Console.WriteLine();
					Console.WriteLine("8. 音符の入力");
					Console.WriteLine("整数 |   整数   |    小数    | 小数");
					Console.WriteLine("音程 | 符の長さ | 発音の長さ | 音量");
					Console.WriteLine("音程:\t\t69が440Hz（A4・ラ）");
					Console.WriteLine("\t\t1大きくなると半音上に");
					Console.WriteLine("\t\t12大きくなると1オクターブ上に");
					Console.WriteLine("符の長さ:\t480が4分音符");
					Console.WriteLine("\t\t1920で1小節（4分の4拍子の場合）");
					Console.WriteLine("発音の長さ:\t0～100");
					Console.WriteLine("\t\t符の長さに対するパーセンテージ");
					Console.WriteLine("音量:\t\tその単一の音符の音量");
					Console.WriteLine("\t\tエクスプレッションよりも細かい調整に使用するといいでしょう");
					Console.WriteLine();
					Console.WriteLine("9. 休符の入力");
					Console.WriteLine("\tr 整数\t数値は8.の符の長さと同様");
					Console.WriteLine();
					Console.WriteLine("10. コメントアウトの入力");
					Console.WriteLine("\t// 文章\t文章をコンソール画面に出力します");
					Console.WriteLine();
					Console.WriteLine("大文字小文字の区別はありません。");
					Console.WriteLine("空行は無視されます。");

					return;
				}

				wave = new WAVE(Arg[0]);
			}
			catch (IndexOutOfRangeException)
			{
				Console.WriteLine("DAW.exeです。");
				Console.WriteLine("テキストファイルに保存された譜面から、音声ファイルを作成します。");
				Console.WriteLine("「daw ?」で、フォーマットの詳細を表示します。");
				Console.WriteLine("「daw sample」で、サンプル譜面の音声ファイルを作成します。");
				Console.WriteLine();
				Console.WriteLine("【使い方】");
				Console.WriteLine("1. 「曲名0.txt」にフォーマットに沿った譜面ファイルを入力して保存してください。※1");
				Console.WriteLine("2. トラック数に応じて、「曲名1.txt」や「曲名2.txt」を保存してください。 ※2");
				Console.WriteLine("3. このコンソール画面に「daw 曲名」と入力してください。");
				Console.WriteLine("4. エラーが出なかったら、コンソール画面に「曲名.wav」と入力してください。すると、Windows Media Playerで再生が始まるはずです。");
				Console.WriteLine("5. 聞き終えたらWindows Media Playerを終了させてください。（これを忘れると、同じファイルに上書きできません）");
				Console.WriteLine("※1 「notepad 曲名0.txt」で、メモ帳を開きます。保存時にエンコード形式を「UTF-8」にするのを絶対に忘れないでください。");
				Console.WriteLine("※2 0→1→2…の順に読み込まれ、前のものに上書きされていきます。音色やコーラスなどの設定は前のテキストファイルで最後に指定されたものを引き継ぎます。テンポも引き継がれますので、途中でテンポを変える時は気をつけて下さい。また、それぞれのトラックはモノシンセ（同時に1つの音しか発音できない）ですので、和音は複数のトラックに分けてください。");
				
				return;
			}
			catch (IOException)
			{
				Console.WriteLine("エクスプローラやWindows Media Playerで " + Arg[0] + ".wav が開かれています。閉じてからもう1回実行してください。");
			
				return;
			}

			for (int i=0; ; i++)
			{
				if (File.Exists(Arg[0] + i.ToString() + ".txt"))
				{
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine('"' + Arg[0] + i.ToString() + ".txt" + '"');
				}
				else if (i==0)
				{
					Console.WriteLine("入力ファイルが1つもありません。");
					break;
				}
				else
					break;

				var file = new StreamReader(Arg[0] + i.ToString() + ".txt", Encoding.UTF8);
				
				for (int loop=1; ; loop++)
				{
					var tmp = file.ReadLine();
					
					if (tmp==null)
						break;
					
					var str = ToString(tmp);

					if (str[0].Length==0)
						continue;

					try
					{
						switch (str[0])
						{
						case "//":
							Console.WriteLine(str[1]);
							break;
						case "r":
							wave.Rest(int.Parse(str[1]));
							break;
						case "t":
							wave.tempo = double.Parse(str[1]);
							Console.WriteLine("テンポ: " + str[1]);
							break;
						case "p":
							wave.panpot = double.Parse(str[1]);
							Console.WriteLine("パンポット: " + str[1]);
							break;
						case "v":
							wave.volume = double.Parse(str[1]);
							Console.WriteLine("ボリューム: " + str[1]);
							break;
						case "e":
							wave.expression = double.Parse(str[1]);
							Console.WriteLine("エクスプレッション: " + str[1]);
							break;
						case "dv":
							wave.delay_vol = double.Parse(str[1]);
							Console.WriteLine("ディレイ音量: " + str[1]);
							break;
						case "ds":
							wave.delay_spe = double.Parse(str[1]);
							Console.WriteLine("ディレイ速度: " + str[1]);
							break;
						case "cv":
							wave.chorus_vol = double.Parse(str[1]);
							Console.WriteLine("コーラス音量: " + str[1]);
							break;
						case "cw":
							wave.chorus_wid = double.Parse(str[1]);
							Console.WriteLine("コーラス幅: " + str[1]);
							break;
						case "vd":
							wave.vib_dep = double.Parse(str[1]);
							Console.WriteLine("ビブラート幅: " + str[1]);
							break;
						case "vf":
							wave.vib_fre = double.Parse(str[1]);
							Console.WriteLine("ビブラート速度: " + str[1]);
							break;
						case "sine":
							wave.tone = WAVE.Tone.Sine;
							Console.WriteLine("音色: 正弦波");
							break;
						case "square":
							wave.tone = WAVE.Tone.Square;
							Console.WriteLine("音色: 矩形波");
							break;
						case "tri":
							wave.tone = WAVE.Tone.Tri;
							Console.WriteLine("音色: 三角波");
							break;
						case "saw":
							wave.tone = WAVE.Tone.Saw;
							Console.WriteLine("音色: のこぎり波");
							break;
						case "noise":
							wave.tone = WAVE.Tone.Noise;
							Console.WriteLine("音色: ノイズ");
							break;
						default:
							int b;
							if (int.TryParse(str[0], out b))
							{
								wave.Add(b, int.Parse(str[1]), double.Parse(str[2]), double.Parse(str[3]));
								if (loop % 10 == 1)
									Console.Write(".");
							}
							else
							{
								Console.WriteLine("【エラー】以下の行で書式が間違っています。");
								Console.WriteLine(Arg[0] + i.ToString() + ".txt " + loop.ToString() + "行目: " + '"' + tmp + '"');
							}
							break;
						}
					}
					catch (FormatException)
					{
						Console.WriteLine();
						Console.WriteLine("【エラー】以下の行で書式が間違っています。");
						Console.WriteLine(Arg[0] + i.ToString() + ".txt " + loop.ToString() + "行目: " + '"' + tmp + '"');
						Console.WriteLine();
					}
				}
				
				wave.NewTrack();
			}

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("音声ファイルの作成が完了しました。");
			Console.WriteLine("「" + Arg[0] +".wav」で再生できます。");

			wave.Close();
		}

		private static string[] ToString(string Arg)
		{
			var str = Arg.ToCharArray();
			string[] res = { "", "", "", "" };

			for (int i=0, j=0; i<str.Length && j<4; i++)
			{
				if (str[i]=='\t' || str[i]==' ')
				{
					if (res[j]!="")
						j++;
				}
				else if (j==0)
					res[j] += char.ToLower(str[i]);
				else
					res[j] += str[i];
			}

			return res;
		}
	}
}