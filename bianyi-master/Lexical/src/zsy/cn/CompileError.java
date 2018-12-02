package zsy.cn;

import java.util.Comparator;
/**
 * 错误类
 * 含有错误的行号和错误的具体信息提示
 * 
 *
 */
public class CompileError {
	public int LineNumber;//行号
	public String ErrorMessage;//提示信息

	public CompileError(int lineNumber, String errorMessage) {
		LineNumber = lineNumber;
		ErrorMessage = errorMessage;
	}

	public String toString() { //打印提示信息
		return "Line " + LineNumber + ":\t" + ErrorMessage;
	}
}
//按照行号进行排序的一个接口
class SortByLineNumber implements Comparator<CompileError> {

	@Override
	public int compare(CompileError o1, CompileError o2) {
		if (o1.LineNumber > o2.LineNumber) {
			return 1;
		} else {
			if (o1.LineNumber < o2.LineNumber) {
				return -1;
			}
		}
		return 0;
	}
}