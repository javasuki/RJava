package jxdo.rjava;

import java.lang.reflect.Constructor;
import java.lang.reflect.Field;
import java.lang.reflect.GenericArrayType;
import java.lang.reflect.Member;
import java.lang.reflect.Method;
import java.lang.reflect.Modifier;
import java.lang.reflect.ParameterizedType;
import java.lang.reflect.Type;
import java.lang.reflect.TypeVariable;
import java.util.ArrayList;
import java.util.List;

class JCSharpHelper {
	public static String getFirstUpperSplit(String name){
		if(name == null)return "";
		if(name.length()==0)return "";
		
		String fMethodName = "";
		boolean bToUpperCase = true;		
		for(char c : name.toCharArray()){
			if(bToUpperCase){
				fMethodName += Character.toString(Character.toUpperCase(c));
				bToUpperCase = false;
			}
			else{
				fMethodName += Character.toString(c);
				if(c == '.') bToUpperCase = true;
			}
		}
		
		return fMethodName;
	}
	
	public static String getNameSpace(Class<?> clazz){
		String pkName = clazz.getPackage() == null ? "" : clazz.getPackage().getName();
		return getFirstUpperSplit(pkName);
	}
	
	public static String getDefineName(Class<?> clazz, boolean ... isSimpleName){
		boolean biIsSimpleName = true;
		if(isSimpleName.length > 0)
			biIsSimpleName = isSimpleName[0];
		return getFirstUpperSplit(biIsSimpleName ? clazz.getSimpleName() : clazz.getName());
	}
	
	public static String getClassForTypeof(Class<?> clazz){
		//用于静态字段中，使用 typeof(...)
		String sGenSign = JCSharpHelper.getClassGenericDefine(clazz);
		if(sGenSign.length() > 0){
			if(sGenSign.indexOf(",")>-1){
				int ilen = sGenSign.split(",").length - 1;
				sGenSign = "<";
				for(int i=0;i<ilen;i++)
					sGenSign += ",";
				sGenSign += ">";
			}
			else
				sGenSign = "<>";
		}
		String className = JCSharpHelper.getDefineName(clazz) + sGenSign;
		return className;
	}
	
	public static String getClassAccessFlag(Class<?> clazz){
		//java class,仅支持  public, abstract, final
		
		StringBuilder sb = new StringBuilder();
		sb.append("public ");
		int iModifier = clazz.getModifiers();
		if(Modifier.isFinal(iModifier))
			sb.append("sealed ");
		if(Modifier.isAbstract(iModifier))
			sb.append("abstract ");
//		if(Modifier.isStatic(iModifier))
//			sb.append("static ");		
		return sb.toString();
	}
	
	public static String getMethodAccessFlag(Method m){
		boolean isSealdClass = Modifier.isFinal(m.getDeclaringClass().getModifiers());
		
		int iModifier = m.getModifiers();
		String nAccessFlag = Modifier.toString(iModifier) + " ";
		if(!Modifier.isFinal(iModifier)){ //可重写的方法
			if(!Modifier.isAbstract(iModifier)){
				if(!getIsStatic(m)) //dotnet中 static方法不能为虚
				{
					if(!getIsOverride(m))
						nAccessFlag = nAccessFlag.replaceFirst(" ", " virtual ");
					else
						nAccessFlag = nAccessFlag.replaceFirst(" ", " override ");
				}
			}
		}
		if(isSealdClass)
			nAccessFlag = nAccessFlag.replaceAll("virtual ", "");
		
		nAccessFlag = nAccessFlag.replaceAll("final ", "").replaceAll("transient ", "");
		return nAccessFlag;
	}
	
	public static String getFieldAccessFlag(Field f){
		StringBuilder sb = new StringBuilder();
		sb.append("public ");
		int iModifier = f.getModifiers();
//		if(Modifier.isFinal(iModifier))
//			sb.append("readonly ");
//		if(Modifier.isAbstract(iModifier))
//			sb.append("abstract ");
		if(Modifier.isStatic(iModifier))
			sb.append("static ");
		
		return sb.toString();
	}
	
	public static boolean getIsFinal(Field f){
		int iModifier = f.getModifiers();
		return Modifier.isFinal(iModifier);
	}
	
