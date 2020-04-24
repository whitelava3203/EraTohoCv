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

namespace EraTohoCv
{


    static class Program
    {

        static void Main()
        {
            UI.InCastle.AddBasicUI();
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
                .Invoke();

            Console.ReadButtonSecure("danger");

            Console.WriteLine("END");
        }
        public static void Test2()
        {
            new Format()
                .AppendInputText("000000","val1")
                .AppendLine()
                .Invoke();

            Console.ReadLine();
            string val1 = Console.GetInputText("val1");
            Console.WriteLine(val1);

        }
        public static void Test3()
        {
            (new Format() - "버" + "튼" - "버" + "튼" - "버" + "튼" - "버" + "튼").InvokeLine();
            Console.WriteLine("버튼버튼버튼버튼");
        }
        public static void Test4()
        {
            (new Format() - "1" + "2" - "1" + "2" - "1" + "2" - "1" + "2").InvokeLine();
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
    }
}
