package rt;

//import jfor2.Cat;

public class Man extends Person {
	
	@Override
	public void doString(String dd) throws Exception{
		System.out.println("java invoke:	Man.doString():" + Name);
	}
	
	public String Name;
	
	public <T extends Number> void doOkTest(T t, String s, T[] tAry){
		
	}
	
	//public Cat MyCat;
	
	//public static Cat MyCat2;

	public class ManN1{	
		class ManN3{
			public class ManN4{
				
			}
		}
	}
	
	public class ManN2{
		
	}
}
