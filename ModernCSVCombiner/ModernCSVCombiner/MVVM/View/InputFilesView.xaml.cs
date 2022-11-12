using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;


namespace ModernCSVCombiner.MVVM.View
{

    /// <summary>
    /// InputFilesView.xaml の相互作用ロジック
    /// </summary>
    public partial class InputFilesView : UserControl
    {
        public InputFilesView()
        {
            InitializeComponent();

            Button_FirstFlipper.Visibility = Visibility.Collapsed;
            Button_SecondFlipper.Visibility = Visibility.Collapsed;

            GetCountryData();

            EnableDragDrop(FirstFileDrop);
            EnableDragDrop(SecondFileDrop);
        }

        public static class Global////Global変数定義
        {
            public static string first_file_path = "path";
            public static string second_file_path = "path";
            public static bool first_file_exists_is = false;
            public static bool second_file_exists_is = false;
            public static List<string[]> CountryData = new List<string[]>();
            public static int FirstRC;
            public static int SecondRC;
            public static string OutputFileName = null;
            public static string FirstFileName;
            public static string SecondFileName;
        }

        public static class Constans
        {
            public const int COUNTRY_NUM = 238;
            public const int COUNTRY_NAME_COLUMN = 4;
            public const int COUNTRY_CODE_COLUMN = 1;
            public const String extension = ".csv";
        }

        public static void GetCountryData()
        {
            string path = "./data\\CountryData.csv";
            // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); // memo: Shift-JISを扱うためのおまじない
            StreamReader readCsvObject = new StreamReader(path, Encoding.GetEncoding("Shift-JIS"));
            while (!readCsvObject.EndOfStream)
            {
                var readCsvLine = readCsvObject.ReadLine();
                if (readCsvLine == null)
                {
                    ShowError();
                    return;
                }
                Global.CountryData.Add(readCsvLine.Split(','));
            }
        }

        private void EnableDragDrop(MaterialDesignThemes.Wpf.Flipper control)
        {
            control.AllowDrop = true;

            control.PreviewDragOver += (s, e) =>
            {
                e.Effects = (e.Data.GetDataPresent(DataFormats.FileDrop)) ? DragDropEffects.Copy : e.Effects = DragDropEffects.None;
                e.Handled = true;
            };

            control.PreviewDrop += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] paths = ((string[])e.Data.GetData(DataFormats.FileDrop));
                    string path = paths[0];

