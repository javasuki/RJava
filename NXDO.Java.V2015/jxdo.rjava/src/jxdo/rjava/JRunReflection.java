package jxdo.rjava;

import java.lang.reflect.Constructor;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.lang.reflect.Modifier;
import java.lang.reflect.ParameterizedType;
import java.lang.reflect.Type;
import java.lang.reflect.TypeVariable;
import java.util.ArrayList;
import java.util.List;
import jxdo.rjava.JParamInfo;

class JRunReflection {
	
	public static boolean[] getClassFlag(Class<?> clazz){
		boolean[] bAry = new boolean[] { false,false,false,false,false,false,false, false };
		if(clazz == null) return bAry;
		int mod = clazz.getModifiers();
		bAry[0] = Modifier.isAbstract(mod);
		bAry[1] = clazz.isArray();
		bAry[2] = clazz.isEnum();
		bAry[3] = clazz.isInterface();
		bAry[4] = Modifier.isPublic(mod);
		bAry[5] = Modifier.isFinal(mod);
		bAry[6] = clazz.isPrimitive();
		bAry[7] = clazz.getTypeParameters().length > 0;
		return bAry;
	}
	
	public static Class<?> getSuperClass(Class<?> clazz){
		return clazz.getSuperclass();
	}
	
	public static Class<?> getDeclaringClass(Class<?> clazz){
		return clazz.getDeclaringClass();
	}
	
	public static Class<?> getElementClass(Class<?> clazz){
		return clazz.isArray() ? clazz.getComponentType() : null;
	}
	
	public static Class<?>[] getInterfaces(Class<?> clazz){
		return clazz.getInterfaces();
	}
	
	public static boolean getIsAssignableFrom(Class<?> clazz, Class<?> clazz2){
		return clazz.isAssignableFrom(clazz2);
	}
	
	public static boolean getIsInstance(Class<?> clazz, Object obj){
		return clazz.isInstance(obj);
	}
	
	public static Class<?> getAsSubClass(Class<?> clazz, Class<?> parentClazz){
		return clazz.asSubclass(parentClazz);
	}
	
	public static Object getAsCast(Class<?> clazz, Object obj){
		return clazz.cast(obj);
	}
	
	public static String getClassName(Class<?> clazz){
		if(clazz == null)return "";
		return clazz.getName();
	}
	
	public static JParamInfo[] getGenericArguments(Class<?> clazz){
		TypeVariable<?>[] typeVars = clazz.getTypeParameters();
		if(typeVars.length == 0)return null;
		
		List<JParamInfo> lst = new ArrayList<JParamInfo>();
		for(TypeVariable<?> typeVar : typeVars){
			String pname = typeVar.getName();
			Type[] tts = typeVar.getBounds(); //泛型方法 的参数的限定
			//System.out.println(tts.length);
			
			JParamInfo jpinfo = new JParamInfo();
			jpinfo.IsGeneric = true;
			jpinfo.setGenericParamName(pname);
			jpinfo.setTypeClass((Class<?>)tts[0]);
			lst.add(jpinfo);
		}
		
		return lst.toArray(new JParamInfo[0]);
	}
	
	public static Constructor<?> getConstructor(Class<?> clazz, Class<?>[] parameterTypes) throws Throwable{
		return clazz.getConstructor(parameterTypes);
	}
	
	public static Constructor<?>[] getConstructors(Class<?> clazz) throws Throwable{
		return clazz.getConstructors();
	}
	
	public static Class<?> getDeclaringClass(Constructor<?> ctor){
		return ctor.getDeclaringClass();
	}	
	

	
	public static String getConstructorName(Constructor<?> ctor){
		if(ctor == null)return "";
		return ctor.getName();
	}
	
	public static boolean[] getCtorModifier(Constructor<?> ctor){
		boolean[] bAry = new boolean[] { false,false,false,false,false };
		if(ctor == null) return bAry;
		int mod = ctor.getModifiers();
		bAry[0] = Modifier.isAbstract(mod); //IsAbstract
		bAry[1] = Modifier.isFinal(mod); //IsFinal		
		bAry[2] = Modifier.isPrivate(mod); //IsPrivate
		bAry[3] = Modifier.isPublic(mod); //IsPublic
		bAry[4] = Modifier.isStatic(mod); //IsStatic
		return bAry;
	}
	
	public static Object invokeConstructor(Constructor<?> ctor, Object[] args) throws Throwable{
		ctor.setAccessible(true);
		return ctor.newInstance(args);
	}
	
	public static Method getMethod(Class<?> clazz, String methodName, Class<?>[] parameterTypes) throws Throwable{
//		System.out.println(clazz.getName());
//		System.out.println(methodName);
//		if(parameterTypes.length>0)
//			System.out.println(parameterTypes[0].getName());
		return clazz.getMethod(methodName, parameterTypes);
	}
	
