package jxdo.rjava;

import java.io.File;
import java.lang.reflect.Constructor;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.net.URL;
import java.net.URLClassLoader;
import java.util.ArrayList;
import java.util.List;

class JarLoader {
	
	List<Object> lstValues;
	List<Class<?>> lstClasses;
	static ClassLoader clazzLoader;
	
	/**
	 * invoke in c++ only first.
	 * @param jarNames
	 * @throws Throwable
	 */
	public JarLoader(String jarNames) throws Throwable {
		lstValues = new ArrayList<Object>();
		lstClasses = new ArrayList<Class<?>>();
		
		
		if(clazzLoader == null){
			boolean bGetContextLoader = false;
			if(jarNames == null)
				bGetContextLoader = true;
			else if(jarNames.length() == 0)
				bGetContextLoader = true;
			
			if(bGetContextLoader)
				clazzLoader = this.getClass().getClassLoader(); //Thread.currentThread().getContextClassLoader();
			else
				clazzLoader = this.getJarClassLoader(jarNames, this.getClass().getClassLoader());
		}
		//System.out.println("load fst:"+jarNames);
		
		
//		Object INSTRUMENTATION_KEY = UUID.fromString("214ac54a-60a5-417e-b3b8-772e80a16667");
//		(Instrumentation) System.getProperties().get(INSTRUMENTATION_KEY);
//		Field f = ClassLoader.class.getDeclaredField("classes");
//		f.setAccessible(true);
//		Vector<Class> classes =  (Vector<Class>) f.get(clazzLoader);
//		for(Class c : classes ){
//			System.out.println(c.getName());
//		}
//		System.out.println("---------------");
	}
		
	/**
	 * invoke in c++, continue add jar file, create new children ClassLoader.
	 * @param jarNames
	 * @throws Throwable
	 */
	@SuppressWarnings("unused")
	private void addClassLaoder(String jarNames) throws Throwable {
		//System.out.println("load agn:" + jarNames);
		
		if(jarNames == null)return;
		else if(jarNames.length() == 0)return;
		
		ClassLoader newClazzLoader = this.getJarClassLoader(jarNames, clazzLoader);
		if(newClazzLoader == clazzLoader)return;
		clazzLoader = newClazzLoader;
	}

	private ClassLoader getJarClassLoader(String jarNames, ClassLoader parentClassLoader) throws Throwable{	
		if(jarNames == null)return parentClassLoader;
		else if(jarNames.length() == 0)return parentClassLoader;
		
		List<URL> lstUrls = new ArrayList<URL>();
		String[] jarNameAry = jarNames.indexOf(";") > -1 ? jarNames.split(";") : new String[]{ jarNames };
		for(String jarName : jarNameAry){	
			File file  = new File(jarName);
			if(!file.exists())
			{
				java.io.FileNotFoundException exp = new java.io.FileNotFoundException(jarName);
				if(exp.getMessage() == null){
					Throwable ta = exp.getCause();
					if(ta!=null)throw ta;
				}
				throw exp;
			}
			
			URL jarUrl = new URL("jar", "","file:" + file.getAbsolutePath()+"!/"); 
			lstUrls.add(jarUrl);
		}
		
		if(lstUrls.size()==0)return parentClassLoader;		
		return URLClassLoader.newInstance(lstUrls.toArray(new URL[0]), parentClassLoader);
	}
	
	public void clearParamValue(){	
		this.lstValues.clear();
		this.lstClasses.clear();
	}
	
	public void addParamValue(boolean o){	
		this.lstValues.add(o);		
		this.lstClasses.add(boolean.class);
	}
	
	public void addParamValue(byte o){	
		this.lstValues.add(o);		
		this.lstClasses.add(byte.class);
	}
	
	public void addParamValue(char o){	
		this.lstValues.add(o);		
		this.lstClasses.add(char.class);
	}
	
	public void addParamValue(short o){	
		this.lstValues.add(o);		
		this.lstClasses.add(short.class);
	}
	
	public void addParamValue(int o){	
		this.lstValues.add(o);		
		this.lstClasses.add(int.class);
	}
	
	public void addParamValue(long o){	
		this.lstValues.add(o);		
		this.lstClasses.add(long.class);
	}
	
	public void addParamValue(float o){	
		this.lstValues.add(o);		
		this.lstClasses.add(float.class);
	}
	
