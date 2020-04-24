using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleApp;
using RichConsole.Argument;

namespace RichConsole
{
    public static class Console
    {
        private static IConsoleApp consoleApp;
        private static bool IsInitialized = false;
        static Console()
        {
            if (!IsInitialized) Initialize();
            IsInitialized = true;
        }
        [STAThread]
        public static void Initialize()
        {
            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            */
            ConsoleApp.ConsoleManager f = new ConsoleApp.ConsoleManager();
            Thread thread = new Thread(() =>
            {
                Application.Run(f);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();


            while (!f.Initialized) Thread.Sleep(25);

            consoleApp = f;
            return;
        }


        private static string Half2Full(string sHalf)
        {
            char[] ch = sHalf.ToCharArray(0, sHalf.Length);
            for (int i = 0; i < sHalf.Length; ++i)
            {
                if (ch[i] > 0x21 && ch[i] <= 0x7e)
                    ch[i] += (char)0xfee0;
                else if (ch[i] == 0x20)
                    ch[i] = (char)0x3000;
            }
            return (new string(ch));
        }
        private static bool 받침확인(char c)
        {
            if (c < 0xAC00 || c > 0xD7A3)
            {
                return false;
            }
            return (c - 0xAC00) % 28 > 0;
        }

        internal static string FixString(string origin)
        {
            StringBuilder sb = new StringBuilder(origin.Replace("을/를", SystemWords[0].ToString()).Replace("이/가", SystemWords[1].ToString()));
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == SystemWords[0])
                {
                    if (받침확인(sb[i - 1]))
                    {
                        sb[i] = '을';
                    }
                    else
                    {
                        sb[i] = '를';
                    }
                }
                else if (sb[i] == SystemWords[1])
                {
                    if (받침확인(sb[i - 1]))
                    {
                        sb[i] = '이';
                    }
                    else
                    {
                        sb[i] = '가';
                    }
                }
            }
            return Half2Full(sb.ToString());


        }

        private static char[] SystemWords = { '◈', '▣', '◐', '◑' };
        public class Format
        {
            static Format()
            {
                if (!IsInitialized) Initialize();
                IsInitialized = true;
            }

            public Format()
            {

            }
            public Format(params string[] tags)
            {
                this.tags.AddRange(tags);
            }

            List<WriteAble> args = new List<WriteAble>();
            public List<string> tags = new List<string>();
            public Format AddTags(params string[] tags)
            {
                this.tags.AddRange(tags);
                foreach (WriteAble arg in args)
                {
                    arg.tags.AddRange(tags);
                }
                return this;
            }


