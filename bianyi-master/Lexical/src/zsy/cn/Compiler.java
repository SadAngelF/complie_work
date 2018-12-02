package zsy.cn;

import java.util.ArrayList;
import java.util.Collections;

/**
 * 编译器类
 * 整个编译流程
 */

public class Compiler {

	
	public static void main(String[] args) {
		//进行词法分析
		NewLexical nl = new NewLexical("配置文件/词法分析字典.txt");
		nl.Anaylise("词法分析结果/词法分析结果.txt");
		
		System.out.println("词法分析结束，词法分析结果在<词法分析结果/词法分析结果.txt>、<词法分析结果/符号表.txt>、<词法分析结果/常量表.txt>中.");
		//语法分析
		System.out.println("加载文法中...");
		Parser ps = new Parser("配置文件/词法分析字典.txt","配置文件/原始文法.txt","语法分析结果/规范后文法.txt","语法分析结果/预测分析表.txt");
		System.out.println("文法加载结束，处理后的文法在<语法分析结果/规范后文法.txt>，预测分析表在<语法分析结果/预测分析表.txt>中");
		System.out.println("语法分析中...");
		
		
		ps.driver("词法分析结果/词法分析结果.txt", "语法分析结果/语法分析结果（未精简）.txt","语法分析结果/语法分析结果（精简两次）.txt","语法分析结果/语法分析树（精简两次）.txt");
		System.out.println("语法分析结束，语法分析结果在<语法分析结果>下的文件中.");
		//输出编译结果和错误列表
		ArrayList<CompileError>errorList = new ArrayList<CompileError>();

		errorList.addAll(ps.getErrorList());
		System.out.println("共产生"+errorList.size() + "个错误");
		Collections.sort(errorList, new SortByLineNumber());		
		for(int i = 0 ; i < errorList.size() ; i++){
			System.out.println("Error " + (i+1)+":\t" + errorList.get(i));
		}
		System.out.println("编译结束.");
	}

}