	public void addParamValue(double o){	
		this.lstValues.add(o);		
		this.lstClasses.add(double.class);
	}

	
	public void addParamValue(Object o, String className) throws Throwable{
		this.lstValues.add(o);
		
		//array类型,容错方式
		if(className != null){
			if(className.startsWith("[L") && !className.endsWith(";"))
				className += ";";	
			
			//System.out.println("addParamValue:"+className);
			this.lstClasses.add(this.getClass(className));
		}
		else
			this.lstClasses.add(o == null ? this.getClass(className) : o.getClass());
	}	
	
//	public void addParamNullValue(Class<?> clazz){
//		this.lstValues.add(null);
//		this.lstClasses.add(clazz);
//	}
	
	public boolean invokeBoolMethod(Object obj, String methodName) throws Throwable{
		Object o = this.invokeObjectMethod(obj, methodName);		
		return ((Boolean)o).booleanValue();
	}
	
	public byte invokeByteMethod(Object obj, String methodName) throws Throwable{
		Object o = this.invokeObjectMethod(obj, methodName);
		return ((Byte)o).byteValue();
	}
	
	public char invokeCharMethod(Object obj, String methodName) throws Throwable{
		Object o = this.invokeObjectMethod(obj, methodName);
		return ((Character)o).charValue();
	}
	
	public short invokeShortMethod(Object obj, String methodName) throws Throwable{
		Object o = this.invokeObjectMethod(obj, methodName);
		return ((Short)o).shortValue();
	}	
	
	public int invokeIntMethod(Object obj, String methodName) throws Throwable{
		Object o = this.invokeObjectMethod(obj, methodName);
		return ((Integer)o).intValue();
	}
	
	public long invokeLongMethod(Object obj, String methodName) throws Throwable{
		Object o = this.invokeObjectMethod(obj, methodName);
		return ((Long)o).longValue();
	}
	
	public float invokeFloatMethod(Object obj, String methodName) throws Throwable{
		Object o = this.invokeObjectMethod(obj, methodName);
		return ((Float)o).floatValue();
	}
	
	public double invokeDoubleMethod(Object obj, String methodName) throws Throwable{
		Object o = this.invokeObjectMethod(obj, methodName);
		return ((Double)o).doubleValue();
	}
	
	public String invokeStringMethod(Object obj, String methodName) throws Throwable{
		Object o = this.invokeObjectMethod(obj, methodName);
		return (String)o;
	}
	
	
	
	public boolean invokeBoolField(Object obj, String fieldName) throws Throwable{
		Object o = this.invokeObjectField(obj, fieldName, false);		
		return ((Boolean)o).booleanValue();
	}
	
	public byte invokeByteField(Object obj, String fieldName) throws Throwable{
		Object o = this.invokeObjectField(obj, fieldName, false);	
		return ((Byte)o).byteValue();
	}
	
	public char invokeCharField(Object obj, String fieldName) throws Throwable{
		Object o = this.invokeObjectField(obj, fieldName, false);	
		return ((Character)o).charValue();
	}
	
	public short invokeShortField(Object obj, String fieldName) throws Throwable{
		Object o = this.invokeObjectField(obj, fieldName, false);	
		return ((Short)o).shortValue();
	}	
	
	public int invokeIntField(Object obj, String fieldName) throws Throwable{
		Object o = this.invokeObjectField(obj, fieldName, false);	
		return ((Integer)o).intValue();
	}
	
	public long invokeLongField(Object obj, String fieldName) throws Throwable{
		Long l = new Long(100);
		Object o = this.invokeObjectField(obj, fieldName, false);	
		return ((Long)o).longValue();
	}
	
	public float invokeFloatField(Object obj, String fieldName) throws Throwable{
		Object o = this.invokeObjectField(obj, fieldName, false);	
		return ((Float)o).floatValue();
	}
	
	public double invokeDoubleField(Object obj, String fieldName) throws Throwable{
		Object o = this.invokeObjectField(obj, fieldName, false);	
		return ((Double)o).doubleValue();
	}
	
	public String invokeStringField(Object obj, String fieldName) throws Throwable{
		Object o = this.invokeObjectField(obj, fieldName, false);	
		return (String)o;
	}
	
	public Class<?> getClass(String className) throws Throwable{
//		if(className.indexOf('/')>-1)
//			className = className.replace('/', '.');
		//System.out.println(className);
		return Class.forName(className, false, clazzLoader);
	}
	
	public Object newInstance(String className) throws Throwable{
		Class<?> clazz = this.getClass(className);
		return this.newInstance(clazz);
	}
	
