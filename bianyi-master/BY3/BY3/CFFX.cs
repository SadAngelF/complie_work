using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BY3
{
    struct WordCode
    {
        String word;       //名称
        int id;            //种别码，编号
        String descri;     //描述
        public WordCode(String word, int id)  //构造方法1 不带描述的构造方法
        {
            this.word = word;
            this.id = id;
            this.descri = "";
        }
        public WordCode(String word, int id, String descri)  //构造方法2 加入描述的构造方法
        {
            this.word = word;
            this.id = id;
            this.descri = descri;
        }
        //各个属性的get方法
        public String getWord()
        {
            return word;
        }
        public int getId()
        {
            return id;
        }
        public String getDecri()
        {
            return descri;
        }
    }
    //词法分析类
    class CFFX
    {
        WordCode identify;                      //标识符
        WordCode[] keyword = new WordCode[50];  //关键字
        WordCode intConst;                      //整型常量
        WordCode charConst;                     //字符常量
        WordCode stringConst;                   //字符串常量
        WordCode floatConst;                    //浮点数常量
        WordCode[] singleDelimiters = new WordCode[30];
        WordCode[] dualDelimiters = new WordCode[50];
        //词法分析类构造方法
        public CFFX()
        {
            
            // identify 标识符
             
            identify = new WordCode("IDN", 1, "identify");   //根据 （名称，编号，描述）来初始化，符合WordCode类的构造方法
            
            // const 各类常量
           
            intConst = new WordCode("INT", 2, "int");
            charConst = new WordCode("CHAR", 3, "char");
            floatConst = new WordCode("FLOAT", 4, "float");
            stringConst = new WordCode("STRING", 5, "string");
            
            // keyword 关键字
            // 从文件keyword.txt中读取关键字列表，共32个
            StreamReader r = new StreamReader(@"..\\..\\code\\keyword.txt");
            String temp;
            for (int i = 0; i < 50; i++)
            {
                temp = r.ReadLine();
                if (temp == null || temp.Equals(""))
                {
                    break;
                }
                keyword[i] = new WordCode(temp, 10 + i, temp); //加入关键字数组keyword[]
            }
            r.Close();
           
            // single delimiters 单个符号 如 + - * / = 等
            // 从code文件夹中的singleDelimiters.txt读取单个符号表，共22个
            r = new StreamReader(@"..\\..\\code\\singleDelimiters.txt");
            String[] s;
            for (int i = 0; i < 30; i++)
            {
                temp = r.ReadLine();
                if (temp == null || temp.Equals(""))
                {
                    break;
                }
                //由于在文件存储的符号后面有对应的解释，故只提取\t前的符号
                s = temp.Split(new char[] { '\t' });
                singleDelimiters[i] = new WordCode(s[0], 60 + i, s[1]); //加入单个符号数组singleDelimiters[]
            }
            r.Close();
            
            // dual delimiters 双符号
            //从code文件夹中的dualDelimiters.txt文件中读取，共15个
            r = new StreamReader(@"..\\..\\code\\dualDelimiters.txt");
            for (int i = 0; i < 50; i++)
            {
                temp = r.ReadLine();
                if (temp == null || temp.Equals(""))
                {
                    break;
                }
                //由于在文件存储的符号后面有对应的解释，故只提取\t前的符号
                s = temp.Split(new char[] { '\t' });
                dualDelimiters[i] = new WordCode(s[0], 90 + i, s[1]);
            }
            r.Close();

        }
        //词法分析
        /**
         * code   需要分析的代码内容
         * phrase 分析后的结果
         * errIn  错误
         * */
        public void fenxi(String code, out String phrase, out String errIn)
        {
            phrase = "";
            errIn = "";
            char[] chars = code.ToCharArray();
            int state = 0;
            String nowWord = "";
            int line = 1;

            for (int i = 0; i < chars.Length; )
            {
                if (i > 0 && '\n' == chars[i - 1])
                {
                    line++;
                }
                //开始状态
                //识别第一个字符 决定进入哪种状态
                if (0 == state)
                {
                    if (isLetter(chars[i]) || '_' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 1;
                    }
                    else if ('\'' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 2;
                    }
                    else if ('"' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 21;
                    }
                    else if (isNumber(chars[i]))
                    {
                        nowWord += chars[i];
                        i++;
                        state = 5;
                    }
                    else if ('/' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 11;
                    }
                    else if (isNoUseChar(chars[i]))
                    {
                        i++;
                    }
                    else
                    {
                        if (isStillSingleDelimiters(chars[i]))
                        {
                            nowWord += chars[i];
                            i++;
                            int sKey = searchSingleDelimiters(nowWord);
                            if (-1 != sKey)//可识别单界符
                            {
                                phrase += getOutput(line, nowWord, singleDelimiters[sKey]);
                            }
                            else           //不可识别单界符
                            {
                                errIn += getOutput(line, nowWord, error(state));
                            }
                            state = 0;
                            nowWord = "";

                        }
                        else
                        {
                            nowWord += chars[i];
                            i++;
                            state = 19;
                        }
                    }
                }
                else if (1 == state)      //识别关键字和标识符
                {
                    if (isNumber(chars[i]) || isLetter(chars[i]) || '_' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                    }
                    else                 //单词识别结束
                    {
                        state = 0;
                        int keyI = searchKeyWord(nowWord);//判断是否为关键字
                        if (keyI == -1)
                        {
                            phrase += getOutput(line, nowWord, identify);
                        }
                        else
                        {
                            phrase += getOutput(line, nowWord, keyword[keyI]);
                        }
                        nowWord = "";
                    }
                }
                else if (2 == state)//识别字符常量 刚接收一个单引号
                {
                    if (32 <= chars[i] && 126 >= chars[i] && 39 != chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 3;
                    }
                    else//未找到可识别的字符
                    {
                        //获得整个出错单词
                        while (i < chars.Length && '\n' != chars[i] && '\'' != chars[i])
                        {
                            nowWord += chars[i];
                            i++;
                        }
                        if (i < chars.Length)
                        {
                            if ('\n' == chars[i])//如果直到换行都没找到另一个单引号
                            {
                                i++;
                            }
                            else if ('\'' == chars[i])//如果已找到另一个单引号
                            {
                                nowWord += chars[i];
                                i++;
                            }
                        }
                        if (nowWord.Length <= 3)
                        {
                            errIn += getOutput(line, nowWord, error(state, 0));
                        }
                        else
                        {
                            errIn += getOutput(line, nowWord, error(state, 1));
                        }
                        state = 0;
                        nowWord = "";
                    }
                }
                else if (3 == state)//识别字符常量 已接收一个单引号一个字符
                {
                    if ('\'' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 4;
                    }
                    else//未找到对应单引号
                    {
                        //获取整个错误单词
                        while (i < chars.Length && '\n' != chars[i] && '\'' != chars[i])
                        {
                            nowWord += chars[i];
                            i++;
                        }
                        if (i < chars.Length)
                        {
                            if ('\n' == chars[i])
                            {
                                i++;
                                errIn += getOutput(line, nowWord, error(state));//直到换行也没找到单引号
                            }
                            else if ('\'' == chars[i])
                            {
                                nowWord += chars[i];
                                i++;
                                errIn += getOutput(line, nowWord, error(state, 1));//已找到单引号
                            }
                        }
                        nowWord = "";
                        state = 0;
                    }
                }
                else if (4 == state)
                {
                    phrase += getOutput(line, nowWord, charConst);
                    state = 0;
                    nowWord = "";
                }
                else if (5 == state)//接收数字 整数状态
                {
                    if (isNumber(chars[i]))//继续接收整数
                    {
                        nowWord += chars[i];
                        i++;
                    }
                    else if ('.' == chars[i])//进入浮点数状态
                    {
                        nowWord += chars[i];
                        i++;
                        state = 7;
                    }
                    else if ('e' == chars[i] || 'E' == chars[i])//进入浮点数状态
                    {
                        nowWord += chars[i];
                        i++;
                        state = 8;
                    }
                    else if (isNoUseChar(chars[i]) || isDelimiters(chars[i]))//整数接收完毕
                    {
                        phrase += getOutput(line, nowWord, intConst);
                        nowWord = "";
                        state = 0;
                    }
                    else//错误
                    {
                        while (i < chars.Length && !isDelimiters(chars[i]) && !isNoUseChar(chars[i]))
                        {
                            nowWord += chars[i];
                            i++;
                        }

                        errIn += getOutput(line, nowWord, error(state));
                        nowWord = "";
                        state = 0;
                    }
                }
                else if (7 == state)
                {
                    if (isNumber(chars[i]))//继续接收小数
                    {
                        nowWord += chars[i];
                        i++;
                    }
                    else if ('e' == chars[i] || 'E' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 8;
                    }
                    else if (isNoUseChar(chars[i]) || isDelimiters(chars[i]) && '.' != chars[i])//浮点数接收完毕
                    {
                        phrase += getOutput(line, nowWord, floatConst);
                        nowWord = "";
                        state = 0;
                    }
                    else//错误
                    {
                        while (i < chars.Length && !isDelimiters(chars[i]) && !isNoUseChar(chars[i]))
                        {
                            nowWord += chars[i];
                            i++;
                        }

                        errIn += getOutput(line, nowWord, error(state));
                        nowWord = "";
                        state = 0;
                    }
                }
                else if (8 == state)
                {
                    if (isNumber(chars[i]))
                    {
                        nowWord += chars[i];
                        i++;
                        state = 10;
                    }
                    else if ('+' == chars[i] || '-' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 9;
                    }
                    else
                    {
                        while (i < chars.Length && !isDelimiters(chars[i]) && !isNoUseChar(chars[i]))
                        {
                            nowWord += chars[i];
                            i++;
                        }

                        errIn += getOutput(line, nowWord, error(state));
                        nowWord = "";
                        state = 0;
                    }
                }
                else if (9 == state)
                {
                    if (isNumber(chars[i]))
                    {
                        nowWord += chars[i];
                        i++;
                        state = 10;
                    }
                    else
                    {
                        while (i < chars.Length && !isDelimiters(chars[i]) && !isNoUseChar(chars[i]))
                        {
                            nowWord += chars[i];
                            i++;
                        }

                        errIn += getOutput(line, nowWord, error(state));
                        nowWord = "";
                        state = 0;
                    }
                }
                else if (10 == state)
                {
                    if (isNumber(chars[i]))
                    {
                        nowWord += chars[i];
                        i++;
                    }
                    else if (isNoUseChar(chars[i]) || isDelimiters(chars[i]))//指数部分接收完毕
                    {
                        phrase += getOutput(line, nowWord, floatConst);
                        nowWord = "";
                        state = 0;
                    }
                    else//错误
                    {
                        while (i < chars.Length && !isDelimiters(chars[i]) && !isNoUseChar(chars[i]))
                        {
                            nowWord += chars[i];
                            i++;
                        }

                        errIn += getOutput(line, nowWord, error(state));
                        nowWord = "";
                        state = 0;
                    }
                }
                else if (11 == state)//匹配注释分支
                {
                    if ('/' == chars[i])// //注释
                    {
                        nowWord += chars[i];
                        i++;
                        state = 12;
                    }
                    else if ('*' == chars[i])// /*注释
                    {
                        nowWord += chars[i];
                        i++;
                        state = 15;
                    }
                    else if (isDelimiters(chars[i]))//去匹配双届符
                    {
                        nowWord += chars[i];
                        i++;
                        state = 20;
                    }
                    else if (isNumber(chars[i]) || isLetter(chars[i]) || isNoUseChar(chars[i]))//为除号
                    {
                        int sKey = searchSingleDelimiters("/");
                        phrase += getOutput(line, nowWord, singleDelimiters[sKey]);
                        state = 0;
                        nowWord = "";
                    }
                }
                else if (12 == state)// 已获得//等待回车结束注释
                {
                    if ('\n' == chars[i])// 遇到回车结束注释
                    {
                        state = 13;
                        i++;
                    }
                    else
                    {
                        nowWord += chars[i];
                        i++;
                        state = 12;
                    }
                }
                else if (13 == state)// //注释结束
                {
                    //phrase += "line " + line + "\t\t//注释\r\n";
                    nowWord = "";
                    state = 0;
                }
                else if (15 == state)//等待*/中的*
                {
                    if ('*' == chars[i])// 开始/*注释
                    {
                        nowWord += chars[i];
                        i++;
                        state = 17;
                    }
                    else
                    {
                        nowWord += chars[i];
                        i++;
                    }
                }
                else if (17 == state)//等待*/中的/
                {
                    if ('/' == chars[i])// 结束注释*/
                    {
                        nowWord += chars[i];
                        i++;
                        state = 18;
                    }
                    else if ('*' == chars[i])// **/扔等待注释
                    {
                        nowWord += chars[i];
                        i++;
                        state = 17;
                    }
                    else
                    {
                        nowWord += chars[i];
                        i++;
                        state = 15;
                    }
                }
                else if (18 == state)
                {
                    //phrase += "line " + line + "\t\t结束注释*/\r\n";
                    nowWord = "";
                    state = 0;
                }
                else if (19 == state)//进入单界符状态
                {
                    if (isNumber(chars[i]) || isLetter(chars[i]) || isNoUseChar(chars[i]))//确定为单界符
                    {
                        int sKey = searchSingleDelimiters(nowWord);
                        if (-1 != sKey)//可识别单界符
                        {
                            phrase += getOutput(line, nowWord, singleDelimiters[sKey]);
                        }
                        else//不可识别单界符
                        {
                            errIn += getOutput(line, nowWord, error(state));
                        }
                        state = 0;
                        nowWord = "";
                    }
                    else//进入双界符
                    {
                        nowWord += chars[i];
                        i++;
                        state = 20;
                    }
                }
                else if (20 == state)//双界符
                {
                    int dKey = searchDualDelimiters(nowWord);
                    if (-1 != dKey)//可识别双界符
                    {
                        phrase += getOutput(line, nowWord, dualDelimiters[dKey]);
                    }
                    else//不可识别双界符
                    {
                        int sKey = searchSingleDelimiters(nowWord.Substring(0, 1));
                        if (-1 != sKey)//可识别单界符
                        {
                            phrase += getOutput(line, nowWord.Substring(0, 1), singleDelimiters[sKey]);
                        }
                        else//不可识别单界符
                        {
                            errIn += getOutput(line, nowWord.Substring(0, 1), error(state));
                        }
                        if ('"' == nowWord.ToCharArray()[1])
                        {
                            state = 21;
                            nowWord = "" + '"';
                            continue;
                        }
                        else if ('/' == nowWord.ToCharArray()[1])
                        {
                            state = 11;
                            nowWord = "" + '/';
                            continue;
                        }
                        else if ('\'' == nowWord.ToCharArray()[1])
                        {
                            state = 2;
                            nowWord = "'";
                            continue;
                        }
                        else
                        {
                            state = 19;
                            nowWord = "" + chars[i - 1];
                            continue;
                        }
                    }
                    state = 0;
                    nowWord = "";
                }
                else if (21 == state)
                {
                    if ('"' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 23;
                    }
                    else
                    {
                        nowWord += chars[i];
                        i++;
                        state = 22;
                    }
                }
                else if (22 == state)
                {
                    if ('"' == chars[i])
                    {
                        nowWord += chars[i];
                        i++;
                        state = 23;
                    }
                    else if ('\n' == chars[i])
                    {
                        //nowWord += chars[i];
                        i++;
                        errIn += getOutput(line, nowWord, error(state));
                        state = 0;
                    }
                    else
                    {
                        nowWord += chars[i];
                        i++;
                    }
                }
                else if (23 == state)
                {
                    phrase += getOutput(line, nowWord, stringConst);
                    state = 0;
                    nowWord = "";
                }
                else
                {
                    state = 0;
                    nowWord = "";
                }

            }
            //已结束循环
            //处理缓存内单词
            if (!nowWord.Equals(""))
            {
                if (0 == state)
                {
                }
                else if (1 == state)
                {
                    int keyI = searchKeyWord(nowWord);
                    if (keyI == -1)
                    {
                        phrase += getOutput(line, nowWord, identify);
                    }
                    else
                    {
                        phrase += getOutput(line, nowWord, keyword[keyI]);
                    }
                }
                else if (2 == state)
                {
                    errIn += getOutput(line, nowWord, error(state));
                }
                else if (3 == state)
                {
                    errIn += getOutput(line, nowWord, error(state));
                }
                else if (4 == state)
                {
                    phrase += getOutput(line, nowWord, charConst);
                }
                else if (5 == state)
                {
                    phrase += getOutput(line, nowWord, intConst);
                }
                else if (7 == state)
                {
                    errIn += getOutput(line, nowWord, error(state));
                }
                else if (8 == state)
                {
                    errIn += getOutput(line, nowWord, error(state));
                }
                else if (9 == state)
                {
                    errIn += getOutput(line, nowWord, error(state));
                }
                else if (10 == state)
                {
                    phrase += getOutput(line, nowWord, floatConst);
                }
                else if (11 == state)
                {
                    int sKey = searchSingleDelimiters("/");
                    phrase += getOutput(line, nowWord, singleDelimiters[sKey]);
                }
                else if (12 == state)
                {
                    //phrase += "line " + line + "\t\t//注释\r\n";
                }
                else if (13 == state)
                {
                    //phrase += "line " + line + "\t\t//注释\r\n";
                }
                else if (15 == state)
                {
                    errIn += "line " + line + "\t\t未找到对应*/就结束了\r\n";
                }
                else if (17 == state)
                {
                    errIn += "line " + line + "\t\t未找到对应*/就结束了\r\n";
                }
                else if (18 == state)
                {
                    //phrase += "line " + line + "\t\t结束注释*/\r\n";
                }
                else if (19 == state)
                {
                    int sKey = searchSingleDelimiters(nowWord);
                    if (-1 != sKey)//可识别单界符
                    {
                        phrase += getOutput(line, nowWord, singleDelimiters[sKey]);
                    }
                    else//不可识别单界符
                    {
                        errIn += getOutput(line, nowWord, error(state));
                    }
                }
                else if (20 == state)
                {
                    int dKey = searchDualDelimiters(nowWord);
                    if (-1 != dKey)//可识别双界符
                    {
                        phrase += getOutput(line, nowWord, dualDelimiters[dKey]);
                    }
                    else//不可识别双界符
                    {
                        int sKey = searchSingleDelimiters(nowWord.Substring(0, 1));
                        if (-1 != sKey)//可识别单界符
                        {
                            phrase += getOutput(line, nowWord.Substring(0, 1), singleDelimiters[sKey]);
                        }
                        else//不可识别单界符
                        {
                            errIn += getOutput(line, nowWord.Substring(0, 1), error(state));
                        }
                        sKey = searchSingleDelimiters(nowWord.Substring(1, 1));
                        if (-1 != sKey)//可识别单界符
                        {
                            phrase += getOutput(line, nowWord.Substring(1, 1), singleDelimiters[sKey]);
                        }
                        else//不可识别单界符
                        {
                            errIn += getOutput(line, nowWord.Substring(1, 1), error(state));
                        }
                    }
                }
                else if (21 == state)
                {
                    errIn += getOutput(line, nowWord, error(state));
                }
                else if (22 == state)
                {
                    errIn += getOutput(line, nowWord, error(state));
                }
                else if (23 == state)
                {
                    phrase += getOutput(line, nowWord, stringConst);
                }
                else
                {
                }
            }
        }
        //end fenxi
        //错误处理信息
        String error(int state, int n = 0)
        {
            switch (state)
            {
                case 1:
                    return "错误：请按规则构造标识符";
                case 2:
                    if (0 == n)
                    {
                        return "错误：找不到可识别的字符";
                    }
                    else
                    {
                        return "错误：找不到可识别的字符，且字符常量中的字符太多";
                    }
                case 3:
                    if (0 == n)
                    {
                        return "错误：找不到右引号";
                    }
                    else
                    {
                        return "错误：字符常量中的字符太多";
                    }
                case 5:
                    return "错误：整数拼写错误";
                case 7:
                    return "错误：数字拼写错误";
                case 8:
                    return "错误：数字拼写错误";
                case 9:
                    return "错误：数字拼写错误";
                case 19:
                    return "错误：不可识别的单界符";
                case 20:
                    return "错误：不可识别的双界符";
                case 21:
                    return "错误：缺少对应的双引号";
                case 22:
                    return "错误：缺少对应的双引号";
                default:
                    return "错误：系统不可识别的错误";
            }
            //return "系统不可识别错误";
        }

        Boolean isNumber(char c)
        {
            if (c <= '9' && c >= '0')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        Boolean isLetter(char c)
        {
            if (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        Boolean isNoUseChar(char c)
        {
            if (' ' == c || '\t' == c || '\n' == c || '\r' == c)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        Boolean isDelimiters(char c)
        {
            if (33 <= c && 47 >= c || 58 <= c && 64 >= c || 91 <= c && 96 >= c || 123 <= c && 126 >= c)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //， ；等只能为单界符的字符
        Boolean isStillSingleDelimiters(char c)
        {
            if (',' == c || '.' == c || ';' == c || '[' == c || '{' == c || '}' == c || ']' == c)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        String getOutput(int line, String nowWord, WordCode w)
        {
            return "line " + line + "\t" + nowWord + "\t( " + w.getWord() + ", " + w.getId() + ", " + w.getDecri() + ")\r\n";
        }

        String getOutput(int line, String nowWord, String error)
        {
            return "line " + line + "\t" + nowWord + "\t" + error + "\r\n";
        }
        //查找关键字
        int searchKeyWord(String word)
        {
            int t = 0;
            for (int i = 0; i < 50; i++)
            {
                try
                {
                    t = keyword[i].getWord().CompareTo(word);
                }
                catch (NullReferenceException)
                {
                    return -1;
                }
                if (0 == t)
                {
                    return i;
                }
                else if (0 < t || keyword[i].getWord().Equals(""))
                {
                    return -1;
                }
            }
            return -1;
        }
        //查找单界符
        int searchSingleDelimiters(String word)
        {
            int t = 0;
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    t = singleDelimiters[i].getWord().CompareTo(word);
                }
                catch (NullReferenceException)
                {
                    return -1;
                }
                if (0 == t)
                {
                    return i;
                }
            }
            return -1;
        }
        //查找双界符
        int searchDualDelimiters(String word)
        {
            int t = 0;
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    t = dualDelimiters[i].getWord().CompareTo(word);
                }
                catch (NullReferenceException)
                {
                    return -1;
                }
                if (0 == t)
                {
                    return i;
                }
            }
            return -1;
        }


    }
}
