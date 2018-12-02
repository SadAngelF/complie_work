package zsy.cn;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Scanner;
import java.util.Stack;

/**
 * 文法操作类
 */
class Grammer {
	private int mTerminalsNums; // 终结符的个数
	private int mNonTerminalsNums; // 非终结符的个数
	private int mVocabularyNums; // 单词表的个数 = 终结符个数 + 非终结符个数
	private int mStartSymbol; // 起始符号
	private int mLambda; // 空的编号
	private ArrayList<ArrayList<Prod>> mProductions; // 产生式列表
	private HashSet<Integer>[] mFirstSet; // First集
	private HashSet<Integer>[] mFollowSet; // Follow集
	private int[][] mPredictMap; // 预测分析表
	private ArrayList<CompileError> mParserErrorList; // 语法分析错误
	private HashMap<String, Integer> mTerminalsMap; // 终结符名称和编号对照表
	private HashMap<String, Integer> mNonTerminalsMap; // 非终结符名称和编号对照表
	private String[] mTerminals; // 编号从小到大终结符名称
	private ArrayList<String> mNonTerminals; // 编号从小到大非终结符名称
	private GrammerAnalysisTree mGrammerTree; // 语法树
	private static final int PREDICT_NULL = -1; // 预测分析表空值的定义
	private static final int PREDICT_SYNCH = -2; // 预测分析表同步记号

	public Grammer(String terminalsDictFileName,
			String originalGrammerFileName, String newGrammerFileName,
			String predictMapFileName) {
		// 加载原始文法
		initFromFile(terminalsDictFileName, originalGrammerFileName);
		// 提取直接公共前缀
		factor();
		// 删除直接左递归
		removeLeftRecursion();
		// 消除间接公共前缀，生成q_文法
		removeIndirectFactor();
		// 向文件里输出处理过的文发
		printProductionList(newGrammerFileName, false, true);
		// 计算first集
		fillFirstSet();
		// 计算所有非终结符的follow集
		fillFollowSet();
		// 建立预测分析表
		createPredictMap();
		// 向文件里输出预测分析表
		printPredictMap(predictMapFileName, true, true, true, true);

		mParserErrorList = new ArrayList<CompileError>();

	}

	/**
	 * 删除等价的文法及非终结符
	 */
	private void removeSameProds() {
		boolean changes;
		do {
			changes = false;
			for (int i = mProductions.size() - 1; i >= 0; i--) {  //这里处理的是一个产生式数组中的相同元素
				ArrayList<Prod> prodsA = mProductions.get(i);
				for (int j = prodsA.size() - 1; j >= 0; j--) {
					Prod pA = prodsA.get(j);
					for (int k = j - 1; k >= 0; k--) {
						if (pA.equals(prodsA.get(k))) { //如果有相同的两个产生式就删除一个
							prodsA.remove(j);
							break;
						}
					}
				}
			}
			for (int i = 0; i < mProductions.size(); i++) {  //这里处理的是可能出现的两个完全相同的产生式数组
				ArrayList<Prod> prodsA = mProductions.get(i);
				for (int j = i + 1; j < mProductions.size(); j++) {
					ArrayList<Prod> prodsB = mProductions.get(j);
					if (prodsA.size() == prodsB.size()) {
						boolean match = true;
						for (int k = prodsA.size() - 1; k >= 0 && match; k--) {
							boolean matchOne = false;
							for (int m = prodsB.size() - 1; m >= 0 && !matchOne; m--) {
								if (prodsA.get(k).equals(prodsB.get(m))) {  //检查两个大小相等的产生式是否每一个元素大小也相同
									matchOne = true;
								}
							}
							if (!matchOne) {
								match = false;
							}
						}
						if (match) {   //如果真的存在这样两个产生式数组
							changes = true;
							mProductions.remove(j);//删除其中之一
							mNonTerminalsNums--;   //总非终结符数量减一，因为只有非终结符才能产生产生式
							for (int k = mProductions.size() - 1; k >= 0; k--) {
								ArrayList<Prod> prodsC = mProductions.get(k);
								for (int m = prodsC.size() - 1; m >= 0; m--) {
									Prod pC = prodsC.get(m);
									if (pC.mLhs > j + mTerminalsNums) { //原来被删除产生式后面的产生式序号要相应第向前移动一位，即左部减一
										pC.mLhs--;
									} else if (pC.mLhs == j + mTerminalsNums) {
										pC.mLhs = i + mTerminalsNums;
									}
									for (int t = pC.mRhs.size() - 1; t >= 0; t--) {
										int pcr = pC.mRhs.get(t);    //对产生式右部元素的操作与之前类似
										if (pcr == j + mTerminalsNums) {
											pcr = i + mTerminalsNums;
											pC.mRhs.set(t, pcr);
										} else if (pcr > j + mTerminalsNums) {
											pcr--;
											pC.mRhs.set(t, pcr);
										}
									}
								}
							}
							j--;
						}

					}
				}
			}
		} while (changes);
	}

