package jxdo.rjava;

import java.io.FileWriter;
import java.io.IOException;
import java.lang.reflect.Constructor;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.lang.reflect.Modifier;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

class JCSharpOuter {
	FileWriter fw;
	Class<?> clazz;
	static String crlf;
	boolean isNested;
	public JCSharpOuter(Class<?> clazz, FileWriter fw, boolean isNested){
		if(crlf == null)crlf= System.getProperty("line.separator");
		this.clazz = clazz;
		this.fw = fw;	
		this.isNested = isNested;
	}
	
	String tabs;
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
					" * Anthor:		jjsiki(ZhuQi) javasuki@hotmail.com" + crlf +
					" * Blog:		http://blog.csdn.net/javasuki" + crlf +
		            " * Generator:	RJacoste(jxdo.rjava.JCSharp) For VS2012" + crlf +
					" * Version:		1.0.0.0" + crlf +
					" * -----------------------------------------------------" + crlf +
					" */" + crlf + 
					"#endregion" + crlf + crlf;
		}
		
		return codeCopyRightCS;
	}
	
	public void writerNamespaceStart(String tabs){
		this.tabs = tabs;
		
		if(isNested)return;
			
		try {
			fw.write("using System;" + crlf);
			fw.write("using System.Collections.Generic;" + crlf + crlf);
			fw.write("using NXDO.RJava;" + crlf);
			fw.write("using NXDO.RJava.Core;" + crlf);
			fw.write("using NXDO.RJava.Reflection;" + crlf);
			fw.write("using NXDO.RJava.Attributes;" + crlf + crlf);
			
			fw.write(getCodeCopyRightCS().replace("_fName", clazz.getName().replaceAll("\\$", ".")+ ".cs"));
			
			String sNameSpace = JCSharpHelper.getNameSpace(clazz);
			if(sNameSpace.length() > 0)
				sNameSpace = "." + sNameSpace;
			
			fw.write("namespace Java" + sNameSpace + crlf);		
			fw.write("{" + crlf);
			
		} catch (Throwable e) {
		}					
	}
		
	public void writerNamespaceEnd(){
		if(isNested)return;
		
		try {
			fw.write("}" + crlf);
			fw.flush();
			fw.close();
		} catch (IOException e) {
		}
	}
		
	public void writerDefineStart() {
		try {
			if(clazz.isInterface())
				this.toInterfaceDefine();
			else if(clazz.isEnum())
				this.toEnumDefine();
			else{
				if(isNested){
					fw.write(crlf + crlf);
					fw.write(tabs + "#region 内嵌类 " + JCSharpHelper.getDefineName(clazz) + crlf);
				}
				
				this.toClassDefine();
			}
		} catch (Throwable e) {
		}
	}
	
	public void writerDefineEnd(){
		try {
			if(isNested)
				fw.write(crlf);
			
			fw.write(tabs + "}" + crlf);
			
			if(isNested)
				fw.write(tabs + "#endregion" + crlf);			
		} catch (Throwable e) {
		}
	}
	
	private void toInterfaceDefine() throws Throwable{
		if(isNested)
			fw.write(crlf);
		
		String jrootNames = JCSharpHelper.getClassParents(clazz);
		String sGenSign = JCSharpHelper.getClassGenericDefine(clazz);
		jrootNames = jrootNames.length() > 0 ? " : " + jrootNames : ""; //Class肯定有 JObject,接口没有
		
		fw.write(tabs + "[JInterface(\"" + clazz.getName() + "\")]" + crlf);
		fw.write(tabs + "public interface " + JCSharpHelper.getDefineName(clazz) + sGenSign + jrootNames + crlf);
		fw.write(tabs + "{" + crlf);
		
		lstPropMethods = new ArrayList<Method>();
		List<JCSharpProperty> lstProps = JCSharpHelper.getPropertys(clazz);	
		for(JCSharpProperty p : lstProps){
			if(p.get != null)lstPropMethods.add(p.get);
			if(p.set != null)lstPropMethods.add(p.set);
			
			fw.write(tabs + "\t" + p.getPropertyType() + " " +  p.getPropertyName()+ crlf);
			fw.write(tabs + "\t" + "{" + crlf);
			if(p.get != null)
				fw.write(tabs + "\t\t" + "get;" + crlf);
			if(p.set != null)
				fw.write(tabs + "\t\t" + "set;" + crlf);
			fw.write(tabs + "\t" + "}" + crlf);
		}
		
		List<Method> methods = JCSharpHelper.getMethods(clazz, lstPropMethods);
		for(Method m : methods){
			if(JCSharpHelper.getIsStatic(m))continue;
			String paramsName = JCSharpHelper.getParamsName(m);
			String returnName = JCSharpHelper.getReturnName(m);
			String nMethodName = JCSharpHelper.getFirstUpperSplit(m.getName());
			fw.write(tabs + "\t" + returnName + " " +  nMethodName + "("+ paramsName +");" + crlf);
		}
	}
	
	private void toEnumDefine() throws Throwable{
		fw.write(tabs + "[JEnum(\"" + clazz.getName() + "\")]" + crlf);
		fw.write(tabs + "public enum " + clazz.getSimpleName() + crlf);
		fw.write(tabs + "{" + crlf);
		
		List<Field> flds = JCSharpHelper.getFields(clazz);
		int idx = 0;
		for(Field f : flds){
			String split = (idx == flds.size()-1) ? "" : ",";
			fw.write(tabs + "\t" + f.getName() + split + crlf);
			idx += 1;
		}
	}

	private void toClassDefine() throws Throwable{

		String jrootNames = JCSharpHelper.getClassParents(clazz);
		String sGenSign = JCSharpHelper.getClassGenericDefine(clazz);
		String cAccessFlag = JCSharpHelper.getClassAccessFlag(clazz);
		
		fw.write(tabs + "[JClass(\"" + clazz.getName() + "\")]" + crlf);
		fw.write(tabs + cAccessFlag + "class " + JCSharpHelper.getDefineName(clazz) + sGenSign + " : " + jrootNames + crlf);
		fw.write(tabs + "{" + crlf);
		
		lstPropMethods = new ArrayList<Method>();
		boolean hasCtor = this.toConstructor();		
		boolean hasProp = this.toPropertyMethod(hasCtor);
		boolean hasCtorOrMethod = this.toMethod(hasProp || hasCtor);		
		this.toPropertyField(hasCtorOrMethod);
		
	}
	
	private boolean toConstructor() throws Throwable{
		fw.write(tabs + "\t" + "#region "+ JCSharpHelper.getDefineName(clazz) +".Class" + crlf);	
		//静态属性  Class
		fw.write(tabs + "\t" + "/// <summary>" + crlf);
		fw.write(tabs + "\t" + "/// 获取 java 的类型，与 java 代码 "+ clazz.getName().replaceAll("\\$", ".") +".class 等价。" + crlf);
		fw.write(tabs + "\t" + "/// </summary>" + crlf);
		fw.write(tabs + "\t" + "public static new JClass Class" + crlf);
		fw.write(tabs + "\t" + "{" + crlf);
		fw.write(tabs + "\t\t" + "get" + crlf);
		fw.write(tabs + "\t\t" + "{" + crlf);
		fw.write(tabs + "\t\t\t" + "return JClass.ForName(\"" + clazz.getName() + "\");" + crlf);
		fw.write(tabs + "\t\t" + "}" + crlf);
		fw.write(tabs + "\t" + "}" + crlf );
		fw.write(tabs + "\t" + "#endregion" + crlf + crlf);
		
		boolean isSealed = Modifier.isFinal(clazz.getModifiers());
		String ctorFlag = isSealed ? "" : "protected ";
		
		fw.write(tabs + "\t" + "#region 构造函数" + crlf);		
		//必须实现的构造函数,由 mc++ 反射调用
		fw.write(tabs + "\t" + "/// <summary>" + crlf);
		fw.write(tabs + "\t" + "/// 请勿在用户代码中直接使用，本构造函数由 NXDO.RJava.Core 内部调用。" + crlf);
		fw.write(tabs + "\t" + "/// </summary>" + crlf);
		fw.write(tabs + "\t" + ctorFlag + "" + JCSharpHelper.getDefineName(clazz) + "(IntPtr objectPtr, IntPtr classPtr)"+ crlf);
		fw.write(tabs + "\t\t" +" : base(objectPtr, classPtr)"+ crlf);
		fw.write(tabs + "\t" + "{"+ crlf);
		fw.write(tabs + "\t" + "}"+ crlf + crlf); 
		
		//ctor
		boolean hasCtor = false;
		List<Constructor<?>> constructors = JCSharpHelper.getConstructors(clazz);
		int idx = 0;
		for(Constructor<?> c : constructors){
			try{
				String paramsName = JCSharpHelper.getParamsName(c);
				String nParamSet = JCSharpHelper.getParamsSet(c);
				nParamSet = nParamSet.length() == 0 ? "" : "new object[] { "+ nParamSet +" }";
				
				fw.write(tabs + "\t" + "public " + JCSharpHelper.getDefineName(clazz) + "("+ paramsName +")" + crlf);
				fw.write(tabs + "\t" + "{"+ crlf);
				fw.write(tabs + "\t\t" + "JSuper("+ nParamSet +");"+ crlf);
				fw.write(tabs + "\t" + "}" + crlf);
			}
			catch(Exception ex){
				fw.write(tabs + "\t" +"//java 构造函数:" + c.toGenericString() + crlf);
				fw.write(tabs + "\t" +"//java 异常:" + ex.getMessage() + crlf);
			}
			
			idx++;
			if(idx < constructors.size())
				fw.write(crlf);
			hasCtor = true;
		}
		fw.write(tabs + "\t" + "#endregion" + crlf);
		
		return hasCtor;
	}
	
	
	private boolean toMethod(boolean hasProp) throws Throwable{
		String sTypeof = JCSharpHelper.getClassForTypeof(clazz);
		
		boolean hasMethod = false;
		List<Method> methods = JCSharpHelper.getMethods(clazz, lstPropMethods);
		if(methods.size() > 0 && hasProp)fw.write(crlf);
		if(methods.size() > 0)fw.write(tabs + "\t" + "#region java 方法的映射" + crlf);	
		int idx = 0;
		for(Method m : methods){

			try{
				String paramsName = JCSharpHelper.getParamsName(m);
				String returnName = JCSharpHelper.getReturnName(m);
				boolean isReturn = JCSharpHelper.getIsReturn(m);
				boolean isStatic = JCSharpHelper.getIsStatic(m);
				boolean isAbstract = JCSharpHelper.getIsAbstract(m);
				String nJMethodAttr = JCSharpHelper.getIsJMethodAttribute(m) ? tabs + "\t" + "[JMethod(\""+ m.getName() +"\")]" + crlf : "";
				String mAccessFlag = JCSharpHelper.getMethodAccessFlag(m);
				String nInvokeName = isReturn ? "return JObject.JInvokeMethod<" + returnName + ">" : "JObject.JInvokeMethod";
				String nCacheType = isStatic ? "typeof("+ sTypeof +")" : "this.GetType()";
				String nCacheKey = "\"" + JCSharpHelper.getMethodCacheKey(m) + "\"";
				String nCacheObj = isStatic ? "null" : "this";
				String nParamSet = JCSharpHelper.getParamsSet(m);
				nParamSet = nParamSet.length() == 0 ? "" : ", new object[] { "+ nParamSet +" }";
				
				String nMethodName = m.getName() == "main" ? "JMain" : JCSharpHelper.getFirstUpperSplit(m.getName());
				nJMethodAttr = m.getName() == "main" ? tabs + "\t" + "[JMethod(\"main\")]" + crlf : nJMethodAttr;
				String sGenParamMethod = JCSharpHelper.getMethodGenericName(m);
				nMethodName += sGenParamMethod;
				
				fw.write(nJMethodAttr);
				fw.write(tabs + "\t" + mAccessFlag + returnName + " " +  nMethodName + "("+ paramsName +")");
				if(isAbstract)
					fw.write(";"+ crlf);
				else{
					fw.write(crlf);			
					fw.write(tabs + "\t" + "{" + crlf);
					fw.write(tabs + "\t\t" + nInvokeName + "("+ nCacheType +", "+ nCacheKey +", "+ nCacheObj + nParamSet +");" + crlf);
					fw.write(tabs + "\t" + "}" + crlf);
				}
			}
			catch(Exception ex){
				fw.write(tabs + "\t" +"//java 方法:" + m.getName() + crlf);
				fw.write(tabs + "\t" +"//java 签名:" + m.toGenericString() + crlf);
				fw.write(tabs + "\t" +"//java 异常:" + ex.getMessage() + crlf);
			}
			
			idx++;
			if(idx < methods.size())
				fw.write(crlf);
			
			hasMethod = true;
		}
		if(methods.size() > 0)fw.write(tabs + "\t" + "#endregion" + crlf);	
		
		return hasProp || hasMethod;
	}
	
	List<Method> lstPropMethods;
	private boolean toPropertyMethod(boolean hasCtor) throws Throwable{
		
		String sTypeof = JCSharpHelper.getClassForTypeof(clazz);
		
//		if(clazz.getName() == "rt.DemoE")
//			System.out.println();
			
		List<JCSharpProperty> lstProps = JCSharpHelper.getPropertys(clazz);	
		if(lstProps.size() > 0 && hasCtor)fw.write(crlf);
		if(lstProps.size() > 0)fw.write(tabs + "\t" + "#region java get/set方法的属性映射" + crlf);	
		
		int idx = 0;
		for(JCSharpProperty p : lstProps){
			try{
				String pTypeName = p.getPropertyType();
				String pPropName = p.getPropertyName();
				if(pTypeName == pPropName)continue; //属性与类型相同,则不生成属性
				
				if(p.get != null)lstPropMethods.add(p.get);
				if(p.set != null)lstPropMethods.add(p.set);
				
				boolean isStatic = p.getIsStatic();
				String nCacheType = isStatic ? "typeof("+ sTypeof +")" : "this.GetType()";
				String nCacheObj = isStatic ? "null" : "this";
				
				fw.write(tabs + "\t" + p.getPropertyAccessFlag() + pTypeName + " " +  pPropName + crlf);
				fw.write(tabs + "\t" + "{" + crlf);
				if(p.get != null){
					String nInvokeName = "return JObject.JInvokeMethod<" + p.getPropertyType() + ">";
					String nCacheKey = "\"" + JCSharpHelper.getMethodCacheKey(p.get) + "\"";
					
					String nJMethodAttr = p.getGetIsJMethodAttribute();
					fw.write(nJMethodAttr.length() > 0 ? tabs + nJMethodAttr + crlf : "");
					fw.write(tabs + "\t\t" + "get" + crlf);
					fw.write(tabs + "\t\t" + "{" + crlf);
					fw.write(tabs + "\t\t\t" + nInvokeName + "("+ nCacheType +", "+ nCacheKey +", "+ nCacheObj +");" + crlf);
					fw.write(tabs + "\t\t" + "}" + crlf);
				}
				if(p.set != null){
					String nInvokeName = "JObject.JInvokeMethod";
					String nCacheKey = "\"" + JCSharpHelper.getMethodCacheKey(p.set) + "\"";
					String nParamSet = "new object[] { value }";
									
					String nJMethodAttr = p.getSetIsJMethodAttribute();
					fw.write(nJMethodAttr.length() > 0 ? tabs  + nJMethodAttr + crlf : "");
					fw.write(tabs + "\t\t" + "set" + crlf);
					fw.write(tabs + "\t\t" + "{" + crlf);
					fw.write(tabs + "\t\t\t" + nInvokeName + "("+ nCacheType +", "+ nCacheKey +", "+ nCacheObj +", "+ nParamSet +");" + crlf);
					fw.write(tabs + "\t\t" + "}" + crlf);
				}
				fw.write(tabs + "\t" + "}" + crlf);
			}
			catch(Exception ex){
				fw.write(tabs + "\t" +"//java 方法:" + p.toString() + crlf);
				fw.write(tabs + "\t" +"//java 异常:" + ex.getMessage() + crlf);
			}
			
			idx++;
			if(idx < lstProps.size())
				fw.write(crlf);
		}
		
		if(lstProps.size() > 0)fw.write(tabs + "\t" + "#endregion" + crlf);
		
		return lstProps.size() > 0;
	}
	
	private void toPropertyField(boolean hasCtorOrMethod) throws Throwable{
			
		String sTypeof = JCSharpHelper.getClassForTypeof(clazz);
		List<Field> flds = JCSharpHelper.getFields(clazz);
		if(flds.size() > 0 && hasCtorOrMethod)fw.write(crlf);
		if(flds.size() > 0)fw.write(tabs + "\t" + "#region java 变量的属性映射" + crlf);	
		
		int idx = 0;
		for(Field f : flds){

			try{
				String returnName = JCSharpHelper.getReturnName(f);
				boolean isStatic = JCSharpHelper.getIsStatic(f);
				String fAccessFlag = JCSharpHelper.getFieldAccessFlag(f);
				String nInvokeName = "JObject.JInvokeField<" + returnName + ">";			
				String nCacheType = isStatic ? "typeof("+ sTypeof +")" : "this.GetType()";
				String nCacheObj = isStatic ? "null" : "this";
				
				fw.write(tabs + "\t" + fAccessFlag + returnName + " " + JCSharpHelper.getFirstUpperSplit(f.getName()) + crlf);
				fw.write(tabs + "\t" + "{" + crlf);
				
				fw.write(tabs + "\t\t" + "get" + crlf);
				fw.write(tabs + "\t\t" + "{" + crlf);
				fw.write(tabs + "\t\t\t" + "return " +  nInvokeName + "("+ nCacheType +", \""+ f.getName() +"\", "+ nCacheObj +");" + crlf);
				fw.write(tabs + "\t\t" + "}" + crlf);
				
				if(!JCSharpHelper.getIsFinal(f)){
					fw.write(tabs + "\t\t" + "set" + crlf);
					fw.write(tabs + "\t\t" + "{" + crlf);
					fw.write(tabs + "\t\t\t" + nInvokeName + "("+ nCacheType +", \""+ f.getName() +"\", "+ nCacheObj +", value);" + crlf);
					fw.write(tabs + "\t\t" + "}" + crlf);
				}
				
				fw.write(tabs + "\t" + "}" + crlf);
			}
			catch(Exception ex){
				fw.write(tabs + "\t" +"//java 字段:" + f.getName() + crlf);
				fw.write(tabs + "\t" +"//java 签名:" + f.toGenericString() + crlf);
				fw.write(tabs + "\t" +"//java 异常:" + ex.getMessage() + crlf);
			}
			
			
			idx++;	
			if(idx < flds.size())
				fw.write(crlf);
		}
		
		if(flds.size() > 0)fw.write(tabs + "\t" + "#endregion" + crlf);
	}
}