	public static String getClassParents(Class<?> clazz){
		Type gt = clazz.getGenericSuperclass();
		String n = getTypeName(gt, clazz);
		
		Class<?> baseClass = clazz.getSuperclass();
		boolean isRootObject = baseClass == Object.class;
		String jRootObjectName = isRootObject ? "JObject" : getFirstUpperSplit(n);
		
		Type[] clsAry = clazz.getGenericInterfaces();
		for(Type t : clsAry){
			String iName = getTypeName(t, clazz, false);
			
			//如果接口为内嵌接口,则使用C#生成对应的接口名,并合并它原来的包名.
			String interfaceName = iName;
			if(interfaceName.lastIndexOf("$") > 0){
				interfaceName = interfaceName.substring(interfaceName.lastIndexOf("$") + 1);
				if(iName.lastIndexOf(".") > 0){
					iName = iName.substring(0, iName.lastIndexOf("."));
					interfaceName = iName + "." + interfaceName; 
				}
			}
			jRootObjectName += (jRootObjectName.length() == 0 ? "" : ", ") + interfaceName;
		}
		return jRootObjectName;
	}
	
	public static String getClassGenericDefine(Class<?> clazz){
		
		StringBuilder sb = new StringBuilder();
		Type[] tAry = clazz.getTypeParameters();
		for(Type t : tAry){
			String n = getTypeName(t, clazz);
			n = sb.length() == 0 ? n : " ," + n;
			sb.append(n );
		}
		
		return sb.length() == 0 ? "" :
				"<"+ sb.toString() +">";
	}

	public static List<JCSharpProperty> getPropertys(Class<?> clazz){
		List<Method> lstMethods = new ArrayList<Method>();
		Method[] methods = clazz.getDeclaredMethods();		
		for(int i=0; i < methods.length; i++){
			Method m = methods[i];
			if(!Modifier.isPublic(m.getModifiers())) continue;
			if(m.getName().equalsIgnoreCase("getClass") || m.getName().equalsIgnoreCase("setClass"))
				continue;
			if(m.isVarArgs())continue;
			
			//方法有泛型参数,类本身是泛型,则此泛型方法的泛型参数 与类本身是不一致的,因为泛型类中相同泛型参数 的方法已被擦除
			String methodGem = getMethodGenericName(m);
			if(methodGem.length() > 0)continue;	
			if(m.getName().length() <= 4)continue;
			
			if(m.getName().toLowerCase().startsWith("get") && m.getParameterTypes().length == 0 && m.getReturnType() != void.class)
				lstMethods.add(m);
			if(m.getName().toLowerCase().startsWith("set") && m.getParameterTypes().length == 1 && m.getReturnType() == void.class)
				lstMethods.add(m);
		}
		
		List<JCSharpProperty> lst = new ArrayList<JCSharpProperty>();
		if(lstMethods.size() == 0)
			return lst;
		
		
		
		for(int i=0; i<lstMethods.size();i++){
			JCSharpProperty prop = new JCSharpProperty();
			
			Method m1 = lstMethods.get(i);	
			
			String m1Name = m1.getName().substring(3);
			
			Type t1 = null;
			if(m1.getName().toLowerCase().startsWith("get")){
				t1 = m1.getGenericReturnType();
				prop.get = m1;
			}
			else if(m1.getName().toLowerCase().startsWith("set")){
				t1 = m1.getGenericParameterTypes()[0];
				prop.set = m1;
			}
			
			lstMethods.remove(m1);
			i -= 1;
			
			
			for(int j=0; j<lstMethods.size(); j++ ){
				Method m2 = lstMethods.get(j);	
				if(m1 == m2)continue;
				
				String m2Name = m2.getName().substring(3);
				if(!m1Name.equalsIgnoreCase(m2Name))continue;
				if(m1.getModifiers() != m2.getModifiers()){
					//除去 get,set后,对应属性名称相同,防止生成两个不同签名的同名属性,则排除这两个方法.
					prop = null;
					continue;
				}

				Type t2= null;
				boolean isGet = false;
				if(m2.getName().toLowerCase().startsWith("get")){
					t2 = m2.getGenericReturnType();
					isGet = true;
				}
				else if(m2.getName().toLowerCase().startsWith("set"))
					t2 = m2.getGenericParameterTypes()[0];
				
				if(t1 != t2)continue;
				
				if(isGet)
					prop.get = m2;
				else
					prop.set = m2;
				
				lstMethods.remove(m2);
				break;
			}	
			
			if(prop != null)
				lst.add(prop);
		}
		
		for(JCSharpProperty prop : lst){
			System.out.println(prop);
		}
		
		return lst;
	}
	
