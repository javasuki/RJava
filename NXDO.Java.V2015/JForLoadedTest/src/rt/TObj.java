package rt;

import java.lang.reflect.Constructor;
import java.lang.reflect.Method;
import java.lang.reflect.TypeVariable;

public class TObj<T> implements IObj, IObj.IObj2.IObj3 {

	public TObj(){
		
	}
	
	T tt;
	public void setValue(T t){
		this.tt = t;
	}
	
	public T getValue(){		
		return this.tt;
	}
	
	public void settt(int i){
		System.out.println(i);
	}
	
	public void setArgs(int ... i){
		
	}

	public static void main(String[] args) throws Throwable {

		TObj<Dog> tt = new TObj<Dog>();
		Method mm = tt.getClass().getMethod("settt", Integer.class);
		mm.invoke(tt, new Object[]{ new Integer(1) });
		tt.setValue(new Dog("xx"));
////		System.out.print(tt.getT());
////		System.out.print(tt.getClass().getName());
//		
//		Object oAry = TObj.class.getTypeParameters();
//		TypeVariable<Class<?>>[] tvs = (TypeVariable<Class<?>>[])oAry;
//		TypeVariable<Class<?>> tv = tvs[0];
//		
//		System.out.println(tv.getName());
//		
//		Method m = TObj.class.getMethod("getT");
//		Object or = m.invoke(tt);
//		Dog d = (Dog)or;
//		System.out.print(d);
		
		//Constructor<?> ctor = TObj.class.getDeclaredConstructors()[0];
		
		
	}
}
