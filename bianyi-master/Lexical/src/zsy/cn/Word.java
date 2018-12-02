package zsy.cn;

class Word {
	public String s;
	public String token;
	public int type;
	public boolean useLastCh;
	public String sType;
	public String attr;

	public String getS() {
		return s;
	}

	public void setS(String s) {
		this.s = s;
	}

	public String getToken() {
		return token;
	}

	public void setToken(String token) {
		this.token = token;
	}

	public int getType() {
		return type;
	}

	public void setType(int type) {
		this.type = type;
	}

	public boolean isUseLastCh() {
		return useLastCh;
	}

	public void setUseLastCh(boolean useLastCh) {
		this.useLastCh = useLastCh;
	}

	public String getsType() {
		return sType;
	}

	public void setsType(String sType) {
		this.sType = sType;
	}

	public String getAttr() {
		return attr;
	}

	public void setAttr(String attr) {
		this.attr = attr;
	}

	public Word(String s, int type, String token, boolean use, String sType,
			String attr) {
		this.s = s;
		this.token = token;
		this.type = type;
		this.useLastCh = use;
		this.sType = sType;
		this.attr = attr;
	}

	public String toString() {
		return token + "\t" + type;
	}
}