	public static List<Method> getMethods(Class<?> clazz, List<Method> lstProps){
		if(lstProps == null)
			lstProps = new ArrayList<Method>();
		
		List<Method> lst = new ArrayList<Method>();
		Method[] methods = clazz.getDeclaredMethods();
		for(Method m : methods){
			int im = m.getModifiers();
			if(!Modifier.isPublic(im))continue;			
			if(lstProps.contains(m))continue;
			
			lst.add(m);
		}
		return lst;
	}	
	
	public static List<Constructor<?>> getConstructors(Class<?> clazz){
		List<Constructor<?>> lst = new ArrayList<Constructor<?>>();
		Constructor<?>[] cotrs = clazz.getDeclaredConstructors();
		for(Constructor<?> c : cotrs){
			int im = c.getModifiers();
			if(!Modifier.isPublic(im))continue;
			lst.add(c);
		}
		return lst;
	}	
	
	public static List<Field> getFields(Class<?> clazz){
		List<Field> lst = new ArrayList<Field>();
		Field[] flds = clazz.getDeclaredFields();
		for(Field f : flds){
			int im = f.getModifiers();
			if(!Modifier.isPublic(im))continue;
			lst.add(f);
		}
		return lst;
	}
	
	
	
	public static boolean getIsReturn(Method m){
		return getReturnName(m) != "void";
	}
	
	public static boolean getIsStatic(Member m){
		int im = m.getModifiers();
		return Modifier.isStatic(im);
	}
	
//	public static boolean getIsStatic(Field f){
//		int im = f.getModifiers();
//		return Modifier.isStatic(im);
//	}
	
	public static boolean getIsAbstract(Method m){
		int im = m.getModifiers();
		return Modifier.isAbstract(im);
	}
	
	
	
	private static boolean getIsOverride(Method m) {
		Class<?> clazz = m.getDeclaringClass().getSuperclass();
		if(clazz == null)return false;
		if(clazz == Object.class)
			return false;
		
		try {
			clazz.getMethod(m.getName(), m.getParameterTypes());
		} catch (NoSuchMethodException e) {
			return false;
		}
		
		return true;
	}
	
	
	
	public static boolean getIsJMethodAttribute(Method m){
		char c = m.getName().toCharArray()[0];
		return c == Character.toUpperCase(c);
	}
	
	public static String getReturnName(Member m){
		
		Type t = null;
		if(m instanceof Method)
			t = ((Method)m).getGenericReturnType();
		else if(m instanceof Field)
			t = ((Field)m).getGenericType();
		
		Class<?> defClazz = m.getDeclaringClass();
		return getTypeName(t, defClazz);
	}
	
//	public static String getReturnName(Field f){
//		Class<?> defClazz = f.getDeclaringClass();
//		return getTypeName(f.getGenericType(), defClazz);
//	}
	
	public static String getMethodCacheKey(Method m){
		Class<?> defClazz = m.getDeclaringClass();

		String genSign = m.toGenericString();
		int pos = genSign.lastIndexOf(")");
		genSign = genSign.substring(0, pos+1);
		pos = genSign.indexOf("(");
		genSign = genSign.substring(pos);
		
		//s = s.substring(s.indexOf("."));		
		genSign = genSign.replaceAll("java.lang.", "")
					.replaceAll("java.util.", "")
					.replaceAll("java.math.", "");
		
		String pkName = defClazz.getPackage() == null ? "" : defClazz.getPackage().getName();
		if(genSign.indexOf(pkName + ".") > -1)
			genSign = genSign.replaceAll(pkName + ".", "");
		return m.getName() + genSign;
	}
	
	public static String getParamsName(Member m){
		
		boolean isVarArgs = false;
		Type[] types = null;
		if(m instanceof Constructor<?>){
			isVarArgs = ((Constructor<?>)m).isVarArgs();
			types = ((Constructor<?>)m).getGenericParameterTypes();
		}
		else if(m instanceof Method){
			isVarArgs = ((Method)m).isVarArgs();		
			types = ((Method)m).getGenericParameterTypes();
		}
		
		Class<?> defClazz = m.getDeclaringClass();
		StringBuilder sb = new StringBuilder();
		for(int i=0;i<types.length; i++){
			Type t  = types[i];
			String ptName = getTypeName(t, defClazz);
			if(isVarArgs && i == types.length-1)
				ptName = "params " + (ptName.endsWith("[]") ? ptName : ptName + "[]");
			String split = sb.length() == 0 ? "" : ", ";
			sb.append(split + ptName + " arg" + i);
		}
		
		return sb.toString();
	}
	
//	public static String getParamsName(Method m){
//
//		Class<?> defClazz = m.getDeclaringClass();
//		StringBuilder sb = new StringBuilder();
//		Type[] types = m.getGenericParameterTypes();
//		for(int i=0;i<types.length; i++){
//			Type t  = types[i];
//			String ptName = getTypeName(t, defClazz);
//			if(m.isVarArgs() && i == types.length-1)
//				ptName = "params " + (ptName.endsWith("[]") ? ptName : ptName + "[]");
//			String split = sb.length() == 0 ? "" : ", ";
//			sb.append(split + ptName + " arg" + i);
//		}
//		
//		return sb.toString();
//	}
	
