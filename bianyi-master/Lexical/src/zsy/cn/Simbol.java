package zsy.cn;

class Simbol {
	public String name;

	public Simbol(String name) {
		this.name = name;
	}

	@Override
	public boolean equals(Object obj) {
		Simbol c = (Simbol) obj;
		if (c == null) {
			return false;
		}
		return c.name.equals(name);
	}
}
