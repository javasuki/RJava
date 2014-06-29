package jxdo.rjava;

import java.io.File;
import java.lang.reflect.Constructor;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.net.URL;
import java.net.URLClassLoader;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.Calendar;
import java.util.HashMap;
import java.util.List;


class JRunCore {
	static ClassLoader clazzLoader;
	
	/**
	 * invoke in c++ only first.
	 * @param jarNames
	 * @throws Throwable
	 */
	public JRunCore(String jarNames) throws Throwable {
		keyClasses = new HashMap<String,Class<?>>();
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
	
	static HashMap<String,Class<?>> keyClasses;
	public Class<?> getClass(String className) throws Throwable{
		if(keyClasses.containsKey(className)){
			//System.out.println("c++ param in:" + className);
			return keyClasses.get(className);
		}
		
		Class<?> clazz = null;
		if(className.indexOf(".") < 0){
			if(className.compareToIgnoreCase("Z")==0 || className.compareToIgnoreCase("boolean") == 0)
				clazz = boolean.class;
			else if(className.compareToIgnoreCase("B") == 0 || className.compareToIgnoreCase("byte") == 0)
				clazz = byte.class;
			else if(className.compareToIgnoreCase("C") == 0 || className.compareToIgnoreCase("char") == 0)
				clazz = char.class;
			else if(className.compareToIgnoreCase("S") == 0 || className.compareToIgnoreCase("short") == 0)
				clazz = short.class;
			else if(className.compareToIgnoreCase("I") == 0 || className.compareToIgnoreCase("int") == 0)
				clazz = int.class;
			else if(className.compareToIgnoreCase("J") == 0 || className.compareToIgnoreCase("long") == 0)
				clazz = long.class;
			else if(className.compareToIgnoreCase("F") == 0 || className.compareToIgnoreCase("float") == 0)
				clazz = float.class;
			else if(className.compareToIgnoreCase("D") == 0 || className.compareToIgnoreCase("double") == 0)
				clazz = double.class;
			
			else if(className.compareToIgnoreCase("[Z")==0 || className.compareToIgnoreCase("boolean[]") == 0)
				clazz = boolean[].class;
			else if(className.compareToIgnoreCase("[B") == 0 || className.compareToIgnoreCase("byte[]") == 0)
				clazz = byte[].class;
			else if(className.compareToIgnoreCase("[C") == 0 || className.compareToIgnoreCase("char[]") == 0)
				clazz = char[].class;
			else if(className.compareToIgnoreCase("[S") == 0 || className.compareToIgnoreCase("short[]") == 0)
				clazz = short[].class;
			else if(className.compareToIgnoreCase("[I") == 0 || className.compareToIgnoreCase("int[]") == 0)
				clazz = int[].class;
			else if(className.compareToIgnoreCase("[J") == 0 || className.compareToIgnoreCase("long[]") == 0)
				clazz = long[].class;
			else if(className.compareToIgnoreCase("[F") == 0 || className.compareToIgnoreCase("float[]") == 0)
				clazz = float[].class;
			else if(className.compareToIgnoreCase("[D") == 0 || className.compareToIgnoreCase("double[]") == 0)
				clazz = double[].class;		
			else //默认的包名,没有.,也不是系统原生类型
				clazz = Class.forName(className, false, clazzLoader);
		}
		else
			clazz = Class.forName(className, false, clazzLoader);
		
		keyClasses.put(className, clazz);
		return clazz;
	}

	public String getClassName(Class<?> clazz)throws Throwable{
		if(clazz == null)
			throw new Exception("参数clazz为空，无法获取类型名称。").getCause();
		return  clazz.isArray() ? clazz.getComponentType().getName() : clazz.getName();
	}
	
	
	
	public boolean checkIsPrimitive(Class<?> clazz){
		return clazz.isArray() ? clazz.getComponentType().isPrimitive() : clazz.isPrimitive();
		
		//clazz.newInstance()
	}
	

	public Object newInstance(Class<?> clazz, Class<?>[] classes, Object[] Values) throws Throwable{

		Constructor<?> ctor = null;
		if(classes.length == 0)
			ctor = clazz.getConstructor();
		else
			ctor = clazz.getConstructor(classes);

		ctor.setAccessible(true);
		Object o = ctor.newInstance(Values);
		return o;
	}	
	
	public Object invokeObjectMethodValue(Object obj,String methodName,Class<?>[] classes,Object[] Values) throws Throwable{
		//obj传入为class,说明是静态方法
		Class<?> clazz = (obj instanceof Class) ? (Class<?>)obj : null;
		boolean isStatic = clazz != null;

		Method method = null;
		try{	
			//System.out.println(methodName);
			//System.out.println(Values[0].getClass());
			method = clazz == null ? 
						obj.getClass().getMethod(methodName, classes) : 
						clazz.getMethod(methodName, classes);
		}
		catch(Exception ex){
			//System.out.println(ex.getMessage());
			if(ex.getMessage() == null){
				Throwable ta = ex.getCause();
				if(ta != null)throw ta;
			}
			throw ex;
		}
		
		//System.out.println(method.toGenericString());
		
		try
		{
			method.setAccessible(true);	
//			System.out.println(Values.length);
			//System.out.println(classes[0].getName());
			//System.out.println(Values[0]);
			return method.invoke(isStatic ? null : obj, Values);
		}
		catch(Exception ex)
		{
			//System.out.println(ex.getMessage());
			if(ex.getMessage() == null)
			{
				Throwable ta = ex.getCause();
				if(ta != null)throw ta;
			}
			throw ex;			
		}
	}

	public Object invokeObjectFieldValue(Object obj, String fieldName, Object Value, boolean isSetValue) throws Throwable{
		//obj传入为class,说明是静态字段
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
			field.set(isStatic ? null : obj, Value);
			return null;
		}		
		return field.get(isStatic ? null : obj);
	}
	
	public Date createDate(String sDateTime, String sFormat) throws Throwable{
		if(sDateTime == null)return null;		
		SimpleDateFormat dFormat = new SimpleDateFormat(sFormat);
		return dFormat.parse(sDateTime);
	}	
	
	public String getDateString(Object o){
		if(o==null)return null;
		if(o instanceof Date)
			return new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format((Date)o);
		else if(o instanceof Calendar){
			
		}
		
		return null;
	}
	
	public int getEnumOrdinal(Object o){
		Enum<?> e = (Enum<?>)o;
		return e.ordinal();
	}
	
	/**
	 * c# to c++ 实现 (c# object.ToString 方法重写),
	 * 并同时提供给枚举返回字符串值,由上层ENUM转换该字符串. Enum 异常,则调用c++ 调用 getEnumOrdinal
	 * @param obj
	 * @return
	 */
	public String getObjectToString(Object obj){
		return obj.toString();
	}
	
	/**
	 * c# to c++ 实现 (c# object.GetHashCode 方法重写)
	 * @param obj
	 * @return
	 */
	public int getObjectHashCode(Object obj){
		return obj.hashCode();
	}
}