	public static String getMethodGenericName(Method m){			
		String s = m.toGenericString();
		
		//void methodName(ArrayList<...>)
		s = s.substring(0, s.indexOf(m.getName())); //获取方法名之前的字符，如果其中包含T<>,则为泛型参数定义
		if(s.indexOf("<") == -1 && s.indexOf(">") == -1)
			return "";
		
		s = s.substring(s.indexOf("<") + 1);
		s = s.substring(0, s.indexOf(">"));
		String[] sAry = null;
		if(s.indexOf(",")>-1)
			sAry = s.split(",");
		else
			sAry = new String[]{ s };
		StringBuilder sb = new StringBuilder();
		for(String sg : sAry){
			String[] sTmp = new String[]{sg};
			if(sg.indexOf("extends") > -1)
				sTmp = sg.split("extends");
			
			for(String s2 : sTmp){
				if(s2.length() == 0)continue;
				String s3 = s2.replaceAll(" ", "");
				sb.append(sb.length()==0 ? s3 : ", " + s3);
			}
		}
		s = sb.toString();		
		return "<"+ s +">";
	}
	
	public static String getParamsSet(Member m){
		int iSize = 0;
		if(m instanceof Constructor<?>)
			iSize = ((Constructor<?>)m).getGenericParameterTypes().length;
		else if(m instanceof Method)		
			iSize = ((Method)m).getGenericParameterTypes().length;
		
		
		StringBuilder sb = new StringBuilder();
		for(int i =0; i< iSize; i++){
			String split = sb.length() == 0 ? "" : ", ";
			sb.append(split + "arg" + i);
		}
		return sb.toString();
	}
	
//	public static String getParamsSet(Constructor<?> c){
//		StringBuilder sb = new StringBuilder();
//		for(int i =0; i<c.getGenericParameterTypes().length; i++){
//			String split = sb.length() == 0 ? "" : ", ";
//			sb.append(split + "arg" + i);
//		}
//		return sb.toString();
//	}
	
	public static String getTypeName(Type t, Class<?> defClazz, boolean ... isReplaceDoller){
		if(t instanceof ParameterizedType){
			//参数为 泛型class
			ParameterizedType pType = (ParameterizedType)t;

			StringBuilder sb = new StringBuilder();
			Type[] ts = pType.getActualTypeArguments();
			for(Type tt : ts){
				String s1 = getTypeName(tt, defClazz, isReplaceDoller);
				s1 = sb.length() == 0 ? s1 : ", " + s1;
				sb.append(s1);
			}
			
			Type tt2 = pType.getRawType(); //获取原始类型  
			String s = getTypeName(tt2, defClazz, isReplaceDoller);
			
			return sb.length() == 0 ? s : s + "<"+ sb.toString() +">";
		}
		else if(t instanceof java.lang.reflect.TypeVariable<?>){
			TypeVariable<?> typeVar = (TypeVariable<?>)t; //泛型方法上定义的 泛型参数
			String pname = typeVar.getName();
			//Type[] tts = typeVar.getBounds(); //泛型方法 的参数的限定
			return pname;
		}
		else if(t instanceof java.lang.reflect.GenericArrayType){
			GenericArrayType at = (GenericArrayType)t;
			String s = getTypeName(at.getGenericComponentType(), defClazz, isReplaceDoller);
			return s.endsWith("[]") ? s : s + "[]";
		}
		else if(t instanceof Class<?>){
			//普通参数			
			Class<?> cls = (Class<?>)t;		
			String pkName = defClazz.getPackage() == null ? "" : defClazz.getPackage().getName();
			return getCsharpTypeName(cls, pkName, isReplaceDoller);
		}
		
		return "";
	}
	
