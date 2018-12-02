package zsy.cn;

import java.util.ArrayList;



/**
 * 
 * 语法分析树类
 *
 */


class GrammerAnalysisTree {
	public class Node {
		int mSymbol; 
		String mValue;             //节点的value
		ArrayList<Node> mSonList;  //儿子节点数组
		Node mFather;              //父节点

		public Node() {
			mSonList = new ArrayList<Node>();
		}

		public Node(int symbol) {
			mSymbol = symbol;
			mSonList = new ArrayList<Node>();
		}

		public void addSon(int index, Node son) {
			son.mFather = this;
			mSonList.add(index, son);
		}

		public Node addSon(int index, int symbol) {
			Node son = new Node(symbol);
			addSon(index, son);
			return son;
		}

		public Node getFather() {
			return mFather;
		}
	}

	public Node mRoot;

	public GrammerAnalysisTree() {
		mRoot = new Node();
		mRoot.mFather = null;
	}
}