	public Object newInstance(Class<?> clazz) throws Throwable{
		Object[] args = this.lstValues.toArray();
		Constructor<?> ctor = null;
		if(args.length == 0)
			ctor = clazz.getConstructor();
		else{
			Class<?>[] clsAry = this.lstClasses.toArray(new Class<?>[0]);
			ctor = clazz.getConstructor(clsAry);
		}
		ctor.setAccessible(true);
		Object o = ctor.newInstance(args);
		ctor.setAccessible(false);
		return o;
	}
		
	
	public Object invokeObjectMethod(Object obj, String methodName) throws Throwable{
		Object[] args = this.lstValues.toArray();
		Class<?>[] clsAry = this.lstClasses.toArray(new Class<?>[0]);	
		
//		if(methodName == "setStrAry"){
//			for(Class cc : clsAry){
//				System.out.println("setStrAry:" + cc.getName());
//			}
//		}
		
		//obj传入为class,说明是静态方法
		Class<?> clazz = (obj instanceof Class) ? (Class<?>)obj : null;		
		Method method = null;
		try{		
			method = clazz == null ? 
						obj.getClass().getMethod(methodName, clsAry) : 
						clazz.getMethod(methodName, clsAry);
			//System.out.println("method: is global ref.");
		}
		catch(Exception ex){
			if(ex.getMessage() == null){
				Throwable ta = ex.getCause();
				if(ta != null)throw ta;
			}
			throw ex;
			
//			for(Method m : obj.getClass().getDeclaredMethods()){
//				System.out.println(m.getName());
//				if(m.getName().compareTo(methodName)==0){
//					method = m;
//					break;
//				}
//			}
//			
//			System.out.println("method:" + method.getName());
//			System.out.println("args:" + args.length);
//			System.out.println("class:" + clsAry.length);
		}
		
		boolean isStatic = clazz != null;
		Object r = null;
		try
		{
			method.setAccessible(true);	
			//静态方法判断,在上面执行(判断传入参数obj)
			//boolean isStatic = Modifier.isStatic(method.getModifiers());
			r = method.invoke(isStatic ? null : obj, args);
		}
		catch(Exception ex)
		{
			if(ex.getMessage() == null)
			{
				Throwable ta = ex.getCause();
				if(ta != null)throw ta;
			}
			throw ex;			
		}
		return r;
	}
	
	
	public Object invokeObjectField(Object obj, String fieldName, boolean isSetValue) throws Throwable{
		//obj传入为class,说明是静态方法
		Class<?> clazz = (obj instanceof Class) ? (Class<?>)obj : null;		
		boolean isStatic = clazz != null;
		Field field = null;
		try{		
			field = clazz == null ? 
						obj.getClass().getField(fieldName) : 
						clazz.getField(fieldName);
			//System.out.println("field: is global ref." + field.getName());
		}
		catch(Exception ex){
			if(ex.getMessage() == null){
				Throwable ta = ex.getCause();
				if(ta != null)throw ta;
			}
			throw ex;
		}

		field.setAccessible(true);
		if(isSetValue){
			Object[] args = this.lstValues.toArray();	
//			System.out.println("method: is global ref.");
//			System.out.println(args[0]);
			field.set(isStatic ? null : obj, args[0]);
			return null;
		}		
		return field.get(isStatic ? null : obj);
	}
		
	
	public Class<?> getClassByValue(Object obj){
		if(obj == null)return null;
		return obj.getClass();
	}
	
	public Object invokeObjectMethodValue(Object obj,String methodName,Class<?>[] classes,Object[] Values) throws Throwable{

		Class<?> clazz = (obj instanceof Class) ? (Class<?>)obj : null;		
		boolean isStatic = clazz != null;
		
		Method method = null;
		try{		
			method = clazz == null ? 
						obj.getClass().getMethod(methodName, classes) : 
						clazz.getMethod(methodName, classes);
		}
		catch(Exception ex){
			if(ex.getMessage() == null){
				Throwable ta = ex.getCause();
				if(ta != null)throw ta;
			}
			throw ex;
		}
		
		try
		{
			method.setAccessible(true);	
			return method.invoke(isStatic ? null : obj, Values);
		}
		catch(Exception ex)
		{
			if(ex.getMessage() == null)
			{
				Throwable ta = ex.getCause();
				if(ta != null)throw ta;
			}
			throw ex;			
		}
	}
	
	public String getInstanceToString(Object obj){
		return obj.toString();
	}
	
	public int getHashCode(Object obj){
		return obj.hashCode();
	}
}