            public Format Append(string str)
            {
                args.Add(new WriteAble.Text(str, tags.ToArray()));
                return this;
            }
            public Format AppendLine(string str)
            {
                Append(str);
                args.Add(new WriteAble.NextLine(tags.ToArray()));
                return this;
            }
            public Format AppendLine()
            {
                args.Add(new WriteAble.NextLine(tags.ToArray()));
                return this;
            }
            public Format AppendButton(string str)
            {
                return AppendButton(str, str);
            }
            public Format AppendButton(string str, string name)
            {
                args.Add(new WriteAble.Button(str, name, tags.ToArray()));
                return this;
            }
            public Format AppendButton(string str, string name, params string[] tags)
            {
                List<string> newtags = tags.ToList();
                newtags.Add(name);
                args.Add(new WriteAble.Button(str, name, newtags.ToArray()));
                return this;
            }
            public Format AppendInputText(string str, string name)
            {
                args.Add(new WriteAble.InputText(str, name, tags.ToArray()));
                return this;
            }
            public Format AppendInputText(string str, string name, params string[] tags)
            {
                List<string> newtags = tags.ToList();
                args.Add(new WriteAble.InputText(str, name, newtags.ToArray()));
                return this;
            }
            public Format ToButton(string str)
            {
                return ToButton(str, str);
            }
            public Format ToButton(string str, string name)
            {
                return ToType<WriteAble.Button>(str, name, "");
            }
            public Format ToButton(string str, string name, params string[] tags)
            {
                return ToType<WriteAble.Button>(str, name, tags);
            }
            public Format ToInputText(string str, string name)
            {
                return ToType<WriteAble.InputText>(str, name);
            }
            public Format ToInputText(string str, string name, params string[] tags)
            {
                return ToType<WriteAble.InputText>(str, name, tags);
            }
            public void Invoke()
            {
                LowConsole.Write(args);
            }
            public void InvokeLine()
            {
                AppendLine();
                LowConsole.Write(args);
            }
            private Format ToType<T>(string strorigin, string name, params string[] tags) where T : WriteAble.Complexed, new()
            {
                string str = FixString(strorigin);

                foreach (WriteAble arg in args)
                {
                    if (arg is WriteAble.Text)
                    {
                        WriteAble.Text text = arg as WriteAble.Text;
                        {
                            if (text.str.Contains(str))
                            {
                                if (text.str.StartsWith(str))
                                {
                                    T b1 = new T();
                                    b1.str = text.str.Substring(0, str.Length);
                                    b1.name = name;
                                    b1.tags = text.tags;
                                    b1.tags.AddRange(tags);
                                    WriteAble.Text t2 = text;
                                    t2.str = text.str.Substring(str.Length);

                                    args.Replace<WriteAble>(text, (WriteAble)b1, t2);
                                    return this;
                                }
                                else if (text.str.EndsWith(str))
                                {
                                    WriteAble.Text t1 = text;
                                    t1.str = text.str.Substring(0, str.Length);

                                    T b2 = new T();
                                    b2.str = text.str.Substring(str.Length);
                                    b2.name = name;
                                    b2.tags = text.tags;
                                    b2.tags.AddRange(tags);


                                    args.Replace<WriteAble>(text, t1, (WriteAble)b2);
                                    return this;
                                }
                                else
                                {
                                    int index = text.str.IndexOf(str);
                                    int length = str.Length;

                                    WriteAble.Text t1 = text;
                                    t1.str = text.str.Substring(0, index);

                                    T b2 = new T();
                                    b2.str = text.str.Substring(index, length);
                                    b2.name = name;
                                    b2.tags = text.tags;
                                    b2.tags.AddRange(tags);

                                    WriteAble.Text t3 = text;
                                    t3.str = text.str.Substring(index + length);


                                    args.Replace<WriteAble>(text, t1, (WriteAble)b2, t3);
                                    return this;
                                }
                            }
                        }

                    }
                }
                return this;
            }
            public static Format Stack(Format f1, Format f2)
            {
                List<List<WriteAble>> f1lists = f1.SeperateFormatToLines();
                List<List<WriteAble>> f2lists = f2.SeperateFormatToLines();

                int min = Math.Min(f1lists.Count, f2lists.Count);

                for (int i = 0; i < min; i++)
                {
                    f1lists[i].AddRange(f2lists[i]);
                }


                if (f1lists.Count == f2lists.Count)
                {
                    return CombineLinesToFormat(f1lists);
                }
                else if (f1lists.Count > f2lists.Count)
                {
                    return CombineLinesToFormat(f1lists);

                }
                else   //f1lists.Count < l2lists.Count
                {
                    for (int i = min; i < f2lists.Count; i++)
                    {
                        f1lists.Add(f2lists[i]);
                    }
                    return CombineLinesToFormat(f1lists);
                }
            }
            private List<List<WriteAble>> SeperateFormatToLines()
            {
                List<List<WriteAble>> arglist = new List<List<WriteAble>>();


                List<WriteAble> imshi = new List<WriteAble>();
                foreach (WriteAble arg in this.args)
                {
                    if (arg is WriteAble.NextLine)
                    {
                        arglist.Add(imshi);
                        imshi = new List<WriteAble>();
                    }
                    else
                    {
                        imshi.Add(arg);
                    }
                }
                arglist.Add(imshi);

                return arglist;
            }
            private static Format CombineLinesToFormat(List<List<WriteAble>> arglist)
            {
                Format format = new Format();

                foreach (List<WriteAble> arg in arglist)
                {
                    format.args.AddRange(arg);
                    format.args.Add(new WriteAble.NextLine());
                }

                return format;
            }


