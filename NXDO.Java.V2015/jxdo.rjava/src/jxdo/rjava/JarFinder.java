package jxdo.rjava;

import java.io.File;
import java.io.FileWriter;
import java.lang.reflect.Constructor;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.lang.reflect.Modifier;
import java.lang.reflect.Type;
import java.lang.reflect.TypeVariable;
import java.net.URL;
import java.net.URLClassLoader;
import java.util.ArrayList;
import java.util.Date;
import java.util.Enumeration;
import java.util.List;
import java.util.jar.JarEntry;
import java.util.jar.JarFile;

class JarFinder {

	static String crlf;
	static ClassLoader clazzLoader;
	List<String> lstClassNames;
	
	public JarFinder(String jarNames) throws Throwable{
		if(crlf == null)crlf= System.getProperty("line.separator");		
		lstClassNames = new ArrayList<String>();
		if(clazzLoader == null)		
			clazzLoader = this.getJarClassLoader(jarNames, this.getClass().getClassLoader());
	}
	
	@SuppressWarnings("unused")
	private void addClassLaoder(String jarNames) throws Throwable {
		lstClassNames.clear();
		ClassLoader newClazzLoader = this.getJarClassLoader(jarNames, clazzLoader);
		if(newClazzLoader != clazzLoader)clazzLoader = newClazzLoader;
	}

	private ClassLoader getJarClassLoader(String jarNames, ClassLoader parentClassLoader) throws Throwable{				
		List<URL> lstUrls = new ArrayList<URL>();
		for(String jarName : jarNames.split(";")){	
			File file  = new File(jarName);
			if(file.getName().compareToIgnoreCase("jxdo.rjava.jar") == 0)continue;
			if(!file.exists())
			{
				java.io.FileNotFoundException exp = new java.io.FileNotFoundException(jarName);
				if(exp.getMessage() == null){
					Throwable ta = exp.getCause();
					if(ta!=null)throw ta;
				}
				throw exp;
			}
			
			this.fillClassNames(jarName);
			
			URL jarUrl = new URL("jar", "","file:" + file.getAbsolutePath()+"!/"); 
			lstUrls.add(jarUrl);
		}
		
		if(lstUrls.size()==0)
			return parentClassLoader;
		
		URL[] urls = lstUrls.toArray(new URL[0]);				
		return URLClassLoader.newInstance(urls, parentClassLoader);
	}

	private void fillClassNames(String jarFileName) throws Throwable{
		JarFile jar = new JarFile(jarFileName);
		Enumeration<JarEntry> entries = jar.entries();
        while (entries.hasMoreElements()) {
            String entry = entries.nextElement().getName();
            if(!entry.endsWith(".class"))continue;
            //System.out.println(entry);
            
            String clsName = entry.replaceAll(".class", "").replaceAll("/", ".");
            lstClassNames.add(clsName);
        }
	}
	
	List<Class<?>> lstClasses;
	public void fillClasses(String directoryName) throws Throwable{
		if(lstClasses == null)
			lstClasses = new ArrayList<Class<?>>();
		else
			lstClasses.clear();
		
		java.io.File f = new File(directoryName);
		if(!f.exists())f.mkdirs();
		
		for(String name : lstClassNames){
			if(name.indexOf("$")>-1)continue; //内部嵌套类
			
			Class<?> clazz = null;
			try {
				clazz = clazzLoader.loadClass(name);
				if(!Modifier.isPublic(clazz.getModifiers()))continue;
			} catch (ClassNotFoundException e) {
				continue;
			}
			lstClasses.add(clazz);
			//System.out.println(clazz.getName());
			this.toCsharpClass(clazz, directoryName);
		}
	}
	
	private void toCsharpEnum(java.io.FileWriter fw, Class<?> clazz) throws Throwable{
		for(Field fld : clazz.getDeclaredFields()){
			if(fld.getName().startsWith("ENUM$"))
				continue;
			
			fw.write("\t\t" + fld.getName() + "," + crlf);			
		}
		toCsharpClassEnd(fw);
	}
	
