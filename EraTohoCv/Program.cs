using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Format = RichConsole.Console.Format;
using Console = RichConsole.Console.SimpleConsole;
using System.Text;
using RichConsole.Argument;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ObjectManager;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;
using System.Runtime.InteropServices;
using RichConsole;

namespace EraTohoCv
{


    public static class Program
    {

        static void Main()
        {
            Test9();
            return;
            

            CheckDebug();

            SenarioRelated.Senario senario = Debug.CreateKoreanWar();
            GameRelated.senarios.Add(senario);
            
            EraTohoCv.GameRelated.FirstStart();
            Console.ReadLine();
            Console.WriteLine("?????");
            Console.ReadLine();
            throw new Exception();


            
        }
        public static void Test1()
        {
            new Format()
                .AppendLine("[추가하기]　[삭제하기]　[취소하기]")
                .ToButton("추가하기", "추가하기", "safe")
                .ToButton("삭제하기", "삭제하기", "danger")
                .ToButton("취소하기", "취소하기", "safe")
                .Print();

            Console.ReadButtonSecure("danger");

            Console.WriteLine("END");
        }
        public static void Test2()
        {
            new Format()
                .AppendInputText("000000","val1")
                .AppendLine()
                .Print();

            Console.ReadLine();
            string val1 = Console.GetInputText("val1");
            Console.WriteLine(val1);

        }
        public static void Test3()
        {
            (new Format() - "버" + "튼" - "버" + "튼" - "버" + "튼" - "버" + "튼").PrintLine();
            Console.WriteLine("버튼버튼버튼버튼");
        }
        public static void Test4()
        {
            (new Format() - "1" + "2" - "1" + "2" - "1" + "2" - "1" + "2").PrintLine();
            Console.WriteLine("12121212");
        }
        public static IEnumerator Test6()
        {
            yield return null;
            yield return null;

            yield return null;

            yield return null;

        }
        public static void Test7()
        {
            Func<IEnumerator> e = Test6;

        }

        public static void Test8()
        {
            
            object obj = CSharpScript.EvaluateAsync("EraTohoCv.Program.Sex = 15;", ScriptOptions.Default.AddReferences(Assembly.GetExecutingAssembly())).Result;
            ;
        }
        public static void Test9()
        {
            new TestScreen().Start();

            Console.WriteLine("끝~~~~~");
            Console.ReadLine();
        }

        public static void CheckDebug()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Contains("-debug"))
            {
                Thread thread = new Thread(() =>
                {
                    DebugConsoleActivity();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        static void DebugConsoleActivity()
        {
            AllocConsole();
            ScriptOptions options = ScriptOptions.Default.AddReferences("EraTohoCv").AddImports("EraTohoCv");
            while (true)
            {
                string code = System.Console.ReadLine();

                try
                {
                    object obj = CSharpScript.EvaluateAsync(code, options).Result;
                    if (obj != null)
                    {
                        System.Console.WriteLine(obj.ToString());
                    }
                    else
                    {
                        System.Console.WriteLine("실행완료");
                    }
                }
                catch(Exception e)
                {
                    System.Console.WriteLine(e.ToString());
                }
            }
        }
    }

    public class TestScreen : UI
    {
        public override string 이름 { get; set; } = "TestScreen";

        public TestScreen()
        {
            this._InnerSectors = new UIBase[][]
            {
                new UIBase[] {new TestSector(this) },
                new UIBase[] {new TestSector2(this) }
            };
        }

        public int i = 0;



        public class TestSector : UIBase<TestScreen>
        {
            public override string 이름 { get; set; } = "Sec1";

            public TestSector(TestScreen parent) : base(parent)
            {

            }

            public override Format GetFormat()
            {
                return (new Format() + "현재 숫자는 : " + this.Parent.i.ToString()).AppendLine();
            }

        }
        public class TestSector2 : StaticSector<TestScreen>
        {
            public override string 이름 { get; set; } = "Sec2";

            public TestSector2(TestScreen parent) : base(parent)
            {
                this.FormatData = new Format().Append("중간 입니다 : ").AppendButton("증가", "plus").AppendButton("감소", "minus").AppendButton("종료", "end").AppendLine();
            }

            protected override void ButtonPressEvent(ButtonReturn btn)
            {
                if (btn.name == "plus")
                {
                    this.Parent.i += 1;
                }
                else if(btn.name == "minus")
                {
                    this.Parent.i -= 1;
                }
                else if(btn.name == "end")
                {
                    this.Terminate();
                }
            }
        }
    }





    public class UI_CharacterUpdate : Screen
    {

    }


}