	public static Method[] getMethods(Class<?> clazz) throws Throwable{
		return clazz.getMethods();
	}
	
	public static Class<?> getDeclaringClass(Method method){
		return method.getDeclaringClass();
	}	
	
	public static boolean[] getAccessModifier(Method method){
		boolean[] bAry = new boolean[] { false,false,false,false,false };
		if(method == null) return bAry;
		int mod = method.getModifiers();
		bAry[0] = Modifier.isAbstract(mod); //IsAbstract
		bAry[1] = Modifier.isFinal(mod); //IsFinal
		bAry[2] = Modifier.isPrivate(mod); //IsPrivate
		bAry[3] = Modifier.isPublic(mod); //IsPublic
		bAry[4] = Modifier.isStatic(mod); //IsStatic
		return bAry;
	}
	
	
	@SuppressWarnings("unused")
	public static JParamInfo[] getMethodParams(Method method){
		//java.lang.reflect.TypeVariable<GenericDeclaration>
		List<JParamInfo> lst = new ArrayList<JParamInfo>();
		 
		for(Type t : method.getGenericParameterTypes()){
			JParamInfo jpinfo = new JParamInfo();
			
			if(t instanceof ParameterizedType){
				//参数为 泛型class
				ParameterizedType pType = (ParameterizedType)t;
				
				//获取原始类型  
				Type rType = pType.getRawType();  
	            //System.out.println("原始类型是："+rType);  
	            //取得泛型类型参数  
	            Type[] tArgs = pType.getActualTypeArguments();  
	            //System.out.println("泛型类型是：");  
	            for (int i = 0; i < tArgs.length; i++) {  
	                //System.out.println("第"+i+"个泛型类型是："+tArgs[i]);  
	            }
	            
//	            jpinfo.IsGenericType = true;
//	            jpinfo.setGenericParamName(typeName)
	            
			}
			else if(t instanceof java.lang.reflect.TypeVariable<?>){
				TypeVariable<?> typeVar = (TypeVariable<?>)t; //泛型方法上定义的 泛型参数
				String pname = typeVar.getName();
				Type[] tts = typeVar.getBounds(); //泛型方法 的参数的限定
				//System.out.println(tts.length);
				
				jpinfo.IsGeneric = true;
				jpinfo.setGenericParamName(pname);
				jpinfo.setTypeClass((Class<?>)tts[0]);
			}
			else if(t instanceof Class<?>){
				//普通参数
				jpinfo.setTypeClass((Class<?>)t);
				
				Class<?> cls = (Class<?>)t;				
				//System.out.println(cls.getName());
				//System.out.println(cls.getCanonicalName());
			}
			lst.add(jpinfo);
		}
		
		JParamInfo[] jpinfos = lst.toArray(new JParamInfo[0]);
		return jpinfos;
	}
	
	
	public static void main(String[] args) throws Throwable{		
//		Class<?> cls = Object.class;
//		TypeVariable<?>[] typeVars = cls.getTypeParameters();
//		for(TypeVariable<?> typeVar : typeVars){
//			String pname = typeVar.getName();
//			System.out.println(pname);
//		}
//		
//		Method m = java.util.ArrayList.class.getMethod("add", Object.class);
//		JRunReflection.getMethodParams(m);
//		
	}
		
	public static String getMethodName(Method method){
		if(method == null)return "";
		return method.getName();
	}
	
	public static Object invokeMethod(Method method, Object obj, Object[] args) throws Throwable{
		method.setAccessible(true);
		return method.invoke(obj, args);
	}
	
	public static boolean checkReturnIsArray(Object obj){
		if(obj == null)return false;
		return obj.getClass().isArray();
	}

	public static Field getField(Class<?> clazz, String name) throws Throwable {
		return clazz.getField(name);
	}
	
	public static Field[] getFields(Class<?> clazz) throws Throwable {
		return clazz.getFields();
	}
	
	public static String getFieldName(Field field){
		return field.getName();
	}
	
	public static Class<?> getDeclaringClass(Field field){
		return field.getDeclaringClass();
	}
	
	public static boolean[] getFieldModifier(Field field){
		boolean[] bAry = new boolean[] { false,false,false,false };
		if(field == null) return bAry;
		int mod = field.getModifiers();
		bAry[0] = Modifier.isFinal(mod); //IsFinal
		bAry[1] = Modifier.isPrivate(mod); //IsPrivate
		bAry[2] = Modifier.isPublic(mod); //IsPublic
		bAry[3] = Modifier.isStatic(mod); //IsStatic
		return bAry;
	}
	
	public static Object getFieldValue(Field field, Object obj) throws Throwable{
		field.setAccessible(true);
		return field.get(obj);
	}
	
	public static void setFieldValue(Field field, Object obj, Object value) throws Throwable{
		field.setAccessible(true);
		field.set(obj, value);
	}
}
