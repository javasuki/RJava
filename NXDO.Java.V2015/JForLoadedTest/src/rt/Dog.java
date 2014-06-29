package rt;

import java.io.File;
import java.lang.reflect.Method;
import java.lang.reflect.Modifier;
import java.lang.reflect.ParameterizedType;
import java.lang.reflect.Type;
import java.net.URL;
import java.util.Enumeration;
import java.util.jar.JarEntry;
import java.util.jar.JarFile;

public class Dog {

	public Dog(){
		
	}
	
	String _name;
	public Dog(String dogName){
		this._name = dogName;
		
		fldStrAry = new String[]{"1f","2f","3f"};
		//this.setStrAry("1","2","3");
		
		fldDogAry = new Dog[]{};
	}
	
	public Dog(int ii){
		
	}
	
	public String[] fldStrAry;
	public int[] fldIntAry;
	public Integer[] fldIObjAry;
	public Dog[] fldDogAry;
	
	public void print(){
		for(Dog s : fldDogAry){
			if(s == null)
				System.out.println("dog IS NULL");
			else
				System.out.println(s._name);
		}
		System.out.println("invoke setDogAry field");
	}
	
	public <T> void testT(T t){		
		System.out.println(t);
	}
	
	Object[] oArys = null;
	public <T> void testTAry(T ... ts){	
		oArys = ts;
		for(T t : ts)
		System.out.println(t);
	}
	
	@SuppressWarnings("unchecked")
	public <T> T[] getTestTAry(){
		return (T[])oArys;
	}
	
	public <T> T GetRunT(T t){
		System.out.println(t);
		return t;
	}
	
	public int doGetintMethod(){
		return 0;
	}
	
	public Dog(String dogName, String[] args){
		this._name = dogName;
		
		for(String s : args){
			//System.out.println(s);
		}
		//System.out.println("invoke Dog .ctor(string,string[])");
		
		fldStrAry = new String[]{"1f","2f","3f"};
		fldIntAry = new int[]{1,2,3};
		fldIObjAry = new Integer[]{1,null,3};
		fldDogAry = new Dog[]{
				new Dog("xx"),
				new Dog("yy"),
				new Dog("zz")
			};
	}
	
	
	public Dog[] getDogAry(){
		return new Dog[]{
			new Dog("xx"),
			new Dog("yy"),
			new Dog("zz")
		};
	}
	
	public void setDogAry(Dog[] values){
		for(Dog s : values){
			if(s == null)
				System.out.println("dog IS NULL");
			else
				System.out.println(s._name);
		}
		System.out.println("invoke setDogAry");
	}
	
	public Integer[] getIntegerAry(){
		return new Integer[]{
				1,
				null,
				3
		};
	}
	
	public void setIntegerAry(Integer[] values){
		for(Integer s : values){
			System.out.println(s);
		}
		System.out.println("invoke setIntegerAry");
	}
	
	public void setAge(int age)
	{
		System.out.println("setAge = " + age);
	}
	
	public void setAgeI(Integer age)
	{
		System.out.println("setAgeI = " + age);
	}
	
	public String getName(){
		return this._name;		
	}
	
	public void setName(String dogName){
		this._name = dogName;
	}

	public void setStrAry(String ... values){
		for(String s : values)
			System.out.println(s);
		System.out.println("string... invoke");
	}
	
	public void setStrAry2(String[] values){
		for(String s : values){
			System.out.println(s);
		}
		System.out.println("invoke array");
	}
	
	public String[] getStrAry2(){
		return new String[]{"123","abc","hello"};
	}
	
	public void setBoolAry(boolean[] values){
		for(boolean s : values){
			System.out.println(s);
		}
		System.out.println("invoke setBoolAry");
	}
	
	public boolean[] getBoolAry(){
		return new boolean[]{true,false,true};
	}
	
	public void setByteAry(byte[] values){
		for(byte s : values){
			System.out.println(s);
		}
		System.out.println("invoke setByteAry");
	}
	
	public byte[] getByteAry(){
		return new byte[]{ 0x01, 0x02,0x03};
	}
	
	public void setCharAry(char[] values){
		for(char s : values){
			System.out.println(s);
		}
		System.out.println("invoke setCharAry");
	}
	
	public char[] getCharAry(){
		return new char[]{ 'a','b','c'};
	}
	
	public void setShortAry(short[] values){
		for(short s : values){
			System.out.println(s);
		}
		System.out.println("invoke setShortAry");
	}
	
	public short[] getShortAry(){
		return new short[]{ 6,7,8};
	}
	