	/**
	 * 向文件里输出所有的产生式
	 * 
	 * @param outputFileName
	 *            文件名
	 * @param printNumber
	 *            是否输出数字形式
	 * @param printName
	 *            是否输出名称形式
	 */
	private void printProductionList(String outputFileName,
			boolean printNumber, boolean printName) {
		try {
			FileOutputStream fout = new FileOutputStream(outputFileName);

			int tot = 0;
			for (int i = mProductions.size() - 1; i >= 0; i--) {
				tot += mProductions.get(i).size();
			}
			fout.write((tot + "\r\n").getBytes());
			for (int i = 0, len = mProductions.size(); i < len; i++) {
				ArrayList<Prod> prods = mProductions.get(i);
				for (int j = 0; j < prods.size(); j++) {
					Prod p = prods.get(j);
					printProduction(fout, p, printNumber, printName);
				}
			}
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	/**
	 * 向文件流fout里输入产生式p
	 * 
	 * @param fout
	 *            要输出到的文件流
	 * @param p
	 *            产生式p
	 * @param printNumber
	 *            是否按数字形式打印
	 * @param printName
	 *            是否按名称形式打印
	 * @throws IOException
	 */
	private void printProduction(FileOutputStream fout, Prod p,
			boolean printNumber, boolean printName) throws IOException {
		if (printNumber) {
			fout.write((p.mLhs + "\t->\t").getBytes());      //向文件中输入产生式的左部
			for (int j = 0; j < p.mRhs.size(); j++) {        //向文件中输入对应产生式的右部
				fout.write((" " + p.mRhs.get(j)).getBytes());
			}
			fout.write("\r\n".getBytes());
		}
		if (printName) {
			fout.write((getWordName(p.mLhs) + "\t->\t").getBytes());   //向文件中输入产生式的左部
			for (int j = 0; j < p.mRhs.size(); j++) {
				fout.write((getWordName(p.mRhs.get(j))).getBytes());   //向文件中输入对应产生式的右部
			}
			fout.write("\r\n".getBytes());
		}
	}


	/**
	 * 获取终结符或者非终结符i的名称
	 * 
	 * @param i
	 * @return
	 */
	private String getWordName(int i) {
		if (i >= 0 && i < mTerminalsNums) {
			return "[" + mTerminals[i] + "]";
		} else if (i >= mTerminalsNums
				&& i < mTerminalsNums + mNonTerminals.size()) {
			return "<" + mNonTerminals.get(i - mTerminalsNums) + ">";
		}
		return "<" + i + ">";
	}

	/**
	 * 从文件里加载语法表
	 * 
	 * @param terminalsDictFileName
	 * @param grammerFileName
	 */
	private void initFromFile(String terminalsDictFileName,
			String grammerFileName) {
		try {
			Scanner terCin = new Scanner(new File(terminalsDictFileName));
			mTerminalsNums = terCin.nextInt(); //词法分析一共有多少中，存储在文件的第一行中
			mTerminalsMap = new HashMap<String, Integer>();// 终结符名称和编号对照表
			mTerminals = new String[mTerminalsNums];// 编号从小到大终结符名称
			for (int i = mTerminalsNums; i > 0; i--) {//生成终结符名称和编号对照表
				terCin.next();
				int num = terCin.nextInt();
				String name = terCin.next();
				terCin.nextLine();
				mTerminalsMap.put(name, num);
				mTerminals[num] = name;
			}
			mLambda = 0;
			mStartSymbol = mTerminalsNums;

			mNonTerminalsMap = new HashMap<String, Integer>();// 非终结符名称和编号对照表
			mNonTerminals = new ArrayList<String>();// 终结符名称和编号对照表
			mNonTerminalsNums = 0;
			Scanner cin = new Scanner(new File(grammerFileName));
			int prodN = cin.nextInt();    //表达式一共有多少种，事先写在文件的第一行中了
			cin.nextLine();
			mProductions = new ArrayList<ArrayList<Prod>>();//产生式列表
			for (int i = 0; i < prodN; i++) {
				Prod p = new Prod();
				p.mRhs = new ArrayList<Integer>();

				String sProduction = cin.nextLine(); //每行就是一种产生式
				String sProductionTail = removeNSPHeader(sProduction);//[]内的是终结符，<>内的是非中介字符，
                //我们只要<>或者[]中的内容，所以要将括号去掉
				String sLeftWord = getSingleWord(sProductionTail);//得到去掉'['或'<'的非终结符（终结符）
				if (sLeftWord == null || sLeftWord.charAt(0) != '<') {//出现异常后报错
					System.out.println("加载第" + i + "条文法错误：" + sProduction);
					System.out.println("错误原因：左边不对！" + sProductionTail + "$"
							+ sLeftWord);
					continue;//直接开始下一次循环
				}
				String leftWord = sLeftWord.substring(1);//去除前面的'<'或者']'，得到最左的非终结符(终结符)
				//System.out.println(leftWord);
				Integer leftIndex = mNonTerminalsMap.get(leftWord);//从非终结表中得到非中介字符的编号
				ArrayList<Prod> currentProdList;
				if (leftIndex == null) {//假如不存在的话
					leftIndex = mTerminalsNums + mNonTerminalsNums++; //新增一个编号
					mNonTerminals.add(leftWord);              //分别加入非终结符表和对照表中
					mNonTerminalsMap.put(leftWord, leftIndex);
					currentProdList = new ArrayList<Prod>();
					mProductions.add(currentProdList);//加入产生式的数组中
				} else {//如果存在的话
					currentProdList = mProductions.get(leftIndex
							- mTerminalsNums);       //从产生式列表中得到产生式
				}
				p.mLhs = leftIndex;

				sProductionTail = removeNSPHeader(sProductionTail
						.substring(sLeftWord.length()));      //得到产生式的后半段，即A->B|C|D的B，C，D部分
				String sRightWord, rightWord;
				Integer rightIndex;
				while ((sRightWord = getSingleWord(sProductionTail)) != null) {
					rightWord = sRightWord.substring(1);
					if (sRightWord.charAt(0) == '<') {       //以'<'开头说明是非终结符
						rightIndex = mNonTerminalsMap.get(rightWord);//从非中介map中找到对应的编号
						if (rightIndex == null) {//没找到
							rightIndex = mTerminalsNums + mNonTerminalsNums++;
							mNonTerminals.add(rightWord);
							mNonTerminalsMap.put(rightWord, rightIndex);
							mProductions.add(new ArrayList<Prod>());
						}
					} else if (sRightWord.charAt(0) == '[') {//假如是终结符的话
						rightIndex = mTerminalsMap.get(rightWord);
					} else {
						rightIndex = null;
					}
					if (rightIndex == null) {
						break;
					}
					p.mRhs.add(rightIndex);
					sProductionTail = removeNSPHeader(sProductionTail
							.substring(sRightWord.length()));
				}
				if (sRightWord != null) {
					System.out.println("加载第" + i + "条文法错误：" + sProduction);
					continue;
				}
				p.mComment = sProductionTail.replace("\t", "");
				currentProdList.add(p);
				//System.out.println(currentProdList.toString());
				//System.out.println(mProductions.toString());
			}
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		}
	}

	/**
	 * 去除字符串s里'<'或'['前的字符
	 * 
	 * @param s
	 * @return
	 */
	private static String removeNSPHeader(String s) {
		if (s == null)
			return null;
		int i = s.indexOf('<');
		int j = s.indexOf('[');
		if (i < j) {
			if (i > -1) {
				return s.substring(i);
			}
			return s.substring(j);
		} else if (i > j) {
			if (j > -1) {
				return s.substring(j);
			}
			return s.substring(i);
		}
		return s;
	}

	/**
	 * 从字符串s中获取第一个终结符或者非终结符
	 * 
	 * @param s
	 * @return
	 */
	private static String getSingleWord(String s) {
		if (s == null)
			return null;
		int i;
		if (s.charAt(0) == '<') {
			i = s.indexOf('>', 2);// 从字符串2号位置开始搜索'>'符号，并返回其位置
		} else if (s.charAt(0) == '[') {
			i = s.indexOf(']', 2);
		} else {
			return null;
		}
		if (i < 0)
			return null;
	//System.out.println(s.substring(0, i));
		return s.substring(0, i);
	}

	/**
	 * 计算First(apha)
	 * 
	 * @param hashSet
	 *            计算出的集合添加到hashSet里
	 * @param alpha
	 *            要计算的表达式 //即产生式的右部，这是一个整型数组
	 */
	private void computeFirst(HashSet<Integer> hashSet, ArrayList<Integer> alpha) {
		if (alpha.size() == 0) {
			hashSet.add(mLambda);  //mLambda大小在前面被赋值为0
		} else {
			boolean oriContainLambda = hashSet.contains(mLambda); //如果该hashset中包含空，则返回true
			hashSet.addAll(mFirstSet[alpha.get(0)]);   //得到右部的第一个元素
			int i = 0;
			for (i = 1; i < alpha.size()
					&& mFirstSet[alpha.get(i - 1)].contains(mLambda); i++) {
				hashSet.addAll(mFirstSet[alpha.get(i)]); //假如右部的前i个元素都为空，那么就将第i个元素加入
			}
			if (!oriContainLambda) {    //假如没有空元素
				if (i < alpha.size()
						|| !mFirstSet[alpha.get(i - 1)].contains(mLambda)) {
					hashSet.remove(mLambda); //且所有元素都非空，就将空从hashset中移除
				}
			}
		}
	}

	/**
	 * 计算First集
	 */
	private void fillFirstSet() {
		mVocabularyNums = mNonTerminalsNums + mTerminalsNums;
		mFirstSet = new HashSet[mVocabularyNums]; //大小为终结符加非终结符之和
		for (int i = mFirstSet.length - 1; i >= 0; i--) {//
			mFirstSet[i] = new HashSet<Integer>();
		}
		for (int i = mTerminalsNums - 1; i >= 0; i--) { //0-84存放终结符，与词法分析字典对应
			mFirstSet[i].add(i);                        //终结符号的first集合就是自身
			//System.out.println(mFirstSet[i].toString());
		}
		boolean changes;
		do {
			changes = false;
			for (int k = mProductions.size() - 1; k >= 0; k--) {
				ArrayList<Prod> currentProds = mProductions.get(k); //得到一个产生式数组
				for (int i = currentProds.size() - 1; i >= 0; i--) {
					Prod p = currentProds.get(i);                //从其中取出一个产生式
					int oriNum = mFirstSet[p.mLhs].size();       //查看该first集中是否有元素，开始时大小为0
					//System.out.println(oriNum);
					computeFirst(mFirstSet[p.mLhs], p.mRhs);
					int newNum = mFirstSet[p.mLhs].size();       //假如新的first集合不是空了
					changes = changes || oriNum != newNum;       //change的值久违true，最后可以跳出do。。while循环
				}
			}
		} while (changes);
	}

	/**
	 * 计算所有非终结符的Follow集
	 */
	private void fillFollowSet() {
		mFollowSet = new HashSet[mNonTerminalsNums];           //follow集大小肯定为非终结符号的结合
		for (int i = mFollowSet.length - 1; i >= 0; i--) {
			mFollowSet[i] = new HashSet<Integer>();            //每个非终结符号对应一个follow集合
		}
		//System.out.println(mStartSymbol - mTerminalsNums);
		mFollowSet[mStartSymbol - mTerminalsNums].add(mLambda);//mStartSymbol - mTerminalsNums大小其实就是0，mStartSymbol之前被赋值为mTerminalsNums
		boolean changes;
		int terminalsLength = mTerminalsNums;
		HashSet<Integer> hashSet = new HashSet<Integer>();

		do {
			changes = false;
			for (int k = mProductions.size() - 1; k >= 0; k--) {
				ArrayList<Prod> currentProds = mProductions.get(k);
				for (int i = currentProds.size() - 1; i >= 0; i--) {
					Prod p = currentProds.get(i);
					int len = p.mRhs.size();//len是p右部的大小
					int A = p.mLhs - terminalsLength; // A的大小为左部的数值-终结符的多少
					ArrayList<Integer> alpha = (ArrayList<Integer>) p.mRhs
							.clone();    //alpha即是p的右部集合
					for (int j = 0; j < len; j++) {
						int B = alpha.get(0);//得到右部的第一个元素
						alpha.remove(0);    //将B清除出当前的右部序列是为了求剩下元素的first集合，相当于求B的follow集合
						if (B >= terminalsLength) {   //假如B不是终结符,因为终结符都在0-terminalsLength之间
							B -= terminalsLength;     // B的大小为右部第一个元素的数值-终结符的多少，这是为了存入新的follow集合中，follow集合的大小只有非终结字符数量的大小
							int oriN = mFollowSet[B].size(); //刚开始时B的follow集合大小为0
							hashSet.clear();
							computeFirst(hashSet, alpha);    //求剩下元素的first集合
							if (hashSet.contains(mLambda)) { //，假如剩下的first集合为空，龙书中对于follow集合的第三种情况,假如B后面已经没有元素了，那么就将B的follow集合加入A的follow集合中
								mFollowSet[B].addAll(mFollowSet[A]);
								hashSet.remove(mLambda);    //去掉空
							}
							mFollowSet[B].addAll(hashSet);
							int nowN = mFollowSet[B].size();
							changes = changes || oriN < nowN;
							/**
							 * 打印一下下 if (B == 10) { printProduction(p, true,
							 * true); for (int ti = 0; ti < mTerminalsNums;
							 * ti++) { if (mFollowSet[B].contains(ti)) {
							 * System.out.print(getWordName(ti)); } }
							 * System.out.println(); }
							 */
						}
					}
				}
			}
		} while (changes);
	}

	/**
	 * 提取公共前缀
	 */
	private void factor() {
		boolean found;
		ArrayList<Prod> sameHeaderList = new ArrayList<Prod>();
		do {
			found = false;
			for (int g = mProductions.size() - 1; g >= 0; g--) {
				ArrayList<Prod> currentProds = mProductions.get(g);
				for (int i = currentProds.size() - 1; i > 0; i--) {
					Prod pA = currentProds.get(i);
					int sameLen = pA.mRhs.size();  //产生式右部有多少个终结符/非终结符
					if (pA.mRhs.get(0) == mLambda) { //如果产生式右部为空，则直接进行下一次循环
						continue;
					}
					sameHeaderList.clear();       //每次都要清空
					sameHeaderList.add(pA);       //先加入pA
					for (int j = i - 1; j >= 0; j--) {
						Prod pB = currentProds.get(j);
						int pBLen = pB.mRhs.size();//产生式右部有多少个终结符/非终结符
						int k;
						for (k = 0; k < sameLen && k < pBLen
								&& pA.mRhs.get(k).equals(pB.mRhs.get(k)); k++) //寻找最长前缀
							;
						if (k > 0) {
							sameLen = k;           //最长公共前缀的长度为k
							sameHeaderList.add(pB);//将pB加入,这说明pB和pA有公共前缀
						}
					}
					if (sameHeaderList.size() > 1) { //如果已经有一些prod公共前缀相同
						/**
						 * 查找有没有直接可用的
						 */
						boolean foundUsable = false;
						for (int ti = mProductions.size() - 1; ti >= 0
								&& !foundUsable; ti--) {
							ArrayList<Prod> mayBeList = mProductions.get(ti);
							if (mayBeList.size() == sameHeaderList.size()) { //如果直接可用，第一点：产生式集合的大小必须相同
								boolean match = true;
								for (int tj = mayBeList.size() - 1; tj >= 0
										&& match; tj--) {
									Prod mayBeProd = mayBeList.get(tj);
									boolean matchOne = false;
									for (int tk = sameHeaderList.size() - 1; tk >= 0
											&& !matchOne; tk--) {
										Prod curProd = sameHeaderList.get(tk);
										if (mayBeProd.mRhs.size() == curProd.mRhs
												.size() - sameLen) {  //如果直接可用，第二点：产生式右部的大小必须和现有的产生式右部去掉公共前缀后的大小相等
											boolean matchEach = true;
											for (int tp = mayBeProd.mRhs.size() - 1; tp >= 0
													&& matchEach; tp--) {
												if (!mayBeProd.mRhs
														.get(tp)
														.equals(curProd.mRhs
																.get(tp
																		+ sameLen))) {
													matchEach = false;
												}
											}
											if (matchEach) { //如果直接可用,第三点：产生式右部的所有元素必须相等
												matchOne = true;
											}
										}
									}
									if (!matchOne) {
										match = false;
									}
								}
								if (match) {
									foundUsable = true;
									for (int tp = sameHeaderList.size() - 1; tp >= 0; tp--) {
										currentProds.remove(sameHeaderList
												.get(tp));
									}
									Prod pC = new Prod(); //新建一个表达式pC
									pC.mLhs = pA.mLhs;    //使其与pA的左部相同
									pC.mRhs = getSubList(pA.mRhs, 0, sameLen); //将pA的右部的公共部分提取出来加入pC的右部
									pC.mRhs.add(ti + mTerminalsNums);  //直接将右部剩下的部分加入找到的元素
									currentProds.add(pC);
									//System.out.println(currentProds.toString());
								}
							}
						}
						if (foundUsable) {
							found = true;
							break;
						}
						/**
						 * 没有则添加
						 */
						Prod pC = new Prod();
						pC.mLhs = pA.mLhs;
						pC.mRhs = getSubList(pA.mRhs, 0, sameLen);
						ArrayList<Prod> newProd = new ArrayList<Prod>();
						for (int j = sameHeaderList.size() - 1; j >= 0; j--) {
							Prod pN = sameHeaderList.get(j);
							currentProds.remove(pN);
							pN.mLhs = mTerminalsNums + mNonTerminalsNums;
							pN.mRhs = getSubList(pN.mRhs, sameLen,
									pN.mRhs.size());
							if (pN.mRhs.size() == 0) {
								pN.mRhs.add(mLambda);
							}
							newProd.add(pN);
						}
						pC.mRhs.add(mTerminalsNums + mNonTerminalsNums++);
						currentProds.add(pC);
						mProductions.add(newProd);
						found = true;
						break;
					}
				}
			}
		} while (found);
	}

	/**
	 * 获取a中[startIndex, endIndex)里的元素
	 * 
	 * @param a
	 * @param startIndex
	 * @param endIndex
	 * @return
	 */
	private <T> ArrayList<T> getSubList(ArrayList<T> a, int startIndex,
			int endIndex) {
		ArrayList<T> newArrayList = new ArrayList<T>();
		int len = a.size();
		for (int i = startIndex; i < len && i < endIndex; i++) {
			newArrayList.add(a.get(i));
		}
		return newArrayList;
	}

	/**
	 * 删除左递归
	 */
	private void removeLeftRecursion() {
		boolean found;
		do {
			found = false;
			for (int k = mProductions.size() - 1; k >= 0; k--) {
				ArrayList<Prod> currentProds = mProductions.get(k);
				for (int i = currentProds.size() - 1; i >= 0; i--) {
					Prod p = currentProds.get(i);
					if (p.mRhs.get(0) == p.mLhs) { //假如文法右部的第一个非终结符和左部的非终结服相同，则出现左递归
						currentProds.remove(i);   //在当前的表达式集合中删除该项
						int T = mTerminalsNums + mNonTerminalsNums++;
						for (int j = currentProds.size() - 1; j >= 0; j--) {
							Prod pB = currentProds.get(j);
							if (pB.mRhs.get(0) == mLambda) {
								pB.mRhs.remove(0);
							}
							pB.mRhs.add(T);
						}
						ArrayList<Prod> prodT = new ArrayList<Prod>();
						mProductions.add(prodT);
						Prod pB = new Prod();
						pB.mLhs = T;        //将pB的左部赋值为T
						p.mRhs.remove(0);   //删除出现左递归的产生式p的右部第一项即出现左递归的哪项
						pB.mRhs = p.mRhs;   //将p剩下的无左递归的部分赋值给pB
						pB.mRhs.add(T);     //将pB的左部T赋值给pB右部的最右边
						if (pB.mRhs.get(0) == mLambda) {
							pB.mRhs.remove(0);
						}
						prodT.add(pB);      
						Prod pC = new Prod();
						pC.mLhs = T;       //将空将加入到与pB相同左部的产生式右部中
						pC.mRhs = new ArrayList<Integer>();
						pC.mRhs.add(mLambda);
						prodT.add(pC);     //左递归消除完毕
						//System.out.println(prodT.toString());
						found = true;
						break;
					}
				}
			}
		} while (found);
	}

	/**
	 * 消除间接公共前缀
	 * 思路是化间接为直接，然后再利用已有函数消除直接公共前缀
	 */
	private void removeIndirectFactor() {
		boolean changes;
		do {
			changes = false;
			for (int i = mProductions.size() - 1; i >= 0; i--) {
				ArrayList<Prod> currentProds = mProductions.get(i);
				for (int j = currentProds.size() - 1; j >= 0; j--) {
					Prod pA = currentProds.get(j); //得到一个产生式
					int b = pA.mRhs.get(0);        //得到产生式右部的第一个符号
					if (b >= mTerminalsNums) {     //如果是非终结符号的话
						changes = true;
						ArrayList<Prod> prodsB = mProductions.get(b
								- mTerminalsNums); //从产生式列表中得到这个产生式,该符号对应的产生式集合
						boolean allStartWithTerminals = true;
						for (int k = prodsB.size() - 1; k >= 0
								&& allStartWithTerminals; k--) {
							allStartWithTerminals = prodsB.get(k).mRhs.get(0) < mTerminalsNums; //找到其中某个产生式第一个字符为终结符号
						}
						if (!allStartWithTerminals)  //如果allStartWithTerminals为真，说明所有前缀都是终结符
							continue;
						boolean hasLambda = false;
						for (int k = prodsB.size() - 1; k >= 0; k--) {
							Prod pB = prodsB.get(k); 
							if (pB.mRhs.get(0) == mLambda) { //假如该产生式首为空
								hasLambda = true;
							} else {                         //不为空
								Prod newProd = new Prod();   //
								newProd.mLhs = pA.mLhs;  
								newProd.mRhs = getSubList(pA.mRhs, 1,
										pA.mRhs.size());     //相当于去掉了公共前缀
								newProd.mRhs.addAll(0, pB.mRhs); //在0位置开始加入pB的右部
								currentProds.add(newProd);
							}
						}
						if (hasLambda) {
							pA.mRhs.remove(0);
							if (pA.mRhs.size() == 0) {
								pA.mRhs.add(mLambda);
							}
						} else {
							currentProds.remove(j);
						}
					}
				}
			}
			if (changes) {
				factor();
				removeSameProds();
			}
		} while (changes);
	}

	/**
	 * 建立预测分析表
	 */
	private void createPredictMap() {
		mPredictMap = new int[mNonTerminalsNums][];   //预测分析表是一个二位整型数组
		for (int i = 0; i < mNonTerminalsNums; i++) {
			mPredictMap[i] = new int[mTerminalsNums];
			Arrays.fill(mPredictMap[i], PREDICT_NULL);//现将所有值都赋值为-1，初始化
		}
		HashSet<Integer> hashSet = new HashSet<Integer>();
		for (int k = mProductions.size() - 1; k >= 0; k--) {
			ArrayList<Prod> currentProds = mProductions.get(k);
			for (int i = currentProds.size() - 1; i >= 0; i--) {
				hashSet.clear();
				Prod p = currentProds.get(i);
				computeFirst(hashSet, p.mRhs); //先找到某个产生式的first集合
				if (hashSet.contains(mLambda)) {//如果first集合中包含空的话
					hashSet.remove(mLambda);
					hashSet.addAll(mFollowSet[p.mLhs - mTerminalsNums]); //hashset中添加follow集合
				}
				for (int j = 0; j < mTerminalsNums; j++) {
					if (hashSet.contains(j)) {  //哈希set中包含的first集合和follow集合中有终结符号j
						mPredictMap[k][j] = i;  //将对应的产生式标号加入数组中，知道k和i即可还原出编号对应的产生式
					} else if (mPredictMap[k][j] == PREDICT_NULL 
							&& mFollowSet[k].contains(j)) {
						mPredictMap[k][j] = PREDICT_SYNCH;
					}
				}
			}
		}
	}
/**
 * 打印预测分析表
 * @param outputFileName
 * @param printProdNumber
 * @param printProdSentence
 * @param printNumber
 * @param printName
 */
	private void printPredictMap(String outputFileName,
			boolean printProdNumber, boolean printProdSentence,
			boolean printNumber, boolean printName) {
		try {
			FileOutputStream fout = new FileOutputStream(outputFileName);
			if (printProdSentence) {
				if (printName) {
					fout.write("\t".getBytes());
					for (int i = 0; i < mTerminalsNums; i++) {
						fout.write((getWordName(i) + "\t").getBytes());
					}
					fout.write("\r\n".getBytes());
					for (int i = 0; i < mNonTerminalsNums; i++) {
						fout.write((getWordName(i + mTerminalsNums) + "\t")
								.getBytes());
						for (int j = 0; j < mTerminalsNums; j++) {
							if (mPredictMap[i][j] > -1) {
								Prod p = mProductions.get(i).get( //产生式的还原
										mPredictMap[i][j]);
								fout.write("->".getBytes());
								for (int k = 0; k < p.mRhs.size(); k++) {
									fout.write(getWordName(p.mRhs.get(k))
											.getBytes());
								}
							}
							fout.write("\t".getBytes());
						}
						fout.write("\r\n".getBytes());
					}
				}
			}
			fout.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	/**
	 * LL分析器驱动程序
	 * 
	 * @throws IOException
	 */
	public void lldriver(String inputFileName, String outputFileName)
			throws IOException {
		Scanner cin = new Scanner(new File(inputFileName)); //从词法分析结果文件中得到所需的数据
		String t = cin.nextLine();
		String[] ts = t.split("\t");
		int a = Integer.parseInt(ts[2]);         //对应该词法在词法分析字典中的序号
		String value = ts[4];                    
		int lineNumber = Integer.parseInt(ts[7]); //该条目出现在源程序所在的行数	
//		System.out.println("0:"+ts[0]);
//		System.out.println("1:"+ts[1]);
//		System.out.println("2:"+ts[2]);
//		System.out.println("3:"+ts[3]);
//		System.out.println("4:"+ts[4]);
//		System.out.println("5:"+ts[5]);
//		System.out.println("6:"+ts[6]);
//		System.out.println("7:"+ts[7]);
		boolean lastErrorHandled = false;
		FileOutputStream fout = new FileOutputStream(outputFileName);
		mGrammerTree = new GrammerAnalysisTree();//新建一棵语法分析树，分析树数据结构详见GrammerAnalysisTree类
		GrammerAnalysisTree.Node currentNode = mGrammerTree.mRoot; //根节点
		currentNode.mSymbol = mStartSymbol; //非终结符开始时的数值,即program
		Stack<GrammerAnalysisTree.Node> stack = new Stack<GrammerAnalysisTree.Node>();//构造一个GrammerAnalysisTree.Node类型的栈
		stack.push(currentNode);  //将根节点压入栈
		while (!stack.empty()) {  //如果栈不为空就说明语法分析未结束，就需要一直循环
			currentNode = stack.pop();
			int x = currentNode.mSymbol;  //当前节点的编号
			if (x >= mTerminalsNums && mPredictMap[x - mTerminalsNums][a] > -1) {  //假如x相当于非终结符，而且其在预测分析表中的值不为-1，即没有出现错误
				int k = mPredictMap[x - mTerminalsNums][a];   //将预测分析表该位置上的值取出,x - mTerminalsNums即其产生式集合的编号
				Prod p = mProductions.get(x - mTerminalsNums).get(k); //得到对应的产生式
				for (int i = p.mRhs.size() - 1; i >= 0; i--) {
					stack.push(currentNode.addSon(0, p.mRhs.get(i))); //将其右部，相当于儿子节点，压入栈
				}
				lastErrorHandled = false;
				printProduction(fout, p, false, true); //打印
			} else if (x == a) {  //假如x的值是与从词法分析结果文件中读出的序号值相等的话
				currentNode.mValue = value;
				lastErrorHandled = false;
				a = mLambda;
				value = "-";
				if (cin.hasNextLine()) {
					t = cin.nextLine();
					ts = t.split("\t");
					if (ts.length > 7) {
						a = Integer.parseInt(ts[2]);   //将a重新赋值
						value = ts[4];                 //得到其value值，INT，CH(字符常量),char等等
						lineNumber = Integer.parseInt(ts[7]);//得到对应的行号
					}
				}
			} else if (x == mLambda) { //如果x为空
				lastErrorHandled = false;
			} else {
				String error;            //错误种类
				int number = lineNumber; //错误出现的具体位置
				/**
				 * 错误恢复
				 */
				if (x >= mTerminalsNums
						&& mPredictMap[x - mTerminalsNums][a] == PREDICT_SYNCH) {
					/**
					 * 如果M[A,a]是synch，那么在试图继续分析时，栈顶的非终结符号 被弹出(根据书中的关于synch的规定)
					 */
					error = getWordName(a)+"之前缺少必要的符号";   //提示错误
				} else if (x >= mTerminalsNums
						&& mPredictMap[x - mTerminalsNums][a] == PREDICT_NULL) {
					error = "不期望的符号"+getWordName(a);      //提示错误
					/**
					 * 如果M[A,a]是空，就忽略输入符号a
					 */
					a = mLambda;
					value = "-";
					if (cin.hasNextLine()) {
						t = cin.nextLine();
						ts = t.split("\t");
						if (ts.length > 7) {
							a = Integer.parseInt(ts[2]);
							value = ts[4];
							lineNumber = Integer.parseInt(ts[7]);
						
						}
					}
				} else if (x < mTerminalsNums) {
					error = "期望输入" + getWordName(x) + ",却得到了不期望的"+getWordName(a);
					/**
					 * 如果栈顶的终结符号和输入符号不匹配，则从栈中弹出该符号
					 */
				}else{
					error = getWordName(a)+"前后有未预料的错误";
				}
				/**
				 * 出错处理
				 */
				if (!lastErrorHandled) {
					mParserErrorList.add(new CompileError(number, error));
					fout.write((error + "\r\n").getBytes());
					currentNode.mFather.mSonList.remove(currentNode);
					lastErrorHandled = true;
				}
			}
		}
		fout.close();
	}

	public void solveGrammerAnalysisTree(
			String clearedTwiceParserResultFileName,
			String clearedTwiceGrammerTreeFileName) throws IOException {
		mGrammerTree.mRoot = clearGrammerTreeUnUsedNode(mGrammerTree.mRoot,
				false);
		mGrammerTree.mRoot = clearGrammerTreeUnUsedNode(mGrammerTree.mRoot,
				true);
		printMeasuredParserResult(clearedTwiceParserResultFileName);
		printGrammerAnalysisTree(clearedTwiceGrammerTreeFileName);
	}

	/**
	 * 清除语法树里的空枝 所谓的空枝意味着此枝下的所有叶子节点要么为非终结符，要么为Lambda
	 * 
	 * @param node
	 *            要清理的枝
	 * @param full
	 *            是否收缩树枝，所谓的收缩意味着将只有一个叶子的枝换成此叶子
	 * @return 返回清理后的枝
	 */
	public GrammerAnalysisTree.Node clearGrammerTreeUnUsedNode(
			GrammerAnalysisTree.Node node, boolean full) {
		if (node.mSymbol < mTerminalsNums) {  //假如节点是终结字符，而且不为空的话就不用删除
			if (node.mSymbol != mLambda) {
				return node;
			}
			return null;
		}
		for (int i = node.mSonList.size() - 1; i >= 0; i--) {//递归调用
			GrammerAnalysisTree.Node son = clearGrammerTreeUnUsedNode(
					node.mSonList.get(i), full);
			if (son == null) {
				node.mSonList.remove(i);
			} else {
				node.mSonList.set(i, son);
			}
		}
		if (node.mSonList.size() == 0) {
			return null;
		}
		if (full) {
			if (node.mSonList.size() == 1) {
				return node.mSonList.get(0);
			}
		}
		return node;
	}

	/**
	 * 输出语法分析树到文件里
	 * 
	 * @param fileName
	 *            文件名
	 * @throws IOException
	 */
	public void printGrammerAnalysisTree(String fileName) throws IOException {
		FileOutputStream fout = new FileOutputStream(fileName);
		ArrayList<String> s = new ArrayList<String>();
		printNode(s, mGrammerTree.mRoot, 0, 3);
		for (int i = 0; i < s.size(); i++) {
			fout.write((s.get(i) + "\r\n").getBytes());
		}
		fout.close();
	}
	/**
	 * 打印每个节点
	 * @param fout
	 * @param node
	 * @param floor
	 * @param spaceNum
	 * @return
	 */
	public int printNode(ArrayList<String> fout, GrammerAnalysisTree.Node node,int floor, int spaceNum){
		String s;
		if(fout.size() <= floor){
			s = "";			
			fout.add(s);
		}else{
			s = fout.get(floor);
		}
		while(s.length() < spaceNum){
			s += " ";
		}
		if(node.mSymbol < mTerminalsNums){ //如果node是终结字符
			s += "("+getWordName(node.mSymbol)+",["+node.mValue+"])";
			fout.set(floor, s);
			return s.length();
		}else{                            //非终结字符
			int len;
			s += getWordName(node.mSymbol);
			fout.set(floor, s);
			len = s.length();
			if(node.mSonList.size() == 0){
				return len;
			}else {
				while(fout.size() < floor+4){
					fout.add("");
				}
				String ts = fout.get(floor+1);
				while(ts.length() < spaceNum+2){
					ts += " ";
				}
				ts += "|";
				fout.set(floor+1, ts);
				ts = fout.get(floor+2);
				
				int maxLen;
				int len1 = ts.length();
				maxLen = len1;
				if(fout.size() > floor + 4){
					ts = fout.get(floor + 4);
					int len2 = ts.length();
					maxLen = Math.max(maxLen, len2);
					if(node.mSonList.get(0).mSonList.size() > 0){
						if(fout.size() > floor + 6){
							ts = fout.get(floor+6);
							int len3 = ts.length();
							maxLen = Math.max(maxLen, len3);
						}
					}
				}
				ts = fout.get(floor+2);
				maxLen += 5;
				if(maxLen < spaceNum+2){
					while(ts.length()<maxLen){
						ts += " ";
					}
					ts += "+";
				}else{
					while(ts.length() < spaceNum+2){
						ts +=" ";
					}
					ts+="+";
					while(ts.length() < maxLen){
						ts+="-";
					}
					if(maxLen != spaceNum+2){
						ts+="+";
					}
				}
				
				fout.set(floor+2, ts);
				ts = fout.get(floor+3);
				while(ts.length() < maxLen){
					ts += " ";
				}
				ts += "|";
				fout.set(floor+3, ts);
				len = Math.max(len, printNode(fout, node.mSonList.get(0), floor+4, maxLen - 2));
				for(int i = 1 ; i < node.mSonList.size() ; i++){
						ts = fout.get(floor + 4);
						int len2 = ts.length();
						maxLen = len2;
						if(node.mSonList.get(i).mSonList.size() > 0){
							if(fout.size() > floor + 6){
								ts = fout.get(floor+6);
								int len3 = ts.length();
								maxLen = Math.max(maxLen, len3);
							}
						}
					maxLen += 5;
					
					ts = fout.get(floor+2);
					while(ts.length() < maxLen){
						ts += ts.length() == spaceNum+2?"+":"-";
					}
					ts+="+";
					fout.set(floor+2, ts);
					ts = fout.get(floor+3);
					while(ts.length() < maxLen){
						ts += " ";
					}
					ts+="|";
					fout.set(floor+3, ts);
					len = printNode(fout, node.mSonList.get(i), floor+4, maxLen - 2);
				}
				ts = fout.get(floor+2);
				while(ts.length() < spaceNum+2 ){
					ts += "-";
				}
				if(ts.length() == spaceNum+2){
					ts+="+";
					fout.set(floor+2, ts);
				}
				return len;
			}
		}
	}

	/**
	 * 将处理过的语法分析结果输出到文件里
	 * 
	 * @param fileName
	 * @throws IOException
	 */
	public void printMeasuredParserResult(String fileName) throws IOException {
		FileOutputStream fout = new FileOutputStream(fileName);
		printEach(fout, mGrammerTree.mRoot);
		fout.close();
	}

	private void printEach(FileOutputStream fout, GrammerAnalysisTree.Node node)
			throws IOException {
		if (node.mSymbol >= mTerminalsNums) {
			fout.write((getWordName(node.mSymbol) + "\t->\t").getBytes());
			for (int i = 0; i < node.mSonList.size(); i++) {
				fout.write(getWordName(node.mSonList.get(i).mSymbol).getBytes());
			}
			fout.write("\r\n".getBytes());
			for (int i = 0; i < node.mSonList.size(); i++) {
				printEach(fout, node.mSonList.get(i));
			}
		}
	}

	/**
	 * 返回语法分析错误列表
	 * 
	 * @return
	 */
	public ArrayList<CompileError> getErrorList() {
		return mParserErrorList;
	}
}