            public static Format operator +(Format f1, string str2)
            {
                return f1.Append(str2);
            }
            public static Format operator -(Format f1, (string str, string name) strs2)
            {
                return f1.AppendButton(strs2.str, strs2.name);
            }
            public static Format operator %(Format f1, (string str, string name) strs2)
            {
                return f1.AppendInputText(strs2.str, strs2.name);
            }

            public static Format operator -(Format f1, string str)
            {
                return f1.AppendButton(str);
            }

            public static Format operator %(Format f1, string str)
            {
                return f1.AppendInputText(str, str);
            }

            public Format Done()
            {
                return this;
            }
        }
        public static class RichConsole
        {
            static RichConsole()
            {
                if (!IsInitialized) Initialize();
                IsInitialized = true;
            }
        }

        public static class SimpleConsole
        {
            static SimpleConsole()
            {
                if (!IsInitialized) Initialize();
            }

            public static void Write(string str)
            {
                LowConsole.Write(Sepreate(str));
            }
            public static void WriteLine(string str)
            {
                List<WriteAble> arglist = Sepreate(str);
                arglist.Add(new WriteAble.NextLine());
                LowConsole.Write(arglist);
            }
            public static void WriteLine()
            {
                List<WriteAble> arglist = new List<WriteAble>();
                arglist.Add(new WriteAble.NextLine());
                LowConsole.Write(arglist);
            }

            private static List<WriteAble> Sepreate(string str)
            {
                List<WriteAble> arglist = new List<WriteAble>();
                string str2 = str.Replace(Environment.NewLine, SystemWords[0].ToString());
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < str2.Length; i++)
                {
                    if (str2[i] != SystemWords[0])
                    {
                        sb.Append(str2[i]);
                    }
                    else
                    {
                        if (sb.Length > 0)
                        {
                            arglist.Add(new WriteAble.Text(sb.ToString()));
                            sb.Clear();
                        }
                        arglist.Add(new WriteAble.NextLine());
                    }
                }

                arglist.Add(new WriteAble.Text(sb.ToString()));
                sb.Clear();

                return arglist;
            }
            public static string ReadLine()
            {
                return MiddleConsole.ReadLine();
            }
            public static WriteAble.Button ReadButton()
            {
                return MiddleConsole.ReadButton();
            }
            public static string ReadButtonName()
            {
                return MiddleConsole.ReadButton().name;
            }
            public static WriteAble.Button ReadButtonSecure(params string[] tags)
            {
            entry1:
                WriteAble.Button btn = MiddleConsole.ReadButton();
            entry2:
                Func<string, bool> Checker = (str) =>
                {
                    if (tags.Contains(str))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };

                if (btn.tags.Any(Checker))
                {
                    Format format = new Format("secure_ask");
                    format.AppendLine("정말　" + btn.name + "　선택합니까?");
                    format.AppendLine("[예]　　　　[아니오]");
                    format.ToButton("[예]", "예");
                    format.ToButton("[아니오]", "아니오");
                    format.Invoke();
                    WriteAble.Button btn2 = SimpleConsole.ReadButton();
                    if (btn2.name == "예")
                    {
                        return btn;
                    }
                    else if (btn2.name == "아니오")
                    {
                        SimpleConsole.Clear("secure_ask");
                        goto entry1;
                    }
                    else
                    {
                        SimpleConsole.Clear("secure_ask");
                        goto entry2;
                    }
                }
                else
                {
                    SimpleConsole.Clear("secure_ask");
                    return btn;
                }
            }

            public static string GetInputText(string name)
            {
                return consoleApp.GetInputText(name);
            }
            public static void Clear()
            {
                LowConsole.Clear();
            }

