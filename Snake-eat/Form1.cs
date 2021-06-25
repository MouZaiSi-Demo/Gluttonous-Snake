using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private class node //点类, 凡是点,使用该类
        {
            public int x;
            public int y;
            public override bool Equals(object obj)// 重载判等函数
            {
                return ((node)obj).x == x && ((node)obj).y == y;
            }
            public override int GetHashCode()// 重写哈希函数
            {
                return base.GetHashCode();
            }
        }

        #region 声明变量
        const int size_body = 15;
        const int size_space = 10;
        static Size size_map;// mapsize的 height and width
        const int scope = 15;// 图的范围
        const int interval = 20;// 线程间隔
        Pen pen = new Pen(Color.Red);
        SolidBrush sb = new SolidBrush(Color.Pink);
        SolidBrush sb_tou = new SolidBrush(Color.Red);
        node food = new node();// 食物node
        //  int[,] map = new int[scope, scope];
        List<node> que = new List<node>();//queue and map is used to BFS
        //int[,] dir = { { 0, -1 }, { 0, 1 }, { -1, 0 }, { 1, 0 } };
        int[,] dir = { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 0 } };//up,down,left,right, 等偏移量
        #endregion

        #region 游戏持续进行, 最后最好的状态是所有的点都被蛇和食物占领, 头会无限追尾, 但因为没有完成哈密顿回路, 所以有可能会有部分点因为会造成死路, 所以没有被经过
        private void NewGame()
        {
            node begain = new node() { x = 5, y = 5 };
            que.Add(begain);
            node begain2 = new node() { x = 4, y = 5 };
            que.Add(begain2);


            NewFood();

            while (que[0].x != que[que.Count - 1].x || que[0].y != que[que.Count - 1].y)
            {
                Movebody(que, Strategy(), false);
            }
        }
        #endregion 

        #region Create_NewFood
        private void NewFood()
        {
            int fx, fy;
            int[,] can = new int[scope, scope];
            foreach (node item in que)// 遍历队列, 判断是否食物与身体重叠
            {
                can[item.x, item.y] = 1;
            }
            Random rd = new Random();
            do
            {
                fx = rd.Next(0, scope);
                fy = rd.Next(0, scope);
                food.x = fx;
                food.y = fy;
            } while (can[fx, fy] == 1);
            ReFreshScreen();
            //刷新
        }
        #endregion

        #region 战略部署(追食物战术和追尾战术)
        private node Strategy()
        {
            List<node> FindFood = null;// 
            List<node> FindTail = null;
            node return_value = null;// 返回值
            //FindFood = BFS(que, food, que[0]);
            FindFood = BFS(que, que[0], food);
            if (FindFood != null)
            {
                FindTail = Movesnake(FindFood);
                if (FindTail != null)
                {
                    return_value = FindFood[FindFood.Count - 1];
                    //return FindFood[FindFood.Count - 1];
                }
            }
            bool foodflag = true, tailflag = true;
            if (FindFood == null)
            {
                foodflag = false;
            }
            else if (FindFood.Count == 0)
            {
                foodflag = false;
            }

            if (FindTail == null)
            {
                tailflag = false;
            }
            else if (FindTail.Count == 0)
            {
                tailflag = false;
            }

            if (foodflag == false || tailflag == false || return_value == null)
            {
                return_value = Faraway(1);
            }
            return return_value;
        }

        #endregion

        #region 移动以及吃食物(会调用曼哈顿距离处理)
        private void Movebody(List<node> snake, node step, bool istest)
        {
            while (step == null)
            {
                // step = Faraway(1);
                //List<node> stepway = new List<node>();
                //stepway = BFS(que, que[0], food);
                // step = stepway[stepway.Count - 1];
                step = Strategy();// 策略一波最终得到的点
                //if (step == null)
                //{
                //    step = Faraway(1);
                //}
            }
            int len = snake.Count;
            len = len - 1;
            if (step.x == food.x && step.y == food.y)// 吃到食物
            {
                len++;
                node eat = new node { x = food.x, y = food.y };
                snake.Add(eat);
                if (istest == false)
                {
                    NewFood();
                }
            }
            for (int i = len; i > 0; i--)
            {
                snake[i].x = snake[i - 1].x;
                snake[i].y = snake[i - 1].y;
            }
            snake[0].x = step.x;
            snake[0].y = step.y;
            if (istest == false)
            {
                ReFreshScreen();

                //----------刷新---------+sleep---//
            }

        }
        


        private List<node> Movesnake(List<node> road)
        {
            List<node> testQue = new List<node>();
            foreach (node item in que)
            {
                node nitem = new node() { x = item.x, y = item.y };
                testQue.Add(nitem);
            }

            for (int i = road.Count - 1; i >= 0; i--)
            {
                Movebody(testQue, road[i], true);//初始逻辑是frue;
            }

            List<node> isFindTail = BFS(testQue, testQue[0], testQue[testQue.Count - 1]);
            //List<node> isFindTail = BFS(testQue, testQue[testQue.Count - 1], testQue[0]);
            return isFindTail;
        }

        #endregion

        #region 曼哈顿距离的处理
        private node Faraway(int num)
        {
            node[] director = new node[4];
            double[] distance = new double[4];
            int nx = que[0].x;
            int ny = que[0].y;
            int fx = food.x;
            int fy = food.y;
            for (int i = 0; i <= 3; i++)
            {
                   distance[i] = Math.Abs(nx + dir[i, 0] - fx) + Math.Abs(ny + dir[i, 1] - fy);
                node nnode = new node { x = nx + dir[i, 0], y = ny + dir[i, 1] };
                if (!que.Contains(nnode) || (nnode.x == que[que.Count - 1].x && nnode.y == que[que.Count - 1].y))
                {
                    if (nnode.x >= 0 && nnode.x < scope && nnode.y >= 0 && nnode.y < scope)
                    {
                        director[i] = nnode;
                    }
                }
            }
            for (int i = 0; i < distance.Length - 1; i++)
            {
                for (int j = 0; j < distance.Length - 1 - i; j++)
                {
                    if (distance[j] < distance[j + 1])
                    {
                        double temp = distance[j];
                        node tem = director[j];
                        distance[j] = distance[j + 1];
                        director[j] = director[j + 1];
                        distance[j + 1] = temp;
                        director[j + 1] = tem;
                    }
                }
            }

            for (int i = 0; i <= 3; i++)
            {
                if (director[i] != null)
                {
                    List<node> testQue2 = new List<node>();
                    foreach (node item in que)
                    {
                        node nnode = new node() { x = item.x, y = item.y };
                        testQue2.Add(nnode);
                    }
                    Movebody(testQue2, director[i], true);
                    List<node> farway = BFS(testQue2, director[i], que[que.Count - num]);
                    // List<node> farway = BFS(que, food, director[i]);
                    if (farway != null)
                    {
                        return director[i];
                    }
                }

            }
            return null;
        }

        #endregion

        #region BFS搜索路径
        private List<node> BFS(List<node> snake, node s, node e)
        {
            int st = 0;
            Random rd = new Random();
            st = rd.Next(0, 4);
            int[,] now = new int[scope, scope]; //搜索地图
            foreach (node item in snake)
            {
                now[item.x, item.y] = 1;
            }
            now[snake[snake.Count - 1].x, snake[snake.Count - 1].y] = 0;


            int f = 1;  //头指针
            int t = 1;  //尾指针
            node[] serch = new node[scope * scope + 5]; //bfs队列
            int[] preindex = new int[scope * scope + 5]; //前驱
            List<node> backway = new List<node>();
            serch[t] = s;
            t++;
            if (s.x == e.x && s.y == e.y)
            {
                backway.Add(e);
                return backway;
            }
            do
            {
                for (int i = 0 + st; i <= 3 + st; i++)
                {
                    int nx = serch[f].x + dir[i, 0];
                    int ny = serch[f].y + dir[i, 1];
                    if (nx < 0 || nx >= scope || ny < 0 || ny >= scope) continue;
                    //node cur = new node() { x = nx, y = ny };
                    //if ((!snake.Contains(cur) && now[nx, ny] != 1) || (nx == e.x && ny == e.y))  
                    if (now[nx, ny] != 1 || (nx == e.x && ny == e.y))
                    {
                        node n = new node();
                        n.x = nx;
                        n.y = ny;
                        now[nx, ny] = 1;
                        serch[t] = n;   //入队
                        preindex[t] = f;   //记录前驱
                        if (n.x == e.x && n.y == e.y)
                        {
                            while (preindex[t] != 0)
                            {
                                backway.Add(serch[t]);
                                t = preindex[t];
                            }
                            return backway;
                        }
                        t++;
                    }
                }
                f++;
            } while (f < t);
            return null;

        }
        // 绘图类实例化
        static Bitmap bmp = new Bitmap(size_body * (scope + size_space * 2), size_body * (scope + size_space * 2));
        static Graphics g = Graphics.FromImage(bmp);
        #endregion

        #region 刷新
        private void ReFreshScreen()
        {
            g.Clear(Color.White);
            int[,] mapint = new int[scope, scope];
            foreach (node item in que)
            {
                mapint[item.x, item.y] = 4;//身体
                //Console.Write(item.x + "," + item.y + " ");
            }
            for (int i = 0; i < que.Count; i++)
            {
                if (i == 0)// 此时头暂时不处理
                {
                    continue;
                }
                else
                {
                    if (que[i].x == que[i - 1].x)
                    {
                        if (que[i].y == que[i - 1].y + 1)
                        {
                            g.FillRectangle(sb, que[i].x * (size_body + size_space), que[i].y * (size_body + size_space) - size_space, size_body, size_space);
                        }
                        else if (que[i].y == que[i - 1].y - 1)
                        {
                            g.FillRectangle(sb, que[i].x * (size_body + size_space), que[i].y * (size_body + size_space) + size_body, size_body, size_space);
                        }
                    }
                    else if (que[i].y == que[i - 1].y)
                    {
                        if (que[i].x == que[i - 1].x + 1)
                        {
                            g.FillRectangle(sb, que[i].x * (size_body + size_space) - size_space, que[i].y * (size_body + size_space), size_space, size_body);
                        }
                        else if (que[i].x == que[i - 1].x - 1)
                        {
                            g.FillRectangle(sb, que[i].x * (size_body + size_space) + size_body, que[i].y * (size_body + size_space), size_space, size_body);
                        }

                    }
                }
            }
            //Console.WriteLine();
            mapint[que[0].x, que[0].y] = 1;//头
            mapint[food.x, food.y] = 2;//食物
            for (int i = 0; i < scope; i++)
            {
                for (int j = 0; j < scope; j++)
                {
                    if (mapint[j, i] == 4)//身体
                    {
                        g.FillRectangle(sb, j * (size_body + size_space), i * (size_body + size_space), size_body, size_body);
                    }
                    else if (mapint[j, i] == 1)//头
                    {
                        g.FillRectangle(sb_tou, j * (size_body + size_space), i * (size_body + size_space), size_body, size_body);
                    }
                    else if (mapint[j, i] == 2)//食物
                    {
                        g.FillEllipse(sb, j * (size_body + size_space), i * (size_body + size_space), size_body, size_body);
                    }

                }
            }
            pictureBox1.Image = bmp;
            Thread.Sleep(interval);
        }
        #endregion

        #region 窗口启动初始化(建图, 线程 )
        private void Form1_Load(object sender, EventArgs e)
        {
            size_map = new Size(scope * (size_body + size_space), scope * (size_body + size_space));
            pictureBox1.Size = size_map;
            pictureBox1.Location = new Point(0, 0);
            bmp = new Bitmap(size_map.Width, size_map.Height);
            g = Graphics.FromImage(bmp);
            Thread t = new Thread(NewGame);
            t.Start();// 操作系统开始给权限

        }
        #endregion
    }
}
