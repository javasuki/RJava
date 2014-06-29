package rt;

import java.lang.reflect.Method;

public class Demo {

	public Demo(){
		System.out.println("ctor");
	}
	
	public Demo(boolean v){
		System.out.println("ctor boolean:" + v);
	}
	
	public Demo(boolean[] v){
		System.out.println("ctor boolean[]:" + v);
	}
	
	public Demo(Boolean v){
		System.out.println("ctor Boolean:" + v);
	}
	
	public Demo(Boolean[] v){
		System.out.println("ctor Boolean[]:" + v);
	}

	public Demo(int v){
		System.out.println("int:" + v);
	}
	
	public Demo(Integer v){
		System.out.println("Integer:" + v);
	}
	
	public Demo(String v){
		System.out.println("String:" + v );
	}
	
	public Demo(String[] v){
		for(String vv : v)
			System.out.println(vv);
		System.out.println("String[]:" + v );
	}
	
	public Demo(Object v){
		System.out.println("Object:" + v );
	}
	
	public Demo(Demo v){
		System.out.println("Demo:" + v );
	}
	
	public Demo(Demo[] v){
		for(Demo vv : v)
			System.out.println(vv);
		System.out.println("Demo[]:" + v );
	}

	public void Say(){
		System.out.println("Say invoke");
	}
	
	public boolean Say(boolean b){
		System.out.println("Say bool:" + b);
		return b;
	}
	
	public Boolean Say(Boolean b){
		System.out.println("Say Boolean:" + b);
		return b;
	}
	
	public byte Say(byte b){
		System.out.println("Say byte:" + b);
		return b;
	}
	
	public Byte Say(Byte b){
		System.out.println("Say Byte:" + b);
		return b;
	}
	
	public char Say(char c){
		System.out.println("Say char:" + c);
		return c;
	}
	
	public Character Say(Character c){
		System.out.println("Say Character:" + c);
		return c;
	}
	
	public short Say(short c){
		System.out.println("Say short:" + c);
		return c;
	}
	
	public Short Say(Short c){
		System.out.println("Say Short:" + c);
		return c;
	}
	
	public int Say(int c){
		System.out.println("Say int:" + c);
		return c;
	}
	
	public Integer Say(Integer c){
		System.out.println("Say Integer:" + c);
		return c;
	}
	
	public long Say(long c){
		System.out.println("Say long:" + c);
		return c;
	}
	
	public Long Say(Long c){
		System.out.println("Say Long:" + c);
		return c;
	}
	
	public float Say(float c){
		System.out.println("Say float:" + c);
		return c;
	}
	
	public Float Say(Float c){
		System.out.println("Say Float:" + c);
		return c;
	}
	
	public double Say(double c){
		System.out.println("Say double:" + c);
		return c;
	}
	
	public Double Say(Double c){
		System.out.println("Say Double:" + c);
		return c;
	}
	
	public String Say(String c){
		System.out.println("Say String:" + c);
		return c;
	}
	
	public Demo Say(Demo c){
		System.out.println("Say v:" + c);
		return c;
	}
	
	
	public boolean[] Say(boolean[] c){
		for(boolean vv : c)
			System.out.println(vv);
		System.out.println("Say boolean[]:" + c);
		System.out.println("");
		return c;
	}
	
	public Boolean[] Say(Boolean[] c){
		for(Boolean vv : c){
			System.out.println(vv);
		}
		System.out.println("Say Boolean[]:" + c);
		System.out.println("");
		return c;
	}
	
	public byte[] Say(byte[] c){
		for(byte vv : c)
			System.out.println(vv);
		System.out.println("Say byte[]:" + c);
		System.out.println("");
		return c;
	}
	
	public Byte[] Say(Byte[] c){
		for(Byte vv : c)
			System.out.println(vv);
		System.out.println("Say Byte[]:" + c);
		System.out.println("");
		return c;
	}
	
	public char[] Say(char[] c){
		for(char vv : c)
			System.out.println(vv);
		System.out.println("Say char[]:" + c);
		System.out.println("");
		return c;
	}
	
	public Character[] Say(Character[] c){
		for(Character vv : c)
			System.out.println(vv);
		System.out.println("Say Character[]:" + c);
		System.out.println("");
		return c;
	}
	
	public short[] Say(short[] c){
		for(short vv : c)
			System.out.println(vv);
		System.out.println("Say short[]:" + c);
		System.out.println("");
		return c;
	}
	
	public Short[] Say(Short[] c){
		for(Short vv : c)
			System.out.println(vv);
		System.out.println("Say Short[]:" + c);
		System.out.println("");
		return c;
	}
	
	public int[] Say(int[] c){
		for(int vv : c)
			System.out.println(vv);
		System.out.println("Say int[]:" + c);
		System.out.println("");
		return c;
	}
	
	public Integer[] Say(Integer[] c){
		for(Integer vv : c)
			System.out.println(vv);
		System.out.println("Say Integer[]:" + c);
		System.out.println("");
		return c;
	}
	
	public long[] Say(long[] c){
		for(long vv : c)
			System.out.println(vv);
		System.out.println("Say long[]:" + c);
		System.out.println("");
		return c;
	}
	