	private String getFirstUpper(String name){
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
	
	List<PropertyMethod> lstProps;
	List<Method> lstMethods;
	private void toCsharpClass(Class<?> clazz, String directoryName) throws Throwable{
		File f = new File(directoryName + File.separator + clazz.getSimpleName() + ".cs");
		if(f.exists())f.delete();
		boolean isCreated = f.createNewFile();
		if(!isCreated)return;
		
		if(lstProps == null)lstProps = new ArrayList<PropertyMethod>();
		else lstProps.clear();
		if(lstMethods == null)lstMethods = new ArrayList<Method>();
		else lstMethods.clear();
		
		List<Method> lstGets = new ArrayList<Method>();
		List<Method> lstSets = new ArrayList<Method>();
		java.io.FileWriter fw = new FileWriter(f);
		this.toCsharpClassStart(fw, clazz);
		if(clazz.isEnum()){
			this.toCsharpEnum(fw, clazz);
			fw.flush();
			fw.close();	
			return;
		}
		
		for(Method m : clazz.getDeclaredMethods()){
			if(Modifier.isPrivate(m.getModifiers()))continue;
			if(Modifier.isProtected(m.getModifiers()))continue;

			if(this.isGenericMethod(m) || m.isVarArgs()){
				//类还未支持泛型,则不支持生成泛型属性
				//可变参数,使用方法
				lstMethods.add(m);	
				continue;
			}
			
			
			if(m.getName().toLowerCase().startsWith("get") && m.getParameterTypes().length==0 && m.getReturnType() != void.class)
				lstGets.add(m);
			else if(m.getName().toLowerCase().startsWith("set") && m.getParameterTypes().length==1 && m.getReturnType() == void.class)
				lstSets.add(m);
			else 
				lstMethods.add(m);		
		}
		
		//匹配属性的  get/set 方法
		for(Method gm : lstGets){
			PropertyMethod prop = new PropertyMethod();
			prop.setGetMethod(gm);

			Method rmSetMethod = null;
			for(Method sm : lstSets){
				if(sm.getName().toLowerCase().compareToIgnoreCase("set" + prop.getPropertyName()) != 0)continue; //名称匹配
				if(prop.getGetMethod().getReturnType() != sm.getParameterTypes()[0])continue; //类型匹配				
				if(Modifier.isStatic(prop.getGetMethod().getModifiers())  != 
						Modifier.isStatic(sm.getModifiers()))continue;
				
				prop.setSetMethod(sm);
				rmSetMethod = sm;
				break;
			}
			
			if(rmSetMethod != null)lstSets.remove(rmSetMethod);			
			lstProps.add(prop);
		}
		
		//剩下的只写属性
		for(Method sm : lstSets){
			PropertyMethod prop = new PropertyMethod();
			prop.setSetMethod(sm);
			lstProps.add(prop);
		}
		
		//生成变量
		for(Field fld : clazz.getDeclaredFields())
			this.toCsharpFieldProperty(fw, clazz, fld);
		
		//生成属性
		for(PropertyMethod propM : lstProps)
			this.toCsharpProperty(fw, clazz, propM);
		
		//生成方法
		for(Method m : lstMethods)
			this.toCsharpMethod(fw, clazz, m, false, false);
		
		this.toCsharpClassEnd(fw);
		
		fw.flush();
		fw.close();	
	}
	
	static String codeCopyRightCS = "";
	private static String getCodeCopyRightCS(){
		if(codeCopyRightCS.length() ==0){
			codeCopyRightCS = "#region 代码说明" + crlf +
					"/*" + crlf +
					" *******************************************************" + crlf +
		            " *       此段代码由 NXDO.RJacoste.Addin 产生  " + crlf +
					" *******************************************************" + crlf +
					" * File Name:	_fName" + crlf +
					" * Date Time:	" + new Date().toString() + crlf +
					" * Anthor:	jjsiki(ZhuQi) javasuki@hotmail.com" + crlf +
					" * Blog:		http://blog.csdn.net/javasuki" + crlf +
		            " * Generator:	RJacoste(jxdo.rjava.JarFinder) For VS2012" + crlf +
					" * Version:		1.0.0.0" + crlf +
					" * -----------------------------------------------------" + crlf +
					" */" + crlf + 
					"#endregion" + crlf;
		}
		
		return codeCopyRightCS;
	}
	
	private void toCsharpClassStart(java.io.FileWriter fw, Class<?> clazz) throws Throwable{
		if(clazz.isInterface())return;
		
		fw.write("using System;" + crlf);
		fw.write("using System.Collections.Generic;" + crlf);
		fw.write("using System.Diagnostics;" + crlf);
		fw.write("using System.Linq;" + crlf);
		fw.write("using System.Text;" + crlf + crlf);
		fw.write("using NXDO.RJava;" + crlf);
		fw.write("using NXDO.RJava.Attributes;" + crlf + crlf);
		
		boolean isEnum = clazz.isEnum();
		
		fw.write(getCodeCopyRightCS().replaceAll("_fName", clazz.getSimpleName() + ".cs") + crlf);
		boolean isGenClass = false;		
		String sGenTypes = "";
		String sGenTypesSplit = "";
		@SuppressWarnings("unchecked")
		TypeVariable<Class<?>>[] tvs = (TypeVariable<Class<?>>[])((Object)clazz.getTypeParameters());
		if(tvs.length>0){
			isGenClass = true;
			for(TypeVariable<Class<?>> tv : tvs){
				sGenTypes += sGenTypes.length() == 0 ? tv.getName() : ", " + tv.getName();
				sGenTypesSplit += sGenTypesSplit.length() == 0 ? " " : ",";
			}
		}
				
		//cs代码,判断是否需要继承自 JObject
		boolean isRootObject = clazz.getSuperclass() == Object.class;
		String jRootObjectName = isRootObject ? "JObject" : this.getFirstUpper(clazz.getSuperclass().getName());
		String packageName = this.getFirstUpper(clazz.getPackage().getName());
		String nClassName = this.getFirstUpper(clazz.getSimpleName());
		String nCtorMethod = nClassName;
		String nTypeofName = nClassName;
		if(isGenClass){
			nClassName += "<"+ sGenTypes +">";
			nTypeofName = nTypeofName + "<"+ sGenTypesSplit.replaceAll(" ", "") +">";
		}
			
		fw.write("namespace Jacoste." + packageName + crlf);
		fw.write("{" + crlf);
		
		if(!isEnum){
			fw.write("\t[JClass(\""+ clazz.getName() +"\")]" + crlf);
			fw.write("\tpublic class " + nClassName + " : " + jRootObjectName + crlf);
		}
		else{
			fw.write("\t[JEnum(\""+ clazz.getName() +"\")]" + crlf);
			fw.write("\tpublic enum " + nClassName + crlf);
			fw.write("\t{" + crlf);
			return;
		}
		
		
		
		fw.write("\t{" + crlf);
		
		fw.write("\t\t" + "[DebuggerBrowsable(DebuggerBrowsableState.Never)]" + crlf);
		fw.write("\t\t" + "static Type nCacheType = typeof("+ nTypeofName +");" + crlf);
		
		fw.write("\t\t" + "protected " + nCtorMethod + "(IntPtr objectPtr, IntPtr classPtr)" + crlf);		
		fw.write("\t\t\t: base(objectPtr, classPtr)" + crlf);
		fw.write("\t\t{" + crlf);
		fw.write("\t\t}" + crlf + crlf);

		
		for(Constructor<?> ctor : clazz.getDeclaredConstructors()){
			
			int iModifier = ctor.getModifiers();
			if(Modifier.isStatic(iModifier))continue;
			if(!Modifier.isPublic(iModifier))continue;
			
			
			String paramTypeNames = this.getConstructorType(ctor, false);
			String paramInvokeNames = this.getParamInvoke(paramTypeNames);
			
			fw.write("\t\t");
			fw.write("public " + nCtorMethod + "("+ paramTypeNames +")");
			
			if(!isRootObject){
				if(paramInvokeNames.isEmpty())
					fw.write(" : base()" + crlf);
				else{
					
					String baseCtorArgs = "";
					int argLen = paramTypeNames.indexOf(",") >-1 ? paramTypeNames.split(",").length : 1;
					for(int i=0;i<argLen;i++)
						baseCtorArgs += baseCtorArgs.isEmpty() ? "arg" + i : ", arg" + i; 
					fw.write(" : base("+ baseCtorArgs +")" + crlf);
				}				
				fw.write("\t\t");
				fw.write("{" + crlf);
			}
			else{
				fw.write(crlf);
				fw.write("\t\t");
				fw.write("{" + crlf);
				
				fw.write("\t\t\t//");
				fw.write(ctor.toGenericString().replaceAll(clazz.getName() , "ctor") + crlf);
				fw.write("\t\t\t");
				
				if(paramInvokeNames.isEmpty())
					fw.write("JSuper();" + crlf);
				else{
					fw.write(paramInvokeNames + crlf);
					fw.write("\t\t\t");
					fw.write("JSuper(_pAryValues);" + crlf);
				}
			}
			
			fw.write("\t\t");
			fw.write("}" + crlf + crlf);
		}
	}
	
	private void toCsharpClassEnd(java.io.FileWriter fw) throws Throwable{
		fw.write("\t}" + crlf);
		fw.write("}" + crlf);
	}
	
	private void toCsharpMethod(java.io.FileWriter fw, Class<?> clazz, Method m, boolean isProperty, boolean isGet) throws Throwable{

		String genSign = m.toGenericString();
		String sMethodKey = this.getMethodKey(m, clazz);
		boolean isGeneric = isGenericMethod(m);
				
		String nMethod = getFirstUpper(m.getName());		
		if(nMethod.compareToIgnoreCase("main") == 0) {
			//java入口函数一定为小写
			fw.write("\t\t[JMethod(\"main\")]" + crlf);
			nMethod = "J" + getFirstUpper(clazz.getSimpleName()) + nMethod;
		}		
		if(isGeneric)nMethod = nMethod + "<"+ getGenericDefineNames(m) +">";
		
		//方法修饰符		
		int iModifier = m.getModifiers();
		boolean isStatic = Modifier.isStatic(iModifier);
		String nAccessFlag = Modifier.toString(iModifier) + " ";
		if(!Modifier.isFinal(iModifier)){ //可重写的方法
			if(!isStatic) //dotnet中 static方法不能为虚
			{
				if(!isOverride(m))
					nAccessFlag = nAccessFlag.replaceFirst(" ", " virtual ");
				else
					nAccessFlag = nAccessFlag.replaceFirst(" ", " override ");
			}
		}
		nAccessFlag = nAccessFlag.replaceAll("final ", "").replaceAll("transient ", "");
		
		//获取参数列表 (方法定义与调用的参数名称)
		String paramTypeNames = this.getParamType(m, isGeneric);
		String paramInvokeNames = this.getParamInvoke(paramTypeNames);

		//返回值的类型名称
		String rTypeName = this.getReturnType(m.getGenericReturnType(), isGeneric); //最后一个字符为空格
		String rGenericTypeName = ", null";
		boolean isGenericReturn = this.getReturnTypeIsGeneric();
		if(isGenericReturn){
			//泛型返回值,则CS中设置,便于java方法调用后,转换结果.
			if(rTypeName.indexOf("[]") > -1)
				rGenericTypeName = rTypeName.substring(0,rTypeName.length()-3);
			else
				rGenericTypeName = rTypeName.substring(0,rTypeName.length()-1);
			rGenericTypeName = ", typeof("+ rGenericTypeName +")";
		}
		
		String sNInvokeMethodName = ""; //包装器调用的方法名称
		if(rTypeName.compareToIgnoreCase("void ") == 0){
			sNInvokeMethodName = "JObject.JInvokeMethod";
			rGenericTypeName = "";
		}
		else
			sNInvokeMethodName = "return JObject.JInvokeMethod<"+ rTypeName.substring(0,rTypeName.length()-1) +">";
		
		//包装器方法所需的变量
		String sNTypeObject = "this";
		if(isStatic)sNTypeObject = "null";
		
		fw.write("\t\t");
		if(this.isJMethodAttribute(m))
			fw.write("[JMethod(\""+ m.getName() +"\")]" + crlf + "\t\t");	
		fw.write(nAccessFlag + rTypeName + nMethod + "("+ paramTypeNames +")" + crlf);		
		fw.write("\t\t{" + crlf);
		
		//写入注释,  java 方法的签名.
		fw.write("\t\t\t//" + genSign.replaceAll(clazz.getName() + ".", "") + crlf);
		fw.write("\t\t\t");
		
		if(paramInvokeNames.isEmpty())
			fw.write(sNInvokeMethodName + "(nCacheType, \""+ sMethodKey +"\", "+ sNTypeObject + ");" + crlf);
		else{
			fw.write(paramInvokeNames + crlf);
			fw.write("\t\t\t");
			fw.write(sNInvokeMethodName + "(nCacheType, \""+ sMethodKey +"\", "+ sNTypeObject +", _pAryValues);" + crlf);
		}
		fw.write("\t\t}" + crlf + crlf);
		//fw.write(m.toGenericString() + crlf);
	}
	
	private void toCsharpProperty(java.io.FileWriter fw, Class<?> clazz, PropertyMethod prop) throws Throwable{
		String propName = prop.getPropertyName();
		String sNTypeObject = "this";
		String sPropTypeName = prop.getPropertyType(); 
		boolean isGenProp = this.getReturnTypeIsGeneric();
		boolean isArray = sPropTypeName.indexOf("[]") > 0;
		String genTypeToCS = "null";
		if(isGenProp){
			String sTmpTypeof = isArray ? sPropTypeName.substring(0,sPropTypeName.length()-3) : 
										  sPropTypeName.substring(0,sPropTypeName.length()-1);
			genTypeToCS = "typeof("+ sTmpTypeof +")";
		}
		
		fw.write("\t\t");
		if(prop.isStatic()){
			sNTypeObject = "null";
			fw.write("public static "+ sPropTypeName + propName + crlf);
		}
		else if(prop.isFinal())
			fw.write("public "+ sPropTypeName + propName + crlf);
		else{
			if(!prop.isOverride())
				fw.write("public virtual "+ sPropTypeName + propName + crlf);
			else
				fw.write("public override "+ sPropTypeName + propName + crlf);
		}		
		fw.write("\t\t{" + crlf);
		
		if(prop.getGetMethod() != null){
			
			String sMethodKey = this.getMethodKey(prop.getGetMethod(), clazz);
			
			//get
			if(prop.isJMethodAttributeForGet())
				fw.write("\t\t\t" + "[JMethod(\""+ prop.getGetMethod().getName() +"\")]" + crlf);
			fw.write("\t\t\t" + "get" + crlf);
			fw.write("\t\t\t" + "{" + crlf );
			fw.write("\t\t\t\t" + "return JObject.JInvokeMethod<"+ sPropTypeName.substring(0,sPropTypeName.length()-1) +">(nCacheType, \""+ sMethodKey +"\", "+ sNTypeObject +");" + crlf );
			fw.write("\t\t\t" + "}" + crlf);
		}
		
		if(prop.getSetMethod() != null){
			String sMethodKey = this.getMethodKey(prop.getSetMethod(), clazz);
			
			//set
			if(prop.isJMethodAttributeForSet())
				fw.write("\t\t\t" + "[JMethod(\""+ prop.getSetMethod().getName() +"\")]" + crlf);
			fw.write("\t\t\t" + "set" + crlf);
			fw.write("\t\t\t" + "{" + crlf );
			fw.write("\t\t\t\t");
			//if(isArray)
				fw.write("JObject.JInvokeMethod(nCacheType, \""+ sMethodKey +"\", "+ sNTypeObject +", new object[] { value });" +  crlf );			
//			else
//				fw.write("JObject.JInvokeMethod(\""+ propName +"\", _tInstanceType, "+ sNTypeObject +", value);" +  crlf );
			fw.write("\t\t\t" + "}" + crlf);
		}
		
		//end prop
		fw.write("\t\t}" + crlf + crlf);
	}
	
	private void toCsharpFieldProperty(java.io.FileWriter fw, Class<?> clazz, Field f) throws Throwable{
		int iModifier = f.getModifiers();
		if(!Modifier.isPublic(iModifier))return;
		
		boolean isStatic = Modifier.isStatic(iModifier);
		boolean isFinal = Modifier.isFinal(iModifier);
		
		String sNTypeObject = !isStatic ? "this" : "null";
		String sPropTypeName = getReturnType(f.getType(), false);
		String propName = this.getFirstUpper(f.getName());
		
		char c = f.getName().toCharArray()[0];
		boolean isJField = c == Character.toUpperCase(c);
		
		if(isJField)
			fw.write("\t\t" + "[JField(\""+ f.getName() +"\")]" + crlf);
		
		fw.write("\t\t");
		if(isStatic)
			fw.write("public static " + sPropTypeName + propName + crlf);
		else
			fw.write("public " + sPropTypeName + propName + crlf);
		
		fw.write("\t\t{" + crlf);
		
		//getter		
		fw.write("\t\t\t" + "get" + crlf);
		fw.write("\t\t\t" + "{" + crlf );
		fw.write("\t\t\t\t" + "return JObject.JInvokeField<"+ sPropTypeName.substring(0,sPropTypeName.length()-1) +">(nCacheType, \""+ f.getName()  +"\", "+ sNTypeObject +");" + crlf );
		fw.write("\t\t\t" + "}" + crlf);
		
		if(!isFinal){
			//setter
			fw.write("\t\t\t" + "set" + crlf);
			fw.write("\t\t\t" + "{" + crlf );
			fw.write("\t\t\t\t");
			fw.write("JObject.JInvokeField(nCacheType, \""+ f.getName()  +"\", "+ sNTypeObject +", value);" +  crlf );
			fw.write("\t\t\t" + "}" + crlf);
		}
		
		fw.write("\t\t}" + crlf + crlf);
	}
	
	private boolean isGenericMethod(Method m){
		String genSign = m.toGenericString();
		return genSign.indexOf("<") > -1 && genSign.indexOf(">") > -1;
	}
	
	private String getGenericDefineNames(Method m){
		String genSign = m.toGenericString();
		//System.out.println(genSign);
		int iPos = genSign.indexOf("<");
		genSign = genSign.substring(iPos+1);
		iPos = genSign.indexOf(">");
		return genSign.substring(0, iPos);
	}
	
	@SuppressWarnings("unused")
	private String[] getGenericNames(Method m){
		String genSign = getGenericDefineNames(m);
		genSign = genSign.replaceAll(" ", "");
		return genSign.indexOf(",") > -1 ? genSign.split(",") : new String[]{ genSign };
	}
	
	/**
	 * 判断是否为重写方法
	 * @param m 方法
	 * @return true为重写,反之亦然
	 * @throws Throwable
	 */
	private boolean isOverride(Method m) {
//		if(m.getName().compareTo("doString")==0){
//			System.out.println(m.toGenericString());
//		}
		
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
		
//		Annotation[] annotations = m.getDeclaredAnnotations();  
//        for (Annotation a : annotations) {  
//            System.out.println(a);  
//        }  
//        
//		boolean bb = m.isAnnotationPresent(Override.class);
//		if(bb){
//			java.lang.Override ov = m.getAnnotation(java.lang.Override.class);
//			return ov != null;
//		}
//		
//		return bb;
	}
	
	/**
	 * 是否需要注解  JMehotdAttribute 到 CSharp 的方法上.
	 * CSharp 默认使用首字目小写的方法,如果JAVA方法名首字母大写,则需要注解.
	 * @param m 方法
	 * @return true,需要注解
	 */
	private boolean isJMethodAttribute(Method m){
		char c = m.getName().toCharArray()[0];
		return c == Character.toUpperCase(c);
	}
	
	/**
	 * 获取方法KEY,作为csharp调用的缓存关键字
	 * @param m 方法
	 * @param clazz 方法所在的类
	 * @return 缓存关键字
	 */
	private String getMethodKey(Method m, Class<?> clazz){
		String genSign = m.toGenericString();
		int pos = genSign.lastIndexOf(")");
		genSign = genSign.substring(0, pos+1);
		pos = genSign.indexOf(clazz.getName());
		genSign = genSign.substring(pos);
		genSign = genSign.replaceAll(clazz.getName() + "." , "")
					.replaceAll("java.lang.", "");
		return genSign;
	}
	
	private String getConstructorType(Constructor<?> c, boolean isGeneric){
		if(!isGeneric){
			Class<?>[] classes = c.getParameterTypes();
			return this.getParamDefineByTypes(classes, null, isGeneric, false);
		}
		
		Type[] types = c.getGenericParameterTypes();
		return this.getParamDefineByTypes(null, types, isGeneric, false);
	}
		
	private String getParamType(Method m, boolean isGeneric){
		if(!isGeneric){
			Class<?>[] classes = m.getParameterTypes();
			return this.getParamDefineByTypes(classes, null, isGeneric, m.isVarArgs());
		}
		
		Type[] types = m.getGenericParameterTypes();
		return this.getParamDefineByTypes(null, types, isGeneric, m.isVarArgs());
	}
	
	
	private String getParamDefineByTypes(Class<?>[] classes, Type[] types, boolean isGeneric, boolean isArgs){
		String sVal = "";
		int iLen = 0;
		if(!isGeneric){
			iLen = classes.length;
			for(int i=0; i< iLen; i++){		
				Class<?> clazz = classes[i];
				String sParDefine = 
						(i == iLen-1 ? 
								(isArgs ? "params " : "") 
						: "")
						 + this.getTypeName(clazz) + "arg" + i;
				sVal += sVal.isEmpty() ? sParDefine : "," + sParDefine;

			}			
			return sVal;
		}		

		iLen = types.length;
		for(int i=0; i< iLen; i++){
			Type t = types[i];
			String fullName = t.toString();
		    if (fullName.startsWith("class "))
		    	fullName = fullName.substring("class ".length());		    
		    
		    String sParDefine = "";
		    try {
				Class<?> clazz = Class.forName(fullName, false, clazzLoader);
				sParDefine = this.getTypeName(clazz) + "arg" + i;							
			} catch (ClassNotFoundException e) {
				//异常,T无法获取类型,证明是 泛型参数
				sParDefine = fullName + " arg" + i;								
			}	
		    sParDefine = (i == iLen-1 ? 
							(isArgs ? "params " : "") 
						 : "")
						 + sParDefine;		    
		    sVal += sVal.isEmpty() ? sParDefine : "," + sParDefine;	
		}		
		return sVal;
	}
	
	private String getParamInvoke(String paramTypeNames){
		if(paramTypeNames.isEmpty())return "";
		int argLen = paramTypeNames.indexOf(",") >-1 ? paramTypeNames.split(",").length : 1;
		
		String sInk = "";
		for(int i=0;i<argLen;i++){
			sInk += sInk.isEmpty() ? "arg" + i : ", arg" + i; 
		}		
		return "var _pAryValues = new object[] {"+ sInk +"};";
	}
	
	boolean _getReturnTypeIsGeneric;
	private boolean getReturnTypeIsGeneric(){
		return _getReturnTypeIsGeneric;
	}
	
	private String getReturnType(Type type, boolean isGeneric){
		_getReturnTypeIsGeneric = false;
		if(!isGeneric){
			//return getTypeName(m.getReturnType());
			
			try{
				return getTypeName((Class<?>)type);
			}
			catch(Exception ex){
				//整个类是泛型类.
			}
		}

		//Type type = m.getGenericReturnType();
		String fullName = type.toString();
	    if (fullName.startsWith("class "))
	    	fullName = fullName.substring("class ".length());
	    
	    //System.out.println(fullName +" "+m.getName());
	    if(fullName.compareToIgnoreCase("void")==0)
	    	return "void ";
	    
	    try {
			Class<?> clazz = Class.forName(fullName, false, clazzLoader);
			return getTypeName(clazz);
		} catch (ClassNotFoundException e) {
			//异常,T无法获取类型,证明是 泛型参数
			
			_getReturnTypeIsGeneric = true;
		}
	    
	    return fullName + " ";
	}
	
	private String getTypeName(Class<?> clazz){
		if(clazz == void.class)
			return "void ";
		
		//简单类型
		if(clazz == boolean.class)
			return "bool ";
		else if(clazz == byte.class)
			return "byte ";
		else if(clazz == char.class)
			return "char ";
		else if(clazz == short.class)
			return "short "; //Int16
		else if(clazz == int.class)
			return "int "; //Int32
		else if(clazz == long.class)
			return "long "; //Int64
		else if(clazz == float.class)
			return "float "; 
		else if(clazz == double.class)
			return "double "; 
		
		//简单类型对应引用类型
		else if(clazz == Boolean.class)
			return "bool? ";
		else if(clazz == Byte.class)
			return "byte? ";
		else if(clazz == Character.class)
			return "char? ";
		else if(clazz == Short.class)
			return "short? "; //Int16
		else if(clazz == Integer.class)
			return "int? "; //Int32
		else if(clazz == Long.class)
			return "long? "; //Int64
		else if(clazz == Float.class)
			return "float? "; 
		else if(clazz == Double.class)
			return "double? "; 
		
		//简单类型 数组
		else if(clazz == boolean[].class)
			return "bool[] ";
		else if(clazz == byte[].class)
			return "byte[] ";
		else if(clazz == char[].class)
			return "char[] ";
		else if(clazz == short[].class)
			return "short[] "; //Int16
		else if(clazz == int[].class)
			return "int[] "; //Int32
		else if(clazz == long[].class)
			return "long[] "; //Int64
		else if(clazz == float[].class)
			return "float[] "; 
		else if(clazz == double[].class)
			return "double[] "; 		
		
		//简单类型数组对应引用类型数组
		else if(clazz == Boolean[].class)
			return "bool?[] ";
		else if(clazz == Byte[].class)
			return "byte?[] ";
		else if(clazz == Character[].class)
			return "char?[] ";
		else if(clazz == Short[].class)
			return "short?[] "; //Int16
		else if(clazz == Integer[].class)
			return "int?[] "; //Int32
		else if(clazz == Long[].class)
			return "long?[] "; //Int64
		else if(clazz == Float[].class)
			return "float?[] "; 
		else if(clazz == Double[].class)
			return "double?[] "; 
		
		//string, string[]
		else if(clazz == String.class)
			return "string "; 
		else if(clazz == String[].class)
			return "string[] "; 
		

		if(!clazz.isArray())
			return this.getFirstUpper(clazz.getName()) + " ";
		else{
			String cAryName = clazz.getName().replace("[L", "").replace(";", "");
			return this.getFirstUpper(cAryName) + "[] ";
		}

		
//		System.out.println(clazz.getName());
//		return "";
	}

	class PropertyMethod{
		Method getMethod;
		Method setMethod;
		public void setGetMethod(Method m){
			getMethod = m;
		}
		
		public Method getGetMethod(){
			return getMethod;
		}
		
		public void setSetMethod(Method m){
			setMethod = m;
		}
		
		public Method getSetMethod(){
			return setMethod;
		}		
		
		String sPropName;
		public String getPropertyName(){
			if(sPropName == null){
				if(getMethod != null)
					sPropName = this.getMethod.getName().substring(3);
				else if(setMethod!=null)
					sPropName = this.setMethod.getName().substring(3);
				else
					sPropName = "";
				
				sPropName = getFirstUpper(sPropName);
			}
			return sPropName;
		}
		
		public String getPropertyType(){
			if(this.getMethod != null)
				return getReturnType(this.getMethod.getGenericReturnType(), isGenericMethod(this.getMethod));
			else if(this.setMethod != null)
				return getReturnType(this.setMethod.getGenericParameterTypes()[0], isGenericMethod(this.setMethod));			
			return "";
		}
		
		public boolean isStatic(){
			if(this.getMethod != null)
				return Modifier.isStatic(this.getMethod.getModifiers());
			else if(this.setMethod != null)
				return Modifier.isStatic(this.setMethod.getModifiers());			
			return false;
		}
		
		public boolean isFinal(){
			if(this.getMethod != null)
				return Modifier.isFinal(this.getMethod.getModifiers());
			else if(this.setMethod != null)
				return Modifier.isFinal(this.setMethod.getModifiers());			
			return false;
		}
		
		public boolean isOverride() throws Throwable{
			boolean bb = false;
			if(this.getMethod != null)
				bb = JarFinder.this.isOverride(this.getMethod);
			if(bb== false && this.setMethod != null)
				bb = JarFinder.this.isOverride(this.setMethod);
			
			return bb;
		}
		
		public boolean isJMethodAttributeForGet(){
			if(this.getMethod == null)return false;
			return isJMethodAttribute(this.getMethod);
		}
		
		public boolean isJMethodAttributeForSet(){
			if(this.setMethod == null)return false;
			return isJMethodAttribute(this.setMethod);
		}
	}
}
