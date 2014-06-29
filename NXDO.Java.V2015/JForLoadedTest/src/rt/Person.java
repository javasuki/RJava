package rt;

//import jfor2.Cat;

public class Person {

	public Person(){
		
	}
	
	
	
	public Person(int age, Dog dog){
		this._dog = dog;
		this._age = age;
	}
	
	public Person(int age){
		this._age = age;
	}
	
	Dog _dog;
	public void setDog(rt.Dog dog){
		System.out.println("setDog ok!");//("setDog:" + dog.toString());
		_dog = dog;
		
	}
	
	public Dog getDog(){
		//System.out.println("getDog:" + _dog.toString());
		return _dog;
	}
	
	int _age;
	public boolean getBool(){	
		return false;
	}
	
	public byte getByte(){	
		return 0x01;
	}
	
	public char getChar(){	
		return 'a';
	}
	
	public double getD(){	
		return new Double(100.0).doubleValue();
	}
	
	public long getL(){
		return 99;
	}
	
	
	public float getF(){	
		return new Float(10.0).floatValue();
	}
	
	public int getI(){	
		return 9;
	}
	
	public short getS(){		
		return 8;
	}
	
	public String getXXX(){
		return "I'm person";
	}
	
	public Integer getAge(){
		return _age;
	}
	
	public void setAge(Integer age){
		//System.out.println("java setAge:");
		_age = age;
		
		//Integer ii = new Integer(value);
	}
	
	public void setSex(Boolean sex){
		//System.out.println("java setAge:" + sex.toString());
	}

	public void doPrint(){
		if(_age==0)
			_age = 1;
//		System.out.println("java print:");
//		System.out.println(_age);
	}	 
	
	public void doString(String dd) throws Exception{
//		if(dd.compareTo("yyy") == 0){
//			Exception exp = new Exception("not support.");
//			throw exp;
//		}
		
		if(dd == null){
			System.out.println("java invoke:	doString 参数为  null.");
			//throw new Exception("参数为  null");
		}
		
		System.out.println("java invoke:	doString");
	}
	
	public static void doStatic(){
		System.out.println("java invoke:	doStatic");
	}
	
	public static int doStatic2(){
		return 1;
	}
	
//	public static Cat doStatic3(Dog dog){
//		return dog._cat;
//	}
	
	public void doDogTest(String dd, Dog dog){
		System.out.println("java invoke:	doDogTest");
	}
	
	public Dog getReturnDog(String ss){
		return new Dog(ss);
	}
	
	public int getReturnInt(String ss){
		return ss == null ? 0 : 1;		 
	}
	
	public String getReturnString(String ss){
		return "你 abc/123";
	}
}
