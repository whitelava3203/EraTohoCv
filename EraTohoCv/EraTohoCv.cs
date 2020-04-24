using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EraTohoCv.SystemRelated;
using Format = RichConsole.Console.Format;
using Console = RichConsole.Console.SimpleConsole;
using RichConsole.Argument;

namespace EraTohoCv
{
    public class UI : Named
    {
        

        public static Format CharacterSelection(Predicate<CharacterRelated.Character> checker, params string[] tags)
        {
            return CharacterSelection(StaticUse.dataStorage.무장.FindAll(checker), tags);
        }
        public static Format CharacterSelection(List<CharacterRelated.Character> chalist,params string[] tags)
        {
            Format format = new Format(tags.Append("CharacterSelection").ToArray());
            

            using (IEnumerator<CharacterRelated.Character> e = chalist.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    format.Append("[");
                    format.AppendButton(e.Current.이름);
                    format.Append("]");

                    if (e.MoveNext())
                    {
                        format.Append("　[");
                        format.AppendButton(e.Current.이름);
                        format.Append("]");
                    }
                    format.AppendLine();
                }
            }
            return format;
        }


        public string 이름 { get; set; }
        public static Format TopScreen(CharacterRelated.Character cha)
        {
            Format format = new Format("TopScreen");
            format.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            format.AppendLine(String.Format("{0}일   {1}:{2}:{3}", GameTime.Day, GameTime.Hour, GameTime.Minute, GameTime.Second));
            (format + "체력　:　" + cha.스탯.체력.ToString() + "/" +cha.스탯.최대체력.ToString()+ "　기력　:　 "+ cha.스탯.기력.ToString() + " / " +cha.스탯.최대기력.ToString() - "[월드맵]" + "　/　" - "[세이브]" + "　/　" - "[로드]").AppendLine();
            format.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            return format;
        }
        

        public class InCastle : UI
        {
            public NameList<Command> commands
            {
                get
                {
                    return StaticUse.dataStorage.castlecommands;
                }
            }

            public Format LeftSide()
            {
                Format format = new Format("LeftSide");
                foreach (Command command in commands)
                {
                    format.Append("[").AppendButton(command.ShowingName,command.이름).Append("]").AppendLine("┃");
                }
                return format;
            }




            public class Command : Named
            {
                public string category;
                public string 이름 { get; set; }
                public string ShowingName;

                public Func<CharacterRelated.Character, bool> 조건;

                public class SimpleButton : Command
                {
                    public Func<CharacterRelated.Character, IEnumerator> Clicked;
                }

                public class OpenNewScreen : Command
                {
                    public Func<Format> NewScreen;
                }

                public class RightSide : Command
                {
                    public Func<Format> _InnerScreen;
                    public Format InnerScreen
                    {
                        get
                        {
                            return _InnerScreen().AddTags(this.이름).AddTags("RightSide");
                        }
                    }



                    public class Button : RightSide
                    {
                        public Func<WriteAble.Button, CharacterRelated.Character, IEnumerator> ButtonClicked;


                    }
                }



            }

            public IEnumerator GetRightSideBtnEnumerator(CharacterRelated.Character cha, WriteAble.Button btn)
            {
                foreach (string tag in btn.tags)
                {
                    foreach (Command.RightSide.Button command in commands)
                    {
                        if (command.이름 == tag)
                        {
                            return command.ButtonClicked(btn, cha);
                        }
                    }
                }
                throw new Exception("no name");
            }

        }



