package rt;

import java.lang.reflect.Method;
import java.lang.reflect.Modifier;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Date;
import java.util.Dictionary;
import java.util.Enumeration;
import java.util.Iterator;
import java.util.List;
import java.util.Queue;
import java.util.Map.Entry;
import java.util.Set;
import java.util.Stack;

public class DemoE {

	public DemoE(){
		System.out.println("<init>");
	}
	
	public DemoE(String s){	
		System.out.println("<init>_s" + s);
	}
	
	public DemoE(int i){
		System.out.println("<init>_i" + i);
	}
	
	public OEnum TestEnum(OEnum o){
		//o.ordinal()
		System.out.println("Set OEnum:" + o);
		System.out.println("");
		
		return o;
	}
	
	public int FFF1 = 0;
	public String SFinal = "xx";
	public static String SStatic = "xx2";
	public final short FShort = 3;
	public static int[] vvv = new int[]{1,2,3};
	
	public Object TestO(Object o){
		System.out.println("TestO:" + o);
		System.out.println("");
		return o;
	}
	
	public OEnum[] TestEnumAry(OEnum[] o){
		for(OEnum vv : o){
			System.out.println(vv);
			System.out.println(vv == null ? "is null" : vv.getClass().getName());
		}
		
		System.out.println("Set OEnum[]:" + o);
		System.out.println("");
		
		return o;
	}
	
	public void AddList(List<Integer> lst){
		
		System.out.println("AddList:" + lst.size());
		for(Integer i : lst)
			System.out.println(i);
		System.out.println();
	}
	
	public void AddArrayList(ArrayList<Integer> lst){
		System.out.println("AddArrayList:" + lst.size());
		for(Integer i : lst)
			System.out.println(i);
		System.out.println();
	}
	
	public void AddArrayListAry(ArrayList<Integer>[] lst){
		System.out.println("AddArrayListAry:" + lst.length);
		for(ArrayList<Integer> a : lst){
			System.out.println(a);
			for(Integer i : a)
				System.out.println(i);
		}
		System.out.println();
	}
	
	
	public Object[] TestOAry(Object[] o){
		for(Object vv : o){
			System.out.println(vv);
			System.out.println(vv == null ? "is null" : vv.getClass().getName());
		}
		
		System.out.println("Set TestEnumAry[]:" + o);
		System.out.println("");
		
		return o;
	}
	
	public java.math.BigDecimal OOD(java.math.BigDecimal bd){
		System.out.println("Set OOD:" + bd);
		System.out.println("");
		return bd;
	}
	
	public java.math.BigDecimal[] OODAry(java.math.BigDecimal[] bd){
		System.out.println("Set OODAry:" + bd[0]);
		System.out.println("");
		return bd;
	}
	
	
	public Date TestDT(Date dt){
		//java.util.Calendar;
		if(dt != null){
		SimpleDateFormat bartDateFormat = new SimpleDateFormat("yyyy-MM-dd");
		String s = bartDateFormat.format(dt);
		System.out.println(s);
		}
		else 
			System.out.println("date is null");
		return dt;
	}
	
	public Date[] TestDTAry(Date[] dts){
		System.out.println("TestDT array");
		return dts;
	}
	
	public void ttt(){
	}
	
	public void InClass(Class<?> cls){
		System.out.println(cls);
	}
	
	public void InClassAry(Class<?>[] cls){
		System.out.println(cls);
	}
	
	public <T extends Number> void SetTValue(T t){
		System.out.println(t);
	}

	public static void main(String[] args) throws ParseException, Throwable {
//		Method m = ArrayList.class.getMethod("add", Object.class);
//		boolean ispublic = Modifier.isPublic(m.getModifiers());
//		System.out.println(ispublic);
		
		Method[] ms = ArrayList.class.getMethods();
		System.out.println(ms.length);
		
	}

}
