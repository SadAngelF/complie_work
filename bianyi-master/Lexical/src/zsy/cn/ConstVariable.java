package zsy.cn;



class ConstVariable {
	public String type;
	public String value;

	public ConstVariable(String type, String value) {
		this.type = type;
		this.value = value;
	}

	@Override
	public boolean equals(Object obj) {
		ConstVariable c = (ConstVariable) obj;
		if (c == null) {
			return false;
		}
		return c.type.equals(type) && c.value.equals(value);
	}
}