                    if (ConfirmFileRightness(path))
                    {
                        String[] SplitFilePath = path.Split('\\');
                        String FileName = SplitFilePath[SplitFilePath.Length - 1];
                        if (control.Name == "FirstFileDrop")
                        {
                            Global.first_file_path = path;
                            Global.first_file_exists_is = true;
                            TextBox_First_FilePath.Text = path;
                            Button_FirstFlipper.Visibility = Visibility.Visible;
                            TextBox_FirstFileName.Text = FileName;
                            Global.FirstFileName = FileName;
                        }
                        else if (control.Name == "SecondFileDrop")
                        {
                            Global.second_file_path = path;
                            Global.second_file_exists_is = true;
                            TextBox_SecondFilePath.Text = path;
                            Button_SecondFlipper.Visibility = Visibility.Visible;
                            TextBox_SecondFileName.Text = FileName;
                            Global.SecondFileName = FileName;
                        }
                        /*
                        if (Global.first_file_exists_is && Global.second_file_exists_is)
                        {
                            Button_Comb.Visibility = Visibility.Visible;
                        }
                        */
                    }
                }
            };
        }


        private static bool ConfirmFileRightness(string path)////指定されたファイルが適切か
        {
            if (path == Global.first_file_path || path == Global.second_file_path)//
            {
                MessageBox.Show("同じファイルが指定されています。");

                return false;
            }
            if (!(path.Substring(path.Length - 4) == ".csv"))//拡張子がcsvじゃない場合は不正
            {
                MessageBox.Show("指定されたファイルはCSVではありません。" +
                    "\nファイルの拡張子を確認してください。");

                return false;
            }


            return true;
        }

        private void First_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)////数字のみ入力されるように(全角には非対応)
        {
            var tmp = "";
            bool yes_parse;
            {
                // 既存のテキストボックス文字列に、
                // 今新規に一文字追加された時、その文字列が
                // 数値として意味があるかどうかをチェック
                {
                    tmp = FirstFileReferenceColumn.Text + e.Text;
                    yes_parse = Single.TryParse(tmp, out _);
                }
            }
            // 更新したい場合は false, 更新したくない場合は true
            // を返すべし。（混乱しやすいので注意！）
            int.TryParse(tmp, out int result);
            Global.FirstRC = result;
            e.Handled = !yes_parse;
        }
        private void Second_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)////数字のみ入力されるように(全角には非対応)
        {
            var tmp = "";
            bool yes_parse;
            {
                // 既存のテキストボックス文字列に、
                // 今新規に一文字追加された時、その文字列が
                // 数値として意味があるかどうかをチェック
                {
                    tmp = SecondFileReferenceColumn.Text + e.Text;
                    yes_parse = Single.TryParse(tmp, out _);
                }
            }
            // 更新したい場合は false, 更新したくない場合は true
            // を返すべし。（混乱しやすいので注意！）
            int.TryParse(tmp, out int result);
            Global.SecondRC = result;
            e.Handled = !yes_parse;
        }

        private void Button_Comb_Click(object sender, RoutedEventArgs e)
        {
            //ユーザが指定した列をint型で取得

            int first_specified_column = Global.FirstRC;
            int second_specified_column = Global.SecondRC;


            ///指定された２つのCSVのを二次元配列に格納
            int first_file_row_count = 0;
            int second_file_row_count = 0;
            int first_file_max_column = 0;
            int second_file_max_column = 0;
            List<string[]> first_file_contents = new List<string[]>();
            List<string[]> second_file_contents = new List<string[]>();
            String[] paths = {Global.first_file_path, Global.second_file_path};
            for(int times = 0; times < 2; times++)///二つのファイルの中身を二次元配列に格納
            {
                //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); // memo: Shift-JISを扱うためのおまじない
                StreamReader readCsvObject = new StreamReader(paths[times], Encoding.GetEncoding("Shift-JIS"));
                while (!readCsvObject.EndOfStream)
                {
                    var readCsvLine = readCsvObject.ReadLine();
                    if (readCsvLine == null)
                    {
                        ShowError();
                        return;
                    }
                    if (times == 0)
                    {
                        int columns = CountChar(readCsvLine, ',');
                        if (first_file_max_column < columns)
                        {
                            first_file_max_column = columns + 1;//列数は","の数+1
                        }

                        first_file_contents.Add(readCsvLine.Split(','));
                        first_file_row_count++;
                    }
                    if (times == 1)
                    {
                        int columns = CountChar(readCsvLine, ',');
                        if (second_file_max_column < columns)
                        {
                            second_file_max_column = columns + 1;//列数は","の数+1
                        }
                        second_file_contents.Add(readCsvLine.Split(','));
                        second_file_row_count++;
                    }
                }
            }

            ///一致している同士を結合する。
            int matched_times = 0;
            List<string[]> concatenated_contents = new List<string[]>();
            for(int i = 0; i < first_file_row_count; i++)
            {
                for(int j = 0; j < second_file_row_count; j++)
                {
                    if (first_file_contents[i][first_specified_column - 1] == second_file_contents[j][second_specified_column - 1])
                    {
                        concatenated_contents.Add(
                            first_file_contents[i].Concat(second_file_contents[j]).ToArray());

                        matched_times++;

                        break;
                    }
                }
            }

            if(matched_times == 0)
            {
                MessageBox.Show("合致する行が見つかりませんでした。" +
                    "指定列を再度確認してください。");
                return;
            }

            ///新しいCSVとして出力する
            List<string> output_contents = new List<string>();
            foreach (var line in concatenated_contents)
            {
                output_contents.Add(string.Join(",", line));
            }

            Export(output_contents);

            MessageBox.Show("結合が終了しました。");
        }

        private void Button_Ins_CN_Click(object sender, RoutedEventArgs e)
        {
            if(Global.first_file_exists_is == false)
            {
                MessageBox.Show("上のファイルボックスにCSVファイルをドラッグアンドドロップしてください。");
                return;
            }

            Insert_Column("CountryName", int.Parse(FirstFileReferenceColumn.Text));
        }
        
        private void Button_Ins_CC_Click(object sender, RoutedEventArgs e)
        {
            if(Global.first_file_exists_is == false)
            {
                MessageBox.Show("上のファイルボックスにCSVファイルをドラッグアンドドロップしてください。");
                return;
            }

            Insert_Column("CountryCode", int.Parse(FirstFileReferenceColumn.Text));
        }
        
        private static void Insert_Column(String object_to_add, int specified_column)
        {
            if(specified_column < 1)
            {
                MessageBox.Show("指定列数は1以上を設定してください");
                
                return;
            }

            Export(CreateInsertList(object_to_add, specified_column));


            MessageBox.Show(object_to_add + "列を追加しました。");
        }

        private static void Export(List<string> insert_list)
        {
            if (Global.OutputFileName == null)
            {
                Global.OutputFileName = "Combined "+  Global.FirstFileName + " and " + Global.SecondFileName;
            }
            using FileStream fs = File.Create("Combined " + Global.FirstFileName + " and " + Global.SecondFileName + ".csv");
            using StreamWriter sw = new StreamWriter(fs);
            foreach (var line in insert_list)
            {
                sw.WriteLine(line);
            }
        }
        
        private static List<string> CreateInsertList(String object_to_add,int specified_column){
            int adding_data = 0;
            int reference_data = 0;
            List<string> insert_list = new List<string>();

            if (object_to_add == "CountryName")
            {
                adding_data = Constans.COUNTRY_NAME_COLUMN;
                reference_data = Constans.COUNTRY_CODE_COLUMN;
            }
            else if(object_to_add == "CountryCode")
            {
                adding_data = Constans.COUNTRY_CODE_COLUMN;
                reference_data = Constans.COUNTRY_NAME_COLUMN;
            }
            
            List<string[]> Contents = new List<string[]>();
            int raw_count = 0;
            int max_column = 0;
            
            //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); // memo: Shift-JISを扱うためのおまじない
            if(Global.first_file_path == null)
            {
                ShowError();
                return insert_list;
            }
            using (StreamReader readCsvObject = new StreamReader(Global.first_file_path, Encoding.GetEncoding("Shift-JIS")))
            {
                while (!readCsvObject.EndOfStream)
                {
                    var readCsvLine = readCsvObject.ReadLine();
                    if (readCsvLine == null)
                    {
                        ShowError();
                        return insert_list;
                    }
                    int columns = CountChar(readCsvLine, ',');
                    
                    if (max_column < columns)
                    {
                        max_column = columns;
                    }
                    
                    readCsvLine += ",";
                    Contents.Add(readCsvLine.Split(','));
                    
                    raw_count++;
                }
            }
            
            for (int i = 0; i < raw_count; i++)
            {
                for (int j = 0; j < Constans.COUNTRY_NUM; j++)
                {
                    if (Contents[i][specified_column - 1] == Global.CountryData[j][reference_data])
                    {
                        String CountryName = Global.CountryData[j][adding_data];
                        Contents[i][max_column + 1] = CountryName;
                        
                        break;
                    }
                }
            }
            
            foreach (var line in Contents)
            {
                insert_list.Add(string.Join(",", line));
            }
            return insert_list;
        }
        
        private static int CountChar(string s, char c)
        {
            return s.Length - s.Replace(c.ToString(), "").Length;
        }

        static async void ShowError()
        {
            MessageBox.Show("重大なエラーです。作成者に連絡してください。" +
                            "\n【github】https://www.github.com/KinjiKawaguchi");
            await Task.Delay(1500);
            Application.Current.Shutdown();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void Flipper_OnIsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
    => System.Diagnostics.Debug.WriteLine($"Card is flipped = {e.NewValue}");

        private void TextBox_OutputName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Global.OutputFileName = TextBox_OutputName.Text;
        }
    }
}


