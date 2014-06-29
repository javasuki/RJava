package jxdo.rjava;

import java.lang.reflect.Method;
import java.lang.reflect.Type;

class JCSharpProperty {
	public Method get;
	public Method set;
	
	@Override
	public String toString() {
		String s = "";
		if(get != null)
			s = get.getName();
		
		if( set != null){
			s += s.length() == 0 ? set.getName() : ", " + set.getName();
		}
		
		return s;
	}
	
	public String getPropertyName(){
		String spName = "";
		if(get != null)
			spName = get.getName().substring(3);
		else
			spName = set.getName().substring(3);
		
		return JCSharpHelper.getFirstUpperSplit(spName);
	}
	
	public String getPropertyType(){
		if(get != null){
			return JCSharpHelper.getReturnName(get);
		}
		
		Class<?> defClazz = set.getDeclaringClass();
		Type t = set.getGenericParameterTypes()[0];
		return JCSharpHelper.getTypeName(t, defClazz);
	}
	
	public String getPropertyAccessFlag(){
		if(get != null){
			return JCSharpHelper.getMethodAccessFlag(get);
		}
		
		return JCSharpHelper.getMethodAccessFlag(set);
	}
	
	public boolean getIsStatic(){
		if(get != null)
			return JCSharpHelper.getIsStatic(get);
		return JCSharpHelper.getIsStatic(set);
	}
	
	public String getGetIsJMethodAttribute(){
		if(get == null)return "";
		return JCSharpHelper.getIsJMethodAttribute(get) ? "\t\t" + "[JMethod(\""+ get.getName() +"\")]" : "";
	}
	
	public String getSetIsJMethodAttribute(){
		if(set == null)return "";
		return JCSharpHelper.getIsJMethodAttribute(set) ? "\t\t" + "[JMethod(\""+ set.getName() +"\")]" : "";
	}
}
