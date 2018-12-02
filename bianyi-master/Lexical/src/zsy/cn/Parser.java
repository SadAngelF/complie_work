package zsy.cn;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Scanner;
import java.util.Stack;

/**
 * Óï·¨·ÖÎöÆ÷
 */
public class Parser {
	private Grammer mGrammer;

	public Parser(String terminalsDictFileName, String originalGrammerFileName,
			String newGrammerFileName, String predictMapFileName) {
		mGrammer = new Grammer(terminalsDictFileName, originalGrammerFileName,
				newGrammerFileName, predictMapFileName);
	}

	public void driver(String lexicalAnalsisResultFileName,
			String originalParserResultFileName,
			String clearedTwiceParserResultFileName,
			String clearedTwiceGrammerTreeFileName) {
		try {
			mGrammer.lldriver(lexicalAnalsisResultFileName,
					originalParserResultFileName);
			mGrammer.solveGrammerAnalysisTree(
					clearedTwiceParserResultFileName,
					clearedTwiceGrammerTreeFileName);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	public ArrayList<CompileError> getErrorList() {
		return mGrammer.getErrorList();
	}
}