        public static void AddBasicUI()
        {
            //uiInCastle.AddCommand("친교", "만나러간다", new Format());
            //uiInCastle.AddCommand("모략", "　잠입한다", new Format());
            //uiInCastle.AddCommand("모략", "투옥시킨다", new Format());
            //uiInCastle.AddCommand("모략", "　명령한다", new Format());
            //uiInCastle.AddCommand("군사", "　병사모집", new Format());
            //uiInCastle.AddCommand("군사", "　분대설정", new Format());
            //uiInCastle.AddCommand("군사", "　　　출격", new Format());
            //uiInCastle.AddCommand("포로", "포로의처리", new Format());
            //uiInCastle.AddCommand("정무", "스킬을습득", new Format());
            //uiInCastle.AddCommand("정무", "　　　노동", new Format());
            //uiInCastle.AddCommand("정무", "사관을모집", new Format());
            //uiInCastle.AddCommand("정무", "　영지탐색", new Format());
            //uiInCastle.AddCommand("정보", "　능력표시", new Format());
            //uiInCastle.AddCommand("정보", "　관계일람", new Format());
            //uiInCastle.AddCommand("특수", "　제국방문", new Format());





            //uiInCastle.AddCommand("매매", "　　아이탬", new Format());




        }

        
    }

    public class GameRelated
    {
        public static void FirstStart()
        {
            Console.WriteLine("아직 간단한 텍스트 전략 게임");
            Console.WriteLine("제작자 : 텍붕이");
            (new Format() - "[새 게임]" + "　" - "[불러오기]").InvokeLine();
            string btn = Console.ReadButtonName();
            Console.Clear();
            if (btn == "[새 게임]")
            {
                NewGame();
            }
            else if (btn == "[불러오기]")
            {
                LoadGame();
            }
        }

        public static void NewGame()
        {
            while (true)
            {
                Console.WriteLine("플레이할 시나리오 선택 : ");
                foreach (SenarioRelated.Senario s in senarios)
                {
                    new Format().AppendButton(s.이름).AppendLine().Invoke();
                }
                string btn = Console.ReadButtonName();
                Console.Clear();
                SenarioRelated.Senario senario = senarios[btn];
                senario.설명.Invoke();
                (new Format() + "이 시나리오를 선택합니까 ? " - "[예]" + ",　" - "[아니오]").InvokeLine();
                
                btn = Console.ReadButton().name;
                if(btn == "[예]")
                {
                    senario.StartThis();
                }
                Console.Clear();
            }
        }

        public static void LoadGame()
        {

        }

        public static NameList<SenarioRelated.Senario> senarios = new NameList<SenarioRelated.Senario>();
    }

    public class SystemRelated
    {
        public static Random rand = new Random();
        public static CharacterRelated.Character CurrentCharacter;

        public static List<Event> TaskQueue = new List<Event>();
        public static void TimeSpan()
        {
            Console.WriteLine("시작!");

            while (true)
            {
                DoEvent(Event.Type.UpdateSecond);
                if (GameTime.Second == 0)
                {
                    DoEvent(Event.Type.UpdateMinit);
                    if (GameTime.Minute == 0)
                    {
                        DoEvent(Event.Type.UpdateHour);
                        if (GameTime.Hour == 0)
                        {
                            DoEvent(Event.Type.UpdateDay);
                        }
                    }
                }
                
                

                GameTime = GameTime.AddSeconds(1);
            }
        }
        
        private static void DoEvent(Event.Type eventtype)
        {
            foreach (CharacterRelated.Character cha in StaticUse.dataStorage.무장)
            {
                CurrentCharacter = cha;
                cha.eventcollection.InvokeAllType(eventtype);
            }
            foreach (MapRelated.Castle castle in StaticUse.dataStorage.맵.locationlist)
            {
                castle.eventCollection.InvokeAllType(eventtype);
            }

            List<Event> RemovedTaskQueue = new List<Event>();
            foreach (Event task in TaskQueue)
            {
                if (task.이름 == eventtype)
                {
                    if (!task.Invoke())
                    {
                        RemovedTaskQueue.Add(task);
                    }
                }
            }

            TaskQueue.RemoveAll((task) => { if (RemovedTaskQueue.Contains(task)) return true; return false; });
        }
        public static DateTime GameTime = new DateTime();

        public static IEnumerator WaitSeconds(int sec)
        {
            DateTime endtime = GameTime.AddSeconds(sec);

            while(GameTime < endtime)
            {
                yield return null;
            }
            yield break;
        }
        public static IEnumerator WaitMinits(int min)
        {
            return WaitSeconds(min*60);
        }
        public static IEnumerator WaitHours(float hour)
        {
            return WaitMinits((int)(hour * 60));
        }
        public static IEnumerator WaitUntil(string format)
        {
            string str = "07:00:00";
            str = "7시0분0초";



            yield return null;
        }
    }

    public class WarRelated
    {

        public static IEnumerator Fight(ArmyUnit army1, ArmyUnit army2)
        {
            IEnumerator waiter;

            while (true)
            {

                Console.WriteLine("행동 설정");
                foreach (ArmyInfo singleinfo in army1.rangeArmy)
                {
                    Console.WriteLine(singleinfo.armyType.이름);
                }
                Console.ReadLine();

                ArmyUnit.AttackEachOtherMeele(ref army1, ref army2);

                waiter = WaitSeconds(10);   
                while (waiter.MoveNext()) yield return null;
            }
        }

        public static void Debug_FightTest()
        {
            Console.WriteLine("내가 지휘할 부대 설정 : ");
        }

        /*
        private static ArmyUnit Debug_CreateArmyUnit()
        {
            Console.Write("보병 수 : ");
            Console.WriteInputText("99    ", "num1");
            Console.Write("궁병 수 : ");
            Console.WriteInputText("99    ", "num2");
            Console.WriteLine();
            Console.Write("창병 수 : ");
            Console.WriteInputText("99    ", "num3");
            Console.Write("기병 수 : ");
            Console.WriteInputText("99    ", "num4");
            Console.WriteLine();
            Console.Write("투석기 수 : ");
            Console.WriteInputText("99    ", "num5");
            Console.Write("공성추 수 : ");
            Console.WriteInputText("99    ", "num6");
            Console.WriteLine();
            Console.WriteButton("확인","yes");
            Console.Write(" , ");
            Console.WriteButton("취소","no");
            Console.WriteLine();
            string btn = Console.ReadButton();
            switch(btn)
            {
                case "yes":

                    break;
            }

         
            ArmyUnit army = new ArmyUnit();
            ArmyInfo armyinfo;

            armyinfo = new ArmyInfo();
            armyinfo.armyType = StaticUse.dataStorage.병과["보병"];
            armyinfo.count = int.Parse(Console.GetInputText("num1"));
            armyinfo.IsMeele = true;
            army.armyUnit.Add(armyinfo);

            armyinfo = new ArmyInfo();
            armyinfo.armyType = StaticUse.dataStorage.병과["궁병"];
            armyinfo.count = int.Parse(Console.GetInputText("num2"));
            armyinfo.IsMeele = false;
            army.armyUnit.Add(armyinfo);

            armyinfo = new ArmyInfo();
            armyinfo.armyType = StaticUse.dataStorage.병과["창병"];
            armyinfo.count = int.Parse(Console.GetInputText("num3"));
            armyinfo.IsMeele = true;
            army.armyUnit.Add(armyinfo);

            armyinfo = new ArmyInfo();
            armyinfo.armyType = StaticUse.dataStorage.병과["기병"];
            armyinfo.count = int.Parse(Console.GetInputText("num4"));
            armyinfo.IsMeele = true;
            army.armyUnit.Add(armyinfo);

            armyinfo = new ArmyInfo();
            armyinfo.armyType = StaticUse.dataStorage.병과["투석기"];
            armyinfo.count = int.Parse(Console.GetInputText("num5"));
            armyinfo.IsMeele = false;
            army.armyUnit.Add(armyinfo);

            armyinfo = new ArmyInfo();
            armyinfo.armyType = StaticUse.dataStorage.병과["공성추"];
            armyinfo.count = int.Parse(Console.GetInputText("num6"));
            armyinfo.IsMeele = true;
            army.armyUnit.Add(armyinfo);

            return army;
        }
        */

        public class ArmyUnit : Named, Moveable
        {

            public string 이름 { get; set; }
            public List<ArmyInfo> armyUnit = new List<ArmyInfo>();


            public List<ArmyInfo> meeleArmy
            {
                get
                {
                    return armyUnit.FindAll(FindMeeleArmy);
                }
            }
            public List<ArmyInfo> rangeArmy
            {
                get
                {
                    return armyUnit.FindAll(FindRangeArmy);
                }
            }



            public int meeleattackhuman
            {
                get
                {
                    int output = 0;
                    foreach (ArmyInfo unitData in meeleArmy)
                    {
                        output += unitData.armyType.meeleattackhuman * unitData.count;
                    }
                    return output;
                }
            }

            public int meeleattackcastle
            {
                get
                {
                    int output = 0;
                    foreach (ArmyInfo unitData in meeleArmy)
                    {
                        output += unitData.armyType.meeleattackcastle * unitData.count;
                    }
                    return output;
                }
            }
            public int meeledefense
            {
                get
                {
                    int output = 0;
                    foreach (ArmyInfo unitData in meeleArmy)
                    {
                        output += unitData.armyType.defense * unitData.count;
                    }
                    return output;
                }
            }

            public bool OnRoad { get; set; } = false;
            public MapRelated.Road 도로 { get; set; }
            public int 도로위치 { get; set; }
            public MapRelated.Location 위치 { get; set; }
            

            public void DamageMeele(float percentage)
            {
                meeleArmy.ForEach((singleUnit) => singleUnit.Damage(percentage));
            }
            public static void AttackEachOtherMeele(ref ArmyUnit army1, ref ArmyUnit army2)
            {
                float attack1;
                float attack2;

                attack1 = (float)army1.meeleattackhuman / army2.meeledefense * 0.2f;
                attack2 = (float)army2.meeleattackhuman / army1.meeledefense * 0.2f;
                army2.DamageMeele(attack1);
                army1.DamageMeele(attack2);
            }

            private static bool FindMeeleArmy(ArmyInfo singleInfo) => singleInfo.IsMeele;

            private static bool FindRangeArmy(ArmyInfo singleInfo) => !singleInfo.IsMeele;
        }
        public class ArmyInfo
        {
            public ArmyType armyType;
            public int count;
            public bool IsMeele;

            public void RangeAttack(ArmyInfo target)
            {
                float attack1 = (float)this.armyType.rangeattackhuman / target.armyType.defense * 0.2f;
            }
            public void Damage(float percentage)
            {
                this.count = (int)(this.count * (1f - percentage));
            }
        }
        public class ArmyType : Named
        {

            

            public string 이름 { get; set; }
            public int 요구노동력;
            public ResourceRelated.ResourceUnit 고정자원사용량 = new ResourceRelated.ResourceUnit();
            public ResourceRelated.ResourceUnit 징집비용 = new ResourceRelated.ResourceUnit();
            public ResourceRelated.ResourceUnit 유지비 = new ResourceRelated.ResourceUnit();
            public NameList<NationRelated.Technology.SingleTech> 요구기술 = new NameList<NationRelated.Technology.SingleTech>();
            public NameList<BuildingRelated.BuildingType> 요구건물 = new NameList<BuildingRelated.BuildingType>();

            public int meeleattackhuman = 0;
            public int meeleattackcastle = 0;
            public int rangeattackhuman = 0;
            public int rangeattackcastle = 0;
            public int defense;

            public int speed;

            public bool prefermeele;
        }
    }

    public class MapRelated
    {
        public class Map : Named
        {
            public string 이름 { get; set; }

            public NameList<Location> locationlist = new NameList<Location>();
            public Func<Format> WorldMapSelection;
        }

        public class Location : Named
        {
            public string 이름 { get; set; }

            public NameList<CharacterRelated.Character> 무장
            {
                get
                {
                    NameList<CharacterRelated.Character> chalist = new NameList<CharacterRelated.Character>();
                    foreach(CharacterRelated.Character cha in StaticUse.dataStorage.무장)
                    {
                        if(!cha.OnRoad)
                        {
                            if(cha.위치 == this)
                            {
                                chalist.Add(cha);
                            }
                        }
                    }
                    return chalist;
                }
            }
            public List<Connection> connections = new List<Connection>();
        }

        public class Castle : Location
        {
            public bool Captured;

            /*
            public int 돈 = 0;
            public int 경제규모 = 0;
            public int 민심 = 1000;
            //~ 200 : 폭동
            //200 ~ 400 : 폭동 직전
            //400 ~ 600 : 시위중
            //600 ~ 800 : 욕을 좀 먹음
            //800 ~ : 성주말 잘따름

            public int 치안 = 1000;
            //~ 200 : 대낮에 살인사건이 일어나고 조폭끼리 싸우고있음
            //200 ~ 400 : 매일매일 강력범죄가 일어남
            //400 ~ 600 : 밤에 돌아다니면 납치당함
            //600 ~ 800 : 약간 위험함
            //800 ~ : 강력 범죄는 없고 가끔 경범죄만 나옴

            public int 인구수;
            public int 수용인구수 { get; set; }

            public int 최대경제규모
            {
                get
                {
                    decimal 치안배율 = (decimal)this.치안 * (decimal)this.치안 / (decimal)-3375000 + (decimal)this.치안 * (decimal)8 / (decimal)5620 + (decimal)0.1;
                    decimal 민심배율 = (decimal)this.민심 * (decimal)this.민심 / (decimal)-600000 + (decimal)this.민심 * (decimal)17 / (decimal)6000;
                    return (int)((decimal)인구수 * (decimal)1 * 치안배율 * 민심배율);
                }
            }

            public int 모집가능인구수;
            */

            public ResourceRelated.ResourceUnit staticincome = new ResourceRelated.ResourceUnit();
            public ResourceRelated.ResourceUnit resource = new ResourceRelated.ResourceUnit();
            public BuildingRelated.BuildingUnit building = new BuildingRelated.BuildingUnit();

            public CharacterRelated.Character 성주
            {
                get
                {
                    foreach (CharacterRelated.Character cha in StaticUse.dataStorage.무장)
                    {
                        if(cha.계급 >= CharacterRelated.Rank.성주)//성주 또는 왕
                        {
                            if (cha.소유한성 == this)
                                return cha;
                        }
                    }
                    throw new Exception();
                }
            }

            public EventCollection eventCollection = new EventCollection();
            public NationRelated.Nation 소속국가;

            public Castle()
            {
                eventCollection.AddEvent(UpdateSecond, Event.Type.UpdateSecond);
                eventCollection.AddEvent(경제계산,Event.Type.UpdateHour);

                tasks = new CastleTask.CastleTaskUnit(this);
                eventCollection.AddEvent(tasks.UpdateSecond,Event.Type.UpdateSecond);
            }

            public IEnumerator UpdateSecond()
            {
                IEnumerator delay;
                while(true)
                {


                    yield return null;
                }
            }

            public IEnumerator 경제계산()
            {
                while (true)
                {
                    if (GameTime.Hour == 7)
                    {
                        //static리소스 초기화
                        foreach (ResourceRelated.ResourceInfo info in this.resource)
                        {
                            if(info.type is ResourceRelated.StaticResourceType)
                            {
                                info.Count = 0;
                            }
                        }

                        //건물일일버프초기화
                        foreach (BuildingRelated.BuildingInfo info in this.building)
                        {
                            info.Reset();
                        }

                        //성의 기본값
                        resource.AddValue(staticincome,true);

                        //기술효과적용
                        foreach(NationRelated.Technology.SingleTech tech in this.소속국가.기술력)
                        {
                            //기술의 자원생산량 적용
                            resource.AddValue(tech.고정자원생산량,true);
                            
                            foreach(Attribute attr in tech.attributes)
                            {
                                if(attr is Attribute.Modifier<Castle>)
                                {
                                    //기술의 성 관련 변경하는 식 적용
                                    (attr as Attribute.Modifier<Castle>).modifier(this);
                                }
                            }
                        }

                        //건물효과적용
                        foreach(BuildingRelated.BuildingInfo info in this.building)
                        {
                            resource.AddValue(info.type.일수입,true);
                            resource.AddValue(info.type.고정자원생산량,true);
                            resource.AddValue(info.type.유지비,false);
                            resource.AddValue(info.type.고정자원사용량, false);
                        }
                        

                        //인구수가 수용인원을 넘어가면 치안과 민심 떡락
                        if(resource["인구수"].Count > resource["수용인원"].Count)
                        {
                            int 넘친인구비율퍼센트 = (int)((double)resource["수용인원"].Count / (resource["인구수"].Count - resource["수용인원"].Count) * 100);
                            resource["치안"].Count -= 넘친인구비율퍼센트 * 7.Range(2);
                            resource["민심"].Count -= 넘친인구비율퍼센트 * 4.Range(1);
                        }

                        //인구와 노예 수만큼 노동력 증가
                        resource["노동력"].Count += resource["인구수"].Count + resource["노예"].Count * 2;

                        //출산율로 인구수 증가
                        resource["인구수"].Count.Multiply((double)resource["출산율"].Count/1000d);


                        //민심과 치안이 낮으면 경제력 감소
                        if(resource["치안"].Count < 750)
                        {
                            resource["경제력"].Count.Multiply((double)((decimal)resource["치안"].Count * (decimal)resource["치안"].Count / (decimal)-3375000 + (decimal)resource["치안"].Count * (decimal)8 / (decimal)5620 + (decimal)0.1));
                        }
                        if(resource["민심"].Count < 500)
                        {
                            resource["경제력"].Count.Multiply((double)((decimal)resource["민심"].Count * (decimal)resource["민심"].Count / (decimal)-600000 + (decimal)resource["민심"].Count * (decimal)17 / (decimal)6000));
                        }

                        //경제력*인구만큼 돈 증가
                        resource["돈"].Count += resource["경제력"].Count * resource["인구수"].Count / 1000;

                        //군인이되고싶은사람 증가 최대 인구의 5%까지
                        if ((decimal)resource["모집가능인구"].Count < (decimal)resource["인구수"].Count * (decimal)0.04)
                        {
                            resource["모집가능인구"].Count += (int)((decimal)resource["인구수"].Count * (decimal)0.01);
                        }
                        else
                        {
                            resource["모집가능인구"].Count = (int)((decimal)resource["인구수"].Count * (decimal)0.05);
                        }
                        



                    }
                    yield return null;
                }
            }

            public void StartCreateBuilding_Task(BuildingRelated.BuildingType type)
            {
                CastleTask.CastleTaskType task = new CastleTask.CastleTaskType();
                resource.AddValue(type.건설비용,false);
                task.요구노동력 = type.요구노동력;
                task.FinishAction = (castle) => 
                {
                    building.AddBuilding(type);
                };
                task.CancleAction = (castle) =>
                {
                    resource.AddValue(type.건설비용, true);
                };

                tasks.AddTask(task);
            }


            public void StartGetSolider_Task(WarRelated.ArmyType type)
            {
                CastleTask.CastleTaskType task = new CastleTask.CastleTaskType();
                resource.AddValue(type.징집비용, false);
                task.요구노동력 = type.요구노동력;
                task.FinishAction = (castle) =>
                {
                    //병사추가
                };
                task.CancleAction = (castle) =>
                {
                    resource.AddValue(type.징집비용, true);
                };

                tasks.AddTask(task);

            }

            public CastleTask.CastleTaskUnit tasks;

            

            public class CastleTask
            {
                public class CastleTaskUnit : List<CastleTaskInfo>
                {
                    private Castle castle;

                    public CastleTaskUnit(Castle castle)
                    {
                        this.castle = castle;
                    }

                    public double 현재작업진행도
                    {
                        get
                        {
                            return (double)base[0].진행도 / base[0].type.요구노동력;
                        }
                    }

                    public double 전체작업진행도
                    {
                        get
                        {
                            int all = 0;
                            foreach(CastleTaskInfo info in this)
                            {
                                all += info.type.요구노동력;
                            }
                            return (double)base[0].진행도 / all;
                        }
                    }

                    public IEnumerator UpdateSecond()
                    {
                        while (true)
                        {
                            if (base.Count > 0)
                            {
                                if(base[0].진행도 >= base[0].type.요구노동력)
                                {
                                    base[0].type.FinishAction(castle);
                                    base.RemoveAt(0);
                                }
                                else
                                {
                                    //task 진행
                                    base[0].진행도 += (decimal)castle.resource["노동력"].Count / (decimal)86400;
                                }
                            }
                            else
                            {
                                yield return null;
                            }
                        }
                        
                    }

                    public void AddTask(CastleTaskType task)
                    {
                        base.Add(new CastleTaskInfo(task,0));
                    }

                    public void CancleTask()
                    {

                    }
                }
                public class CastleTaskInfo
                {
                    public CastleTaskType type;
                    public decimal 진행도;

                    public CastleTaskInfo(CastleTaskType type, decimal 진행도)
                    {
                        this.type = type;
                        this.진행도 = 진행도;
                    }
                }
                public class CastleTaskType
                {

                    public int 요구노동력;

                    public Action<Castle> FinishAction;
                    public Action<Castle> CancleAction;
                }
            }
        }

        public class Crossroad : Location
        {
            
        }



        public class Road
        {
            NameList<CharacterRelated.Character> 무장
            { get; }
            NameList<WarRelated.ArmyUnit> 군대 { get; }

            public double 길이;
            public double 시야범위 = 20;

            

            public Road()
            {

            }
            public Road(double length)
            {
                this.길이 = length;
            }

            public Road(double length, double visblearea)
            {
                
                this.길이 = length;
                this.시야범위 = visblearea;
            }
        }

        public class Connection
        {
            public Location 위치;
            public Road 길;
            public Connection()
            {

            }
            public Connection(Location target, Road road)
            {
                this.위치 = target;
                this.길 = road;
            }


            public class Debug_ConnectionCreator
            {
                public Debug_ConnectionCreator(NameList<Location> loclist)
                {
                    this.loclist = loclist;
                }
                private NameList<Location> loclist;
                public void c(string str1, string str2, Road road)
                {
                    Location loc1 = loclist[str1];
                    Location loc2 = loclist[str2];

                    loc1.connections.Add(new Connection(loc2, road));
                    loc2.connections.Add(new Connection(loc1, road));
                }
                public void c(string str1, string str2, Road road1, Road road2)
                {
                    Location loc1 = loclist[str1];
                    Location loc2 = loclist[str2];

                    loc1.connections.Add(new Connection(loc2, road1));
                    loc2.connections.Add(new Connection(loc1, road2));
                }
            }
        }
    }

    public class CharacterRelated
    {

        public class Character : Named, Moveable
        {
            public Color 색깔;
            public EventCollection eventcollection = new EventCollection();

            public bool Controlable = false;

            public string 이름 { get; set; }

            public bool OnRoad { get; set; }

            public MapRelated.Road 도로 { get; set; }

            MapRelated.Location Moveable.위치 { get; set; }
            public int 도로위치 { get; set; }

            public ResourceRelated.ResourceUnit 소유자원 = new ResourceRelated.ResourceUnit();
            
            public CharacterStat 스탯 = new CharacterStat();


            public MapRelated.Location 위치;

            public NationRelated.Nation 소속국가;
            public Rank 계급;
            public MapRelated.Castle 소유한성;
            


            public Character()
            {
                this.eventcollection.AddEvent(UpdateSecond,Event.Type.UpdateSecond);
            }

            public void ChangeNation(NationRelated.Nation nation)
            {
                if(this.소속국가 != null)
                {
                    this.소속국가.무장.Remove(this);
                }
                
                nation.무장.Add(this);
            }


            public IEnumerator UpdateSecond()
            {
                Format rightside = new Format();
                while (true)
                {
                    //Console.WriteLine(this.이름 + "의 턴 진행중");

                    UI.InCastle inCastle = new UI.InCastle();
                    switch (this.계급)
                    {
                        case Rank.왕:
                            break;
                        case Rank.성주:
                            break;
                        case Rank.장교:
                            break;
                    }

                    if (Controlable)
                    {

                        Console.Clear();
                        UI.TopScreen(this).Invoke();
                        Format.Stack(inCastle.LeftSide(),rightside).Invoke();
                        

                        WriteAble.Button btn = Console.ReadButton();
                        if (btn.tags.Contains("LeftSide"))
                        {
                            if(inCastle.commands[btn.name] is UI.InCastle.Command.RightSide)
                            {
                                rightside = (inCastle.commands[btn.name] as UI.InCastle.Command.RightSide).InnerScreen;
                            }
                            else if(inCastle.commands[btn.name] is UI.InCastle.Command.OpenNewScreen)
                            {
                                //미니맵같은거 여는코드 여기다 넣어야함
                            }
                            else if(inCastle.commands[btn.name] is UI.InCastle.Command.SimpleButton)
                            {
                                IEnumerator e = (inCastle.commands[btn.name] as UI.InCastle.Command.SimpleButton).Clicked(this);
                                while (e.MoveNext())
                                {
                                    yield return null;
                                }
                            }

                        }
                        else if (btn.tags.Contains("RightSide"))
                        {
                            IEnumerator e = inCastle.GetRightSideBtnEnumerator(this, btn);
                            while (e.MoveNext())
                            {
                                yield return null;
                            }

                        }

                        
                    }
                    else
                    {
                        //인공지능
                        yield return null;
                    }
                }
            }
        }

        

        public enum Rank
        {
            방랑자,
            장교,
            성주,
            왕
        }

        public class CharacterStat
        {
            public int 전투;
            public int 방어;
            public int 지능;

            public int 최대체력;
            public int 최대기력;

            private int _체력;
            
            public int 체력
            {
                get
                {
                    return _체력;
                }
                set
                {
                    if(value > 최대체력)
                    {
                        _체력 = 최대체력;
                    }
                    else
                    {
                        _체력 = value;
                    }
                }
            }

            private int _기력;
            public int 기력
            {
                get
                {
                    return _기력;
                }
                set
                {
                    if (value > 최대기력)
                    {
                        _기력 = 최대기력;
                    }
                    else
                    {
                        _기력 = value;
                    }
                }
            }
        }
    }
    
    public class NationRelated
    {
        public class Nation : Named
        {
            public string 이름 { get; set; }

            public CharacterRelated.Character 왕
            {
                get
                {
                    foreach(CharacterRelated.Character cha in StaticUse.dataStorage.무장)
                    {
                        if(cha.계급 == CharacterRelated.Rank.왕)
                        {
                            if(cha.소속국가 == this)
                            {
                                return cha;
                            }
                        }
                    }
                    throw new Exception();
                }
            }
            public NameList<CharacterRelated.Character> 무장
            {
                get
                {
                    NameList<CharacterRelated.Character> chalist = new NameList<CharacterRelated.Character>();
                    foreach (CharacterRelated.Character cha in StaticUse.dataStorage.무장)
                    {
                        if(cha.소속국가 == this)
                        {
                            chalist.Add(cha);
                        }
                    }
                    return chalist;
                }
            }
            public NameList<MapRelated.Castle> 성
            {
                get
                {
                    NameList<MapRelated.Castle> caslist = new NameList<MapRelated.Castle>();
                    foreach(MapRelated.Location loc in StaticUse.dataStorage.맵.locationlist)
                    {
                        if(loc is MapRelated.Castle)
                        {
                            caslist.Add(loc as MapRelated.Castle);
                        }
                    }
                    return caslist;
                }
            }
            public Technology 기술력 = new Technology();

            public NationTechUpgrade.NationTechUpgradeUnit techupgrade;
            public class NationTechUpgrade
            {

                public class NationTechUpgradeUnit
                {
                    private Technology nationtech;

                    public NationTechUpgradeUnit(Technology nationtech)
                    {
                        this.nationtech = nationtech;
                    }
                }
                public class NationTechUpgradeInfo
                {
                    public bool Active = true;
                    public decimal 진행도;
                    public Technology.SingleTech type;

                    public NationTechUpgradeInfo(Technology.SingleTech type, decimal 진행도)
                    {
                        this.type = type;
                        this.진행도 = 진행도;
                    }
                }
            }
        }


        public class Technology : NameList<Technology.SingleTech>
        {




            public class SingleTech : Named
            {
                public List<SingleTech> 요구기술 = new List<SingleTech>();

                public string 이름 { get; set; }
                public string 설명;

                public ResourceRelated.ResourceUnit 비용 = new ResourceRelated.ResourceUnit();
                public int 연구요구량;


                public List<Attribute> attributes = new List<Attribute>();
                public ResourceRelated.ResourceUnit 일수입 = new ResourceRelated.ResourceUnit();
                public ResourceRelated.ResourceUnit 고정자원생산량 = new ResourceRelated.ResourceUnit();
            }
        }
    }
    
    public class SenarioRelated
    {
        public class Senario : Named
        {
            public DataStorage 데이터 = new DataStorage();
            public Format 설명;


            public string 이름 { get; set; }


            public void StartThis()
            {
                StaticUse.dataStorage = this.데이터;
                Console.Clear();
                Console.WriteLine("플레이할 무장을 선택");
                StaticUse.dataStorage.맵.WorldMapSelection().Invoke();


                while (true)
                {
                    
                    


                    WriteAble.Button btn = Console.ReadButtonSecure("CharacterSelection");
                    if (btn.tags.Contains("CharacterSelection"))
                    {
                        CharacterRelated.Character cha = StaticUse.dataStorage.무장[btn.name];
                        cha.Controlable = true;
                        TimeSpan();//무한루프 시작
                        throw new Exception("무한루프 벗어남");
                    }
                    else if (btn.tags.Contains("WorldMapSelection"))
                    {
                        Console.Clear("CharacterSelection");
                        MapRelated.Location loc = StaticUse.dataStorage.맵.locationlist[btn.name];
                        UI.CharacterSelection(loc.무장).Invoke();
                    }
                    else
                    {
                        throw new Exception("ERROR");
                    }

                }
            }
        }

        public void GetDefaultSenario()
        {
            
        }

        
    }

    public class ResourceRelated
    {

        public class ResourceUnit : NameList<ResourceInfo>
        {
            public void Add(ResourceType type, int count)
            {
                base.Add(new ResourceInfo(type,count));
            }
            public void AddValue(ResourceUnit unit, bool b)
            {
                foreach(ResourceInfo info in unit)
                {
                    AddValue(info, b);
                }
            }
            public void AddValue(ResourceInfo info, bool b)
            {
                if(base.TryGetName(info.이름,out ResourceInfo origin))
                {
                    if (b)
                    {
                        origin.Count += info.Count;
                    }
                    else
                    {
                        origin.Count -= info.Count;
                    }
                }
                else
                {
                    if (b)
                    {
                        base.Add(info);
                    }
                    else
                    {
                        throw new Exception("0밑으로내려감");
                    }
                }
            }
        }
        public class ResourceInfo:Named
        {
            public int Count;
            public ResourceType type;

            public string 이름
            {
                get
                {
                    return type.이름;
                }
                set
                {
                    type.이름 = value;
                }
            }

            public ResourceInfo(ResourceType type, int count)
            {
                this.type = type;
                this.Count = count;
            }
        }

        public class ResourceType : Named
        {
            public string 이름 { get; set; }

            public string 설명;

        }
        public class DynamicResourceType : ResourceType
        {

        }
        public class StaticResourceType : ResourceType
        {

        }
    }

    public class BuildingRelated
    {
        public class BuildingUnit : NameList<BuildingInfo>
        {
            public void AddBuilding(BuildingType type)
            {
                if(base.TryGetName(type.이름,out BuildingInfo info))
                {
                    if (info.Count < type.Max)
                    {
                        info.Count++;
                    }
                }
                else
                {
                    base.Add(new BuildingInfo(type,1));
                }
            }
        }
        public class BuildingInfo : Named
        {
            public int Count;
            public BuildingType type;
            public readonly BuildingType origintype;

            public string 이름
            {
                get
                {
                    return type.이름;
                }
                set
                {
                    type.이름 = value;
                }
            }

            public BuildingInfo(BuildingType type, int Count)
            {
                this.type = type;
                this.origintype = type;
                this.Count = Count;
            }
            public void Reset()
            {
                type = origintype.Copy();
            }
        }
        public class BuildingType : Named
        {
            public string 이름 { get; set; }
            public string 설명;
            public ResourceRelated.ResourceUnit 일수입 = new ResourceRelated.ResourceUnit();
            public ResourceRelated.ResourceUnit 유지비 = new ResourceRelated.ResourceUnit();

            public ResourceRelated.ResourceUnit 고정자원사용량 = new ResourceRelated.ResourceUnit();
            public ResourceRelated.ResourceUnit 고정자원생산량 = new ResourceRelated.ResourceUnit();

            public NameList<NationRelated.Technology.SingleTech> 요구기술 = new NameList<NationRelated.Technology.SingleTech>();
            public ResourceRelated.ResourceUnit 건설비용 = new ResourceRelated.ResourceUnit();

            public int 요구노동력;

            public int Max = int.MaxValue;
        }
    }

    public interface Attribute : Named
    {
        public class Modifier<T> : Attribute
        {
            public Func<T,T> modifier;
            public Modifier(string name,Func<T, T> modifier)
            {
                this.이름 = name;
                this.modifier = modifier;
            }

            public string 이름 { get; set; }
        }
    }
    public class Information
    {
        public class CharacterInfo
        {
            public ResourceRelated.ResourceUnit 소유자원 = new ResourceRelated.ResourceUnit();
            public CharacterRelated.CharacterStat 스탯;

            DateTime RecordTime;
        }
        public class NationInfo
        {
            DateTime RecordTime;
        }
        public class CastleInfo
        {

            DateTime RecordTime;
        }

        public List<CharacterInfo> 무장 = new List<CharacterInfo>();
        public List<CastleInfo> 성 = new List<CastleInfo>();
    }

    public static class StaticUse
    {
        public static DataStorage dataStorage = new DataStorage();
    }

    public class DataStorage
    {
        public NameList<CharacterRelated.Character> 무장 = new NameList<CharacterRelated.Character>();
        public MapRelated.Map 맵 = new MapRelated.Map();
        public NameList<NationRelated.Nation> 나라 = new NameList<NationRelated.Nation>();


        public NameList<WarRelated.ArmyType> 병과 = new NameList<WarRelated.ArmyType>();
        public NameList<ResourceRelated.ResourceType> resources = new NameList<ResourceRelated.ResourceType>();
        public NameList<BuildingRelated.BuildingType> buildings = new NameList<BuildingRelated.BuildingType>();
        public NameList<NationRelated.Technology.SingleTech> techs = new NameList<NationRelated.Technology.SingleTech>();
        public NameList<UI.InCastle.Command> castlecommands = new NameList<UI.InCastle.Command>();
    }
    public class SaveFile
    {
        public int chaindex;

        public NameList<CharacterRelated.Character> 무장 = new NameList<CharacterRelated.Character>();
        public MapRelated.Map 맵 = new MapRelated.Map();
        public NameList<NationRelated.Nation> 나라 = new NameList<NationRelated.Nation>();
    }

    public class Debug
    {
        private static SenarioRelated.Senario senario = new SenarioRelated.Senario();
        public static SenarioRelated.Senario CreateKoreanWar()
        {
            
            
            senario.이름 = "한국전쟁";
            Format format = new Format();
            format.AppendLine("한국에 내전이 일어났다!");
            senario.설명 = format;

            senario.데이터.resources = GetDefaultResourceTypes();
            senario.데이터.techs = GetDefaultTechnologyTree();
            senario.데이터.buildings = GetDefaultBuildingTypes();
            senario.데이터.나라 = GetDefaultNations();
            senario.데이터.맵 = GetDefaultMap();
            senario.데이터.병과 = GetDefaultArmyType();
            senario.데이터.무장 = GetDefaultCharacter();
            senario.데이터.무장.AddRange(GetDuplicatedCharacter("안철수", 2, senario.데이터.나라["안랩"], senario.데이터.맵.locationlist["서울"]));
            senario.데이터.무장.AddRange(GetDuplicatedCharacter("홍준표", 2, senario.데이터.나라["자유한국당"], senario.데이터.맵.locationlist["부산"]));
            senario.데이터.무장.AddRange(GetDuplicatedCharacter("허경영", 2, senario.데이터.나라["국가혁명배당금당"], senario.데이터.맵.locationlist["대전"]));
            


            return senario;
        }

        public static MapRelated.Map GetDefaultMap()
        {
            MapRelated.Map map = new MapRelated.Map();
            map.이름 = "한국";
            map.locationlist = GetDefaultLocations();

            map.WorldMapSelection = () =>
            {
                return new Format("WorldMapSelection")
                    .AppendLine("■■■■■■■■")
                    .AppendLine("■서울┒　　　■")
                    .AppendLine("■　　┃　　　■")
                    .AppendLine("■　　┗대전　■")
                    .AppendLine("■　　　　┃　■")
                    .AppendLine("■　　　　┃　■")
                    .AppendLine("■　　　부산　■")
                    .AppendLine("■　　　　　　■")
                    .AppendLine("■■■■■■■■")
                    .ToButton("서울")
                    .ToButton("대전")
                    .ToButton("부산");
            };


            
            return map;
        }
        private static NameList<MapRelated.Location> GetDefaultLocations()
        {
            NameList<MapRelated.Location> loclist = new NameList<MapRelated.Location>();

            MapRelated.Castle cas;
            MapRelated.Crossroad rod = new MapRelated.Crossroad();

            cas = new MapRelated.Castle();
            cas.이름 = "서울";
            cas.resource.Add(senario.데이터.resources["인구"],10000);
            cas.resource.Add(senario.데이터.resources["경제력"], 10000);
            cas.resource.Add(senario.데이터.resources["돈"], 50000);
            cas.resource.Add(senario.데이터.resources["치안"], 900);
            cas.resource.Add(senario.데이터.resources["민심"], 600);
            cas.resource.Add(senario.데이터.resources["수용인원"], 15000);
            cas.Captured = true;
            cas.소속국가 = senario.데이터.나라["안랩"];
            
            loclist.Add(cas);

            cas = new MapRelated.Castle();
            cas.이름 = "대전";
            cas.resource.Add(senario.데이터.resources["인구"], 6500);
            cas.resource.Add(senario.데이터.resources["경제력"], 6500);
            cas.resource.Add(senario.데이터.resources["돈"], 5000);
            cas.resource.Add(senario.데이터.resources["치안"], 850);
            cas.resource.Add(senario.데이터.resources["민심"], 900);
            cas.resource.Add(senario.데이터.resources["수용인원"], 16000);
            cas.Captured = true;
            cas.소속국가 = senario.데이터.나라["국가혁명배당금당"];

            loclist.Add(cas);

            cas = new MapRelated.Castle();
            cas.이름 = "부산";
            cas.resource.Add(senario.데이터.resources["인구"], 7500);
            cas.resource.Add(senario.데이터.resources["경제력"], 7500);
            cas.resource.Add(senario.데이터.resources["돈"], 10000);
            cas.resource.Add(senario.데이터.resources["치안"], 800);
            cas.resource.Add(senario.데이터.resources["민심"], 800);
            cas.resource.Add(senario.데이터.resources["수용인원"], 13000);
            cas.Captured = true;
            cas.소속국가 = senario.데이터.나라["자유한국당"];

            loclist.Add(cas);


            MapRelated.Connection.Debug_ConnectionCreator c = new MapRelated.Connection.Debug_ConnectionCreator(loclist);
            c.c("서울", "대전", new MapRelated.Road(50, 20));
            c.c("대전", "부산", new MapRelated.Road(30, 20));


            return loclist;
        }
        public static NameList<WarRelated.ArmyType> GetDefaultArmyType()
        {
            NameList<WarRelated.ArmyType> typelist = new NameList<WarRelated.ArmyType>();

            WarRelated.ArmyType at;

            at = new WarRelated.ArmyType();
            at.이름 = "보병";
            at.징집비용.Add(senario.데이터.resources["인구"], 1);
            at.징집비용.Add(senario.데이터.resources["모집가능인구"], 1);
            at.징집비용.Add(senario.데이터.resources["돈"],10);
            at.유지비.Add(senario.데이터.resources["돈"], 1);
            at.meeleattackhuman = 10;
            at.meeleattackcastle = 10;
            at.rangeattackhuman = 0;
            at.rangeattackcastle = 0;
            at.defense = 10;
            at.speed = 10;
            at.prefermeele = true;
            typelist.Add(at);

            at = new WarRelated.ArmyType();
            at.이름 = "창병";
            at.징집비용.Add(senario.데이터.resources["인구"], 1);
            at.징집비용.Add(senario.데이터.resources["모집가능인구"], 1);
            at.징집비용.Add(senario.데이터.resources["돈"], 30);
            at.유지비.Add(senario.데이터.resources["돈"], 2);
            at.요구건물.Add(senario.데이터.buildings["대장간"]);
            at.meeleattackhuman = 18;
            at.meeleattackcastle = 10;
            at.rangeattackhuman = 0;
            at.rangeattackcastle = 0;
            at.defense = 15;
            at.speed = 9;
            at.prefermeele = true;
            typelist.Add(at);

            at = new WarRelated.ArmyType();
            at.이름 = "정찰병";
            at.징집비용.Add(senario.데이터.resources["인구"], 1);
            at.징집비용.Add(senario.데이터.resources["모집가능인구"], 1);
            at.징집비용.Add(senario.데이터.resources["돈"], 20);
            at.유지비.Add(senario.데이터.resources["돈"], 1);
            at.meeleattackhuman = 7;
            at.meeleattackcastle = 1;
            at.rangeattackhuman = 0;
            at.rangeattackcastle = 0;
            at.defense = 6;
            at.speed = 25;
            at.prefermeele = true;
            typelist.Add(at);

            at = new WarRelated.ArmyType();
            at.이름 = "기병";
            at.징집비용.Add(senario.데이터.resources["인구"], 1);
            at.징집비용.Add(senario.데이터.resources["모집가능인구"], 1);
            at.징집비용.Add(senario.데이터.resources["돈"], 50);
            at.징집비용.Add(senario.데이터.resources["말"], 1);
            at.유지비.Add(senario.데이터.resources["돈"], 10);
            at.meeleattackhuman = 25;
            at.meeleattackcastle = 13;
            at.rangeattackhuman = 0;
            at.rangeattackcastle = 0;
            at.defense = 13;
            at.speed = 40;
            at.prefermeele = true;
            typelist.Add(at);
            /*
            at = new WarRelated.ArmyType();
            at.이름 = "궁병";
            at.인구 = 1;
            at.징집비용 = 25;
            at.유지비 = 2;
            at.meeleattackhuman = 3;
            at.meeleattackcastle = 2;
            at.rangeattackhuman = 15;
            at.rangeattackcastle = 15;
            at.defense = 4;
            at.speed = 9;
            at.prefermeele = false;
            typelist.Add(at);

            at = new WarRelated.ArmyType();
            at.이름 = "투석기";
            at.인구 = 4;
            at.징집비용 = 150;
            at.유지비 = 4;
            at.meeleattackhuman = 10;
            at.meeleattackcastle = 15;
            at.rangeattackhuman = 35;
            at.rangeattackcastle = 120;
            at.defense = 10;
            at.speed = 3;
            at.prefermeele = false;
            typelist.Add(at);

            at = new WarRelated.ArmyType();
            at.이름 = "공성추";
            at.인구 = 6;
            at.징집비용 = 80;
            at.유지비 = 4;
            at.meeleattackhuman = 9;
            at.meeleattackcastle = 100;
            at.rangeattackhuman = 0;
            at.rangeattackcastle = 0;
            at.defense = 18;
            at.speed = 6;
            at.prefermeele = true;
            typelist.Add(at);
            */

            return typelist;
        }
        public static NameList<CharacterRelated.Character> GetDefaultCharacter()
        {
            NameList<CharacterRelated.Character> chalist = new NameList<CharacterRelated.Character>();

            CharacterRelated.Character cha;

            cha = new CharacterRelated.Character();
            cha.이름 = "안철수";
            cha.소유자원.Add(senario.데이터.resources["돈"],5000);
            cha.색깔 = Color.LightBlue;
            cha.스탯.전투 = 85;
            cha.스탯.방어 = 98;
            cha.스탯.지능 = 116;
            cha.스탯.최대체력 = 2000;
            cha.스탯.최대기력 = 2000;
            cha.OnRoad = false;
            cha.위치 = senario.데이터.맵.locationlist["서울"];
            cha.소속국가 = senario.데이터.나라["안랩"];
            chalist.Add(cha);

            cha = new CharacterRelated.Character();
            cha.이름 = "홍준표";
            cha.소유자원.Add(senario.데이터.resources["돈"], 2000);
            cha.색깔 = Color.DarkRed;
            cha.스탯.전투 = 110;
            cha.스탯.방어 = 91;
            cha.스탯.지능 = 95;
            cha.스탯.최대체력 = 2000;
            cha.스탯.최대기력 = 2000;
            cha.OnRoad = false;
            cha.위치 = senario.데이터.맵.locationlist["부산"];
            cha.소속국가 = senario.데이터.나라["자유한국당"];
            chalist.Add(cha);

            cha = new CharacterRelated.Character();
            cha.이름 = "허경영";
            cha.소유자원.Add(senario.데이터.resources["돈"], 3500);
            cha.색깔 = Color.Gray;
            cha.스탯.전투 = 98;
            cha.스탯.방어 = 105;
            cha.스탯.지능 = 100;
            cha.스탯.최대체력 = 2000;
            cha.스탯.최대기력 = 2000;
            cha.OnRoad = false;
            cha.위치 = senario.데이터.맵.locationlist["대전"];
            cha.소속국가 = senario.데이터.나라["국가혁명배당금당"];
            chalist.Add(cha);



            return chalist;
        }
        public static NameList<CharacterRelated.Character> GetDuplicatedCharacter(string name, int count, NationRelated.Nation nation, MapRelated.Location loc)
        {
            Random rand = new Random();
            NameList<CharacterRelated.Character> chalist = new NameList<CharacterRelated.Character>();
            CharacterRelated.Character cha;
            for (int i = 0; i < count; i++) 
            {
                cha = new CharacterRelated.Character();
                cha.이름 = name+"부하"+(i+1).ToString();
                cha.소유자원.Add(senario.데이터.resources["돈"], 100);
                cha.색깔 = Color.White;
                cha.스탯.전투 = rand.Next(70,110);
                cha.스탯.방어 = rand.Next(70, 110);
                cha.스탯.지능 = rand.Next(70, 110);
                cha.스탯.최대체력 = rand.Next(1200, 1800);
                cha.스탯.최대기력 = rand.Next(1200, 1800);
                cha.OnRoad = false;
                cha.위치 = loc;
                cha.소속국가 = nation;
                chalist.Add(cha);
            }

            return chalist;
        }
        public static NameList<NationRelated.Nation> GetDefaultNations()
        {
            NameList<NationRelated.Nation> nationlist = new NameList<NationRelated.Nation>();
            NationRelated.Nation nation;

            nation = new NationRelated.Nation();
            nation.이름 = "안랩";
            nationlist.Add(nation);

            nation = new NationRelated.Nation();
            nation.이름 = "자유한국당";
            nationlist.Add(nation);

            nation = new NationRelated.Nation();
            nation.이름 = "국가혁명배당금당";
            nationlist.Add(nation);

            return nationlist;
        }

        public static NameList<ResourceRelated.ResourceType> GetDefaultResourceTypes()
        {
            NameList<ResourceRelated.ResourceType> typelist = new NameList<ResourceRelated.ResourceType>();
            ResourceRelated.StaticResourceType stype;
            ResourceRelated.DynamicResourceType type;

            stype = new ResourceRelated.StaticResourceType();
            stype.이름 = "공간";
            stype.설명 = "성에 건물을 짓기 위한 공간입니다.";
            typelist.Add(stype);

            stype = new ResourceRelated.StaticResourceType();
            stype.이름 = "치안";
            stype.설명 = "치안";
            typelist.Add(stype);

            stype = new ResourceRelated.StaticResourceType();
            stype.이름 = "민심";
            stype.설명 = "시민들의 충성도입니다. 낮으면 시민들이 폭동을 일으키며 경제력에 큰 영향을 끼칩니다.";
            typelist.Add(stype);

            stype = new ResourceRelated.StaticResourceType();
            stype.이름 = "노동력";
            stype.설명 = "건물을 짓거나 할때 필요한 노동력입니다.\n하루에 최대로 사용할수 있는 양을 표시합니다.";
            typelist.Add(stype);

            stype = new ResourceRelated.StaticResourceType();
            stype.이름 = "연구력";
            stype.설명 = "하루에 얻을수 있는 최대 연구력입니다.\n기술을 연구할때 사용합니다.";
            typelist.Add(stype);

            stype = new ResourceRelated.StaticResourceType();
            stype.이름 = "경제력";
            stype.설명 = "매일 경제력/1000*인구 만큼 돈이 증가합니다.";
            typelist.Add(stype);

            stype = new ResourceRelated.StaticResourceType();
            stype.이름 = "수용인원";
            stype.설명 = "인구가 증가할수 있는 한계치입니다..\n인구가 이 수치를 넘어가면 안좋은 일이 많이 발생합니다.";
            typelist.Add(stype);

            stype = new ResourceRelated.StaticResourceType();
            stype.이름 = "출산율";
            stype.설명 = "인구의 증가 수치입니다.";
            typelist.Add(stype);

            stype = new ResourceRelated.StaticResourceType();
            stype.이름 = "모집가능인구";
            stype.설명 = "군대에 가고 싶어하는 인구의 수입니다. 이 수치를 넘어서 징집하면 민심이 하락합니다.";
            typelist.Add(stype);



            type = new ResourceRelated.DynamicResourceType();
            type.이름 = "돈";
            type.설명 = "money";
            typelist.Add(type);

            type = new ResourceRelated.DynamicResourceType();
            type.이름 = "인구";
            type.설명 = "population";
            typelist.Add(type);

            type = new ResourceRelated.DynamicResourceType();
            type.이름 = "노예";
            type.설명 = "noyeah";
            typelist.Add(type);

            type = new ResourceRelated.DynamicResourceType();
            type.이름 = "목재";
            type.설명 = "거의 모든 곳에 사용되는 필수 자원입니다.\n재재소를 건설해 생산할수 있습니다.";
            typelist.Add(type);

            type = new ResourceRelated.DynamicResourceType();
            type.이름 = "말";
            type.설명 = "기마병을 모집하거나 빠르게 이동할때 필요한 자원입니다.\n마구간을 건설해 생산할수 있습니다.";
            typelist.Add(type);

            type = new ResourceRelated.DynamicResourceType();
            type.이름 = "철 광석";
            type.설명 = "광산에서 채취한 철 광석입니다.\n대장간이나 제철소에서 제련할수 있습니다.";
            typelist.Add(type);

            type = new ResourceRelated.DynamicResourceType();
            type.이름 = "철";
            type.설명 = "많은 초반 병사를 생산하고 기술을 연구하는데 필요한 자원입니다.";
            typelist.Add(type);

            type = new ResourceRelated.DynamicResourceType();
            type.이름 = "강철";
            type.설명 = "강한 병사를 만들수 있으며 구조물을 강화할수 있습니다.";
            typelist.Add(type);

            type = new ResourceRelated.DynamicResourceType();
            type.이름 = "석탄";
            type.설명 = "";
            typelist.Add(type);

            return typelist;
        }
        public static NationRelated.Technology GetDefaultTechnologyTree()
        {
            NationRelated.Technology technology = new NationRelated.Technology();

            NationRelated.Technology.SingleTech tech;

            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "시작기술";
            tech.설명 = "기본적으로 가지고 시작하는 기술";
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "신학";
            tech.설명 = "성소를 지을수 있습니다.";
            tech.고정자원생산량.Add(senario.데이터.resources["민심"], 75);
            tech.고정자원생산량.Add(senario.데이터.resources["치안"], 50);
            tech.비용.Add(senario.데이터.resources["돈"],1500);
            tech.연구요구량 = 2000;
            tech.요구기술.Add(technology["시작기술"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "금속 주조";
            tech.설명 = "군사 유닛이 강화되고 창병을 생산할수 있게 됩니다.\n건축 기술을 연구했다면 대장간을 지을수 있습니다.";
            tech.비용.Add(senario.데이터.resources["돈"], 1500);
            tech.연구요구량 = 2000;
            tech.요구기술.Add(technology["시작기술"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "건축 기술";
            tech.설명 = "경찰서, 성벽, 집을 지을수 있게 되며 다른 기술을 연구하면 더 많은 건물을 지을수 있게 됩니다.";
            tech.비용.Add(senario.데이터.resources["돈"], 1500);
            tech.연구요구량 = 2000;
            tech.요구기술.Add(technology["시작기술"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "교육학";
            tech.설명 = "건축 기술을 연구했다면 대학을 지을수 있습니다.";
            tech.고정자원생산량.Add(senario.데이터.resources["민심"], 30);
            tech.비용.Add(senario.데이터.resources["돈"], 5000);
            tech.연구요구량 = 9000;
            tech.요구기술.Add(technology["신학"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "상업";
            tech.설명 = "경제규모를 크게 늘리는 시장을 지을수 있고 연결된 자국의 성 하나당 경제력이 15% 증가합니다.";
            
            tech.attributes.Add(new Attribute.Modifier<MapRelated.Castle>("상업",(cas) =>
            {
                int count = 0;
                foreach (MapRelated.Connection con in cas.connections)
                {
                    if (con.위치 is MapRelated.Castle)
                    {
                        if ((con.위치 as MapRelated.Castle).소속국가 == cas.소속국가)
                        {
                            count++;
                        }
                    }
                }

                cas.resource["경제력"].Count += 150 * count;

                return cas;
            }));
            tech.비용.Add(senario.데이터.resources["돈"], 5000);
            tech.연구요구량 = 9000;
            tech.요구기술.Add(technology["신학"]);
            tech.요구기술.Add(technology["건축 기술"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "기계";
            tech.설명 = "많은 철을 생산하는 제철소를 지을수 있습니다. 경제력이 20% 증가합니다.";
            tech.고정자원생산량.Add(senario.데이터.resources["경제력"], 200);
            tech.비용.Add(senario.데이터.resources["돈"], 5000);
            tech.연구요구량 = 9000;
            tech.요구기술.Add(technology["금속 주조"]);
            tech.요구기술.Add(technology["건축 기술"]);
            technology.Add(tech);

            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "강철";
            tech.설명 = "강철.";
            tech.비용.Add(senario.데이터.resources["돈"], 5000);
            tech.연구요구량 = 9000;
            tech.요구기술.Add(technology["금속 주조"]);
            technology.Add(tech);

            //여기서부터 르네상스 전기

            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "음향학";
            tech.설명 = "민심에 도움이 되는 건물을 지을수 있게 되며";
            tech.비용.Add(senario.데이터.resources["돈"], 15000);
            tech.연구요구량 = 25000;
            tech.요구기술.Add(technology["교육학"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "은행업";
            tech.설명 = "은행을 건설할수 있고 경제력이 50% 증가합니다. 연결된 자국의 성 하나당 경제력이 추가로 10% 증가합니다.";
            tech.고정자원생산량.Add(senario.데이터.resources["경제력"], 500);
            tech.attributes.Add(new Attribute.Modifier<MapRelated.Castle>("은행업", (cas) =>
            {
                int count = 0;
                foreach (MapRelated.Connection con in cas.connections)
                {
                    if (con.위치 is MapRelated.Castle)
                    {
                        if ((con.위치 as MapRelated.Castle).소속국가 == cas.소속국가)
                        {
                            count++;
                        }
                    }
                }

                cas.resource["경제력"].Count += 100 * count;

                return cas;
            }));
            tech.비용.Add(senario.데이터.resources["돈"], 15000);
            tech.연구요구량 = 25000;
            tech.요구기술.Add(technology["상업"]);
            tech.요구기술.Add(technology["기계"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "흑색화약";
            tech.설명 = "머스킷병을 모집할수 있습니다. 광산의 철 광석 생산량이 40% 증가합니다.";
            tech.attributes.Add(new Attribute.Modifier<MapRelated.Castle>("광산생산량40%증가", (cas) =>
            {
                foreach (BuildingRelated.BuildingInfo info in cas.building)
                {
                    if(info.이름 == "광산")
                    {
                        info.type.일수입["철 광석"].Count.Multiply(1.4);
                        return cas;
                    }
                }
                return cas;
            }));
            tech.비용.Add(senario.데이터.resources["돈"], 15000);
            tech.연구요구량 = 25000;
            tech.요구기술.Add(technology["강철"]);
            tech.요구기술.Add(technology["기계"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "물리학";
            tech.설명 = "연구소를 지을수 있고 모든 학교와 대학의 연구력이 25% 증가합니다.";
            tech.attributes.Add(new Attribute.Modifier<MapRelated.Castle>("학교와대학연구력25%증가", (cas) =>
            {
                bool schoolend = false;
                bool collageend = false;
                foreach (BuildingRelated.BuildingInfo info in cas.building)
                {
                    if (info.이름 == "학교")
                    {
                        info.type.고정자원생산량["연구력"].Count.Multiply(1.25);
                        schoolend = true;
                    }
                    else if(info.이름 == "대학")
                    {
                        info.type.고정자원생산량["연구력"].Count.Multiply(1.25);
                        schoolend = true;
                    }
                    if(schoolend && collageend)
                    {
                        return cas;
                    }
                }
                return cas;
            }));
            tech.비용.Add(senario.데이터.resources["돈"], 15000);
            tech.연구요구량 = 25000;
            tech.요구기술.Add(technology["교육학"]);
            tech.요구기술.Add(technology["기계"]);
            technology.Add(tech);

            //여기서부터 르네상스 후기

            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "인쇄 기술";
            tech.설명 = "";
            tech.고정자원생산량.Add(senario.데이터.resources["민심"], 50);
            tech.비용.Add(senario.데이터.resources["돈"], 40000);
            tech.연구요구량 = 60000;
            tech.요구기술.Add(technology["상업"]);
            tech.요구기술.Add(technology["기계"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "고급 건축";
            tech.설명 = "다양한 건물을 지을수 있게 됩니다.\n모든 건물의 공간 소요가 10% 감소합니다.";
            tech.attributes.Add(new Attribute.Modifier<MapRelated.Castle>("공간소요10%감소곱연산", (cas) =>
            {
                foreach (BuildingRelated.BuildingInfo info in cas.building)
                {
                    info.type.고정자원사용량["공간"].Count.Multiply(0.9);
                }
                return cas;
            }));
            tech.비용.Add(senario.데이터.resources["돈"], 40000);
            tech.연구요구량 = 70000;
            tech.요구기술.Add(technology["물리학"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "경제학";
            tech.설명 = "모든 건물의 돈 생산량이 25% 증가합니다.";
            tech.attributes.Add(new Attribute.Modifier<MapRelated.Castle>("건물돈생산량25%증가", (cas) =>
            {
                foreach (BuildingRelated.BuildingInfo info in cas.building)
                {
                    info.type.일수입["돈"].Count.Multiply(1.25);
                }
                return cas;
            }));
            tech.비용.Add(senario.데이터.resources["돈"], 40000);
            tech.연구요구량 = 80000;
            tech.요구기술.Add(technology["교육학"]);
            tech.요구기술.Add(technology["은행업"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "화학";
            tech.설명 = "";
            tech.비용.Add(senario.데이터.resources["돈"], 40000);
            tech.연구요구량 = 60000;
            tech.요구기술.Add(technology["흑색화약"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "금속학";
            tech.설명 = "군수 창고를 건설할수 있으며 모든 강철과 철 생산량이 10% 증가합니다.\n보병의 전투력이 20% 증가합니다.";
            tech.attributes.Add(new Attribute.Modifier<MapRelated.Castle>("금속학", (cas) =>
            {
                foreach (BuildingRelated.BuildingInfo info in cas.building)
                {
                    if(info.type.일수입.TryGetName("철",out ResourceRelated.ResourceInfo res))
                    {
                        res.Count.Multiply(1.1);
                    }

                    if (info.type.일수입.TryGetName("강철", out ResourceRelated.ResourceInfo res2))
                    {
                        res2.Count.Multiply(1.1);
                    }
                }

                //보병전투력20%증가도넣어야함
                return cas;
            }));
            tech.비용.Add(senario.데이터.resources["돈"], 40000);
            tech.연구요구량 = 60000;
            tech.요구기술.Add(technology["물리학"]);
            technology.Add(tech);

            //여기서부터 산업시대 전기

            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "비료";
            tech.설명 = "민심과 출산율이 크게 증가합니다.";
            tech.고정자원생산량.Add(senario.데이터.resources["민심"], 100);
            tech.고정자원생산량.Add(senario.데이터.resources["출산율"], 10);
            tech.비용.Add(senario.데이터.resources["돈"], 80000);
            tech.연구요구량 = 100000;
            tech.요구기술.Add(technology["화학"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "탄도학";
            tech.설명 = "소총병을 모집할수 있게 됩니다.";
            tech.비용.Add(senario.데이터.resources["돈"], 80000);
            tech.연구요구량 = 100000;
            tech.요구기술.Add(technology["물리학"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "산업화";
            tech.설명 = "석탄 채굴장과 공장을 지을수 있게 됩니다.";
            tech.비용.Add(senario.데이터.resources["돈"], 80000);
            tech.연구요구량 = 100000;
            tech.요구기술.Add(technology["인쇄 기술"]);
            tech.요구기술.Add(technology["고급 건축"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "시멘트";
            tech.설명 = "추가적인 건물을 지을수 있고 모든 건물의 공간 소요가 5% 감소합니다.";
            tech.attributes.Add(new Attribute.Modifier<MapRelated.Castle>("공간소요5%감소곱연산", (cas) =>
            {
                foreach (BuildingRelated.BuildingInfo info in cas.building)
                {
                    info.type.고정자원사용량["공간"].Count.Multiply(0.95);
                }
                return cas;
            }));
            tech.비용.Add(senario.데이터.resources["돈"], 60000);
            tech.연구요구량 = 80000;
            tech.요구기술.Add(technology["화학"]);
            tech.요구기술.Add(technology["고급 건축"]);
            technology.Add(tech);

            //여기서부터 산업시대 중기

            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "고고학";
            tech.설명 = "박물관을 지을수 있고";
            tech.비용.Add(senario.데이터.resources["돈"], 100000);
            tech.연구요구량 = 100000;
            tech.요구기술.Add(technology["인쇄 기술"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "증기기관";
            tech.설명 = "";
            tech.비용.Add(senario.데이터.resources["돈"], 120000);
            tech.연구요구량 = 120000;
            tech.요구기술.Add(technology["산업화"]);
            tech.요구기술.Add(technology["탄도학"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "화약";
            tech.설명 = "";
            tech.비용.Add(senario.데이터.resources["돈"], 120000);
            tech.연구요구량 = 120000;
            tech.요구기술.Add(technology["화학"]);
            tech.요구기술.Add(technology["산업화"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "전기";
            tech.설명 = "";
            tech.비용.Add(senario.데이터.resources["돈"], 120000);
            tech.연구요구량 = 150000;
            tech.요구기술.Add(technology["산업화"]);
            technology.Add(tech);

            //여기서부터 산업시대 후기

            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "생물학";
            tech.설명 = "";
            tech.비용.Add(senario.데이터.resources["돈"], 150000);
            tech.연구요구량 = 250000;
            tech.요구기술.Add(technology["인쇄 기술"]);
            tech.요구기술.Add(technology["비료"]);
            tech.요구기술.Add(technology["전기"]);
            technology.Add(tech);



            tech = new NationRelated.Technology.SingleTech();
            tech.이름 = "전기 시설";
            tech.설명 = "";
            tech.비용.Add(senario.데이터.resources["돈"], 150000);
            tech.연구요구량 = 350000;
            tech.요구기술.Add(technology["산업화"]);
            tech.요구기술.Add(technology["전기"]);
            technology.Add(tech);



            return technology;
        }
        public static NameList<BuildingRelated.BuildingType> GetDefaultBuildingTypes()
        {
            NameList<BuildingRelated.BuildingType> typelist = new NameList<BuildingRelated.BuildingType>();
            BuildingRelated.BuildingType type;

            type = new BuildingRelated.BuildingType();
            type.이름 = "광산";
            type.설명 = "철 광석을 생산합니다.";
            
            type.유지비.Add(senario.데이터.resources["돈"],500);

            type.일수입.Add(senario.데이터.resources["철 광석"],5);

            type.건설비용.Add(senario.데이터.resources["돈"], 15000);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 50);
            type.요구노동력 = 30000;
            type.요구기술.Add(senario.데이터.techs["시작기술"]);
            typelist.Add(type); 



            type = new BuildingRelated.BuildingType();
            type.이름 = "제재소";
            type.설명 = "목재를 생산합니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 400);

            type.일수입.Add(senario.데이터.resources["목재"], 5);

            type.건설비용.Add(senario.데이터.resources["돈"], 10000);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 30);
            type.요구노동력 = 25000;
            type.요구기술.Add(senario.데이터.techs["시작기술"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "성소";
            type.설명 = "치안과 민심과 출산율을 올립니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 250);

            type.고정자원생산량.Add(senario.데이터.resources["치안"], 8);
            type.고정자원생산량.Add(senario.데이터.resources["민심"], 8);
            type.고정자원생산량.Add(senario.데이터.resources["출산율"], 1);

            type.건설비용.Add(senario.데이터.resources["돈"], 20000);
            type.건설비용.Add(senario.데이터.resources["목재"], 20);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 30);
            type.요구노동력 = 15000;
            type.요구기술.Add(senario.데이터.techs["신학"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "마구간";
            type.설명 = "말을 생산합니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 750);

            type.일수입.Add(senario.데이터.resources["말"], 3);

            type.건설비용.Add(senario.데이터.resources["돈"], 20000);
            type.건설비용.Add(senario.데이터.resources["목재"], 60);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 20);
            type.요구노동력 = 35000;
            type.요구기술.Add(senario.데이터.techs["건축 기술"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "경찰서";
            type.설명 = "치안이 증가합니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 1500);

            type.고정자원생산량.Add(senario.데이터.resources["치안"], 75);

            type.건설비용.Add(senario.데이터.resources["돈"], 30000);
            type.건설비용.Add(senario.데이터.resources["목재"], 50);
            type.건설비용.Add(senario.데이터.resources["철"], 30);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 50);
            type.요구노동력 = 50000;
            type.요구기술.Add(senario.데이터.techs["건축 기술"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "집";
            type.설명 = "성의 수용 인원수가 증가합니다.";

            type.유지비.Add(senario.데이터.resources["돈"],20);

            type.고정자원생산량.Add(senario.데이터.resources["수용인원"], 10);

            type.건설비용.Add(senario.데이터.resources["돈"], 2500);
            type.건설비용.Add(senario.데이터.resources["목재"], 10);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 5);
            type.요구노동력 = 3000;
            type.요구기술.Add(senario.데이터.techs["건축 기술"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "학교";
            type.설명 = "경제력이 5% 증가하고 연구력이 100 증가합니다. 하지만 학생들을 노리는 범죄가 늘어나 치안이 낮아집니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 800);
            type.고정자원사용량.Add(senario.데이터.resources["치안"], 10);

            type.고정자원생산량.Add(senario.데이터.resources["연구력"], 100);
            type.고정자원생산량.Add(senario.데이터.resources["경제력"], 50);
            type.고정자원생산량.Add(senario.데이터.resources["민심"], 5);

            type.건설비용.Add(senario.데이터.resources["돈"], 50000);
            type.건설비용.Add(senario.데이터.resources["목재"], 100);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 80);
            type.요구노동력 = 60000;
            type.요구기술.Add(senario.데이터.techs["건축 기술"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "대장간";
            type.설명 = "철을 정제하며 보병과 창병을 모집할수 있습니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 250);
            type.유지비.Add(senario.데이터.resources["철 광석"], 10);

            type.일수입.Add(senario.데이터.resources["철"], 5);

            type.건설비용.Add(senario.데이터.resources["돈"], 1000);
            type.건설비용.Add(senario.데이터.resources["목재"], 50);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 30);
            type.요구노동력 = 30000;
            type.요구기술.Add(senario.데이터.techs["금속 주조"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "대학";
            type.설명 = "경제력이 15% 증가하고 연구력이 300 증가합니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 3000);

            type.고정자원생산량.Add(senario.데이터.resources["연구력"], 300);
            type.고정자원생산량.Add(senario.데이터.resources["경제력"], 150);
            type.고정자원생산량.Add(senario.데이터.resources["민심"], 15);

            type.건설비용.Add(senario.데이터.resources["돈"], 75000);
            type.건설비용.Add(senario.데이터.resources["목재"], 120);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 80);
            type.요구노동력 = 150000;
            type.요구기술.Add(senario.데이터.techs["교육학"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "시장";
            type.설명 = "돈을 많이 벌수 있으며 경제력이 10% 증가합니다.\n그런데 사람이 많이 모이는곳이라 치안이 하락합니다.";

            type.고정자원사용량.Add(senario.데이터.resources["치안"],30);

            type.일수입.Add(senario.데이터.resources["돈"], 800);
            type.고정자원생산량.Add(senario.데이터.resources["경제력"], 100);
            type.고정자원생산량.Add(senario.데이터.resources["노동력"], 20);

            type.건설비용.Add(senario.데이터.resources["돈"], 40000);
            type.건설비용.Add(senario.데이터.resources["목재"], 50);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 150);
            type.요구노동력 = 80000;
            type.요구기술.Add(senario.데이터.techs["상업"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "창관";
            type.설명 = "노예를 소모해 돈과 민심을 얻지만 치안이 낮아집니다.";

            type.유지비.Add(senario.데이터.resources["노예"], 5);
            type.고정자원사용량.Add(senario.데이터.resources["치안"], 50);

            type.일수입.Add(senario.데이터.resources["돈"], 1600);
            type.고정자원생산량.Add(senario.데이터.resources["민심"], 20);

            type.건설비용.Add(senario.데이터.resources["돈"], 20000);
            type.건설비용.Add(senario.데이터.resources["목재"], 30);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 20);
            type.요구노동력 = 15000;
            type.요구기술.Add(senario.데이터.techs["상업"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "제철소";
            type.설명 = "많은 철을 생산하며 경제력이 20% 증가합니다. 각 성당 하나만 지을수 있습니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 1000);
            type.유지비.Add(senario.데이터.resources["철 광석"], 100);

            type.일수입.Add(senario.데이터.resources["철"], 100);
            type.고정자원생산량.Add(senario.데이터.resources["경제력"], 200);

            type.건설비용.Add(senario.데이터.resources["돈"], 120000);
            type.건설비용.Add(senario.데이터.resources["목재"], 300);
            type.건설비용.Add(senario.데이터.resources["철"], 150);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 100);
            type.요구노동력 = 250000;
            type.요구기술.Add(senario.데이터.techs["기계"]);
            type.Max = 1;
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "강철 용광로";
            type.설명 = "강철을 제련합니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 700);
            type.유지비.Add(senario.데이터.resources["철"], 20);

            type.일수입.Add(senario.데이터.resources["강철"], 4);

            type.건설비용.Add(senario.데이터.resources["돈"], 50000);
            type.건설비용.Add(senario.데이터.resources["목재"], 45);
            type.건설비용.Add(senario.데이터.resources["철"], 80);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 30);
            type.요구노동력 = 65000;
            type.요구기술.Add(senario.데이터.techs["강철"]);
            typelist.Add(type);

            //여기서부터 르네상스 전기

            type = new BuildingRelated.BuildingType();
            type.이름 = "오페라 하우스";
            type.설명 = "민심과 출산율을 크게 올립니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 2500);

            type.고정자원생산량.Add(senario.데이터.resources["민심"], 320);
            type.고정자원생산량.Add(senario.데이터.resources["출산율"], 15);

            type.건설비용.Add(senario.데이터.resources["돈"], 280000);
            type.건설비용.Add(senario.데이터.resources["목재"], 800);
            type.건설비용.Add(senario.데이터.resources["철"], 450);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 350);
            type.요구노동력 = 300000;
            type.요구기술.Add(senario.데이터.techs["음향학"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "은행";
            type.설명 = "돈을 많이 벌지만 치안이 낮을경우 일정 확률로 약탈당해 돈을 크게 잃습니다.";

            type.일수입.Add(senario.데이터.resources["돈"], 3500);
            type.고정자원생산량.Add(senario.데이터.resources["경제력"], 150);
            type.고정자원생산량.Add(senario.데이터.resources["민심"], 5);

            type.건설비용.Add(senario.데이터.resources["돈"], 80000);
            type.건설비용.Add(senario.데이터.resources["목재"], 30);
            type.건설비용.Add(senario.데이터.resources["철"], 90);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 70);
            type.요구노동력 = 75000;
            type.요구기술.Add(senario.데이터.techs["은행업"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "연구소";
            type.설명 = "연구력이 1000 증가합니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 6000);

            type.고정자원생산량.Add(senario.데이터.resources["연구력"], 1000);
            type.고정자원생산량.Add(senario.데이터.resources["민심"], 5);

            type.건설비용.Add(senario.데이터.resources["돈"], 12000);
            type.건설비용.Add(senario.데이터.resources["목재"], 30);
            type.건설비용.Add(senario.데이터.resources["강철"], 90);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 60);
            type.요구노동력 = 400000;
            type.요구기술.Add(senario.데이터.techs["교육학"]);
            type.요구기술.Add(senario.데이터.techs["물리학"]);
            type.요구기술.Add(senario.데이터.techs["흑색화약"]);
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "대성당";
            type.설명 = "민심이 크게 오릅니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 1000);

            type.고정자원생산량.Add(senario.데이터.resources["민심"], 200);

            type.건설비용.Add(senario.데이터.resources["돈"], 7000);
            type.건설비용.Add(senario.데이터.resources["목재"], 80);
            type.건설비용.Add(senario.데이터.resources["철"], 90);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 100);
            type.요구노동력 = 90000;
            type.요구기술.Add(senario.데이터.techs["음향학"]);
            type.요구기술.Add(senario.데이터.techs["고급 건축"]);
            typelist.Add(type);


            /*
            type = new BuildingRelated.BuildingType();
            type.이름 = "강화 성벽";
            type.설명 = "성의 방어력이 500 증가합니다. 각 성당 하나만 지을수 있습니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 100);

            type.고정자원생산량.Add(senario.데이터.resources["성방어력"], 500);

            type.건설비용.Add(senario.데이터.resources["돈"], 30000);
            type.건설비용.Add(senario.데이터.resources["목재"], 400);
            type.건설비용.Add(senario.데이터.resources["철"], 400);
            type.건설비용.Add(senario.데이터.resources["강철"], 300);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 300);
            type.요구노동력 = 40000;
            type.요구기술.Add(senario.데이터.techs["고급 건축"]);
            type.Max = 1;
            typelist.Add(type);



            type = new BuildingRelated.BuildingType();
            type.이름 = "군수창고";
            type.설명 = "주둔해있는 모든 병사의 유지비가 50% 감소합니다. 각 성당 하나만 지을수 있습니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 75);

            type.고정자원생산량.Add(senario.데이터.resources["병사유지비감소"], 500);

            type.건설비용.Add(senario.데이터.resources["돈"], 4000);
            type.건설비용.Add(senario.데이터.resources["목재"], 130);
            type.건설비용.Add(senario.데이터.resources["철"], 80);
            type.건설비용.Add(senario.데이터.resources["강철"], 30);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 300);
            type.요구노동력 = 40000;
            type.요구기술.Add(senario.데이터.techs["고급 건축"]);
            type.Max = 1;
            typelist.Add(type);
            */


            type = new BuildingRelated.BuildingType();
            type.이름 = "공장";
            type.설명 = "노동력이 크게 증가합니다.";

            type.유지비.Add(senario.데이터.resources["돈"], 1500);

            type.고정자원생산량.Add(senario.데이터.resources["노동력"], 500);

            type.건설비용.Add(senario.데이터.resources["돈"], 80000);
            type.건설비용.Add(senario.데이터.resources["철"], 100);
            type.건설비용.Add(senario.데이터.resources["강철"], 70);
            type.고정자원사용량.Add(senario.데이터.resources["공간"], 150);
            type.요구노동력 = 100000;
            type.요구기술.Add(senario.데이터.techs["산업화"]);
            typelist.Add(type);



            return typelist;
        }

        public static NameList<UI.InCastle.Command> GetDefaultUICastleCommands()
        {

            NameList<UI.InCastle.Command> commands = new NameList<UI.InCastle.Command>();

            UI.InCastle.Command.RightSide.Button c1 = new UI.InCastle.Command.RightSide.Button();
            c1.category = "특수";
            c1.이름 = "쉰다";
            c1.ShowingName = "      쉰다";
            c1._InnerScreen = () =>
            {
                return new Format()
                    .AppendButton("[10분 쉰다]", "10")
                    .AppendLine()
                    .AppendButton("[30분 쉰다]", "30")
                    .AppendLine()
                    .AppendButton("[1시간 쉰다]", "60")
                    .AppendLine();
            };
            c1.조건 = (cha) =>
            {
                return true;
            };
            c1.ButtonClicked = 쉰다;
            IEnumerator 쉰다(WriteAble.Button btn, CharacterRelated.Character cha)
            {
                IEnumerator waiter;
                switch (btn.name)
                {
                    case "10":
                        waiter = WaitMinits(10);
                        break;
                    case "30":
                        waiter = WaitMinits(30);
                        break;
                    case "60":
                        waiter = WaitMinits(60);
                        break;
                    default:
                        throw new Exception();
                }
                DateTime time = new DateTime();
                while (waiter.MoveNext())
                {
                    if (time.Second == 0)
                    {
                        cha.스탯.체력 += 1;
                    }
                    yield return null;
                }

            }
            commands.Add(c1);



            UI.InCastle.Command.SimpleButton c2 = new UI.InCastle.Command.SimpleButton();
            c2.category = "특수";
            c2.이름 = "잔다";
            c2.ShowingName = "      잔다";
            c2.조건 = (cha) =>
            {
                if((double)cha.스탯.기력/cha.스탯.최대기력 < 0.5)
                {
                    return true;
                }
                return false;
            };
            c2.Clicked = 잔다;
            IEnumerator 잔다(CharacterRelated.Character cha)
            {
                DateTime time = new DateTime();
                while (cha.스탯.체력 < cha.스탯.최대체력 * 0.85)
                {
                    if (time.Second == 0)
                    {
                        cha.스탯.기력 += 3;
                    }
                    yield return null;
                }
            }
            commands.Add(c2);


            UI.InCastle.Command.OpenNewScreen c3 = new UI.InCastle.Command.OpenNewScreen();
            c2.category = "특수";
            c2.이름 = "월드맵";
            c2.ShowingName = "     월드맵";
            c2.조건 = (cha) =>
            {
                return true;
            };
            return commands;
        }
    }

    public interface Named : Named<string> { }
    public class NameList<T> : NameList<T, string> where T : Named { }
    public interface Named<T>
    {
        public T 이름 { get; set; }
    }
    public class NameList<T1, T2> : List<T1> where T1 : Named<T2>
    {
        public T1 this[T2 name]
        {
            get
            {
                foreach (T1 item in this)
                {
                    if (item.이름.Equals(name))
                        return item;
                }
                throw new Exception("이름을 찾을수 없습니다.");
            }
        }

        public NameList<T1, T2> FindNameAll(T2 name)
        {
            NameList<T1, T2> namelist = new NameList<T1, T2>();
            foreach (T1 item in this)
            {
                if (item.이름.Equals(name))
                    namelist.Add(item);
            }
            return namelist;
        }

        public bool CheckName(T2 name)
        {
            foreach (T1 item in this)
            {
                if (item.이름.Equals(name))
                    return true;
            }
            return false;
        }

        public bool TryGetName(T2 name, out T1 value)
        {
            foreach (T1 item in this)
            {
                if (item.이름.Equals(name))
                {
                    value = item;
                    return true;
                }
            }
            value = default;
            return false;
        }
    }

    public interface Moveable
    {
        public bool OnRoad { get; set; }
        public MapRelated.Road 도로 { get; set; }

        public int 도로위치 { get; set; }
        public MapRelated.Location 위치 { get; set; }
    }

    public class Event : Named<Event.Type>
    {
        //private bool Initialized = false;
        private IEnumerator CurrentEvent;
        

        public bool Invoke()
        {
            return CurrentEvent.MoveNext();
        }
        public Type 이름 { get; set; }

        private readonly Func<IEnumerator> EventBlock;

        public Event(Func<IEnumerator> EventBlock, Event.Type eventtype)
        {
            this.EventBlock = EventBlock;
            this.이름 = eventtype;
            this.CurrentEvent = this.EventBlock();
        }

        public enum Type
        {
            UpdateSecond,
            UpdateMinit,
            UpdateHour,
            UpdateDay,
            CastleJob
        }
    }
    public class EventCollection
    {
        public NameList<Event, Event.Type> eventcollection = new NameList<Event, Event.Type>();

        public void AddEvent(Func<IEnumerator> e, Event.Type eventtype)
        {
            eventcollection.Add(new Event(e,eventtype));
        }

        public void InvokeAllType(Event.Type eventtype)
        {
            eventcollection.FindNameAll(eventtype).ForEach((e) => e.Invoke());
        }
    }

    public static class ExtensionMethod
    {
        public static int Range(this int i,int range)
        {
            return i + rand.Next(-1*range,range);
        }

        public static int Multiply(this int i, double val)
        {
            return (int)((double)i * val);
        }
    }
}