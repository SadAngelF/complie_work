package zsy.cn;

import java.util.ArrayList;

/**
 * 产生式
 */
class Prod {
	public int mLhs; // 产生式的左边
	public ArrayList<Integer> mRhs; // 产生式的右边
	public String mComment;

	public String toString() {
		String s = mLhs + " ->";
		for (int i = 0, len = mRhs.size(); i < len; i++) {
			s += " " + mRhs.get(i);
		}
		return s;
	}

	/**
	 * 判断两个文法是否相等
	 */
	public boolean equals(Object b) {
		Prod pB = (Prod) b;
		if (pB == null || mRhs.size() != pB.mRhs.size()) {
			return false;
		}
		for (int i = mRhs.size() - 1; i >= 0; i--) {
			int ai = mRhs.get(i);
			int bi = pB.mRhs.get(i);
			if (ai != bi || ai == pB.mLhs || ai == mLhs) {
				if (ai != mLhs || bi != pB.mLhs) {
					return false;
				}
			}
		}
		return true;
	}
}