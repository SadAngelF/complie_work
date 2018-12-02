using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace BY3
{
    //存储终结符与非终结符的结构体
    struct WordCode2
    {
        String value;     //值
        String name;      //名称
        int id;           //种别码，编号
        String descri;    //描述
        public int line;  //行号
        ArrayList first;  //first集
        ArrayList follow; //follow集
        //构造方法 1 初始化参数没有行号时
        public WordCode2(String name, int id, String descri, String value)
        {
            this.name = name;
            this.id = id;
            this.descri = descri;
            this.value = value;
            first = new ArrayList();
            follow = new ArrayList();
            line = -1;
        }
        //构造方法 2 初始化参数有行号时
        public WordCode2(String name, int id, String descri, String value, int line)
        {
            this.name = name;
            this.id = id;
            this.descri = descri;
            this.value = value;
            first = new ArrayList();
            follow = new ArrayList();
            this.line = line;
        }
        //构造方法 3 初始化参数只有名称，编号，描述时
        public WordCode2(String name, int id, String descri)
        {
            this.name = name;
            this.id = id;
            this.descri = descri;
            this.value = "";
            first = new ArrayList();
            follow = new ArrayList();
            line = -1;
        }
        //属性对应的get方法
        public String getName()
        {
            return name;
        }
        public int getId()
        {
            return id;
        }
        public String getDecri()
        {
            return descri;
        }
        public String getValue()
        {
            return value;
        }
        //对first集合的相关操作
        public void addFirst(String f)
        {
            first.Add(f);
        }
        public void addFirst(ArrayList f)
        {
            foreach (Object o in f)
            {
                if (!this.first.Contains(o))
                {
                    this.first.Add(o);
                }
            }
        }
        public void clearFirst(String f)
        {
            this.first = new ArrayList();
        }
        public ArrayList getFirst()
        {
            return first;
        }
        //对follow集合的相关操作
        public void addFollow(String f)
        {
            follow.Add(f);
        }
        public void addFollow(ArrayList f)
        {
            foreach (Object o in f)
            {
                if (!this.follow.Contains(o))
                {
                    this.follow.Add(o);
                }
            }
        }
        public void clearFollow(String f)
        {
            this.follow = new ArrayList();
        }
        public ArrayList getFollow()
        {
            return follow;
        }
    }
    //句法分析类
    class JFFX
    {
        private Stack<WordCode2> input;     //输入缓冲区
        private Stack<WordCode2> derivating;//推导符号串
        private Hashtable nTerminals;       //非终结符集
        private Hashtable terminals;        //终结符集
        private Hashtable products;         //产生式集
        private Hashtable products3;        //含代码产生式集
        private Hashtable deriTable;        //预测分析表
        //构造函数
        public JFFX()
        {
            input = new Stack<WordCode2>();
            derivating = new Stack<WordCode2>();
            //初始化非终结符
            nTerminals = new Hashtable();
            initNTerminal();
            //初始化终结符
            terminals = new Hashtable();
            initTerminal();
            //初始化产生式集
            products = new Hashtable();
            initProducts();
            products3 = new Hashtable();
            initProducts3();
            //计算每个符号的first集
            getFirstX();
            //计算每个非终结符的follow集
            getFollowX();
            //计算每个产生式的select集
            getSelect();
            //预测分析表
            deriTable = new Hashtable();
            initDeriTable();
        }
        /*
         * 句法分析函数
         * phrase 词法分析结果
         * sen 句法分析输出
         * errIn 错误信息
         * */
        public void fenxi(String phrase, out String sen, out String errIn)
        {
            //初始化
            sen = "";
            errIn = "";
            //格式化词法分析结果
            readInput(phrase);
            
            //控制程序
            YYFX.setInit();
            while (input.Count > 0)
            {
                if (derivating.Count < 1)
                {
                    errIn += "系统错误" + "\r\n";
                    break;
                }
                WordCode2 x = derivating.Pop();//推导符号串出栈
                derivating.Push(x);
                WordCode2 a = input.Pop();     //输入缓冲区出栈
                input.Push(a);
                if (x.getId() > 490)          //语义子程序的编号
                {
                    derivating.Pop();
                    YYFX.run(int.Parse(x.getName()), a.getValue(),""+a.line);
                }
                else if (x.getId() < 290)       //终结符
                {
                    if (x.getName().Equals(a.getName())) //如果输入与推导符号串栈中的匹配
                    {
                        derivating.Pop();                //将该符号从推导符号串栈中弹出
                        input.Pop();                     //从输入缓冲中弹出
                    }
                    else
                    {
                        errIn += ("line " + a.line + "\t输入 " + WFormat.name(a) + "不符合语法规则" + "\r\n");
                        //input.Pop();
                        derivating.Pop();
                        //break;
                    }
                }
                else                         //非终结符
                {
                    Product p = (Product)(((Hashtable)deriTable[x.getName()])[a.getName()]);//从预测分析表取出
                    if (p == null)
                    {
                        errIn += "系统错误" + "\r\n";
                        input.Pop();
                    }
                    else if (p.getLeft().Equals("synch"))
                    {
                        errIn += "错误：分析栈顶为 <" + x.getName() + "> 不可推导输入栈顶 " + a.getName() + "\r\n";
                        derivating.Pop();
                    }
                    else if (p.getLeft().Equals("error"))
                    {
                        errIn += "错误：输入栈顶为 " + a.getName() + " 不可用分析栈顶 <" + x.getName() + "> 规约\r\n";
                        input.Pop();
                    }
                    else//输出语法分析结果
                    {
                        sen += "line "+a.line+"\t<" + x.getName() + "> -> ";
                        derivating.Pop();
                        ArrayList right = p.getRight();
                        //把产生式右部压入栈
                        for (int i = 0; i < right.Count; i++)
                        {
                            Object o = nTerminals[right[right.Count - i - 1]];
                            if (o != null)
                            {
                                WordCode2 w = (WordCode2)o;
                                if (!w.getName().Equals("空"))
                                {
                                    derivating.Push(w);
                                }
                            }
                            else
                            {
                                o = terminals[right[right.Count - i - 1]];
                                if (o != null)
                                {
                                    derivating.Push((WordCode2)o);
                                }
                                else
                                {
                                    derivating.Push(new WordCode2((String)(right[right.Count - i - 1]),500,"代码"));
                                }
                            }
                        }
                        //输出产生式
                        for (int i = 0; i < right.Count; i++)
                        {
                            Object o = nTerminals[right[i]];
                            if (o != null)
                            {
                                sen += "<" + right[i] + "> ";
                            }
                            else
                            {
                                o = terminals[right[i]];
                                if (o != null)
                                {
                                    sen += "" + right[i] + " ";
                                }
                            }
                        }
                        sen += "\r\n";
                    }
                }
            }


            errIn += "\r\n语义分析错误:\r\n";
            errIn += YYFX.errIn;
            
        }
        //初始化非终结符
        void initNTerminal()
        {
            //从code文件夹中的nTerminals读入所有非终结符，共32个
            StreamReader r = new StreamReader(@"..\\..\\code\\nTerminals.txt");
            String temp;
            for (int i = 0; i < 100; i++)     //编号从300开始，分配100个
            {
                temp = r.ReadLine();
                if (temp == null || temp.Equals(""))
                {
                    break;
                }
                nTerminals[temp] = new WordCode2(temp, 300 + i, temp);
            }
            r.Close();
        }
        //初始化终结符
        void initTerminal()
        {
            terminals["结束"] = new WordCode2("结束", -1, "结束");
            //从code文件夹中的Terminals读入所有终结符，共39个
            StreamReader r = new StreamReader(@"..\\..\\code\\terminals.txt");
            String temp;
            for (int i = 0; i < 100; i++)
            {
                temp = r.ReadLine();
                if (temp == null || temp.Equals(""))
                {
                    break;
                }
                terminals[temp] = new WordCode2(temp, i, temp); //编号从0开始，分配100个
            }
            r.Close();
        }
        //初始化产生式
        /*
         * products 为hashtable
         * 每一项存储一个非终结符的推导
         * key为非终结符名称
         * value为一个arraylist<Product>
         * Product类存储一个右部，而且包含select集
         * 语义分析之前使用的产生式
         * */
        void initProducts()
        {
            String left = "";
            ArrayList cans;
            ArrayList right;

            left = "开始";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("基本类型");
            right.Add("指针");
            right.Add("IDN");
            right.Add("A");
            right.Add("开始");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add(";");
            right.Add("开始");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "A";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("(");
            right.Add("参数组");
            right.Add(")");
            right.Add("{");
            right.Add("语句组");
            right.Add("}");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("数组");
            right.Add("初始化");
            right.Add("后变量");
            right.Add(";");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "后变量";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add(",");
            right.Add("变量");
            right.Add("初始化");
            right.Add("后变量");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "初始化";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("=");
            right.Add("表达式");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "指针";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("*");
            right.Add("指针");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "语句块";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("语句");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("{");
            right.Add("语句组");
            right.Add("}");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "语句组";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("语句");
            right.Add("语句组");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "语句";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("基本类型");
            right.Add("变量");
            right.Add("初始化");
            right.Add("后变量");
            right.Add(";");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("分支");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("循环");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            //right.Add("使用变量");
            right.Add("IDN");
            right.Add("=");
            right.Add("表达式");
            right.Add(";");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add(";");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "表达式";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("STRING");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("T");
            right.Add("E");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "E";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("+");
            right.Add("T");
            right.Add("E");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("-");
            right.Add("T");
            right.Add("E");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "T";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("F");
            right.Add("R");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "R";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("*");
            right.Add("F");
            right.Add("R");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("/");
            right.Add("F");
            right.Add("R");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "F";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("INT");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("FLOAT");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("(");
            right.Add("表达式");
            right.Add(")");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("使用变量");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "参数组";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("参数");
            right.Add("后参数组");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "后参数组";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add(",");
            right.Add("参数");
            right.Add("后参数组");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "参数";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("基本类型");
            //right.Add("61");
            right.Add("指针");
            //right.Add("62");
            right.Add("IDN");
            right.Add("数组");
            //right.Add("63");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "分支";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("if");
            right.Add("(");
            right.Add("判断");
            right.Add(")");
            right.Add("语句块");
            right.Add("分支1");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "分支1";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("else");
            right.Add("语句块");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "循环";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("while");
            right.Add("(");
            right.Add("判断");
            right.Add(")");
            right.Add("语句块");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("do");
            right.Add("语句块");
            right.Add("while");
            right.Add("(");
            right.Add("判断");
            right.Add(")");
            right.Add(";");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("for");
            right.Add("(");
            right.Add("语句");
            right.Add("判断1");
            right.Add(";");
            right.Add("语句3");
            right.Add(")");
            right.Add("语句块");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "语句3";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("使用变量");
            right.Add("=");
            right.Add("表达式");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "使用变量";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("IDN");
            right.Add("使用数组");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "变量";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("指针");
            right.Add("IDN");
            right.Add("数组");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "数组";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("[");
            right.Add("INT");
            right.Add("]");
            right.Add("数组");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "使用数组";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("[");
            right.Add("表达式");
            right.Add("]");
            right.Add("使用数组");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "基本类型";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("char");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("double");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("float");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("int");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("long");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("short");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "判断";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("表达式");
            right.Add("后判断");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "后判断";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("比较符号");
            right.Add("表达式");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "比较符号";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add(">=");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("<=");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("==");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("!=");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("<");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add(">");
            cans.Add(new Product(left, right));
            products[left] = cans;

            left = "判断1";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("判断");
            cans.Add(new Product(left, right));
            products[left] = cans;
        }
        //初始化产生式列表
        //加入语义子程序编号后的产生式，语义子程序在YYFX中实现，共64个
        void initProducts3()
        {
            String left = "";
            ArrayList cans;
            ArrayList right;

            left = "开始";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("基本类型");
            right.Add("1");
            right.Add("指针");
            right.Add("2");
            right.Add("IDN");
            right.Add("A");
            right.Add("开始");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add(";");
            right.Add("开始");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "A";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("60");
            right.Add("(");
            right.Add("参数组");
            right.Add(")");
            right.Add("64");
            right.Add("{");
            right.Add("语句组");
            right.Add("}");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("11");  //在YYFX中case 中能够确定数组的类型
            right.Add("数组");
            right.Add("12"); //数组的初始化
            right.Add("初始化");
            right.Add("后变量");
            right.Add(";");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "后变量";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add(",");
            right.Add("变量");
            right.Add("初始化");
            right.Add("后变量");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "初始化";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("58");
            right.Add("=");
            right.Add("表达式");
            right.Add("16");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "指针";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("*");
            right.Add("9");
            right.Add("指针");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "语句块";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("语句");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("{");
            right.Add("语句组");
            right.Add("}");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "语句组";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("语句");
            right.Add("语句组");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "语句";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("基本类型");
            right.Add("变量");
            right.Add("初始化");
            right.Add("后变量");
            right.Add(";");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("分支");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("循环");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            //right.Add("使用变量");
            right.Add("IDN");
            right.Add("17");
            right.Add("=");
            right.Add("表达式");
            right.Add("18");
            right.Add(";");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add(";");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "表达式";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("19");
            right.Add("STRING");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("T");
            right.Add("E");
            right.Add("20");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "E";  //加，减运算的表达式，优先级较低，中间包含乘除的表达式，乘除优先级高
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("+");
            right.Add("21");
            right.Add("T");
            right.Add("22");
            right.Add("E");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("-");
            right.Add("23");
            right.Add("T");
            right.Add("24");
            right.Add("E");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "T";        //包含乘除的部分
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("F");
            right.Add("R");
            right.Add("25");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "R";       //乘法，除法表达式部分
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("*");
            right.Add("26");
            right.Add("F");
            right.Add("27");
            right.Add("R");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("/");
            right.Add("28");
            right.Add("F");
            right.Add("29");
            right.Add("R");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "F";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("30");
            right.Add("INT");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("31");
            right.Add("FLOAT");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("32");
            right.Add("(");
            right.Add("表达式");
            right.Add(")");
            right.Add("33");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("34");
            right.Add("使用变量");
            right.Add("35");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "参数组";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("参数");
            right.Add("后参数组");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "后参数组";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add(",");
            right.Add("参数");
            right.Add("后参数组");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "参数";
            cans = new ArrayList();
            right = new ArrayList();
            //right.Add("基本类型");
            //right.Add("变量");
            right.Add("基本类型");
            right.Add("61");
            right.Add("指针");
            right.Add("62");
            right.Add("IDN");
            right.Add("数组");
            right.Add("63");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "分支";
            cans = new ArrayList();
            right = new ArrayList();
            //right.Add("56");
            right.Add("if");
            right.Add("(");
            right.Add("判断");
            right.Add(")");
            right.Add("39");
            right.Add("语句块");
            right.Add("40");
            right.Add("分支1");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "分支1";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("else");
            right.Add("56");
            right.Add("语句块");
            right.Add("42");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("空");
            right.Add("41");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "循环";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("while");
            right.Add("57");
            right.Add("(");
            right.Add("判断");
            right.Add(")");
            right.Add("43");
            right.Add("语句块");
            right.Add("44");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("do");
            right.Add("45");
            right.Add("语句块");
            right.Add("while");
            right.Add("(");
            right.Add("判断");
            right.Add(")");
            right.Add(";");
            right.Add("46");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("for");
            right.Add("(");
            right.Add("语句");
            right.Add("判断1");
            right.Add(";");
            right.Add("语句3");
            right.Add(")");
            right.Add("语句块");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "语句3";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("使用变量");
            right.Add("=");
            right.Add("表达式");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "使用变量";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("38");
            right.Add("IDN");
            right.Add("使用数组");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "变量";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("13");
            right.Add("指针");
            right.Add("14");
            right.Add("IDN");
            right.Add("数组");
            right.Add("15");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "数组";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("[");
            right.Add("10");
            right.Add("INT");
            right.Add("]");
            right.Add("数组");
           // right.Add("使用数组"); //////
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "使用数组";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            right.Add("59");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("36");
            right.Add("[");
            right.Add("表达式");
            right.Add("]");
            right.Add("37");
            right.Add("使用数组");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "基本类型";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("char");
            right.Add("3");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("double");
            right.Add("4");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("float");
            right.Add("5");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("int");
            right.Add("6");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("long");
            right.Add("7");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("short");
            right.Add("8");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "判断";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("表达式");
            right.Add("后判断");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "后判断";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            right.Add("47");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("比较符号");
            right.Add("48");
            right.Add("表达式");
            right.Add("49");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "比较符号";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("50");
            right.Add(">=");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("51");
            right.Add("<=");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("52");
            right.Add("==");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("53");
            right.Add("!=");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("54");
            right.Add("<");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("55");
            right.Add(">");
            cans.Add(new Product(left, right));
            products3[left] = cans;

            left = "判断1";
            cans = new ArrayList();
            right = new ArrayList();
            right.Add("空");
            cans.Add(new Product(left, right));
            right = new ArrayList();
            right.Add("判断");
            cans.Add(new Product(left, right));
            products3[left] = cans;
        }
        //计算first集
        void getFirstX()
        {
            //终结符
            foreach (DictionaryEntry d in terminals)
            {
                WordCode2 w = (WordCode2)d.Value;
                w.addFirst(w.getName());
            }
            //非终结符
            //X->a..
            foreach (DictionaryEntry d in products)
            {
                ArrayList a = (ArrayList)d.Value;
                WordCode2 n = (WordCode2)nTerminals[(String)d.Key];
                foreach (Product p in a)
                {
                    String f = (String)((ArrayList)p.getRight())[0];
                    if (terminals[f] != null)
                    {
                        n.addFirst(f);
                    }
                }
            }
            //X->Y..
            Boolean end = false;
            while (!end)
            {
                end = true;
                //遍历产生式
                foreach (DictionaryEntry d in products)
                {
                    ArrayList a = (ArrayList)d.Value;
                    WordCode2 n = (WordCode2)nTerminals[(String)d.Key];
                    foreach (Product p in a)
                    {
                        ArrayList right = (ArrayList)p.getRight();
                        String f = (String)(right)[0];
                        //X->Y..
                        if (nTerminals[f] != null)
                        {
                            WordCode2 w = (WordCode2)nTerminals[f];
                            foreach (String s in w.getFirst())
                            {
                                if (!n.getFirst().Contains(s))
                                {
                                    end = false;
                                    n.addFirst(s);
                                }
                            }
                        }
                        //X->Y..
                        //Y->空
                        for (int i = 0; i < right.Count && !((String)(right)[i]).Equals("空") && nTerminals[(String)(right)[i]] != null; i++)
                        {
                            //判断是否能否退出空
                            Boolean isNull = false;
                            foreach (Product ap in (ArrayList)products[(String)(right)[i]])
                            {
                                if (ap.getRight().Count == 1 && ap.getRight()[0].Equals("空"))
                                {
                                    isNull = true;
                                    break;
                                }
                            }
                            if (isNull)
                            {
                                WordCode2 w = (WordCode2)nTerminals[(String)(right)[i]];
                                foreach (String s in w.getFirst())
                                {
                                    if (!n.getFirst().Contains(s))
                                    {
                                        end = false;
                                        n.addFirst(s);
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }

                        }

                    }
                }
            }

        }
        //计算符号串的first集
        ArrayList getFirstXs(ArrayList a)
        {
            ArrayList b = new ArrayList();
            if (a.Count < 1)
            {
                return b;
            }
            //初始化
            //first(a) = first(X1)
            Object o = nTerminals[(String)a[0]];
            if (o != null)
            {
                WordCode2 w = (WordCode2)o;
                foreach (String s in w.getFirst())
                {
                    if (!b.Contains(s))
                    {
                        b.Add(s);
                    }
                }
            }
            else
            {
                o = terminals[(String)a[0]];
                if (o != null)
                {
                    WordCode2 w = (WordCode2)o;
                    foreach (String s in w.getFirst())
                    {
                        if (!b.Contains(s))
                        {
                            b.Add(s);
                        }
                    }
                }
            }
            //for
            for (int i = 0; i < a.Count-1 && !((String)(a)[i]).Equals("空") && nTerminals[(String)(a)[i]] != null; i++)
            {
                Boolean isNull = false;
                foreach (Product ap in (ArrayList)products[(String)(a)[i]])
                {
                    if (ap.getRight().Count == 1 && ap.getRight()[0].Equals("空"))
                    {
                        isNull = true;
                        break;
                    }
                }
                if (isNull)
                {
                    Object oo = nTerminals[(String)(a)[i+1]];
                    if (oo != null)
                    {
                        WordCode2 w = (WordCode2)oo;
                        foreach (String s in w.getFirst())
                        {
                            if (!b.Contains(s))
                            {
                                b.Add(s);
                            }
                        }
                    }
                    else
                    {
                        oo = terminals[(String)(a)[i + 1]];
                        if (oo != null)
                        {
                            WordCode2 w = (WordCode2)oo;
                            foreach (String s in w.getFirst())
                            {
                                if (!b.Contains(s))
                                {
                                    b.Add(s);
                                }
                            }
                        }
                    }
                }
                else
                {
                    break;
                }

            }
            return b;
        }
        //计算符号的follow集
        void getFollowX()
        {
            WordCode2 w = (WordCode2)nTerminals["开始"];
            w.addFollow("结束");
            //A->..B..
            //非右部最后一个符号
            foreach (DictionaryEntry d in products)
            {
                ArrayList a = (ArrayList)d.Value;
                WordCode2 n = (WordCode2)nTerminals[(String)d.Key];
                foreach (Product p in a)//遍历产生式
                {
                    ArrayList right = p.getRight();
                    foreach (String b in right)
                    {
                        if (right.IndexOf(b) < right.Count - 1)//如果不是右部最后一个符号
                        {
                            ArrayList aa = new ArrayList();
                            for (int i = right.IndexOf(b) + 1; i < right.Count; i++)
                            {
                                aa.Add(right[i]);
                            }
                            ArrayList fs = getFirstXs(aa);
                            Object oo = nTerminals[b];
                            if (oo != null)//当前为非终结符，则添加
                            {
                                WordCode2 ww = (WordCode2)nTerminals[b];
                                foreach (String c in fs)
                                {
                                    if (!ww.getFollow().Contains(c))
                                    {
                                        ww.addFollow(c);
                                    }
                                }
                            }
                        }

                    }
                }
            }
            //A->..B
            //右部最后一个符号
            Boolean end = false;
            while (!end)
            {
                end = true;
                foreach (DictionaryEntry d in products)
                {
                    ArrayList a = (ArrayList)d.Value;
                    WordCode2 n = (WordCode2)nTerminals[(String)d.Key];
                    foreach (Product p in a)//遍历产生式
                    {
                        ArrayList right = p.getRight();
                        String b = (String)right[right.Count - 1];//右部最后一个元素
                        Object oo = nTerminals[b];
                        if (oo != null)
                        {
                            WordCode2 ww = (WordCode2)nTerminals[b];
                            foreach (String c in n.getFollow())
                            {
                                if (!ww.getFollow().Contains(c))
                                {
                                    end = false;
                                    ww.addFollow(c);
                                }
                            }
                            //当前符号的右边能推为空
                            for (int i = right.Count - 1; i > 0 && !((String)(right)[i]).Equals("空") && nTerminals[(String)(right)[i]] != null; i--)
                            {
                                //是否能为空
                                Boolean isNull = false;
                                foreach (Product ap in (ArrayList)products[(String)(right)[i]])
                                {
                                    if (ap.getRight().Count == 1 && ap.getRight()[0].Equals("空"))
                                    {
                                        isNull = true;
                                        break;
                                    }
                                }
                                if (isNull)//是空的话，添加
                                {
                                    String bb = (String)right[i - 1];
                                    Object ooo = nTerminals[bb];
                                    if (ooo != null)
                                    {
                                        WordCode2 www = (WordCode2)ooo;
                                        foreach (String c in n.getFollow())
                                        {
                                            if (!www.getFollow().Contains(c))
                                            {
                                                end = false;
                                                www.addFollow(c);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
            }

        }
        //计算产生式select集
        void getSelect()
        {
            foreach (DictionaryEntry d in products)
            {
                ArrayList a = (ArrayList)d.Value;
                WordCode2 n = (WordCode2)nTerminals[(String)d.Key];
                foreach (Product p in a)//遍历产生式集
                {
                    //如果右部为空
                    ArrayList right = p.getRight();
                    if (right.Count == 1 && right[0].Equals("空"))
                    {
                        p.addSelect(n.getFollow());
                    }
                    else//如果右部不为空
                    {
                        p.addSelect(getFirstXs(right));
                    }
                }
            }
        }
        //计算预测分析表
        void initDeriTable()
        {
            foreach (DictionaryEntry d1 in nTerminals)
            {
                if (d1.Key.Equals("空"))
                    continue;
                ArrayList a = (ArrayList)products[(String)d1.Key];
                ArrayList b = (ArrayList)products3[(String)d1.Key];
                for (int i = 0; i < a.Count;i++ )
                {
                    ((Product)b[i]).addSelect(((Product)a[i]).getSelect());
                }
            }

            foreach (DictionaryEntry d1 in nTerminals)
            {
                if (d1.Key.Equals("空"))
                    continue;
                deriTable[d1.Key] = new Hashtable();
                foreach (DictionaryEntry d2 in terminals)
                {
                    ArrayList a = (ArrayList)products3[(String)d1.Key];
                    if (a == null)
                        continue;
                    Boolean isSelect = false;
                    foreach (Product p in a)
                    {
                        if (p.getSelect().Contains(d2.Key))
                        {
                            ((Hashtable)deriTable[d1.Key])[d2.Key] = p;
                            isSelect = true;
                            break;
                        }
                    }
                    if (!isSelect)
                    {
                        ArrayList a2 = ((WordCode2)nTerminals[d1.Key]).getFollow();
                        foreach (String s1 in a2)
                        {
                            if (s1.Equals(d2.Key))
                            {
                                ((Hashtable)deriTable[d1.Key])[d2.Key] = new Product("synch", null);
                                isSelect = true;
                                break;
                            }
                        }
                    }
                    if (!isSelect)
                    {
                        ((Hashtable)deriTable[d1.Key])[d2.Key] = new Product("error", null);
                    }
                }
            }
        }
        //格式化词法分析结果为句法分析输入
        private void readInput(String phrase)
        {
            Stack<WordCode2> sw = new Stack<WordCode2>();
            //正则匹配
            MatchCollection mc = Regex.Matches(phrase, "line [0-9]+\t(.+)\t[(] (.+)+, [0-9]+, (.+)");
            foreach (Match m in mc)
            {
                String a = m.Value;
                a = a.Substring(5, a.Length - 5);
                String[] s = a.Split(new char[] { '\t' });
                s[2] = s[2].Substring(0, s[2].Length - 2);
                s[2] = s[2].Substring(2, s[2].Length - 2);
                String[] t = s[2].Split(new char[] { ',', ' ' });

                //s[0] line s[1] value t[0]name t[2] id t[4] description
                WordCode2 w;
                try
                {
                    w = new WordCode2(t[0], int.Parse(t[2]), t[4], s[1], int.Parse(s[0]));
                }
                catch
                {
                    w = new WordCode2(",", int.Parse(t[3]), t[5], s[1], int.Parse(s[0]));
                }
                sw.Push(w);
            }
            //逆向入栈
            input.Push((WordCode2)terminals["结束"]);
            while (sw.Count > 0)
            {
                input.Push(sw.Pop());
            }
            derivating.Push((WordCode2)terminals["结束"]);
            derivating.Push((WordCode2)nTerminals["开始"]);
        }
    }
}
