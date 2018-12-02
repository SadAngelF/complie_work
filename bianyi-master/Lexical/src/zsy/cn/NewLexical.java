package zsy.cn;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.Scanner;

public class NewLexical {

	/**
	 * @param args
	 */
	private Word[] mWordList;//单词列表
	//private ArrayList<CompileError> mLexicalErrorList;//词法错误列表
	private ArrayList<Simbol> mSimbolList;//符号表
	private ArrayList<ConstVariable> mConstVariableList;//常量表
	public NewLexical(String dictFileName){
		try {
			Scanner cin = new Scanner(new File(dictFileName));
			mWordList = new Word[cin.nextInt()];     //单词列表
			for (int i = 0; i < mWordList.length; i++) {
				mWordList[i] = new Word(cin.next(), cin.nextInt(), cin.next(),
						cin.nextBoolean(), cin.next(), cin.next());
				//System.out.println(mWordList[i].getS());
			}
			//mLexicalErrorList = new ArrayList<CompileError>();
			mSimbolList = new ArrayList<Simbol>(); //符号表
			mConstVariableList = new ArrayList<ConstVariable>();
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	public  void Anaylise(String outputFileName) {
		String infile = "代码.txt";
		try {
			FileInputStream f = new FileInputStream(infile);
			BufferedReader dr = new BufferedReader(new InputStreamReader(f));
			FileOutputStream fout = new FileOutputStream(outputFileName);
			int count = 0;
			int linenumber = 0;
			String line = "";
			String token = "";
			boolean flag = true;  //注释换行标识符号,为false就是出现注释换行，否则没有
			while ((line = dr.readLine()) != null) {
				linenumber++;
					char[] strLine = line.toCharArray();
					for (int i = 0; i < strLine.length; i++) {
						char ch = strLine[i];
						//String token = "";
						/**
						 * 检查数值
						 */
						if (CheckDigital(ch) && flag) 
						{
							int state = 1;
							boolean isfloat = false;
							boolean ispower = false;
							boolean error = false;
							boolean errork = false;   //是否有数字开头但是后面却紧接着字母
							String errortoken = "";
							while (ch != '\0' && (CheckDigital(ch) || ch == '.' || ch == 'e' || ch == '-')) {
								i++;
								if(CheckChar(strLine[i])){ //假如数字后面有字母与其直接相连
									error = true;
									i--;
									while(true){
										errortoken = errortoken + strLine[i];
										i++;
										if(strLine[i]==';'){
											i++;
											errork = true;
											break;
										}
									}
									System.out.print("<"+errortoken+">"+"--wrong token!");
									System.out.println(" 数字后面不能有字母直接与其相连！");
								}
								i--;
								if(errork){ //假如出现上述的错误就直接跳出循环
									break;
								}
								if (ch == '.' ){
									isfloat = true;
								}else if(ch == 'e'){
									ispower = true;	
								}
								int k;
								for (k = 1; k <= 6; k++) {
									char NFAstr[] = digitDFA[state].toCharArray();
									if (ch != '#'
											&& DigitNFA(ch, NFAstr[k])) {
										token += ch;
										state = k;
										break;
									}
								}
								if (k > 6)
									break;
								i++;
								if(i>=strLine.length)
									break;
								ch = strLine[i];
							}
							if(state == 2 && isfloat){
								System.out.println("<"+token+"> is wrong!请检查小数点后是否有数字");
								error = true;
							}else if(state == 4 && ispower){
								System.out.println("<"+token+"> is wrong!幂e的后面必须带有‘-’和数字");
								error = true;
							}else if(state == 5 && ispower){
								System.out.println("<"+token+"> is wrong!幂e-的后面必须带有数字");
								error = true;
							}
							if(!error){
								System.out.println("<"+token+">");
								String fileover="";
								fileover = token+"$\t(\t" +81 + "\t,\t-\t)\tINUM\t" + linenumber + "\r\n";
								fout.write(fileover.getBytes());
							}
							--i; //使外层的for循环的i变为原来的值，因为马上还要被+1
							token = "";
						}
						/**
						 * 检查字符常量
						 */
						 else if (ch == '\'' && flag) 
							{
								int state = 0;
								boolean mistake = false;
								String tokenprint = "";
								tokenprint += ch;
								while (state != 3) {
									i++;
									if(i>=strLine.length){
										mistake = true;
										System.out.println("字符常量为非正常关闭引号");
										break;
									}	
									ch = strLine[i];
									if (ch == '\0') { 
										mistake = true;
										System.out.println("字符常量为非正常关闭引号");
										break;
									}
									
									for (int k = 0; k < 4; k++) {
										char tmpstr[] = simconstNFA[state].toCharArray();
										if (CheckConstNFA(ch, tmpstr[k])) {   //假如出现状态的转变，对tokenprint和token进行操作
											tokenprint += ch;                 // 我们要输出的，和token是有区别的
											if (k == 2 && state == 1) {
												if (CheckEscapeCharacter(ch)) // 是转义字符
													token = token + '\\' + ch;
												else
													token += ch;
											} else if (k != 3 && k != 1)
												token += ch;
											state = k;
											break;
										}
									}
								}
								
									if (token.length() == 1 && !mistake) { //
										System.out.println(tokenprint+"<字符常量,"+token+">");
										String fileover="";
										fileover = tokenprint+"$\t(\t" +83 + "\t,\t-\t)\tCH\t" + linenumber + "\r\n";
										fout.write(fileover.getBytes());
									} else if (token.length() == 2 && !mistake) {
										if (CheckEscapeCharacter(token.charAt(1))
												&& token.charAt(0) == '\\') {
											System.out.println(tokenprint+"<转义字符,"+token+">");
											String fileover="";
											fileover = tokenprint+"$\t(\t" +83 + "\t,\t-\t)\tCH\t" + linenumber + "\r\n";
											fout.write(fileover.getBytes());
										}
									} else if(token.length() == 2 && mistake){
										if (CheckEscapeCharacter(token.charAt(1))
												&& token.charAt(0) == '\\') {
											System.out.println(tokenprint+"<转义字符,"+token+">"+"---wrong token");
										}
									} else if(token.length() == 1 && mistake){
										System.out.println(tokenprint+"<字符常量,"+token+">"+"---wrong token");
									}
								
								token = "";
							}
						/**
						 * 检查标识符和关键字
						 */
						 else if(CheckChar(ch) && flag){
							 do {
									token += ch;
									i++;
									if(i>=strLine.length)
										break;
									ch = strLine[i];
								} while (ch != '\0' && (CheckChar(ch) || CheckDigital(ch)));
								--i; 
								if (CheckKeywords(token.toString()))   // 关键字
								{
									System.out.println("<"+token+"--keywordws>");
									
									fout.write(CheckmWordList(token.toString(),linenumber).getBytes());
								} else                                 // 标识符
								{
									count++;
									System.out.println("< id,"+count+">  :" +token);
									String fileover="";
									fileover = token+"$\t(\t" +1 + "\t,\t-\t)\tIDN\t" + linenumber + "\r\n";
									
									fout.write(fileover.getBytes());
								}
								token = "";
						 }
						/**
						 * 检查注释和除号
						 */
						 else if(ch=='/' && flag){
							 token = token + ch;
							 i++;
							 //先检查是否是除号
							 if((strLine[i]!='*' && strLine[i]!='/')){
								 System.out.println("<"+token+">"); //是除号的话直接输出
								 token="";
								 i--;                              //指针归位
							 }
							
							 else 
								{
								 ch = strLine[i];
									if (ch == '*') {        //是/*类型的注释
										token += ch; 
										int state = 2;     //此时已经是处于状态转移图的2号状态

										while (state != 4) {
											i++;           //指针继续向前移动
											if(i==strLine.length-1 && strLine[i]!='/')
												flag = false; //假如终结符不在本行,将flag标识置为false
											if(i>=strLine.length) //长度超过本行最大长度就停止 
												break;
											ch = strLine[i]; 
											
											for (int k = 2; k <= 4; k++) {
												char tmpstr[] = noteNFA[state]
														.toCharArray();
												if (CheckNoteDFA(ch, tmpstr[k],
														state)) { //检查是否可以进行相应的状态转移
													token += ch;
													state = k;
													break;
												}
											}
										}
									}
									else if(ch == '/') //假如是//类型的注释
									{
										int index = line.lastIndexOf("//");
										String tmpstr=line.substring(index);//从//处开始读到行的末尾
										int tmpint = tmpstr.length();//计算指针需要重新偏移多少
										for(int k=0;k<tmpint;k++) 
										{
											i++; //将指针调到正常值以便下次的循环
										}
										token = tmpstr;
									}
									if(flag){
										System.out.println(token+"注釋");
										token = "";
									}
									
								}
							 
						 }
						 /**
						  *  處理各種符號，包括”==“等
						  */
						 else if (CheckOperation(ch) && flag)
							{
								token += ch;
								if(i<strLine.length-1)
								i++;
								if(CheckOperation(strLine[i]) && strLine[i-1]!='('){
								String doubleoperation = token + strLine[i];
								if( doubleoperation.equals(":=") || doubleoperation.equals("||") || doubleoperation.equals("&&") || doubleoperation.equals("<=") || doubleoperation.equals(">=") || doubleoperation.equals("!=") || doubleoperation.equals("==") || doubleoperation.equals("/=") || doubleoperation.equals("+=") || doubleoperation.equals("-=")
										|| doubleoperation.equals("*=") || doubleoperation.equals("++") || doubleoperation.equals("--") || doubleoperation.equals("**")){
									token = doubleoperation;
								}
								}else{
									i--;
								}
								System.out.println("<"+token+">");
								fout.write(CheckmWordList(token.toString(),linenumber).getBytes());
								token = "";
							}
						/**
						 * 处理中间有换行存在的/*类型注释，如果该注释在一行之内
						 * 没有结束，就说明下一行的文字应全部是注释内容，知道找到与
						 * 其匹配的右部为止，flag的值都为flase，找到之后再将其置为true
						 */
						 else if(!flag){ 
							 int index = line.lastIndexOf("*/");
							 if(index==-1){
								 String tmpstr = line.substring(0);
								 token = token + tmpstr;
							 }else{
								 String tmpstr = line.substring(0,index);
								 token = token + tmpstr+"*/";
								 System.out.println(token+"注释");
								 flag = true;
								 token = "";
								 i = line.lastIndexOf("/");
							 }
						 }
						/**
						 * 處理字符串常量
						 * */
						 else if (ch == '"')
							{
								String token1 = "";
								token1 += ch;
								int state = 0;
								boolean mistake = false;
								while (state != 3 ) {
									i++;
									if(i>=strLine.length-1) 
									{
										mistake = true;
										System.out.println("字符串常量为正常关闭双引号");
										break;
									}
									ch = strLine[i];
									if (ch == '\0') {
										mistake = true;
										System.out.println("字符串常量为正常关闭双引号");
										break;
									}
									for (int k = 0; k < 4; k++) {
										char tmpstr[] = constNFA[state].toCharArray();
										if (CheckStringDFA(ch, tmpstr[k])) {
											token1 += ch;
											if (k == 2 && state == 1) {  //public static String constNFA[] = { "#\\d#", "##a#", "#\\d\"", "####" };
												if (CheckEscapeCharacter(ch)) // 是转义字符
													token = token + '\\' + ch;
												else
													token += ch;
											} else if (k != 3 && k != 1)
												token += ch;
											state = k;
											break;
										}
									}
								}
								if(mistake){
									System.out.println("<"+token+"> :"+token1+"--wrong token!" );
									token = "";
								}else{
									System.out.println("<"+token+"> :"+token1 );
									String fileover="";
									fileover = token+"$\t(\t" +1 + "\t,\t-\t)\tSTR\t" + linenumber + "\r\n";
									fout.write(fileover.getBytes());
									token = "";
								}
							}
				
			}
					}
			fout.close();
	}catch (IOException e) {
		e.printStackTrace();
	}
		
	}
	
	/**
	 * 检查字符是否满足注释的状态转移条件
	 * @param ch
	 * @param key
	 * @param s
	 * @return
	 */
	public static boolean CheckNoteDFA(char ch, char key, int s) {
		if (s == 2) {
			if (key == 'c') {
				if (ch != '*')
					return true;
				else
					return false;
			}
		}
		if (s == 3) {
			if (key == 'c') {
				if (ch != '*' && ch != '/')
					return true;
				else
					return false;
			}
		}
		return ch == key;
	}
	/**
	 * 注释的状态转移表
	 */
	public static String noteNFA[] = { "#####", "##*##", "##c*#", "##c*/", "#####" };
	/**
	 * 数值状态转移表
	 */
	public static String digitDFA[] = { "#######", "#d.#e##", "###d###", "###de##",
		"#####-d", "######d", "######d" };
	/**
	 * 检查是否是数字
	 * @param ch
	 * @return
	 */
	public static Boolean CheckDigital(char ch) {
		return (ch >= '0' && ch <= '9');
	}

	/**
	 * 符号举例
	 */
	public static char operation[] = { ':','+', '-', '*', '=', '<', '>', '&', '|', '~',
		'^', '!', '(', ')', '[', ']', '{', '}', '%', ';', ',', '#', '.' };
	/**
	 * 判断是否为常用的符号
	 * @param ch
	 * @return
	 */
	public static Boolean CheckOperation(char ch) 
	{
		for (int i = 0; i < 22; i++)
			if (ch == operation[i]) {
				return true;
			}
		return false;
	}
	/**
	 * 检查是否满足数值的状态转移条件
	 * @param ch
	 * @param key
	 * @return
	 */
	public static boolean DigitNFA(char ch, char key) {
		if (key == 'd') {
			if (CheckDigital(ch))
				return true;
			else
				return false;
		}
		return ch == key;
	}
	/**
	 * 检查是否为转义字符
	 * @param ch
	 * @return
	 */
	public static boolean CheckEscapeCharacter(char ch) {
		return ch == 'a' || ch == 'b' || ch == 'f' || ch == 'n' || ch == 'r'
				|| ch == 't' || ch == 'v' || ch == '?' || ch == '0';
	}
	/**
	 * 检查是否满足字符常量的状态转移要求
	 * @param ch
	 * @param key
	 * @return
	 */
	public static boolean CheckConstNFA(char ch, char key) {
		if (key == 'a') //
			return true;
		else if (key == '\\' || key == '\'')
			return ch == key;
		else if (key == 'd')
			return ch != '\\' && ch != '\'';
		return false;
	}
	/**
	 * 字符串常量的状态转移表
	 */
	public static String constNFA[] = { "#\\d#", "##a#", "#\\d\"", "####" };
	/**
	 * 字符字符常量的状态转移表
	 */
	public static String simconstNFA[] = {"#\\d#","##a#","###\'","####"};
	/**
	 * 关键字举例
	 */
	public static String keywords[] = { "auto", "double", "int", "struct",
		"break", "else", "long", "switch", "case", "enum", "register",
		"typedef", "char", "extern", "return", "union", "float",
		"short", "unsigned", "continue", "for", "signed", "void",
		"default", "goto", "sizeof", "volatile", "do", "if", "while",
		"static" ,"String"};
	/**
	 * 
	 * @param ch
	 * @return boolean
	 * 检查是否为构成标识符或者关键字的字符
	 */
	public static boolean CheckChar(char ch){
		if((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == '_'){
			return true;
		}else{
			return false;
		}
	}
	/**
	 * 
	 * @param str
	 * @return boolean
	 * 检查是否为关键字
 	 */
	public static boolean CheckKeywords(String str){
		for(int i=0;i<keywords.length;i++){
			if(str.equals(keywords[i]))
				return true;
		}
		return false;
	}
	/**
	 * 
	 * @param ch
	 * @param key
	 * @return boolean
	 * 检查字符串是否满足相应的状态以便进行状态转移
	 */
	public static boolean CheckStringDFA(char ch, char key) {
		if (key == 'a')
			return true;
		else if (key == '\\' || key == '"')
			return ch == key;
		else if (key == 'd')
			return ch != '\\' && ch != '"';
		return false;
	}
	
	public String CheckmWordList(String word,int linenumber){
		String fileover = "";
		for(int i=0;i<mWordList.length;i++){
			if(word.equals(mWordList[i].getToken())){
				fileover = word+"$\t(\t" +mWordList[i].getType() + "\t,\t-\t)\t"+mWordList[i].getToken()+"\t" + linenumber + "\r\n";
				System.out.println(fileover);
				break;
		}
	}
		return fileover;
}
}
				
