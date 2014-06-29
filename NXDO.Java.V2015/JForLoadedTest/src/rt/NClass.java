package rt;

public class NClass {
	public class AClass{
		public class BClass{
			public void ttt(){
				System.out.println(NClass.AClass.this.toString());
			}
		}
	}
}
