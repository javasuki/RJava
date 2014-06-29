package rt;

public enum OEnum {
	Field{
		@Override
		public String toString() {
			return "o_field";
		}
	},
	
	Property,
	Mehotd,
	Ctor;
}
