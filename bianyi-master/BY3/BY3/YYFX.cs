using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BY3
{
    struct FuHao               //符号表
    {
        public string name;
        public int offset;
        public string leixin;
        public int width;
        public FuHao(string name, int offset, string leixin,int w)
        {
            this.name = name;
            this.offset = offset;
            this.leixin = leixin;
            this.width = w;
        }
    }
    class YYFX               //语义分析类
    {
        private static String zxin = "", name = "",fname="", bxin = "",  sxin = "",  xin = "",  Ez = "", Fz = "", Tz = "", Rz = "", Le = "", Lin = "", wb = "", we = "",p="",pq="",bj="",wy="",shuZuName="",fhxin="",canshuzu="",hanshuname="";
        public static int line = 0;                                   //行号
        private static int zw = 0, sw = 0, w = 0, offset = 0, tempi = 0, sd = 0, nw = 0;//sd数组维数(shuzu dimensionality)，一维数组二维数组等，不是偏移量；偏移量是wy
        private static Stack<string> biaodashi= new Stack<string>();  //表达式堆栈
        public static string yuyi = "";                               //语义
        public static string errIn = "";                              //错误
        public static Hashtable fuhaoBiao = new Hashtable();          //符号表，用hash表储存
        public static void setInit()
        {
            zxin = "";//指针类型
            name = ""; //变量名称
            fname = "";//变量名称
            bxin = ""; //变量类型
            sxin = ""; //数组类型
            xin = "";  //泛指敲定的最后类型
            tempi = 0; //临时变量
            wy = "";   //数组的偏移量
            shuZuName = "";   //shuZuName：数组名
            Ez = ""; Fz = ""; Tz = ""; Rz = ""; //加减乘除的操作数
            Le = ""; Lin = ""; wb = ""; we = "";//if条件句跳转true时行号，false时行号；while语句跳转true 时行号，false行号
            p = ""; pq = ""; bj = "";            //boolean判断式；比较符号
            fhxin = "";                          //符号类型
            canshuzu = "";                       //参数组
            hanshuname = "";                     //函数名
            line = 0; zw = 0; sw = 0;              //行号，指针宽度，数组宽度
            w = 0;                               //w是字节数，即类型占多少字节 ，如int占4节，w=4
            offset = 0;                          //offset是偏移量
            sd = 0; nw = 0;                      //数组维数，数组总宽度
            biaodashi = new Stack<string>();     //表达式栈
            yuyi = "";                           //语义
            errIn = "";                          //错误
            fuhaoBiao = new Hashtable();         //符号表是一个hash表
        }
        //新建一个临时变量
        private static string newTmp()
        {
            return ("t" + tempi++);
        }
        //得到前一个或几个的临时变量值，一般用于赋值语句的语义分析
        private static string getTmp(int o)
        {
            return ("t" + (tempi - o));
        }
        //调用语义子程序的函数，在语法分析时同步调用，即可边进行语法分析，边得到语义分析结果
        //i 得到具体要调用哪个语义子程序
        //n 调用时变量等的值
        //行号，报错时会用到
        public static void run(int i, String n,string el)
        {

            switch (i)
            {
                case 1:              //由于主函数以及其他函数比较特殊，涉及到int main，main是一个标识符，但却不需要进行语义分析。所以将其类型赋值为特殊
                    zxin = "teshu";
                    zw = 3;
                    break;
                case 2:
                    name = n;        //a.value，将函数中的函数名，即一个标识符赋值给name变量
                    break;
                case 3://char
                    bxin = "char";
                    w = 1;           //变量宽度为1
                    break;
                case 4://double
                    bxin = "double";
                    w = 8;          //变量宽度为8
                    break;
                case 5://float
                    bxin = "float";
                    w = 8;
                    break;
                case 6://int
                    bxin = "int";
                    w = 4;
                    break;
                case 7://long
                    bxin = "long";
                    w = 4;
                    break;
                case 8://short
                    bxin = "short";
                    w = 4;
                    break;
                case 9://指针
                    zxin = "pointer( "+zxin+" )"; //类型是指针
                    zw = 4;                   //指针占多少字节
                    break;
                case 10://数组
                    sxin = sxin.Replace(bxin, "array("+int.Parse(n)+", "+bxin+")");//敲定类型，int.Parse(n)是数组大小，bxin是数组类型
                    sw = int.Parse(n) * sw;   //数组占多少个字节，即程序一开始要为数组分配多少内存；n的值就是数组[]内的值
                 //   yuyi += "" + (++line) + "\t" + newTmp() + " = " + int.Parse(n) + "\r\n";//
                   // yuyi += "" + (++line) + "\t" + newTmp() + " = " + shuZuName + "[" + getTmp(2) + "]\r\n";//
                   // Console.WriteLine("Hello World!");
                    break;
                case 11://数组
                    sxin = bxin;    //数组的类型，int，char等
                    sw = w;         //数组的宽度
                    break;
                case 12: //数组初始化，紧接11
                    xin = zxin.Replace("teshu", sxin); //确定类型，将得到的sxin替换teshu得到新属性
                    w = (4 - zw) * (sw - 3) + zw;  //重新计算数组所占内存大小，zw为指针宽度，变量宽度=(4-指针宽度)*(数组宽度-3)+指针宽度；zw被初始化为3
                    if (fuhaoBiao[name] == null)   //如果该数组在fuhaobiao中不存在，就将其添加至fuhaobiao中
                    {
                        fuhaoBiao[name] = new FuHao(name, offset, xin, w);
                        offset += w;
                    }
                    else    //变量重复声明时报错
                    {
                        w = 0;
                        xin = "";
                        errIn += "line " + el + "\t变量 " + name + " 重复声明\r\n";
                    }
                    break;
                case 13:   //老规矩，不知道时就先用teshu替代
                    zxin="teshu";
                    zw = 3;
                    break;
                case 14:  //已经知道到底是什么类型了，名称和所占的内存大小也知道了
                    name=n;//名称
                    sxin=bxin; //类型
                    sw = w;  //位移，内存
                    break;
                case 15:
                    xin = zxin.Replace("teshu", sxin); //找出真正的类型
                    w = (4 - zw) * (sw - 3) + zw;     //计算一个这样的符号应占多少内存
                    if (fuhaoBiao[name] == null)
                    {
                        fuhaoBiao[name] = new FuHao(name, offset, xin, w);//将变量加入符号表中
                        offset += w;
                    }
                    else
                    {
                        w = 0;
                        xin = "";
                        errIn += "line "+el+"\t变量 "+name+" 重复声明\r\n";
                    }
                    break;
                case 16: //赋值语句，类似 t3 = 0 ， a（这是个变量） = t3
                    yuyi += "" + (++line) + "\t" + fname + " = " + getTmp(1) + "\r\n";
                    break;
                case 17: //得到变量的名称
                    fname = name;
                    break;
                case 18: //和17配套，作用同16
                    yuyi += "" + (++line) + "\t" + fname + " = " + getTmp(1) + "\r\n";
                    break;
                case 19: //例如，int a = 0；那么case 19就是t0 = 0，t0是一个临时变量
                    yuyi += "" + (++line) + "\t" + newTmp() + " = " + n + "\r\n";
                    break;
                case 20: //将表达式得到的一个临时变量重新赋给一个新的临时变量，如t5 = t3*t4，t6 = t5这里表达的就是t6=t5这一层含义
                    yuyi += "" + (++line) + "\t" + newTmp() + " = " + Tz + "\r\n";
                    break;
                case 21:
                    Ez = Tz;  //
                    break;
                case 22://加法减法 左部 Ez  右部  Tz
                    yuyi += "" + (++line) + "\t" + newTmp() + " = " + Ez + " + " + Tz + "\r\n";
                    Tz = getTmp(1);
                    break;
                case 23:
                    Ez = Tz;
                    break;
                case 24:
                    yuyi += "" + (++line) + "\t" + newTmp() + " = " + Ez + " - " + Tz + "\r\n";
                    Tz = getTmp(1);
                    break;
                case 25:
                    Tz = Fz;
                    break;
                case 26: //乘法操作前变量赋值
                    Rz = Fz;
                    break;
                case 27://乘法除法  的左部Rz，开始乘法操作
                    yuyi += "" + (++line) + "\t" + newTmp() + " = " + Rz + " * " + Fz + "\r\n";
                    Fz = getTmp(1);
                    break;
                case 28:
                    Rz = Fz;
                    break;
                case 29:
                    yuyi += "" + (++line) + "\t" + newTmp() + " = " + Rz + " / " + Fz + "\r\n";
                    Fz = getTmp(1);
                    break;
                case 30://四则表达式的值可能是int值
                    Fz = n;
                    break;
                case 31://四则表达式的值可能是float值
                    Fz = n;
                    break;
                case 32: //将得到的表达式中各个部分的值压入堆栈备用
                    biaodashi.Push(Ez);
                    biaodashi.Push(Tz);
                    biaodashi.Push(Fz);
                    biaodashi.Push(Rz);
                    break;
                case 33: //使用堆栈中的变量，进行出栈操作
                    Tz = biaodashi.Pop();
                    Ez = biaodashi.Pop();
                    Rz = biaodashi.Pop();
                    Fz = biaodashi.Pop();
                    Fz = getTmp(1);
                    break;
                case 34:  //在不知道数组大小时先初始化为0
                    //wy = 0;
                    sd = 0;
                    break;
                case 35:
                    //Fz = name;
                    Fz = getTmp(1);
                    break;
                case 36:
                    biaodashi.Push(Ez);
                    biaodashi.Push(Tz);
                    biaodashi.Push(Fz);
                    biaodashi.Push(Rz);
                    //yuyi += "" + (++line) + "\t" + newTmp() + " = " + wy + "\r\n";
                    sd++;
                    break;
                case 37: //使用数组
                    Rz = biaodashi.Pop();
                    Fz = biaodashi.Pop();
                    Tz = biaodashi.Pop();
                    Ez = biaodashi.Pop();
                    FuHao f = (FuHao)fuhaoBiao[shuZuName];
                    String [] s = f.leixin.Split(", ".ToCharArray()); //将数组类型分类出来，数组类型一般为array（5，int）这样
                    try
                    {
                        String ss=s[2 * sd - 2];   //一维就取出S[0]，二维取出S[1]，以此类推。
                        ss = ss.Substring(6, ss.Length - 6);  
                        nw = nw/int.Parse(ss);  //数组总占内存数/数组的的元素总量即为当前每个元素所占的大小
                        yuyi += "" + (++line) + "\t" + newTmp() + " = " + getTmp(2) + " * " + nw + "\r\n";
                        if (sd > 1)  //数组维数大于1维
                        {
                            yuyi += "" + (++line) + "\t" + newTmp() + " = " + getTmp(2) + " + " + wy + "\r\n";
                        }
                        wy = getTmp(1);
                    }
                    catch
                    {
                        nw = 0;
                        errIn += "line " + el + "\t变量 " + shuZuName + " 数组引用错误\r\n";
                    }
                    break;
                case 38: //假如符号表中没有该变量，就说明该变量是未声明就直接使用了，此时应输出异常
                    if (fuhaoBiao[n] == null)
                    {
                        errIn += "line " + el + "\t变量 " + name + " 没有声明即使用\r\n";
                    }
                    shuZuName = n;
                    nw = ((FuHao)fuhaoBiao[n]).width;  //符号表中存储的一个该类型的符号所占内存大小
                    break;
                case 39:       //if条件句，p代表判断条件，panduan
                    yuyi += "" + (++line) + "\tif " + p + " goto " + (line + 2) + "\r\n";
                    yuyi += "" + (++line) + "\tgo1to Le \r\n";
                    break;
                case 40:       //如果判断条件实效，和case 39配套
                    yuyi += "" + (++line) + "\tgo2to Lin \r\n";
                    break;
                case 41:      //回填对应的行号
                    Le=""+(line+1);
                    yuyi = yuyi.Replace("Le", Le);
                    Lin=""+(line+1);
                    yuyi = yuyi.Replace("Lin", Lin);
                    break;
                case 42:     //else语句块中语句的行号指示
                    Le=""+(line+1);
                    yuyi = yuyi.Replace("Le", Le);
                    Lin=""+(line+1);
                    yuyi = yuyi.Replace("Lin", Lin);
                    break;
                case 43: //while循环
                    yuyi += "" + (++line) + "\tif " + p + " goto " + (line + 2) + "\r\n"; //条件正确的话去那一行
                    yuyi += "" + (++line) + "\tgo3to We \r\n";                            //条件错误的话去那一行
                    biaodashi.Push(wb);//将条件正确时跳转到的行号压入biaodashi栈留作后用
                    break;
                case 44:
                    wb = biaodashi.Pop();//弹出wb
                    yuyi += "" + (++line) + "\tgoto " + wb + " \r\n";  //这是是真跳出循环，不加的话会顺序执行，所以必须要加
                    we = "" + (line + 1);
                    yuyi = yuyi.Replace("We", we);                     //回填行号
                    break;
                case 45:   //do while
                    wb = "" + (line + 1);//找到条件正确时要跳转的行号
                    biaodashi.Push(wb);  //压入栈留作备用
                    break;
                case 46:  //do while
                    wb = biaodashi.Pop();
                    yuyi += "" + (++line) + "\tif " + p + " goto " + wb + "\r\n"; //while（）中的条件，正确的话跳转到wb
                    break;
                case 47: //条件判断
                    p = "" + getTmp(1) + "!=0";
                    break;
                case 48:
                    pq = getTmp(1);
                    break;
                case 49: //最后得到的结果形如 p = t11<t10
                    p = pq + bj + getTmp(1);
                    break;
                case 50: //比较符号 bj means比较，n代表具体的符号
                    bj = n;
                    break;
                case 51: //比较
                    bj = n;
                    break;
                case 52: //比较
                    bj = n;
                    break;
                case 53: //比较
                    bj = n;
                    break;
                case 54: //比较
                    bj = n;
                    break;
                case 55: //比较
                    bj = n;
                    break;
                case 56://真正的else语句，接case40
                    Le=""+(line+1);
                    yuyi = yuyi.Replace("Le", Le); //le才是真正else语句中的内容所对应的行号
                    break;
                case 57:  //while循环
                    wb = "" + (line + 1);
                    break;
                case 58:   //变量，数组等一系列初始化的开始，现将变量名赋值给fname
                    fname = name;
                    break;
                case 59:
                    if (sd > 0) //数组维数如果大于一维的话
                    {  //wy代表数组的偏移
                        yuyi += "" + (++line) + "\t" + newTmp() + " = " + shuZuName + "[" + wy + "]" + "\r\n";
                        
                        FuHao ff = (FuHao)fuhaoBiao[shuZuName];
                        
                        String [] sss = ff.leixin.Split(", ".ToCharArray()); //举例，array[5,int]
                        String ss22;
                        
                        try
                        {
                            ss22 = sss[2 * sd]; //得到数组类型，int或float
                        }
                        catch
                        {
                            errIn += "line " + el + "\t变量 " + shuZuName + " 引用错误\r\n";
                            return;
                        }
                        
                        if (ss22.Substring(0, 3).Equals("int") || ss22.Substring(0, 5).Equals("float"))
                        {
                        }
                        else
                        {
                            errIn += "line " + el + "\t变量 " + shuZuName + " 未引用到基本类型\r\n";
                        }
                         

                    }
                    else
                    {
                        yuyi += "" + (++line) + "\t" + newTmp() + " = " + shuZuName + "\r\n";
                    }
                    break;
                case 60:          //将case 2中的IDN，即函数的名称赋值给hanshuname，原来的name是泛指，这里就变成特指函数名了
                    hanshuname = name;
                    fhxin = zxin.Replace("teshu", bxin);
                    canshuzu = "";//此时还不知道具体的参数，暂且赋值为空
                    break;
                case 61:         //类型未知时先赋值为teshu
                    zxin = "teshu";
                    break;
                case 62:        //数组的具体类型
                    sxin = bxin;
                    break;
                case 63:        //将原来的未知类型teshu替换成已知的数组类型
                    String canshu = zxin.Replace("teshu", sxin);
                    if (canshuzu.Equals("")) //假如参数组中没有任何内容
                    {
                        canshuzu = canshu;
                    } 
                    else                     //否则的话，在原有内容的基础上加入新内容
                    {
                        canshuzu = canshuzu + "X" + canshu;
                    }
                    break;
                case 64:  
                    if (fuhaoBiao[hanshuname] == null)   
                    {
                        if (canshuzu.Equals(""))  //如果参数组中没有任何值，就将其赋值为“空”
                        {
                            canshuzu = "空";
                        }
                        fuhaoBiao[hanshuname] = new FuHao(hanshuname, offset, canshuzu + "->" + fhxin, 4);
                        offset += 4;
                    }
                    else //出现错误
                    {
                        errIn += "line " + el + "\t变量 " + name + " 重复声明\r\n";
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