	private static String getCsharpTypeName(Class<?> clazz, String packageName,boolean ... isReplaceDoller){
		boolean isRepDoller = true;
		if(isReplaceDoller.length > 0)
			isRepDoller = isReplaceDoller[0];
		
		if(clazz == void.class)
			return "void";
		
		//简单类型
		if(clazz == boolean.class)
			return "bool";
		else if(clazz == byte.class)
			return "byte";
		else if(clazz == char.class)
			return "char";
		else if(clazz == short.class)
			return "short"; //Int16
		else if(clazz == int.class)
			return "int"; //Int32
		else if(clazz == long.class)
			return "long"; //Int64
		else if(clazz == float.class)
			return "float"; 
		else if(clazz == double.class)
			return "double"; 
		
		//简单类型对应引用类型
		else if(clazz == Boolean.class)
			return "bool?";
		else if(clazz == Byte.class)
			return "byte?";
		else if(clazz == Character.class)
			return "char?";
		else if(clazz == Short.class)
			return "short?"; //Int16
		else if(clazz == Integer.class)
			return "int?"; //Int32
		else if(clazz == Long.class)
			return "long?"; //Int64
		else if(clazz == Float.class)
			return "float?"; 
		else if(clazz == Double.class)
			return "double?"; 
		else if(clazz == java.math.BigDecimal.class)
			return "decimal?";
		else if(clazz == java.util.Date.class)
			return "DateTime?";
		
		//简单类型 数组
		else if(clazz == boolean[].class)
			return "bool[]";
		else if(clazz == byte[].class)
			return "byte[]";
		else if(clazz == char[].class)
			return "char[]";
		else if(clazz == short[].class)
			return "short[]"; //Int16
		else if(clazz == int[].class)
			return "int[]"; //Int32
		else if(clazz == long[].class)
			return "long[]"; //Int64
		else if(clazz == float[].class)
			return "float[]"; 
		else if(clazz == double[].class)
			return "double[]"; 		
		
		
		//简单类型数组对应引用类型数组
		else if(clazz == Boolean[].class)
			return "bool?[]";
		else if(clazz == Byte[].class)
			return "byte?[]";
		else if(clazz == Character[].class)
			return "char?[]";
		else if(clazz == Short[].class)
			return "short?[]"; //Int16
		else if(clazz == Integer[].class)
			return "int?[]"; //Int32
		else if(clazz == Long[].class)
			return "long?[]"; //Int64
		else if(clazz == Float[].class)
			return "float?[]"; 
		else if(clazz == Double[].class)
			return "double?[]"; 
		else if(clazz == java.math.BigDecimal[].class)
			return "decimal?[]";
		else if(clazz == java.util.Date[].class)
			return "DateTime?[]";
		
		//string, string[]
		else if(clazz == String.class)
			return "string"; 
		else if(clazz == String[].class)
			return "string[]"; 
		
		else if(clazz == Class.class)
			return "JClass"; 
		else if(clazz == Class[].class)
			return "JClass[]"; 
		
		
		//C#端的预定义
		else if(clazz == java.util.List.class)
			return "JList";
		else if(clazz == java.util.ArrayList.class)
			return "[JParameter(\"java.util.ArrayList\")]JList";	
		
		else if(clazz == java.util.Map.class)
			return "JDictionary";
		else if(clazz == java.util.HashMap.class)
			return "[JParameter(\"java.util.HashMap\")]JDictionary";
		
		else if(clazz == java.util.Queue.class)
			return "JQueue";
		else if(clazz == java.util.LinkedList.class)
			return "[JParameter(\"java.util.LinkedList\")]JQueue";
		
		else if(clazz == java.util.Stack.class)
			return "JStack";
		
		else if(clazz == java.lang.reflect.Field.class)
			return "JField";
		else if(clazz == java.lang.reflect.Method.class)
			return "JMethod";
		else if(clazz == java.lang.reflect.Constructor.class)
			return "JConstructor";

		
		String currName = clazz.getName();
		if(currName.indexOf("java.lang.Object") > -1)
			currName = currName.replace("java.lang.", "J");
//		if(currName.indexOf(packageName + ".") > -1)
//			currName = currName.replace(packageName + ".", "");
		
		if(isRepDoller && currName.indexOf("$") > -1)
			currName = currName.replaceAll("\\$", ".");
			
		if(!clazz.isArray())
			return getFirstUpperSplit(currName) ;
		else{
			String cAryName = currName.replace("[L", "").replace(";", "");
			return getFirstUpperSplit(cAryName) + "[]";
		}

	}
	
}
