using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class GameOne : Form
    {
        public GameOne()
        {
            InitializeComponent();
        }

        #region Variable declarations
        //绘制地图
        int mapX = 40, mapY = 20;//设定游戏地图边界
        Label[,] mapArray;//定义地图格子的二维数组
        List<int> snakeX = new List<int> { 0, 1, 2 },//snakeX[snakeX.Count-1]是蛇头, 初始值为2。(下标)
                 snakeY = new List<int> { 1, 1, 1 };//利用List<int>存放蛇身(下标)
        int foodX, foodY;//食物的坐标点
        string kCode = "D";//控制蛇移动的方向,初始值向右
        //level等级snakeX.Count>=10
        int[] level = { 300, 250, 200, 150, 100 };//值是timer的时间，等级越高时间越短
        int k = 0;//等级

        //得分
        int Score = 0;
        //历史最高分
        int MaxScore;
        //游戏时间
        int GameTime = 0;
        //存储最高分记录的文件路径
        string path = "MaxScoreOne.dat";

        //字符串队列，键盘每次的输入则入队
        string ClikRecord = "";
        #endregion

        #region Game_Load_initialization
        private void Game_Load(object sender, EventArgs e)
        {

            //从文件中读取历史最高分
            GetMaxScore();
            //创建地图
            GreateMap();
            //创建贪吃蛇
            GreateSnake();
            //创建食物
            GreateFood();

            //贪吃蛇开始移动
            timer1.Start();//计时器开启

            //初始化状态栏
            label1.Text = "当前得分：" + Score.ToString();
            label2.Text = "当前等级：" + (k + 1).ToString();
            label3.Text = "游戏时间：" + SecToHMS(GameTime);
            label4.Text = "历史最高分：" + MaxScore.ToString();
        }
        #endregion

        #region Timer event

        private void timer1_Tick(object sender, EventArgs e)
        {
            //状态栏保持更新
            label1.Text = "当前得分：" + Score.ToString();
            label2.Text = "当前等级：" + (k + 1).ToString();

            if (Score % 10 == 0 && Score / 10 <= 2)
            {
                k = Score / 10; //当得分在20以内时，设置等级
                timer1.Interval = level[k]; //根据不同的等级设置不同的interval值，下同
            }
            if (Score % 10 == 0 && Score / 10 == 4)
            {
                k = Score / 20 + 1;//设置等级4（k = 3）
                timer1.Interval = level[k];
            }
            if (Score % 10 == 0 && Score / 10 == 6)
            {
                k = Score / 20 + 1;//设置等级5（k = 4）
                timer1.Interval = level[k];
            }
            if (Score % 10 == 0 && Score / 10 >= 9 && (Score / 10) % 3 == 0)
            {
                k = Score / 30 + 2; //每增加30得分升级
            }
            if (ClikRecord.Length > 0)
            {
                kCode = ClikRecord.Substring(0, 1);//取第一个字符的字符串
                ClikRecord = ClikRecord.Substring(1, ClikRecord.Length - 1);
            }
            SnakeMove();//蛇移动方法
        }
        //时间刷新
        private void timer2_Tick(object sender, EventArgs e)
        {
            GameTime++;
            label3.Text = "游戏时间：" + SecToHMS(GameTime);
        }

        #endregion

        #region time form transformation
        private string SecToHMS(long duration)
        {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(duration));
            string str = "";
            if (ts.Hours > 0)
            {
                str = String.Format("{0:00}", ts.Hours) + ":" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = "00:" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = "00:00:" + String.Format("{0:00}", ts.Seconds);
            }
            return str;
        }
        #endregion

        #region create_Map
        private void flpMap_Paint(object sender, PaintEventArgs e)
        {

        }
        private void GreateMap()
        {
            mapArray = new Label[mapY, mapX];//Label是格子
            for (int y = 0; y < mapY; y++)
            {
                for (int x = 0; x < mapX; x++)
                {
                    Label lb1 = new Label();
                    lb1.Width = 20;
                    lb1.Height = 20;
                    lb1.BackColor = Color.DarkCyan;
                    lb1.Margin = new Padding(0);//设置label之间不要距离
                    //lb1.Text = x.ToString();
                    flpMap.Controls.Add(lb1);
                    mapArray[y, x] = lb1;
                }
            }
        }
        #endregion

        #region BulidSnake
        private void GreateSnake()
        {
            for (int i = 0; i < snakeX.Count; i++)
            {
                mapArray[snakeY[i], snakeX[i]].BackColor = Color.Blue;//label数组mapArry,根据蛇身的下标，改变label的颜色当作蛇身
            }
            mapArray[snakeY[snakeY.Count - 1], snakeX[snakeX.Count - 1]].BackColor = Color.Black;
            //mapArray[snakeY[snakeY.Count - 1], snakeX[snakeX.Count - 1]].Text = "U1";
            //mapArray[snakeY[snakeY.Count - 1], snakeX[snakeX.Count - 1]].ForeColor = Color.White;
        }
        #endregion

        #region ClearSnake
        private void ClearSnake()
        {
            for (int i = 0; i < snakeX.Count; i++)
            {
                mapArray[snakeY[i], snakeX[i]].BackColor = Color.DarkCyan;//label数组mapArry,根据蛇身的下标，改变label的颜色当作蛇身
            }
            //mapArray[snakeY[snakeY.Count - 1], snakeX[snakeX.Count - 1]].Text = "";
        }
        #endregion

        #region Create food
        private void GreateFood()
        {
            bool flag = false;//标记蛇身体是否和新创建的食物重合
            do
            {
                flag = false;
                Random r = new Random();
                foodX = r.Next(mapX);
                foodY = r.Next(mapY);
                for (int i = 0; i < snakeX.Count; i++)// 
                {
                    if (snakeX[i] == foodX && snakeY[i] == foodY)//如果食物与蛇身重叠
                    {
                        flag = true;
                        break;
                    }
                }

            } while (flag);//若flag为true，则表示重叠，则重新循环

            mapArray[foodY, foodX].BackColor = Color.Red;
        }
        #endregion

        #region Movew_snake
        private void SnakeMove()
        {
            //1、擦除
            ClearSnake();
            //MessageBox.Show("Moving");
            //2、修改坐标
            if (kCode != "P" && kCode != "L")
            {
                for (int i = 0; i < snakeX.Count - 1; i++)//snakeX.Count-1忽略头部
                {
                    //数组往前覆盖实现
                    snakeX[i] = snakeX[i + 1];
                    snakeY[i] = snakeY[i + 1];
                }

                switch (kCode)
                {
                    case "A":
                        snakeX[snakeX.Count - 1]--;//头部坐标减1
                        break;
                    case "W":
                        snakeY[snakeX.Count - 1]--;
                        break;
                    case "D":
                        snakeX[snakeX.Count - 1]++;
                        break;
                    case "S":
                        snakeY[snakeX.Count - 1]++;
                        break;
                }
            }
            //3、吃食物
            if (EatFood())//若吃到了食物
            {
                Score++;//得分加1
                snakeX.Add(0);
                snakeY.Add(0);//增加一个元素，值先不管，就按0吧
                //蛇身还原
                for (int i = snakeY.Count - 1; i > 0; i--)
                {
                    snakeX[i] = snakeX[i - 1];
                    snakeY[i] = snakeY[i - 1];
                }
                GreateFood();//重新生成一个食物
            }

            //4、判断是否咬到自己的身体（只需判断头部与身体是否重叠即可）
            for (int i = 0; i < snakeX.Count - 1; i++)
            {
                if (snakeX[snakeX.Count - 1] == snakeX[i] &&
                    snakeY[snakeX.Count - 1] == snakeY[i] )
                {
                    timer1.Stop();
                    //PlayerMusic(Sound.gameover);//播放死亡的音乐
                    MessageBox.Show("你把自己咬死了！！！");
                    if (Score > MaxScore)//如果打破了记录
                    {
                        BreadARecord();//将新纪录写入文件
                    }
                    this.Close();
                    return;
                }
            }
            //5、判断蛇是否超出边界
            if (snakeX[snakeX.Count - 1] < 0 ||
                snakeY[snakeX.Count - 1] < 0 ||
                snakeX[snakeX.Count - 1] > mapX - 1 ||
                snakeY[snakeX.Count - 1] > mapY - 1)
            {
                timer1.Stop();
                MessageBox.Show("你撞墙上了！！！");
                if (Score > MaxScore)//如果打破了记录
                {
                    BreadARecord();//将新纪录写入文件
                }
                this.Close();
                return;
            }
            //6、重新显示
            GreateSnake();
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            string newKey = e.KeyCode.ToString();
            //注意，当蛇向右时不能直接向左。同理，其他情况也类似
            List<string> list = new List<string>() { "A", "W", "D", "S", "P", "L" };// 用于判断是否为该六个键值
            if (list.Contains(newKey) == false)//若按下的键不是这六个键的话
            {
                return;
            }
            if (kCode == "A" && newKey == "D" ||
               kCode == "D" && newKey == "A" ||
               kCode == "W" && newKey == "S" ||
               kCode == "S" && newKey == "W") // 屏蔽ad ws的情况
            {
                return;
            }
            if (newKey == "P")
            {
                timer1.Stop();
                timer2.Stop();
                return;
            }
            if (newKey == "L")
            {
                timer1.Start();
                timer2.Start();
                return;
            }
            //修复BUG：
            //这里一个BUG的情况：在游戏时，假如蛇的方向为D，快速敲击键盘W、D时，
            //程序会来不及进入timer事件，这时蛇的状态不能经历W，直接到D，造成后面的程序
            //会判断蛇咬到了自己。
            //办法：加入string类型的ClikRecord当作键盘输入队列，在timer事件中取其第一个字符。
            //同时仍要加入kCode=newKey，若不加的话，则前面的kcode可能会等于现在的newkey，即使他们没有挨着
            kCode = newKey;
            ClikRecord = ClikRecord + newKey;
        }
        #endregion

        #region Judge if you're eating your food
        private bool EatFood()
        {
            if (snakeX[snakeX.Count - 1] == foodX && snakeY[snakeX.Count - 1] == foodY)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Get the highest score in history
        private void GetMaxScore()
        {
            if (File.Exists(path) == false)//若不存在此文件
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(0);
                MaxScore = 0;
                fs.Close();
                bw.Close();
            }
            else
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                MaxScore = br.ReadInt32();
                fs.Close();
                br.Close();
            }
        }
        #endregion

        #region The act of breaking a record
        private void BreadARecord()
        {
            //将新纪录写入文件
            FileStream fs = new FileStream(path, FileMode.Truncate, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(Score);
            fs.Close();
            bw.Close();
            //告诉玩家这个好消息
            MessageBox.Show("恭喜你，创造了新纪录！！！");
        }
        //关闭窗体即停止游戏
        private void GameOne_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            timer2.Stop();
        }
        #endregion
    }
}