            public static void Clear(string tag)
            {
                LowConsole.Clear((arg) =>
                {
                    if (arg.tags.Contains(tag))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
        }
        internal static class MiddleConsole
        {



            public static string ReadLine()
            {
                return Task.Run<string>(Console.consoleApp.ReadLine).Result;
            }

            public static WriteAble.Button ReadButton()
            {
                return Task.Run<WriteAble.Button>(Console.consoleApp.ReadButton).Result;
            }


        }
        internal static class LowConsole
        {
            public static List<WriteAble.NextLine> nextlinelist = new List<WriteAble.NextLine>();

            public static void Write(IEnumerable<WriteAble> line)
            {
                IEnumerator<WriteAble> e = line.GetEnumerator();
                while (e.MoveNext())
                {
                    Write(e.Current);
                }
            }
            public static void Write(WriteAble obj)
            {
                TextObject textObj = new TextObject();

                int width;
                switch (obj)
                {
                    case WriteAble.Text text:
                        textObj.data = obj as WriteAble.Complexed;
                        width = GetStringWidth(text.str, Argument.StaticArgs.BaseFont);
                        textObj.location = new Rectangle(currentxpos, currentypos, width, 20);
                        currentxpos += width;
                        Console.consoleApp.Write(textObj);
                        break;
                    case WriteAble.Button text:
                        textObj.data = obj as WriteAble.Complexed;
                        width = GetStringWidth(text.str, Argument.StaticArgs.BaseFont);
                        textObj.location = new Rectangle(currentxpos, currentypos, width, 20);
                        currentxpos += width;
                        Console.consoleApp.Write(textObj);
                        break;
                    case WriteAble.InputText text:
                        textObj.data = obj as WriteAble.Complexed;
                        width = GetStringWidth(text.str, Argument.StaticArgs.BaseFont);
                        textObj.location = new Rectangle(currentxpos, currentypos, width, 20);
                        currentxpos += width;
                        Console.consoleApp.Write(textObj);
                        break;
                    case WriteAble.NextLine nextline:
                        currentxpos = 12;
                        nextlinelist.Add(nextline);
                        return;
                    default:
                        throw new Exception();
                }



            }

            public static void Clear()
            {
                Console.consoleApp.Clear((_) => true);
                nextlinelist.Clear();
            }

            public static void Clear(Predicate<WriteAble> predicate)
            {

                Console.consoleApp.Clear(predicate);
                int i = nextlinelist.RemoveAll(predicate);
            }


            private static Point screensize = new Point();
            private static int lineborder = 20;
            private static int fontsize = 9;



            private static int currentline
            {
                get
                {
                    return nextlinelist.Count + 1;
                }
            }
            private static int currentypos
            {
                get
                {
                    return currentline * (lineborder + 1);
                }
            }
            private static int currentxpos = 12;





            private static int GetStringWidth(string text, Font font)
            {
                return text.Length * 17;


                int bytecount = new UTF8Encoding().GetByteCount(text);
                int length = text.Length;
                return ((bytecount - length) / 2 + length) * 9 - 1;

                Image fakeImage = new Bitmap(1, 1);
                Graphics graphics = Graphics.FromImage(fakeImage);
                return graphics.MeasureString(text, font).ToSize().Width - 4;
            }
            private static int GetStringHeight(string text, Font font)
            {
                Image fakeImage = new Bitmap(1, 1);
                Graphics graphics = Graphics.FromImage(fakeImage);
                return graphics.MeasureString(text, font).ToSize().Height;
            }
        }
    }



    namespace Argument
    {
        public static class StaticArgs
        {
            public static TextColor BaseTextColor = new TextColor(Color.White, Color.Black, Color.White, Color.Black);
            public static TextColor BaseButtonColor = new TextColor(Color.White, Color.Black, Color.Yellow, Color.Black);
            public static TextColor BaseInputTextColor = new TextColor(Color.Black, Color.White, Color.Black, Color.White);

            public static Font BaseFont;// { get; set;} = new Font("돋움체", 12f, FontStyle.Bold);
        }
        public struct TextColor
        {
            public Color basecolor;
            public Color backcolor;
            public Color hoverbasecolor;
            public Color hoverbackcolor;
            public TextColor(Color basecolor, Color backcolor, Color hoverbasecolor, Color hoverbackcolor)
            {
                this.basecolor = basecolor;
                this.backcolor = backcolor;
                this.hoverbasecolor = hoverbasecolor;
                this.hoverbackcolor = hoverbackcolor;
            }
        }
        public interface WriteAble
        {
            public interface Complexed : WriteAble
            {
                public string str { get; set; }
                public string name { get; set; }
                public TextColor textColor { get; set; }
                public Font font { get; set; }
            }
            public class Text : Complexed
            {
                public Text(string str)
                {
                    this.str = str;
                    this.name = "";

                }
                public Text(string str, params string[] tags)
                {
                    this.str = str;
                    this.name = "";
                    this.tags = tags.ToList();
                }

                private string _str;
                public string str
                {
                    get
                    {
                        return _str;
                    }
                    set
                    {
                        _str = RichConsole.Console.FixString(value);
                    }
                }
                public string name { get; set; } = "";
                public List<string> tags { get; set; } = new List<string>();

                public TextColor textColor { get; set; } = Argument.StaticArgs.BaseTextColor;
                public Font font { get; set; } = Argument.StaticArgs.BaseFont;


            }
            public class Button : Complexed
            {
                public Button()
                {

                }
                public Button(string str, string name)
                {
                    this.str = str;
                    this.name = name;
                }
                public Button(string str, string name, params string[] tags)
                {
                    this.str = str;
                    this.name = name;
                    this.tags = tags.ToList();
                }
                private string _str;
                public string str
                {
                    get
                    {
                        return _str;
                    }
                    set
                    {
                        _str = RichConsole.Console.FixString(value);
                    }
                }
                public string name { get; set; }
                public List<string> tags { get; set; } = new List<string>();

                public TextColor textColor { get; set; } = Argument.StaticArgs.BaseButtonColor;
                public Font font { get; set; } = Argument.StaticArgs.BaseFont;


            }
            public class InputText : Complexed
            {
                public InputText()
                {

                }

                public InputText(string str, string name)
                {
                    this.str = str;
                    this.name = name;
                }

                public InputText(string str, string name, params string[] tags)
                {
                    this.str = str;
                    this.name = name;
                    this.tags = tags.ToList();
                }

                private string _str;
                public string str
                {
                    get
                    {
                        return _str;
                    }
                    set
                    {
                        _str = RichConsole.Console.FixString(value);
                    }
                }
                public string name { get; set; } = "";
                public List<string> tags { get; set; } = new List<string>();

                public TextColor textColor { get; set; } = Argument.StaticArgs.BaseInputTextColor;
                public Font font { get; set; } = Argument.StaticArgs.BaseFont;


            }
            public class NextLine : WriteAble
            {
                public NextLine(params string[] tags)
                {
                    this.tags = tags.ToList();
                }
                public List<string> tags { get; set; } = new List<string>();


            }



            public List<string> tags { get; set; }
        }

        public class TextObject
        {




            public Rectangle location;
            public WriteAble.Complexed data;
        }
        public class ImageObject
        {

        }

        public class ReturnedButton
        {
            string value;
            string name;
            List<string> tags;
        }
    }

    public interface IConsoleApp
    {
        public void Write(TextObject obj);
        public Task<string> ReadLine();
        public Task<WriteAble.Button> ReadButton();
        public string GetInputText(string name);
        public void Clear(Predicate<WriteAble> predicate);


        public bool Initialized { get; set; }
    }
    internal static class ExtensionMethods
    {

        public static void Replace<T>(this List<T> ts, T origin, params T[] replace)
        {
            int index = ts.IndexOf(origin);
            ts.RemoveAt(index);
            ts.InsertRange(index, replace);
        }
        public static void Replace<T>(this List<T> ts, T origin, List<T> replace)
        {
            int index = ts.IndexOf(origin);
            ts.RemoveAt(index);
            ts.InsertRange(index, replace);
        }
        public static void ReplaceAll<T>(this List<T> ts, T origin, params T[] replace)
        {
            while (ts.Contains(origin))
            {
                int index = ts.IndexOf(origin);
                ts.RemoveAt(index);
                ts.InsertRange(index, replace);
            }
        }

        public static void ReplaceAll<T>(this List<T> ts, T origin, List<T> replace)
        {
            while (ts.Contains(origin))
            {
                int index = ts.IndexOf(origin);
                ts.RemoveAt(index);
                ts.InsertRange(index, replace);
            }
        }
    }
}