	public void setIntAry(int[] values){
		for(int s : values){
			System.out.println(s);
		}
		System.out.println("invoke setIntAry");
	}
	
	public int[] getIntAry(){
		return new int[]{ 0,10,20};
	}
	
	public void setLongAry(long[] values){
		for(long s : values){
			System.out.println(s);
		}
		System.out.println("invoke setLongAry");
	}
	
	public long[] getLongAry(){
		return new long[]{ (long) 11.1,(long) 22.2,(long) 33.3};
	}

	public void setFloatAry(float[] values){
		for(float s : values){
			System.out.println(s);
		}
		System.out.println("invoke setFloatAry");
	}
	
	public float[] getFloatAry(){

		System.out.println("invoke getFloatAry");
		
		return new float[]{ (float) 1.0,(float) 2.0,(float) 3.0};
	}
	
	public void setDoubleAry(double[] values){
		for(double s : values){
			System.out.println(s);
		}
		System.out.println("invoke setDoubleAry");
	}
	
	public final double[] getDoubleAry(){
		return new double[]{ (double) 1.1,(double) 2.2,(double) 3.3};
	}
	
	private void testM(String s,int ii){
		
	}
	
	public static void main(String[] args) throws Throwable {
				
		String fname = "E:\\DotNet2012\\CSharp2Java\\NXDO.Mixed.V2015\\Tester\\RJavaX64\\bin\\Debug\\jforloaded.jar";
		JarFile jar = new JarFile(fname);
		Enumeration<JarEntry> entries = jar.entries();
        while (entries.hasMoreElements()) {
            String entry = entries.nextElement().getName();
            if(!entry.endsWith(".class"))continue;
            System.out.println(entry);
        }
		
		for(Method m : Dog.class.getDeclaredMethods()){
			
			if(m.getModifiers() == Modifier.PRIVATE)continue;
			//Method m1 = Dog.class.getDeclaredMethod("testT", Object.class);
			if(m.getName() == "testT"){
				Class<?>[] clsAry = m.getParameterTypes();
				System.out.println(m.isVarArgs());
			}
			if(m.getName() == "setStrAry")
				System.out.println(m.isVarArgs());
			if(m.getName() == "setStrAry2")
				System.out.println(m.isVarArgs());
			
			System.out.println(m.toGenericString());
		}		
		int ir=1;
		if(ir==1)return;
		
		new Dog("xx").testT(null);
//		System.out.println(float.class.getSimpleName());
//		Class<?> fcls = Class.forName(float.class.getName());
//		System.out.println(fcls.getName());
//		System.out.println("");
		System.out.println(Dog[].class.getName());
		
		System.out.println(boolean.class.getName());
		System.out.println(byte.class.getName());
		System.out.println(char.class.getName());
		System.out.println(short.class.getName());
		System.out.println(int.class.getName());
		System.out.println(long.class.getName());
		System.out.println(float.class.getName());
		System.out.println(double.class.getName());
		System.out.println("");
		
		System.out.println(boolean[].class.getName());
		System.out.println(Boolean[].class.getName());
		System.out.println(byte[].class.getName());
		System.out.println(Byte[].class.getName());
		System.out.println(char[].class.getName());
		System.out.println(Character[].class.getName());
		System.out.println(short[].class.getName());
		System.out.println(Short[].class.getName());
		System.out.println(int[].class.getName());
		System.out.println(Integer[].class.getName());
		System.out.println(long[].class.getName());
		System.out.println(Long[].class.getName());
		System.out.println(float[].class.getName());
		System.out.println(Float[].class.getName());
		System.out.println(double[].class.getName());
		System.out.println(Double[].class.getName());
		System.out.println(String[].class.getName());
		System.out.println(Object[].class.getName());
		

		
		System.out.println(String.class.getName()); //java.lang.String
		Class<?> cls = Class.forName("[Ljava.lang.String;");//String[].class.getName());
		Method m = Dog.class.getMethod("setStrAry", cls);
		System.out.println(m.isVarArgs());
		System.out.println(m.getName());
		
		m = Dog.class.getMethod("setStrAry2", cls);
		System.out.println(m.isVarArgs());
		
//		m = Dog.class.getMethod("setName", String.class);
//		System.out.println(m.isVarArgs());
	}
	
//	Cat _cat;
//	public void setFriend(Cat cat){
//		
//		if(_cat == null)
//			_cat = cat;
//		else{
//			System.out.println("java invoke: dog's friend is " + (_cat == cat ? "smae" : "diffrent"));
//		}
//		
//		System.out.println("java invoke: dog's friend is " + cat.toString());
//	}

}