	public Long[]  Say(Long[] c){
		for(Long vv : c)
			System.out.println(vv);
		System.out.println("Say Long[]:" + c);
		System.out.println("");
		return c;
	}
	
	public float[] Say(float[] c){
		for(float vv : c)
			System.out.println(vv);
		System.out.println("Say float[]:" + c);
		System.out.println("");
		return c;
	}
	
	public Float[] Say(Float[] c){
		for(Float vv : c)
			System.out.println(vv);
		System.out.println("Say Float[]:" + c);
		System.out.println("");
		return c;
	}
	
	public double[] Say(double[] c){
		for(double vv : c)
			System.out.println(vv);
		System.out.println("Say double[]:" + c);
		System.out.println("");
		return c;
	}
	
	public Double[] Say(Double[] c){
		for(Double vv : c)
			System.out.println(vv);
		System.out.println("Say Double[]:" + c);
		System.out.println("");
		return c;
	}
	
	public String[] Say(String[] c){
		for(String vv : c)
			System.out.println(vv);
		System.out.println("Say String[]:" + c);
		System.out.println("");
		return c;
	}
	
	public Demo[] Say(Demo[] c){
		for(Demo vv : c)
			System.out.println(vv);
		System.out.println("Say Demo[]:" + c);
		System.out.println("");
		return c;
	}
	
	
	public Object SayObject(Object c){
		
		System.out.println("SayObject Object:" + c.toString());
		System.out.println("");
		return c;
	}
	
	public Object[] SayObjectAry(Object[] c){
		for(Object vv : c){
			System.out.println(vv);
			System.out.println(vv == null ? "is null" : vv.getClass().getName());
		}
		System.out.println("SayObject Object[]:" + c);
		System.out.println("");
		return c;
		//return new Object[]{1,2,3};
		//return new Object[]{};
	}
	
	
	
	
	public <T> T SayT(T t){
		System.out.println("SayT invoke:" + t);
		return t;
	}
	
	public <T> T[] SayT(T[] ts){
		for(T vv : ts)
			System.out.println(vv);
		System.out.println("SayT invoke:" + ts);
		return ts;
	}
	
	public static void TestStatic(){
		System.out.println("TestStatic");
	}
	
	public static int FieldSI = 1000;
	public static Integer FieldSLI = 2000;
	
	public int FieldI = 100;
	public Integer FieldLI = 200;
	
	private int iii;
	public void setAge(int i){
		iii = i;
	}
	
	public int getAge()
	{
		return iii+1;
	}
	
	public boolean DBool(boolean b){
		System.out.println("DBool:" + b);
		return b;
	}
	
	public Boolean DBool(Boolean b){
		System.out.println("O DBool:" + b);
		return b;
	}
	
	public void DBoolArray(boolean[] bs){
		System.out.println("bs length:" + bs.length);
		for(boolean b : bs)
			System.out.println(b);
		System.out.println("DBoolArray");
	}
	
	public void DBoolArray(Boolean[] bs){
		System.out.println("bs length:" + bs.length);
		for(Boolean b : bs)
			System.out.println(b);
		System.out.println("O DBoolArray");
	}
	
	public void DString(String s){
		System.out.println("DString:" + s);
	}
	
	public void DStringAry(String[] ss){
		System.out.println("ss length:" + ss.length);
		for(String b : ss)
			System.out.println(b);
		System.out.println("DStringAry");
	}
	
	public void DDemo(Demo o){
		System.out.println("DDemo:" + o != null ? o.toString() : " is null");
	}
	
	public void DDemoAry(Demo[] os){
		System.out.println("os length:" + os.length);
		for(Demo o : os)
			System.out.println(o != null ? o.toString() : " is null");
		System.out.println("DDemoAry");
	}
	
	public void DObject(Object o){
		System.out.println("DObject:" + o != null ? o.toString() : " is null");
	}
	
	public void DObjectAry(Object[] os){
		System.out.println("os length:" + os.length);
		for(Object o : os)
			System.out.println(o != null ? o.toString() : " is null");
		
		System.out.println("DObjectAry");
	}
	
	public <T> void DTObj(T o){
		boolean b = o == null;
		System.out.println("DTObj:" + (b ? " is null" : o.toString()));
		
	}
	
	public <T> void DTObjAry(T[] os){
		System.out.println("os length:" + os.length);
		for(T o : os)
			System.out.println(o != null ? o.toString() : " is null");
		
		System.out.println("DTObjAry");
	}

	
	public static void main(String[] args) throws Throwable {
		
		
		
//		System.out.println(Demo[].class.getName());
//		System.out.println(int[].class.getName());
//		
//		Class ioCls = Class.forName("[Ljava.lang.Integer;");
//		
//		Class<?>[] clsAry = new Class<?>[2];
//		clsAry[0] = int.class;
//		clsAry[1] = int[].class;
		
		Demo d = new Demo();
		d.SayObject(new Integer(1));
		Object[] oAry = d.SayObjectAry(new Object[]{1,2,3});
		d.DTObj(null);
		
		Object[] values = new Demo[1];
		values[0] = d;
		//values[1] = new Demo();
		
		Method m = Demo.class.getMethod("Say", Demo[].class);
		m.setAccessible(true);
		m.invoke(d, new Object[]{values});
	}
}
